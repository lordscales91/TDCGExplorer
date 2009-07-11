using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace TDCGExplorer
{
    public class PNGTsoData
    {
        public uint tsoID;
        public byte[] tsodata;
    }

    public class PNGStream
    {
        private List<PNGTsoData> tsoData = new List<PNGTsoData>();

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

        public PNGStream()
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
    }
}
