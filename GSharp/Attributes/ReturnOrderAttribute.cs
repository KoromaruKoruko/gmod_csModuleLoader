using System;

namespace GSharp.Attributes
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    sealed class ReturnIndexAttribute : Attribute
    {
        public Int32 ReturnIndex { get; set; }

        // This is a positional argument
        public ReturnIndexAttribute(Int32 returnIndex) => this.ReturnIndex = returnIndex;
    }
}