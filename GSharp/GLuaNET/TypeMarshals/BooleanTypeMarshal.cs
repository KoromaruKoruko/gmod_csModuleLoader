namespace GSharp.GLuaNET.TypeMarshals
{
    public class BooleanTypeMarshal : ILuaTypeMarshal
    {
        public System.Object Get(GLua GLua, System.Int32 stackPos = -1) => GLua.IsType(stackPos, LuaType.Boolean) ? GLua.LuaBase.GetBool(stackPos) : (System.Object)null;

        public void Push(GLua GLua, System.Object obj)
        {
            if (obj is System.Boolean val)
            {
                GLua.LuaBase.PushBool(val);
            }
        }
    }
}
