using System;
using System.Runtime.InteropServices;

namespace GSharp.Native.Classes
{
    /// <summary>
    /// Color class (RGB), parseable to LUA and containing auto cast from <see cref="ConsoleColor"/>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct Color // Unknown Reason for unsafe, no unsafe operations
    {
        /// <summary>
        /// Red Color tone, 0-255
        /// </summary>
        public Byte R;
        /// <summary>
        /// Green Color tone, 0-255
        /// </summary>
        public Byte G;
        /// <summary>
        /// Blue Color tone, 0-255
        /// </summary>
        public Byte B;
        /// <summary>
        /// Alpha (Opacity) tone, 0-255
        /// </summary>
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

        // implicit auto cast from System.ConsoleColor to GSharp.Native.Classes.Color
        // Note: used from c#8.0 | netstd3.0 | coreclr(dotnet core)3.0
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
    }
}
