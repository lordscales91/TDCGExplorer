using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

using TDCG;
using jp.nyatla.nymmd.cs.types;

namespace TDCGUtils
{
    public class Trans
    {
        /// <summary>
        /// Tso -> Pmdにおいて、マイナスをつけるなどの変換を踏まえた上でコピー
        /// </summary>
        public static MmdVector3 CopyPos(Vector3 pos)
        {
            return new MmdVector3(pos.X, pos.Y, -pos.Z);
        }

        /// <summary>
        /// Tso -> Pmdにおいて、マイナスをつけるなどの変換を踏まえた上でコピー
        /// </summary>
        public static MmdVector3 CopyMat2Pos(Matrix mat)
        {
            return new MmdVector3(mat.M41, mat.M42, -mat.M43);
        }
    }
}
