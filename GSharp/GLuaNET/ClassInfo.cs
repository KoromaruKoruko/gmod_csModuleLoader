using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace GSharp.GLuaNET
{
    public class MethodLuaJITInfo
    {
        public Type ReturnType { get; set; }
        public List<Type> Args { get; set; }
        public String Name { get; set; }
        public MethodInfo MethodInfo { get; set; }

        public MethodLuaJITInfo()
        {
        }

        public MethodLuaJITInfo(MethodInfo method)
        {
            this.ReturnType = method.ReturnType;
            this.Name = method.Name;
            this.MethodInfo = method;

            this.Args = new List<Type>();

            foreach (ParameterInfo paramInfo in method.GetParameters())
            {
                this.Args.Add(paramInfo.ParameterType);
            }
        }
    }

    public class ClassLuaJITInfo
    {
        public List<MethodLuaJITInfo> Methods { get; set; } = new List<MethodLuaJITInfo>();

        public ClassLuaJITInfo()
        {
        }

        public ClassLuaJITInfo(Type classType)
        {
            MethodInfo[] methods = classType.GetMethods();
            for (Int32 i = 0; i < methods.Length; i++)
            {
                this.Methods.Add(new MethodLuaJITInfo(methods[i]));
            }
        }
    }

    static class TypeExtensions
    {
        public static Boolean IsCastableTo(this Type from, Type to)
        {
            if (to.IsAssignableFrom(from))
            {
                return true;
            }
            IEnumerable<MethodInfo> methods = from.GetMethods(BindingFlags.Public | BindingFlags.Static)
                              .Where(
                                  m => m.ReturnType == to &&
                                       (m.Name == "op_Implicit" ||
                                        m.Name == "op_Explicit")
                              );
            return methods.Count() > 0;
        }
    }
}