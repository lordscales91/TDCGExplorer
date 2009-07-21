using System;
using System.Collections.Generic;
using System.IO;
//using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Checksums;
//using ICSharpCode.SharpZipLib.Zip.Compression.Streams;

namespace TDCG
{
    /// <summary>
    /// PNGFileを書き出すメソッド群
    /// </summary>
    public class PNGWriter
    {
        /// <summary>
        /// CSCチェックを行うオブジェクト
        /// </summary>
        protected static Crc32 crc = new Crc32();

        /// <summary>
        /// 指定ライタにbyte配列を書き出します。
        /// </summary>
        /// <param name="bw">ライタ</param>
        /// <param name="bytes">bute配列</param>
        public static void Write(BinaryWriter bw, byte[] bytes)
        {
            if (bw != null)
                bw.Write(bytes);
        }
        /// <summary>
        /// 指定ライタにチャンクを書き出します。
        /// </summary>
        /// <param name="bw">ライタ</param>
        /// <param name="type">チャンクタイプ</param>
        /// <param name="chunk_data">チャンク</param>
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
        /// 指定ライタにIHDRチャンクを書き出します。
        /// </summary>
        /// <param name="bw">ライタ</param>
        /// <param name="chunk_data">チャンク</param>
        public static void WriteIHDR(BinaryWriter bw, byte[] chunk_data)
        {
            WriteChunk(bw, "IHDR", chunk_data);
        }
        /// <summary>
        /// 指定ライタにIDATチャンクを書き出します。
        /// </summary>
        /// <param name="bw">ライタ</param>
        /// <param name="chunk_data">チャンク</param>
        public static void WriteIDAT(BinaryWriter bw, byte[] chunk_data)
        {
            WriteChunk(bw, "IDAT", chunk_data);
        }
        /// <summary>
        /// 指定ライタにIENDチャンクを書き出します。
        /// </summary>
        /// <param name="bw">ライタ</param>
        public static void WriteIEND(BinaryWriter bw)
        {
            WriteChunk(bw, "IEND", new byte[] {});
        }
    }
}
