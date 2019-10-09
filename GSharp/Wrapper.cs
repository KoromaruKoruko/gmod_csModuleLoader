using System;
using System.Collections.Generic;
using System.Reflection;

namespace GSharp
{
    public static class Wrapper
    {
        public static IEnumerable<Type> GetAllDeclaredTypesUsed(Type type, HashSet<Type> typesUsed = null)
        {
            typesUsed ??= new HashSet<Type>();
            String declaredAssembly = type.Assembly.FullName;

            // TO:"because recursive", this got added in a while ago.
            void addType(Type pierceType)
            {
                pierceType = (pierceType.IsByRef || pierceType.IsArray) ? pierceType.GetElementType() : pierceType;
                if (pierceType.Assembly.FullName == declaredAssembly && !pierceType.IsGenericParameter)
                {
                    if (typesUsed.Add(pierceType))
                    {
                        foreach (Type t2 in GetAllDeclaredTypesUsed(pierceType, typesUsed))
                        {
                            addType(pierceType);
                        }
                    }
                }
            }

            foreach (FieldInfo field in type.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static))
            {
                addType(field.FieldType);
            }

            foreach (PropertyInfo property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static))
            {
                addType(property.PropertyType);
            }

            foreach (MethodInfo method in type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static))
            {
                addType(method.ReturnType);
                foreach (ParameterInfo parameter in method.GetParameters())
                {
                    addType(parameter.ParameterType);
                }
            }

            return typesUsed;
        }

        public static void WrapType()
        {
        }
    }
}
