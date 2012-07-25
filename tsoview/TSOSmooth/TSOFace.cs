using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TSOSmooth
{
    /// <summary>
    /// 面
    /// </summary>
    public class TSOFace
    {
        /// <summary>
        /// 頂点a
        /// </summary>
        public readonly ushort a;
        /// <summary>
        /// 頂点b
        /// </summary>
        public readonly ushort b;
        /// <summary>
        /// 頂点c
        /// </summary>
        public readonly ushort c;

        /// テクスチャU座標a
        public readonly float ua;
        /// テクスチャV座標a
        public readonly float va;
        /// テクスチャU座標b
        public readonly float ub;
        /// テクスチャV座標b
        public readonly float vb;
        /// テクスチャU座標c
        public readonly float uc;
        /// テクスチャV座標c
        public readonly float vc;

        /// <summary>
        /// シェーダ設定番号
        /// </summary>
        public readonly int spec;

        /// <summary>
        /// 面を生成します。
        /// </summary>
        /// <param name="a">頂点a</param>
        /// <param name="b">頂点b</param>
        /// <param name="c">頂点c</param>
        /// <param name="ua">テクスチャU座標a</param>
        /// <param name="va">テクスチャV座標a</param>
        /// <param name="ub">テクスチャU座標b</param>
        /// <param name="vb">テクスチャV座標b</param>
        /// <param name="uc">テクスチャU座標c</param>
        /// <param name="vc">テクスチャV座標c</param>
        /// <param name="spec">シェーダ設定番号</param>
        public TSOFace(ushort a, ushort b, ushort c, float ua, float va, float ub, float vb, float uc, float vc, int spec)
        {
            this.a = a;
            this.b = b;
            this.c = c;

            this.ua = ua;
            this.va = va;
            this.ub = ub;
            this.vb = vb;
            this.uc = uc;
            this.vc = vc;

            this.spec = spec;
        }
    }
}
