using System;
using System.Collections.Generic;
using System.IO;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Checksums;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;

namespace TAHdecrypt
{
    public class PNGFile
    {
        protected BinaryReader reader;
        protected BinaryWriter writer;

        internal string dest_path;
        internal byte[] header;

        internal int numTMO = 0;
        internal int numTSO = 0;
	protected Crc32 crc = new Crc32();

        public int Compose(string dest_path)
        {
            this.reader = new BinaryReader(File.OpenRead(dest_path + @"\thumbnail.png"), System.Text.Encoding.Default);
            this.writer = new BinaryWriter(File.Create(dest_path + @".new.png"), System.Text.Encoding.Default);
            this.dest_path = dest_path;
            this.header = reader.ReadBytes(8);

            writer.Write(header);

            while ( true ) {
                byte[] buf = reader.ReadBytes(4);
                Array.Reverse(buf);
                int length = (int)BitConverter.ToUInt32(buf, 0);

                byte[] chunk_type = reader.ReadBytes(4);
                String type = System.Text.Encoding.ASCII.GetString(chunk_type);

                byte[] chunk_data = reader.ReadBytes(length);
                byte[] crc_buf = reader.ReadBytes(4);
                Array.Reverse(crc_buf);
                uint sum = BitConverter.ToUInt32(crc_buf, 0);

                crc.Reset();
                crc.Update(chunk_type);
                crc.Update(chunk_data);

		if (sum != crc.Value)
		    throw new ICSharpCode.SharpZipLib.GZip.GZipException("GZIP crc sum mismatch");

                if (type == "IHDR")
                {
                    WriteIHDR(chunk_data);
                }
                else if (type == "IDAT")
                {
                    WriteIDAT(chunk_data);
                }
                else if (type == "IEND")
                {
                    WriteHsavOrPoseOrScne();
                    WriteIEND();
                    break;
                }
            }
            return 0;
        }

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
            this.reader = new BinaryReader(File.OpenRead(source_file), System.Text.Encoding.Default);
            this.writer = new BinaryWriter(File.Create(dest_path + @"\thumbnail.png"), System.Text.Encoding.Default);
            this.dest_path = dest_path;
            this.header = reader.ReadBytes(8);

            writer.Write(header);

            while ( true ) {
                byte[] buf = reader.ReadBytes(4);
                Array.Reverse(buf);
                int length = (int)BitConverter.ToUInt32(buf, 0);

                byte[] chunk_type = reader.ReadBytes(4);
                String type = System.Text.Encoding.ASCII.GetString(chunk_type);

                byte[] chunk_data = reader.ReadBytes(length);
                byte[] crc_buf = reader.ReadBytes(4);
                Array.Reverse(crc_buf);
                uint sum = BitConverter.ToUInt32(crc_buf, 0);

                crc.Reset();
                crc.Update(chunk_type);
                crc.Update(chunk_data);

		if (sum != crc.Value)
		    throw new ICSharpCode.SharpZipLib.GZip.GZipException("GZIP crc sum mismatch");

                if (type == "taOb")
                {
                    ReadTaOb(chunk_data);
                }
                else if (type == "IHDR")
                {
                    WriteIHDR(chunk_data);
                }
                else if (type == "IDAT")
                {
                    WriteIDAT(chunk_data);
                }
                else if (type == "IEND")
                {
                    WriteIEND();
                    break;
                }
            }
            return 0;
        }

        protected void WriteChunk(string type, byte[] chunk_data)
        {
            byte[] buf = BitConverter.GetBytes((UInt32)chunk_data.Length);
            Array.Reverse(buf);
            writer.Write(buf);

            byte[] chunk_type = System.Text.Encoding.ASCII.GetBytes(type);
            writer.Write(chunk_type);
            writer.Write(chunk_data);

            crc.Reset();
            crc.Update(chunk_type);
            crc.Update(chunk_data);

            byte[] crc_buf = BitConverter.GetBytes((UInt32)crc.Value);
            Array.Reverse(crc_buf);
            writer.Write(crc_buf);
        }

        protected void WriteIHDR(byte[] chunk_data)
        {
            WriteChunk("IHDR", chunk_data);
        }

        protected void WriteIDAT(byte[] chunk_data)
        {
            WriteChunk("IDAT", chunk_data);
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

        protected void CreateFigureDirectory()
        {
            string figure_path = dest_path + @"\" + numTMO;
            if (! System.IO.Directory.Exists(figure_path))
                System.IO.Directory.CreateDirectory(figure_path);
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

        protected void WriteIEND()
        {
            WriteChunk("IEND", new byte[] {});
        }

        protected void ReadIHDR(byte[] chunk_data)
        {
            WriteIHDR(chunk_data);
        }

        protected void ReadIDAT(byte[] chunk_data)
        {
            WriteIDAT(chunk_data);
        }

        protected void ReadIEND()
        {
            WriteIEND();
        }

        protected void WriteTaOb(string type, uint opt0, uint opt1, byte[] data)
        {
            Console.WriteLine("WriteTaOb {0}", type);
            Console.WriteLine("taOb extract length {0}", data.Length);
            byte[] chunk_type = System.Text.Encoding.ASCII.GetBytes(type);

            MemoryStream dest = new MemoryStream();
            using (DeflaterOutputStream gzip = new DeflaterOutputStream(dest))
            {
                gzip.IsStreamOwner = false;
                gzip.Write(data, 0, data.Length);
            }
            dest.Seek(0, SeekOrigin.Begin);
            Console.WriteLine("taOb length {0}", dest.Length);
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
            WriteChunk("taOb", chunk_data);
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
            Console.WriteLine("taOb extract length {0}", new FileInfo(dest_file).Length);

            MemoryStream dest = new MemoryStream();
            using (Stream source = File.OpenRead(dest_file))
            using (DeflaterOutputStream gzip = new DeflaterOutputStream(dest))
            {
                gzip.IsStreamOwner = false;

                byte[] b = new byte[4096];
                StreamUtils.Copy(source, gzip, b);
            }
            dest.Seek(0, SeekOrigin.Begin);
            Console.WriteLine("taOb length {0}", dest.Length);

            byte[] chunk_type = System.Text.Encoding.ASCII.GetBytes(type);
            byte[] chunk_data = new byte[dest.Length + 20];

            Array.Copy(chunk_type, 0, chunk_data, 0, 4);

            byte[] buf;
            buf = BitConverter.GetBytes((UInt32)opt0);
            Array.Copy(buf, 0, chunk_data, 4, 4);
            buf = BitConverter.GetBytes((UInt32)opt1);
            Array.Copy(buf, 0, chunk_data, 8, 4);

            buf = BitConverter.GetBytes((UInt32)new FileInfo(dest_file).Length);
            Array.Copy(buf, 0, chunk_data, 12, 4);
            buf = BitConverter.GetBytes((UInt32)dest.Length);
            Array.Copy(buf, 0, chunk_data, 16, 4);

            dest.Read(chunk_data, 20, (int)dest.Length);
            WriteChunk("taOb", chunk_data);
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

        protected void ReadTaOb(byte[] chunk_data)
        {
            String type = System.Text.Encoding.ASCII.GetString(chunk_data, 0, 4);
            Console.WriteLine("taOb chunk type: {0}", type);
            int extract_length = BitConverter.ToInt32(chunk_data, 12);
            int length = BitConverter.ToInt32(chunk_data, 16);
            Console.WriteLine("taOb extract length: {0}", extract_length);
            Console.WriteLine("taOb length: {0}", length);
            MemoryStream ms = new MemoryStream(chunk_data, 20, chunk_data.Length - 20);

            using (Stream dest = new InflaterInputStream(ms))
            if (type == "TDCG")
            {
                byte[] buf = new byte[extract_length];
                dest.Read(buf, 0, extract_length);
                String str = System.Text.Encoding.ASCII.GetString(buf);
                Console.WriteLine("TDCG data: {0}", str);
            }
            else if (type == "HSAV")
            {
                byte[] buf = new byte[extract_length];
                dest.Read(buf, 0, extract_length);

                string dest_file = dest_path + @"\TDCG.txt";
                Console.WriteLine("This is HSAV Save File: " + dest_file);

                using (StreamWriter sw = new StreamWriter(dest_file))
                    sw.WriteLine(type);

                CreateFigureDirectory();
            }
            else if (type == "POSE")
            {
                byte[] buf = new byte[extract_length];
                dest.Read(buf, 0, extract_length);

                string dest_file = dest_path + @"\TDCG.txt";
                Console.WriteLine("This is POSE Save File: " + dest_file);

                using (StreamWriter sw = new StreamWriter(dest_file))
                    sw.WriteLine(type);
            }
            else if (type == "SCNE")
            {
                byte[] buf = new byte[extract_length];
                dest.Read(buf, 0, extract_length);

                string dest_file = dest_path + @"\TDCG.txt";
                Console.WriteLine("This is SCNE Save File: " + dest_file);

                using (StreamWriter sw = new StreamWriter(dest_file))
                    sw.WriteLine(type);
            }
            else if (type == "CAMI") //Camera
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
            }
            else if (type == "LGTA") //LightA
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
            }
            else if (type == "FIGU") //Figure
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

                CreateFigureDirectory();
            }
            else if (type == "FTMO") //TMO
            {
                string dest_file = dest_path + @"\" + numTMO + ".tmo";
                Console.WriteLine("TMO Save File: " + dest_file);

                using (FileStream file = File.Create(dest_file))
                {
                    byte[] buf = new byte[4096];
                    StreamUtils.Copy(dest, file, buf);
                }
            }
            else if (type == "FTSO") //TSO
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
                for (int i = 8; i < 12; i++)
                {
                    sw.Write("{0:X2}", chunk_data[i]);
                }
            }
        }
    }
}
