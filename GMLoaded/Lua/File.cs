using System;
using GMLoaded.Lua.Smart;

namespace GMLoaded.Lua
{
    public class File : ITableBase
    {
        public File(GLua LuaHandle, Int32 IStackPos) : base(LuaHandle, IStackPos)
        {
        }
        public File(SmartLuaReferance Ref) : base(Ref)
        {
        }
        // TODO: add all Std File funcs here!
    }
}
