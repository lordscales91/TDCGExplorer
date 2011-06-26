using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

using jp.nyatla.nymmd.cs.types;
using jp.nyatla.nymmd.cs.core;
using jp.nyatla.nymmd.cs.struct_type;
using jp.nyatla.nymmd.cs.struct_type.pmd;

namespace Tso2Pmd
{
    public class PmdUtils
    {
        // 名前からボーンIDを得る
        public static int GetBoneIDFromName(PmdFileData pmd, string name)
        {
            for (int i = 0; i < pmd.pmd_bone.Length; i++)
            {
                if (pmd.pmd_bone[i].szName == name) return i;
            }

            return -1;
        }

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
