//#define SEARCH_DEBUG
using System;
using System.Collections.Generic;
using System.Text;

namespace Tso2MqoGui
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
        public List<Point3>         points;
        public int                  div;
        public float                divu;
        public Array3D<List<int>>   clusters;
        public Point3               min= new Point3(float.MaxValue, float.MaxValue, float.MaxValue);
        public Point3               max= new Point3(float.MinValue, float.MinValue, float.MinValue);

        public PointCluster(int n)
        {
            points  = new List<Point3>(n);
        }

        public Point3 GetPoint(int i)
        {
            return points[i];
        }

        public void Add(Point3 p)
        {
            points.Add(p);
            if(p.x < min.x) min.x= p.x; else if(p.x > max.x) max.x= p.x;
            if(p.y < min.y) min.y= p.y; else if(p.y > max.y) max.y= p.y;
            if(p.z < min.z) min.z= p.z; else if(p.z > max.z) max.z= p.z;
        }

        public void Add(float x, float y, float z)
        {
            Add(new Point3(x, y, z));
        }

        public void Clustering()
        {
            float   x   = max.x - min.x;
            float   y   = max.y - min.y;
            float   z   = max.z - min.z;
            div         = (int)Math.Ceiling((float)Math.Sqrt(Math.Sqrt(points.Count)));

                 if(x >= y && x >= z)   divu= x / div;
            else if(y >= x && y >= z)   divu= y / div;
            else if(z >= x && z >= y)   divu= z / div;

            clusters    = new Array3D<List<int>>
                (Math.Max(1, (int)(x / divu)),
                 Math.Max(1, (int)(y / divu)),
                 Math.Max(1, (int)(z / divu)));

            for(int i= 0, n= points.Count; i < n; ++i)
                AddCluster(i, points[i].x, points[i].y, points[i].z);
        }

        public int Clump(int a, int min, int max)
        {
            return a < min ? min : a > max ? max : a;
        }

        public int  IndexX(float x) { return Clump((int)((x-min.x) / divu), 0, clusters.xx-1); }
        public int  IndexY(float y) { return Clump((int)((y-min.y) / divu), 0, clusters.yy-1); }
        public int  IndexZ(float z) { return Clump((int)((z-min.z) / divu), 0, clusters.zz-1); }

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

        public int NearestIndex(float x, float y, float z)
        {
#if SEARCH_DEBUG
            int     dbgcount= 0;    
#endif
            int     limit   = 99;
            int     near    = -1;
            float   distsq  = float.MaxValue;
            int     a       = IndexX(x);
            int     b       = IndexY(y);
            int     c       = IndexZ(z);

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
                        Point3  p   = points[j];
                        p.x         -=x;
                        p.y         -=y;
                        p.z         -=z;
                        float   d   = p.x*p.x + p.y*p.y + p.z*p.z;
#if SEARCH_DEBUG
                        ++dbgcount;
#endif
                        if(d >= distsq)
                            continue;

                        if(limit == 99)
                            limit   = i + 1;
                        distsq      = d;
                        near        = j;
                    }
                }
            }
#if SEARCH_DEBUG
            System.Diagnostics.Debug.WriteLine(string.Format(
                "dbgcount:{0} index:{1} distance:{2}", dbgcount, near, distsq));
#endif
            return near;
        }
    }
}
