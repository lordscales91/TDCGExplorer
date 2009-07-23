using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using TDCG;
using CSScriptLibrary;

namespace TAHTool
{
    public partial class Form1 : Form
    {
        string source_file = null;
        Decrypter decrypter = new Decrypter();

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

        public Form1()
        {
            InitializeComponent();

            string[] script_files = Directory.GetFiles(GetProportionPath(), "*.cs");
            foreach (string script_file in script_files)
            {
                string class_name = "TDCG.Proportion." + Path.GetFileNameWithoutExtension(script_file);
                var script = CSScript.Load(script_file).CreateInstance(class_name).AlignToInterface<IProportion>();
                pro_list.Add(script);
            }
            tpo_list.SetProportionList(pro_list);
            DumpPortions();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (diaOpen1.ShowDialog() == DialogResult.OK)
            {
                source_file = diaOpen1.FileName;
                decrypter.Load(source_file);
                btnCompress.Enabled = false;
                btnLoad.Enabled = false;
                lbStatus.Text = "Processing...";
                DumpEntries();
                lbStatus.Text = "ok. Loaded";
                btnLoad.Enabled = true;
                btnCompress.Enabled = true;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            lbStatus.Text = "Processing...";
            btnCompress.Enabled = false;
            btnLoad.Enabled = false;
            DumpFiles();
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            TPOConfig config = TPOConfig.Load(GetTPOConfigPath());
            Dictionary<string, Proportion> portion_map = new Dictionary<string, Proportion>();
            foreach (Proportion portion in config.Proportions)
                portion_map[portion.ClassName] = portion;

            Encrypter encrypter = new Encrypter();
            encrypter.SourcePath = @".";

            Dictionary<string, TAHEntry> entries = new Dictionary<string, TAHEntry>();

            foreach (TAHEntry entry in decrypter.Entries)
            {
                string file_name = entry.file_name;

                if (entry.flag % 2 == 1)
                {
                    byte[] data_output;
                    decrypter.ExtractResource(entry, out data_output);
                    file_name += GetExtensionFromMagic(data_output);
                }

                string ext = Path.GetExtension(file_name).ToLower();
                if (ext == ".tmo")
                {
                    string true_file_name = encrypter.SourcePath + "/" + file_name;
                    entries[true_file_name] = entry;
                    encrypter.Add(true_file_name);
                }
                else
                if (ext == ".png")
                {
                    string true_file_name = encrypter.SourcePath + "/" + file_name;
                    entries[true_file_name] = entry;
                    encrypter.Add(true_file_name);
                }
            }
            
            int entries_count = encrypter.Count;
            int current_index = 0;
            encrypter.GetFileEntryStream = delegate(string true_file_name)
            {
                Console.WriteLine("compressing {0}", true_file_name);
                TAHEntry entry = entries[true_file_name];
                string ext = Path.GetExtension(true_file_name).ToLower();
                byte[] data_output;
                decrypter.ExtractResource(entry, out data_output);

                Stream ret_stream = null;
                if (ext == ".tmo")
                {
                    TMOFile tmo = new TMOFile();
                    MemoryStream tmo_stream = new MemoryStream(data_output);
                    tmo.Load(tmo_stream);

                    if (tmo.nodes[0].Name == "|W_Hips")
                    {
                        tpo_list.Tmo = tmo;

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

                        tmo_stream.Seek(0, SeekOrigin.Begin);
                        tmo.Save(tmo_stream);
                    }
                    tmo_stream.Seek(0, SeekOrigin.Begin);
                    ret_stream = tmo_stream;
                }
                else
                if (ext == ".png")
                {
                    MemoryStream png_stream = new MemoryStream(data_output);
                    ret_stream = new MemoryStream();
                    Process(png_stream, ret_stream);
                    ret_stream.Seek(0, SeekOrigin.Begin);
                }
                current_index++;
                int percent = current_index * 100 / entries_count;
                worker.ReportProgress(percent);
                return ret_stream;
            };

            encrypter.Save(@"tmo-" + Path.GetFileName(source_file));
        }

        private void DumpPortions()
        {
            TPOConfig config = TPOConfig.Load(GetTPOConfigPath());
            gvPortions.Rows.Clear();
            foreach (Proportion portion in config.Proportions)
            {
                string[] row = { portion.ClassName, string.Format("{0:F2}", portion.Ratio) };
                gvPortions.Rows.Add(row);
            }
        }

        private void DumpEntries()
        {
            gvEntries.Rows.Clear();
            foreach (TAHEntry entry in decrypter.Entries)
            {
                string file_name = entry.file_name;

                if (entry.flag % 2 == 1)
                {
                    byte[] data_output;
                    decrypter.ExtractResource(entry, out data_output);
                    file_name += GetExtensionFromMagic(data_output);
                }

                string ext = Path.GetExtension(file_name).ToLower();
                if (ext == ".tmo")
                {
                    string[] row = { entry.file_name, entry.offset.ToString(), entry.length.ToString() };
                    gvEntries.Rows.Add(row);
                }
                else
                if (ext == ".png")
                {
                    string[] row = { entry.file_name, entry.offset.ToString(), entry.length.ToString() };
                    gvEntries.Rows.Add(row);
                }
            }
        }

        private void DumpFiles()
        {
            bwCompress.RunWorkerAsync(source_file);
        }

        static string GetExtensionFromMagic(byte[] magic)
        {
            string ext;
            if (magic[0] == '8' && magic[1] == 'B' && magic[2] == 'P' && magic[3] == 'S')
                ext = ".psd";
            else
            if (magic[0] == 'T' && magic[1] == 'M' && magic[2] == 'O' && magic[3] == '1')
                ext = ".tmo";
            else
            if (magic[0] == 'T' && magic[1] == 'S' && magic[2] == 'O' && magic[3] == '1')
                ext = ".tso";
            else
            if (magic[0] == 'O' && magic[1] == 'g' && magic[2] == 'g' && magic[3] == 'S')
                ext = ".ogg";
            else
            if (magic[0] == 'B' && magic[1] == 'B' && magic[2] == 'B' && magic[3] == 'B')
                ext = ".tbn";
            else
            if (magic[0] == 0x89 && magic[1] == 'P' && magic[2] == 'N' && magic[3] == 'G')
                ext = ".png";
            else
                ext = ".cgfx";
            return ext;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            decrypter.Close();
        }

        private void bwCompress_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Console.WriteLine("completed");
            lbStatus.Text = "ok. Compressed";
            pbStatus.Value = 0;
            btnLoad.Enabled = true;
            btnCompress.Enabled = true;
        }

        private void bwCompress_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            pbStatus.Value = e.ProgressPercentage;
        }

        public class TSOFigure
        {
            internal List<byte[]> TSODataList = new List<byte[]>();
            internal List<byte[]> TSOOpt1List = new List<byte[]>();

            internal TMOFile tmo = null;

            internal byte[] lgta;
            internal byte[] figu;
        }

        internal List<TSOFigure> TSOFigureList = new List<TSOFigure>();

        byte[] cami;

        protected BinaryWriter writer;

        public bool Process(Stream png_stream, Stream ret_stream)
        {
            List<TSOFigure> fig_list = new List<TSOFigure>();

            PNGFile png = new PNGFile();
            string source_type = "";

            {
                TSOFigure fig = null;
                TMOFile tmo = null;

                png.Hsav += delegate(string type)
                {
                    source_type = type;

                    fig = new TSOFigure();
                    fig_list.Add(fig);
                };
                png.Pose += delegate(string type)
                {
                    source_type = type;
                };
                png.Scne += delegate(string type)
                {
                    source_type = type;
                };
                png.Cami += delegate(Stream dest, int extract_length)
                {
                    cami = new byte[extract_length];
                    dest.Read(cami, 0, extract_length);
                };
                png.Lgta += delegate(Stream dest, int extract_length)
                {
                    byte[] lgta = new byte[extract_length];
                    dest.Read(lgta, 0, extract_length);

                    fig = new TSOFigure();
                    fig.lgta = lgta;
                    fig_list.Add(fig);
                };
                png.Ftmo += delegate(Stream dest, int extract_length)
                {
                    tmo = new TMOFile();
                    tmo.Load(dest);
                    if (fig == null)
                    {
                        source_type = "PTMO";
                        fig = new TSOFigure();
                        fig.lgta = null;
                        fig_list.Add(fig);
                    }
                    fig.tmo = tmo;
                };
                png.Figu += delegate(Stream dest, int extract_length)
                {
                    byte[] figu = new byte[extract_length];
                    dest.Read(figu, 0, extract_length);

                    fig.figu = figu;
                };
                png.Ftso += delegate(Stream dest, int extract_length, byte[] opt1)
                {
                    byte[] ftso = new byte[extract_length];
                    dest.Read(ftso, 0, extract_length);

                    fig.TSODataList.Add(ftso);
                    fig.TSOOpt1List.Add(opt1);
                };

                png.Load(png_stream);
                png_stream.Close();
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

            png.WriteTaOb += delegate(BinaryWriter bw)
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
                    case "PTMO":
                        WritePtmo();
                        break;
                }
            };
            png.Save(ret_stream);

            TSOFigureList.Clear();
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

        protected void WritePtmo()
        {
            WriteTDCG();
            WritePOSE();
            foreach (TSOFigure fig in TSOFigureList)
            {
                WriteFTMO(fig.tmo);
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
