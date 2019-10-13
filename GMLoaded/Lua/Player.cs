using System;
using GMLoaded.Lua.Smart;
using System.Collections.Generic;
using System.Text;

namespace GMLoaded.Lua
{
    public class Player : Entity
    {
        public Player(GLua Glua, Int32 IStackPos) : base(Glua, IStackPos)
        { }
        public Player(SmartLuaReferance Ref) : base(Ref)
        { }
        public Player(Entity Ent) : base(Ent.SmartRef)
        { }
        // TODO: add all Std Entity funcs here!
    }
}
