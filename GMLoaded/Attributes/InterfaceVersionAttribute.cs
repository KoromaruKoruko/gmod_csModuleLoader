using System;
namespace GMLoaded.Attributes
{
    [AttributeUsage(AttributeTargets.Interface, AllowMultiple = false)]
    class InterfaceVersionAttribute : Attribute
    {
        public String Identifier { get; set; }

        public InterfaceVersionAttribute(String versionIdentifier) => this.Identifier = versionIdentifier;
    }

    public class InterfaceVersions
    {
        public static String GetInterfaceIdentifier(Type targetClass)
        {
            foreach (InterfaceVersionAttribute attribute in targetClass.GetCustomAttributes(typeof(InterfaceVersionAttribute), false))
            {
                return attribute.Identifier;
            }

            throw new Exception("Version identifier not found for class " + targetClass);
        }
    }
}
