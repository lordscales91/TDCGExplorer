using System;
using System.Collections.Generic;
using System.IO;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Checksums;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;

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

        protected BinaryWriter writer;

        public PNGWriter(BinaryWriter bw)
        {
            this.writer = bw;
        }

        protected void WriteTaOb(string type, uint opt0, uint opt1, byte[] data)

        {
            //Console.WriteLine("WriteTaOb {0}", type);
            //Console.WriteLine("taOb extract length {0}", data.Length);
            byte[] chunk_type = System.Text.Encoding.ASCII.GetBytes(type);

            MemoryStream dest = new MemoryStream();
            using (DeflaterOutputStream gzip = new DeflaterOutputStream(dest))
            {
                gzip.IsStreamOwner = false;
                gzip.Write(data, 0, data.Length);
            }
            dest.Seek(0, SeekOrigin.Begin);
            //Console.WriteLine("taOb length {0}", dest.Length);
            byte[] chunk_data = new byte[dest.Length + 20];

            Array.Copy(chunk_type, 0, chunk_data, 0, 4);
            byte[] buf;
            buf = BitConverter.GetBytes((UInt32)opt0);
            Array.Copy(buf, 0, chunk_data, 4, 4);
            buf = BitConverter.GetBytes((UInt32)opt1);
            Array.Copy(buf, 0, chunk_data, 8, 4);
            buf = BitConverter.GetBytes((UInt32)data.Length);
            Array.Copy(buf, 0, chunk_data, 12, 4);
            buf = BitConverter.GetBytes((UInt32)dest.Length);
            Array.Copy(buf, 0, chunk_data, 16, 4);

            dest.Read(chunk_data, 20, (int)dest.Length);
            PNGWriter.WriteChunk(writer, "taOb", chunk_data);
        }

        protected void WriteTaOb(string type, byte[] data)
        {
            WriteTaOb(type, 0, 0, data);
        }

        public void WriteTDCG()
        {
            byte[] data = System.Text.Encoding.ASCII.GetBytes("$XP$");
            WriteTaOb("TDCG", data);
        }

        public void WriteHSAV()
        {
            byte[] data = System.Text.Encoding.ASCII.GetBytes("$XP$");
            WriteTaOb("HSAV", data);
        }

        public void WritePOSE()
        {
            byte[] data = System.Text.Encoding.ASCII.GetBytes("$XP$");
            WriteTaOb("POSE", data);
        }

        public void WriteSCNE(int figure_count)
        {
            byte[] data = System.Text.Encoding.ASCII.GetBytes("$XP$");
            WriteTaOb("SCNE", 0, (uint)figure_count, data);
        }

        public void WriteCAMI(byte[] data)
        {
            WriteTaOb("CAMI", data);
        }

        public void WriteLGTA(byte[] data)
        {
            WriteTaOb("LGTA", data);
        }

        public void WriteFIGU(byte[] data)
        {
            WriteTaOb("FIGU", data);
        }

        protected void WriteFile(string type, uint opt0, uint opt1, Stream source)
        {
            //Console.WriteLine("taOb extract length {0}", source.Length);

            MemoryStream dest = new MemoryStream();
            using (DeflaterOutputStream gzip = new DeflaterOutputStream(dest))
            {
                gzip.IsStreamOwner = false;

                byte[] b = new byte[4096];
                StreamUtils.Copy(source, gzip, b);
            }
            dest.Seek(0, SeekOrigin.Begin);
            //Console.WriteLine("taOb length {0}", dest.Length);

            byte[] chunk_type = System.Text.Encoding.ASCII.GetBytes(type);
            byte[] chunk_data = new byte[dest.Length + 20];

            Array.Copy(chunk_type, 0, chunk_data, 0, 4);

            byte[] buf;
            buf = BitConverter.GetBytes((UInt32)opt0);
            Array.Copy(buf, 0, chunk_data, 4, 4);
            buf = BitConverter.GetBytes((UInt32)opt1);
            Array.Copy(buf, 0, chunk_data, 8, 4);

            buf = BitConverter.GetBytes((UInt32)source.Length);
            Array.Copy(buf, 0, chunk_data, 12, 4);
            buf = BitConverter.GetBytes((UInt32)dest.Length);
            Array.Copy(buf, 0, chunk_data, 16, 4);

            dest.Read(chunk_data, 20, (int)dest.Length);
            PNGWriter.WriteChunk(writer, "taOb", chunk_data);
        }

        public void WriteFTMO(TMOFile tmo)
        {
            MemoryStream ms = new MemoryStream();
            tmo.Save(ms);
            ms.Seek(0, SeekOrigin.Begin);
            WriteFTMO(ms);
        }

        public void WriteFTMO(Stream stream)
        {
            WriteFile("FTMO", 0xADCFB72F, 0, stream);
        }

        public void WriteFTSO(uint opt1, byte[] data)
        {
            MemoryStream ms = new MemoryStream(data);
            WriteFTSO(opt1, ms);
        }

        public void WriteFTSO(uint opt1, Stream stream)
        {
            WriteFile("FTSO", 0x26F5B8FE, opt1, stream);
        }
    }
}
