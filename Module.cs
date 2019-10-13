using System;
using GMLoaded;

namespace ModuleExample
{
    public static class Module
    {
        public static Int32 Close(GLua LUA)
        {
            LUA.GetGlobal("print");
            LUA.Push("Hello! Module is being unloaded");
            LUA.Call(1, 0);
            return 0;
        }

        public static Int32 Open(GLua LUA)
        {
            LUA.GetGlobal("print");
            LUA.Push("Hello! Module has been loaded");
            LUA.Call(1, 0);
            return 0;
        }
    }
}
