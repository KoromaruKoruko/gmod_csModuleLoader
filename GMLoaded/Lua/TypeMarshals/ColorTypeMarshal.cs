using System;
namespace GMLoaded.Lua.TypeMarshals
{
    public class ColorTypeMarshal : ILuaTypeMarshal
    {
        public Object Get(GLua GLua, Int32 stackPos = -1) => new Color(GLua, stackPos);
        public void Push(GLua GLua, Object obj) => ((Color)obj).Push(GLua);

        private ColorTypeMarshal() { }
        private static ColorTypeMarshal Instance;
        public static ColorTypeMarshal Create()
        {
            Instance = Instance ?? new ColorTypeMarshal();
            return Instance;
        }
    }
}
