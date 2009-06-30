using System;
using System.Collections.Generic;
using System.IO;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Checksums;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;

namespace TDCG
{
    public class PNGFile
    {
        protected BinaryReader reader;

        internal byte[] header;
        internal byte[] ihdr;
        internal List<byte[]> IdatList = new List<byte[]>();

	protected Crc32 crc = new Crc32();

        public delegate void BinaryWriterHandler(BinaryWriter bw);

        public BinaryWriterHandler WriteTaOb;

        public int Save(string dest_file)
        {
            BinaryWriter bw = new BinaryWriter(File.Create(dest_file), System.Text.Encoding.Default);

            PNGWriter.Write(bw, header);
            PNGWriter.WriteIHDR(bw, ihdr);
            foreach (byte[] idat in IdatList)
            {
                PNGWriter.WriteIDAT(bw, idat);
            }
            if (WriteTaOb != null)
                WriteTaOb(bw);
            PNGWriter.WriteIEND(bw);

            bw.Close();
            return 0;
        }

        public delegate void PngHeaderHandler(byte[] header);
        public delegate void PngDataHandler(byte[] data);

        public PngHeaderHandler Header;
        public PngDataHandler Ihdr;
        public PngDataHandler Idat;
        public PngDataHandler Iend;

        public int Load(string source_file)
        {
            this.reader = new BinaryReader(File.OpenRead(source_file), System.Text.Encoding.Default);
            this.header = reader.ReadBytes(8);
            if(header[0] != 0x89
            || header[1] != (byte)'P'
            || header[2] != (byte)'N'
            || header[3] != (byte)'G')
                throw new Exception("File is not PNG");

            if (Header != null)
                Header(header);

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
                    ihdr = chunk_data;
                    if (Ihdr != null)
                        Ihdr(ihdr);
                }
                else if (type == "IDAT")
                {
                    byte[] idat = chunk_data;
                    IdatList.Add(idat);
                    if (Idat != null)
                        Idat(idat);
                }
                else if (type == "IEND")
                {
                    if (Iend != null)
                        Iend(chunk_data);
                    break;
                }
            }
            reader.Close();
            return 0;
        }

        public delegate void TaobTypeHandler(string type);
        public delegate void TaobCamiHandler(Stream stream, int length);
        public delegate void TaobLgtaHandler(Stream stream, int length);
        public delegate void TaobFiguHandler(Stream stream, int length);
        public delegate void TaobFtmoHandler(Stream stream, int length);
        public delegate void TaobFtsoHandler(Stream stream, int length, byte[] opt1);

        public TaobTypeHandler Hsav;
        public TaobTypeHandler Pose;
        public TaobTypeHandler Scne;
        public TaobCamiHandler Cami;
        public TaobLgtaHandler Lgta;
        public TaobFiguHandler Figu;
        public TaobFtmoHandler Ftmo;
        public TaobFtsoHandler Ftso;

        protected void ReadTaOb(byte[] chunk_data)
        {
            String type = System.Text.Encoding.ASCII.GetString(chunk_data, 0, 4);
            //Console.WriteLine("taOb chunk type: {0}", type);
            int extract_length = BitConverter.ToInt32(chunk_data, 12);
            int length = BitConverter.ToInt32(chunk_data, 16);
            //Console.WriteLine("taOb extract length: {0}", extract_length);
            //Console.WriteLine("taOb length: {0}", length);
            MemoryStream ms = new MemoryStream(chunk_data, 20, chunk_data.Length - 20);

            using (Stream dest = new InflaterInputStream(ms))
            if (type == "TDCG")
            {
                byte[] buf = new byte[extract_length];
                dest.Read(buf, 0, extract_length);
                String str = System.Text.Encoding.ASCII.GetString(buf);
                //Console.WriteLine("TDCG data: {0}", str);
            }
            else if (type == "HSAV")
            {
                if (Hsav != null)
                    Hsav(type);
            }
            else if (type == "POSE")
            {
                if (Pose != null)
                    Pose(type);
            }
            else if (type == "SCNE")
            {
                if (Scne != null)
                    Scne(type);
            }
            else if (type == "CAMI") //Camera
            {
                if (Cami != null)
                    Cami(dest, extract_length);
            }
            else if (type == "LGTA") //LightA
            {
                if (Lgta != null)
                    Lgta(dest, extract_length);
            }
            else if (type == "FIGU") //Figure
            {
                if (Figu != null)
                    Figu(dest, extract_length);
            }
            else if (type == "FTMO") //TMO
            {
                if (Ftmo != null)
                    Ftmo(dest, extract_length);
            }
            else if (type == "FTSO") //TSO
            {
                byte[] opt1 = new byte[4];
                Array.Copy(chunk_data, 8, opt1, 0, 4);

                if (Ftso != null)
                    Ftso(dest, extract_length, opt1);
            }
        }
    }
}
