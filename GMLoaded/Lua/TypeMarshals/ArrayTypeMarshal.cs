using System;
namespace GMLoaded.Lua.TypeMarshals
{
    public class ArrayTypeMarshal<T> : ILuaTypeMarshal where T : class
    {
        public T[] GetT(GLua GLua) => GLua.GetArray<T>();

        public Object Get(GLua GLua, Int32 stackPos = -1)
        {
            if (stackPos != -1)
                throw new Exception("stackpos not implemented on array marshal");

            return GLua.GetArray<T>();
        }

        public void Push(GLua GLua, Object obj) => GLua.SetArray<T>(obj as T[]);

        private ArrayTypeMarshal() { }
        private static ArrayTypeMarshal<T> Instance;
        public static ArrayTypeMarshal<T> Create()
        {
            Instance = Instance ?? new ArrayTypeMarshal<T>();
            return Instance;
        }
    }
}
