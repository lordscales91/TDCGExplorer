using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace Tso2MqoGui
{
    public partial struct Color3
    {
        public float    r, g, b;
        
        public Color3(float r, float g, float b)
        {
            this.r  = r;
            this.g  = g;
            this.b  = b;
        }

        public float    R   { get { return r; } set { r= value; } }
        public float    G   { get { return g; } set { g= value; } }
        public float    B   { get { return b; } set { b= value; } }

        public override string ToString()
        {
            return R+","+G+","+B;
        }
    }

    public partial struct Color3
    {
        public static Vector3 Parse(string[] t, int begin)
        {
            return new Vector3(
                float.Parse(t[begin+0]),
                float.Parse(t[begin+1]),
                float.Parse(t[begin+2]));
        }
    }

    public partial struct Point2
    {
        public static Vector2 Parse(string[] t, int begin)
        {
            return new Vector2(
                float.Parse(t[begin+0]),
                float.Parse(t[begin+1]));
        }
    }

    public partial struct Point3
    {
        public static Vector3 Parse(string[] t, int begin)
        {
            return new Vector3(
                float.Parse(t[begin+0]),
                float.Parse(t[begin+1]),
                float.Parse(t[begin+2]));
        }
    }
}
