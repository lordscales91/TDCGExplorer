using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace TDCG
{

public class TPOFile
{
    internal TPONode[] nodes;

    internal float ratio = 0.8f;

    internal Dictionary<string, TPONode> nodemap;

    internal TMOFile tmo = null;
    public TMOFile Tmo
    {
        get {
            return tmo;
        }
        set {
            tmo = value;

            int node_count = tmo.nodes.Length;
            nodes = new TPONode[node_count];
            nodemap = new Dictionary<string, TPONode>();

            for (int i = 0; i < node_count; i++)
            {
                nodes[i] = new TPONode();
                nodes[i].id = i;
                nodes[i].name = tmo.nodes[i].name;
                nodes[i].sname = nodes[i].name.Substring(nodes[i].name.LastIndexOf('|')+1);
                nodemap.Add(nodes[i].name, nodes[i]);

                //Console.WriteLine(i + ": " + nodes[i].sname);
            }

            for (int i = 0; i < node_count; i++)
            {
                int index = nodes[i].name.LastIndexOf('|');
                if (index <= 0)
                    continue;
                string pname = nodes[i].name.Substring(0, index);
                nodes[i].parent = nodemap[pname];
                nodes[i].parent.children.Add(nodes[i]);
            }
        }
    }

    public TPOFile()
    {
    }

    public TPONode FindNodeByName(string name)
    {
        return nodemap[name];
    }

    public void Transform()
    {
        foreach (TPONode node in nodes)
            Transform(node);
    }

    public void Transform(TPONode node)
    {
        for (int i = 0; i < tmo.frames.Length; i++)
        {
            TMOMat mat = tmo.GetTMOMat(node.name, i);

            Matrix m = mat.m;
            Vector3 scaling = Vector3.Empty;
            Vector3 t = TMOMat.DecomposeMatrix(ref m, out scaling);

            scaling.X *= (float)Math.Pow(node.Scaling.X, ratio);
            scaling.Y *= (float)Math.Pow(node.Scaling.Y, ratio);
            scaling.Z *= (float)Math.Pow(node.Scaling.Z, ratio);
            m *= node.RotationMatrix(m, ratio);
            t += node.Translation * ratio;

            mat.m = Matrix.Scaling(scaling) * m * Matrix.Translation(t);
        }
    }
}

public class TPONode
{
    internal int id;
    internal string name;

    internal Vector3 scaling = new Vector3(1,1,1);
    public Vector3 Scaling
    {
        get { return scaling; } set { scaling = value; }
    }
    /*
    internal Quaternion rotation = Quaternion.Identity;
    public Quaternion Rotation
    {
        get { return rotation; } set { rotation = value; }
    }
    */
    internal Vector3 rot = Vector3.Empty;
    internal Vector3 translation = Vector3.Empty;
    public Vector3 Translation
    {
        get { return translation; } set { translation = value; }
    }

    internal string sname;
    internal List<TPONode> children = new List<TPONode>();
    internal TPONode parent;

    public int ID { get { return id; } }
    public string Name { get { return name; } }
    public string ShortName { get { return sname; } }

    public TPONode()
    {
    }

    public Matrix ScalingMatrix
    {
        get { return Matrix.Scaling(scaling); }
    }
    /*
    public Matrix RotationMatrix
    {
        get { return Matrix.RotationQuaternion(rotation); }
    }
    */
    public Matrix RotationMatrix(Matrix m)
    {
        Vector3 vx = new Vector3(m.M11, m.M12, m.M13);
        Quaternion qx = Quaternion.RotationAxis(vx, rot.X);

        Vector3 vy = new Vector3(m.M21, m.M22, m.M23);
        Quaternion qy = Quaternion.RotationAxis(vy, rot.Y);

        Vector3 vz = new Vector3(m.M31, m.M32, m.M33);
        Quaternion qz = Quaternion.RotationAxis(vz, rot.Z);

        return Matrix.RotationQuaternion(qy * qx * qz);
    }

    public Matrix RotationMatrix(Matrix m, float ratio)
    {
        Vector3 vx = new Vector3(m.M11, m.M12, m.M13);
        Quaternion qx = Quaternion.RotationAxis(vx, rot.X * ratio);

        Vector3 vy = new Vector3(m.M21, m.M22, m.M23);
        Quaternion qy = Quaternion.RotationAxis(vy, rot.Y * ratio);

        Vector3 vz = new Vector3(m.M31, m.M32, m.M33);
        Quaternion qz = Quaternion.RotationAxis(vz, rot.Z * ratio);

        return Matrix.RotationQuaternion(qy * qx * qz);
    }
    public Matrix TranslationMatrix
    {
        get { return Matrix.Translation(translation); }
    }
    /*
    public Matrix TransformationMatrix
    {
        get { return ScalingMatrix * RotationMatrix * TransformationMatrix; }
    }
    */

    public void Scale(float x, float y, float z)
    {
        scaling.X *= x;
        scaling.Y *= y;
        scaling.Z *= z;
    }

    public void Scale1(float x, float y, float z)
    {
        Scale(x, y, z);
        foreach (TPONode child in children)
            child.Scale0(x, y, z);
    }

    public void Scale0(float x, float y, float z)
    {
        scaling.X /= x;
        scaling.Y /= y;
        scaling.Z /= z;
    }

    public void RotateX(float angle)
    {
        //rotation.RotateAxis(new Vector3(1,0,0), angle);
        rot.X += angle;
    }

    public void RotateY(float angle)
    {
        //rotation.RotateAxis(new Vector3(0,1,0), angle);
        rot.Y += angle;
    }

    public void RotateZ(float angle)
    {
        //rotation.RotateAxis(new Vector3(0,0,1), angle);
        rot.Z += angle;
    }

    public void Move(float x, float y, float z)
    {
        translation += new Vector3(x, y, z);
    }
}
}
