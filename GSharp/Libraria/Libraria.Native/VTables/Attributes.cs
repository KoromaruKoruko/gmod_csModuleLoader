using System;

namespace Libraria.Native
{
    [AttributeUsage(AttributeTargets.Interface, AllowMultiple = false)]
    class InterfaceVersionAttribute : Attribute
    {
        public String Identifier { get; set; }

        public InterfaceVersionAttribute(String versionIdentifier) => this.Identifier = versionIdentifier;
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class VTableSlotAttribute : Attribute
    {
        public Int32 Slot { get; set; }

        public VTableSlotAttribute(Int32 s) => this.Slot = s;
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class VTableOffsetAttribute : Attribute
    {
        public Int32 Offset { get; set; }

        public VTableOffsetAttribute(Int32 s) => this.Offset = s;
    }
}