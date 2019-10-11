using System;
using System.Collections.Generic;

namespace GMLoaded.Source
{
    internal class StringTableUserDataEnumerator<TClass> : IEnumerator<Entry<TClass>>
    {
        private readonly StringUserDataTable<TClass> userdata;
        private Int32 position = -1;
        private StringTable table;

        public StringTableUserDataEnumerator(StringTable<TClass> table)
        {
            this.table = table;
            this.userdata = table.UserData;
        }

        public Object Current => (this as IEnumerator<Entry<TClass>>).Current;
        Entry<TClass> IEnumerator<Entry<TClass>>.Current => new Entry<TClass>(this.table[this.position], this.userdata[this.position]);

        public void Dispose() => this.table = null;

        public Boolean MoveNext() => ++this.position < this.table.Count();

        public void Reset() => this.position = -1;
    }
}
