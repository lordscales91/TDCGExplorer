using System;
using System.Collections.Generic;
using System.Diagnostics;
using SharpDX;
using SharpDX.Direct3D9;
using TDCG;

namespace TSOWeightCopy
{
    /// 頂点探索クラスタに格納するセル
    public class UniqueCell
    {
        /// <summary>
        /// 指定位置間の距離の平方を返します。
        /// </summary>
        /// <param name="a">位置a</param>
        /// <param name="b">位置b</param>
        /// <returns></returns>
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

        /// <summary>
        /// x値
        /// </summary>
        public int X { get { return x; } }
        /// <summary>
        /// y値
        /// </summary>
        public int Y { get { return y; } }
        /// <summary>
        /// z値
        /// </summary>
        public int Z { get { return z; } }
        /// <summary>
        /// x軸を含むか
        /// </summary>
        public bool ContainsZeroX { get { return contains_zero_x; } }

        /// <summary>
        /// 同一視頂点リスト
        /// </summary>
        public List<UniqueVertex> vertices;

        /// <summary>
        /// 対称位置にあるセル
        /// </summary>
        public UniqueCell opposite_cell;

        /// <summary>
        /// セルを生成します。
        /// </summary>
        /// <param name="cluster">クラスタ</param>
        /// <param name="x">x値</param>
        /// <param name="y">y値</param>
        /// <param name="z">z値</param>
        /// <param name="contains_zero_x">x軸を含むか</param>
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

        /// <summary>
        /// 文字列表現を得ます。
        /// </summary>
        /// <returns>文字列表現</returns>
        public override string ToString()
        {
            return string.Format("x:{0} y:{1} z:{2} #v:{3}", x, y, z, vertices.Count);
        }

        /// <summary>
        /// 文字列表現を出力します。
        /// </summary>
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
        public UniqueVertex FindVertexAt(Vector3 position, out float min_len_sq)
        {
            min_len_sq = 10.0f;
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

        /// <summary>
        /// 指定位置に最も近い同一視頂点を見つけます。
        /// </summary>
        /// <param name="position">位置</param>
        /// <param name="cell">探索対象セル</param>
        /// <param name="found">前回見つけた頂点</param>
        /// <param name="min_len_sq">前回見つけた頂点までの距離の平方</param>
        /// <returns></returns>
        public static UniqueVertex FindVertex(Vector3 position, UniqueCell cell, UniqueVertex found, ref float min_len_sq)
        {
            if (cell != null)
            {
                float len_sq;
                UniqueVertex v = cell.FindVertexAt(position, out len_sq);
                if (min_len_sq > len_sq)
                {
                    min_len_sq = len_sq;
                    found = v;
                }
            }
            return found;
        }

        /// <summary>
        /// 指定値だけ移動した位置にあるセルを得ます。
        /// </summary>
        /// <param name="dx">x値</param>
        /// <param name="dy">y値</param>
        /// <param name="dz">z値</param>
        /// <returns></returns>
        public UniqueCell GetNeighbor(int dx, int dy, int dz)
        {
            return cluster.GetCell(x + dx, y + dy, z + dz);
        }

        int GetSign(float x)
        {
            float o = (float)Math.Floor(x + 0.5f);
            float d = x - o;
            return Math.Abs(d) < float.Epsilon ? 0 : (d < 0.0f ? -1 : +1);
        }

        void FindOppositeVertex(UniqueVertex v)
        {
            Vector3 opp_p = v.GetOppositePosition();
            float x = opp_p.X;
            float y = opp_p.Y;
            float z = opp_p.Z;

            int dx = GetSign(x);
            int dy = GetSign(y);
            int dz = GetSign(z);

            UniqueVertex opp_v = null;
            float min_len_sq = 10.0f;

            opp_v = FindVertex(opp_p, opposite_cell, opp_v, ref min_len_sq);
            if (dx != 0)
                opp_v = FindVertex(opp_p, opposite_cell.GetNeighbor(dx, 0, 0), opp_v, ref min_len_sq);
            if (dy != 0)
                opp_v = FindVertex(opp_p, opposite_cell.GetNeighbor(0, dy, 0), opp_v, ref min_len_sq);
            if (dz != 0)
                opp_v = FindVertex(opp_p, opposite_cell.GetNeighbor(0, 0, dz), opp_v, ref min_len_sq);
            if (dx != 0 && dy != 0)
                opp_v = FindVertex(opp_p, opposite_cell.GetNeighbor(dx, dy, 0), opp_v, ref min_len_sq);
            if (dy != 0 && dz != 0)
                opp_v = FindVertex(opp_p, opposite_cell.GetNeighbor(0, dy, dz), opp_v, ref min_len_sq);
            if (dz != 0 && dx != 0)
                opp_v = FindVertex(opp_p, opposite_cell.GetNeighbor(dx, 0, dz), opp_v, ref min_len_sq);
            if (dx != 0 && dy != 0 && dz != 0)
                opp_v = FindVertex(opp_p, opposite_cell.GetNeighbor(dx, dy, dz), opp_v, ref min_len_sq);

            v.opposite_vertex = opp_v;
        }

        /// 対称位置にある同一視頂点を保持します。
        public void AssignOppositeVertices()
        {
            if (contains_zero_x)
            {
                foreach (UniqueVertex v in vertices)
                {
                    if (Math.Abs(v.position.X) < 1.0e-4f)
                    {
                        v.opposite_vertex = v;
                        continue;
                    }
                    FindOppositeVertex(v);
                }
            }
            else
            {
                foreach (UniqueVertex v in vertices)
                    FindOppositeVertex(v);
            }
        }

        /// 対称位置にある同一視頂点のスキンウェイトを複写します。
        public void CopyOppositeWeights()
        {
            if (contains_zero_x)
            {
                foreach (UniqueVertex v in vertices)
                {
                    if (Math.Abs(v.position.X) < 1.0e-4f)
                        continue;
                    if (cluster.dir == CopyDirection.LtoR && v.position.X < 0.0f)
                        continue;
                    if (cluster.dir == CopyDirection.RtoL && v.position.X > 0.0f)
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
