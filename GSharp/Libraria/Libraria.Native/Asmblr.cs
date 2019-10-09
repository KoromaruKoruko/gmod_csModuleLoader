using System;

namespace Libraria.Native
{
    public static class Asmblr
    {
        public static Byte[] CreateByteArray(params Object[] Args)
        {
            Byte[] Ret = new Byte[] { };

            for (Int32 i = 0; i < Args.Length; i++)
            {
                Type ArgType = Args[i].GetType();

                if (ArgType == typeof(Byte))
                    Ret = Ret.Append(new Byte[] { (Byte)Args[i] });
                else
                    Ret = Ret.Append((Byte[])typeof(BitConverter).GetMethod("GetBytes", new Type[] { Args[i].GetType() })
                        .Invoke(null, new Object[] { Args[i] }));
            }

            return Ret;
        }
    }
}