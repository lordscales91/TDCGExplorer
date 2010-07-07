using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Tso2MqoGui
{
    public class TDCGFile
    {
        protected BinaryReader  r;

        public TDCGFile(string file)
            : this(File.OpenRead(file))
        {
        }

        public TDCGFile(Stream s)
            : this(new BinaryReader(s))
        {
        }

        public TDCGFile(BinaryReader r)
        {
            this.r  = r;
        }

        public static int   debug_count = 0;

        public void ReadVertex(ref Vertex v)
        {
            v.Pos.X     = r.ReadSingle();
            v.Pos.Y     = r.ReadSingle();
            v.Pos.Z     = r.ReadSingle();
            v.Nrm.X     = r.ReadSingle();
            v.Nrm.Y     = r.ReadSingle();
            v.Nrm.Z     = r.ReadSingle();
            v.Tex.X     = r.ReadSingle();
            v.Tex.Y     = r.ReadSingle();

            int     cnt = r.ReadInt32();
            byte[]  idx = new byte[4]{0, 0, 0, 0};

          //System.Diagnostics.Debug.Assert(cnt >= 1 && cnt <= 4);
#if false
            if(cnt >= 1)    { idx[0]= (byte)r.ReadInt32(); v.Wgt.X = r.ReadSingle(); }
            if(cnt >= 2)    { idx[1]= (byte)r.ReadInt32(); v.Wgt.Y = r.ReadSingle(); }
            if(cnt >= 3)    { idx[2]= (byte)r.ReadInt32(); v.Wgt.Z = r.ReadSingle(); }
            if(cnt >= 4)    { idx[3]= (byte)r.ReadInt32(); v.Wgt.W = r.ReadSingle(); }
#endif
#if false
            if(cnt >= 1)    { idx[3]= (byte)r.ReadInt32(); v.Wgt.X = r.ReadSingle(); }
            if(cnt >= 2)    { idx[2]= (byte)r.ReadInt32(); v.Wgt.Y = r.ReadSingle(); }
            if(cnt >= 3)    { idx[1]= (byte)r.ReadInt32(); v.Wgt.Z = r.ReadSingle(); }
            if(cnt >= 4)    { idx[0]= (byte)r.ReadInt32(); v.Wgt.W = r.ReadSingle(); }
#endif
#if false
            if(cnt >= 1)    { idx[0]= (byte)r.ReadInt32(); v.Wgt.W = r.ReadSingle(); }
            if(cnt >= 2)    { idx[1]= (byte)r.ReadInt32(); v.Wgt.Z = r.ReadSingle(); }
            if(cnt >= 3)    { idx[2]= (byte)r.ReadInt32(); v.Wgt.Y = r.ReadSingle(); }
            if(cnt >= 4)    { idx[3]= (byte)r.ReadInt32(); v.Wgt.X = r.ReadSingle(); }
#endif
#if true
            if(cnt >= 1)    { idx[3]= (byte)r.ReadInt32(); v.Wgt.W = r.ReadSingle(); }
            if(cnt >= 2)    { idx[2]= (byte)r.ReadInt32(); v.Wgt.Z = r.ReadSingle(); }
            if(cnt >= 3)    { idx[1]= (byte)r.ReadInt32(); v.Wgt.Y = r.ReadSingle(); }
            if(cnt >= 4)    { idx[0]= (byte)r.ReadInt32(); v.Wgt.X = r.ReadSingle(); }
            if(cnt >= 5)    { r.ReadInt32(); r.ReadSingle(); }
            if(cnt >= 6)    { r.ReadInt32(); r.ReadSingle(); }
            if(cnt >= 7)    { r.ReadInt32(); r.ReadSingle(); }
            if(cnt >= 8)    { r.ReadInt32(); r.ReadSingle(); }
#endif

            /*
          //if(debug_count++ < 30)
            {
                System.Diagnostics.Debug.WriteLine("["
                    + idx[0] + ":" + v.Wgt.X.ToString("N8") +"], ["
                    + idx[1] + ":" + v.Wgt.Y.ToString("N8") +"], ["
                    + idx[2] + ":" + v.Wgt.Z.ToString("N8") +"], ["
                    + idx[3] + ":" + v.Wgt.W.ToString("N8") +"]");
            }
            */

            v.Idx   = BitConverter.ToUInt32(idx, 0);
        }

        public void ReadVertexDebug(ref Vertex v)
        {
            v.Pos.X     = r.ReadSingle();
            v.Pos.Y     = r.ReadSingle();
            v.Pos.Z     = r.ReadSingle();
            v.Nrm.X     = r.ReadSingle();
            v.Nrm.Y     = r.ReadSingle();
            v.Nrm.Z     = r.ReadSingle();
            v.Tex.X     = r.ReadSingle();
            v.Tex.Y     = r.ReadSingle();

            int     cnt = r.ReadInt32();
            byte[]  idx = new byte[4]{0, 0, 0, 0};

            System.Diagnostics.Debug.Assert(cnt >= 1 && cnt <= 4);

            if(cnt >= 1)    { idx[3]= (byte)r.ReadInt32(); v.Wgt.W = r.ReadSingle(); }
            if(cnt >= 2)    { idx[2]= (byte)r.ReadInt32(); v.Wgt.Z = r.ReadSingle(); }
            if(cnt >= 3)    { idx[1]= (byte)r.ReadInt32(); v.Wgt.Y = r.ReadSingle(); }
            if(cnt >= 4)    { idx[0]= (byte)r.ReadInt32(); v.Wgt.X = r.ReadSingle(); }

            v.Idx   = BitConverter.ToUInt32(idx, 0);

            System.Diagnostics.Debug.WriteLine("cnt=" + cnt);
        }

        Encoding    enc = Encoding.GetEncoding("Shift_JIS");
        List<byte>  buf = new List<byte>();

        public string ReadString()
        {
            buf.Clear();

            for(;;)
            {
                byte    b   = r.ReadByte();

                if(b == 0)
                    break;

                buf.Add(b);
            }

            return enc.GetString(buf.ToArray());
        }

        public unsafe Matrix44 ReadMatrix()
        {
            Matrix44    m   = new Matrix44();
            float*      p   = &m.m11;

            for(int i= 0; i < 16; ++i)
                *p++    = r.ReadSingle();

            return m;
        }
    }
}
