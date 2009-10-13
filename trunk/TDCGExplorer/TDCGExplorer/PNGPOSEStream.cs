using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Checksums;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;

namespace TDCGExplorer
{
#if false
    public class PNGPoseTsoData
    {
        public UInt32 id = 0;
        public byte[] data = null;
    }
#endif
    public class PNGPoseTmoData
    {
        public byte[] data = null;
    }

    public class PNGPoseLight
    {
        public Byte[] data = null;
    }

    public class PNGPoseFigureData
    {
        public Byte[] data = null;
        public PNGPoseTmoData tmo = null;
        public PNGPoseLight light = null;
        public List<PNGTsoData> tsos = new List<PNGTsoData>();
    }

    public class PNGPoseData
    {
        public bool scene;
        public int nfig = 0;
        public Byte[] camera = null;
        public List<PNGPoseFigureData> figures = new List<PNGPoseFigureData>();

        public List<float> GetCamera()
        {
            List<float> retval = new List<float>();
            for (int offset = 0; offset < camera.Length; offset += sizeof(float))
            {
                float flo = BitConverter.ToSingle(camera, offset);
                retval.Add(flo);
            }
            return retval;
        }
    }

    public class PNGPOSEStream
    {
        protected PNGPoseData posedata;
        protected Crc32 crc = new Crc32();

        public PNGPOSEStream()
        {
        }

        public PNGPoseData PoseData
        {
            set { posedata = value; }
        }

        public PNGPoseData LoadStream(Stream stream)
        {
            posedata = new PNGPoseData();
            try
            {

                PNGFile png = new PNGFile();

                int fnum = 0;
                int flgt = 0;

                png.Hsav += delegate(string type)
                {
                    // ここにはこない.
                };
                png.Pose += delegate(string type)
                {
                    posedata.scene = false;
                };
                png.Scne += delegate(string type)
                {
                    posedata.scene = true;
                };
                png.Cami += delegate(Stream dest, int extract_length)
                {
                    BinaryReader reader = new BinaryReader(dest, System.Text.Encoding.Default);
                    posedata.camera = reader.ReadBytes(extract_length);
                };
                png.Lgta += delegate(Stream dest, int extract_length)
                {
                    if(flgt!=0) fnum++;
                    posedata.figures.Add(new PNGPoseFigureData());
                    BinaryReader reader = new BinaryReader(dest, System.Text.Encoding.Default);
                    posedata.figures[fnum].light=new PNGPoseLight();
                    posedata.figures[fnum].light.data = reader.ReadBytes(extract_length);
                    flgt++;
                };
                png.Figu += delegate(Stream dest, int extract_length)
                {
                    BinaryReader reader = new BinaryReader(dest, System.Text.Encoding.Default);
                    posedata.figures[fnum].data = reader.ReadBytes(extract_length);
                };
                png.Ftmo += delegate(Stream dest, int extract_length)
                {
                    BinaryReader reader = new BinaryReader(dest, System.Text.Encoding.Default);
                    posedata.figures[fnum].tmo = new PNGPoseTmoData();
                    posedata.figures[fnum].tmo.data = reader.ReadBytes(extract_length);
                };
                png.Ftso += delegate(Stream dest, int extract_length, byte[] opt1)
                {
                    PNGTsoData tsodata = new PNGTsoData();

                    // optionをuint32に変換する
                    uint u32;
                    unsafe
                    {
                        Marshal.Copy(opt1, 0, (IntPtr)(void*)&u32, sizeof(uint));
                    }
                    Byte[] data;

                    BinaryReader reader = new BinaryReader(dest, System.Text.Encoding.Default);
                    data = reader.ReadBytes(extract_length);

                    tsodata.tsoID = u32;
                    tsodata.tsodata = data;
                    posedata.figures[fnum].tsos.Add(tsodata);
                };
                png.Load(stream);
                posedata.nfig = fnum+1;
            }
            catch (Exception)
            {
            }
            return posedata;
        }

        // PNGをロードする.
        public PNGFile GetPNG(Stream stream)
        {
            PNGFile png = null;
            try
            {
                png = new PNGFile();
                png.Load(stream);
            }
            catch (Exception)
            {
                png = null;
            }
            return png;
        }

        // ヘビーセーブファイルを保存する.
        public void SavePNGFile(PNGFile png, Stream stream)
        {
            png.WriteTaOb += delegate(BinaryWriter bw)
            {
                if (posedata.scene == false)
                {
                    WriteTDCG(bw);
                    WritePOSE(bw);
                    WriteCAMI(bw, posedata.camera);
                    WriteLGTA(bw, posedata.figures[0].light.data);
                    WriteFTMO(bw, posedata.figures[0].tmo.data);
                }
                else
                {
                    WriteTDCG(bw);
                    WriteSCNE(bw, posedata.nfig);
                    WriteCAMI(bw, posedata.camera);
                    foreach (PNGPoseFigureData figure in posedata.figures)
                    {
                        WriteLGTA(bw, figure.light.data);
                        WriteFTMO(bw, figure.tmo.data);
                        WriteFIGU(bw, figure.data);
                        foreach (PNGTsoData tso in figure.tsos)
                            WriteFTSO(bw, tso.tsoID, tso.tsodata);
                    }

                }
            };
            png.Save(stream);
        }

        protected void WriteTDCG(BinaryWriter bw)
        {
            byte[] data = System.Text.Encoding.ASCII.GetBytes("$XP$");
            WriteTaOb(bw,"TDCG", data);
        }

        protected void WriteHSAV(BinaryWriter bw)
        {
            byte[] data = System.Text.Encoding.ASCII.GetBytes("$XP$");
            WriteTaOb(bw,"HSAV", data);
        }

        protected void WritePOSE(BinaryWriter bw)
        {
            byte[] data = System.Text.Encoding.ASCII.GetBytes("$XP$");
            WriteTaOb(bw, "POSE", data);
        }

        protected void WriteSCNE(BinaryWriter bw, int FigureCount)
        {
            byte[] data = System.Text.Encoding.ASCII.GetBytes("$XP$");
            WriteTaOb(bw, "SCNE", 0, (uint)FigureCount, data);
        }

        protected void WriteFTSO(BinaryWriter bw,uint tsoID,byte[] tsodata)
        {
            WriteTaOb(bw, "FTSO", 0x26F5B8FE, tsoID, tsodata);
        }

        protected void WriteCAMI(BinaryWriter bw, byte[] data)
        {
            WriteTaOb(bw, "CAMI", data);
        }

        protected void WriteLGTA(BinaryWriter bw, byte[] data)
        {
            WriteTaOb(bw, "LGTA", data);
        }

        protected void WriteFIGU(BinaryWriter bw, byte[] data)
        {
            WriteTaOb(bw, "FIGU", data);
        }

        protected void WriteFTMO(BinaryWriter bw,byte[] data)
        {
            WriteTaOb(bw,"FTMO", 0xADCFB72F, 0, data);
        }

        protected void WriteTaOb(BinaryWriter bw,string type, uint opt0, uint opt1, byte[] data)
        {
            byte[] chunk_type = System.Text.Encoding.ASCII.GetBytes(type);

            MemoryStream dest = new MemoryStream();
            using (DeflaterOutputStream gzip = new DeflaterOutputStream(dest))
            {
                gzip.IsStreamOwner = false;
                gzip.Write(data, 0, data.Length);
            }
            dest.Seek(0, SeekOrigin.Begin);
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
            WriteChunk(bw,"taOb", chunk_data);
        }

        protected void WriteChunk(BinaryWriter bw,string type, byte[] chunk_data)
        {
            byte[] buf = BitConverter.GetBytes((UInt32)chunk_data.Length);
            Array.Reverse(buf);
            bw.Write(buf);

            byte[] chunk_type = System.Text.Encoding.ASCII.GetBytes(type);
            bw.Write(chunk_type);
            bw.Write(chunk_data);

            crc.Reset();
            crc.Update(chunk_type);
            crc.Update(chunk_data);

            byte[] crc_buf = BitConverter.GetBytes((UInt32)crc.Value);
            Array.Reverse(crc_buf);
            bw.Write(crc_buf);
        }

        protected void WriteTaOb(BinaryWriter bw, string type, byte[] data)
        {
            WriteTaOb(bw,type, 0, 0, data);
        }

    }
}
