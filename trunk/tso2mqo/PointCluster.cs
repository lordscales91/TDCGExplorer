using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace tso2mqo
{
    public class Array3D<T>
    {
        public T[]  data;
        public int  xx, yy, zz;

        public Array3D(int x, int y, int z)
        {
            data    = new T[x*y*z];
            xx      = x;
            yy      = y;
            zz      = z;
        }

        public T Get(int x, int y, int z)
        {
            return data[x+y*xx+z*xx*yy];
        }

        public void Set(int x, int y, int z, T v)
        {
            data[x+y*xx+z*xx*yy]    = v;
        }
    }

    public class PointCluster
    {
        public List<Vector3> points;
        public int                  div;
        public float                divu;
        public Array3D<List<int>>   clusters;
        public Vector3 min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
        public Vector3 max = new Vector3(float.MinValue, float.MinValue, float.MinValue);

        public PointCluster(int n)
        {
            points = new List<Vector3>(n);
        }

        public Vector3 GetPoint(int i)
        {
            return points[i];
        }

        public void Add(Vector3 p)
        {
            points.Add(p);
            if(p.X < min.X) min.X= p.X; else if(p.X > max.X) max.X= p.X;
            if(p.Y < min.Y) min.Y= p.Y; else if(p.Y > max.Y) max.Y= p.Y;
            if(p.Z < min.Z) min.Z= p.Z; else if(p.Z > max.Z) max.Z= p.Z;
        }

        public void Add(float x, float y, float z)
        {
            Add(new Vector3(x, y, z));
        }

        public void Clustering()
        {
            float   x   = max.X - min.X;
            float   y   = max.Y - min.Y;
            float   z   = max.Z - min.Z;
            div         = (int)Math.Ceiling((float)Math.Sqrt(Math.Sqrt(points.Count)));

                 if(x >= y && x >= z)   divu= x / div;
            else if(y >= x && y >= z)   divu= y / div;
            else if(z >= x && z >= y)   divu= z / div;

            clusters    = new Array3D<List<int>>
                (Math.Max(1, (int)(x / divu)),
                 Math.Max(1, (int)(y / divu)),
                 Math.Max(1, (int)(z / divu)));

            for(int i= 0, n= points.Count; i < n; ++i)
                AddCluster(i, points[i].X, points[i].Y, points[i].Z);
        }

        public int Clump(int a, int min, int max)
        {
            return a < min ? min : a > max ? max : a;
        }

        public int  IndexX(float x) { return Clump((int)((x-min.X) / divu), 0, clusters.xx-1); }
        public int  IndexY(float y) { return Clump((int)((y-min.Y) / divu), 0, clusters.yy-1); }
        public int  IndexZ(float z) { return Clump((int)((z-min.Z) / divu), 0, clusters.zz-1); }

        public void AddCluster(int i, float x, float y, float z)
        {
            int         a   = IndexX(x), b= IndexY(y), c= IndexZ(z);
            List<int>   l;
            
            try
            {
                l       = clusters.Get(a, b, c);

                if(l == null)
                    clusters.Set(a, b, c, l= new List<int>());

                l.Add(i);
            } catch(Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e);
            }
        }

        public int NearestIndex(Vector3 p)
        {
            int     limit   = 99;
            int     near    = -1;
            float   distsq  = float.MaxValue;
            int     a       = IndexX(p.X);
            int     b       = IndexY(p.Y);
            int     c       = IndexZ(p.Z);

            for(int i= 0; i <= limit; ++i)
            {
                for(int xx= a-i; xx <= a+i; ++xx)
                for(int yy= b-i; yy <= b+i; ++yy)
                for(int zz= c-i; zz <= c+i; ++zz)
                {
                    if(xx < 0 || xx >= clusters.xx) continue;
                    if(yy < 0 || yy >= clusters.yy) continue;
                    if(zz < 0 || zz >= clusters.zz) continue;

                    List<int>   l   = clusters.Get(xx, yy, zz);

                    if(l == null)
                        continue;

                    foreach(int j in l)
                    {
                        float   d   = Vector3.LengthSq(points[j] - p);
                        if(d >= distsq)
                            continue;

                        if(limit == 99)
                            limit   = i + 1;
                        distsq      = d;
                        near        = j;
                    }
                }
            }
            return near;
        }
    }
}
