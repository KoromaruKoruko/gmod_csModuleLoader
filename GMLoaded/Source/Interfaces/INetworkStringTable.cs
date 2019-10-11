using System.ComponentModel;
using System.Runtime.InteropServices;
using System;
namespace GMLoaded.Source.Interaces
{
    public interface INetworkStringTable
    {
        void dtorINetworkStringTable();

        String GetTableName();

        Int32 GetTableId();

        Int32 GetNumStrings();

        Int32 GetMaxStrings();

        Int32 GetEntryBits();

        /// <param name='tick'></param>
        void SetTick(Int32 tick);

        /// <param name='tick'></param>
        Boolean ChangedSinceTick(Int32 tick);

        /// <param name='bIsServer'></param>
        /// <param name='value'></param>
        /// <param name='length'>Default: -1</param>
        /// <param name='userdata'>Default: 0</param>
        Int32 AddString(Boolean bIsServer, String value, [Optional()] [DefaultValue(-1)] Int32 length, [Optional()] [DefaultValue(0)] IntPtr userdata);

        /// <param name='stringNumber'></param>
        String GetString(Int32 stringNumber);

        /// <param name='stringNumber'></param>
        /// <param name='length'></param>
        /// <param name='userdata'></param>
        void SetStringUserData(Int32 stringNumber, Int32 length, IntPtr userdata);

        /// <param name='stringNumber'></param>
        /// <param name='length'></param>
        IntPtr GetStringUserData(Int32 stringNumber, IntPtr length);

        /// <param name='string'></param>
        Int32 FindStringIndex(String @string);

        /// <param name='object'></param>
        /// <param name='changeFunc'></param>
        void SetStringChangedCallback(IntPtr @object, IntPtr changeFunc);
    }
}
