using GMLoaded.Lua;
using GMLoaded.Native;
using System;
using System.IO;
using System.Text;

namespace GMLoaded.Source
{
    /// <summary>
    /// Installable wrapper for the Client Console,
    ///     Otherwise not needed.
    /// </summary>
    /// <remarks>It will not work if started by the server, and STD IO doesnt work if using hl2 host</remarks>
    public static class GModClientConsole
    {
        private static IGameConsoleTextWriter GameConsoleWriter;

        public static Color Color
        {
            get => GameConsoleWriter == null ? new Color() : GameConsoleWriter.Color;

            set
            {
                if (GameConsoleWriter != null)
                    GameConsoleWriter.Color = value;
            }
        }

        public static IGameConsole RerouteConsole()
        {
            if (GameConsoleWriter == null)
            {
                IGameConsole GameConsole = Natives.SystemType switch
                {
                    System.Windows => InterfaceLoader.Load<IGameConsole>("gameui.dll"),
                    _ => throw new PlatformNotSupportedException("Need the lib name/path")
                };
                GameConsole.Activate();
                Console.SetOut(GameConsoleWriter = new IGameConsoleTextWriter(GameConsole));
            }

            return GameConsoleWriter.GameConsole;
        }

        private class IGameConsoleTextWriter : TextWriter
        {
            public Color Color;
            public IGameConsole GameConsole;

            public IGameConsoleTextWriter(IGameConsole console) => this.GameConsole = console;

            public override Encoding Encoding => throw new NotImplementedException();

            public override void Write(Char value) =>
                Tier0.ConColorMsg(ref this.Color, value.ToString(), null);

            public override void Write(String value) => Tier0.ConColorMsg(ref this.Color, value, null);

            public override void WriteLine(String value) => Tier0.ConColorMsg(ref this.Color, value + "\n", null);
        }
    }
}
