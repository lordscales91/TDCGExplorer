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

        private void bwCompress_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            string[] cols = File.ReadAllLines(GetColsPath());

            Encrypter encrypter = new Encrypter();
            encrypter.SourcePath = @".";

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
                    string src_path = Path.Combine(GetHairKitPath(), string.Format(@"{0}.tbn", src_basename));
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
                    string src_path = Path.Combine(GetHairKitPath(), string.Format(@"icon\ICON_{0}.psd", col));
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

        public static string GetHairKitPath()
        {
            return Path.Combine(Application.StartupPath, @"HAIR_KIT");
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

        static Regex re_kami = new Regex(@"kami", RegexOptions.IgnoreCase);
        static Regex re_housen = new Regex(@"housen", RegexOptions.IgnoreCase);
        static Regex re_ribon = new Regex(@"ribon", RegexOptions.IgnoreCase);

        public static string GetKamiTexPath(string col)
        {
            return Path.Combine(GetHairKitPath(), string.Format(@"image_kami\KIT_BASE_{0}.bmp", col));
        }

        public static string GetKageTexPath(string col)
        {
            return Path.Combine(GetHairKitPath(), string.Format(@"image_kage\KIT_KAGE_{0}.bmp", col));
        }

        public static string GetRibonTexPath(string col)
        {
            return Path.Combine(GetHairKitPath(), string.Format(@"image_ribon\KIT_RIBON_{0}.bmp", col));
        }

        public bool Process(Stream tso_stream, Stream ret_stream, string col)
        {
            TSOFile tso = new TSOFile();
            tso.Load(tso_stream);

            Dictionary<string, TSOTex> texmap = new Dictionary<string, TSOTex>();

            foreach (TSOTex tex in tso.textures)
            {
                texmap[tex.Name] = tex;
            }

            foreach (TSOSubScript sub in tso.sub_scripts)
            {
                Console.WriteLine("  sub name {0} file {1}", sub.Name, sub.FileName);

                Shader shader = sub.shader;
                string color_tex_name = shader.ColorTexName;
                string shade_tex_name = shader.ShadeTexName;

                // Housen ÇÕ Kami ÇÊÇËóDêÊÇ∑ÇÈÅB
                // ex. KamiHousen ÇÃèÍçáÇÕ Housen Ç∆ÇµÇƒèàóù
                if (re_housen.IsMatch(sub.Name))
                {
                    string sub_path = Path.Combine(GetHairKitPath(), string.Format(@"Cgfx_kage\Cgfxkage_{0}", col));
                    sub.Load(sub_path);

                    TSOTex colorTex;
                    if (texmap.TryGetValue(color_tex_name, out colorTex))
                    {
                        colorTex.Load(GetKageTexPath(col));
                    }
                }
                else
                if (re_ribon.IsMatch(sub.Name))
                {
                    TSOTex colorTex;
                    if (texmap.TryGetValue(color_tex_name, out colorTex))
                    {
                        colorTex.Load(GetRibonTexPath(col));
                    }
                }
                else
                if (re_kami.IsMatch(sub.Name))
                {
                    string sub_path = Path.Combine(GetHairKitPath(), string.Format(@"Cgfx_kami\Cgfxkami_{0}", col));
                    sub.Load(sub_path);

                    TSOTex colorTex;
                    if (texmap.TryGetValue(color_tex_name, out colorTex))
                    {
                        colorTex.Load(GetKamiTexPath(col));
                    }
                }
                else
                {
                    TSOTex colorTex;
                    if (texmap.TryGetValue(color_tex_name, out colorTex))
                    {
                        string file = colorTex.FileName.Trim('"');

                        if (re_housen.IsMatch(file))
                        {
                            string sub_path = Path.Combine(GetHairKitPath(), string.Format(@"Cgfx_kage\Cgfxkage_{0}", col));
                            sub.Load(sub_path);

                            colorTex.Load(GetKageTexPath(col));
                        }
                        else
                        if (re_ribon.IsMatch(file))
                        {
                            colorTex.Load(GetRibonTexPath(col));
                        }
                        else
                        if (re_kami.IsMatch(file))
                        {
                            string sub_path = Path.Combine(GetHairKitPath(), string.Format(@"Cgfx_kami\Cgfxkami_{0}", col));
                            sub.Load(sub_path);

                            colorTex.Load(GetKamiTexPath(col));
                        }
                    }
                }

                sub.GenerateShader();
                Shader new_shader = sub.shader;
                new_shader.ColorTexName = color_tex_name;
                new_shader.ShadeTexName = shade_tex_name;
                sub.SaveShader();

                Console.WriteLine("    shader color tex name {0}", color_tex_name);
                Console.WriteLine("    shader shade tex name {0}", shade_tex_name);
            }

            foreach (TSOTex tex in tso.textures)
            {
                Console.WriteLine("  tex name {0} file {1}", tex.Name, tex.FileName);
            }

            tso.Save(ret_stream);
            return true;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            decrypter.Close();
        }
    }
}
