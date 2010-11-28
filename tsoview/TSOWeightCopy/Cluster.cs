using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using TDCG;

namespace TSOWeightCopy
{
    /// <summary>
    /// 複写方向
    /// </summary>
    public enum CopyDirection
    {
        /// <summary>
        /// 左から右
        /// </summary>
        LtoR,
        /// <summary>
        /// 右から左
        /// </summary>
        RtoL
    };

    /// 頂点探索クラスタ
    public class Cluster
    {
        /// <summary>
        /// 複写方向
        /// </summary>
        public CopyDirection dir = CopyDirection.LtoR;

        Vector3 min;
        Vector3 max;
        UniqueCell[, ,] cells;
        int xlen;
        int ylen;
        int zlen;

        /// <summary>
        /// 頂点探索クラスタを生成します。
        /// </summary>
        /// <param name="min">頂点の最小位置</param>
        /// <param name="max">頂点の最大位置</param>
        public Cluster(Vector3 min, Vector3 max)
        {
            this.min = min;
            this.max = max;
            this.xlen = (int)Math.Floor(max.X + 0.5f) - (int)Math.Floor(min.X + 0.5f) + 1;
            this.ylen = (int)Math.Floor(max.Y + 0.5f) - (int)Math.Floor(min.Y + 0.5f) + 1;
            this.zlen = (int)Math.Floor(max.Z + 0.5f) - (int)Math.Floor(min.Z + 0.5f) + 1;
            this.cells = new UniqueCell[xlen, ylen, zlen];
        }

        /// 指定x座標にある頂点を格納するセルのx値を得ます。
        public int GetX(float x)
        {
            return (int)Math.Floor(x + 0.5f) - (int)Math.Floor(min.X + 0.5f);
        }

        /// 指定y座標にある頂点を格納するセルのy値を得ます。
        public int GetY(float y)
        {
            return (int)Math.Floor(y + 0.5f) - (int)Math.Floor(min.Y + 0.5f);
        }

        /// 指定z座標にある頂点を格納するセルのz値を得ます。
        public int GetZ(float z)
        {
            return (int)Math.Floor(z + 0.5f) - (int)Math.Floor(min.Z + 0.5f);
        }

        /// 指定座標にあるセルを得ます。
        public UniqueCell GetCell(int x, int y, int z)
        {
            UniqueCell cell = null;
            try
            {
                if (cells[x, y, z] == null)
                    cells[x, y, z] = new UniqueCell(this, x, y, z, x == GetX(0.0f));
                cell = cells[x, y, z];
            }
            catch (IndexOutOfRangeException)
            {
            }
            return cell;
        }

        /// 頂点を追加します。
        public void Push(Vertex a, TSOSubMesh sub)
        {
            int x = GetX(a.position.X);
            int y = GetY(a.position.Y);
            int z = GetZ(a.position.Z);
            UniqueCell cell = GetCell(x, y, z);
            cell.Push(a, sub);
        }

        /// <summary>
        /// 文字列表現を出力します。
        /// </summary>
        public void Dump()
        {
            foreach (UniqueCell cell in cells)
                if (cell != null)
                    cell.Dump();
        }

        /// 同一視頂点リスト
        public List<UniqueVertex> vertices
        {
            get
            {
                List<UniqueVertex> ary = new List<UniqueVertex>();
                foreach (UniqueCell cell in cells)
                    if (cell != null)
                        ary.AddRange(cell.vertices);
                return ary;
            }
        }

        /// 対称位置にあるセルのx値を得ます。
        public int GetOppositeX(int x)
        {
            int xend = xlen - 1;
            return xend - x;
        }

        /// 対称位置にあるセルを保持します。
        public void AssignOppositeCells()
        {
            foreach (UniqueCell cell in cells)
                if (cell != null)
                {
                    int x = GetOppositeX(cell.X);
                    int y = cell.Y;
                    int z = cell.Z;
                    cell.opposite_cell = GetCell(x, y, z);
                }
        }

        /// 対称位置にある同一視頂点を保持します。
        public void AssignOppositeVertices()
        {
            foreach (UniqueCell cell in cells)
                if (cell != null)
                    cell.AssignOppositeVertices();
        }
        
        /// 対称位置にある同一視頂点のスキンウェイトを複写します。
        public void CopyOppositeWeights()
        {
            int x = GetX(0.0f);
            foreach (UniqueCell cell in cells)
                if (cell != null)
                {
                    if (dir == CopyDirection.LtoR && cell.X > x)
                        continue;
                    if (dir == CopyDirection.RtoL && cell.X < x)
                        continue;

                    cell.CopyOppositeWeights();
                }
        }
    }
}
