namespace GSharp.GLuaNET
{
    public class LuaType
    {
        public static readonly LuaType None = new LuaType(-1, "None");
        public static readonly LuaType Nil = new LuaType(0, "Nil");
        public static readonly LuaType Boolean = new LuaType(1, "Boolean");
        public static readonly LuaType LightUserData = new LuaType(2, "LightUserData");
        public static readonly LuaType Number = new LuaType(3, "Number");
        public static readonly LuaType String = new LuaType(4, "String");
        public static readonly LuaType Table = new LuaType(5, "Table");
        public static readonly LuaType Function = new LuaType(6, "Function");
        public static readonly LuaType UserData = new LuaType(7, "UserData");
        public static readonly LuaType Thread = new LuaType(8, "Thread");

        readonly System.Int32 type;
        readonly System.String name;
        private LuaType(System.Int32 type, System.String name = "")
        {
            this.type = type;
            this.name = name;
        }

        public static implicit operator System.Int32(LuaType value) => value.type;

        public static implicit operator LuaType(System.Int32 value) => new LuaType(value);
    }
}