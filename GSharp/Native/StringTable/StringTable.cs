using GSharp.Generated.NativeClasses;

using Libraria.Native;

using System;
using System.Collections;
using System.Collections.Generic;

namespace GSharp.Native.StringTable
{
    public struct Entry<TClass>
    {
        public String String;
        public TClass UserData;

        public Entry(String str, TClass udata)
        {
            this.String = str;
            this.UserData = udata;
        }
    }

    public static class StringTableInterfaceName
    {
        public const String CLIENT = "VEngineClientStringTable001";
        public const String SERVER = "VEngineServerStringTable001";
    }

    public class StringTable
    {
        private static readonly INetworkStringTableContainer container;
        protected INetworkStringTable table;

        internal StringTable(INetworkStringTable table) => this.table = table;

        static StringTable() =>
#if CLIENT
            container = NativeInterface.Load<INetworkStringTableContainer>("engine", StringTableInterfaceName.CLIENT);
#elif SERVER
            container = NativeInterface.Load<INetworkStringTableContainer>("engine", StringTableInterfaceName.SERVER);
#else
#warning StringTable needs CLIENT or SERVER defined
            throw new NotSupportedException("StringTable needs CLIENT or SERVER defined at compile time");

#endif

        public virtual String this[Int32 index] => this.GetString(index);

        private static INetworkStringTable FindTableInternal(String name)
        {
            if (container == null) return null;
            IntPtr stringTablePointer = container.FindTable(name);
            if (stringTablePointer == IntPtr.Zero) return null;
            INetworkStringTable stringTable = JIT.ConvertInstance<INetworkStringTable>(stringTablePointer);
            return stringTable;
        }

        private static INetworkStringTable GetTableInternal(Int32 index)
        {
            if (container == null) return null;
            IntPtr stringTablePointer = container.GetTable(index);
            if (stringTablePointer == IntPtr.Zero) return null;
            INetworkStringTable stringTable = JIT.ConvertInstance<INetworkStringTable>(stringTablePointer);
            return stringTable;
        }

        public static StringTable FindTable(String name)
        {
            INetworkStringTable table = FindTableInternal(name);
            return new StringTableBare(table);
        }

        public static StringTable<TClass> FindTable<TClass>(String name)
        {
            INetworkStringTable table = FindTableInternal(name);
            return new StringTable<TClass>(table);
        }

        public static StringTable GetTable(Int32 index)
        {
            INetworkStringTable table = GetTableInternal(index);
            return new StringTableBare(table);
        }

        public static StringTable<TClass> GetTable<TClass>(Int32 index)
        {
            INetworkStringTable table = GetTableInternal(index);
            return new StringTable<TClass>(table);
        }

        #region INetworkStringTable Passthrough

        public Int32 AddString(Boolean bIsServer, String value, Int32 length = -1, IntPtr userdata = default) => this.table.AddString(bIsServer, value, length, userdata);

        public Boolean ChangedSinceTick(Int32 tick) => this.table.ChangedSinceTick(tick);

        public Int32 Count() => this.table.GetNumStrings();

        public Int32 FindStringIndex(String str) => this.table.FindStringIndex(str);

        public Int32 GetEntryBits() => this.table.GetEntryBits();

        public Int32 GetMaxStrings() => this.table.GetMaxStrings();

        public String GetString(Int32 stringNumber) => this.table.GetString(stringNumber);

        public IntPtr GetStringUserData(Int32 stringNumber, IntPtr length) => this.table.GetStringUserData(stringNumber, length);

        public Int32 GetTableId() => this.table.GetTableId();

        public String GetTableName() => this.table.GetTableName();

        public void SetStringChangedCallback(IntPtr obj, IntPtr changeFunc) => this.table.SetStringChangedCallback(obj, changeFunc);

        public void SetStringUserData(Int32 stringNumber, Int32 length, IntPtr userdata) => this.table.SetStringUserData(stringNumber, length, userdata);

        public void SetTick(Int32 tick) => this.table.SetTick(tick);

        #endregion INetworkStringTable Passthrough
    }

    public class StringTable<TClass> : StringTable, IEnumerable<Entry<TClass>>
    {
        public StringTable(INetworkStringTable table) : base(table) => this.UserData = new StringUserDataTable<TClass>(table);

        public StringUserDataTable<TClass> UserData { get; protected set; }

        public IEnumerator<Entry<TClass>> GetEnumerator() => new StringTableUserDataEnumerator<TClass>(this);

        IEnumerator IEnumerable.GetEnumerator() => (this as IEnumerable<Entry<TClass>>).GetEnumerator();
    }

    public class StringTableBare : StringTable, IEnumerable<String>
    {
        public StringTableBare(INetworkStringTable table) : base(table)
        {
        }

        public IEnumerator<String> GetEnumerator() => new StringTableEnumerator(this);

        IEnumerator IEnumerable.GetEnumerator() => new StringTableEnumerator(this);
    }
}
