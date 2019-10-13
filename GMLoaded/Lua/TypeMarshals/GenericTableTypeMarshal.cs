using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace GMLoaded.Lua.TypeMarshals
{
    public interface IGenericTableTypeMarshal<T> : ILuaTypeMarshal
    {
        T GetT(GLua GLua, Int32 StackPos);
        void Push(GLua GLua, T Obj);
    }

    public static class GenericTableTypeMarshal
    {
        private static readonly Dictionary<Type, ILuaTypeMarshal> Marshals = new Dictionary<Type, ILuaTypeMarshal>();

        private static ModuleBuilder moduleBuilder;

        /// <summary>
        /// Create an Optimized GenericTableMarshal(ILGeneration) or return a pre-existing one
        /// </summary>
        public static IGenericTableTypeMarshal<T> Create<T>() where T : ITableBase
        {
            Type ReturnType = typeof(T);

            if (ReturnType == typeof(ITableBase))
                throw new InvalidOperationException("Use the ITableBaseTypeMarshal!");

            if (Marshals.ContainsKey(ReturnType))
                return Marshals[ReturnType] as IGenericTableTypeMarshal<T>;
            else
            {
                Type GluaType = typeof(GLua);
                Type Int32Type = typeof(Int32);

                // verify ReturnType Constructor to be valid
                ConstructorInfo ReturnType_Constructor = ReturnType.GetConstructor(BindingFlags.Public, null, new Type[] { GluaType, Int32Type }, null);

                if (ReturnType_Constructor == null)
                    throw new ArgumentException("Generic Type does not contain valid ITableBase Constructor  new T(GLua Glua, Int32 IStackPos) : base(Glua, IStackPos)", "Generic<T>");

                if (moduleBuilder == null)
                {
                    AssemblyBuilder assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("JITGenericTableMarhsals"), AssemblyBuilderAccess.RunAndCollect);
                    moduleBuilder = assemblyBuilder.DefineDynamicModule("LuaTableMarshalsJIT");
                }

                Type Container = typeof(IGenericTableTypeMarshal<T>);
                // Create Type to build the marshler
                TypeBuilder builder;
                {
                    String Name = $"{ReturnType.Name}TableMarshal_True";
                    builder = moduleBuilder.DefineType(Name, TypeAttributes.Class, null, new Type[1] { Container });
                }
                builder.AddInterfaceImplementation(Container);

                // Build Functions
                Type ObjectType = typeof(Object);
                Type VoidType = typeof(void);

                // T GetT(GLua arg1, Int32 arg2) => new T(arg1, arg2);
                {
                    MethodBuilder GetT = builder.DefineMethod("GetT", MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Virtual, CallingConventions.HasThis, ReturnType, new[] { GluaType, Int32Type });

                    builder.DefineMethodOverride(GetT, Container.GetMethod("GetT"));

                    ILGenerator IL = GetT.GetILGenerator();
                    IL.Emit(OpCodes.Ldarg_1);
                    IL.Emit(OpCodes.Ldarg_2);
                    IL.Emit(OpCodes.Newobj, ReturnType_Constructor);
                    IL.Emit(OpCodes.Ret);
                }

                //Object Get(GLua GLua, Int32 stackPos = -1) => new T(arg1, arg2) as Object;
                {
                    MethodBuilder Get = builder.DefineMethod("Get", MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Virtual, CallingConventions.HasThis, ObjectType, new[] { GluaType, Int32Type });

                    builder.DefineMethodOverride(Get, Container.GetMethod("Get"));

                    ILGenerator IL = Get.GetILGenerator();
                    IL.Emit(OpCodes.Ldarg_1);
                    IL.Emit(OpCodes.Ldarg_2);
                    IL.Emit(OpCodes.Newobj, ReturnType_Constructor);
                    IL.Emit(OpCodes.Box);
                    IL.Emit(OpCodes.Ret);
                }

                ConstructorInfo InvalidOperationExceptionType_Constructor = typeof(InvalidOperationException).GetConstructor(BindingFlags.Public, null, new[] { typeof(String) }, null);
                FieldInfo ReturnType_LuaHandle = ReturnType.GetField("LuaHandle");
                FieldInfo ReturnType_Referance = ReturnType.GetField("Refrance");
                MethodInfo GLua_ReferancePush = GluaType.GetMethod("ReferancePush", BindingFlags.Public, null, new[] { Int32Type }, null);

                //void Push(GLua GLua, T obj)
                //if (GLua != obj.LuaHandle)
                //    throw new InvalidOperationException("You cant transfer tables between LuaStates");
                //GLua.ReferencePush(Proxy.Refrance);
                {
                    MethodBuilder Push = builder.DefineMethod("Push", MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Virtual, CallingConventions.HasThis, VoidType, new[] { GluaType, ReturnType });

                    builder.DefineMethodOverride(Push, Container.GetMethod("Push", new[] { GluaType, ReturnType }));

                    ILGenerator IL = Push.GetILGenerator();
                    IL.Emit(OpCodes.Ldarg_2);                                   // push obj
                    IL.Emit(OpCodes.Ldfld, ReturnType_LuaHandle);               // push obj::LuaHandle
                    IL.Emit(OpCodes.Ldarg_1);                                   // push GLua
                    IL.Emit(OpCodes.Ceq);                                       // compair GLua with obj::LuaHandle

                    Label Lb = new Label();
                    IL.Emit(OpCodes.Brfalse, Lb);                               // if false throw new InvalidOperationException("You cant transfer tables between LuaStates")

                    IL.Emit(OpCodes.Ldstr, "You cant transfer tables between LuaStates");
                    IL.Emit(OpCodes.Newobj, InvalidOperationExceptionType_Constructor);
                    IL.Emit(OpCodes.Throw);

                    IL.MarkLabel(Lb);                                           // if true GLua.ReferancePush(obj::Referance)
                    IL.Emit(OpCodes.Ldarg_1);
                    IL.Emit(OpCodes.Ldarg_2);
                    IL.Emit(OpCodes.Ldfld, ReturnType_Referance);
                    IL.EmitCall(OpCodes.Call, GLua_ReferancePush, null);

                    IL.Emit(OpCodes.Ret);
                }

                //void Push(GLua GLua, Object obj)
                //if (!(obj is T))
                //    throw new InvalidCastExcaption()
                //if (GLua != obj.LuaHandle)
                //    throw new InvalidOperationException("You cant transfer tables between LuaStates");
                //GLua.ReferencePush(Proxy.Refrance);
                {
                    MethodBuilder Push = builder.DefineMethod("Push", MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Virtual, CallingConventions.HasThis, VoidType, new[] { GluaType, ObjectType });

                    builder.DefineMethodOverride(Push, Container.GetMethod("Push", new[] { GluaType, ObjectType }));

                    ILGenerator IL = Push.GetILGenerator();

                    IL.Emit(OpCodes.Ldarg_2);
                    IL.Emit(OpCodes.Castclass, ReturnType);

                    IL.Emit(OpCodes.Stloc_0);
                    IL.Emit(OpCodes.Ldloc_0);

                    Label Lb = new Label();
                    IL.Emit(OpCodes.Brtrue, Lb);
                    IL.ThrowException(typeof(InvalidCastException)); // incase the initial case failed

                    IL.MarkLabel(Lb);
                    IL.Emit(OpCodes.Ldloc_0);                                   // push obj
                    IL.Emit(OpCodes.Ldfld, ReturnType_LuaHandle);               // push obj::LuaHandle
                    IL.Emit(OpCodes.Ldarg_1);                                   // push GLua
                    IL.Emit(OpCodes.Ceq);                                       // compair GLua with obj::LuaHandle

                    Lb = new Label();
                    IL.Emit(OpCodes.Brfalse, Lb);                               // if false throw new InvalidOperationException("You cant transfer tables between LuaStates")

                    IL.Emit(OpCodes.Ldstr, "You cant transfer tables between LuaStates");
                    IL.Emit(OpCodes.Newobj, InvalidOperationExceptionType_Constructor);
                    IL.Emit(OpCodes.Throw);

                    IL.MarkLabel(Lb);                                           // if true GLua.ReferancePush(obj::Referance)
                    IL.Emit(OpCodes.Ldarg_1);
                    IL.Emit(OpCodes.Ldloc_0);
                    IL.Emit(OpCodes.Ldfld, ReturnType_Referance);
                    IL.EmitCall(OpCodes.Call, GLua_ReferancePush, null);

                    IL.Emit(OpCodes.Ret);
                }

                return Activator.CreateInstance(builder.CreateType()) as IGenericTableTypeMarshal<T>;
            }
        }
    }
}
