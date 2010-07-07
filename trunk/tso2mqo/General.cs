using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace Tso2MqoGui
{
    public partial struct Point2
    {
        public float x, y;

        public Point2(float x, float y)
        {
            this.x  = x;
            this.y  = y;
        }

        public float    X   { get { return x; } set { x= value; } }
        public float    Y   { get { return y; } set { y= value; } }

        public override string ToString()
        {
            return X+","+Y;
        }
    }

    public partial struct Point3
    {
        public float x, y, z;

        public Point3(float x, float y, float z)
        {
            this.x  = x;
            this.y  = y;
            this.z  = z;
        }

        public float    X   { get { return x; } set { x= value; } }
        public float    Y   { get { return y; } set { y= value; } }
        public float    Z   { get { return z; } set { z= value; } }

        public static Point3 operator+(Point3 a, Point3 b)
        {
            return new Point3(a.x+b.x, a.y+b.y, a.z+b.z);
        }

        public static Point3 operator-(Point3 a, Point3 b)
        {
            return new Point3(a.x-b.x, a.y-b.y, a.z-b.z);
        }

        public override string ToString()
        {
            return X+","+Y+","+Z;
        }

        public static Point3    Cross(Point3 p, Point3 q)
        {
            return new Point3(
                p.y*q.z - p.z*q.y,
                p.z*q.x - p.x*q.z,
                p.x*q.y - p.y*q.x);
        }

        public static Point3    Normalize(Point3 p)
        {
            float   d   = p.x*p.x + p.y*p.y + p.z*p.z;

            if(d < 0.00001f)
                return p;

            d           = (float)(1 / (Math.Sqrt(d)));
            return new Point3(p.x*d, p.y*d, p.z*d);
        }
    }

    public partial struct Point4
    {
        public float x, y, z, w;

        public Point4(float x, float y, float z, float w)
        {
            this.x  = x;
            this.y  = y;
            this.z  = z;
            this.w  = w;
        }

        public float    X   { get { return x; } set { x= value; } }
        public float    Y   { get { return y; } set { y= value; } }
        public float    Z   { get { return z; } set { z= value; } }
        public float    W   { get { return w; } set { w= value; } }

        public override string ToString()
        {
            return X+","+Y+","+Z+","+W;
        }
    }

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
        public static Color3    Parse(string[] t, int  begin)
        {
            return new Color3(
                float.Parse(t[begin+0]),
                float.Parse(t[begin+1]),
                float.Parse(t[begin+2]));
        }
    }

    public partial struct Point2
    {
        public static Point2    Parse(string[] t, int  begin)
        {
            return new Point2(
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
