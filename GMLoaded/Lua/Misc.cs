using System;
using System.Runtime.InteropServices;

namespace GMLoaded.Lua
{
    /// <summary>
    /// LUA CFunc handle and/or loader
    /// </summary>
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate Int32 CFunc(IntPtr L);

    public enum GarrysMod_Lua_State
    {
        CLIENT = 0,
        SERVER = 1,
        MENU = 2,
    }

    [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public delegate IntPtr CreateInterfaceFn(String pName, IntPtr pReturnCode);

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
        public static readonly LuaType Entity = new LuaType(9, "Entity");
        public static readonly LuaType Vector = new LuaType(10, "Vector");
        public static readonly LuaType Angle = new LuaType(11, "Angle");
        public static readonly LuaType PhysObj = new LuaType(12, "PhysObj");
        public static readonly LuaType ISave = new LuaType(13, "ISave");
        public static readonly LuaType IRestore = new LuaType(14, "IRestore");
        public static readonly LuaType CTakeDamageInfo = new LuaType(15, "CTakeDamageInfo");
        public static readonly LuaType CEffectData = new LuaType(16, "CEffectData");
        public static readonly LuaType CMoveData = new LuaType(17, "CMoveData");
        public static readonly LuaType CRecipientFilter = new LuaType(18, "CRecipientFilter");
        public static readonly LuaType CUserCmd = new LuaType(19, "CUserCmd");
        public static readonly LuaType IMaterial = new LuaType(21, "IMaterial");
        public static readonly LuaType Panel = new LuaType(22, "Panel");
        public static readonly LuaType CLuaParticle = new LuaType(23, "CLuaParticle");
        public static readonly LuaType CLuaEmitter = new LuaType(24, "CLuaEmitter");
        public static readonly LuaType ITexture = new LuaType(25, "ITexture");
        public static readonly LuaType bf_read = new LuaType(26, "bf_read");
        public static readonly LuaType ConVar = new LuaType(27, "ConVar");
        public static readonly LuaType IMesh = new LuaType(28, "IMesh");
        public static readonly LuaType VMatrix = new LuaType(29, "VMatrix");
        public static readonly LuaType CSoundPatch = new LuaType(30, "CSoundPatch");
        public static readonly LuaType pixelvis_handle_t = new LuaType(31, "pixelvis_handle_t");
        public static readonly LuaType dlight_t = new LuaType(32, "dlight_t");
        public static readonly LuaType IVideoWriter = new LuaType(33, "IVideoWriter");
        public static readonly LuaType File = new LuaType(34, "File");
        public static readonly LuaType CLuaLocomotion = new LuaType(35, "CLuaLocomotion");
        public static readonly LuaType PathFollower = new LuaType(36, "PathFollower");
        public static readonly LuaType CNavArea = new LuaType(37, "CNavArea");
        public static readonly LuaType IGModAudioChannel = new LuaType(38, "IGModAudioChannel");
        public static readonly LuaType CNavLadder = new LuaType(39, "CNavLadder");
        public static readonly LuaType CNewParticleEffect = new LuaType(40, "	CNewParticleEffect");
        public static readonly LuaType ProjectedTexture = new LuaType(41, "ProjectedTexture");
        public static readonly LuaType PhysCollide = new LuaType(42, "PhysCollide");
        public static readonly LuaType NumberOfEnums = new LuaType(43, "NumberOfEnums");
        public static readonly LuaType Color = new LuaType(255, "Color");

        public readonly Int32 TypeID;
        public readonly String Name;

        private LuaType(Int32 type, String name = "")
        {
            this.TypeID = type;
            this.Name = name;
        }

        public static implicit operator Int32(LuaType value) => value.TypeID;

        public static implicit operator LuaType(Int32 value) => new LuaType(value);
    }
}
