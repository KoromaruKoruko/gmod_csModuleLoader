using System;
namespace GMLoaded.Lua.TypeMarshals
{
    public interface ILuaTypeMarshal
    {
        Object Get(GLua GLua, Int32 stackPos = -1);
        void Push(GLua GLua, Object obj);
    }
}
