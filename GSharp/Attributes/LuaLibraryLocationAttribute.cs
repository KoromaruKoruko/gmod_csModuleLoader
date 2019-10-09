using System;

namespace GSharp.Attributes
{
    [AttributeUsage(AttributeTargets.Interface, AllowMultiple = false)]
    class LuaLibraryLocationAttribute : Attribute
    {
        public String Path { get; set; }

        public LuaLibraryLocationAttribute(String path) => this.Path = path;
    }
}