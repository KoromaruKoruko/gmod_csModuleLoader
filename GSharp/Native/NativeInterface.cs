using GSharp.Attributes;
using GSharp.Generated.NativeClasses;
using GSharp.Native.Classes;

using Libraria.Native;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

// NOTE: SourceSDK => "https://github.com/ValveSoftware/source-sdk-2013"
// NOTE: Found loaded definitions for Kernel32 Imports, redirecting functions to pre existing Kernel32 Class definitions

namespace GSharp.Native
{
    /// <summary>
    /// Interface for Native to Managed operations
    /// </summary>
    public static unsafe class NativeInterface
    {
        /// <summary>
        /// Cache of allready Loaded Library Pointers, Optimizations are good ya know.
        /// </summary>
        private static readonly Dictionary<String, IntPtr> LibraryCache = new Dictionary<String, IntPtr>();

        /*
        /// <summary>
        /// Gets a Module's Pointer by FullyQualifiedName
        /// </summary>
        /// <param name="path">FullyQualifiedName of Module</param>
        /// <remarks>Error is in <see cref="Marshal.GetLastWin32Error()"/> if failed</remarks>
        /// <returns>Module's Pointer</returns>
        [DllImport("kernel32")]
        private static extern IntPtr GetModuleHandle(String path);
        */

        /// <summary>
        /// Get CreateInterface Function from DLL
        /// </summary>
        /// <param name="dllPath">Path to DLL file</param>
        /// <returns>Native (SourceSDK) CreateInterface(String)</returns>
        private static CreateInterfaceFn LoadCreateInterface(String dllPath)
        {
            // Load DLL into Memory
            IntPtr hModule = LoadLibrary(dllPath);
            if (hModule == IntPtr.Zero)
                return null;

            // Get Native CreateInterface(String, IntPtr) function
            //IntPtr functionAddress = GetProcAddress(hModule, "CreateInterface");
            IntPtr functionAddress = Kernel32.GetProcAddress(hModule, "CreateInterface");
            return functionAddress == IntPtr.Zero
                ? null
                : (CreateInterfaceFn)Marshal.GetDelegateForFunctionPointer(functionAddress, typeof(CreateInterfaceFn)); // Return Managed Delegate to Native CreateInterface(String, IntPtr) function
        }

        /*
        /// <summary>
        /// Loads a DLL or LIB into memory and return Module (DLL|LIB) Pointer
        /// </summary>
        /// <param name="path">Path to DLL or LIB</param>
        /// <remarks>Error is in <see cref="Marshal.GetLastWin32Error()"/> if failed</remarks>
        /// <returns>Module Pointer</returns>
        [DllImport("kernel32", EntryPoint = "LoadLibrary")]
        private static extern IntPtr LoadLibraryInternal(String path);
        */

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

        /*
        /// <summary>
        /// Gets the Function Address inside a Module
        /// </summary>
        /// <param name="hModule">Module Pointer</param>
        /// <param name="procName">Function Name</param>
        /// <remarks>Error is in <see cref="Marshal.GetLastWin32Error()"/> if failed</remarks>
        /// <returns>Address of Function</returns>
        [DllImport("kernel32")]
        public static extern IntPtr GetProcAddress(IntPtr hModule, String procName);
        */

        /// <summary>
        /// Load Interface <typeparamref name="TClass"/>, must have <see cref="ModuleNameAttribute"/>
        /// </summary>
        /// <typeparam name="TClass">Interface Wrapper/Representory Class</typeparam>
        /// <returns>Instance of <typeparamref name="TClass"/></returns>
        public static TClass Load<TClass>() where TClass : class
        {
            if (!(typeof(TClass).GetCustomAttributes(typeof(ModuleNameAttribute), false).FirstOrDefault() is ModuleNameAttribute moduleName))
            {
                throw new Exception($"{typeof(TClass).Name} does not have a ModuleName attribute.");
            }

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
            // Call Native (SourceSDK) CreateInterface(String:InterfaceIdentifier, NULL)
            // And parse to TClass via JIT Conversion.
            CreateInterfaceFn factory = LoadCreateInterface(dllname);
            IntPtr classptr = factory(interfaceVersionString, IntPtr.Zero);
            return JIT.ConvertInstance<TClass>(classptr);
        }

        /// <summary>
        /// Loads a DLL or LIB into memory and return Module (DLL|LIB) Pointer
        /// </summary>
        /// <param name="path">Path to DLL or LIB</param>
        /// <returns>Module Pointer</returns>
        public static IntPtr LoadLibrary(String path)
        {
            if (LibraryCache.ContainsKey(path)) return LibraryCache[path];

            //IntPtr handle = GetModuleHandle(path);
            IntPtr handle = Kernel32.GetModuleHandle(path);
            if (handle == IntPtr.Zero)
                //handle = LoadLibraryInternal(path);
                handle = Kernel32.LoadLibrary(path);
            if (handle == IntPtr.Zero)
                return IntPtr.Zero;

            LibraryCache.Add(path, handle);
            return handle;
        }

        /// <summary>
        /// Get the Pointer of a Variable in DLL or LIB, will load it in if not allready loaded
        /// </summary>
        /// <remarks>Returns the END of the variable, to get the START use <see cref="LoadVariable{T}(String, String)"/></remarks>
        /// <param name="dllname">Path or Name of DLL to load from</param>
        /// <param name="variableName">name of global exported variable</param>
        /// <returns>Pointer to Variable</returns>
        public static IntPtr LoadVariable(String dllname, String variableName)
        {
            IntPtr dllhandle = LoadLibrary(dllname);
            //IntPtr varHandle = GetProcAddress(dllhandle, variableName);
            IntPtr varHandle = Kernel32.GetProcAddress(dllhandle, variableName);
            return varHandle;
        }

        /// <summary>
        /// Get the Pointer of a Variable in DLL or LIB, will load it in if not allready loaded
        /// </summary>
        /// <param name="dllname">Path or Name of DLL to load from</param>
        /// <param name="variableName">name of global exported variable</param>
        /// <returns>Pointer to Variable</returns>
        public static IntPtr LoadVariable<T>(String dllname, String variableName) where T : struct => LoadVariable(dllname, variableName) - Marshal.SizeOf(typeof(T));

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
            GCHandle.Alloc(newDelegate); // Fuck it.

            String hookName = typeof(T).Name;
            if (typeof(VCR_t).GetField(hookName) == null)
                throw new Exception("Could not find hook " + hookName);
            Int32 offset = (Int32)Marshal.OffsetOf<VCR_t>(hookName);

            T original = Marshal.GetDelegateForFunctionPointer<T>(Marshal.ReadIntPtr(VCR, offset));
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
        public static T OverwriteVCRHook<T>(T newDelegate) where T : class => OverwriteVCRHook(LoadVariable<VCR_t>("tier0.dll", "g_pVCR"), newDelegate);
    }
}
