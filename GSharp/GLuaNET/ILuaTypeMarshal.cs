namespace GSharp.GLuaNET
{
    public interface ILuaTypeMarshal
    {
        System.Object Get(GLua GLua, System.Int32 stackPos = -1);
        void Push(GLua GLua, System.Object obj);
    }
}