using System;
using GMLoaded.Lua.Smart;

namespace GMLoaded.Lua
{
    public class Entity : ITableBase
    {
        public Entity(GLua LuaHandle, Int32 IStackPos) : base(LuaHandle, IStackPos)
        {
        }
        public Entity(SmartLuaReferance Ref) : base(Ref)
        {
        }

        // TODO: add all Std Entity funcs here!
    }
}
