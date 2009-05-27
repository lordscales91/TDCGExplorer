using System;
using System.Collections.Generic;
using System.IO;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;

namespace TAHdecrypt
{
    public class TSOFigure
    {
        internal List<byte[]> TSODataList = new List<byte[]>();
        internal List<byte[]> TSOOpt1List = new List<byte[]>();

        internal TMOFile tmo = null;

        internal byte[] lgta;
        internal byte[] figu;
    }

    class PNGNodeCopy
    {
        static void Main(string[] args) 
        {
            if (args.Length < 2)
            {
                System.Console.WriteLine("Usage: PNGNodeCopy <source png> <motion png> [node name]");
                return;
            }

            string source_file = args[0];
            string motion_file = args[1];
            string node_name = "W_Hips";

            if (args.Length > 2)
                node_name = args[2];

            PNGNodeCopy png = new PNGNodeCopy();
            png.CopyNode(source_file, motion_file, node_name);
        }

        internal List<TSOFigure> TSOFigureList = new List<TSOFigure>();

        byte[] cami;

        protected BinaryWriter writer;

        public bool CopyNode(string source_file, string motion_file, string node_name)
        {
            List<TSOFigure> fig_list = new List<TSOFigure>();

            Console.WriteLine("Load File: " + source_file);
            PNGFile source = new PNGFile();
            string source_type = "";

            try
            {
                TSOFigure fig = null;
                TMOFile tmo = null;

                source.Hsav += delegate(string type)
                {
                    source_type = type;

                    fig = new TSOFigure();
                    fig_list.Add(fig);
                };
                source.Pose += delegate(string type)
                {
                    source_type = type;
                };
                source.Scne += delegate(string type)
                {
                    source_type = type;
                };
                source.Cami += delegate(Stream dest, int extract_length)
                {
                    cami = new byte[extract_length];
                    dest.Read(cami, 0, extract_length);
                };
                source.Lgta += delegate(Stream dest, int extract_length)
                {
                    byte[] lgta = new byte[extract_length];
                    dest.Read(lgta, 0, extract_length);

                    fig = new TSOFigure();
                    fig.lgta = lgta;
                    fig_list.Add(fig);
                };
                source.Ftmo += delegate(Stream dest, int extract_length)
                {
                    tmo = new TMOFile();
                    tmo.Load(dest);
                    fig.tmo = tmo;
                };
                source.Figu += delegate(Stream dest, int extract_length)
                {
                    byte[] figu = new byte[extract_length];
                    dest.Read(figu, 0, extract_length);

                    fig.figu = figu;
                };
                source.Ftso += delegate(Stream dest, int extract_length, byte[] opt1)
                {
                    byte[] ftso = new byte[extract_length];
                    dest.Read(ftso, 0, extract_length);

                    fig.TSODataList.Add(ftso);
                    fig.TSOOpt1List.Add(opt1);
                };

                source.Load(source_file);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }

            foreach (TSOFigure fig in fig_list)
            {
                TSOFigureList.Add(fig);
            }

            Console.WriteLine("Load File: " + motion_file);
            PNGFile motion = new PNGFile();
            TMOFile motion_tmo = null;
            try
            {
                motion.Ftmo += delegate(Stream dest, int extract_length)
                {
                    motion_tmo = new TMOFile();
                    motion_tmo.Load(dest);
                };

                motion.Load(motion_file);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }

            if (motion_tmo != null)
                foreach (TSOFigure fig in TSOFigureList)
                    if (fig.tmo != null)
                        fig.tmo.CopyNodeFrom(motion_tmo, node_name);

            string dest_file = source_file + ".tmp";
            Console.WriteLine("Save File: " + dest_file);
            source.WriteTaOb += delegate(BinaryWriter bw)
            {
                this.writer = bw;
                switch (source_type)
                {
                case "HSAV":
                    WriteHsav();
                    break;
                case "POSE":
                    WritePose();
                    break;
                case "SCNE":
                    WriteScne();
                    break;
                }
            };
            source.Save(dest_file);

            File.Delete(source_file);
            File.Move(dest_file, source_file);
            Console.WriteLine("updated " + source_file);

            return true;
        }

        protected void WriteHsav()
        {
            WriteTDCG();
            WriteHSAV();
            foreach (TSOFigure fig in TSOFigureList)
                for (int i = 0; i < fig.TSODataList.Count; i++)
                    WriteFTSO(fig.TSODataList[i], fig.TSOOpt1List[i]);
        }

        protected void WritePose()
        {
            WriteTDCG();
            WritePOSE();
            WriteCAMI(cami);
            foreach (TSOFigure fig in TSOFigureList)
            {
                WriteLGTA(fig.lgta);
                WriteFTMO(fig.tmo);
            }
        }

        protected void WriteScne()
        {
            WriteTDCG();
            WriteSCNE();
            WriteCAMI(cami);
            foreach (TSOFigure fig in TSOFigureList)
            {
                WriteLGTA(fig.lgta);
                WriteFTMO(fig.tmo);
                WriteFIGU(fig.figu);
                for (int i = 0; i < fig.TSODataList.Count; i++)
                    WriteFTSO(fig.TSODataList[i], fig.TSOOpt1List[i]);
            }
        }

        protected int FigureCount()
        {
            return TSOFigureList.Count;
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

        protected void WritePOSE()
        {
            byte[] data = System.Text.Encoding.ASCII.GetBytes("$XP$");
            WriteTaOb("POSE", data);
        }

        protected void WriteSCNE()
        {
            byte[] data = System.Text.Encoding.ASCII.GetBytes("$XP$");
            WriteTaOb("SCNE", 0, (uint)FigureCount(), data);
        }

        protected void WriteCAMI(byte[] data)
        {
            WriteTaOb("CAMI", data);
        }

        protected void WriteLGTA(byte[] data)
        {
            WriteTaOb("LGTA", data);
        }

        protected void WriteFIGU(byte[] data)
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

        protected void WriteFTMO(TMOFile tmo)
        {
            MemoryStream dest = new MemoryStream();
            tmo.Save(dest);
            dest.Seek(0, SeekOrigin.Begin);
            WriteFile("FTMO", 0xADCFB72F, 0, dest);
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
