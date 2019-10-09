using System;
namespace GSharp.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    class VTableSlotAttribute : Attribute
    {
        public Int32 Slot { get; set; }

        public VTableSlotAttribute(Int32 s) => this.Slot = s;
    }
}