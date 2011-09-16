using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace tso2mqo
{
    public class PointCluster
    {
        LinerOctreeManager octree = new LinerOctreeManager();
        public List<Vector3> points;
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
            if (p.X < min.X) min.X = p.X; else if (p.X > max.X) max.X = p.X;
            if (p.Y < min.Y) min.Y = p.Y; else if (p.Y > max.Y) max.Y = p.Y;
            if (p.Z < min.Z) min.Z = p.Z; else if (p.Z > max.Z) max.Z = p.Z;
        }

        public void Clustering()
        {
            octree.Init(5, ref min, ref max);
            for (int i = 0; i < points.Count; i++)
            {
                Vector3 p = points[i];
                if (!octree.Regist(ref p, ref p, new LinkedListNode<int>(i)))
                {
		    Console.WriteLine("failed to regist node");
		}
            }
        }

        public int NearestIndex(Vector3 p)
        {
            int near = -1;
            float nearest_lensq = float.MaxValue;
            UInt32 num = octree.GetMortonNumber(ref p, ref p);
            if (num < octree.ncell)
            {
                Cell cell = octree.cells[num];
                //Console.WriteLine("cell {0} {1}", num, cell.Count);
                foreach (int i in cell)
                {
                    float lensq = Vector3.LengthSq(points[i] - p);
                    if (lensq < nearest_lensq)
                    {
                        nearest_lensq = lensq;
                        near = i;
                    }
                }
            }
            else
                Console.WriteLine("near index not found");
            return near;
        }
    }
}
