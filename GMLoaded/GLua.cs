using GMLoaded.Attributes;
using GMLoaded.Lua;
using GMLoaded.Lua.TypeMarshals;
using GMLoaded.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using GMLoaded.Lua.Smart;
using GMLoaded.Lua.LibraryWrappers;

namespace GMLoaded
{
    public delegate void Gluaclose(GLua GLua);

    public partial class GLua
    {
        #region Extended GLua (Mainly AsyncSupport and Library Wraping)

        private readonly ManualResetEvent mLock = new ManualResetEvent(false);

        private Thread tLock;

        public event Gluaclose OnClose;

        public Boolean IsLocked => this.tLock != null;

        public void Close()
        {
            OnClose?.Invoke(this);
            Gluas.Remove(this);
        }

        public unsafe Object Get(Int32 IStackPos = -1)
        {
            Boolean B = this.Lock();

            Object Ret = null;

            Int32 T = this.LuaBase.GetType(IStackPos);
            switch (T)
            {
                case -1:
                case 0:
                    if (B)
                        this.UnLock();
                    return null;

                case 1:
                    Ret = this.LuaBase.GetBool(IStackPos);
                    break;

                case 2:
                case 7: // need to make a userdata marshal
                    Ret = this.LuaBase.GetUserdata(IStackPos);
                    break;

                case 3:
                    Ret = this.LuaBase.GetNumber(IStackPos);
                    break;

                case 4:
                    Ret = this.LuaBase.GetString(IStackPos, IntPtr.Zero);
                    break;

                case 5:
                    Ret = this.Get<ITableBase>(IStackPos);
                    break;

                case 6:
                    Ret = Marshal.GetDelegateForFunctionPointer<CFunc>(this.LuaBase.GetCFunction(IStackPos));
                    break;

                case 8: // Thread
                    Ret = this.Get<ITableBase>(IStackPos);
                    break;

                case 9:
                    Ret = this.Get<Entity>(IStackPos);
                    break;

                case 10:
                    Ret = this.Get<Vector>(IStackPos);
                    break;

                case 11:
                    Ret = this.Get<Angle>(IStackPos);
                    break;

                case 12:
                    Ret = this.Get<PhysObj>(IStackPos);
                    break;

                case 13:
                    Ret = this.Get<ISave>(IStackPos);
                    break;

                case 14:
                    Ret = this.Get<IRestore>(IStackPos);
                    break;

                case 15:
                    Ret = this.Get<CTakeDamageInfo>(IStackPos);
                    break;

                case 16:
                    Ret = this.Get<CEffectData>(IStackPos);
                    break;

                case 17:
                    Ret = this.Get<CMoveData>(IStackPos);
                    break;

                case 18:
                    Ret = this.Get<CRecipientFilter>(IStackPos);
                    break;

                case 19:
                    Ret = this.Get<CUserCmd>(IStackPos);
                    break;

                case 20:
                    throw new InvalidCastException("This object ID:20 is pre Gmod13 and is not supported in GMLoaded");

                case 21:
                    Ret = this.Get<IMaterial>(IStackPos);
                    break;

                case 22:
                    Ret = this.Get<Panel>(IStackPos);
                    break;

                case 23:
                    Ret = this.Get<CLuaParticle>(IStackPos);
                    break;

                case 24:
                    Ret = this.Get<CLuaEmitter>(IStackPos);
                    break;

                case 25:
                    Ret = this.Get<ITexture>(IStackPos);
                    break;

                case 26:
                    Ret = this.Get<bf_read>(IStackPos);
                    break;

                case 27:
                    Ret = this.Get<ConVar>(IStackPos);
                    break;

                case 28:
                    Ret = this.Get<IMesh>(IStackPos);
                    break;

                case 29:
                    Ret = this.Get<VMatrix>(IStackPos);
                    break;

                case 30:
                    Ret = this.Get<CSoundPatch>(IStackPos);
                    break;

                case 31:
                    Ret = this.Get<pixelvis_handle_t>(IStackPos);
                    break;

                case 32:
                    Ret = this.Get<dlight_t>(IStackPos);
                    break;

                case 33:
                    Ret = this.Get<IVideoWriter>(IStackPos);
                    break;

                case 34:
                    Ret = this.Get<File>(IStackPos);
                    break;

                case 35:
                    Ret = this.Get<CLuaLocomotion>(IStackPos);
                    break;

                case 36:
                    Ret = this.Get<PathFollower>(IStackPos);
                    break;

                case 37:
                    Ret = this.Get<CNavArea>(IStackPos);
                    break;

                case 38:
                    Ret = this.Get<IGModAudioChannel>(IStackPos);
                    break;

                case 39:
                    Ret = this.Get<CNavLadder>(IStackPos);
                    break;

                case 40:
                    Ret = this.Get<CNewParticleEffect>(IStackPos);
                    break;

                case 41:
                    Ret = this.Get<ProjectedTexture>(IStackPos);
                    break;

                case 42:
                    Ret = this.Get<PhysCollide>(IStackPos);
                    break;

                case 43:
                    Ret = 45; // Amount of Type ENUMS might as well hard code it
                    break;

                case 255:
                    Ret = this.Get<Color>(IStackPos);
                    break;

                default:
                    throw new Exception("Unable to cast to base lua type, Unknown Type:" + T);
            }

            if (B)
                this.UnLock();

            return Ret;
        }

        public void GetGlobal(String Name) => this.GetField(LUA_GLOBALSINDEX, Name);

        #region Optimized Library Wrappers

        private Hooks _Hooks;

        public Hooks GetHooks() => this._Hooks ?? (this._Hooks = new Hooks(this));

        #endregion Optimized Library Wrappers

        #region Smart Lua Stack management

        public SmartLuaReferance CreateSmartReferance()
        {
            Int32 Ref = this.LuaBase.ReferenceCreate();
            return new SmartLuaReferance(this, Ref);
        }

        #endregion Smart Lua Stack management

        public Int32 GlobalRefrenceCreate(String Global)
        {
            Boolean b = this.Lock();

            this.GetGlobal(Global);
            Int32 R = this.ReferenceCreate();
            this.Pop();

            if (b)
                this.UnLock();

            return R;
        }

        /// <summary>
        /// this is to lock the Handler, this SHOULD be used for any multi-command operations on the stack.
        /// this will not lock out direct access to the stack, this is so that single command operations can be done.
        /// if this is called twice on the same thread it will parse through but the return will be false.
        /// use the return to see if you should call unlock. <see langword="true"/> = Unlock, <see langword="false"/> = Dont Unlock.
        /// </summary>
        public Boolean Lock()
        {
            Thread CT = Thread.CurrentThread;
            if (this.tLock == CT)
                return false;
            while (true)
            {
                this.mLock.WaitOne();
                lock (this.mLock)
                    if (this.mLock.Reset())
                    {
                        this.tLock = CT;
                        return true;
                    }
            }
        }

        public Object PopGet()
        {
            Boolean B = this.Lock();

            Object O = this.Get();
            this.Pop();

            if (B)
                this.UnLock();

            return O;
        }

        public void UnLock()
        {
            this.tLock = null;
            this.mLock.Set();
        }

        #endregion Extended GLua (Mainly AsyncSupport and Library Wraping)

        private static readonly List<GLua> Gluas = new List<GLua>();

        private static readonly Dictionary<Type, ILuaTypeMarshal> Marshals = new Dictionary<Type, ILuaTypeMarshal>();

        public const Int32 LUA_ENVIRONINDEX = -10001;

        public const Int32 LUA_GLOBALSINDEX = -10002;

        public const Int32 LUA_REGISTRYINDEX = -10000;

        private GLua(Lua_State luaState)
        {
            this.State = luaState;
            this.LuaBase = JIT.ConvertInstance<ILuaBase>(luaState.Luabase);
        }

        static GLua()
        {
            RegisterMarshal(typeof(String[]), ArrayTypeMarshal<String>.Create());
            RegisterMarshal(typeof(CFunc), CFunctionTypeMarshal.Create());
            RegisterMarshal(typeof(Delegate), DelegateTypeMarshal.Create());
            RegisterMarshal(typeof(String), StringTypeMarshal.Create());
            RegisterMarshal(typeof(Boolean), BooleanTypeMarshal.Create());
            RegisterMarshal(typeof(Double), NumberTypeMarshal.Create());
            RegisterMarshal(typeof(Single), NumberTypeMarshal.Create());
            RegisterMarshal(typeof(Int64), NumberTypeMarshal.Create());
            RegisterMarshal(typeof(UInt64), NumberTypeMarshal.Create());
            RegisterMarshal(typeof(Int32), NumberTypeMarshal.Create());
            RegisterMarshal(typeof(UInt32), NumberTypeMarshal.Create());
            RegisterMarshal(typeof(Int16), NumberTypeMarshal.Create());
            RegisterMarshal(typeof(UInt16), NumberTypeMarshal.Create());
            RegisterMarshal(typeof(SByte), NumberTypeMarshal.Create());
            RegisterMarshal(typeof(Byte), NumberTypeMarshal.Create());
            RegisterMarshal(typeof(Color), ColorTypeMarshal.Create());
            RegisterMarshal(typeof(Vector), VectorTypeMarshal.Create());
            RegisterMarshal(typeof(Angle), AngleTypeMarshal.Create());

            RegisterMarshal(typeof(ITableBase), TableTypeMarshal.Create());
            RegisterMarshal(typeof(PhysCollide), GenericTableTypeMarshal.Create<PhysCollide>());
            RegisterMarshal(typeof(Entity), GenericTableTypeMarshal.Create<Entity>());
            RegisterMarshal(typeof(PhysObj), GenericTableTypeMarshal.Create<PhysObj>());
            RegisterMarshal(typeof(ISave), GenericTableTypeMarshal.Create<ISave>());
            RegisterMarshal(typeof(IRestore), GenericTableTypeMarshal.Create<IRestore>());
            RegisterMarshal(typeof(CTakeDamageInfo), GenericTableTypeMarshal.Create<CTakeDamageInfo>());
            RegisterMarshal(typeof(CEffectData), GenericTableTypeMarshal.Create<CEffectData>());
            RegisterMarshal(typeof(CMoveData), GenericTableTypeMarshal.Create<CMoveData>());
            RegisterMarshal(typeof(CRecipientFilter), GenericTableTypeMarshal.Create<CRecipientFilter>());
            RegisterMarshal(typeof(CUserCmd), GenericTableTypeMarshal.Create<CUserCmd>());
            RegisterMarshal(typeof(IMaterial), GenericTableTypeMarshal.Create<IMaterial>());
            RegisterMarshal(typeof(Panel), GenericTableTypeMarshal.Create<Panel>());
            RegisterMarshal(typeof(CLuaParticle), GenericTableTypeMarshal.Create<CLuaParticle>());
            RegisterMarshal(typeof(CLuaEmitter), GenericTableTypeMarshal.Create<CLuaEmitter>());
            RegisterMarshal(typeof(ITexture), GenericTableTypeMarshal.Create<ITexture>());
            RegisterMarshal(typeof(bf_read), GenericTableTypeMarshal.Create<bf_read>());
            RegisterMarshal(typeof(ConVar), GenericTableTypeMarshal.Create<ConVar>());
            RegisterMarshal(typeof(IMesh), GenericTableTypeMarshal.Create<IMesh>());
            RegisterMarshal(typeof(VMatrix), GenericTableTypeMarshal.Create<VMatrix>());
            RegisterMarshal(typeof(CSoundPatch), GenericTableTypeMarshal.Create<CSoundPatch>());
            RegisterMarshal(typeof(pixelvis_handle_t), GenericTableTypeMarshal.Create<pixelvis_handle_t>());
            RegisterMarshal(typeof(dlight_t), GenericTableTypeMarshal.Create<dlight_t>());
            RegisterMarshal(typeof(IVideoWriter), GenericTableTypeMarshal.Create<IVideoWriter>());
            RegisterMarshal(typeof(File), GenericTableTypeMarshal.Create<File>());
            RegisterMarshal(typeof(CLuaLocomotion), GenericTableTypeMarshal.Create<CLuaLocomotion>());
            RegisterMarshal(typeof(PathFollower), GenericTableTypeMarshal.Create<PathFollower>());
            RegisterMarshal(typeof(CNavArea), GenericTableTypeMarshal.Create<CNavArea>());
            RegisterMarshal(typeof(IGModAudioChannel), GenericTableTypeMarshal.Create<IGModAudioChannel>());
            RegisterMarshal(typeof(CNavLadder), GenericTableTypeMarshal.Create<CNavLadder>());
            RegisterMarshal(typeof(CNewParticleEffect), GenericTableTypeMarshal.Create<CNewParticleEffect>());
            RegisterMarshal(typeof(ProjectedTexture), GenericTableTypeMarshal.Create<ProjectedTexture>());

            RegisterMarshal(typeof(Player), GenericTableTypeMarshal.Create<Player>());
            InitJIT();
        }

        public ILuaBase LuaBase { get; protected set; }

        public Lua_State State { get; protected set; }

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

        public static void RegisterMarshal(Type t, ILuaTypeMarshal marshal) => Marshals.Add(t, marshal);

        /// <summary>
        /// Runs the action for every "next", with key at -2 and the value at -1, on the table at the top of the stack
        /// </summary>
        /// <param name="action"></param>
        public void ForEach(Action action)
        {
            if (!this.IsType(-1, LuaType.Table)) return;
            this.PushNil();
            while (this.Next(-2) != 0)
            {
                action?.Invoke();
                this.Pop(1);
            }
        }

        public T Get<T>(Int32 stackPos = -1) => (T)this.Get(typeof(T), stackPos);

        public Object Get(Type type, Int32 stackPos = -1)
        {
            if (Marshals.ContainsKey(type))
            {
                ILuaTypeMarshal marshal = Marshals[type];
                return Convert.ChangeType(marshal.Get(this, stackPos), type);
            }
            return null;
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

        public T GetGlobal<T>(String Name)
        {
            this.GetField(LUA_GLOBALSINDEX, Name);
            return this.Get<T>();
        }

        public T GetReturnType<T>() where T : new()
        {
            IEnumerable<PropertyInfo> properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(p => Attribute.IsDefined(p, typeof(ReturnIndexAttribute)));
            PropertyInfo[] pops = new PropertyInfo[properties.Count()];

            foreach (PropertyInfo prop in properties)
                if (prop.GetCustomAttribute(typeof(ReturnIndexAttribute)) is ReturnIndexAttribute attr)
                    pops[attr.ReturnIndex] = prop;

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

        public void Push<T>(T obj) => this.Push(obj, typeof(T));

        public void Push(Object obj, Type type)
        {
            if (Marshals.ContainsKey(type))
            {
                ILuaTypeMarshal marshal = Marshals[type] as ILuaTypeMarshal;
                marshal.Push(this, obj);
            }
            else
            {
                ILuaTypeMarshal backupMarshal = Marshals.Where(kv => type.IsCastableTo(kv.Key)).Select(kv => kv.Value).FirstOrDefault();
                if (backupMarshal != null)
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

        public void SetGlobal<T>(String Name, T Obj)
        {
            this.Push(Obj);
            this.SetField(LUA_GLOBALSINDEX, Name);
        }

        #region ILuaBase Passthrough(Some of it)

        public void ArgError(Int32 iArgNum, String strMessage) => this.LuaBase.ArgError(iArgNum, strMessage);

        public void Call(Int32 iArgs, Int32 iResults) => this.LuaBase.Call(iArgs, iResults);

        public void CheckType(Int32 iStackPos, Int32 iType) => this.LuaBase.CheckType(iStackPos, iType);

        public void CreateMetaTableType(String strName, LuaType iType) => this.LuaBase.CreateMetaTableType(strName, iType);

        public void CreateTable() => this.LuaBase.CreateTable();

        public Int32 Equal(Int32 iA, Int32 iB) => this.LuaBase.Equal(iA, iB);

        public void GetField(Int32 iStackPos, String strName) => this.LuaBase.GetField(iStackPos, strName);

        public LuaType GetLuaType(Int32 iStackPos = -1) => this.LuaBase.GetType(iStackPos);

        public Boolean GetMetaTable(Int32 i) => this.LuaBase.GetMetaTable(i);

        public void GetTable(Int32 iStackPos) => this.LuaBase.GetTable(iStackPos);

        public String GetTypeName(LuaType iType) => this.LuaBase.GetTypeName(iType);

        public IntPtr GetUserdata(Int32 iStackPos = -1) => this.LuaBase.GetUserdata(iStackPos);

        public void Insert(Int32 iStackPos) => this.LuaBase.Insert(iStackPos);

        public Boolean IsType(Int32 iStackPos, LuaType iType) => this.LuaBase.IsType(iStackPos, iType);

        public IntPtr NewUserdata(UInt32 iSize) => this.LuaBase.NewUserdata(iSize);

        public Int32 Next(Int32 iStackPos) => this.LuaBase.Next(iStackPos);

        public Int32 ObjLen(Int32 index = -1) => this.LuaBase.ObjLen(index);

        public Int32 PCall(Int32 iArgs, Int32 iResults, Int32 iErrorFunc) => this.LuaBase.PCall(iArgs, iResults, iErrorFunc);

        public void Pop(Int32 iAmt = 1) => this.LuaBase.Pop(iAmt);

        public void Push(Int32 iStackPos) => this.LuaBase.Push(iStackPos);

        public void PushCFunction(CFunc val)
        {
            IntPtr ptr = Marshal.GetFunctionPointerForDelegate(val);
            this.LuaBase.PushCFunction(ptr);
        }

        public void PushCFunction(IntPtr val) => this.LuaBase.PushCFunction(val);

        public void PushNil() => this.LuaBase.PushNil();

        public void PushSpecial(LuaType iType) => this.LuaBase.PushSpecial(iType);

        public void PushUserdata(IntPtr pointer) => this.LuaBase.PushUserdata(pointer);

        public Int32 RawEqual(Int32 iA, Int32 iB) => this.LuaBase.RawEqual(iA, iB);

        public void RawGet(Int32 iStackPos) => this.LuaBase.RawGet(iStackPos);

        public void RawSet(Int32 iStackPos) => this.LuaBase.RawSet(iStackPos);

        public Int32 ReferenceCreate() => this.LuaBase.ReferenceCreate();

        public void ReferenceFree(Int32 i) => this.LuaBase.ReferenceFree(i);

        public void ReferencePush(Int32 i) => this.LuaBase.ReferencePush(i);

        public void Remove(Int32 iStackPos) => this.LuaBase.Remove(iStackPos);

        public void SetField(Int32 iStackPos, String strName) => this.LuaBase.SetField(iStackPos, strName);

        public void SetMetaTable(Int32 i) => this.LuaBase.SetMetaTable(i);

        public void SetTable(Int32 i) => this.LuaBase.SetTable(i);

        public void ThrowError(String strError) => this.LuaBase.ThrowError(strError);

        public Int32 Top() => this.LuaBase.Top();

        #endregion ILuaBase Passthrough(Some of it)
    }
}
