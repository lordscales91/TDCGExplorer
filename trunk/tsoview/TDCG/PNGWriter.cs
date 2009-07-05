using System;
using System.Collections.Generic;
using System.IO;
//using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Checksums;
//using ICSharpCode.SharpZipLib.Zip.Compression.Streams;

namespace TDCG
{
    /// <summary>
    /// PNGFile�������o�����\�b�h�Q
    /// </summary>
    public class PNGWriter
    {
        /// <summary>
        /// CSC�`�F�b�N���s���I�u�W�F�N�g
        /// </summary>
        protected static Crc32 crc = new Crc32();

        /// <summary>
        /// �w�胉�C�^��byte�z��������o���܂��B
        /// </summary>
        /// <param name="bw">���C�^</param>
        /// <param name="bytes">bute�z��</param>
        public static void Write(BinaryWriter bw, byte[] bytes)
        {
            if (bw != null)
                bw.Write(bytes);
        }
        /// <summary>
        /// �w�胉�C�^�Ƀ`�����N�������o���܂��B
        /// </summary>
        /// <param name="bw">���C�^</param>
        /// <param name="type">�`�����N�^�C�v</param>
        /// <param name="chunk_data">�`�����N</param>
        public static void WriteChunk(BinaryWriter bw, string type, byte[] chunk_data)
        {
            byte[] buf = BitConverter.GetBytes((UInt32)chunk_data.Length);
            Array.Reverse(buf);
            Write(bw, buf);

            byte[] chunk_type = System.Text.Encoding.ASCII.GetBytes(type);
            Write(bw, chunk_type);
            Write(bw, chunk_data);

            crc.Reset();
            crc.Update(chunk_type);
            crc.Update(chunk_data);

            byte[] crc_buf = BitConverter.GetBytes((UInt32)crc.Value);
            Array.Reverse(crc_buf);
            Write(bw, crc_buf);
        }
        /// <summary>
        /// �w�胉�C�^��IHDR�`�����N�������o���܂��B
        /// </summary>
        /// <param name="bw">���C�^</param>
        /// <param name="chunk_data">�`�����N</param>
        public static void WriteIHDR(BinaryWriter bw, byte[] chunk_data)
        {
            WriteChunk(bw, "IHDR", chunk_data);
        }
        /// <summary>
        /// �w�胉�C�^��IDAT�`�����N�������o���܂��B
        /// </summary>
        /// <param name="bw">���C�^</param>
        /// <param name="chunk_data">�`�����N</param>
        public static void WriteIDAT(BinaryWriter bw, byte[] chunk_data)
        {
            WriteChunk(bw, "IDAT", chunk_data);
        }
        /// <summary>
        /// �w�胉�C�^��IEND�`�����N�������o���܂��B
        /// </summary>
        /// <param name="bw">���C�^</param>
        public static void WriteIEND(BinaryWriter bw)
        {
            WriteChunk(bw, "IEND", new byte[] {});
        }
    }
}
