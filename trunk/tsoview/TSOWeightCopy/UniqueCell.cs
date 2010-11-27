using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using TDCG;

namespace TSOWeightCopy
{
    /// 頂点探索クラスタに格納するセル
    public class UniqueCell
    {
        public static float LengthSq(Vector3 a, Vector3 b)
        {
            float dx = b.X - a.X;
            float dy = b.Y - a.Y;
            float dz = b.Z - a.Z;
            return dx * dx + dy * dy + dz * dz;
        }

        Cluster cluster;
        int x;
        int y;
        int z;
        bool contains_zero_x;

        public int X { get { return x; } }
        public int Y { get { return y; } }
        public int Z { get { return z; } }
        public bool ContainsZeroX { get { return contains_zero_x; } }

        public List<UniqueVertex> vertices;
        public UniqueCell opposite_cell;

        public UniqueCell(Cluster cluster, int x, int y, int z, bool contains_zero_x)
        {
            this.cluster = cluster;
            this.x = x;
            this.y = y;
            this.z = z;
            this.contains_zero_x = contains_zero_x;
            this.vertices = new List<UniqueVertex>();
            this.opposite_cell = null;
        }

        /// 頂点を追加します。
        public void Push(Vertex a, TSOSubMesh sub)
        {
            bool found = false;
            foreach (UniqueVertex v in vertices)
            {
                if (LengthSq(a.position, v.position) < float.Epsilon)
                {
                    v.Push(a, sub);
                    found = true;
                    break;
                }
            }
            if (!found)
                vertices.Add(new UniqueVertex(a, sub));
        }

        public override string ToString()
        {
            return string.Format("x:{0} y:{1} z:{2} #v:{3}", x, y, z, vertices.Count);
        }

        public void Dump()
        {
            Console.WriteLine(this);
            if (vertices.Count > 0)
            {
                UniqueVertex v = vertices[0];
                v.Dump();
            }
        }

        /// 指定位置に最も近い同一視頂点を見つけます。
        public UniqueVertex FindVertexAt(Vector3 position)
        {
            float min_len_sq = 10.0f;
            UniqueVertex found = null;
            foreach (UniqueVertex v in vertices)
            {
                float len_sq = LengthSq(position, v.position);
                if (min_len_sq > len_sq)
                {
                    min_len_sq = len_sq;
                    found = v;
                }
            }
            return found;
        }

        /// 対称位置にある同一視頂点を保持します。
        public void AssignOppositeVertices()
        {
            if (contains_zero_x)
            {
                foreach (UniqueVertex v in vertices)
                    v.opposite_vertex = Math.Abs(v.position.X) < 1.0e-4f ? v : opposite_cell.FindVertexAt(v.GetOppositePosition());
            }
            else
            {
                foreach (UniqueVertex v in vertices)
                    v.opposite_vertex = opposite_cell.FindVertexAt(v.GetOppositePosition());
            }
        }

        /// 対称位置にある同一視頂点のスキンウェイトを複写します。
        public void CopyOppositeWeights()
        {
            if (contains_zero_x)
            {
                foreach (UniqueVertex v in vertices)
                {
                    if (cluster.dir == CopyDirection.LtoR && v.position.X > -1.0e-4f)
                        continue;
                    if (cluster.dir == CopyDirection.RtoL && v.position.X < +1.0e-4f)
                        continue;

                    v.CopyOppositeWeights();
                }
            }
            else
            {
                foreach (UniqueVertex v in vertices)
                    v.CopyOppositeWeights();
            }
        }
    }
}
