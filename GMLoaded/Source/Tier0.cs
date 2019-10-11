using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace GMLoaded.Source
{
    namespace Native
    {
        public static class Tier0_lin
        {
            [DllImport("libtier0.so", CallingConvention = CallingConvention.Cdecl)]
            public static extern void ConColorMsg(ref Color Color, String Fmt, String Arg0);

            [DllImport("libtier0.so", CallingConvention = CallingConvention.Cdecl)]
            public static extern void ConDMsg(String Fmt, String Arg0);

            [DllImport("libtier0.so", CallingConvention = CallingConvention.Cdecl)]
            public static extern void ConLog(Int32 Lvl, String Fmt, String Arg0);

            [DllImport("libtier0.so", CallingConvention = CallingConvention.Cdecl)]
            public static extern void ConMsg(Int32 lvl, String Fmt, String Arg0);

            [DllImport("libtier0.so", CallingConvention = CallingConvention.Cdecl)]
            public static extern void DevMsg(String Fmt, String Arg0);

            [DllImport("libtier0.so", CallingConvention = CallingConvention.Cdecl)]
            public static extern void DevWarning(String Fmt, String Arg0);
        }

        public static class Tier0_win
        {
            [DllImport("tier0.dll", CallingConvention = CallingConvention.Cdecl)]
            public static extern void ConColorMsg(ref Color Color, String Fmt, String Arg0);

            [DllImport("tier0.dll", CallingConvention = CallingConvention.Cdecl)]
            public static extern void ConDMsg(String Fmt, String Arg0);

            [DllImport("tier0.dll", CallingConvention = CallingConvention.Cdecl)]
            public static extern void ConLog(Int32 Lvl, String Fmt, String Arg0);

            [DllImport("tier0.dll", CallingConvention = CallingConvention.Cdecl)]
            public static extern void ConMsg(Int32 lvl, String Fmt, String Arg0);

            [DllImport("tier0.dll", CallingConvention = CallingConvention.Cdecl)]
            public static extern void DevMsg(String Fmt, String Arg0);

            [DllImport("tier0.dll", CallingConvention = CallingConvention.Cdecl)]
            public static extern void DevWarning(String Fmt, String Arg0);
        }
    }

    /// <summary>
    /// Color class (RGB), parseable to gmod and containing auto cast from <see cref="ConsoleColor"/> and <see cref="global::System.Drawing.Color"/>
    /// </summary>
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

    // im forced to do the ugly here, cant marshal generic delegates,

    [StaticInterface(typeof(Native.Tier0_win), typeof(Native.Tier0_lin))]
    public static class Tier0
    {
        public static void ConColorMsg(ref Color Color, String Fmt, String Arg0) => throw new NotLoadedException();

        public static void ConDMsg(String Fmt, String Arg0) => throw new NotLoadedException();

        public static void ConLog(Int32 Lvl, String Fmt, String Arg0) => throw new NotLoadedException();

        public static void ConMsg(Int32 Lvl, String Fmt, String Arg0) => throw new NotLoadedException();

        public static void DevMsg(String Fmt, String Arg0) => throw new NotLoadedException();

        public static void DevWarning(String Fmt, String Arg0) => throw new NotLoadedException();
    }
}
