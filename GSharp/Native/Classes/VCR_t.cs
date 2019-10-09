using System;
using System.Runtime.InteropServices;
namespace GSharp.Native.Classes
{
    namespace VCR
    {
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public unsafe delegate Double Hook_Sys_FloatTime(Double time);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public unsafe delegate Int32 Hook_PeekMessage(IntPtr msg, IntPtr hWnd, UInt32 wMsgFilterMin, UInt32 wMsgFilterMax, UInt32 wRemoveMsg);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public unsafe delegate void Hook_RecordGameMsg(IntPtr InputEvent_t);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public unsafe delegate void Hook_RecordEndGameMsg();
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public unsafe delegate Boolean Hook_PlaybackGameMsg(IntPtr pEvent);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public unsafe delegate Int32 Hook_recvfrom(Int32 s, Byte* buf, Int32 len, Int32 flags, IntPtr from, IntPtr fromlen);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public unsafe delegate void Hook_GetCursorPos(IntPtr pt);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public unsafe delegate void Hook_ScreenToClient(IntPtr hWnd, IntPtr pt);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public unsafe delegate void Hook_Cmd_Exec(String[] Args);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public unsafe delegate String Hook_GetCommandLine();
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public unsafe delegate Int64 Hook_RegOpenKeyEx(IntPtr hKey, String lpSubKey, UInt64 ulOptions, UInt64 samDesired, IntPtr pHKey);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public unsafe delegate Int64 Hook_RegSetValueEx(IntPtr hKey, String lpValueName, UInt64 Reserved, UInt64 dwType, IntPtr lpData, UInt64 cbData);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public unsafe delegate Int64 Hook_RegQueryValueEx(IntPtr hKey, String lpValueName, UInt64* lpReserved, UInt64* lpType, Byte* lpData, UInt64* lpcbData);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public unsafe delegate Int64 Hook_RegCreateKeyEx(IntPtr hKey, String lpSubKey, UInt64 Reserved, String lpClass, UInt64 dwOptions, UInt64 samDesired, IntPtr lpSecurityAttributes, IntPtr phkResult, UInt64* lpdwDisposition);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public unsafe delegate void Hook_RegCloseKey(IntPtr hKey);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public unsafe delegate Int32 Hook_GetNumberOfConsoleInputEvents(IntPtr hInput, UInt64* pNumEvents);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public unsafe delegate Int32 Hook_ReadConsoleInput(IntPtr hInput, IntPtr pRecs, Int32 nMaxRecs, UInt64* pNumRead);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public unsafe delegate void Hook_LocalTime(IntPtr today);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public unsafe delegate Int16 Hook_GetKeyState(Int32 nVirtKey);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public unsafe delegate Int32 Hook_recv(Int32 s, Char* buf, Int32 len, Int32 flags);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public unsafe delegate Int32 Hook_send(Int32 s, String buf, Int32 len, Int32 flags);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public unsafe delegate IntPtr Hook_CreateThread(IntPtr lpThreadAttributes, UInt64 dwStackSize, IntPtr lpStartAddress, IntPtr lpParameter, UInt64 dwCreationFlags, UInt64* lpThreadID);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public unsafe delegate UInt64 Hook_WaitForSingleObject(IntPtr handle, UInt64 dwMilliseconds);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public unsafe delegate void Hook_EnterCriticalSection(IntPtr pCS);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public unsafe delegate void Hook_Time(Int64* pTime);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public unsafe delegate UInt64 Hook_WaitForMultipleObjects(UInt32 nHandles, IntPtr pHandles, Int32 bWaitAll, UInt32 timeout);
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct VCR_t
    {
        public IntPtr Start;
        public IntPtr End;
        public IntPtr GetVCRTraceInterface;
        public IntPtr GetMode;
        public IntPtr SetEnabled;
        public IntPtr SyncToken;

        public IntPtr Hook_Sys_FloatTime;
        public IntPtr Hook_PeekMessage;
        public IntPtr Hook_RecordGameMsg;
        public IntPtr Hook_RecordEndGameMsg;
        public IntPtr Hook_PlaybackGameMsg;
        public IntPtr Hook_recvfrom;
        public IntPtr Hook_GetCursorPos;
        public IntPtr Hook_ScreenToClient;
        public IntPtr Hook_Cmd_Exec;
        public IntPtr Hook_GetCommandLine;
        public IntPtr Hook_RegOpenKeyEx;
        public IntPtr Hook_RegSetValueEx;
        public IntPtr Hook_RegQueryValueEx;
        public IntPtr Hook_RegCreateKeyEx;
        public IntPtr Hook_RegCloseKey;
        public IntPtr Hook_GetNumberOfConsoleInputEvents;
        public IntPtr Hook_ReadConsoleInput;
        public IntPtr Hook_LocalTime;
        public IntPtr Hook_GetKeyState;
        public IntPtr Hook_recv;
        public IntPtr Hook_send;

        public IntPtr GenericRecord;
        public IntPtr GenericPlayback;
        public IntPtr GenericValue;
        public IntPtr GetPercentCompleted;

        public IntPtr Hook_CreateThread;
        public IntPtr Hook_WaitForSingleObject;
        public IntPtr Hook_EnterCriticalSection;
        public IntPtr Hook_Time;

        public IntPtr GenericString;
        public IntPtr GenericValueVerify;

        public IntPtr Hook_WaitForMultipleObjects;

        public T OverwriteHook<T>(T NewDelegate) where T : class
        {
            fixed (VCR_t* ThisFixed = &this)
                return NativeInterface.OverwriteVCRHook<T>(ThisFixed, NewDelegate);
        }
    }
}
