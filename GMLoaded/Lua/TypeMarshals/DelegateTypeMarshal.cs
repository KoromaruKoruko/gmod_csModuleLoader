using System;
using System.Collections.Generic;
using System.Reflection;

namespace GMLoaded.Lua.TypeMarshals
{
    public class DelegateTypeMarshal : ILuaTypeMarshal
    {
        public Object Get(GLua GLua, Int32 stackPos = -1) => throw new NotImplementedException();

        public void Push(GLua GLua, Object obj)
        {
            if (obj is Delegate dele)
            {
                Int32 cfunc(IntPtr L)
                {
                    List<Object> args = new List<Object>();
                    foreach (ParameterInfo param in dele.Method.GetParameters())
                    {
                        args.Add(GLua.Get(param.ParameterType, param.Position + 1));
                    }
                    Object rtn = dele.DynamicInvoke(args.ToArray());
                    if (rtn != null)
                    {
                        GLua.Push(rtn, rtn.GetType());
                        return 1;
                    }
                    return 0;
                }
#pragma warning disable IDE0009 // Member access should be qualified. unknown issue, with space check, will post issue on forums
                GLua.PushCFunction(cfunc);
#pragma warning restore IDE0009
            }
            else
                throw new Exception("wrapper func must be castable to delegate");
        }
    }
}
