using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using TDCG;

namespace tso2mqo
{
    public class UnifiedPositionVertex : Vertex
    {
        public UnifiedPositionVertex(Vertex a)
        {
            this.position = a.position;
            this.normal = a.normal;
            this.u = a.u;
            this.v = a.v;
        }
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            UnifiedPositionVertex v = obj as UnifiedPositionVertex;
            if ((object)v == null)
                return false;
            return this.position == v.position;
        }
        public bool Equals(UnifiedPositionVertex v)
        {
            if ((object)v == null)
                return false;
            return this.position == v.position;
        }
        public override int GetHashCode()
        {
            return position.GetHashCode();
        }
    }
}
