using System;
using System.Runtime.InteropServices;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.IO;
using GMLoaded.Native;
using GMLoaded.Native.Classes;
using GMLoaded.Lua;
using GMLoaded.Attributes;

namespace GMLoaded
{
    public enum System : Byte
    {
        Linux = 0,
        Windows = 1,
        OSx = 2,
    }

    public unsafe static class InterfaceLoader
    {
        /// <summary>
        /// Get CreateInterface Function from DLL
        /// </summary>
        /// <param name="dllPath">Path to DLL file</param>
        /// <returns>Native (SourceSDK) CreateInterface(String)</returns>
        private static CreateInterfaceFn LoadCreateInterface(String dllPath)
        {
            IntPtr hModule = Natives.LoadDL(dllPath);
            if (hModule == IntPtr.Zero)
                return null;
            IntPtr functionAddress = Natives.GetDLSymbol(hModule, "CreateInterface");
            return functionAddress == IntPtr.Zero
                ? null
                : (CreateInterfaceFn)Marshal.GetDelegateForFunctionPointer(functionAddress, typeof(CreateInterfaceFn)); // Return Managed Delegate to Native CreateInterface(String, IntPtr) function
        }

        /// <summary>
        /// Creates Instance of <typeparamref name="TClass"/>, must have <see cref="ModuleNameAttribute"/>
        /// </summary>
        /// <typeparam name="TClass">Interface Wrapper/Representory Class</typeparam>
        /// <returns>Pointer to <typeparamref name="TClass"/></returns>
        public static IntPtr GetClassPointer<TClass>() where TClass : class
        {
            // Validate That class has a Module Name Attribute.
            if (!(typeof(TClass).GetCustomAttributes(typeof(ModuleNameAttribute), false).FirstOrDefault() is ModuleNameAttribute moduleName))
                throw new Exception($"{typeof(TClass).Name} does not have a ModuleName attribute.");

            // Call Native (SourceSDK) CreateInterface(String:InterfaceIdentifier, NULL)
            CreateInterfaceFn factory = LoadCreateInterface(moduleName.ModuleName);
            return factory(InterfaceVersions.GetInterfaceIdentifier(typeof(TClass)), IntPtr.Zero);
        }

        /// <summary>
        /// Creates Instance of <typeparamref name="TClass"/>, using a preloaded CreateInterface delegate from a Module in memory
        /// </summary>
        /// <typeparam name="TClass">Interface Wrapper/Representory Class</typeparam>
        /// <param name="factory">CreateInterface Delegate</param>
        /// <returns>Instance of <typeparamref name="TClass"/></returns>
        public static TClass GetFromFactory<TClass>(CreateInterfaceFn factory) where TClass : class
        {
            if (factory == null)
            {
                return null;
                //throw new JITEngineException("GetFromFactory called with NULL factory");
            }

            // Create Instance of TClass
            IntPtr classptr = factory(InterfaceVersions.GetInterfaceIdentifier(typeof(TClass)), IntPtr.Zero);
            // if not NULL cast Pointer to TClass via JIT Conversion. And Return
            return classptr == IntPtr.Zero ? null : JIT.ConvertInstance<TClass>(classptr);
        }

        /// <summary>
        /// this injections a new function pointer into a function,
        ///     it is recomended not to use this unless you have to.
        /// </summary>
        /// <returns>Managed Injection Handle, Reverts on Dispose</returns>
        public static unsafe ManagedMethodInjectionHandle Inject(MethodInfo methodToReplace, MethodInfo methodToInject)
        {
            RuntimeHelpers.PrepareMethod(methodToReplace.MethodHandle);
            RuntimeHelpers.PrepareMethod(methodToInject.MethodHandle);
            IntPtr tar = methodToReplace.MethodHandle.Value;
            if (!methodToReplace.IsVirtual)
                tar += 8;
            else
            {
                Int32 index = (Int32)(((*(Int64*)tar) >> 32) & 0xFF);
                IntPtr classStart = *(IntPtr*)(methodToReplace.DeclaringType.TypeHandle.Value + (IntPtr.Size == 4 ? 40 : 64));
                tar = classStart + (IntPtr.Size * index);
            }
            IntPtr inj = methodToInject.MethodHandle.Value + 8;
            ManagedMethodInjectionHandle state = new ManagedMethodInjectionHandle(tar, *(IntPtr*)tar);
            *(IntPtr*)tar = *(IntPtr*)inj;
            return state;
        }

        public static ManagedMethodInjectionHandle[] Inject(Type R, Type I)
        {
            List<ManagedMethodInjectionHandle> Outp = new List<ManagedMethodInjectionHandle>();
            foreach (MethodInfo RMI in R.GetMethods())
            {
				MethodInfo IMI = I.GetMethod(RMI.Name);
				if (IMI != null)
					if (RMI.ReturnType == IMI.ReturnType)
					{
						ParameterInfo[] RPI = RMI.GetParameters();
						ParameterInfo[] IPI = IMI.GetParameters();

						if (RPI.Length != IPI.Length)
							continue;
						Boolean Load = true;
						for (Int32 x = 0; x < RPI.Length; x++)
							if (RPI[x].ParameterType != IPI[x].ParameterType || RPI[x].IsOptional != IPI[x].IsOptional ||
								RPI[x].IsOut != IPI[x].IsOut || RPI[x].IsIn != IPI[x].IsIn)
							{
								Load = false;
								break;
							}
						if (Load)
							Outp.Add(Inject(RMI, IMI));
					}
            }
            return Outp.ToArray();
        }

        /// <summary>
        /// Load Interface <typeparamref name="TClass"/>, must have <see cref="ModuleNameAttribute"/>
        /// </summary>
        /// <typeparam name="TClass">Interface Wrapper/Representory Class</typeparam>
        /// <returns>Instance of <typeparamref name="TClass"/></returns>
        public static TClass Load<TClass>() where TClass : class
        {
            if (!(typeof(TClass).GetCustomAttributes(typeof(ModuleNameAttribute), false).FirstOrDefault() is ModuleNameAttribute moduleName))
                throw new Exception($"{typeof(TClass).Name} does not have a ModuleName attribute.");

            return Load<TClass>(moduleName.ModuleName);
        }

        /// <summary>
        /// Load Interface <typeparamref name="TClass"/>
        /// </summary>
        /// <typeparam name="TClass">Interface Wrapper/Representory Class</typeparam>
        /// <param name="dllname">Path or Name of DLL to load from</param>
        /// <returns>Instance of <typeparamref name="TClass"/></returns>
        public static TClass Load<TClass>(String dllname) where TClass : class => GetFromFactory<TClass>(LoadCreateInterface(dllname));

        /// <summary>
        /// Load Interface <typeparamref name="TClass"/>
        /// </summary>
        /// <typeparam name="TClass">Interface Wrapper/Representory Class</typeparam>
        /// <param name="dllname">Path or Name of DLL to load from</param>
        /// <param name="interfaceVersionString">Version of Interface to load</param>
        /// <returns>Instance of <typeparamref name="TClass"/></returns>
        public static TClass Load<TClass>(String dllname, String interfaceVersionString) where TClass : class
        {
            CreateInterfaceFn factory = LoadCreateInterface(dllname);
            IntPtr classptr = factory(interfaceVersionString, IntPtr.Zero);
            return JIT.ConvertInstance<TClass>(classptr);
        }

        public static void LoadStaticInterface(Type T)
        {
            if (!(T.GetCustomAttribute(typeof(StaticInterfaceAttribute)) is StaticInterfaceAttribute SI))
                throw new ArgumentException("Must be A Static Interface Class");

            switch (Natives.SystemType)
            {
                case System.Linux:
                    Inject(T, SI.lin);
                    break;

                case System.Windows:
                    Inject(T, SI.win);
                    break;

                case System.OSx:
                    Inject(T, SI.osx);
                    break;
            }
        }

        /// <summary>
        /// Get the Pointer of a Variable in DLL or LIB, will load it in if not allready loaded
        /// </summary>
        /// <param name="dllname">Path or Name of DLL to load from</param>
        /// <param name="variableName">name of global exported variable</param>
        /// <returns>Pointer to Variable</returns>
        public static IntPtr LoadVariable<T>(String dllname, String variableName) where T : struct => LoadVariable(dllname, variableName) - Marshal.SizeOf(typeof(T));

        /// <summary>
        /// Get the Pointer of a Variable in DLL or LIB, will load it in if not allready loaded
        /// </summary>
        /// <param name="dllname">Path or Name of DLL to load from</param>
        /// <param name="variableName">name of global exported variable</param>
        /// <returns>Pointer to Variable</returns>
        public static IntPtr LoadVariable(String dllname, String variableName)
        {
            IntPtr dllhandle = Natives.LoadDL(dllname);
            if (dllhandle == IntPtr.Zero)
                throw new DllNotFoundException();
            IntPtr varHandle = Natives.GetDLSymbol(dllhandle, variableName);
            return varHandle;
        }

        /// <summary>
        /// Overwrites VCR Hook. (SourceSDK) "tier0/vcrmode.h"
        /// </summary>
        /// <remarks>DO NOT USE; Unless you know what you are doing!</remarks>
        /// <typeparam name="T">VCR Hook Handler</typeparam>
        /// <param name="VCR">Pointer to VCR Struct</param>
        /// <param name="newDelegate">New VCR Hook Handler</param>
        /// <returns>Original or Old VCR Hook</returns>
        public static T OverwriteVCRHook<T>(IntPtr VCR, T newDelegate) where T : class
        {
            String hookName = typeof(T).Name;
            if (typeof(VCR_t).GetField(hookName) == null)
                throw new Exception("Could not find hook " + hookName);
            Int32 offset = (Int32)Marshal.OffsetOf<VCR_t>(hookName);
            T original = Marshal.GetDelegateForFunctionPointer<T>(Marshal.ReadIntPtr(VCR, offset));

            GCHandle.Alloc(newDelegate); // Fuck it.
            IntPtr newHookPointer = Marshal.GetFunctionPointerForDelegate(newDelegate);

            Marshal.WriteIntPtr(VCR, offset, newHookPointer);

            return original;
        }

        /// <summary>
        /// Overwrites VCR Hook. (SourceSDK) "tier0/vcrmode.h"
        /// </summary>
        /// <remarks>DO NOT USE; Unless you know what you are doing!</remarks>
        /// <typeparam name="T">VCR Hook Handler</typeparam>
        /// <param name="VCR">VCR Struct Pointer</param>
        /// <param name="newDelegate">New VCR Hook Handler</param>
        /// <returns>Original or Old VCR Hook</returns>
        public static T OverwriteVCRHook<T>(VCR_t* VCR, T newDelegate) where T : class => OverwriteVCRHook((IntPtr)VCR, newDelegate);

        /// <summary>
        /// Overwrites global 'g_pVCR' Hook. (SourceSDK) "tier0/vcrmode.h"
        /// </summary>
        /// <remarks>DO NOT USE; Unless you know what you are doing!</remarks>
        /// <typeparam name="T">VCR Hook Handler</typeparam>
        /// <param name="newDelegate">New VCR Hook Handler</param>
        /// <returns>Original or Old VCR Hook</returns>
        public static T OverwriteVCRHook<T>(T newDelegate) where T : class => OverwriteVCRHook(LoadVariable<VCR_t>(Natives.Tier0Library, "g_pVCR"), newDelegate);

        public class ManagedMethodInjectionHandle : IDisposable
        {
            public readonly IntPtr Location;
            public readonly IntPtr Original;

            public ManagedMethodInjectionHandle(IntPtr Location, IntPtr OriginalValue)
            {
                this.Location = Location;
                this.Original = OriginalValue;
            }

            public unsafe void Dispose() => *(IntPtr*)this.Location = this.Original;
        }
    }

    public class NotLoadedException : Exception
    {
        public NotLoadedException() : base("this function has not been imported")
        {
        }
    }

    public class StaticInterfaceAttribute : Attribute
    {
        public readonly Type lin;
        public readonly Type osx;
        public readonly Type win;

        public StaticInterfaceAttribute(Type win, Type lin, Type osx = null)
        {
            this.win = win;
            this.lin = lin;
            this.osx = osx;
        }
    }
}
