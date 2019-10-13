using System;
using System.Runtime.InteropServices;
using GMLoaded.Native;
using GMLoaded;
using GMLoaded.Lua;
using System.Text;

namespace ModuleExample
{
    public static class ModuleExportInterface
    {
        [return: MarshalAs(UnmanagedType.I4)]
        public static Int32 Close32([MarshalAs(UnmanagedType.I4)]Int32 Ptr)
        {
            try
            {
                GLua Lua = GLua.Get(Marshal.PtrToStructure<Lua_State32>(new IntPtr(Ptr)));
                Int32 Ret = Module.Close(Lua);
                Lua.Close();
                return Ret;
            }
            catch (Exception E)
            {
                WriteException(E);
                return -1;
            }
        }

        [return: MarshalAs(UnmanagedType.I4)]
        public static Int32 Close64([MarshalAs(UnmanagedType.I8)]Int64 Ptr)
        {
            try
            {
                GLua Lua = GLua.Get(Marshal.PtrToStructure<Lua_State64>(new IntPtr(Ptr)));
                Int32 Ret = Module.Close(Lua);
                Lua.Close();
                return Ret;
            }
            catch (Exception E)
            {
                WriteException(E);
                return -1;
            }
        }

        public static void Define(
            [MarshalAs(UnmanagedType.Bool)] Boolean isX64,
            [MarshalAs(UnmanagedType.U1)] Byte Syscode
            ) => Natives.Init(isX64, (GMLoaded.System)Syscode);

        [return: MarshalAs(UnmanagedType.I4)]
        public static Int32 Open32([MarshalAs(UnmanagedType.I4)]Int32 Ptr)
        {
            try
            {
                return Module.Open(GLua.Get(Marshal.PtrToStructure<Lua_State32>(new IntPtr(Ptr))));
            }
            catch (Exception E)
            {
                WriteException(E);
                return -1;
            }
        }

        [return: MarshalAs(UnmanagedType.I4)]
        public static Int32 Open64([MarshalAs(UnmanagedType.I8)]Int64 Ptr)
        {
            try
            {
                return Module.Open(GLua.Get(Marshal.PtrToStructure<Lua_State64>(new IntPtr(Ptr))));
            }
            catch (Exception E)
            {
                WriteException(E);
                return -1;
            }
        }

        public static void Write(String Text) => global::System.IO.File.AppendAllText("csModuleErrLog.Log", $"\n{DateTime.Now.ToString()}\n{Text}\n");

        public static void WriteException(Exception E)
        {
            StringBuilder Builder = new StringBuilder($"\n{DateTime.Now.ToString()}");

            String Indent = "";
            void WriteE(Exception E)
            {
                Builder.Append($"\n{Indent}Exception:{E.Message}\n{Indent}Type:{E.GetType()}");
                Indent += " ";
                Builder.Append($"\n{Indent}{E.StackTrace}\n");
                if (E.InnerException != null)
                    WriteE(E.InnerException);
                Indent = Indent.Substring(1);
            }
            WriteE(E);

            global::System.IO.File.AppendAllText("csModuleErrLog.Log", Builder.ToString());
        }
    }
}
