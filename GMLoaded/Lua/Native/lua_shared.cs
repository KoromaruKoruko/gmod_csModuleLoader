using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace GMLoaded.Lua.Native
{
    public static class lua_shared_win // lin: lua_shared.so
    {
        //[DllImport("lua_shared.dll", EntryPoint = "lua_atpanic")]
        //public static unsafe extern void AtPanic(IntPtr Lua_state);
        //[DllImport("lua_shared.dll", EntryPoint = "lua_call")]
        //public static extern void Call(IntPtr Lua_state);
        //[DllImport("lua_shared.dll", EntryPoint = "lua_checkstack")]
        //public static extern void CheckStack(IntPtr Lua_state);
        //[DllImport("lua_shared.dll", EntryPoint = "lua_close")]
        //public static extern void Close(IntPtr Lua_state);
        //[DllImport("lua_shared.dll", EntryPoint = "lua_concat")]
        //public static extern void ConCat(IntPtr Lua_state);
        //[DllImport("lua_shared.dll", EntryPoint = "lua_copy")]
        //public static extern void Copy(IntPtr Lua_state);
        //[DllImport("lua_shared.dll", EntryPoint = "lua_cpcall")]
        //public static extern void CpCall(IntPtr Lua_state);
        //[DllImport("lua_shared.dll", EntryPoint = "lua_createtable")]
        //public static extern void CreateTable(IntPtr Lua_state);
        //[DllImport("lua_shared.dll", EntryPoint = "lua_dump")]
        //public static extern void Dump(IntPtr Lua_state);
        //[DllImport("lua_shared.dll", EntryPoint = "lua_equal")]
        //public static extern void Equal(IntPtr Lua_state);
        //[DllImport("lua_shared.dll", EntryPoint = "lua_error")]
        //public static extern void Error(IntPtr Lua_state);
        //[DllImport("lua_shared.dll", EntryPoint = "lua_gc")]
        //public static extern void GC(IntPtr Lua_state);
        //[DllImport("lua_shared.dll", EntryPoint = "lua_getallocf")]
        //public static extern void GetAllocF(IntPtr Lua_state);
        //[DllImport("lua_shared.dll", EntryPoint = "lua_getfenv")]
        //public static extern void GetFEnv(IntPtr Lua_state);
        //[DllImport("lua_shared.dll", EntryPoint = "lua_getfield")]
        //public static extern void GetField(IntPtr Lua_state);
        //[DllImport("lua_shared.dll", EntryPoint = "lua_gethook")]
        //public static extern void GetHook(IntPtr Lua_state);
        //[DllImport("lua_shared.dll", EntryPoint = "lua_gethookcount")]
        //public static extern void GetHookCount(IntPtr Lua_state);
        //[DllImport("lua_shared.dll", EntryPoint = "lua_gethookmask")]
        //public static extern void GetHookMask(IntPtr Lua_state);
        //[DllImport("lua_shared.dll", EntryPoint = "lua_getinfo")]
        //public static extern void GetInfo(IntPtr Lua_state);
        //[DllImport("lua_shared.dll", EntryPoint = "lua_getlocal")]
        //public static extern void GetLocal(IntPtr Lua_state);
        //[DllImport("lua_shared.dll", EntryPoint = "lua_getmetatable")]
        //public static extern void GetMetaTable(IntPtr Lua_state);
        //[DllImport("lua_shared.dll", EntryPoint = "lua_getstack")]
        //public static extern void GetStack(IntPtr Lua_state);
        //[DllImport("lua_shared.dll", EntryPoint = "lua_gettable")]
        //public static extern void GetTable(IntPtr Lua_state);
        //[DllImport("lua_shared.dll", EntryPoint = "lua_gettop")]
        //public static extern void GetTop(IntPtr Lua_state);
        //[DllImport("lua_shared.dll", EntryPoint = "lua_getupvalue")]
        //public static extern void GetUpValue(IntPtr Lua_state);
        //[DllImport("lua_shared.dll", EntryPoint = "lua_insert")]
        //public static extern void Insert(IntPtr Lua_state);
        //[DllImport("lua_shared.dll", EntryPoint = "lua_iscfunction")]
        //public static extern void IsCFunction(IntPtr Lua_state);
        //[DllImport("lua_shared.dll", EntryPoint = "lua_isnumber")]
        //public static extern void IsNumber(IntPtr Lua_state);
        //[DllImport("lua_shared.dll", EntryPoint = "lua_isstring")]
        //public static extern void IsString(IntPtr Lua_state);
        //[DllImport("lua_shared.dll", EntryPoint = "lua_isuserdata")]
        //public static extern void IsUserdata(IntPtr Lua_state);
        //[DllImport("lua_shared.dll", EntryPoint = "lua_isyieldable")]
        //public static extern void IsYieldable(IntPtr Lua_state);
        //[DllImport("lua_shared.dll", EntryPoint = "lua_lessthan")]
        //public static extern void Lessthen(IntPtr Lua_state);
        //[DllImport("lua_shared.dll", EntryPoint = "lua_load")]
        //public static extern void Load(IntPtr Lua_state);
        //[DllImport("lua_shared.dll", EntryPoint = "lua_loadx")]
        //public static extern void Loadx(IntPtr Lua_state);
        //[DllImport("lua_shared.dll", EntryPoint = "lua_newstate")]
        //public static extern void NewState(IntPtr Lua_state);
        //[DllImport("lua_shared.dll", EntryPoint = "lua_netthread")]
        //public static extern void NewThread(IntPtr Lua_state);
        //[DllImport("lua_shared.dll", EntryPoint = "lua_newuserdata")]
        //public static extern void NewUserdata(IntPtr Lua_state);
        //[DllImport("lua_shared.dll", EntryPoint = "lua_next")]
        //public static extern void Next(IntPtr Lua_state);
        //[DllImport("lua_shared.dll", EntryPoint = "lua_objlen")]
        //public static extern void ObjLen(IntPtr Lua_state);
        //[DllImport("lua_shared.dll", EntryPoint = "lua_pcall")]
        //public static extern void PCall(IntPtr Lua_state);
        //[DllImport("lua_shared.dll", EntryPoint = "lua_pushboolean")]
        //public static extern void PushBoolean(IntPtr Lua_state);
        //[DllImport("lua_shared.dll", EntryPoint = "lua_pushcclosure")]
        //public static extern void PushCClosure(IntPtr Lua_state);
        //[DllImport("lua_shared.dll", EntryPoint = "lua_pushfstring")]
        //public static extern void PushFString(IntPtr Lua_state);
        //[DllImport("lua_shared.dll", EntryPoint = "lua_pushinteger")]
        //public static extern void PushInteger(IntPtr Lua_state);
        //[DllImport("lua_shared.dll", EntryPoint = "lua_pushlightuserdata")]
        //public static extern void PushLightUserdata(IntPtr Lua_state);
        //[DllImport("lua_shared.dll", EntryPoint = "lua_pushlstring")]
        //public static extern void PushLString(IntPtr Lua_state);
        //[DllImport("lua_shared.dll", EntryPoint = "lua_pushnil")]
        //public static extern void PushNil(IntPtr Lua_state);
        //[DllImport("lua_shared.dll", EntryPoint = "lua_pushnumber")]
        //public static extern void PushNumber(IntPtr Lua_state);
        [DllImport("lua_shared.dll", EntryPoint = "lua_pushstring")]
        public static extern void PushString(IntPtr Lua_state, String Str);
        //[DllImport("lua_shared.dll", EntryPoint = "lua_pushthread")]
        //public static extern void PushThread(IntPtr Lua_state);
        //[DllImport("lua_shared.dll", EntryPoint = "lua_pushvalue")]
        //public static extern void PushValue(IntPtr Lua_state);
        //[DllImport("lua_shared.dll", EntryPoint = "lua_pushvfstring")]
        //public static extern void PushVFString(IntPtr Lua_state);
        //[DllImport("lua_shared.dll", EntryPoint = "lua_rawequal")]
        //public static extern void RawEqual(IntPtr Lua_state);
        //[DllImport("lua_shared.dll", EntryPoint = "lua_rawget")]
        //public static extern void RawGet(IntPtr Lua_state);
        //[DllImport("lua_shared.dll", EntryPoint = "lua_rawgeti")]
        //public static extern void RawGetI(IntPtr Lua_state);
        //[DllImport("lua_shared.dll", EntryPoint = "lua_rawset")]
        //public static extern void RawSet(IntPtr Lua_state);
        //[DllImport("lua_shared.dll", EntryPoint = "lua_rawseti")]
        //public static extern void RawSetI(IntPtr Lua_state);
        [DllImport("lua_shared.dll", EntryPoint = "lua_remove")]
        public static extern void Remove(IntPtr Lua_state, Int32 istackpos);
        //[DllImport("lua_shared.dll", EntryPoint = "lua_replace")]
        //public static extern void Replace(IntPtr Lua_state);
        //[DllImport("lua_shared.dll", EntryPoint = "lua_resume_real")]
        //public static extern void Resume_Real(IntPtr Lua_state);
        //[DllImport("lua_shared.dll", EntryPoint = "lua_setallocf")]
        //public static extern void SetAllocF(IntPtr Lua_state);
        //[DllImport("lua_shared.dll", EntryPoint = "lua_setfenv")]
        //public static extern void SetFEnv(IntPtr Lua_state);
        //[DllImport("lua_shared.dll", EntryPoint = "lua_setfield")]
        //public static extern void SetField(IntPtr Lua_state);
        //[DllImport("lua_shared.dll", EntryPoint = "lua_sethook")]
        //public static extern void SetHook(IntPtr Lua_state);
        //[DllImport("lua_shared.dll", EntryPoint = "lua_setlocal")]
        //public static extern void SetLocal(IntPtr Lua_state);
        //[DllImport("lua_shared.dll", EntryPoint = "lua_setmetatable")]
        //public static extern void SetMetaTable(IntPtr Lua_state);
        //[DllImport("lua_shared.dll", EntryPoint = "lua_settable")]
        //public static extern void SetTable(IntPtr Lua_state);
        //[DllImport("lua_shared.dll", EntryPoint = "lua_settop")]
        //public static extern void SetTop(IntPtr Lua_state);
        //[DllImport("lua_shared.dll", EntryPoint = "lua_setupvalue")]
        //public static extern void SetUpValue(IntPtr Lua_state);
        //[DllImport("lua_shared.dll", EntryPoint = "lua_status")]
        //public static extern void Status(IntPtr Lua_state);
        //[DllImport("lua_shared.dll", EntryPoint = "lua_toboolean")]
        //public static extern void ToBoolean(IntPtr Lua_state);
        //[DllImport("lua_shared.dll", EntryPoint = "lua_tocfunction")]
        //public static extern void ToCFunction(IntPtr Lua_state);
        //[DllImport("lua_shared.dll", EntryPoint = "lua_tointeger")]
        //public static extern void ToInteger(IntPtr Lua_state);
        //[DllImport("lua_shared.dll", EntryPoint = "lua_tointegerx")]
        //public static extern void ToIntegerX(IntPtr Lua_state);
        //[DllImport("lua_shared.dll", EntryPoint = "lua_tolstring")]
        //public static extern void ToLString(IntPtr Lua_state);
        //[DllImport("lua_shared.dll", EntryPoint = "lua_tonumber")]
        //public static extern void ToNumber(IntPtr Lua_state);
        //[DllImport("lua_shared.dll", EntryPoint = "lua_tonumberx")]
        //public static extern void ToNumberX(IntPtr Lua_state);
        //[DllImport("lua_shared.dll", EntryPoint = "lua_topointer")]
        //public static extern void ToPointer(IntPtr Lua_state);
        //[DllImport("lua_shared.dll", EntryPoint = "lua_tothread")]
        //public static extern void ToThread(IntPtr Lua_state);
        //[DllImport("lua_shared.dll", EntryPoint = "lua_touserdata")]
        //public static extern void ToUserdata(IntPtr Lua_state);
        //[DllImport("lua_shared.dll", EntryPoint = "lua_type")]
        //public static extern void Type(IntPtr Lua_state);
        //[DllImport("lua_shared.dll", EntryPoint = "lua_typename")]
        //public static extern void TypeName(IntPtr Lua_state);
        //[DllImport("lua_shared.dll", EntryPoint = "lua_upvalueid")]
        //public static extern void UpValueID(IntPtr Lua_state);
        //[DllImport("lua_shared.dll", EntryPoint = "lua_upvaluejoin")]
        //public static extern void UpValueJoin(IntPtr Lua_state);
        //[DllImport("lua_shared.dll", EntryPoint = "lua_version")]
        //public static extern void Version(IntPtr Lua_state);
        //[DllImport("lua_shared.dll", EntryPoint = "lua_xmove")]
        //public static extern void XMove(IntPtr Lua_state);
        //[DllImport("lua_shared.dll", EntryPoint = "lua_yield")]
        //public static extern void Yield(IntPtr Lua_state);
        //[DllImport("lua_shared.dll", EntryPoint = "luaJIT_profile_dumpstack")]
        //public static extern void JIT_ProfileDumpStack(IntPtr Lua_state);
        //[DllImport("lua_shared.dll", EntryPoint = "luaJIT_profile_start")]
        //public static extern void JIT_ProfileStart(IntPtr Lua_state);
        //[DllImport("lua_shared.dll", EntryPoint = "luaJIT_profile_stop")]
        //public static extern void JIT_ProfileStop(IntPtr Lua_state);
        //[DllImport("lua_shared.dll", EntryPoint = "luaJIT_setmode")]
        //public static extern void JIT_setmode(IntPtr Lua_state);
        //[DllImport("lua_shared.dll", EntryPoint = "luaL_addlstring")]
        //public static extern void L_AddLString(IntPtr Lua_state);
        //[DllImport("lua_shared.dll", EntryPoint = "luaL_addstring")]
        //public static extern void L_AddString(IntPtr Lua_state);
        //[DllImport("lua_shared.dll", EntryPoint = "luaL_addvalue")]
        //public static extern void L_AddValue(IntPtr Lua_state);
        //[DllImport("lua_shared.dll", EntryPoint = "luaL_argerror")]
        //public static extern void L_ArgError(IntPtr Lua_state);
        //[DllImport("lua_shared.dll", EntryPoint = "luaL_buffinit")]
        //public static extern void L_BuffInit(IntPtr Lua_state);
        //[DllImport("lua_shared.dll", EntryPoint = "luaL_callmeta")]
        //public static extern void L_CallMeta(IntPtr Lua_state);
        //[DllImport("lua_shared.dll", EntryPoint = "luaL_checkany")]
        //public static extern void L_CheckAny(IntPtr Lua_state);
        //[DllImport("lua_shared.dll", EntryPoint = "luaL_checkinteger")]
        //public static extern void L_CheckInteger(IntPtr Lua_state);
        //[DllImport("lua_shared.dll", EntryPoint = "luaL_checklstring")]
        //public static extern void L_CheckLString(IntPtr Lua_state);
        //[DllImport("lua_shared.dll", EntryPoint = "luaL_checknumber")]
        //public static extern void L_CheckNumber(IntPtr Lua_state);
        //[DllImport("lua_shared.dll", EntryPoint = "luaL_checkoption")]
        //public static extern void L_CheckOption(IntPtr Lua_state);
        //[DllImport("lua_shared.dll", EntryPoint = "luaL_checkstack")]
        //public static extern void L_CheckStack(IntPtr Lua_state);
        //[DllImport("lua_shared.dll", EntryPoint = "luaL_checktype")]
        //public static extern void L_Checktype(IntPtr Lua_state);
        //[DllImport("lua_shared.dll", EntryPoint = "luaL_checkudata")]
        //public static extern void L_CheckUserdata(IntPtr Lua_state);
        //[DllImport("lua_shared.dll", EntryPoint = "luaL_error")]
        //public static extern void L_Error(IntPtr Lua_state);
        //[DllImport("lua_shared.dll", EntryPoint = "luaL_execresult")]
        //public static extern void L_ExecuteResult(IntPtr Lua_state);
        //[DllImport("lua_shared.dll", EntryPoint = "luaL_fileresult")]
        //public static extern void L_FileResult(IntPtr Lua_state);
        //[DllImport("lua_shared.dll", EntryPoint = "luaL_findtable")]
        //public static extern void L_FindTable(IntPtr Lua_state);
        //[DllImport("lua_shared.dll", EntryPoint = "luaL_getmetafield")]
        //public static extern void L_GetMetaField(IntPtr Lua_state);
        //[DllImport("lua_shared.dll", EntryPoint = "luaL_gsub")]
        //public static extern void L_GSub(IntPtr Lua_state);
        //[DllImport("lua_shared.dll", EntryPoint = "luaL_loadbuffer")]
        //public static extern void L_LoadBuffer(IntPtr Lua_state);
        //[DllImport("lua_shared.dll", EntryPoint = "luaL_loadbufferx")]
        //public static extern void L_LoadBufferX(IntPtr Lua_state);
        //[DllImport("lua_shared.dll", EntryPoint = "luaL_loadfile")]
        //public static extern void L_LoadFile(IntPtr Lua_state);
        //[DllImport("lua_shared.dll", EntryPoint = "luaL_loadfilex")]
        //public static extern void L_LoadFileX(IntPtr Lua_state);
        //[DllImport("lua_shared.dll", EntryPoint = "luaL_loadstring")]
        //public static extern void L_LoadString(IntPtr Lua_state);
        //[DllImport("lua_shared.dll", EntryPoint = "luaL_newmetatable")]
        //public static extern void L_NewMetaTable(IntPtr Lua_state);
        //[DllImport("lua_shared.dll", EntryPoint = "luaL_newmetatable_type")]
        //public static extern void L_NewMetaTableType(IntPtr Lua_state);
        //[DllImport("lua_shared.dll", EntryPoint = "luaL_newstate")]
        //public static extern void L_NewState(IntPtr Lua_state);
        //[DllImport("lua_shared.dll", EntryPoint = "luaL_openlib")]
        //public static extern void L_OpenLib(IntPtr Lua_state);
        //[DllImport("lua_shared.dll", EntryPoint = "luaL_openlibs")]
        //public static extern void L_OpenLibs(IntPtr Lua_state);
        //[DllImport("lua_shared.dll", EntryPoint = "luaL_optinteger")]
        //public static extern void L_OptInteger(IntPtr Lua_state);
        //[DllImport("lua_shared.dll", EntryPoint = "luaL_optlstring")]
        //public static extern void L_OptLString(IntPtr Lua_state);
        //[DllImport("lua_shared.dll", EntryPoint = "luaL_optnumber")]
        //public static extern void L_OptNumber(IntPtr Lua_state);
        //[DllImport("lua_shared.dll", EntryPoint = "luaL_prepbuffer")]
        //public static extern void L_PrepBuffer(IntPtr Lua_state);
        //[DllImport("lua_shared.dll", EntryPoint = "luaL_pushmodule")]
        //public static extern void L_PushModule(IntPtr Lua_state);
        //[DllImport("lua_shared.dll", EntryPoint = "luaL_pushresult")]
        //public static extern void L_PushResult(IntPtr Lua_state);
        //[DllImport("lua_shared.dll", EntryPoint = "luaL_ref")]
        //public static extern void L_Ref(IntPtr Lua_state);
        //[DllImport("lua_shared.dll", EntryPoint = "luaL_register")]
        //public static extern void L_Register(IntPtr Lua_state);
        //[DllImport("lua_shared.dll", EntryPoint = "luaL_setfuncs")]
        //public static extern void L_SetFuncs(IntPtr Lua_state);
        //[DllImport("lua_shared.dll", EntryPoint = "luaL_setmetatable")]
        //public static extern void L_SetMetaTable(IntPtr Lua_state);
        //[DllImport("lua_shared.dll", EntryPoint = "luaL_testudata")]
        //public static extern void L_TestUserdata(IntPtr Lua_state);
        //[DllImport("lua_shared.dll", EntryPoint = "luaL_traceback")]
        //public static extern void L_Traceback(IntPtr Lua_state);
        //[DllImport("lua_shared.dll", EntryPoint = "luaL_typerror")] // not a typo, the exported function is truly typerror
        //public static extern void L_TypeError(IntPtr Lua_state);
        //[DllImport("lua_shared.dll", EntryPoint = "luaL_unref")]
        //public static extern void L_UnRef(IntPtr Lua_state);
        //[DllImport("lua_shared.dll", EntryPoint = "luaL_where")]
        //public static extern void L_Where(IntPtr Lua_state);
        //[DllImport("lua_shared.dll", EntryPoint = "luaopen_base")]
        //public static extern void Open_Base(IntPtr Lua_state);
        //[DllImport("lua_shared.dll", EntryPoint = "luaopen_bit")]
        //public static extern void Open_Bit(IntPtr Lua_state);
        //[DllImport("lua_shared.dll", EntryPoint = "luaopen_debug")]
        //public static extern void Open_Debug(IntPtr Lua_state);
        //[DllImport("lua_shared.dll", EntryPoint = "luaopen_jit")]
        //public static extern void Open_jit(IntPtr Lua_state);
        //[DllImport("lua_shared.dll", EntryPoint = "luaopen_math")]
        //public static extern void Open_Math(IntPtr Lua_state);
        //[DllImport("lua_shared.dll", EntryPoint = "luaopen_os")]
        //public static extern void Open_OS(IntPtr Lua_state);
        //[DllImport("lua_shared.dll", EntryPoint = "luaopen_package")]
        //public static extern void Open_Package(IntPtr Lua_state);
        //[DllImport("lua_shared.dll", EntryPoint = "luaopen_string")]
        //public static extern void Open_String(IntPtr Lua_state);
        //[DllImport("lua_shared.dll", EntryPoint = "luaopen_table")]
        //public static extern void Open_Table(IntPtr Lua_state);
    }
}
