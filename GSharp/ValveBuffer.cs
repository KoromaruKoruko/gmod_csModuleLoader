using System;
using System.IO;
using System.Text;

namespace GSharp
{
    public class ValveBuffer : IDisposable
    {
        readonly MemoryStream stream;
        readonly BinaryWriter writer;

        public ValveBuffer()
        {
            this.stream = new MemoryStream();
            this.writer = new BinaryWriter(this.stream);
        }

        public void WriteByte(Byte val) => this.writer.Write(val);

        public void WriteShort(Int16 val) => this.writer.Write(val);

        public void WriteLong(Int32 val) => this.writer.Write(val);

        public void WriteFloat(Single val) => this.writer.Write(val);

        public void WriteLongLong(Int64 val) => this.writer.Write(val);

        public void WriteString(String val)
        {
            this.writer.Write(Encoding.UTF8.GetBytes(val));
            this.writer.Write((Byte)0x00);
        }

        public Byte[] ToArray() => this.stream.ToArray();

        public void Dispose()
        {
            this.writer.Dispose();
            this.stream.Dispose();
        }
    }
}