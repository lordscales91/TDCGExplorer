using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using tso2mqo.Extensions;

namespace tso2mqo
{
    public class TSOWriter
    {
        public static void WriteHeader(BinaryWriter bw)
        {
            bw.Write(0x314F5354);
        }

        public static void Write(BinaryWriter bw, string s)
        {
            foreach(byte i in Encoding.Default.GetBytes(s))
                bw.Write(i);

            bw.Write((byte)0);
        }

        public static void Write(BinaryWriter bw, string[] s)
        {
            int n   = s[s.Length-1] == "" ? s.Length-1 : s.Length;

            bw.Write(n);

            for(int i= 0; i < n; ++i)
                Write(bw, s[i]);
        }

        public static void Write(BinaryWriter bw, TSONode[] items)
        {
            bw.Write(items.Length);

            foreach(var i in items)
                Write(bw, i);

            bw.Write(items.Length);

            foreach (var i in items)
            {
                Matrix m = i.Matrix;
                bw.Write(ref m);
            }
        }

        public static void Write(BinaryWriter bw, TSONode item)
        {
            Write(bw, item.Name);
        }

        public static void Write(BinaryWriter bw, TSOTex[] items)
        {
            bw.Write(items.Length);

            foreach(var i in items)
                Write(bw, i);
        }

        public static void Write(BinaryWriter bw, TSOTex item)
        {
            Write(bw, item.name);
            Write(bw, item.file);
            bw.Write(item.Width);
            bw.Write(item.Height);
            bw.Write(item.Depth);
            bw.Write(item.data, 0, item.data.Length);
        }

        public static void Write(BinaryWriter bw, TSOEffect[] items)
        {
            bw.Write(items.Length);

            foreach(var i in items)
                Write(bw, i);
        }

        public static void Write(BinaryWriter bw, TSOEffect item)
        {
            Write(bw, item.Name);
            Write(bw, item.Code.Split('\n'));
        }

        public static void Write(BinaryWriter bw, TSOMaterial[] items)
        {
            bw.Write(items.Length);

            foreach(var i in items)
                Write(bw, i);
        }

        public static void Write(BinaryWriter bw, TSOMaterial item)
        {
            Write(bw, item.Name);
            Write(bw, item.File);
            Write(bw, item.Code.Split('\n'));
        }

        public static void Write(BinaryWriter bw, TSOMesh[] items)
        {
            bw.Write(items.Length);

            foreach(var i in items)
                Write(bw, i);
        }

        public static void Write(BinaryWriter bw, TSOMesh item)
        {
            Write(bw, item.Name);
            Matrix m = item.Matrix;
            bw.Write(ref m);
            bw.Write(1);
            Write(bw, item.sub);
        }

        public static void Write(BinaryWriter bw, TSOSubMesh[] items)
        {
            bw.Write(items.Length);

            foreach(var i in items)
                Write(bw, i);
        }

        public static void Write(BinaryWriter bw, TSOSubMesh item)
        {
            bw.Write(item.spec);
            bw.Write(item.numbones);

            foreach(int k in item.bones)
                bw.Write(k);

            bw.Write(item.numvertices);

            foreach(Vertex k in item.vertices)
                Write(bw, k);
        }

        public unsafe static void Write(BinaryWriter bw, Vertex v)
        {
            uint        idx0    = v.Idx;
            byte*       idx     = (byte*)(&idx0);
            List<int>   idxs    = new List<int>(4);
            List<float> wgts    = new List<float>(4);

            if(v.Wgt.X > 0) { idxs.Add(idx[0]); wgts.Add(v.Wgt.X); }
            if(v.Wgt.Y > 0) { idxs.Add(idx[1]); wgts.Add(v.Wgt.Y); }
            if(v.Wgt.Z > 0) { idxs.Add(idx[2]); wgts.Add(v.Wgt.Z); }
            if(v.Wgt.W > 0) { idxs.Add(idx[3]); wgts.Add(v.Wgt.W); }

            bw.Write(v.Pos.X); bw.Write(v.Pos.Y); bw.Write(v.Pos.Z);
            bw.Write(v.Nrm.X); bw.Write(v.Nrm.Y); bw.Write(v.Nrm.Z);
            bw.Write(v.Tex.X); bw.Write(v.Tex.Y);

            bw.Write(wgts.Count);

            for(int i= 0, n= idxs.Count; i < n; ++i)
            {
                bw.Write(idxs[i]);
                bw.Write(wgts[i]);
            }
        }
    }
}
