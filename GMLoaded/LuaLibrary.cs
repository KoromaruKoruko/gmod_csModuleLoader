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
                default: ilgen.Emit(OpCodes.Ldc_I4, index); break;
            }
        }

        public static void EmitLoadArg(this ILGenerator ilgen, Int32 index)
        {
            switch (index)
            {
                case 0: ilgen.Emit(OpCodes.Ldarg_0); break;
                case 1: ilgen.Emit(OpCodes.Ldarg_1); break;
                case 2: ilgen.Emit(OpCodes.Ldarg_2); break;
                case 3: ilgen.Emit(OpCodes.Ldarg_3); break;
                default: ilgen.Emit(OpCodes.Ldarg_S, (Byte)index); break;
            }
        }
    }

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
                String interfaceName = targetInterface.Name + "_" + (IntPtr.Size * 8).ToString();

                while (JITNames.Contains(interfaceName))
                    interfaceName += "X"; // im so super lazy fuck you
                JITNames.Add(interfaceName);

                TypeBuilder builder = moduleBuilder.DefineType(interfaceName, TypeAttributes.Class, null, new Type[] { targetInterface });
                builder.AddInterfaceImplementation(targetInterface);

                FieldBuilder GLuaField = builder.DefineField("GLua", typeof(GLua), FieldAttributes.Public);

                ClassLuaJITInfo classInfo = new ClassLuaJITInfo(targetInterface);

                MethodInfo getFieldMethodInfo = typeof(GLua).GetMethod(nameof(GLua.GetField));
                MethodInfo pushMethodInfo = typeof(GLua).GetMethods().Where(m => m.Name == nameof(GLua.Push) && m.ContainsGenericParameters).FirstOrDefault();
                MethodInfo getMethodInfo = typeof(GLua).GetMethods().Where(m => m.Name == nameof(GLua.Get) && m.ContainsGenericParameters).FirstOrDefault();
                MethodInfo pcallMethodInfo = typeof(GLua).GetMethod(nameof(GLua.PCall));

                foreach (MethodLuaJITInfo method in classInfo.Methods)
                {
                    MethodBuilder mbuilder = builder.DefineMethod(method.Name, MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Virtual, CallingConventions.HasThis);

                    mbuilder.SetReturnType(method.ReturnType);
                    mbuilder.SetParameters(method.Args.ToArray());
                    builder.DefineMethodOverride(mbuilder, method.MethodInfo);
                    ILGenerator ilgen = mbuilder.GetILGenerator();

                    ilgen.Emit(OpCodes.Nop);
                    ilgen.Emit(OpCodes.Ldarg_0);
                    ilgen.Emit(OpCodes.Ldfld, GLuaField);
                    ilgen.EmitLDC(LUA_GLOBALSINDEX);
                    ilgen.Emit(OpCodes.Ldstr, globalName);
                    ilgen.EmitCall(OpCodes.Callvirt, getFieldMethodInfo, null);

                    ilgen.Emit(OpCodes.Nop);
                    ilgen.Emit(OpCodes.Ldarg_0);
                    ilgen.Emit(OpCodes.Ldfld, GLuaField);
                    ilgen.Emit(OpCodes.Ldc_I4_M1);
                    ilgen.Emit(OpCodes.Ldstr, method.Name);
                    ilgen.EmitCall(OpCodes.Callvirt, getFieldMethodInfo, null);

                    for (Int32 i = 0; i < method.Args.Count; i++)
                    {
                        Type param = method.Args[i];
                        ilgen.Emit(OpCodes.Nop);
                        ilgen.Emit(OpCodes.Ldarg_0);
                        ilgen.Emit(OpCodes.Ldfld, GLuaField);
                        ilgen.EmitLoadArg(i + 1);
                        ilgen.EmitCall(OpCodes.Callvirt, pushMethodInfo.MakeGenericMethod(param), null);
                    }

                    ilgen.Emit(OpCodes.Nop);
                    ilgen.Emit(OpCodes.Ldarg_0);
                    ilgen.Emit(OpCodes.Ldfld, GLuaField);
                    ilgen.EmitLDC(method.Args.Count);
                    if (method.ReturnType != typeof(void)) { ilgen.Emit(OpCodes.Ldc_I4_1); } else { ilgen.Emit(OpCodes.Ldc_I4_0); }
                    ilgen.Emit(OpCodes.Ldc_I4_0);
                    ilgen.EmitCall(OpCodes.Callvirt, pcallMethodInfo, null);
                    ilgen.Emit(OpCodes.Pop);

                    if (method.ReturnType != typeof(void))
                    {
                        ilgen.Emit(OpCodes.Ldarg_0);
                        ilgen.Emit(OpCodes.Ldfld, GLuaField);
                        ilgen.EmitCall(OpCodes.Callvirt, getMethodInfo.MakeGenericMethod(method.ReturnType), null);
                    }
                    ilgen.Emit(OpCodes.Ret);
                }

                implClass = builder.CreateType();

                this.Libraries.Add(new Tuple<Type, String>(targetInterface, globalName), implClass);
            }

            Object instClass = Activator.CreateInstance(implClass);
            FieldInfo GLuaInstanceField = implClass.GetField("GLua", BindingFlags.Public | BindingFlags.Instance);
            GLuaInstanceField.SetValue(instClass, this);

            return (TClass)instClass;
        }
    }
}
