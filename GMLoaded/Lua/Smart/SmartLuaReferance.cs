using System;
using System.Collections.Generic;
using System.Text;

namespace GMLoaded.Lua.Smart
{
    public delegate void SmartLuaReferanceDestruct(SmartLuaReferance Source);

    /// <summary>
    /// KurzGesagt, std::shared_ptr but for lua objects
    /// </summary>
    public class SmartLuaReferance : IDisposable
    {
        public event SmartLuaReferanceDestruct OnDestruct;
        public readonly GLua Handle;
        public readonly Int32 Referance;

        public Int32 Referances { get; private set; }

        public SmartLuaReferance(GLua Lua, Int32 Referance)
        {
            this.Referance = Referance;
            this.Handle = Lua;
            this.Referances = 1;
            this.Handle.OnClose += this.Handle_OnClose;
        }

        private void Handle_OnClose(GLua GLua) => this.Dispose();

        public void Free()
        {
            this.Referances--;

            if (this.Referances == 0)
                this.Dispose();
        }

        public void Create() => this.Referances++;

        public void Dispose()
        {
            OnDestruct?.Invoke(this);
            this.Handle.LuaBase.ReferenceFree(this.Referance);
            this.Handle.OnClose -= this.Handle_OnClose;
        }
    }
}
