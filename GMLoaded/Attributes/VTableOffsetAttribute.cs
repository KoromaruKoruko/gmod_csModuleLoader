using System;
namespace GMLoaded.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    class VTableOffsetAttribute : Attribute
    {
        public Int32 Offset { get; set; }

        public VTableOffsetAttribute(Int32 s) => this.Offset = s;
    }
}
