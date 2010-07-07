using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace Tso2MqoGui
{
    public enum MqoBoneMode
    {
        None,
        RokDeBone,
        Mikoto,
    }

    public class Pair<T, U>
    {
        public T    First;
        public U    Second;

        public Pair()
        {
        }

        public Pair(T first, U second)
        {
            First   = first;
            Second  = second;
        }
    }

    public class MqoWriter : IDisposable
    {
        public TextWriter   tw;
        public string       OutPath;
        public string       OutFile;
        public MqoBoneMode  BoneMode    = MqoBoneMode.None;

        public MqoWriter(string file)
        {
            FileStream  fs  = File.OpenWrite(file);
            fs.SetLength(0);
            tw              = new StreamWriter(fs, Encoding.Default);
            OutFile         = file;
            OutPath         = Path.GetDirectoryName(file);
        }

        void IDisposable.Dispose()
        {
            Close();
        }

        public void Close()
        {
            if(tw != null)
                tw.Close();
            tw  = null;
        }

        public void CreateTextureFile(TSOTex tex)
        {
            string  file= Path.Combine(OutPath, Path.GetFileName(tex.file.Trim('"')));
            byte[]  data= tex.data;

            /*
            if(Path.GetExtension(file).ToUpper() == ".BMP")
            for(int i= 0; i < data.Length; i+=tex.Depth)
            {
                byte    x   = data[i+0];
                data[i+0]   = data[i+2];
                data[i+2]   = x;
            }
            */

            using(FileStream fs= File.OpenWrite(file))
            {
                BinaryWriter    bw  = new BinaryWriter(fs);

                switch(Path.GetExtension(file).ToUpper())
                {
                case ".TGA":
                    bw.Write((byte)0);              // id
                    bw.Write((byte)0);              // colormap
                    bw.Write((byte)2);              // imagetype
                    bw.Write((byte)0);              // unknown0
                    bw.Write((byte)0);              // unknown1
                    bw.Write((byte)0);              // unknown2
                    bw.Write((byte)0);              // unknown3
                    bw.Write((byte)0);              // unknown4
                    bw.Write((short)0);             // width
                    bw.Write((short)0);             // height
                    bw.Write((short)tex.Width);     // width
                    bw.Write((short)tex.Height);    // height
                    bw.Write((byte)(tex.depth * 8));// depth
                    bw.Write((byte)0);              // depth
                    break;

                case ".BMP":
                    bw.Write((byte)'B');
                    bw.Write((byte)'M');
                    bw.Write((int)(54 + data.Length));
                    bw.Write((int)0);
                    bw.Write((int)54);
                    bw.Write((int)40);
                    bw.Write((int)tex.Width);
                    bw.Write((int)tex.Height);
                    bw.Write((short)1);
                    bw.Write((short)(tex.Depth*8));
                    bw.Write((int)0);
                    bw.Write((int)data.Length);
                    bw.Write((int)0);
                    bw.Write((int)0);
                    bw.Write((int)0);
                    bw.Write((int)0);
                    break;
                }

                bw.Write(data, 0, data.Length);
                bw.Flush();
            }
        }

        public void Write(TSOFile file)
        {
            tw.WriteLine("Metasequoia Document");
            tw.WriteLine("Format Text Ver 1.0");
            tw.WriteLine("");
            tw.WriteLine("Scene {");
            tw.WriteLine("	pos -7.0446 4.1793 1541.1764");
            tw.WriteLine("	lookat 11.8726 193.8590 0.4676");
            tw.WriteLine("	head 0.8564");
            tw.WriteLine("	pich 0.1708");
            tw.WriteLine("	ortho 0");
            tw.WriteLine("	zoom2 31.8925");
            tw.WriteLine("	amb 0.250 0.250 0.250");
            tw.WriteLine("}");

          //VertexHeap<UVertex> vh  = new VertexHeap<UVertex>();
            VertexHeap          vh  = new VertexHeap();
            List<ushort>        face= new List<ushort>(2048*3);
            List<float>         uv  = new List<float>(2048*3 * 2);
            List<int>           mtl = new List<int>(2048);

            foreach(TSOTex i in file.textures)
                CreateTextureFile(i);

            tw.WriteLine("Material {0} {{", file.materials.Length);

            foreach(TSOMaterial i in file.materials)
            {
                TSOTex  tex = file.texturemap[i.ColorTex];
                tw.WriteLine(
                    "	\"{0}\" shader(3) col(1.00 1.00 1.00 1.00) dif(0.800) amb(0.600) emi(0.000) spc(0.000) power(5.00) tex(\"{1}\")",
                    i.name, Path.Combine(OutPath, tex.File.Trim('"')));
            }

            tw.WriteLine("}");

            foreach(TSOMesh i in file.meshes)
            {
                vh.Clear();
                face.Clear();
                uv.Clear();
                mtl.Clear();

                foreach(TSOSubMesh j in i.sub)
                {
                    int     cnt = 0;
                    ushort  a= 0, b= 0, c= 0;
                    Vertex  va= new Vertex(), vb= new Vertex(), vc= new Vertex();

                    foreach(Vertex k in j.vertices)
                    {
                        ++cnt;
                        va= vb; a= b;
                        vb= vc; b= c;
                        vc= k;  c= vh.Add(new UVertex(k.Pos.X, k.Pos.Y, k.Pos.Z, k.Nrm.X, k.Nrm.Y, k.Nrm.Z, k.Tex.X, k.Tex.Y, j.spec));

                        if(cnt < 3)                     continue;
                        if(a == b || b == c || c == a)  continue;

                        if((cnt & 1) == 0)
                        {
                          //face.Add(a); uv.Add(va.Tex.x); uv.Add(va.Tex.y);
                          //face.Add(b); uv.Add(vb.Tex.x); uv.Add(vb.Tex.y);
                          //face.Add(c); uv.Add(vc.Tex.x); uv.Add(vc.Tex.y);
                            face.Add(a); uv.Add(va.Tex.X); uv.Add(1-va.Tex.Y);
                            face.Add(b); uv.Add(vb.Tex.X); uv.Add(1-vb.Tex.Y);
                            face.Add(c); uv.Add(vc.Tex.X); uv.Add(1-vc.Tex.Y);
                            mtl.Add(j.spec);
                        } else
                        {
                          //face.Add(a); uv.Add(va.Tex.x); uv.Add(va.Tex.y);
                          //face.Add(c); uv.Add(vc.Tex.x); uv.Add(vc.Tex.y);
                          //face.Add(b); uv.Add(vb.Tex.x); uv.Add(vb.Tex.y);
                            face.Add(a); uv.Add(va.Tex.X); uv.Add(1-va.Tex.Y);
                            face.Add(c); uv.Add(vc.Tex.X); uv.Add(1-vc.Tex.Y);
                            face.Add(b); uv.Add(vb.Tex.X); uv.Add(1-vb.Tex.Y);
                            mtl.Add(j.spec);
                        }
                    }
                }

                tw.WriteLine("Object \"{0}\" {{", i.Name);
                tw.WriteLine("	visible {0}", 15);
                tw.WriteLine("	locking {0}", 0);
                tw.WriteLine("	shading {0}", 1);
                tw.WriteLine("	facet {0}", 59.5);
                tw.WriteLine("	color {0} {1} {2}", 0.898f, 0.498f, 0.698f);
                tw.WriteLine("	color_type {0}", 0);

                //
                tw.WriteLine("	vertex {0} {{", vh.Count);

                foreach(UVertex j in vh.verts)
                    WriteVertex(j.x, j.y, j.z);

                tw.WriteLine("	}");

                //
                tw.WriteLine("	face {0} {{", face.Count);

                System.Diagnostics.Debug.Assert(face.Count*2 == uv.Count);
                System.Diagnostics.Debug.Assert(face.Count == mtl.Count * 3);

                for(int j= 0, n= face.Count; j < n; j+=3)
                    WriteFace(face[j+0], face[j+1], face[j+2],
#if true
                              uv[j*2+0], uv[j*2+1],
                              uv[j*2+2], uv[j*2+3],
                              uv[j*2+4], uv[j*2+5],
#else
                              uv[j*2+0], uv[j*2+2], uv[j*2+4],
                              uv[j*2+1], uv[j*2+3], uv[j*2+5],
#endif
                              mtl[j/3]);
                tw.WriteLine("	}");
                tw.WriteLine("}");
            }

            // ボーンを出す
            switch(BoneMode)
            {
            case MqoBoneMode.None:      break;
            case MqoBoneMode.RokDeBone:
                {
                // マトリクス計算
                foreach(TSONode i in file.nodes)
                {
                    if(i.parent == null)
                            i.world = i.Matrix;
                    else    i.world = Matrix.Multiply(i.Matrix, i.parent.World);
                }
                
#if false
                // 位置一覧
                Dictionary<string, Point3>  pointmap= new Dictionary<string, Point3>();
                Dictionary<string, int>     indexmap= new Dictionary<string, int();

                foreach(TSONode i in file.nodes)
                {
                    points.Add(i.World.Translation);
                    pointmap.Add(i.name, i.World.Translation);
                }

              //RDBBonFile                  bonfile = new RDBBonFile();
#endif
                List<Vector3> points = new List<Vector3>();
                List<int>                   bones   = new List<int>();

                tw.WriteLine("Object \"{0}\" {{", "Bone");
#if true
                tw.WriteLine("	visible {0}", 15);
                tw.WriteLine("	locking {0}", 0);
                tw.WriteLine("	shading {0}", 1);
                tw.WriteLine("	facet {0}", 59.5);
                tw.WriteLine("	color {0} {1} {2}", 1, 0, 0);
                tw.WriteLine("	color_type {0}", 0);
#else
                tw.WriteLine("	depth {0}", 0);
	            tw.WriteLine("	folding {0}", 0);
	            tw.WriteLine("	scale {0} {1} {2}", 1.000000, 1.000000, 1.000000);
	            tw.WriteLine("	rotation {0} {1} {2}", 0.000000, 0.000000, 0.000000);
	            tw.WriteLine("	translation {0} {1} {2}", 0.000000, 0.000000, 0.000000);
	            tw.WriteLine("	visible {0}", 15);
	            tw.WriteLine("	locking {0}", 0);
	            tw.WriteLine("	shading {0}", 1);
	            tw.WriteLine("	facet {0}", 59.5);
	            tw.WriteLine("	color {0} {1} {2}", 0.898, 0.498, 0.698);
	            tw.WriteLine("	color_type {0}", 0);
#endif

                foreach(TSONode i in file.nodes)
                {
                    if(i.children.Count == 0)
                        continue;

                    Vector3 q = new Vector3(i.world.M41, i.world.M42, i.world.M43);
                    Vector3 p = Vector3.Empty;
                    
                    foreach(TSONode j in i.children)
                    {
                        p.X     +=j.world.M41;
                        p.Y     +=j.world.M42;
                        p.Z     +=j.world.M43;
                    }

                    p.X         /=i.children.Count;
                    p.Y         /=i.children.Count;
                    p.Z         /=i.children.Count;

                    bones.Add(points.Count); points.Add(q);
                    bones.Add(points.Count); points.Add(p);
                }

                tw.WriteLine("	vertex {0} {{", points.Count);

                foreach (Vector3 j in points)
                    WriteVertex(j.X, j.Y, j.Z);

                tw.WriteLine("	}");

                //
                tw.WriteLine("	face {0} {{", bones.Count / 2);

                for(int j= 0, n= bones.Count; j < n; j+=2)
                    tw.WriteLine(string.Format("		2 V({0} {1})", bones[j+0], bones[j+1]));

                tw.WriteLine("	}");
                tw.WriteLine("}");
                }
                break;

            case MqoBoneMode.Mikoto:
                {
                }
                break;
            }

            tw.WriteLine("Eof");
        }

        public void WriteFace(int a, int b, int c, float u1, float v1, float u2, float v2, float u3, float v3, int m)
        {
            tw.WriteLine("		{0} V({1} {2} {3}) M({10}) UV({4} {5} {6} {7} {8} {9})",
                3, a, b, c, u1, v1, u2, v2, u3, v3, m);
        }

        public void WriteVertex(float x, float y, float z)
        {
            tw.WriteLine("		{0} {1} {2}", x.ToString("N6"), y.ToString("N6"), z.ToString("N6"));
        }
    }

    public class UVertex : IComparable<UVertex>
    {
        public float    x, y, z, nx, ny, nz, u, v;
        public int      mtl;
        
        public UVertex()
        {
        }

        public UVertex(float x, float y, float z, float nx, float ny, float nz, float u, float v, int mtl)
        {
            this.x  = x;
            this.y  = y;
            this.z  = z;
            this.nx = nx;
            this.ny = ny;
            this.nz = nz;
            this.u  = u;
            this.v  = v;
            this.mtl= mtl;
        }

        public int CompareTo(UVertex o)
        {
            if(x   < o.x)   return -1; if(x   > o.x)   return 1;
            if(y   < o.y)   return -1; if(y   > o.y)   return 1;
            if(z   < o.z)   return -1; if(z   > o.z)   return 1;
            if(nx  < o.nx)  return -1; if(nx  > o.nx)  return 1;
            if(ny  < o.ny)  return -1; if(ny  > o.ny)  return 1;
            if(nz  < o.nz)  return -1; if(nz  > o.nz)  return 1;
            if(u   < o.u)   return -1; if(u   > o.u)   return 1;
            if(v   < o.v)   return -1; if(v   > o.v)   return 1;
            return mtl - o.mtl;
        }

        public override int GetHashCode()
        {
            return x .GetHashCode() ^ y .GetHashCode() ^ z .GetHashCode()
                 ^ nx.GetHashCode() ^ ny.GetHashCode() ^ nz.GetHashCode()
               //^ u .GetHashCode() ^ v .GetHashCode()
                 ^ mtl.GetHashCode()
                 ;
        }

        public override bool Equals(object obj)
        {
            UVertex o   = obj as UVertex;

            if(o == null)
                return false;

            return x  == o.x  && y  == o.y  && y  == o.y
                && nx == o.nx && ny == o.ny && ny == o.ny
              //&& u  == o.u  && v  == o.y
                && mtl == o.mtl
                ;
        }
    }
}
