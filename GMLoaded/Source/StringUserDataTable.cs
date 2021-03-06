﻿using System;
using System.Runtime.InteropServices;
using GMLoaded.Source.Interaces;

namespace GMLoaded.Source
{
    public class StringUserDataTable<TClass>
    {
        private readonly INetworkStringTable table;

        internal StringUserDataTable(INetworkStringTable table) => this.table = table;

        public TClass this[Int32 index] => this.GetUserData(index);

        public Int32 Count() => this.table.GetNumStrings();

        public TClass GetUserData(Int32 index, IntPtr lengthOut = default)
        {
            IntPtr ptr = this.table.GetStringUserData(index, lengthOut);
            return Marshal.PtrToStructure<TClass>(ptr);
        }
    }
}
