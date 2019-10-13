using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace GMLoaded.Lua
{
    public interface ILuaBase
    {

        Int32 Top();

        /// <param name='iStackPos'></param>
        void Push(Int32 iStackPos);

        /// <param name='iAmt'>Default: 1</param>
        void Pop([Optional()] [DefaultValue(1)] Int32 iAmt);

        /// <param name='iStackPos'></param>
        void GetTable(Int32 iStackPos);

        /// <param name='iStackPos'></param>
        /// <param name='strName'></param>
        void GetField(Int32 iStackPos, String strName);

        /// <param name='iStackPos'></param>
        /// <param name='strName'></param>
        void SetField(Int32 iStackPos, String strName);

        void CreateTable();

        /// <param name='iStackPos'></param>
        void SetTable(Int32 iStackPos);

        /// <param name='iStackPos'></param>
        void SetMetaTable(Int32 iStackPos);

        /// <param name='i'></param>
        Boolean GetMetaTable(Int32 i);

        /// <param name='iArgs'></param>
        /// <param name='iResults'></param>
        void Call(Int32 iArgs, Int32 iResults);

        /// <param name='iArgs'></param>
        /// <param name='iResults'></param>
        /// <param name='iErrorFunc'></param>
        Int32 PCall(Int32 iArgs, Int32 iResults, Int32 iErrorFunc);

        /// <param name='iA'></param>
        /// <param name='iB'></param>
        Int32 Equal(Int32 iA, Int32 iB);

        /// <param name='iA'></param>
        /// <param name='iB'></param>
        Int32 RawEqual(Int32 iA, Int32 iB);

        /// <param name='iStackPos'></param>
        void Insert(Int32 iStackPos);

        /// <param name='iStackPos'></param>
        void Remove(Int32 iStackPos);

        /// <param name='iStackPos'></param>
        Int32 Next(Int32 iStackPos);

        /// <param name='iSize'></param>
        IntPtr NewUserdata(UInt32 iSize);

        /// <param name='strError'></param>
        void ThrowError(String strError);

        /// <param name='iStackPos'></param>
        /// <param name='iType'></param>
        void CheckType(Int32 iStackPos, Int32 iType);

        /// <param name='iArgNum'></param>
        /// <param name='strMessage'></param>
        void ArgError(Int32 iArgNum, String strMessage);

        /// <param name='iStackPos'></param>
        void RawGet(Int32 iStackPos);

        /// <param name='iStackPos'></param>
        void RawSet(Int32 iStackPos);

        /// <param name='iStackPos'>Default: -1</param>
        /// <param name='iOutLen'></param>
        String GetString([Optional()] [DefaultValue(-1)] Int32 iStackPos, IntPtr iOutLen);

        /// <param name='iStackPos'>Default: -1</param>
        Double GetNumber([Optional()] [DefaultValue(-1)] Int32 iStackPos);

        /// <param name='iStackPos'>Default: -1</param>
        Boolean GetBool([Optional()] [DefaultValue(-1)] Int32 iStackPos);

        /// <param name='iStackPos'>Default: -1</param>
        IntPtr GetCFunction([Optional()] [DefaultValue(-1)] Int32 iStackPos);

        /// <param name='iStackPos'>Default: -1</param>
        IntPtr GetUserdata([Optional()] [DefaultValue(-1)] Int32 iStackPos);

        void PushNil();

        /// <param name='val'></param>
        /// <param name='iLen'>Default: 0</param>
        void PushString(String val, [Optional()] [DefaultValue(0)] UInt32 iLen);

        /// <param name='val'></param>
        void PushNumber(Double val);

        /// <param name='val'></param>
        void PushBool(Boolean val);

        /// <param name='val'></param>
        void PushCFunction(IntPtr val);

        /// <param name='val'></param>
        /// <param name='iVars'></param>
        void PushCClosure(IntPtr val, Int32 iVars);

        /// <param name='param0'></param>
        void PushUserdata(IntPtr param0);

        Int32 ReferenceCreate();

        /// <param name='i'></param>
        void ReferenceFree(Int32 i);

        /// <param name='i'></param>
        void ReferencePush(Int32 i);

        /// <param name='iType'></param>
        void PushSpecial(Int32 iType);

        /// <param name='iStackPos'></param>
        /// <param name='iType'></param>
        Boolean IsType(Int32 iStackPos, Int32 iType);

        /// <param name='iStackPos'></param>
        Int32 GetType(Int32 iStackPos);

        /// <param name='iType'></param>
        String GetTypeName(Int32 iType);

        /// <param name='strName'></param>
        /// <param name='iType'></param>
        void CreateMetaTableType(String strName, Int32 iType);

        /// <param name='iStackPos'>Default: -1</param>
        String CheckString([Optional()] [DefaultValue(-1)] Int32 iStackPos);

        /// <param name='iStackPos'>Default: -1</param>
        Double CheckNumber([Optional()] [DefaultValue(-1)] Int32 iStackPos);

        /// <param name='iStackPos'>Default: -1</param>
        Int32 ObjLen([Optional()] [DefaultValue(-1)] Int32 iStackPos);

        /// <param name='iStackPos'>Default: -1</param>
        IntPtr GetAngle([Optional()] [DefaultValue(-1)] Int32 iStackPos);

        /// <param name='iStackPos'>Default: -1</param>
        IntPtr GetVector([Optional()] [DefaultValue(-1)] Int32 iStackPos);

        /// <param name='val'></param>
        void PushAngle(IntPtr val);

        /// <param name='val'></param>
        void PushVector(IntPtr val);

        /// <param name='L'></param>
        void SetState(IntPtr L);

        /// <param name='strName'></param>
        Int32 CreateMetaTable(String strName);

        /// <param name='iType'></param>
        Boolean PushMetaTable(Int32 iType);

        /// <param name='data'></param>
        /// <param name='iType'></param>
        void PushUserType(IntPtr data, Int32 iType);

        /// <param name='iStackPos'></param>
        /// <param name='data'></param>
        void SetUserType(Int32 iStackPos, IntPtr data);
    }
}
