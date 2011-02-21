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
using TDCG.TAHTool;

namespace TAHProportion
{
    public partial class Form1 : Form
    {
        string source_file = null;
        Decrypter decrypter = new Decrypter();

        TPOFileList tpo_list = new TPOFileList();
    
        public string GetTPOConfigPath()
        {
            return Path.Combine(Application.StartupPath, @"TPOConfig.xml");
        }

        public Form1()
        {
            InitializeComponent();

            LoadTPOFileList();
            DumpPortions();
        }

        private void LoadTPOFileList()
        {
            tpo_list.Load();
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            if (diaOpen1.ShowDialog() == DialogResult.OK)
            {
                source_file = diaOpen1.FileName;
                decrypter.Open(source_file);
                btnCompress.Enabled = false;
                btnLoad.Enabled = false;
                lbStatus.Text = "Processing...";
                DumpEntries();
                lbStatus.Text = "ok. Loaded";
                btnLoad.Enabled = true;
                btnCompress.Enabled = true;
            }
        }

        private void btnCompress_Click(object sender, EventArgs e)
        {
            lbStatus.Text = "Processing...";
            btnCompress.Enabled = false;
            btnLoad.Enabled = false;
            DumpFiles();
        }

        private void bwCompress_DoWork(object sender, DoWorkEventArgs e)
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
                    file_name += TAHFileUtils.GetExtensionFromMagic(decrypter.ExtractResource(entry));
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

                Stream ret_stream = null;
                if (ext == ".tmo")
                {
                    TMOFile tmo = new TMOFile();
                    MemoryStream tmo_stream = new MemoryStream(decrypter.ExtractResource(entry));
                    tmo.Load(tmo_stream);

                    if (tmo.nodes[0].Path == "|W_Hips")
                    {
                        tpo_list.Tmo = tmo;

                        foreach (TPOFile tpo in tpo_list.files)
                        {
                            Debug.Assert(tpo.Proportion != null, "tpo.Proportion should not be null");
                            Proportion portion;
                            if (portion_map.TryGetValue(tpo.ProportionName, out portion))
                                tpo.Ratio = portion.Ratio;
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
                    MemoryStream png_stream = new MemoryStream(decrypter.ExtractResource(entry));
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
                    file_name += TAHFileUtils.GetExtensionFromMagic(decrypter.ExtractResource(entry));
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

        internal List<TSOFigure> TSOFigureList = new List<TSOFigure>();

        byte[] cami;

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

                    TSOData tso = new TSOData();
                    tso.opt1 = BitConverter.ToUInt32(opt1, 0);
                    tso.ftso = ftso;
                    fig.TSOList.Add(tso);
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
                    if (fig.tmo.nodes[0].Path == "|W_Hips")
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
                PNGWriter pw = new PNGWriter(bw);
                switch (source_type)
                {
                    case "HSAV":
                        WriteHsav(pw);
                        break;
                    case "POSE":
                        WritePose(pw);
                        break;
                    case "SCNE":
                        WriteScne(pw);
                        break;
                    case "PTMO":
                        WritePtmo(pw);
                        break;
                }
            };
            png.Save(ret_stream);

            TSOFigureList.Clear();
            return true;
        }

        protected void WriteHsav(PNGWriter pw)
        {
            pw.WriteTDCG();
            pw.WriteHSAV();
            foreach (TSOFigure fig in TSOFigureList)
                foreach (TSOData tso in fig.TSOList)
                    pw.WriteFTSO(tso.opt1, tso.ftso);
        }

        protected void WritePose(PNGWriter pw)
        {
            pw.WriteTDCG();
            pw.WritePOSE();
            pw.WriteCAMI(cami);
            foreach (TSOFigure fig in TSOFigureList)
            {
                pw.WriteLGTA(fig.lgta);
                pw.WriteFTMO(fig.tmo);
            }
        }

        protected void WriteScne(PNGWriter pw)
        {
            pw.WriteTDCG();
            pw.WriteSCNE(FigureCount());
            pw.WriteCAMI(cami);
            foreach (TSOFigure fig in TSOFigureList)
            {
                pw.WriteLGTA(fig.lgta);
                pw.WriteFTMO(fig.tmo);
                pw.WriteFIGU(fig.figu);
                foreach (TSOData tso in fig.TSOList)
                    pw.WriteFTSO(tso.opt1, tso.ftso);
            }
        }

        protected void WritePtmo(PNGWriter pw)
        {
            pw.WriteTDCG();
            pw.WritePOSE();
            foreach (TSOFigure fig in TSOFigureList)
            {
                pw.WriteFTMO(fig.tmo);
            }
        }

        protected int FigureCount()
        {
            return TSOFigureList.Count;
        }
    }

    public class TSOData
    {
        internal uint opt1;
        internal byte[] ftso;
    }

    public class TSOFigure
    {
        internal List<TSOData> TSOList = new List<TSOData>();
        internal TMOFile tmo = null;

        internal byte[] lgta;
        internal byte[] figu;
    }
}
