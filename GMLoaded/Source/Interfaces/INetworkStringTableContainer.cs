using System.ComponentModel;
using System.Runtime.InteropServices;
using System;

namespace GMLoaded.Source.Interaces
{
    public interface INetworkStringTableContainer
    {
        /// <param name='tableName'></param>
        /// <param name='maxentries'></param>
        /// <param name='userdatafixedsize'>Default: 0</param>
        /// <param name='userdatanetworkbits'>Default: 0</param>
        IntPtr CreateStringTable(String tableName, Int32 maxentries, [Optional()] [DefaultValue(0)] Int32 userdatafixedsize, [Optional()] [DefaultValue(0)] Int32 userdatanetworkbits);

        /// <param name='tableName'></param>
        /// <param name='maxentries'></param>
        /// <param name='userdatafixedsize'>Default: 0</param>
        /// <param name='userdatanetworkbits'>Default: 0</param>
        /// <param name='bIsFilenames'>Default: false</param>
        IntPtr CreateStringTableEx(String tableName, Int32 maxentries, [Optional()] [DefaultValue(0)] Int32 userdatafixedsize, [Optional()] [DefaultValue(0)] Int32 userdatanetworkbits, [Optional()] [DefaultValue(false)] Boolean bIsFilenames);

        void dtorINetworkStringTableContainer();

        /// <param name='tableName'></param>
        IntPtr FindTable(String tableName);

        Int32 GetNumTables();

        /// <param name='stringTable'></param>
        IntPtr GetTable(Int32 stringTable);

        void RemoveAllTables();

        /// <param name='table'></param>
        /// <param name='bAllowClientSideAddString'></param>
        void SetAllowClientSideAddString(IntPtr table, Boolean bAllowClientSideAddString);
    }
}
