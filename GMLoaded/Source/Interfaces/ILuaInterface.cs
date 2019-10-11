using System.ComponentModel;
using System.Runtime.InteropServices;
using System;

namespace GMLoaded.Source.Interaces
{
    public interface ILuaInterface : ILuaBase
    {
        /// <param name='param0'></param>
        IntPtr AddThreadedCall(IntPtr param0);

        /// <param name='param0'></param>
        /// <param name='param1'></param>
        void AppendStackTrace(IntPtr param0, Int32 param1);

        /// <param name='param0'></param>
        /// <param name='param1'></param>
        /// <param name='param2'></param>
        Boolean CallFunctionProtected(Int32 param0, Int32 param1, Boolean param2);

        /// <param name='args'></param>
        /// <param name='rets'></param>
        void CallInternal(Int32 args, Int32 rets);

        /// <param name='args'></param>
        /// <param name='obj'></param>
        Boolean CallInternalGet(Int32 args, IntPtr obj);

        /// <param name='args'></param>
        Boolean CallInternalGetBool(Int32 args);

        /// <param name='args'></param>
        String CallInternalGetString(Int32 args);

        /// <param name='args'></param>
        void CallInternalNoReturns(Int32 args);

        /// <param name='dumper'></param>
        /// <param name='stringToCompile'></param>
        void CompileString(IntPtr dumper, IntPtr stringToCompile);

        /// <param name='param0'></param>
        /// <param name='param1'></param>
        /// <param name='param2'></param>
        /// <param name='param3'></param>
        /// <param name='param4'></param>
        IntPtr CreateConCommand(String param0, String param1, Int32 param2, IntPtr param3, IntPtr param4);

        /// <param name='param0'></param>
        /// <param name='param1'></param>
        /// <param name='param2'></param>
        /// <param name='param3'></param>
        IntPtr CreateConVar(String param0, String param1, String param2, Int32 param3);

        IntPtr CreateObject();

        void Cycle();

        /// <param name='obj'></param>
        void DestroyObject(IntPtr obj);

        /// <param name='err'></param>
        void Error(String err);

        /// <param name='fmt'></param>
        void ErrorFromLua(String fmt);

        /// <param name='fmt'></param>
        void ErrorNoHalt(String fmt);

        /// <param name='filename'></param>
        /// <param name='run'></param>
        /// <param name='showErrors'></param>
        /// <param name='param3'></param>
        /// <param name='param4'></param>
        Boolean FindAndRunScript(String filename, Boolean run, Boolean showErrors, String param3, Boolean param4);

        /// <param name='tableIndex'></param>
        /// <param name='keyIndex'></param>
        Boolean FindObjectOnTable(Int32 tableIndex, Int32 keyIndex);

        /// <param name='objIndex'></param>
        /// <param name='keyIndex'></param>
        Boolean FindOnObjectsMetaTable(Int32 objIndex, Int32 keyIndex);

        /// <param name='type'></param>
        String GetActualTypeName(Int32 type);

        /// <param name='index'></param>
        Int32 GetColor(Int32 index);

        /// <param name='outStr'></param>
        void GetCurrentFile(IntPtr outStr);

        IntPtr GetCurrentLocation();

        /// <param name='index'></param>
        /// <param name='str'></param>
        IntPtr GetDataString(Int32 index, IntPtr str);

        /// <param name='index'></param>
        Int32 GetFlags(Int32 index);

        /// <param name='what'></param>
        /// <param name='dbg'></param>
        Int32 GetInfo(String what, IntPtr dbg);

        /// <param name='dbg'></param>
        /// <param name='n'></param>
        String GetLocal(IntPtr dbg, Int32 n);

        /// <param name='name'></param>
        /// <param name='type'></param>
        IntPtr GetMetaTableObject(String name, Int32 type);

        /// <param name='index'></param>
        IntPtr GetMetaTableObject(Int32 index);

        void GetNewTable();

        /// <param name='index'></param>
        IntPtr GetObject(Int32 index);

        String GetPath();

        String GetPathID();

        /// <param name='index'></param>
        String GetPooledString(Int32 index);

        /// <param name='index'></param>
        IntPtr GetReturn(Int32 index);

        /// <param name='level'></param>
        /// <param name='dbg'></param>
        Int32 GetStack(Int32 level, IntPtr dbg);

        /// <param name='index'></param>
        String GetStringOrError(Int32 index);

        /// <param name='funcIndex'></param>
        /// <param name='n'></param>
        String GetUpvalue(Int32 funcIndex, Int32 n);

        IntPtr Global();

        /// <param name='param0'></param>
        /// <param name='param1'></param>
        Boolean Init(IntPtr param0, Boolean param1);

        Boolean IsClient();

        /// <param name='objA'></param>
        /// <param name='objB'></param>
        Boolean IsEqual(IntPtr objA, IntPtr objB);

        Boolean IsMenu();

        Boolean IsServer();

        /// <param name='index'></param>
        Boolean isUserData(Int32 index);

        /// <param name='err'></param>
        /// <param name='index'></param>
        void LuaError(String err, Int32 index);

        /// <param name='fmt'></param>
        void Msg(String fmt);

        /// <param name='col'></param>
        /// <param name='fmt'></param>
        void MsgColour(IntPtr col, String fmt);

        /// <param name='name'></param>
        void NewGlobalTable(String name);

        IntPtr NewTemporaryObject();

        void PopPath();

        /// <param name='arrelems'></param>
        /// <param name='nonarrelems'></param>
        void PreCreateTable(Int32 arrelems, Int32 nonarrelems);

        /// <param name='color'></param>
        void PushColor(IntPtr color);

        /// <param name='num'></param>
        void PushLong(Int32 num);

        /// <param name='func'></param>
        void PushLuaFunction(IntPtr func);

        /// <param name='obj'></param>
        void PushLuaObject(IntPtr obj);

        /// <param name='path'></param>
        void PushPath(String path);

        /// <param name='index'></param>
        void PushPooledString(Int32 index);

        /// <param name='name'></param>
        void Require(String name);

        /// <param name='name'></param>
        Boolean RunLuaModule(String name);

        /// <param name='filename'></param>
        /// <param name='path'></param>
        /// <param name='stringToRun'></param>
        /// <param name='run'></param>
        /// <param name='showErrors'></param>
        Boolean RunString(String filename, String path, String stringToRun, Boolean run, Boolean showErrors);

        /// <param name='filename'></param>
        /// <param name='path'></param>
        /// <param name='stringToRun'></param>
        /// <param name='run'></param>
        /// <param name='printErrors'></param>
        /// <param name='dontPushErrors'></param>
        /// <param name='noReturns'></param>
        Boolean RunStringEx(String filename, String path, String stringToRun, Boolean run, Boolean printErrors, Boolean dontPushErrors, Boolean noReturns);

        /// <param name='table'></param>
        /// <param name='key'></param>
        /// <param name='value'></param>
        void SetMember(IntPtr table, IntPtr key, IntPtr value);

        /// <param name='table'></param>
        /// <param name='key'></param>
        void SetMember(IntPtr table, Single key);

        /// <param name='table'></param>
        /// <param name='key'></param>
        /// <param name='value'></param>
        void SetMember(IntPtr table, Single key, IntPtr value);

        /// <param name='table'></param>
        /// <param name='key'></param>
        void SetMember(IntPtr table, String key);

        /// <param name='table'></param>
        /// <param name='key'></param>
        /// <param name='value'></param>
        void SetMember(IntPtr table, String key, IntPtr value);

        /// <param name='table'></param>
        /// <param name='keyIndex'></param>
        /// <param name='valueIndex'></param>
        void SetMemberFast(IntPtr table, Int32 keyIndex, Int32 valueIndex);

        /// <param name='pathID'></param>
        void SetPathID(String pathID);

        /// <param name='param0'></param>
        void SetType(Byte param0);

        void Shutdown();

        /// <param name='name'></param>
        /// <param name='index'></param>
        void TypeError(String name, Int32 index);
    }
}
