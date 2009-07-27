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

            if (tmo == null)
                return;

            if (tmo.nodes == null)
                return;

            int node_count = tmo.nodes.Length;
            nodes = new TPONode[node_count];

            //tmo nodeからnameを得て設定する。
            //そしてnodemapに追加する。
            for (int i = 0; i < node_count; i++)
            {
                string name = tmo.nodes[i].Name;
                nodes[i] = new TPONode(i, name);
                nodemap.Add(name, nodes[i]);
            }

            //親子関係を設定する。
            for (int i = 0; i < node_count; i++)
            {
                int index = nodes[i].Name.LastIndexOf('|');
                if (index <= 0)
                    continue;
                string pname = nodes[i].Name.Substring(0, index);
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
    public IProportion Proportion { get { return proportion; } set { proportion = value; }}

    public void ExecuteProportion()
    {
        if (proportion == null)
            return;

        Dictionary<string, TPONode> nodemap = new Dictionary<string, TPONode>();
        foreach (TPONode node in nodes)
            nodemap[node.ShortName] = node;

        proportion.Nodes = nodemap;
        //TPONodeに変形係数を設定する。
        try
        {
            proportion.Execute();
        }
        catch (KeyNotFoundException)
        {
            /* not found */
        }
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
            node.Transform(mat, ratio);
        }
    }
}

public class TPOCommand
{
    public enum Type { Scale, Scale1, Scale0, RotateX, RotateY, RotateZ, Move };
    internal Type type;
    internal Vector3 v;
    internal float angle;
}

public class TPONode
{
    private int id;
    private string name;
    private string sname;
    
    internal List<TPONode> children = new List<TPONode>();
    internal TPONode parent;

    public int ID { get { return id; } }
    public string Name { get { return name; } }
    public string ShortName { get { return sname; } }

    internal List<TPOCommand> command_list = new List<TPOCommand>();

    public void AddCommand(TPOCommand command)
    {
        command_list.Add(command);
    }
    
    public void AddCommand(TPOCommand.Type type, Vector3 v)
    {
        TPOCommand command = new TPOCommand();
        command.type = type;
        command.v = v;
        command_list.Add(command);
    }
    
    public void AddCommand(TPOCommand.Type type, float angle)
    {
        TPOCommand command = new TPOCommand();
        command.type = type;
        command.angle = angle;
        command_list.Add(command);
    }

    public TPONode(int id, string name)
    {
        this.id = id;
        this.name = name;
        this.sname = this.name.Substring(this.name.LastIndexOf('|') + 1);
    }

    public void Transform(TMOMat mat, float ratio)
    {
        foreach (TPOCommand command in command_list)
        {
            Matrix scaling = Matrix.Identity;
            switch (command.type)
            {
                case TPOCommand.Type.Scale:
                case TPOCommand.Type.Scale1:
                case TPOCommand.Type.Scale0:
                    Vector3 v;
                    v.X = (float)Math.Pow(command.v.X, ratio);
                    v.Y = (float)Math.Pow(command.v.Y, ratio);
                    v.Z = (float)Math.Pow(command.v.Z, ratio);
                    scaling = Matrix.Scaling(v);
                    break;
            }
            switch (command.type)
            {
                case TPOCommand.Type.Scale:
                    mat.Scale(scaling);
                    break;
                case TPOCommand.Type.Scale1:
                    mat.Scale1(scaling);
                    break;
                case TPOCommand.Type.Scale0:
                    mat.Scale0(scaling);
                    break;
                case TPOCommand.Type.RotateX:
                    mat.RotateX(command.angle * ratio);
                    break;
                case TPOCommand.Type.RotateY:
                    mat.RotateY(command.angle * ratio);
                    break;
                case TPOCommand.Type.RotateZ:
                    mat.RotateZ(command.angle * ratio);
                    break;
                case TPOCommand.Type.Move:
                    mat.Move(command.v);
                    break;
            }
        }
    }

    public void Scale(float x, float y, float z)
    {
        AddCommand(TPOCommand.Type.Scale, new Vector3(x, y, z));
    }

    public void Scale1(float x, float y, float z)
    {
        AddCommand(TPOCommand.Type.Scale1, new Vector3(x, y, z));

        foreach (TPONode child in children)
            child.Scale0(x, y, z);
    }

    public void Scale0(float x, float y, float z)
    {
        AddCommand(TPOCommand.Type.Scale0, new Vector3(x, y, z));
    }

    public void RotateX(float angle)
    {
        AddCommand(TPOCommand.Type.RotateX, angle);
    }

    public void RotateY(float angle)
    {
        AddCommand(TPOCommand.Type.RotateY, angle);
    }

    public void RotateZ(float angle)
    {
        AddCommand(TPOCommand.Type.RotateZ, angle);
    }

    public void Move(float x, float y, float z)
    {
        AddCommand(TPOCommand.Type.Move, new Vector3(x, y, z));
    }
}
}
