using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace TDCG
{

public class TPOFileList
{
    List<TPOFile> list = new List<TPOFile>();

    public TPOFile this[int i]
    {
        get
        {
            return list[i];
        }
    }

    public int Count
    {
        get
        {
            return list.Count;
        }
    }
    
    public void Add(TPOFile tpo)
    {
        tpo.Tmo = tmo;
        list.Add(tpo);
    }

    public void Clear()
    {
        list.Clear();
    }

    public void SetProportionList(List<IProportion> pro_list)
    {
        Clear();
        foreach (IProportion pro in pro_list)
        {
            TPOFile tpo = new TPOFile();
            tpo.Proportion = pro;
            Add(tpo);
        }
    }

    public void Transform()
    {
        LoadMatrix();
        foreach (TPOFile tpo in list)
            tpo.Transform();
    }

    public void Transform(int i)
    {
        LoadMatrix();
        foreach (TPOFile tpo in list)
            tpo.Transform(i);
    }

    //初期モーション行列値を保持するフレーム配列
    internal TMOFrame[] frames;

    internal TMOFile tmo = null;
    public TMOFile Tmo
    {
        get
        {
            return tmo;
        }
        set
        {
            tmo = value;

            foreach (TPOFile tpo in list)
                tpo.Tmo = tmo;

            CreateFrames();
            SaveMatrix();
        }
    }

    //初期モーション行列値を保持する領域を確保する。
    private void CreateFrames()
    {
        if (tmo.frames == null)
            return;

        int frame_count = tmo.frames.Length;
        frames = new TMOFrame[frame_count];
        for (int i = 0; i < frame_count; i++)
        {
            int matrix_count = tmo.frames[i].matrices.Length;
            frames[i] = new TMOFrame();
            frames[i].id = i;
            frames[i].matrices = new TMOMat[matrix_count];
            for (int j = 0; j < matrix_count; j++)
            {
                frames[i].matrices[j] = new TMOMat();
            }
        }
    }

    public void LoadMatrix()
    {
        if (frames == null)
            return;

        if (tmo.frames == null)
            return;

        int frame_count = frames.Length;
        for (int i = 0; i < frame_count; i++)
        {
            int matrix_count = frames[i].matrices.Length;
            for (int j = 0; j < matrix_count; j++)
                tmo.frames[i].matrices[j].m = frames[i].matrices[j].m;
        }
    }

    public void SaveMatrix()
    {
        if (frames == null)
            return;

        if (tmo.frames == null)
            return;

        int frame_count = frames.Length;
        for (int i = 0; i < frame_count; i++)
        {
            int matrix_count = frames[i].matrices.Length;
            for (int j = 0; j < matrix_count; j++)
                frames[i].matrices[j].m = tmo.frames[i].matrices[j].m;
        }
    }
}

public class TPOFile
{
    public TPONode[] nodes;

    internal float ratio = 0.0f;
    /// <summary>
    /// TPONodeの変形係数に乗ずる変形比率
    /// </summary>
    public float Ratio
    {
        get { return ratio; } set { ratio = value; }
    }
    internal Dictionary<string, TPONode> nodemap = new Dictionary<string, TPONode>();

    internal TMOFile tmo = null;
    public TMOFile Tmo
    {
        get
        {
            return tmo;
        }

        set
        {
            nodemap.Clear();
            nodes = null;

            tmo = value;

            if (tmo.nodes == null)
                return;

            int node_count = tmo.nodes.Length;
            nodes = new TPONode[node_count];

            //tmo nodeからnameを得て設定する。
            //そしてnodemapに追加する。
            for (int i = 0; i < node_count; i++)
            {
                nodes[i] = new TPONode();
                nodes[i].id = i;
                nodes[i].name = tmo.nodes[i].name;
                nodes[i].sname = nodes[i].name.Substring(nodes[i].name.LastIndexOf('|')+1);
                nodemap.Add(nodes[i].name, nodes[i]);

                //Console.WriteLine(i + ": " + nodes[i].sname);
            }

            //親子関係を設定する。
            for (int i = 0; i < node_count; i++)
            {
                int index = nodes[i].name.LastIndexOf('|');
                if (index <= 0)
                    continue;
                string pname = nodes[i].name.Substring(0, index);
                nodes[i].parent = nodemap[pname];
                nodes[i].parent.children.Add(nodes[i]);
            }

            ExecuteProportion();
        }
    }

    public TPOFile()
    {
    }

    IProportion proportion = null;
    public IProportion Proportion { get { return Proportion; } set { proportion = value; }}

    public void ExecuteProportion()
    {
        if (proportion == null)
            return;

        Dictionary<string, TPONode> nodemap = new Dictionary<string, TPONode>();
        foreach (TPONode node in nodes)
            nodemap[node.sname] = node;

        proportion.Nodes = nodemap;
        //TPONodeに変形係数を設定する。
        proportion.Execute();
    }

    /// <summary>
    /// TPONodeの変形係数に従ってTpo.Tmoのモーション行列値を変形する。
    /// </summary>
    public void Transform()
    {
        if (tmo.frames == null)
            return;

        int frame_count = tmo.frames.Length;
        for (int i = 0; i < frame_count; i++)
        {
            Transform(i);
        }
    }

    public void Transform(int i)
    {
        if (tmo.frames == null)
            return;

        int matrix_count = tmo.frames[i].matrices.Length;
        for (int j = 0; j < matrix_count; j++)
        {
            TPONode node = nodes[j];
            Debug.Assert(node != null, "node should not be null j=" + j.ToString());
            TMOMat mat = tmo.frames[i].matrices[j];//変形対象モーション行列

            Matrix m = mat.m;

            Vector3 v = node.ScalingVector3(ratio);
            m.M11 *= v.X;
            m.M12 *= v.X;
            m.M13 *= v.X;
            m.M21 *= v.Y;
            m.M22 *= v.Y;
            m.M23 *= v.Y;
            m.M31 *= v.Z;
            m.M32 *= v.Z;
            m.M33 *= v.Z;

            m = m * node.RotationMatrix(m, ratio);

            Vector3 t = node.Translation;
            m.M41 += t.X;
            m.M42 += t.X;
            m.M43 += t.X;

            mat.m = m;
        }
    }
}

public class TPONode
{
    internal int id;
    internal string name;

    internal Vector3 scaling = new Vector3(1, 1, 1);
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

    public Matrix ScalingMatrix(float ratio)
    {
        Vector3 v;
        v.X = (float)Math.Pow(scaling.X, ratio);
        v.Y = (float)Math.Pow(scaling.Y, ratio);
        v.Z = (float)Math.Pow(scaling.Z, ratio);
        return Matrix.Scaling(v);
    }

    public Vector3 ScalingVector3(float ratio)
    {
        Vector3 v;
        v.X = (float)Math.Pow(scaling.X, ratio);
        v.Y = (float)Math.Pow(scaling.Y, ratio);
        v.Z = (float)Math.Pow(scaling.Z, ratio);
        return v;
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
        scaling.X *= x;
        scaling.Y *= y;
        scaling.Z *= z;
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
