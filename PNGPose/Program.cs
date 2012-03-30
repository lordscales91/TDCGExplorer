using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using TDCG;

namespace PNGPose
{
class Program
{
    public static void Main(string[] args)
    {
        if (args.Length != 1)
        {
            System.Console.WriteLine("Usage: PNGPose <png file>");
            return;
        }

        string source_file = args[0];

        if (Path.GetExtension(source_file).ToLower() == ".png")
        {
            string dest_path = Path.GetDirectoryName(source_file);
            string file_name = Path.GetFileName(source_file);
            string folder_name = Path.GetFileNameWithoutExtension(source_file);
            dest_path = Path.Combine(dest_path, folder_name);
            Console.WriteLine("Extract File: " + source_file);

            Program program = new Program();

            program.Extract(source_file, dest_path);
        }
        else if (Directory.Exists(source_file))
        {
            Console.WriteLine("Compose File: " + source_file);

            Program program = new Program();

            program.Compose(source_file);
        }
    }
        int numTMO = 0;

        public int Extract(string source_file, string dest_path)
        {
            try
            {
                if (! System.IO.Directory.Exists(dest_path))
                    System.IO.Directory.CreateDirectory(dest_path);
            }
            catch (Exception)
            {
                System.Console.Out.WriteLine("Error: Cannot prepare destination directory for file writing.");
                return -1;
            }
            PNGFile png = new PNGFile();

            string source_type = null;

            png.Hsav += delegate(string type)
            {
                Console.WriteLine("This is {0} Save File", type);
                DumpSourceType(dest_path + @"\TDCG.txt", type);
                source_type = type;

                string figure_path = dest_path + @"\" + numTMO;
                if (! System.IO.Directory.Exists(figure_path))
                    System.IO.Directory.CreateDirectory(figure_path);
            };
            png.Pose += delegate(string type)
            {
                Console.WriteLine("This is {0} Save File", type);
                DumpSourceType(dest_path + @"\TDCG.txt", type);
                source_type = type;
            };
            png.Scne += delegate(string type)
            {
                Console.WriteLine("This is {0} Save File", type);
                DumpSourceType(dest_path + @"\TDCG.txt", type);
                source_type = type;
            };
            png.Cami += delegate(Stream dest, int extract_length)
            {
                byte[] buf = new byte[extract_length];
                dest.Read(buf, 0, extract_length);

                string dest_file = dest_path + @"\Camera.txt";
                Console.WriteLine("CAMI Save File: " + dest_file);

                using (StreamWriter sw = new StreamWriter(dest_file))
                for (int offset = 0; offset < extract_length; offset+= sizeof(float))
                {
                    float flo = BitConverter.ToSingle(buf, offset);
                    sw.WriteLine(flo);
                }
            };
            png.Lgta += delegate(Stream dest, int extract_length)
            {
                numTMO++;

                byte[] buf = new byte[extract_length];
                dest.Read(buf, 0, extract_length);

                string dest_file = dest_path + @"\LightA" + numTMO + ".txt";
                Console.WriteLine("LGTA Save File: " + dest_file);

                using (StreamWriter sw = new StreamWriter(dest_file))
                for (int offset = 0; offset < extract_length; offset+= sizeof(float))
                {
                    float flo = BitConverter.ToSingle(buf, offset);
                    sw.WriteLine(flo);
                }
            };
            png.Figu += delegate(Stream dest, int extract_length)
            {
                byte[] buf = new byte[extract_length];
                dest.Read(buf, 0, extract_length);

                string dest_file = dest_path + @"\Figure" + numTMO + ".txt";
                Console.WriteLine("FIGU Save File: " + dest_file);

                using (StreamWriter sw = new StreamWriter(dest_file))
                for (int offset = 0; offset < extract_length; offset+= sizeof(float))
                {
                    float flo = BitConverter.ToSingle(buf, offset);
                    sw.WriteLine(flo);
                }

                string figure_path = dest_path + @"\" + numTMO;
                if (! System.IO.Directory.Exists(figure_path))
                    System.IO.Directory.CreateDirectory(figure_path);
            };
            png.Ftmo += delegate(Stream dest, int extract_length)
            {
                string dest_file = dest_path + @"\" + numTMO + ".tmo";
                Console.WriteLine("TMO Save File: " + dest_file);

                using (FileStream file = File.Create(dest_file))
                {
                    byte[] buf = new byte[4096];
                    StreamUtils.Copy(dest, file, buf);
                }
            };
            png.Ftso += delegate(Stream dest, int extract_length, byte[] opt1)
            {
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < 4; i++)
                {
                    sb.Append(string.Format("{0:X2}", opt1[i]));
                }
                string code = sb.ToString();

                string dest_file = dest_path + @"\" + numTMO + @"\" + code + ".tso";
                Console.WriteLine("TSO Save File: " + dest_file);

                using (FileStream file = File.Create(dest_file))
                {
                    byte[] buf = new byte[4096];
                    StreamUtils.Copy(dest, file, buf);
                }
            };

            png.Load(source_file);
            png.Save(dest_path + @"\thumbnail.png");

            if (source_type == "HSAV")
            {
                BMPSaveData data = new BMPSaveData();

                using (Stream stream = File.OpenRead(dest_path + @"\thumbnail.png"))
                    data.Read(stream);

                string dest_file = dest_path + @"\thumbnail.txt";
                Console.WriteLine("dump bmp save data: " + dest_file);
                DumpBmpSaveData(data, dest_file);
            }

            return 0;
        }

        internal string dest_path;

        public int Compose(string dest_path)
        {
            this.dest_path = dest_path;
            PNGFile png = new PNGFile();

            string source_type = ReadSourceType(dest_path + @"\TDCG.txt");
            Console.WriteLine("This is {0} Save File", source_type);

            png.WriteTaOb += delegate(BinaryWriter bw)
            {
                PNGWriter pw = new PNGWriter(bw);
                WriteHsavOrPoseOrScne(pw, source_type);
            };

            png.Load(dest_path + @"\thumbnail.png");
            png.Save(dest_path + @".new.png");
            return 0;
        }

        protected void WriteHsav(PNGWriter pw)
        {
            pw.WriteTDCG();
            pw.WriteHSAV();
            foreach (string file in Directory.GetFiles(dest_path + @"\" + numTMO, "*.tso"))
                WriteFTSO(pw, file);
        }

        protected void WritePose(PNGWriter pw)
        {
            pw.WriteTDCG();
            pw.WritePOSE();
            WriteCAMI(pw);
            WriteLGTA(pw);
            WriteFTMO(pw);
        }

        protected void WriteScne(PNGWriter pw)
        {
            pw.WriteTDCG();
            pw.WriteSCNE(GetFiguresCount());
            WriteCAMI(pw);
            while (NextFIGUExists())
            {
                WriteLGTA(pw);
                WriteFTMO(pw);
                WriteFIGU(pw);
                foreach (string file in Directory.GetFiles(dest_path + @"\" + numTMO, "*.tso"))
                    WriteFTSO(pw, file);
            }
        }

        protected bool NextFIGUExists()
        {
            string dest_file = dest_path + @"\Figure" + (numTMO + 1) + ".txt";
            return File.Exists(dest_file);
        }

        protected int GetFiguresCount()
        {
            int num = 0;
            while ( true )
            {
                string dest_file = dest_path + @"\Figure" + (num + 1) + ".txt";
                if (! File.Exists(dest_file))
                    break;
                num++;
            }
            return num;
        }

        void DumpSourceType(string dest_file, string type)
        {
            using (StreamWriter sw = new StreamWriter(dest_file))
                sw.WriteLine(type);
        }

        string ReadSourceType(string source_file)
        {
            string source_type;
            using (StreamReader source = new StreamReader(File.OpenRead(source_file)))
            {
                source_type = source.ReadLine();
            }
            return source_type;
        }

        protected void WriteHsavOrPoseOrScne(PNGWriter pw, string source_type)
        {
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
            }
        }

        void DumpBmpSaveData(BMPSaveData data, string dest_file)
        {
            using (StreamWriter sw = new StreamWriter(dest_file))
            {
                for (int i = 0; i < 32; i++)
                {
                    sw.WriteLine(data.GetFileName(i));
                }

                for (int i = 0; i < 14; i++)
                {
                    switch (i)
                    {
                        case 0: case 4: case 5: case 6: case 7: case 8: case 11:
                            sw.WriteLine(data.GetSliderValue(i));
                            break;
                        case 1:
                            sw.WriteLine(BitConverter.ToUInt32(data.GetBytes(i), 0));
                            break;
                        default:
                            sw.WriteLine("0x{0:X8}", BitConverter.ToUInt32(data.GetBytes(i), 0));
                            break;
                    }
                }
            }
        }

        protected byte[] ReadFloats(string dest_file)
        {
            List<float> floats = new List<float>();
            string line;
            using (StreamReader source = new StreamReader(File.OpenRead(dest_file)))
            while ((line = source.ReadLine()) != null)
            {
                floats.Add(Single.Parse(line));
            }

            byte[] data = new byte[ sizeof(Single) * floats.Count ];
            int offset = 0;
            foreach (float flo in floats)
            {
                byte[] buf_flo = BitConverter.GetBytes(flo);
                buf_flo.CopyTo(data, offset);
                offset += buf_flo.Length;
            }
            return data;
        }

        protected void WriteCAMI(PNGWriter pw)
        {
            string dest_file = dest_path + @"\Camera.txt";
            Console.WriteLine("CAMI Load File: " + dest_file);

            byte[] cami = ReadFloats(dest_file);
            pw.WriteCAMI(cami);
        }

        protected void WriteLGTA(PNGWriter pw)
        {
            numTMO++;

            string dest_file = dest_path + @"\LightA" + numTMO + ".txt";
            Console.WriteLine("LGTA Load File: " + dest_file);

            byte[] lgta = ReadFloats(dest_file);
            pw.WriteLGTA(lgta);
        }

        protected void WriteFIGU(PNGWriter pw)
        {
            string dest_file = dest_path + @"\Figure" + numTMO + ".txt";
            Console.WriteLine("FIGU Load File: " + dest_file);

            byte[] figu = ReadFloats(dest_file);
            pw.WriteFIGU(figu);
        }

        protected void WriteFTMO(PNGWriter pw)
        {
            string tmo_file = dest_path + @"\" + numTMO + ".tmo";
            Console.WriteLine("TMO Load File: " + tmo_file);

            using (Stream tmo_stream = File.OpenRead(tmo_file))
            {
                pw.WriteFTMO(tmo_stream);
            }
        }

        protected uint opt_value(string code)
        {
            byte[] buf = new byte[4];
            int offset = 0;
            for (int i = 0; i < 8; i += 2)
                buf[offset++] = Convert.ToByte(code.Substring(i, 2), 16);
            return BitConverter.ToUInt32(buf, 0);
        }

        protected void WriteFTSO(PNGWriter pw, string tso_file)
        {
            Console.WriteLine("TSO Load File: " + tso_file);

            string code = Path.GetFileNameWithoutExtension(tso_file);
            uint opt1 = opt_value(code);

            using (Stream tso_stream = File.OpenRead(tso_file))
            {
                pw.WriteFTSO(opt1, tso_stream);
            }
        }
}
}
