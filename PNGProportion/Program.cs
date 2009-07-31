using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using TDCG;
using CSScriptLibrary;

namespace PNGProportion
{
    public class TSOFigure
    {
        internal List<byte[]> TSODataList = new List<byte[]>();
        internal List<byte[]> TSOOpt1List = new List<byte[]>();

        internal TMOFile tmo = null;

        internal byte[] lgta;
        internal byte[] figu;
    }

    class Program
    {
        static void Main(string[] args) 
        {
            if (args.Length < 1)
            {
                System.Console.WriteLine("Usage: PNGProportion <source png>");
                return;
            }

            string source_file = args[0];

            Program program = new Program();
            program.SetProportionList();
            program.Process(source_file);
        }

        internal List<TSOFigure> TSOFigureList = new List<TSOFigure>();

        byte[] cami;

        protected BinaryWriter writer;

        List<IProportion> pro_list = new List<IProportion>();
        TPOFileList tpo_list = new TPOFileList();
    
        public string GetProportionPath()
        {
            return Path.Combine(Application.StartupPath, @"Proportion");
        }

        public string GetTPOConfigPath()
        {
            return Path.Combine(Application.StartupPath, @"TPOConfig.xml");
        }

        public void SetProportionList()
        {
            string[] script_files = Directory.GetFiles(GetProportionPath(), "*.cs");
            foreach (string script_file in script_files)
            {
                string class_name = "TDCG.Proportion." + Path.GetFileNameWithoutExtension(script_file);
                var script = CSScript.Load(script_file).CreateInstance(class_name).AlignToInterface<IProportion>();
                pro_list.Add(script);
            }
            tpo_list.SetProportionList(pro_list);
        }

        public bool Process(string source_file)
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

            TPOConfig config = TPOConfig.Load(GetTPOConfigPath());
            Dictionary<string, Proportion> portion_map = new Dictionary<string, Proportion>();
            foreach (Proportion portion in config.Proportions)
                portion_map[portion.ClassName] = portion;

            foreach (TSOFigure fig in TSOFigureList)
                if (fig.tmo != null)
                if (fig.tmo.nodes[0].Name == "|W_Hips")
                {
                    tpo_list.Tmo = fig.tmo;

                    for (int i = 0; i < tpo_list.Count; i++)
                    {
                        TPOFile tpo = tpo_list[i];
                        {
                            Debug.Assert(tpo.Proportion != null, "tpo.Proportion should not be null");
                            Proportion portion;
                            if (portion_map.TryGetValue(tpo.Proportion.ToString(), out portion))
                                tpo.Ratio = portion.Ratio;
                        }
                    }

                    tpo_list.Transform();
                }

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
