using System;
namespace GMLoaded.Native
{
    public static class Asmblr
    {
        public static Byte[] CreateByteArray(params Object[] Args)
        {
            Byte[] Ret = new Byte[] { };

            for (Int32 i = 0; i < Args.Length; i++)
                Ret = Args[i].GetType() == typeof(Byte)
                    ? Ret.Append(new Byte[] { (Byte)Args[i] }) // this is done like so because Args is Object and BitCovnerter doesnt support Objects
                    : Ret.Append((Byte[])typeof(BitConverter).GetMethod("GetBytes", new Type[] { Args[i].GetType() }).Invoke(null, new Object[] { Args[i] }));

            return Ret;
        }
    }
}
