using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TSOMeshOptimize
{
    /// <summary>
    /// 面
    /// </summary>
    public class TSOFace
    {
        /// <summary>
        /// 頂点a
        /// </summary>
        public readonly UnifiedPositionSpecVertex a;
        /// <summary>
        /// 頂点b
        /// </summary>
        public readonly UnifiedPositionSpecVertex b;
        /// <summary>
        /// 頂点c
        /// </summary>
        public readonly UnifiedPositionSpecVertex c;
        /// <summary>
        /// シェーダ設定番号
        /// </summary>
        public readonly int spec;
        /// <summary>
        /// 頂点配列
        /// </summary>
        public readonly UnifiedPositionSpecVertex[] vertices;

        /// <summary>
        /// 面を生成します。
        /// </summary>
        /// <param name="a">頂点a</param>
        /// <param name="b">頂点b</param>
        /// <param name="c">頂点c</param>
        public TSOFace(UnifiedPositionSpecVertex a, UnifiedPositionSpecVertex b, UnifiedPositionSpecVertex c)
        {
            this.a = a;
            this.b = b;
            this.c = c;
            this.spec = a.spec;
            vertices = new UnifiedPositionSpecVertex[3];
            vertices[0] = a;
            vertices[1] = b;
            vertices[2] = c;
        }
    }
}
