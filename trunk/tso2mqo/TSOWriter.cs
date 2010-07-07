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

        public static void Write(BinaryWriter bw, string[] s)
        {
            int n   = s[s.Length-1] == "" ? s.Length-1 : s.Length;

            bw.Write(n);

            for(int i= 0; i < n; ++i)
                bw.WriteCString(s[i]);
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
            bw.WriteCString(item.Name);
        }

        public static void Write(BinaryWriter bw, TSOTex[] items)
        {
            bw.Write(items.Length);

            foreach(var i in items)
                Write(bw, i);
        }

        public static void Write(BinaryWriter bw, TSOTex item)
        {
            bw.WriteCString(item.name);
            bw.WriteCString(item.file);
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
            bw.WriteCString(item.Name);
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
            bw.WriteCString(item.Name);
            bw.WriteCString(item.File);
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
            bw.WriteCString(item.Name);
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

            bw.Write(ref v.Pos);
            bw.Write(ref v.Nrm);
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
