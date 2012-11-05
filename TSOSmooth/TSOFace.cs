using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TSOSmooth
{
    /// 面頂点
    public struct FaceVertex
    {
        /// 頂点参照
        public ushort i;
        /// テクスチャU座標
        public float u;
        /// テクスチャV座標
        public float v;
    }

    /// <summary>
    /// 面
    /// </summary>
    public class TSOFace
    {
        /// 面頂点a
        public readonly FaceVertex a;
        /// 面頂点b
        public readonly FaceVertex b;
        /// 面頂点c
        public readonly FaceVertex c;

        /// <summary>
        /// シェーダ設定番号
        /// </summary>
        public readonly int spec;

        /// <summary>
        /// 面を生成します。
        /// </summary>
        /// <param name="a">面頂点a</param>
        /// <param name="b">面頂点b</param>
        /// <param name="c">面頂点c</param>
        /// <param name="spec">シェーダ設定番号</param>
        public TSOFace(FaceVertex a, FaceVertex b, FaceVertex c, int spec)
        {
            this.a = a;
            this.b = b;
            this.c = c;

            this.spec = spec;
        }

        public IEnumerable<FaceVertex> vertices
        {
            get {
                yield return a;
                yield return b;
                yield return c;
            }
        }

        public IEnumerable<ushort> vert_indices
        {
            get {
                yield return a.i;
                yield return b.i;
                yield return c.i;
            }
        }
    }
}
