using GMLoaded.Attributes;
using GMLoaded.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace GMLoaded
{
    public static class ILExtensions
    {
        /// <summary>
        /// Push Ldc_I4_(index), used for dynamic class building
        /// </summary>
        /// <param name="index">Int32 (-1 - 8), will use Ldc_I4 if not within range</param>
        public static void EmitLDC(this ILGenerator ilgen, Int32 index)
        {
            switch (index)
            {
                case -1: ilgen.Emit(OpCodes.Ldc_I4_M1); break;
                case 0: ilgen.Emit(OpCodes.Ldc_I4_0); break;
                case 1: ilgen.Emit(OpCodes.Ldc_I4_1); break;
                case 2: ilgen.Emit(OpCodes.Ldc_I4_2); break;
                case 3: ilgen.Emit(OpCodes.Ldc_I4_3); break;
                case 4: ilgen.Emit(OpCodes.Ldc_I4_4); break;
                case 5: ilgen.Emit(OpCodes.Ldc_I4_5); break;
                case 6: ilgen.Emit(OpCodes.Ldc_I4_6); break;
                case 7: ilgen.Emit(OpCodes.Ldc_I4_7); break;
                case 8: ilgen.Emit(OpCodes.Ldc_I4_8); break;
                default: ilgen.Emit(OpCodes.Ldc_I4, index); break; // slower and takes more space
            }
        }

        /// <summary>
        /// push Ldarg_(index), used for dynamic class building
        /// </summary>
        /// <param name="index">Byte (0 - 3), will use Ldarg_S if not within range</param>
        public static void EmitLoadArg(this ILGenerator ilgen, Byte index)
        {
            switch (index)
            {
                case 0: ilgen.Emit(OpCodes.Ldarg_0); break;
                case 1: ilgen.Emit(OpCodes.Ldarg_1); break;
                case 2: ilgen.Emit(OpCodes.Ldarg_2); break;
                case 3: ilgen.Emit(OpCodes.Ldarg_3); break;
                default: ilgen.Emit(OpCodes.Ldarg_S, index); break;
            }
        }
    }

    // TODO: Optimize WrapLibrary to Work on Static PreMade Classes & Call Apon Marshals insted of using Generic function GLua.Push<_.type>()

    public partial class GLua
    {
        private static readonly HashSet<String> JITNames = new HashSet<String>();

        private static ModuleBuilder moduleBuilder;

        private readonly Dictionary<Tuple<Type, String>, Type> Libraries = new Dictionary<Tuple<Type, String>, Type>();

        private static void InitJIT()
        {
            if (moduleBuilder == null)
            {
                // NOTE: Edited for NetSTD2.2+
                AssemblyBuilder assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("JIT"), AssemblyBuilderAccess.RunAndCollect);
                moduleBuilder = assemblyBuilder.DefineDynamicModule("LuaLibraryJIT");
            }
        }

        /// <summary>
        /// Wraps a lua table using the specified class as a template.
        /// warning is is slower then just writting your own wrapper as it will fully use the compilers optimizer.
        /// </summary>
        /// <typeparam name="TClass">Template</typeparam>
        /// <param name="globalName">GlobalTable Name if Template does not contain <see cref="LuaLibraryLocationAttribute"/></param>
        /// <returns>a Fully Wrapped Class</returns>
        public TClass WrapLibrary<TClass>(String globalName = null)
        {
            Type targetInterface = typeof(TClass);

            LuaLibraryLocationAttribute locationattr = targetInterface.GetCustomAttribute(typeof(LuaLibraryLocationAttribute)) as LuaLibraryLocationAttribute;
            if (globalName == null && locationattr == null)
                throw new Exception("Define either LuaLibraryLocationAttribute or globalName");
            if (locationattr != null) globalName = locationattr.Path;
            Type implClass;

            Type existing = this.Libraries.Where(kv => kv.Key.Item1 == targetInterface && kv.Key.Item2 == globalName).Select(kv => kv.Value).FirstOrDefault();
            if (existing != null)
                implClass = existing;
            else
            {
                // Create Interface name within Dynamic Assembly
                String interfaceName = targetInterface.Name + "_" + (IntPtr.Size * 8).ToString();

                // Find Available Name
                while (JITNames.Contains(interfaceName))
                    interfaceName += "X"; // im so super lazy fuck you
                JITNames.Add(interfaceName);

                // Create the Interface within the Dynamic Assembly
                TypeBuilder builder = moduleBuilder.DefineType(interfaceName, TypeAttributes.Class, null, new Type[] { targetInterface });

                // define our Interface to be a derivitive of _template_ 'local::Generic<TClass>'
                builder.AddInterfaceImplementation(targetInterface);

                // Define a Field called GLua of type GLua this will act as the (this) field
                FieldBuilder GLuaField = builder.DefineField("GLua", typeof(GLua), FieldAttributes.Public);

                // Build JIT Info about the newly made Interface Class
                ClassLuaJITInfo classInfo = new ClassLuaJITInfo(targetInterface);

                // Get GLua.* for building the functions of the new Interface class
                MethodInfo getFieldMethodInfo = typeof(GLua).GetMethod(nameof(GLua.GetField));
                MethodInfo pushMethodInfo = typeof(GLua).GetMethods().Where(m => m.Name == nameof(GLua.Push) && m.ContainsGenericParameters).FirstOrDefault();
                MethodInfo getMethodInfo = typeof(GLua).GetMethods().Where(m => m.Name == nameof(GLua.Get) && m.ContainsGenericParameters).FirstOrDefault();
                MethodInfo pcallMethodInfo = typeof(GLua).GetMethod(nameof(GLua.PCall));

                // Fill in the Interface class based on functions within _template_
                foreach (MethodLuaJITInfo method in classInfo.Methods)
                {
                    // Define Dynamic method
                    MethodBuilder mbuilder = builder.DefineMethod(method.Name, MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Virtual, CallingConventions.HasThis);

                    // Define the return type and parameters to match that of _template_.(dyn)FunctionName
                    mbuilder.SetReturnType(method.ReturnType);
                    mbuilder.SetParameters(method.Args.ToArray());

                    // Define method as override of _template_.(dyn)FunctionName
                    builder.DefineMethodOverride(mbuilder, method.MethodInfo);

                    // Create IL Stream to fill function body
                    ILGenerator ilgen = mbuilder.GetILGenerator();

                    // Luabase.GetField(LUA_GLOBALINDEX, _input_Lua_LibraryName);
                    //ilgen.Emit(OpCodes.Nop);
                    ilgen.Emit(OpCodes.Ldarg_0);
                    ilgen.Emit(OpCodes.Ldfld, GLuaField);   // push GLua(this)
                    ilgen.EmitLDC(LUA_GLOBALSINDEX);        // push GLua.LUA_GLOBALINDEX into call stack
                    ilgen.Emit(OpCodes.Ldstr, globalName);  // push _input_Lua_LibraryName into call stack
                    ilgen.EmitCall(OpCodes.Callvirt, getFieldMethodInfo, null); // Call GLua.GetField

                    // Luabase.GetField(-1, _template_.(dyn)FunctionName) 'local::method.Name');
                    //ilgen.Emit(OpCodes.Nop);
                    ilgen.Emit(OpCodes.Ldarg_0);
                    ilgen.Emit(OpCodes.Ldfld, GLuaField);   // push GLua(this)
                    ilgen.Emit(OpCodes.Ldc_I4_M1);          // push -1
                    ilgen.Emit(OpCodes.Ldstr, method.Name); // push _template_.(dyn)FunctionName  'local::method.Name'
                    ilgen.EmitCall(OpCodes.Callvirt, getFieldMethodInfo, null); // Call GLua.GetField

                    // Load Args int Lua stack
                    // for _template_.(dyn)FunctionName.nArgs 'local::method.Args.Count'
                    //  do Luabase.Push()
                    for (Int32 i = 0; i < method.Args.Count; i++)
                    {
                        Type param = method.Args[i]; // _template_.(dyn)FunctionName.(dyn)param 'local::param'
                        //ilgen.Emit(OpCodes.Nop);
                        ilgen.Emit(OpCodes.Ldarg_0);
                        ilgen.Emit(OpCodes.Ldfld, GLuaField);   // push GLua(this)
                        ilgen.EmitLoadArg((Byte)(i + 1));               // Set Arg Position for call
                        ilgen.EmitCall(OpCodes.Callvirt, pushMethodInfo.MakeGenericMethod(param), null); // Call GLua.Push<_template_.(dyn)FunctionName.(dyn)Args.Type>(*implicit:-1)
                    }

                    //ilgen.Emit(OpCodes.Nop);
                    ilgen.Emit(OpCodes.Ldarg_0);
                    ilgen.Emit(OpCodes.Ldfld, GLuaField);   // push GLua(this)
                    ilgen.EmitLDC(method.Args.Count);   // push _template_.(dyn)FunctionName.nArgs 'local::method.Args.Count'

                    if (method.ReturnType != typeof(void)) // Determin Results
                        ilgen.Emit(OpCodes.Ldc_I4_1);   // push 1
                    else
                        ilgen.Emit(OpCodes.Ldc_I4_0);   // push 0

                    ilgen.Emit(OpCodes.Ldc_I4_0);       // push 0 (IStackPos_ErrorFunction)
                    ilgen.EmitCall(OpCodes.Callvirt, pcallMethodInfo, null); // Call GLua.PCall
                    ilgen.Emit(OpCodes.Pop);            // pop GLua.PCall(...).return

                    // if return needed
                    if (method.ReturnType != typeof(void))
                    {
                        ilgen.Emit(OpCodes.Ldarg_0);
                        ilgen.Emit(OpCodes.Ldfld, GLuaField); // push GLua(this)
                        ilgen.EmitCall(OpCodes.Callvirt, getMethodInfo.MakeGenericMethod(method.ReturnType), null); // Call GLua.Get<_template_.(dyn)FunctionName.ReturnType>(*implicit:-1)
                    }
                    ilgen.Emit(OpCodes.Ret); // return value if value on stack otherwise return NULL 'void'
                }

                // Build Dynamic Type of interface _template_
                implClass = builder.CreateType();

                // Add Interface to Library of Interfaces
                this.Libraries.Add(new Tuple<Type, String>(targetInterface, globalName), implClass);
            }

            // Create Instance of Interface
            Object instClass = Activator.CreateInstance(implClass);
            // Set GLua to 'this'
            FieldInfo GLuaInstanceField = implClass.GetField("GLua", BindingFlags.Public | BindingFlags.Instance);
            GLuaInstanceField.SetValue(instClass, this);

            // Return Interface as _template_
            return (TClass)instClass;
        }
    }
}
