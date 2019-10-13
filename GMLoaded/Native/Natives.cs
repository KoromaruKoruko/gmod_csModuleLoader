using System;
using System.Runtime.InteropServices;

namespace GMLoaded.Native
{
    public static partial class Natives
    {
        public static Boolean IsX64 { get; private set; }
        public static String LibraryTier0Name { get; private set; }
        public static System SystemType { get; private set; }

        public static void FreeDL(IntPtr DLHandle)
        {
            switch (SystemType)
            {
                case System.Linux:
                    Libdl.DLclose(DLHandle);
                    break;

                case System.Windows:
                    Kernel32.FreeLibrary(DLHandle);
                    break;
            }
        }

        public static IntPtr GetDLSymbol(IntPtr DLHandle, String Symbol) => SystemType switch
        {
            System.Linux => Libdl.DLsym(DLHandle, Symbol),
            System.Windows => Kernel32.GetProcAddress(DLHandle, Symbol),
            _ => IntPtr.Zero,
        };

        public static void Init(Boolean IsX64, System SystemType)
        {
            // TODO: Make this inject native handler functions to remove a few lines of code during runtime
            Natives.IsX64 = IsX64;
            Natives.SystemType = SystemType;

            LibraryTier0Name = SystemType switch
            {
                System.Windows => "tier0.dll",
                System.OSx => "libtier0.so",
                _ => null,
            };
        }

        public static IntPtr LoadDL(String DLPath)
        {
            IntPtr Ret = IntPtr.Zero;
            switch (SystemType)
            {
                case System.Linux:
                    Ret = Libdl.DLopen(DLPath, Libdl.Dlopen_Flags.RTLD_LAZY | Libdl.Dlopen_Flags.RTLD_GLOBAL);
                    break;

                case System.Windows:
                    Ret = Kernel32.GetModuleHandle(DLPath);
                    if (Ret == IntPtr.Zero)
                        Ret = Kernel32.LoadLibrary(DLPath);
                    break;
            }
            return Ret;
        }

        /// <summary>
        /// General Native Windows API
        /// </summary>
        public static class Kernel32
        {
            [Flags]
            public enum AllocationType : UInt32
            {
                COMMIT = 0x1000,
                RESERVE = 0x2000,
                DECOMMIT = 0x4000,
                RELEASE = 0x8000,
                RESET = 0x80000,
                LARGE_PAGES = 0x20000000,
                PHYSICAL = 0x400000,
                TOP_DOWN = 0x100000,
                WRITE_WATCH = 0x200000
            }

            [Flags]
            public enum MemoryProtection : UInt32
            {
                EXECUTE = 0x10,
                EXECUTE_READ = 0x20,
                EXECUTE_READWRITE = 0x40,
                EXECUTE_WRITECOPY = 0x80,
                NOACCESS = 0x01,
                READONLY = 0x02,
                READWRITE = 0x04,
                WRITECOPY = 0x08,
                GUARD_Modifierflag = 0x100,
                NOCACHE_Modifierflag = 0x200,
                WRITECOMBINE_Modifierflag = 0x400
            }

            [DllImport("kernel32")]
            public static extern Boolean AllocConsole();

            /// <summary>
            /// Close/Unload an open dl
            /// </summary>
            /// <param name="DlPtr"><see cref="LoadLibrary"/></param>
            /// <returns>If the Library was Free'd</returns>
            [DllImport("kernel32.dll")]
            public static extern Boolean FreeLibrary(IntPtr DlPtr);

            [DllImport("kernel32")]
            public static extern IntPtr GetConsoleWindow();

            [DllImport("kernel32", SetLastError = true, CallingConvention = CallingConvention.Winapi)]
            public static extern IntPtr GetModuleHandle(String ModuleName);

            /// <summary>
            /// Get a Symbol
            /// </summary>
            /// <param name="DlPtr"><see cref="LoadLibrary"/></param>
            /// <param name="Symbol">Symbol to get</param>
            /// <returns>Pointer of Symbol or NULL</returns>
            [DllImport("kernel32.dll")]
            public static extern IntPtr GetProcAddress(IntPtr DlPtr, String Symbol);

            /// <summary>
            /// Load Library
            /// </summary>
            /// <param name="DllPath">Path to the Library</param>
            /// <returns>DlPtr or NULL</returns>
            [DllImport("Kernel32", SetLastError = true, CallingConvention = CallingConvention.Winapi)]
            public static extern IntPtr LoadLibrary(String DllPath);

            [DllImport("kernel32")]
            public static extern IntPtr RtlPcToFileHeader(IntPtr PC, out IntPtr Base);

            [DllImport("kernel32", SetLastError = true)]
            public static extern IntPtr VirtualAlloc(IntPtr Addr, IntPtr Size,
                AllocationType AllocType = AllocationType.COMMIT, MemoryProtection MemProtect = MemoryProtection.EXECUTE_READWRITE);

            [DllImport("kernel32", SetLastError = true)]
            public static extern Boolean VirtualFree(IntPtr Addr, IntPtr Size, AllocationType FreeType);

            public static Boolean VirtualFree(IntPtr Addr) => VirtualFree(Addr, IntPtr.Zero, AllocationType.RELEASE);

            [DllImport("kernel32", SetLastError = true, CallingConvention = CallingConvention.Winapi)]
            public static extern Boolean VirtualProtect(IntPtr Addr, UInt32 Size, MemoryProtection NewProtect, out MemoryProtection OldProtect);

            public static Boolean VirtualProtect(IntPtr Addr, Int32 Size, MemoryProtection NewProtect, out MemoryProtection OldProtect) => VirtualProtect(Addr, (UInt32)Size, NewProtect, out OldProtect);

            public static Boolean VirtualProtect(IntPtr Addr, UInt32 Size, MemoryProtection NewProtect) => VirtualProtect(Addr, Size, NewProtect, out MemoryProtection Old);

            public static Boolean VirtualProtect(IntPtr Addr, Int32 Size, MemoryProtection NewProtect) => VirtualProtect(Addr, (UInt32)Size, NewProtect);
        }

        /// <summary>
        /// Linux GNU C++ Stdandard Library
        /// </summary>
        public static class Libc
        {
            [Flags]
            public enum Madvflags
            {
                MADV_NORMAL = 0,
                MADV_RANDOM = 1,
                MADV_SEQUENTIAL = 2,
                MADV_WILLNEED = 3,
                MADV_DONTNEED = 4,
            }

            [Flags]
            public enum Mapflags
            {
                MAP_FILE = 0x0001,
                MAP_ANON = 0x0002,
                MAP_TYPE = 0x000f,
                MAP_COPY = 0x0020,
                MAP_SHARED = 0x0010,
                MAP_PRIVATE = 0x0000,
                MAP_FIXED = 0x0100,
                MAP_NOEXTENDED = 0x0200,
                MAP_HASSEMPHORE = 0x0400,
                MAP_INHERIT = 0x0800,
            }

            [Flags]
            public enum Protflags : Int32
            {
                PROT_READ = 0x04,
                PROT_WRITE = 0x02,
                PROT_EXEC = 0x01,
                PROT_NONE = 0x00,
            }

            [DllImport("libc.so", EntryPoint = "mprotect")]
            public static extern Int32 MProtect(IntPtr StartAddress, Int32 Length, Protflags falgs);

            [DllImport("libc.so", EntryPoint = "pkey_mprotect")]
            public static extern Int32 PKey_MProtect(IntPtr StartAddress, Int32 Length, Protflags falgs, Int32 PKey);
        }

        /// <summary>
        /// Linux Library loader, reader and unloader
        /// </summary>
        public static class Libdl
        {
            //https://github.molgen.mpg.de/git-mirror/glibc/blob/master/sysdeps/mips/bits/dlfcn.h (10.10.2019 DD/MM/YYYY)
            [Flags]
            public enum Dlopen_Flags : Int32
            {
                /// <summary>
                /// Perform lazy binding. Only resolve symbols as the code that references them is executed. If the symbol is never referenced, then it is never resolved. (Lazy binding is only performed for function references; references to variables are always immediately bound when the library is loaded.)
                /// </summary>
                RTLD_LAZY = 1,

                /// <summary>
                /// If this value is specified, or the environment variable LD_BIND_NOW is set to a nonempty string, all undefined symbols in the library are resolved before dlopen() returns. If this cannot be done, an error is returned.
                /// </summary>
                RTLD_NOW = 2,

                /// <summary>
                /// The symbols defined by this library will be made available for symbol resolution of subsequently loaded libraries.
                /// </summary>
                RTLD_GLOBAL = 256,

                /// <summary>
                /// This is the converse of RTLD_GLOBAL, and the default if neither flag is specified. Symbols defined in this library are not made available to resolve references in subsequently loaded libraries.
                /// </summary>
                RTLD_LOCAL = 0,

                /// <summary>
                /// Do not unload the library during dlclose(). Consequently, the library's static variables are not reinitialized if the library is reloaded with dlopen() at a later time. This flag is not specified in POSIX.1-2001.
                /// </summary>
                RTLD_NODELETE = 4096,

                /// <summary>
                /// Don't load the library. This can be used to test if the library is already resident (dlopen() returns NULL if it is not, or the library's handle if it is resident). This flag can also be used to promote the flags on a library that is already loaded. For example, a library that was previously loaded with RTLD_LOCAL can be reopened with RTLD_NOLOAD | RTLD_GLOBAL. This flag is not specified in POSIX.1-2001.
                /// </summary>
                RTLD_NOLOAD = 4,

                /// <summary>
                /// Place the lookup scope of the symbols in this library ahead of the global scope. This means that a self-contained library will use its own symbols in preference to global symbols with the same name contained in libraries that have already been loaded. This flag is not specified in POSIX.1-2001.
                /// </summary>
                RTLD_DEEPBIND = 8,

                /// <summary>
                /// Unknown,
                /// </summary>
                RTLD_BINDING_MASK = 0x3,
            }

            /// <summary>
            /// Close/unload an open dl (if other librarys no longer use it)
            /// </summary>
            /// <param name="DlPtr"><see cref="Dlopen"/></param>
            /// <returns>returns 0 or error code</returns>
            [DllImport("libdl", EntryPoint = "dlclose")]
            public static extern Int32 DLclose(IntPtr DlPtr);

            /// <summary>
            /// Get last Error
            /// </summary>
            /// <returns>String Pointer or NULL</returns>
            [DllImport("libdl", EntryPoint = "dlerror")]
            public static extern IntPtr DLerror();

            /// <summary>
            /// Open a Library File
            /// </summary>
            /// <param name="FilePath">Path to Library</param>
            /// <param name="Flags"><see cref="Dlopen_Flags"/></param>
            /// <returns>DL Pointer or NULL</returns>
            [DllImport("libdl", EntryPoint = "dlopen")]
            public static extern IntPtr DLopen(String FilePath, Dlopen_Flags Flags);

            /// <summary>
            /// Gets a pointer to a specific Symbol within a loaded DL
            /// </summary>
            /// <param name="DlPtr"><see cref="Dlopen"/></param>
            /// <param name="Symbol">Name of the exported item</param>
            /// <returns>Pointer to item or NULL</returns>
            [DllImport("libdl", EntryPoint = "dlsym")]
            public static extern IntPtr DLsym(IntPtr DlPtr, String Symbol);
        }
    }
}
