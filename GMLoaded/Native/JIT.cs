using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;

namespace GMLoaded.Native
{
    public static class JIT
    {
        private static readonly Dictionary<String, Type> NativeClassWrappers = new Dictionary<String, Type>();
        private static AssemblyBuilder AsmBuilder;
        private static ModuleBuilder ModBuilder;

        public static T ConvertInstance<T>(IntPtr Instance) where T : class
        {
            Type WrapperType = CreateWrapper<T>(out NativeClassInfo NativeInfo);

            NativeClassImpl NativeClass = (NativeClassImpl)Activator.CreateInstance(WrapperType);
            NativeClass.InstancePointer = Instance;
            NativeClass.NativeInfo = NativeInfo;

            return (T)(Object)NativeClass;
        }

        public static Type CreateInterfaceImpl<T>(String Name, Action<TypeBuilder> Create) where T : class => CreateType(Name, (TB) =>
                                                                                                                        {
                                                                                                                            TB.SetParent(typeof(NativeClassImpl));
                                                                                                                            TB.AddInterfaceImplementation(typeof(T));
                                                                                                                            Create(TB);
                                                                                                                        });

        public static void CreateMethodImpl(TypeBuilder TB, MethodInfo Template, Action<MethodBuilder, ILGenerator, Type[]> Create)
        {
            MethodBuilder MB = TB.DefineMethod(Template.Name, MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Virtual);
            Type[] ParamTypes = Template.GetParameters().Select((PI) => PI.ParameterType).ToArray();
            MB.SetParameters(ParamTypes);
            MB.SetReturnType(Template.ReturnType);
            Create(MB, MB.GetILGenerator(), ParamTypes);
        }

        public static Type CreateType(String Name, Action<TypeBuilder> Create)
        {
            ModuleBuilder ModBuiler = GetModuleBuilder();
            TypeBuilder TB = ModBuilder.DefineType(Name, TypeAttributes.Public | TypeAttributes.Class);
            Create(TB);
            return TB.CreateType();
        }

        public static void CreateVariableImpl(TypeBuilder TB, PropertyInfo Template, Int32 InstanceOffset)
        {
            MethodAttributes Attribs = MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Virtual | MethodAttributes.SpecialName;
            PropertyBuilder PB = TB.DefineProperty(Template.Name, PropertyAttributes.None, Template.PropertyType, Type.EmptyTypes);

            MethodBuilder GetMethod = TB.DefineMethod("get_" + Template.Name, Attribs);
            GetMethod.SetReturnType(Template.PropertyType);
            ILGenerator GetGen = GetMethod.GetILGenerator();
            {
                GetGen.Emit(OpCodes.Ldarg_0);
                GetGen.Emit(OpCodes.Ldc_I4, InstanceOffset);
                GetGen.EmitCall(OpCodes.Call, TB.BaseType.GetMethod(nameof(NativeClassImpl.OffsetThisPtr)), null);
                // ptr to field on stack ^

                GetGen.Emit(OpCodes.Ldobj, Template.PropertyType);
                GetGen.Emit(OpCodes.Ret);
            }
            PB.SetGetMethod(GetMethod);

            MethodBuilder SetMethod = TB.DefineMethod("set_" + Template.Name, Attribs);
            SetMethod.SetReturnType(typeof(void));
            SetMethod.SetParameters(Template.PropertyType);
            ILGenerator SetGen = SetMethod.GetILGenerator();
            {
                SetGen.Emit(OpCodes.Ldarg_0);
                SetGen.Emit(OpCodes.Ldc_I4, InstanceOffset);
                SetGen.EmitCall(OpCodes.Call, TB.BaseType.GetMethod(nameof(NativeClassImpl.OffsetThisPtr)), null);
                // ptr to field on stack ^

                SetGen.Emit(OpCodes.Ldarg_1);
                SetGen.Emit(OpCodes.Stobj, Template.PropertyType);
                SetGen.Emit(OpCodes.Ret);
            }
            PB.SetSetMethod(SetMethod);
        }

        public static Type CreateWrapper<T>(out NativeClassInfo NativeCInfo) where T : class
        {
            NativeCInfo = null;

            String WrapperName = typeof(T).Name + "_impl";
            if (NativeClassWrappers.ContainsKey(WrapperName))
                return NativeClassWrappers[WrapperName];

            NativeClassInfo NativeInfo = NativeCInfo = NativeClass.CalculateClassLayout(typeof(T));

            Type TypeWrapper = CreateInterfaceImpl<T>(WrapperName, (TB) =>
            {
                foreach (NativeVariableInfo VarInf in NativeInfo.VariableInfo)
                    CreateVariableImpl(TB, VarInf.PropertyInfo, VarInf.Offset);

                foreach (NativeMethodInfo MetInf in NativeInfo.MethodInfo)
                {
                    if (MetInf.DoesOverride)
                        continue;

                    CreateMethodImpl(TB, MetInf.Method, (MB, ILGen, ParamTypes) =>
                    {
                        ILGen.Emit(OpCodes.Ldarg_0);
                        ILGen.Emit(OpCodes.Ldc_I4, MetInf.ThisOffset);
                        ILGen.EmitCall(OpCodes.Call, TB.BaseType.GetMethod(nameof(NativeClassImpl.OffsetThisPtr)), null);

                        for (Int32 i = 0; i < ParamTypes.Length; i++)
                            ILGen.Emit(OpCodes.Ldarg, i + 1);

                        ILGen.Emit(OpCodes.Ldarg_0);
                        ILGen.Emit(OpCodes.Ldc_I4, MetInf.VTableOffset);
                        ILGen.Emit(OpCodes.Ldc_I4, MetInf.MethodIndex);
                        ILGen.EmitCall(OpCodes.Call, TB.BaseType.GetMethod(nameof(NativeClassImpl.GetMethodPointer)), null);

                        ILGen.EmitCalli(OpCodes.Calli, CallingConvention.ThisCall, MetInf.Method.ReturnType, new Type[] { typeof(IntPtr) }.Append(ParamTypes));
                        ILGen.Emit(OpCodes.Ret);
                    });
                }
            });

            NativeClassWrappers.Add(WrapperName, TypeWrapper);
            return TypeWrapper;
        }

        public static AssemblyBuilder GetAssemblyBuilder()
        {
            if (AsmBuilder != null)
                return AsmBuilder;

            AssemblyName AName = new AssemblyName(nameof(JIT));
            return AsmBuilder = AssemblyBuilder.DefineDynamicAssembly(AName, AssemblyBuilderAccess.Run);
        }

        public static ModuleBuilder GetModuleBuilder()
        {
            if (ModBuilder != null)
                return ModBuilder;

            AssemblyBuilder AsmBuilder = GetAssemblyBuilder();
            return ModBuilder = AsmBuilder.DefineDynamicModule(AsmBuilder.GetName().Name);
        }
    }

    public unsafe class NativeClassImpl
    {
        public IntPtr InstancePointer;
        public NativeClassInfo NativeInfo;

        public IntPtr GetMethodPointer(Int32 VTableOffset, Int32 MethodIndex)
        {
            IntPtr* VTable = *(IntPtr**)this.InstancePointer + VTableOffset;
            IntPtr Method = VTable[MethodIndex];
            return Method;
        }

        public NativeTypeInfo GetTypeInfo() => NativeClass.GetTypeInfo(this.InstancePointer);

        public IntPtr GetVariablePointer(Int32 Offset) => this.InstancePointer + Offset;

        public IntPtr OffsetThisPtr(Int32 Offset) => this.InstancePointer + Offset;
    }

    public class NativeClassMarshal<T> : ICustomMarshaler where T : class
    {
        public static ICustomMarshaler GetInstance() => new NativeClassMarshal<T>();

        public void CleanUpManagedData(Object ManagedObj)
        {
        }

        public void CleanUpNativeData(IntPtr pNativeData)
        {
        }

        public Int32 GetNativeDataSize() => IntPtr.Size;

        public IntPtr MarshalManagedToNative(Object ManagedObj) => IntPtr.Zero;

        public Object MarshalNativeToManaged(IntPtr pNativeData) => JIT.ConvertInstance<T>(pNativeData);
    }
}
