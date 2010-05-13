using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using TDCG;
using TDCG.TAHTool;
using TDCG.TSOHair;

namespace TAHHair
{
    public class TAHHairProcessor
    {
        Decrypter decrypter = new Decrypter();
        TSOHairProcessor tsohair_processor;

        public string KitRoot { get; set; }
        public List<TBNHairPart> parts;
        public string PsdPath { get; set; }

        public TAHHairProcessor()
        {
            tsohair_processor = TSOHairProcessor.Load(Path.Combine(Application.StartupPath, @"TSOHairProcessor.xml"));
        }

        public void CreateParts()
        {
            parts = new List<TBNHairPart>();
            TBNHairPart part;

            part = new TBNHairPart();
            part.Row = "B";
            part.TbnPath = "N000FHEA_B00.tbn";
            parts.Add(part);

            part = new TBNHairPart();
            part.Row = "C";
            part.TbnPath = "N000BHEA_C00.tbn";
            parts.Add(part);
        }

        public void Dump(string dest_file)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(TAHHairProcessor));
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Encoding = Encoding.GetEncoding("Shift_JIS");
            settings.Indent = true;
            XmlWriter writer = XmlWriter.Create(dest_file, settings);
            serializer.Serialize(writer, this);
            writer.Close();
        }

        public static TAHHairProcessor Load(string source_file)
        {
            XmlReader reader = XmlReader.Create(source_file);
            XmlSerializer serializer = new XmlSerializer(typeof(TAHHairProcessor));
            TAHHairProcessor program = serializer.Deserialize(reader) as TAHHairProcessor;
            reader.Close();
            return program;
        }

        int tah_version = 10;
        public int TahVersion
        {
            set
            {
                tah_version = value;
            }
        }

        string colsname = "default";
        public string Colsname
        {
            set
            {
                colsname = value;
            }
        }

        public static string GetColsRoot()
        {
            return Path.Combine(Application.StartupPath, @"cols");
        }

        public List<string> GetColsNames()
        {
            List<string> names = new List<string>();
            string[] files = Directory.GetFiles(GetColsRoot(), "*.txt");
            foreach (string file in files)
            {
                string name = Path.GetFileNameWithoutExtension(file);
                names.Add(name);
            }
            return names;
        }

        public string GetColsPath()
        {
            return Path.Combine(GetColsRoot(), colsname + ".txt");
        }

        string source_file = null;

        public void Open(string source_file)
        {
            this.source_file = source_file;
            decrypter.Open(source_file);
        }

        public void Close()
        {
            decrypter.Close();
        }

        Regex re_hair_tsofile = new Regex(@"(B|C)00\.tso$");

        public delegate void ProgressChangedHandler(int percent);
        [XmlIgnore]
        public ProgressChangedHandler ProgressChanged;

        public List<TAHEntry> GetTSOHairEntries()
        {
            List<TAHEntry> entries = new List<TAHEntry>();
            foreach (TAHEntry entry in decrypter.Entries)
            {
                if (entry.flag % 2 == 1)
                    continue;

                string path = entry.file_name;

                if (re_hair_tsofile.IsMatch(path))
                {
                    entries.Add(entry);
                }
            }
            return entries;
        }

        public void Process()
        {
            string[] cols = File.ReadAllLines(GetColsPath());

            Encrypter encrypter = new Encrypter();
            encrypter.SourcePath = @".";
            encrypter.Version = tah_version;

            Dictionary<string, TAHEntry> entries = new Dictionary<string, TAHEntry>();

            foreach (TAHEntry entry in GetTSOHairEntries())
            {
                string path = entry.file_name;

                string basename = Path.GetFileNameWithoutExtension(path);
                string code = basename.Substring(0, 8);
                string row = basename.Substring(9, 1);

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

            int entries_count = encrypter.Count;
            int current_index = 0;
            encrypter.GetFileEntryStream = delegate(string path)
            {
                Console.WriteLine("compressing {0}", path);

                string basename = Path.GetFileNameWithoutExtension(path);
                string code = basename.Substring(0, 8);
                string row = basename.Substring(9, 1);
                string col = basename.Substring(10, 2);

                TAHEntry entry = entries[code];
                string ext = Path.GetExtension(path).ToLower();

                Stream ret_stream = null;
                if (ext == ".tbn")
                {
                    string src_path = null;
                    foreach (TBNHairPart part in parts)
                    {
                        if (row == part.Row)
                        {
                            src_path = part.TbnPath;
                            break;
                        }
                    }
                    using (FileStream source_stream = File.OpenRead(Path.Combine(KitRoot, src_path)))
                    {
                        ret_stream = new MemoryStream();
                        ProcessTBNFile(source_stream, ret_stream, basename);
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
                        ProcessTSOFile(tso_stream, ret_stream, col);
                    }
                    ret_stream.Seek(0, SeekOrigin.Begin);
                }
                else
                if (ext == ".psd")
                {
                    string src_path = Path.Combine(KitRoot, string.Format(PsdPath, col));
                    using (FileStream source_stream = File.OpenRead(src_path))
                    {
                        ret_stream = new MemoryStream();
                        Copy(source_stream, ret_stream);
                    }
                    ret_stream.Seek(0, SeekOrigin.Begin);
                }
                current_index++;
                int percent = current_index * 100 / entries_count;
                ProgressChanged(percent);
                return ret_stream;
            };
            encrypter.Save(@"col-" + colsname + "-" + Path.GetFileName(source_file));
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

        Regex re_tsofile = new Regex(@"\.tso$");

        public void ProcessTBNFile(FileStream source_stream, Stream ret_stream, string basename)
        {
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

        public void ProcessTSOFile(Stream tso_stream, Stream ret_stream, string col)
        {
            TSOFile tso = new TSOFile();
            tso.Load(tso_stream);

            tsohair_processor.Process(tso, col);

            tso.Save(ret_stream);
        }

    }
}
