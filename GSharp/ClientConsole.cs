﻿using GSharp.Native;
using GSharp.Native.Classes;

using System;
using System.IO;
using System.Text;

namespace GSharp
{
    public static class ClientConsole
    {
        private class IGameConsoleTextWriter : TextWriter
        {
            public IGameConsole GameConsole;
            public Color Color;
            public override Encoding Encoding => throw new NotImplementedException();

            public IGameConsoleTextWriter(IGameConsole console) => this.GameConsole = console;

            public override void Write(Char value) =>
                //console.Printf("%s", value.ToString());
                Tier0.ConColorMsg(0, ref this.Color, value.ToString());

            public override void Write(String value) => Tier0.ConColorMsg(0, ref this.Color, value);

            public override void WriteLine(String value) => Tier0.ConColorMsg(0, ref this.Color, value + "\n");
        }

#if CLIENT
        static IGameConsoleTextWriter GameConsoleWriter;
        public static IGameConsole RerouteConsole()
        {
            if (GameConsoleWriter == null)
            {
                IGameConsole GameConsole = NativeInterface.Load<IGameConsole>("gameui.dll");
                GameConsole.Activate();
                Console.SetOut(GameConsoleWriter = new IGameConsoleTextWriter(GameConsole));
            }

            return GameConsoleWriter.GameConsole;
        }

        public static Color Color
        {
            get
            {
                if (GameConsoleWriter == null)
                    return new Color();
                return GameConsoleWriter.Color;
            }

            set
            {
                if (GameConsoleWriter != null)
                    GameConsoleWriter.Color = value;
            }
        }
#endif
    }
}