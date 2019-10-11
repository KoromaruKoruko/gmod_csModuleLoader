using System.ComponentModel;
using System.Runtime.InteropServices;
using System;
namespace GMLoaded.Source.Interaces
{
    public interface IBaseFileSystem
    {
        /// <param name='pOutput'></param>
        /// <param name='size'></param>
        /// <param name='file'></param>
        Int32 Read(IntPtr pOutput, Int32 size, IntPtr file);

        /// <param name='pInput'></param>
        /// <param name='size'></param>
        /// <param name='file'></param>
        Int32 Write(IntPtr pInput, Int32 size, IntPtr file);

        /// <param name='pFileName'></param>
        /// <param name='pOptions'></param>
        /// <param name='pathID'>Default: 0</param>
        IntPtr Open(String pFileName, String pOptions, [Optional()] [DefaultValue(0)] String pathID);

        /// <param name='file'></param>
        void Close(IntPtr file);

        /// <param name='file'></param>
        /// <param name='pos'></param>
        /// <param name='seekType'></param>
        void Seek(IntPtr file, Int32 pos, IntPtr seekType);

        /// <param name='file'></param>
        UInt32 Tell(IntPtr file);

        /// <param name='file'></param>
        UInt32 Size(IntPtr file);

        /// <param name='pFileName'></param>
        /// <param name='pPathID'>Default: 0</param>
        UInt32 Size(String pFileName, [Optional()] [DefaultValue(0)] String pPathID);

        /// <param name='file'></param>
        void Flush(IntPtr file);

        /// <param name='pFileName'></param>
        /// <param name='pPathID'>Default: 0</param>
        Boolean Precache(String pFileName, [Optional()] [DefaultValue(0)] String pPathID);

        /// <param name='pFileName'></param>
        /// <param name='pPathID'>Default: 0</param>
        Boolean FileExists(String pFileName, [Optional()] [DefaultValue(0)] String pPathID);

        /// <param name='pFileName'></param>
        /// <param name='pPathID'>Default: 0</param>
        Boolean IsFileWritable(String pFileName, [Optional()] [DefaultValue(0)] String pPathID);

        /// <param name='pFileName'></param>
        /// <param name='writable'></param>
        /// <param name='pPathID'>Default: 0</param>
        Boolean SetFileWritable(String pFileName, Boolean writable, [Optional()] [DefaultValue(0)] String pPathID);

        /// <param name='pFileName'></param>
        /// <param name='pPathID'>Default: 0</param>
        Int32 GetFileTime(String pFileName, [Optional()] [DefaultValue(0)] String pPathID);

        /// <param name='pFileName'></param>
        /// <param name='pPath'></param>
        /// <param name='buf'></param>
        /// <param name='nMaxBytes'>Default: 0</param>
        /// <param name='nStartingByte'>Default: 0</param>
        /// <param name='pfnAlloc'></param>
        Boolean ReadFile(String pFileName, String pPath, IntPtr buf, [Optional()] [DefaultValue(0)] Int32 nMaxBytes, [Optional()] [DefaultValue(0)] Int32 nStartingByte, IntPtr pfnAlloc);

        /// <param name='pFileName'></param>
        /// <param name='pPath'></param>
        /// <param name='buf'></param>
        Boolean WriteFile(String pFileName, String pPath, IntPtr buf);

        /// <param name='pFileName'></param>
        /// <param name='pPath'></param>
        /// <param name='pDestination'></param>
        Boolean UnzipFile(String pFileName, String pPath, String pDestination);
    }
}
