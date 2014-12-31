using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using TDCG;

namespace TSOSmooth
{
    /// <summary>
    /// 位置が一意な頂点
    /// </summary>
    public class UnifiedPositionVertex : Vertex, IComparable
    {
        /// <summary>
        /// シェーダ設定番号
        /// </summary>
        public int spec;

        /// <summary>
        /// 位置が一意な頂点を生成します。
        /// </summary>
        /// <param name="a">頂点</param>
        /// <param name="bone_indices">ボーン参照配列</param>
        /// <param name="spec">シェーダ設定番号</param>
        public UnifiedPositionVertex(Vertex a, int[] bone_indices, int spec)
        {
            this.position = a.position;
            this.normal = a.normal;
            this.u = a.u;
            this.v = a.v;
            this.skin_weights = new SkinWeight[4];
            for (int i = 0; i < 4; i++)
            {
                skin_weights[i] = new SkinWeight(bone_indices[a.skin_weights[i].bone_index], a.skin_weights[i].weight);
            }
            this.spec = spec;
        }

        /// <summary>
        /// 比較関数
        /// </summary>
        /// <param name="obj">object</param>
        /// <returns></returns>
        public int CompareTo(object obj)
        {
            UnifiedPositionVertex v = obj as UnifiedPositionVertex;
            if ((object)v == null)
                throw new ArgumentException("not a UnifiedPositionVertex");
            int cmp = this.position.X.CompareTo(v.position.X);
            if (cmp == 0)
                cmp = this.position.Y.CompareTo(v.position.Y);
            if (cmp == 0)
                cmp = this.position.Z.CompareTo(v.position.Z);
            return cmp;
        }

        /// <summary>
        /// 等値関数
        /// </summary>
        /// <param name="obj">object</param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            UnifiedPositionVertex v = obj as UnifiedPositionVertex;
            if ((object)v == null)
                return false;
            return this.position == v.position;
        }

        /// <summary>
        /// 等値関数
        /// </summary>
        /// <param name="v">v</param>
        /// <returns></returns>
        public bool Equals(UnifiedPositionVertex v)
        {
            if ((object)v == null)
                return false;
            return this.position == v.position;
        }

        /// <summary>
        /// ハッシュ関数
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return position.GetHashCode();
        }
    }
}
