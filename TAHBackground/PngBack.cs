using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using TAHTool;
using TDCG;

namespace TAHBackground
{
    public class TSOFigure
    {
        internal List<byte[]> TSODataList = new List<byte[]>();
        internal List<byte[]> TSOOpt1List = new List<byte[]>();

        internal TMOFile tmo = null;

        internal byte[] lgta;
        internal byte[] figu;
    }

    public class PngBack
    {
        TBNFile tbn = null;
        public TBNFile Tbn { get { return tbn; } }

        PSDFile psd = null;
        public PSDFile Psd { get { return psd; } }

        PNGFile png = null;
        public PNGFile Png { get { return png; } }

        TSOFigure fig = null;
        public TSOFigure Figure { get { return fig; } }

        string source_file = null;
        protected BinaryWriter writer = null;

        public void Load(Stream tbn_stream, Stream psd_stream)
        {
            tbn = new TBNFile();
            tbn.Load(tbn_stream);

            psd = new PSDFile();
            psd.Load(psd_stream);

            png = CreatePNGFileFromPSDFile(psd);

            fig = new TSOFigure();
        }

        public static string GetTBNPathFromPSDPath(string psd_path)
        {
            return @"script/backgrounds/" + Path.GetFileNameWithoutExtension(psd_path) + @".tbn";
        }

        public static List<string> GetTSOPathListFromTBNFile(TBNFile tbn)
        {
            List<string> ret = new List<string>();
            List<string> strings = tbn.GetStringList();
            Regex re_tsofile = new Regex(@"\.tso$");
            foreach (string str in strings)
            {
                if (re_tsofile.IsMatch(str))
                {
                    string tso_path = str.ToLower();
                    ret.Add(tso_path);
                }
            }
            return ret;
        }

        static PNGFile CreatePNGFileFromPSDFile(PSDFile psd)
        {
            MemoryStream ms = new MemoryStream();
            psd.Bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            ms.Seek(0, SeekOrigin.Begin);

            PNGFile png = new PNGFile();
            png.Load(ms);

            return png;
        }

        byte[] opt1 = new byte[] { 0x00, 0x00, 0x00, 0x00 };

        public void AddTSOFile(byte[] ftso)
        {
            fig.TSODataList.Add(ftso);
            fig.TSOOpt1List.Add(opt1);
        }

        public void Save(string dest_file)
        {
            png.WriteTaOb += delegate(BinaryWriter bw)
            {
                this.writer = bw;
                WriteHsav();
            };
            png.Save(dest_file);
        }
        
        protected void WriteHsav()
        {
            WriteTDCG();
            WriteHSAV();
            for (int i = 0; i < fig.TSODataList.Count; i++)
                WriteFTSO(fig.TSODataList[i], fig.TSOOpt1List[i]);
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

        protected void WriteTDCG()
        {
            byte[] data = System.Text.Encoding.ASCII.GetBytes("$XP$");
            WriteTaOb("TDCG", data);
        }

        protected void WriteHSAV()
        {
            byte[] data = System.Text.Encoding.ASCII.GetBytes("$XP$");
            WriteTaOb("HSAV", data);
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

        protected uint opt_value(byte[] bytes)
        {
            return BitConverter.ToUInt32(bytes, 0);
        }

        protected void WriteFTSO(byte[] data, byte[] opt1)
        {
            MemoryStream dest = new MemoryStream(data);
            WriteFile("FTSO", 0x26F5B8FE, opt_value(opt1), dest);
        }
    }
}
