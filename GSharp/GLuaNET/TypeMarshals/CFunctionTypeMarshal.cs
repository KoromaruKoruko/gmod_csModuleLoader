using GSharp.Generated.NativeClasses;

namespace GSharp.GLuaNET.TypeMarshals
{
    public class CFunctionTypeMarshal : ILuaTypeMarshal
    {
        public System.Object Get(GLua GLua, System.Int32 stackPos = -1)
        {
            if (GLua.IsType(stackPos, LuaType.Function))
            {
                return GLua.LuaBase.GetCFunction(stackPos);
            }
            return null;
        }

        public void Push(GLua GLua, System.Object obj)
        {
            if (obj is CFunc func)
            {
                GLua.PushCFunction(func);
            }
        }
    }
}