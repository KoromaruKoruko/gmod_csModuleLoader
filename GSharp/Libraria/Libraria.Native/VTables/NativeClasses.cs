using Libraria.Collections;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

using DWORD = System.UInt32;

namespace Libraria.Native
{
    public unsafe struct TypeDescriptor
    {
        public IntPtr VTable;
        public IntPtr Name;
        public fixed Char Mangled[128];
    }

    public unsafe struct RTTICompleteObjectLocator
    {
        public Int64 Signature;
        public Int64 Offset;
        public Int64 ConstructorDisplacementOffset;
        public TypeDescriptor* TypeDescriptor;
        public RTTIClassHierarchyDescriptor* HierarchyDescriptor;
        public IntPtr Self;
    }

    public unsafe struct RTTICompleteObjectLocator2
    {
        public DWORD Signature;
        public Int32 BaseClassOffset;
        public Int32 Flags;
        public Int32 TypeDesc;
        public Int32 TypeHierarchy;
        public Int32 ObjectLocator;
    }

    public unsafe struct RTTIClassHierarchyDescriptor
    {
        public DWORD Signature;
        public DWORD Attribute;
        public DWORD NumBaseClasses;
        public IntPtr BaseClassArray;
    }

    public unsafe struct RTTIBaseClassArray
    {
        public IntPtr* Bases;
    }

    public unsafe struct RTTIBaseClassDescriptor
    {
        public TypeDescriptor* TypeDescriptor;
        public DWORD NumBaseClasses;
        public PMD Where;
        public DWORD Attributes;
        public RTTIClassHierarchyDescriptor* HierarchyDescriptor;
    }

    public struct PMD
    {
        public DWORD MemberDisplacement;
        public DWORD VTableDisplacement;
        public DWORD DisplacementInsideVTable;
    }

    // RTTI END ///////////////////////////////////////////

    public struct NativeClassHierarchyDescriptor
    {
        public Int32 Signature;
        public Int32 Attributes;
        public RTTIBaseClassDescriptor[] BaseClasses;
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

    public class NativeMethodInfo
    {
        public Boolean DoesOverride;
        public Int32 VTableOffset;
        public Int32 MethodIndex;
        public Int32 ThisOffset;

        public MethodInfo Method;
        public MethodInfo BaseMethod;

        public T GetDelegate<T>(IntPtr Instance) where T : class => (T)this.GetDelegate(typeof(T), Instance);

        public Object GetDelegate(Type DelegateType, IntPtr Instance)
        {
            IntPtr F = ReadVTableSlot(NativeClass.GetVTable(Instance, this.VTableOffset), this.MethodIndex);
            return Marshal.GetDelegateForFunctionPointer(F, DelegateType);
        }

        /*public static IntPtr GetVTablePtr(IntPtr Instance, int VTableIdx = 0) {
			return Marshal.ReadIntPtr(Instance, IntPtr.Size * VTableIdx);
		}*/

        public static IntPtr ReadVTableSlot(IntPtr VTable, Int32 Slot) => Marshal.ReadIntPtr(VTable, IntPtr.Size * Slot);
    }

    public class NativeVariableInfo
    {
        public Int32 Offset;
        public String Name;
        public Type VariableType;
        public PropertyInfo PropertyInfo;

        public IntPtr GetAddress(IntPtr Instance) => Instance + this.Offset;
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

        /*public void Invoke(IntPtr Instance, MethodInfo Info) {
			Find(Info).Invoke(Instance);
		}*/
    }

    public unsafe class NativeClass
    {
        static readonly Int32 VTablePointerSize = IntPtr.Size;

        public static IntPtr GetVTable(IntPtr Instance, Int32 VTableOffset = 0) => Marshal.ReadIntPtr(Instance, VTableOffset);

        static Int32 SizeOfVariables(Type T) => T.GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Select((PI) => Marshal.SizeOf(PI.PropertyType)).Sum();

        static PropertyInfo GetPropertyForMethod(MethodInfo MI)
        {
            PropertyInfo[] Props = MI.DeclaringType.GetProperties();

            for (Int32 i = 0; i < Props.Length; i++)
                if (Props[i].GetGetMethod() == MI || Props[i].GetSetMethod() == MI)
                    return Props[i];

            return null;
        }

        static Boolean BaseContains(MethodInfo MethodInfo, out MethodInfo BaseMethodInfo)
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

        static IEnumerable<Tree<Type>.TreeNode> GetNodes(Type T)
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

        static void CalculateVTableLayout(Type T, Action<Int32, Type> OnTypeResolve)
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

        static IntPtr GetOffset(RTTICompleteObjectLocator2* Locator, IntPtr Offset)
        {
            IntPtr Base;

            if (Locator->Signature == 0)
                Base = Kernel32.RtlPcToFileHeader((IntPtr)Locator, out Base);
            else
                Base = (IntPtr)Locator - Locator->ObjectLocator;

            return Base + (Int32)Offset;
        }

        static RTTICompleteObjectLocator2* GetLocator(IntPtr Instance) => (*(RTTICompleteObjectLocator2***)Instance)[-1];

        public static TypeDescriptor* GetTypeDesc(IntPtr Instance)
        {
            RTTICompleteObjectLocator2* Locator = GetLocator(Instance);
            return (TypeDescriptor*)GetOffset(Locator, (IntPtr)Locator->TypeDesc);
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

        public static NativeTypeInfo GetTypeInfo(IntPtr Instance) => new NativeTypeInfo(GetTypeDesc(Instance));
    }

    /*public unsafe class VTable {
		public static VTable GetVTable(object Interface) {
			Type ObjType = Interface.GetType();
			FieldInfo VTableField = ObjType.GetField(nameof(VTable));

			if (VTableField == null || VTableField.FieldType != typeof(VTable))
				throw new Exception(Interface.ToString() + " not a valid interface type");

			VTable VT = (VTable)VTableField.GetValue(Interface);
			if (VT == null)
				VTableField.SetValue(Interface, VT = new VTable(Interface));

			return VT;
		}

		protected object Object;
		protected Type ObjectType;
		protected Type InterfaceType;

		protected IntPtr ObjectAddress;
		protected IntPtr* VTbl;

		protected Dictionary<int, IntPtr> Originals;
		protected List<GCHandle> NewHookHandles;

		private VTable(object Object) {
			Originals = new Dictionary<int, IntPtr>();
			NewHookHandles = new List<GCHandle>();

			this.Object = Object;
			ObjectType = Object.GetType();
			InterfaceType = ObjectType.GetInterfaces().First();

			ObjectAddress = (IntPtr)ObjectType.GetField("ObjectAddress").GetValue(Object);
			VTbl = *(IntPtr**)ObjectAddress;
		}

		public int GetIdxForName(string Name) {
			MethodInfo MethInfo = InterfaceType.GetMethod(Name);
			return MethInfo.GetCustomAttribute<VTableSlotAttribute>().Slot;
		}

		public IntPtr Hook(int Idx, IntPtr New) {
			if (!Originals.ContainsKey(Idx))
				Originals.Add(Idx, VTbl[Idx]);

			MemProtection P = MemProtection.ReadWrite;
			Kernel32.VirtualProtect((IntPtr)(&VTbl[Idx]), IntPtr.Size, P, out P);

			IntPtr Old = VTbl[Idx];
			VTbl[Idx] = New;
			return Old;
		}

		public IntPtr Hook(string Name, IntPtr New) {
			return Hook(GetIdxForName(Name), New);
		}

		public T Hook<T>(int Idx, T New) where T : class {
			NewHookHandles.Add(GCHandle.Alloc(New));
			return Marshal.GetDelegateForFunctionPointer(Hook(Idx, Marshal.GetFunctionPointerForDelegate(New)), typeof(T)) as T;
		}

		public T Hook<T>(string Name, T New) where T : class {
			return Hook<T>(GetIdxForName(Name), New);
		}

		public void Unhook(int Idx) {
			if (Originals.ContainsKey(Idx)) {
				IntPtr Old = Originals[Idx];
				Originals.Remove(Idx);
				VTbl[Idx] = Old;
			}
		}

		public void UnhookAll() {
			int[] Hooked = Originals.Keys.ToArray();
			for (int i = 0; i < Hooked.Length; i++)
				Unhook(Hooked[i]);

			foreach (var Handle in NewHookHandles)
				Handle.Free();
			NewHookHandles.Clear();
		}
	}*/
}