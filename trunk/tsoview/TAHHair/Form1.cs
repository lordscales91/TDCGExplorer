using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using TDCG;
using TDCG.TAHTool;

namespace TAHHair
{
    public partial class Form1 : Form
    {
        string source_file = null;
        Decrypter decrypter = new Decrypter();

        public Form1()
        {
            InitializeComponent();
        }

        private void btnLoad_Click(object sender, EventArgs e)
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

        Regex re_hair_tsofile = new Regex(@"(B|C)00\.tso$");

        private void DumpEntries()
        {
            gvEntries.Rows.Clear();
            foreach (TAHEntry entry in decrypter.Entries)
            {
                if (entry.flag % 2 == 1)
                    continue;

                string file_name = entry.file_name;

                if (re_hair_tsofile.IsMatch(file_name))
                {
                    string[] row = { entry.file_name, entry.offset.ToString(), entry.length.ToString() };
                    gvEntries.Rows.Add(row);
                }
            }
        }

        private void btnCompress_Click(object sender, EventArgs e)
        {
            lbStatus.Text = "Processing...";
            btnCompress.Enabled = false;
            btnLoad.Enabled = false;
            DumpFiles();
        }

        private void DumpFiles()
        {
            bwCompress.RunWorkerAsync(source_file);
        }

        Regex re_tsofile = new Regex(@"\.tso$");

        public static string GetColsPath()
        {
            return Path.Combine(Application.StartupPath, @"cols.txt");
        }

        int tah_version = 10;

        private void bwCompress_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            string[] cols = File.ReadAllLines(GetColsPath());

            Encrypter encrypter = new Encrypter();
            encrypter.SourcePath = @".";
            encrypter.Version = tah_version;

            Dictionary<string, TAHEntry> entries = new Dictionary<string, TAHEntry>();

            foreach (TAHEntry entry in decrypter.Entries)
            {
                if (entry.flag % 2 == 1)
                    continue;

                string path = entry.file_name;

                if (re_hair_tsofile.IsMatch(path))
                {
                    string basename = Path.GetFileNameWithoutExtension(path);
                    string code = basename.Substring(0, 8);
                    string row  = basename.Substring(9, 1);

                    entries[code] = entry;

                    foreach (string col in cols)
                    {
                        string new_basename = code + "_" + row + col;

                        string tbn_path = encrypter.SourcePath + "/script/items/" + new_basename + ".tbn";
                        encrypter.Add(tbn_path);

                        string tso_path = encrypter.SourcePath + "/data/model/" + new_basename + ".tso";
                        encrypter.Add(tso_path);

                        string psd_path = encrypter.SourcePath + "/data/icon/items/" + new_basename + ".psd";
                        encrypter.Add(psd_path);
                    }
                }
            }
            
            int entries_count = encrypter.Count;
            int current_index = 0;
            encrypter.GetFileEntryStream = delegate(string path)
            {
                Console.WriteLine("compressing {0}", path);

                string basename = Path.GetFileNameWithoutExtension(path);
                string code = basename.Substring(0, 8);
                string row  = basename.Substring(9, 1);
                string col  = basename.Substring(10, 2);

                TAHEntry entry = entries[code];
                string ext = Path.GetExtension(path).ToLower();

                Stream ret_stream = null;
                if (ext == ".tbn")
                {
                    string src_basename = null;
                    if (row == "B")
                        src_basename = "N000FHEA_B00";
                    if (row == "C")
                        src_basename = "N000BHEA_C00";
                    string src_path = Path.Combine(TDCG.TSOHair.TSOHairProcessor.GetHairKitPath(), string.Format(@"{0}.tbn", src_basename));
                    using (FileStream source_stream = File.OpenRead(src_path))
                    {
                        ret_stream = new MemoryStream();

                        TBNFile tbn = new TBNFile();
                        tbn.Load(source_stream);

                        Dictionary<uint, string> strings = tbn.GetStringDictionary();
                        foreach (uint i in strings.Keys)
                        {
                            string str = strings[i];
                            if (re_tsofile.IsMatch(str))
                            {
                                Console.WriteLine("{0:X4}: {1}", i, str);
                                tbn.SetString(i, string.Format("data/model/{0}.tso", basename));
                            }
                        }

                        tbn.Save(ret_stream);
                    }
                    ret_stream.Seek(0, SeekOrigin.Begin);
                }
                else
                if (ext == ".tso")
                {
                    byte[] data_output;
                    decrypter.ExtractResource(entry, out data_output);
                    using (MemoryStream tso_stream = new MemoryStream(data_output))
                    {
                        ret_stream = new MemoryStream();
                        Process(tso_stream, ret_stream, col);
                    }
                    ret_stream.Seek(0, SeekOrigin.Begin);
                }
                else
                if (ext == ".psd")
                {
                    string src_path = Path.Combine(TDCG.TSOHair.TSOHairProcessor.GetHairKitPath(), string.Format(@"icon\ICON_{0}.psd", col));
                    using (FileStream source_stream = File.OpenRead(src_path))
                    {
                        ret_stream = new MemoryStream();
                        Copy(source_stream, ret_stream);
                    }
                    ret_stream.Seek(0, SeekOrigin.Begin);
                }
                current_index++;
                int percent = current_index * 100 / entries_count;
                worker.ReportProgress(percent);
                return ret_stream;
            };
            encrypter.Save(@"col-" + Path.GetFileName(source_file));
        }

        private void bwCompress_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            pbStatus.Value = e.ProgressPercentage;
        }

        private void bwCompress_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Console.WriteLine("completed");
            lbStatus.Text = "ok. Compressed";
            pbStatus.Value = 0;
            btnLoad.Enabled = true;
            btnCompress.Enabled = true;
        }

        public void Copy(Stream source_stream, Stream ret_stream)
        {
            const int BUFSIZE = 4096;
            byte[] buf = new byte[BUFSIZE];
            int nbyte;
            while ((nbyte = source_stream.Read(buf, 0, BUFSIZE)) > 0)
            {
                ret_stream.Write(buf, 0, nbyte);
            }
        }

        public bool Process(Stream tso_stream, Stream ret_stream, string col)
        {
            TSOFile tso = new TSOFile();
            tso.Load(tso_stream);

            TDCG.TSOHair.TSOHairProcessor processor = TDCG.TSOHair.TSOHairProcessor.Load(Path.Combine(Application.StartupPath, @"TSOHairProcessor.xml"));
            processor.Process(tso, col);

            tso.Save(ret_stream);
            return true;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            decrypter.Close();
        }

        private void udTahVersion_ValueChanged(object sender, EventArgs e)
        {
            tah_version = (int)udTahVersion.Value;
        }
    }
}
