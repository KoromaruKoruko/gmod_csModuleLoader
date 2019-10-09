using GSharp.Attributes;
using GSharp.Generated.NativeClasses;
using GSharp.GLuaNET.TypeMarshals;

using Libraria.Native;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

namespace GSharp.GLuaNET
{
    public partial class GLua
    {
        private static readonly List<GLua> Gluas = new List<GLua>();

        public static GLua Get(Lua_State luaState)
        {
            GLua g = Gluas.Where(l => l.State.Luabase.Equals(luaState.Luabase)).FirstOrDefault();
            if (g == null)
            {
                g = new GLua(luaState);
                Gluas.Add(g);
            }
            return g;
        }

        public const Int32 LUA_REGISTRYINDEX = -10000;
        public const Int32 LUA_ENVIRONINDEX = -10001;
        public const Int32 LUA_GLOBALSINDEX = -10002;

        private static readonly Dictionary<Type, ILuaTypeMarshal> Marshals = new Dictionary<Type, ILuaTypeMarshal>();

        public static void RegisterMarshal(Type t, ILuaTypeMarshal marshal) => Marshals.Add(t, marshal);

        static GLua()
        {
            RegisterMarshal(typeof(String[]), new ArrayTypeMarshal<String>());
            RegisterMarshal(typeof(CFunc), new CFunctionTypeMarshal());
            RegisterMarshal(typeof(Delegate), new DelegateTypeMarshal());
            RegisterMarshal(typeof(String), new StringTypeMarshal());
            RegisterMarshal(typeof(Boolean), new BooleanTypeMarshal());

            RegisterMarshal(typeof(Double), new NumberTypeMarshal());
            RegisterMarshal(typeof(Single), new NumberTypeMarshal());
            RegisterMarshal(typeof(Int32), new NumberTypeMarshal());
            RegisterMarshal(typeof(Int64), new NumberTypeMarshal());

            InitJIT();
        }

        public Lua_State State { get; protected set; }

        public ILuaBase LuaBase { get; protected set; }

        private GLua(Lua_State luaState)
        {
            this.State = luaState;
            this.LuaBase = JIT.ConvertInstance<ILuaBase>(luaState.Luabase);
        }

        public T Get<T>(Int32 stackPos = -1) => (T)this.Get(typeof(T), stackPos);

        public void Push<T>(T obj) => this.Push(obj, typeof(T));

        public Object Get(Type type, Int32 stackPos = -1)
        {
            if (Marshals.ContainsKey(type))
            {
                ILuaTypeMarshal marshal = Marshals[type] as ILuaTypeMarshal;
                return Convert.ChangeType(marshal.Get(this, stackPos), type);
            }
            return null;
        }

        public void Push(Object obj, Type type)
        {
            if (Marshals.ContainsKey(type))
            {
                ILuaTypeMarshal marshal = Marshals[type] as ILuaTypeMarshal;
                marshal.Push(this, obj);
                return;
            }

            ILuaTypeMarshal backupMarshal = Marshals.Where(kv => type.IsCastableTo(kv.Key)).Select(kv => kv.Value).FirstOrDefault();
            if (backupMarshal != null)
            {
                backupMarshal.Push(this, obj);
            }
        }

        public void SetArray<T>(T[] array)
        {
            this.CreateTable();
            for (Int32 i = 0; i < array.Length; i++)
            {
                this.Push(array[i]);
                this.Push<Int32>(i);
                this.RawSet(-3);
                this.Pop(2);
            }
        }

        public T[] GetArray<T>()
        {
            Int32 count = this.ObjLen();
            T[] newArray = new T[count];
            for (Int32 i = 1; i <= count; i++)
            {
                this.Push<Int32>(i);
                this.RawGet(-2);
                newArray[i - 1] = this.Get<T>();
                this.Pop(2);
            }

            return newArray;
        }

        public T GetReturnType<T>() where T : new()
        {
            IEnumerable<PropertyInfo> properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(p => Attribute.IsDefined(p, typeof(ReturnIndexAttribute)));
            PropertyInfo[] pops = new PropertyInfo[properties.Count()];
            foreach (PropertyInfo prop in properties)
            {
                if (prop.GetCustomAttribute(typeof(ReturnIndexAttribute)) is ReturnIndexAttribute attr)
                {
                    pops[attr.ReturnIndex] = prop;
                }
            }
            Array.Reverse(pops);
            T newReturnObject = new T();
            for (Int32 i = 0; i < pops.Length; i++)
            {
                Object value = this.Get(pops[i].PropertyType);
                pops[i].SetValue(newReturnObject, value);
                this.Pop(1);
            }

            return newReturnObject;
        }

        public void SetGlobal<T>(String Name, T Obj)
        {
            this.Push(Obj);
            this.SetField(LUA_GLOBALSINDEX, Name);
        }

        public T GetGlobal<T>(String Name)
        {
            this.GetField(LUA_GLOBALSINDEX, Name);
            return this.Get<T>();
        }

        /// <summary>
        /// Runs the action for every "next", with key at -2 and the value at -1, on the table at the top of the stack
        /// </summary>
        /// <param name="action"></param>
        public void ForEach(Action action)
        {
            if (!this.IsType(-1, LuaType.Table)) { return; }
            this.PushNil();
            while (this.Next(-2) != 0)
            {
                action?.Invoke();
                this.Pop(1);
            }
        }

        #region ILuaBase Passthrough(Some of it)

        public void PushCFunction(CFunc val)
        {
            IntPtr ptr = Marshal.GetFunctionPointerForDelegate(val);
            this.LuaBase.PushCFunction(ptr);
        }
        public void PushCFunction(IntPtr val) => this.LuaBase.PushCFunction(val);

        public Int32 Top() => this.LuaBase.Top();

        public void Push(Int32 iStackPos) => this.LuaBase.Push(iStackPos);

        public void Pop(Int32 iAmt = 1) => this.LuaBase.Pop(iAmt);

        public void GetTable(Int32 iStackPos) => this.LuaBase.GetTable(iStackPos);

        public void GetField(Int32 iStackPos, String strName) => this.LuaBase.GetField(iStackPos, strName);

        public void SetField(Int32 iStackPos, String strName) => this.LuaBase.SetField(iStackPos, strName);

        public void CreateTable() => this.LuaBase.CreateTable();

        public void SetTable(Int32 i) => this.LuaBase.SetTable(i);

        public void SetMetaTable(Int32 i) => this.LuaBase.SetMetaTable(i);

        public Boolean GetMetaTable(Int32 i) => this.LuaBase.GetMetaTable(i);

        public void Call(Int32 iArgs, Int32 iResults) => this.LuaBase.Call(iArgs, iResults);

        public Int32 PCall(Int32 iArgs, Int32 iResults, Int32 iErrorFunc) => this.LuaBase.PCall(iArgs, iResults, iErrorFunc);

        public Int32 Equal(Int32 iA, Int32 iB) => this.LuaBase.Equal(iA, iB);

        public Int32 RawEqual(Int32 iA, Int32 iB) => this.LuaBase.RawEqual(iA, iB);

        public void Insert(Int32 iStackPos) => this.LuaBase.Insert(iStackPos);

        public void Remove(Int32 iStackPos) => this.LuaBase.Remove(iStackPos);

        public Int32 Next(Int32 iStackPos) => this.LuaBase.Next(iStackPos);

        public IntPtr NewUserdata(UInt32 iSize) => this.LuaBase.NewUserdata(iSize);

        public void ThrowError(String strError) => this.LuaBase.ThrowError(strError);

        public void CheckType(Int32 iStackPos, Int32 iType) => this.LuaBase.CheckType(iStackPos, iType);

        public void ArgError(Int32 iArgNum, String strMessage) => this.LuaBase.ArgError(iArgNum, strMessage);

        public void RawGet(Int32 iStackPos) => this.LuaBase.RawGet(iStackPos);

        public void RawSet(Int32 iStackPos) => this.LuaBase.RawSet(iStackPos);

        public IntPtr GetUserdata(Int32 iStackPos = -1) => this.LuaBase.GetUserdata(iStackPos);

        public void PushNil() => this.LuaBase.PushNil();

        public void PushUserdata(IntPtr pointer) => this.LuaBase.PushUserdata(pointer);

        public Int32 ReferenceCreate() => this.LuaBase.ReferenceCreate();

        public void ReferenceFree(Int32 i) => this.LuaBase.ReferenceFree(i);

        public void ReferencePush(Int32 i) => this.LuaBase.ReferencePush(i);

        public void PushSpecial(LuaType iType) => this.LuaBase.PushSpecial(iType);

        public Boolean IsType(Int32 iStackPos, LuaType iType) => this.LuaBase.IsType(iStackPos, iType);

        public LuaType GetLuaType(Int32 iStackPos = -1) => this.LuaBase.GetType(iStackPos);

        public String GetTypeName(LuaType iType) => this.LuaBase.GetTypeName(iType);

        public void CreateMetaTableType(String strName, LuaType iType) => this.LuaBase.CreateMetaTableType(strName, iType);

        public Int32 ObjLen(Int32 index = -1) => this.LuaBase.ObjLen(index);

        #endregion ILuaBase Passthrough(Some of it)
    }
}
