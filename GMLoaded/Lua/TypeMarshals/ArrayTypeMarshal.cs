using System;
namespace GMLoaded.Lua.TypeMarshals
{
    public class ArrayTypeMarshal<T> : ILuaTypeMarshal where T : class
    {
        public Object Get(GLua GLua, Int32 stackPos = -1)
        {
            if (stackPos != -1)
                throw new Exception("stackpos not implemented on array marshal");

            GLua.GetArray<T>();
            return null;
        }

        public void Push(GLua GLua, Object obj) => GLua.SetArray<T>(obj as T[]);
    }
}
