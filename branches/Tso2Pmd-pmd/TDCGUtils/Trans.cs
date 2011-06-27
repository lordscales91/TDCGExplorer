using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace TDCGUtils
{
    public class Trans
    {
        /// <summary>
        /// Tso -> Pmdにおいて、マイナスをつけるなどの変換を踏まえた上でコピー
        /// </summary>
        public static Vector3 CopyPos(Vector3 pos)
        {
            return new Vector3(pos.X, pos.Y, -pos.Z);
        }

        /// <summary>
        /// Tso -> Pmdにおいて、マイナスをつけるなどの変換を踏まえた上でコピー
        /// </summary>
        public static Vector3 CopyMat2Pos(Matrix mat)
        {
            return new Vector3(mat.M41, mat.M42, -mat.M43);
        }
    }
}
