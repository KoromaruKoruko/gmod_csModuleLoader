using System;
using GMLoaded.Lua.Smart;

namespace GMLoaded.Lua
{
    public class Entity : Table
    {
        //https://wiki.garrysmod.com/page/Category:Entity

        public Entity(GLua LuaHandle, Int32 IStackPos) : base(LuaHandle, IStackPos)
        {
        }

        public Entity(SmartLuaReferance Ref) : base(Ref)
        {
        }
    }
}
