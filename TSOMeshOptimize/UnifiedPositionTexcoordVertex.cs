using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using TDCG;

namespace TSOMeshOptimize
{
    class UnifiedPositionTexcoordVertex : Vertex, IComparable
    {
        public static float WeightEpsilon = float.Epsilon;

	public UnifiedPositionTexcoordVertex(UnifiedPositionVertex v, Dictionary<int, ushort> bone_idmap)
        {
            this.position = v.position;
            this.normal = v.normal;
            this.u = v.u;
            this.v = v.v;
            this.skin_weights = new SkinWeight[4];
            for (int i = 0; i < 4; i++)
            {
                if (v.skin_weights[i].weight < WeightEpsilon)
                    this.skin_weights[i] = new SkinWeight(0, 0.0f);
                else
                    this.skin_weights[i] = new SkinWeight(bone_idmap[v.skin_weights[i].bone_index], v.skin_weights[i].weight);
            }
            GenerateBoneIndices();
        }

        public int CompareTo(object obj)
        {
            UnifiedPositionTexcoordVertex v = obj as UnifiedPositionTexcoordVertex;
            if ((object)v == null)
                throw new ArgumentException("not a UnifiedPositionTexcoordVertex");
            int cmp = this.position.X.CompareTo(v.position.X);
            if (cmp == 0)
                cmp = this.position.Y.CompareTo(v.position.Y);
            if (cmp == 0)
                cmp = this.position.Z.CompareTo(v.position.Z);
            if (cmp == 0)
                cmp = this.u.CompareTo(v.u);
            if (cmp == 0)
                cmp = this.v.CompareTo(v.v);
            return cmp;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            UnifiedPositionTexcoordVertex v = obj as UnifiedPositionTexcoordVertex;
            if ((object)v == null)
                return false;
            return this.position == v.position && this.u == v.u && this.v == v.v;
        }

        public bool Equals(UnifiedPositionTexcoordVertex v)
        {
            if ((object)v == null)
                return false;
            return this.position == v.position && this.u == v.u && this.v == v.v;
        }

        public override int GetHashCode()
        {
            return position.GetHashCode() ^ u.GetHashCode() ^ v.GetHashCode();
        }
    }
}
