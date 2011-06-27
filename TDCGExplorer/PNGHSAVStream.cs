using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Checksums;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;

namespace TDCGExplorer
{
    public class PNGTsoData : IDisposable
    {
        public uint tsoID;
        public byte[] tsodata;
        public void Dispose()
        {
            tsodata = null;
        }
    }

    public class PNGHSAVStream
    {
        private List<PNGTsoData> tsoData = new List<PNGTsoData>();
        protected Crc32 crc = new Crc32();

        // tsoデータ数を返す
        public int count
        {
            get { return tsoData.Count; }
        }

        // TSOデータを返す
        public List<PNGTsoData> get
        {
            get { return tsoData; }
        }

        public PNGHSAVStream()
        {
        }

        /// <summary>
        /// 指定パスからPNGFileを読み込みフィギュアを作成します。
        /// </summary>
        /// <param name="source_file">PNGFileのパス</param>
        public void LoadPNGFile(Stream stream)
        {
            try
            {
                PNGFile png = new PNGFile();

                png.Hsav += delegate(string type)
                {
                };
                png.Lgta += delegate(Stream dest, int extract_length)
                {
                };
                png.Ftmo += delegate(Stream dest, int extract_length)
                {
                };
                png.Figu += delegate(Stream dest, int extract_length)
                {
                };
                png.Ftso += delegate(Stream dest, int extract_length, byte[] opt1)
                {
                    PNGTsoData tsodata = new PNGTsoData();

                    // optionをuint32に変換する
                    uint u32;
                    unsafe
                    {
                        Marshal.Copy(opt1, 0, (IntPtr)(void*)&u32, sizeof(uint));
                    }
                    Byte[] data;

                    BinaryReader reader = new BinaryReader(dest, System.Text.Encoding.Default);
                    data = reader.ReadBytes(extract_length);

                    tsodata.tsoID = u32;
                    tsodata.tsodata = data;
                    tsoData.Add(tsodata);
                };
                png.Load(stream);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex);
            }
        }

        // PNGをロードする.
        public PNGFile GetPNG(Stream stream)
        {
            PNGFile png = null;
            try
            {
                png = new PNGFile();
                png.Load(stream);
            }
            catch (Exception)
            {
                png = null;
            }
            return png;
        }

        // ヘビーセーブファイルを保存する.
        public void SavePNGFile(PNGFile png,Stream stream)
        {
            png.WriteTaOb += delegate(BinaryWriter bw)
            {
                // TaOBを書き出す.
                WriteTDCG(bw);
                WriteHSAV(bw);
                foreach(PNGTsoData tsodata in tsoData)
                    WriteFTSO(bw,tsodata);

            };
            png.Save(stream);
        }

        protected void WriteTDCG(BinaryWriter bw)
        {
            byte[] data = System.Text.Encoding.ASCII.GetBytes("$XP$");
            WriteTaOb(bw,"TDCG", data);
        }

        protected void WriteHSAV(BinaryWriter bw)
        {
            byte[] data = System.Text.Encoding.ASCII.GetBytes("$XP$");
            WriteTaOb(bw,"HSAV", data);
        }

        protected void WriteFTSO(BinaryWriter bw,PNGTsoData tso)
        {
            WriteTaOb(bw, "FTSO", 0x26F5B8FE, tso.tsoID, tso.tsodata);
        }

        protected void WriteTaOb(BinaryWriter bw,string type, uint opt0, uint opt1, byte[] data)
        {
            Console.WriteLine("WriteTaOb {0}", type);
            Console.WriteLine("taOb extract length {0}", data.Length);
            byte[] chunk_type = System.Text.Encoding.ASCII.GetBytes(type);

            MemoryStream dest = new MemoryStream();
            using (DeflaterOutputStream gzip = new DeflaterOutputStream(dest))
            {
                gzip.IsStreamOwner = false;
                gzip.Write(data, 0, data.Length);
            }
            dest.Seek(0, SeekOrigin.Begin);
            Console.WriteLine("taOb length {0}", dest.Length);
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
            WriteChunk(bw,"taOb", chunk_data);
        }

        protected void WriteChunk(BinaryWriter bw,string type, byte[] chunk_data)
        {
            byte[] buf = BitConverter.GetBytes((UInt32)chunk_data.Length);
            Array.Reverse(buf);
            bw.Write(buf);

            byte[] chunk_type = System.Text.Encoding.ASCII.GetBytes(type);
            bw.Write(chunk_type);
            bw.Write(chunk_data);

            crc.Reset();
            crc.Update(chunk_type);
            crc.Update(chunk_data);

            byte[] crc_buf = BitConverter.GetBytes((UInt32)crc.Value);
            Array.Reverse(crc_buf);
            bw.Write(crc_buf);
        }

        protected void WriteTaOb(BinaryWriter bw, string type, byte[] data)
        {
            WriteTaOb(bw,type, 0, 0, data);
        }

    }
}
