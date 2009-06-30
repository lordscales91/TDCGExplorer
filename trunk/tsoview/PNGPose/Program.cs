using System;
using System.Collections.Generic;
using System.IO;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using TAHdecrypt;

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

        protected BinaryWriter writer;

        internal string dest_path;

        public int Compose(string dest_path)
        {
            this.dest_path = dest_path;
            PNGFile png = new PNGFile();

            png.WriteTaOb += delegate(BinaryWriter bw)
            {
                this.writer = bw;
                WriteHsavOrPoseOrScne();
            };

            png.Load(dest_path + @"\thumbnail.png");
            png.Save(dest_path + @".new.png");
            return 0;
        }

        protected void WriteHsav()
        {
            WriteTDCG();
            WriteHSAV();
            while (NextFTSOExists())
                WriteFTSO();
        }

        protected void WritePose()
        {
            WriteTDCG();
            WritePOSE();
            WriteCAMI();
            WriteLGTA();
            WriteFTMO();
        }

        protected void WriteScne()
        {
            WriteTDCG();
            WriteSCNE();
            WriteCAMI();
            while (NextFIGUExists())
            {
                WriteLGTA();
                WriteFTMO();
                WriteFIGU();
                while (NextFTSOExists())
                    WriteFTSO();
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

        protected void WriteHsavOrPoseOrScne()
        {
            string dest_file = dest_path + @"\TDCG.txt";

            string line;
            using (StreamReader source = new StreamReader(File.OpenRead(dest_file)))
                line = source.ReadLine();
            if (line == "HSAV")
            {
                Console.WriteLine("This is HSAV Save File: " + dest_file);
                WriteHsav();
            }
            else if (line == "POSE")
            {
                Console.WriteLine("This is POSE Save File: " + dest_file);
                WritePose();
            }
            else if (line == "SCNE")
            {
                Console.WriteLine("This is SCNE Save File: " + dest_file);
                WriteScne();
            }
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

        protected void WriteFloats(string type, string dest_file)
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
            WriteTaOb(type, data);
        }

        protected void WriteCAMI()
        {
            string dest_file = dest_path + @"\Camera.txt";
            Console.WriteLine("CAMI Load File: " + dest_file);

            WriteFloats("CAMI", dest_file);
        }

        protected void WriteLGTA()
        {
            numTMO++; numTSO = 0;

            string dest_file = dest_path + @"\LightA" + numTMO + ".txt";
            Console.WriteLine("LGTA Load File: " + dest_file);

            WriteFloats("LGTA", dest_file);
        }

        protected void WriteFIGU()
        {
            string dest_file = dest_path + @"\Figure" + numTMO + ".txt";
            Console.WriteLine("FIGU Load File: " + dest_file);

            WriteFloats("FIGU", dest_file);
        }

        protected void WriteFile(string type, uint opt0, uint opt1, string dest_file)
        {
            using (Stream source = File.OpenRead(dest_file))
                WriteFile(type, opt0, opt1, source);
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

        protected void WriteFTMO()
        {
            string dest_file = dest_path + @"\" + numTMO + ".tmo";
            Console.WriteLine("TMO Load File: " + dest_file);

            WriteFile("FTMO", 0xADCFB72F, 0, dest_file);
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

        protected void WriteFTSO()
        {
            numTSO++;

            string dest_file = dest_path + @"\" + numTMO + @"\" + numTSO + ".tso";
            Console.WriteLine("TSO Load File: " + dest_file);

            string option_file = dest_path + @"\" + numTMO + @"\tso" + numTSO + ".txt";
            WriteFile("FTSO", 0x26F5B8FE, opt_value(option_file), dest_file);
        }
}
}
