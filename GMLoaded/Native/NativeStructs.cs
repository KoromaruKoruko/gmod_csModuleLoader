using GMLoaded.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace GMLoaded.Native
{
#warning Unknown if this is Win32 Specific or even just Win* Specific

    public struct NativeClassHierarchyDescriptor
    {
        public Int32 Attributes;
        public RTTIBaseClassDescriptor[] BaseClasses;
        public Int32 Signature;
    }

    public struct PMD
    {
        //public DWORD DisplacementInsideVTable;
        public Int32 DisplacementInsideVTable;

        //public DWORD MemberDisplacement;
        public Int32 MemberDisplacement;

        //public DWORD VTableDisplacement;
        public Int32 VTableDisplacement;
    }

    public unsafe struct RTTIBaseClassArray
    {
        public IntPtr* Bases;
    }

    public unsafe struct RTTIBaseClassDescriptor
    {
        //public DWORD Attributes;
        public Int32 Attributes;

        public RTTIClassHierarchyDescriptor* HierarchyDescriptor;

        //public DWORD NumBaseClasses;
        public Int32 NumBaseClasses;

        public TypeDescriptor* TypeDescriptor;
        public PMD Where;
    }

    public unsafe struct RTTIClassHierarchyDescriptor
    {
        //public DWORD Attribute;
        public Int32 Attribute;

        public IntPtr BaseClassArray;

        //public DWORD NumBaseClasses;
        public Int32 NumBaseClasses;

        //public DWORD Signature;
        public Int32 Signature;
    }

    public unsafe struct RTTICompleteObjectLocator
    {
        public Int64 ConstructorDisplacementOffset;
        public RTTIClassHierarchyDescriptor* HierarchyDescriptor;
        public Int64 Offset;
        public IntPtr Self;
        public Int64 Signature;
        public TypeDescriptor* TypeDescriptor;
    }

    public unsafe struct RTTICompleteObjectLocator2
    {
        public Int32 BaseClassOffset;
        public Int32 Flags;
        public Int32 ObjectLocator;

        //public DWORD Signature;
        public Int32 Signature;

        public Int32 TypeDesc;
        public Int32 TypeHierarchy;
    }

    public unsafe struct TypeDescriptor
    {
        public fixed Char Mangled[128];
        public IntPtr Name;
        public IntPtr VTable;
    }

    public unsafe class NativeClass
    {
        private static readonly Int32 VTablePointerSize = IntPtr.Size;

        private static Boolean BaseContains(MethodInfo MethodInfo, out MethodInfo BaseMethodInfo)
        {
            Type IntType = MethodInfo.DeclaringType;

            Type[] AllInterfaces = IntType.GetInterfaces();
            for (Int32 i = 0; i < AllInterfaces.Length; i++)
            {
                MethodInfo[] AllMethods = AllInterfaces[i].GetMethods();

                for (Int32 j = 0; j < AllMethods.Length; j++)
                {
                    MethodInfo BaseM = AllMethods[j];
                    if (MethodInfo.Name == BaseM.Name && MethodInfo.ReturnType == BaseM.ReturnType)
                    {
                        ParameterInfo[] MInfParams = MethodInfo.GetParameters();
                        ParameterInfo[] BaseParams = BaseM.GetParameters();

                        if (MInfParams.Length != BaseParams.Length)
                            continue;

                        Boolean Continue = false;
                        for (Int32 k = 0; k < MInfParams.Length; k++)
                            if (MInfParams[k].ParameterType != BaseParams[k].ParameterType)
                            {
                                Continue = true;
                                break;
                            }

                        if (Continue)
                            continue;

                        BaseMethodInfo = BaseM;
                        return true;
                    }
                }
            }

            BaseMethodInfo = null;
            return false;
        }

        private static void CalculateVTableLayout(Type T, Action<Int32, Type> OnTypeResolve)
        {
            Tree<Type> InterfaceTree = new Tree<Type>(T, GetNodes(T));
            Tree<Type>.TreeNode[] Leaves = InterfaceTree.GetLeaves();

            List<Type> Interfaces = new List<Type>();
            Int32 VTableOffset = 0;

            for (Int32 i = 0; i < Leaves.Length; i++)
            {
                Tree<Type>.TreeNode[] PTP = Leaves[i].PathToParent();

                Int32 SizeOffset = 0;

                for (Int32 j = 0; j < PTP.Length; j++)
                {
                    if (!Interfaces.Contains(PTP[j].Userdata))
                    {
                        Interfaces.Add(PTP[j].Userdata);

                        SizeOffset += SizeOfVariables(PTP[j].Userdata);
                        OnTypeResolve(VTableOffset, PTP[j].Userdata);
                    }
                }

                VTableOffset += VTablePointerSize + SizeOffset;
            }
        }

        private static RTTICompleteObjectLocator2* GetLocator(IntPtr Instance) => (*(RTTICompleteObjectLocator2***)Instance)[-1];

        private static IEnumerable<Tree<Type>.TreeNode> GetNodes(Type T)
        {
            Type[] Interfaces = T.GetInterfaces();
            Type[] Types = Interfaces.Except(Interfaces.SelectMany(Typ => Typ.GetInterfaces())).ToArray();

            for (Int32 i = 0; i < Types.Length; i++)
            {
                Tree<Type>.TreeNode N = new Tree<Type>.TreeNode(GetNodes(Types[i]))
                {
                    Userdata = Types[i]
                };
                yield return N;
            }
        }

        private static IntPtr GetOffset(RTTICompleteObjectLocator2* Locator, IntPtr Offset)
        {
            IntPtr Base = Locator->Signature == 0
                ? Natives.Kernel32.RtlPcToFileHeader((IntPtr)Locator, out Base)
                : (IntPtr)Locator - Locator->ObjectLocator;
            return Base + (Int32)Offset;
        }

        private static PropertyInfo GetPropertyForMethod(MethodInfo MI)
        {
            PropertyInfo[] Props = MI.DeclaringType.GetProperties();

            for (Int32 i = 0; i < Props.Length; i++)
                if (Props[i].GetGetMethod() == MI || Props[i].GetSetMethod() == MI)
                    return Props[i];

            return null;
        }

        private static Int32 SizeOfVariables(Type T) => T.GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Select((PI) => Marshal.SizeOf(PI.PropertyType)).Sum();

        public static NativeClassInfo CalculateClassLayout(Type T)
        {
            List<NativeMethodInfo> MethodInfos = new List<NativeMethodInfo>();
            List<NativeVariableInfo> VariableInfos = new List<NativeVariableInfo>();

            Dictionary<Int32, Int32> MethodCounter = new Dictionary<Int32, Int32>();
            Dictionary<Int32, Int32> VariableOffsets = new Dictionary<Int32, Int32>();

            CalculateVTableLayout(T, (VTableOffset, Interface) =>
            {
                MethodInfo[] Methods = Interface.GetMethods();

                if (!MethodCounter.ContainsKey(VTableOffset))
                {
                    MethodCounter.Add(VTableOffset, 0);
                    VariableOffsets.Add(VTableOffset, VTablePointerSize);
                }

                for (Int32 i = 0; i < Methods.Length; i++)
                {
                    PropertyInfo Prop;
                    if ((Prop = GetPropertyForMethod(Methods[i])) != null)
                    {
                        if (Prop.GetSetMethod() == Methods[i])
                            continue;

                        NativeVariableInfo VarInfo = new NativeVariableInfo
                        {
                            Offset = VariableOffsets[VTableOffset],

                            PropertyInfo = Prop
                        };
                        VarInfo.VariableType = VarInfo.PropertyInfo.PropertyType;
                        VarInfo.Name = VarInfo.PropertyInfo.Name;

                        VariableOffsets[VTableOffset] += Marshal.SizeOf(VarInfo.VariableType);
                        VariableInfos.Add(VarInfo);
                        continue;
                    }

                    NativeMethodInfo VMethInfo = new NativeMethodInfo
                    {
                        Method = Methods[i]
                    };

                    if (BaseContains(Methods[i], out MethodInfo BMI))
                    {
                        VMethInfo.BaseMethod = BMI;
                        VMethInfo.DoesOverride = true;
                    }
                    else
                    {
                        VMethInfo.BaseMethod = null;
                        VMethInfo.MethodIndex = MethodCounter[VTableOffset]++;
                        VMethInfo.VTableOffset = VTableOffset;
                        VMethInfo.DoesOverride = false;
                    }

                    MethodInfos.Add(VMethInfo);
                }
            });

            NativeClassInfo Ret = new NativeClassInfo();
            Ret.MethodInfo.AddRange(MethodInfos);
            Ret.VariableInfo.AddRange(VariableInfos);
            return Ret;
        }

        public static NativeClassHierarchyDescriptor GetClassHierarchy(IntPtr Instance)
        {
            // TODO: FEEEEEEEEEEEEEEX
            RTTICompleteObjectLocator2* Locator = GetLocator(Instance);
            RTTIClassHierarchyDescriptor* Hierarchy = (RTTIClassHierarchyDescriptor*)GetOffset(Locator, (IntPtr)Locator->TypeHierarchy);
            IntPtr BaseClassArray = GetOffset(Locator, Hierarchy->BaseClassArray);

            NativeClassHierarchyDescriptor D = new NativeClassHierarchyDescriptor
            {
                Attributes = (Int32)Hierarchy->Attribute,
                Signature = (Int32)Hierarchy->Signature,
                BaseClasses = new RTTIBaseClassDescriptor[Hierarchy->NumBaseClasses]
            };

            for (Int32 i = 0; i < D.BaseClasses.Length; i++)
            {
                IntPtr OFF = Marshal.ReadIntPtr(Marshal.ReadIntPtr(BaseClassArray), IntPtr.Size * i);

                RTTIBaseClassDescriptor* DP = (RTTIBaseClassDescriptor*)GetOffset(Locator, OFF);
                //RTTIBaseClassDescriptor* DP = ((RTTIBaseClassDescriptor**)BaseClassArray)[i];
                D.BaseClasses[i] = *DP;
            }

            return D;
        }

        public static TypeDescriptor* GetTypeDesc(IntPtr Instance)
        {
            RTTICompleteObjectLocator2* Locator = GetLocator(Instance);
            return (TypeDescriptor*)GetOffset(Locator, (IntPtr)Locator->TypeDesc);
        }

        public static NativeTypeInfo GetTypeInfo(IntPtr Instance) => new NativeTypeInfo(GetTypeDesc(Instance));

        public static IntPtr GetVTable(IntPtr Instance, Int32 VTableOffset = 0) => Marshal.ReadIntPtr(Instance, VTableOffset);
    }

    public class NativeClassInfo
    {
        public List<NativeMethodInfo> MethodInfo;
        public List<NativeVariableInfo> VariableInfo;

        public NativeClassInfo()
        {
            this.MethodInfo = new List<NativeMethodInfo>();
            this.VariableInfo = new List<NativeVariableInfo>();
        }

        public NativeMethodInfo Find(MethodInfo Info, Boolean SkipOverrides = false)
        {
            NativeMethodInfo Ret = this.MethodInfo.FirstOrDefault((VMI) => VMI.Method == Info && (SkipOverrides ? VMI.DoesOverride != true : true));
            if (Ret != null && Ret.DoesOverride)
                return this.Find(Ret.BaseMethod, true);
            return Ret;
        }

        public NativeVariableInfo FindVariable(PropertyInfo Prop)
        {
            foreach (NativeVariableInfo VI in this.VariableInfo)
            {
                if (VI.PropertyInfo == Prop)
                    return VI;
            }
            return null;
        }
    }

    public class NativeMethodInfo
    {
        public MethodInfo BaseMethod;
        public Boolean DoesOverride;
        public MethodInfo Method;
        public Int32 MethodIndex;
        public Int32 ThisOffset;
        public Int32 VTableOffset;

        public static IntPtr ReadVTableSlot(IntPtr VTable, Int32 Slot) => Marshal.ReadIntPtr(VTable, IntPtr.Size * Slot);

        public T GetDelegate<T>(IntPtr Instance) where T : class => (T)this.GetDelegate(typeof(T), Instance);

        public Object GetDelegate(Type DelegateType, IntPtr Instance)
        {
            IntPtr F = ReadVTableSlot(NativeClass.GetVTable(Instance, this.VTableOffset), this.MethodIndex);
            return Marshal.GetDelegateForFunctionPointer(F, DelegateType);
        }
    }

    public unsafe class NativeTypeInfo
    {
        public TypeDescriptor* TypeDesc;
        public IntPtr VTable;

        public NativeTypeInfo(TypeDescriptor* TypeDesc)
        {
            this.TypeDesc = TypeDesc;
            this.VTable = TypeDesc->VTable;
        }

        public String MangledName => Marshal.PtrToStringAnsi((IntPtr)this.TypeDesc->Mangled);

        public String Name
        {
            get
            {
                if (this.TypeDesc->Name == IntPtr.Zero)
                    return null;
                return Marshal.PtrToStringAnsi(this.TypeDesc->Name);
            }
        }
    }

    public class NativeVariableInfo
    {
        public String Name;
        public Int32 Offset;
        public PropertyInfo PropertyInfo;
        public Type VariableType;

        public IntPtr GetAddress(IntPtr Instance) => Instance + this.Offset;
    }
}
