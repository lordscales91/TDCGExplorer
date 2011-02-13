using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using TDCG;

namespace TSOMeshOptimize
{
    /// <summary>
    /// 位置とシェーダ設定の組が一意な頂点
    /// </summary>
    public class UnifiedPositionSpecVertex : Vertex, IComparable
    {
        /// <summary>
        /// シェーダ設定番号
        /// </summary>
        public int spec;

        /// <summary>
        /// 位置とシェーダ設定の組が一意な頂点を生成します。
        /// </summary>
        /// <param name="a">頂点</param>
        /// <param name="sub">頂点を含むサブメッシュ</param>
        public UnifiedPositionSpecVertex(Vertex a, TSOSubMesh sub)
        {
            this.position = a.position;
            this.normal = a.normal;
            this.u = a.u;
            this.v = a.v;
            this.skin_weights = new SkinWeight[4];
            for (int i = 0; i < 4; i++)
            {
                skin_weights[i] = new SkinWeight(sub.bone_indices[a.skin_weights[i].bone_index], a.skin_weights[i].weight);
            }
            this.spec = sub.spec;
        }

        /// <summary>
        /// 比較関数
        /// </summary>
        /// <param name="obj">object</param>
        /// <returns></returns>
        public int CompareTo(object obj)
        {
            UnifiedPositionSpecVertex v = obj as UnifiedPositionSpecVertex;
            if ((object)v == null)
                throw new ArgumentException("not a UnifiedPositionSpecVertex");
            int cmp = this.position.X.CompareTo(v.position.X);
            if (cmp == 0)
                cmp = this.position.Y.CompareTo(v.position.Y);
            if (cmp == 0)
                cmp = this.position.Z.CompareTo(v.position.Z);
            if (cmp == 0)
                cmp = this.spec.CompareTo(v.spec);
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
            UnifiedPositionSpecVertex v = obj as UnifiedPositionSpecVertex;
            if ((object)v == null)
                return false;
            return this.position == v.position && this.spec == v.spec;
        }

        /// <summary>
        /// 等値関数
        /// </summary>
        /// <param name="v">v</param>
        /// <returns></returns>
        public bool Equals(UnifiedPositionSpecVertex v)
        {
            if ((object)v == null)
                return false;
            return this.position == v.position && this.spec == v.spec;
        }

        /// <summary>
        /// ハッシュ関数
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return position.GetHashCode() ^ spec.GetHashCode();
        }
    }
}
