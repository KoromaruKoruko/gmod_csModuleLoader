using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace GMLoaded.Lua
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Color
    {
        /// <summary>
        /// Red Color tone, 0-255
        /// </summary>
        [MarshalAs(UnmanagedType.U1)]
        public Byte R;

        /// <summary>
        /// Green Color tone, 0-255
        /// </summary>
        [MarshalAs(UnmanagedType.U1)]
        public Byte G;

        /// <summary>
        /// Blue Color tone, 0-255
        /// </summary>
        [MarshalAs(UnmanagedType.U1)]
        public Byte B;

        /// <summary>
        /// Alpha (Opacity) tone, 0-255
        /// </summary>
        [MarshalAs(UnmanagedType.U1)]
        public Byte A;

        public Color(GLua gLua, Int32 IStackPos)
        {
            gLua.Lock();

            gLua.GetField(IStackPos, "r");
            this.R = (Byte)gLua.LuaBase.GetNumber(-1);
            gLua.GetField(IStackPos, "g");
            this.G = (Byte)gLua.LuaBase.GetNumber(-1);
            gLua.GetField(IStackPos, "b");
            this.B = (Byte)gLua.LuaBase.GetNumber(-1);
            gLua.GetField(IStackPos, "a");
            this.A = (Byte)gLua.LuaBase.GetNumber(-1);
            gLua.Pop(4);

            gLua.UnLock();
        }

        public void Push(GLua gLua)
        {
            gLua.Lock();

            gLua.LuaBase.CreateTable();
            gLua.LuaBase.PushNumber(this.R);
            gLua.LuaBase.SetField(-2, "r");
            gLua.LuaBase.PushNumber(this.G);
            gLua.LuaBase.SetField(-2, "g");
            gLua.LuaBase.PushNumber(this.B);
            gLua.LuaBase.SetField(-2, "b");
            gLua.LuaBase.PushNumber(this.A);
            gLua.LuaBase.SetField(-2, "a");

            gLua.LuaBase.CreateMetaTable("COLOR");
            gLua.LuaBase.SetMetaTable(-2);

            gLua.UnLock();
        }

        /// <summary></summary>
        /// <param name="R">Red Color tone 0-255</param>
        /// <param name="G">Green Color tone 0-255</param>
        /// <param name="B">Blue Color tone 0-255</param>
        /// <param name="A">Alpha (Opacity) tone 0-255</param>
        public Color(Byte R, Byte G, Byte B, Byte A)
        {
            this.R = R;
            this.G = G;
            this.B = B;
            this.A = A;
        }

        /// <summary></summary>
        /// <remarks>Altha tone is 255 (FULL_COLOR)</remarks>
        /// <param name="R">Red Color tone 0-255</param>
        /// <param name="G">Green Color tone 0-255</param>
        /// <param name="B">Blue Color tone 0-255</param>
        public Color(Byte R, Byte G, Byte B) : this(R, G, B, 255)
        {
        }

        public static implicit operator Color(ConsoleColor ConColor) => ConColor switch
        {
            ConsoleColor.Black => new Color(0x00, 0x00, 0x0),

            ConsoleColor.DarkBlue => new Color(0x00, 0x00, 0x8B),

            ConsoleColor.DarkGreen => new Color(0x00, 0x64, 0x00),

            ConsoleColor.DarkCyan => new Color(0x00, 0x8B, 0x8B),

            ConsoleColor.DarkRed => new Color(0x8B, 0x00, 0x00),

            ConsoleColor.DarkMagenta => new Color(0x8B, 0x00, 0x8B),

            ConsoleColor.DarkYellow => new Color(0xD7, 0xC3, 0x2A),

            ConsoleColor.Gray => new Color(0x80, 0x80, 0x80),

            ConsoleColor.DarkGray => new Color(0xA9, 0xA9, 0xA9),

            ConsoleColor.Blue => new Color(0x00, 0x00, 0xFF),

            ConsoleColor.Green => new Color(0x00, 0xFF, 0x00),

            ConsoleColor.Cyan => new Color(0x00, 0xFF, 0xFF),

            ConsoleColor.Red => new Color(0xFF, 0x00, 0x00),

            ConsoleColor.Magenta => new Color(0xFF, 0x00, 0xFF),

            ConsoleColor.Yellow => new Color(0xFF, 0xFF, 0x00),

            ConsoleColor.White => new Color(0xFF, 0xFF, 0xFF),

            _ => throw new NotImplementedException(),
        };

        public static implicit operator Color(global::System.Drawing.Color C) => new Color(C.R, C.G, C.B, C.A);

        public static implicit operator global::System.Drawing.Color(Color C) => global::System.Drawing.Color.FromArgb(C.A, C.R, C.G, C.B);
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Vector
    {
        public Single X;
        public Single Y;
        public Single Z;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Angle
    {
        public Single Pitch;
        public Single Yaw;
        public Single Roll;
    }
}
