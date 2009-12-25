using System;
using System.Collections.Generic;
using System.IO;
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

        if (Path.GetExtension(source_file).ToUpper() == ".PNG")
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
        int numTSO = 0;

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

            png.Hsav += delegate(string type)
            {
                string dest_file = dest_path + @"\TDCG.txt";
                Console.WriteLine("This is HSAV Save File: " + dest_file);

                using (StreamWriter sw = new StreamWriter(dest_file))
                    sw.WriteLine(type);

                string figure_path = dest_path + @"\" + numTMO;
                if (! System.IO.Directory.Exists(figure_path))
                    System.IO.Directory.CreateDirectory(figure_path);
            };
            png.Pose += delegate(string type)
            {
                string dest_file = dest_path + @"\TDCG.txt";
                Console.WriteLine("This is POSE Save File: " + dest_file);

                using (StreamWriter sw = new StreamWriter(dest_file))
                    sw.WriteLine(type);
            };
            png.Scne += delegate(string type)
            {
                string dest_file = dest_path + @"\TDCG.txt";
                Console.WriteLine("This is SCNE Save File: " + dest_file);

                using (StreamWriter sw = new StreamWriter(dest_file))
                    sw.WriteLine(type);
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
                numTMO++; numTSO = 0;

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
                numTSO++;

                string dest_file = dest_path + @"\" + numTMO + @"\" + numTSO + ".tso";
                Console.WriteLine("TSO Save File: " + dest_file);

                using (FileStream file = File.Create(dest_file))
                {
                    byte[] buf = new byte[4096];
                    StreamUtils.Copy(dest, file, buf);
                }

                dest_file = dest_path + @"\" + numTMO + @"\tso" + numTSO + ".txt";
                Console.WriteLine("TSO option Save File: " + dest_file);

                using (StreamWriter sw = new StreamWriter(dest_file))
                for (int i = 0; i < 4; i++)
                {
                    sw.Write("{0:X2}", opt1[i]);
                }
            };

            png.Load(source_file);
            png.Save(dest_path + @"\thumbnail.png");
            return 0;
        }

        internal string dest_path;

        public int Compose(string dest_path)
        {
            this.dest_path = dest_path;
            PNGFile png = new PNGFile();

            png.WriteTaOb += delegate(BinaryWriter bw)
            {
                PNGWriter pw = new PNGWriter(bw);
                WriteHsavOrPoseOrScne(pw);
            };

            png.Load(dest_path + @"\thumbnail.png");
            png.Save(dest_path + @".new.png");
            return 0;
        }

        protected void WriteHsav(PNGWriter pw)
        {
            pw.WriteTDCG();
            pw.WriteHSAV();
            while (NextFTSOExists())
                WriteFTSO(pw);
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
            pw.WriteSCNE(FigureCount());
            WriteCAMI(pw);
            while (NextFIGUExists())
            {
                WriteLGTA(pw);
                WriteFTMO(pw);
                WriteFIGU(pw);
                while (NextFTSOExists())
                    WriteFTSO(pw);
            }
        }

        protected bool NextFIGUExists()
        {
            string dest_file = dest_path + @"\Figure" + (numTMO + 1) + ".txt";
            return File.Exists(dest_file);
        }

        protected bool NextFTSOExists()
        {
            string dest_file = dest_path + @"\" + numTMO + @"\" + (numTSO + 1) + ".tso";
            return File.Exists(dest_file);
        }

        protected int FigureCount()
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

        protected void WriteHsavOrPoseOrScne(PNGWriter pw)
        {
            string dest_file = dest_path + @"\TDCG.txt";

            string source_type;
            using (StreamReader source = new StreamReader(File.OpenRead(dest_file)))
            {
                source_type = source.ReadLine();
            }
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
            numTMO++; numTSO = 0;

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

        protected uint opt_value(string option_file)
        {
            string line;
            using (StreamReader source = new StreamReader(File.OpenRead(option_file)))
                line = source.ReadLine();
            byte[] buf = new byte[4];
            for (int i = 0; i < 4; i++)
                buf[i] = Convert.ToByte(line.Substring(i*2, 2), 16);
            return BitConverter.ToUInt32(buf, 0);
        }

        protected void WriteFTSO(PNGWriter pw)
        {
            numTSO++;

            string tso_file = dest_path + @"\" + numTMO + @"\" + numTSO + ".tso";
            Console.WriteLine("TSO Load File: " + tso_file);

            string option_file = dest_path + @"\" + numTMO + @"\tso" + numTSO + ".txt";
            uint opt1 = opt_value(option_file);

            using (Stream tso_stream = File.OpenRead(tso_file))
            {
                pw.WriteFTSO(opt1, tso_stream);
            }
        }
}
}
