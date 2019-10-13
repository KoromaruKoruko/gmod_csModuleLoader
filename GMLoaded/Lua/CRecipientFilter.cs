using System;
using GMLoaded.Lua.TypeMarshals;

namespace GMLoaded.Lua
{
    public class CRecipientFilter : ITableBase
    {
        public CRecipientFilter(GLua Glua, Int32 IStackPos) : base(Glua, IStackPos)
        {
        }

        public void AddAllPlayers()
        {
            Boolean B = this.LuaHandle.Lock();
            this.LuaHandle.ReferencePush(this.Referance);
            this.LuaHandle.GetField(-1, "AddAllPlayers");
            this.LuaHandle.Insert(-2);
            this.LuaHandle.Call(1, 0);
            if (B)
                this.LuaHandle.UnLock();
        }

        public void AddPAS(Vector Pos)
        {
            Boolean B = this.LuaHandle.Lock();
            this.LuaHandle.ReferencePush(this.Referance);
            this.LuaHandle.GetField(-1, "AddPAS");
            this.LuaHandle.Insert(-2);
            VectorTypeMarshal.Create().Push(this.LuaHandle, Pos);
            this.LuaHandle.Call(2, 0);
            if (B)
                this.LuaHandle.UnLock();
        }

        public void AddPlayer(Player Ply)
        {
            Boolean B = this.LuaHandle.Lock();
            this.LuaHandle.ReferencePush(this.Referance);
            this.LuaHandle.GetField(-1, "AddPlayer");
            this.LuaHandle.Insert(-2);
            GenericTableTypeMarshal.Create<Player>().Push(this.LuaHandle, Ply);
            this.LuaHandle.Call(2, 0);
            if (B)
                this.LuaHandle.UnLock();
        }

        public void AddPVS(Vector Pos)
        {
            Boolean B = this.LuaHandle.Lock();
            this.LuaHandle.ReferencePush(this.Referance);
            this.LuaHandle.GetField(-1, "AddPVS");
            this.LuaHandle.Insert(-2);
            VectorTypeMarshal.Create().Push(this.LuaHandle, Pos);
            this.LuaHandle.Call(2, 0);
            if (B)
                this.LuaHandle.UnLock();
        }

        public void AddRecipientsByTeam(Double ID)
        {
            Boolean B = this.LuaHandle.Lock();
            this.LuaHandle.ReferencePush(this.Referance);
            this.LuaHandle.GetField(-1, "AddRecipientsByTeam");
            this.LuaHandle.Insert(-2);
            this.LuaHandle.LuaBase.PushNumber(ID);
            this.LuaHandle.Call(2, 0);
            if (B)
                this.LuaHandle.UnLock();
        }

        public Double GetCount()
        {
            Boolean B = this.LuaHandle.Lock();
            this.LuaHandle.ReferencePush(this.Referance);
            this.LuaHandle.GetField(-1, "GetCount");
            this.LuaHandle.Insert(-2);
            this.LuaHandle.Call(1, 1);
            Double Ret = this.LuaHandle.LuaBase.GetNumber();
            if (B)
                this.LuaHandle.UnLock();
            return Ret;
        }

        public Player[] GetPlayers()
        {
            Boolean B = this.LuaHandle.Lock();
            this.LuaHandle.ReferencePush(this.Referance);
            this.LuaHandle.GetField(-1, "GetPlayers");
            this.LuaHandle.Insert(-2);
            this.LuaHandle.Call(1, 1);
            Player[] Ret = ArrayTypeMarshal<Player>.Create().GetT(this.LuaHandle);
            if (B)
                this.LuaHandle.UnLock();
            return Ret;
        }

        public void RemoveAllPlayers()
        {
            Boolean B = this.LuaHandle.Lock();
            this.LuaHandle.ReferencePush(this.Referance);
            this.LuaHandle.GetField(-1, "RemoveAllPlayers");
            this.LuaHandle.Insert(-2);
            this.LuaHandle.Call(1, 0);
            if (B)
                this.LuaHandle.UnLock();
        }

        public void RemovePAS(Vector Pos)
        {
            Boolean B = this.LuaHandle.Lock();
            this.LuaHandle.ReferencePush(this.Referance);
            this.LuaHandle.GetField(-1, "RemovePAS");
            this.LuaHandle.Insert(-2);
            VectorTypeMarshal.Create().Push(this.LuaHandle, Pos);
            this.LuaHandle.Call(2, 0);
            if (B)
                this.LuaHandle.UnLock();
        }

        public void RemovePlayer(Player Ply)
        {
            Boolean B = this.LuaHandle.Lock();
            this.LuaHandle.ReferencePush(this.Referance);
            this.LuaHandle.GetField(-1, "RemovePlayer");
            this.LuaHandle.Insert(-2);
            GenericTableTypeMarshal.Create<Player>().Push(this.LuaHandle, Ply);
            this.LuaHandle.Call(2, 0);
            if (B)
                this.LuaHandle.UnLock();
        }

        public void RemovePVS(Vector Pos)
        {
            Boolean B = this.LuaHandle.Lock();
            this.LuaHandle.ReferencePush(this.Referance);
            this.LuaHandle.GetField(-1, "RemovePVS");
            this.LuaHandle.Insert(-2);
            VectorTypeMarshal.Create().Push(this.LuaHandle, Pos);
            this.LuaHandle.Call(2, 0);
            if (B)
                this.LuaHandle.UnLock();
        }

        public void RemoveRecipientsByTeam(Double ID)
        {
            Boolean B = this.LuaHandle.Lock();
            this.LuaHandle.ReferencePush(this.Referance);
            this.LuaHandle.GetField(-1, "RemoveRecipientsByTeam");
            this.LuaHandle.Insert(-2);
            this.LuaHandle.LuaBase.PushNumber(ID);
            this.LuaHandle.Call(2, 0);
            if (B)
                this.LuaHandle.UnLock();
        }

        public void RemoveRecipientsNotOnTeam(Double ID)
        {
            Boolean B = this.LuaHandle.Lock();
            this.LuaHandle.ReferencePush(this.Referance);
            this.LuaHandle.GetField(-1, "RemoveRecipientsNotOnTeam");
            this.LuaHandle.Insert(-2);
            this.LuaHandle.LuaBase.PushNumber(ID);
            this.LuaHandle.Call(2, 0);
            if (B)
                this.LuaHandle.UnLock();
        }
    }
}
