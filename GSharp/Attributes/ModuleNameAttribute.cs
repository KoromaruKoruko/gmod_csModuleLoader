using System;
namespace GSharp.Attributes
{
    [AttributeUsage(AttributeTargets.Interface, AllowMultiple = false)]
    class ModuleNameAttribute : Attribute
    {
        public String ModuleName { get; set; }

        public ModuleNameAttribute(String moduleName) => this.ModuleName = moduleName;
    }
}