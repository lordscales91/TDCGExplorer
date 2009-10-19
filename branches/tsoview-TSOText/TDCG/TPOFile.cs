using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace TDCG
{
/// <summary>
/// TPOファイルのリストです。
/// </summary>
public class TPOFileList
{
    private List<TPOFile> list = new List<TPOFile>();
    //初期モーション行列値を保持するフレーム配列
    private TMOFrame[] frames;

    private TMOFile tmo = null;

    /// <summary>
    /// インデクサ
    /// </summary>
    /// <param name="i">index</param>
    /// <returns>tpo</returns>
    public TPOFile this[int i]
    {
        get
        {
            return list[i];
        }
    }

    /// <summary>
    /// 要素数
    /// </summary>
    public int Count
    {
        get
        {
            return list.Count;
        }
    }
    
    /// <summary>
    /// tpoを追加します。
    /// </summary>
    /// <param name="tpo"></param>
    public void Add(TPOFile tpo)
    {
        tpo.Tmo = tmo;
        list.Add(tpo);
    }

    /// <summary>
    /// リストを消去します。
    /// </summary>
    public void Clear()
    {
        list.Clear();
    }

    /// <summary>
    /// 体型リストを設定します。
    /// </summary>
    /// <param name="pro_list">体型リスト</param>
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

    /// <summary>
    /// Tpo.Tmoに含まれるモーション行列値を変形します。
    /// </summary>
    public void Transform()
    {
        LoadMatrix();
        foreach (TPOFile tpo in list)
            tpo.Transform();
    }

    /// <summary>
    /// 指定番号のフレームに含まれるモーション行列値を変形します。
    /// </summary>
    /// <param name="i">フレーム番号</param>
    /// <param name="frame_count">フレーム数</param>
    public void Transform(int i, int frame_count)
    {
        LoadMatrix();
        foreach (TPOFile tpo in list)
            tpo.Transform(i, frame_count);
    }

    /// <summary>
    /// tmo
    /// </summary>
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

    /// <summary>
    /// 退避モーション行列値をtmoに戻します。
    /// </summary>
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

    /// <summary>
    /// モーション行列値をtmoから退避します。
    /// </summary>
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

    /// <summary>
    /// TPOファイルを扱います。
    /// </summary>
public class TPOFile
{
    private float ratio = 0.0f;
    private TMOFile tmo = null;
    private Dictionary<string, TPONode> nodemap = new Dictionary<string, TPONode>();
    private IProportion proportion = null;

    /// <summary>
    /// bone配列
    /// </summary>
    public TPONode[] nodes;

    /// <summary>
    /// TPONodeの変形係数に乗ずる変形比率
    /// </summary>
    public float Ratio
    {
        get { return ratio; } set { ratio = value; }
    }

    /// <summary>
    /// tmo
    /// </summary>
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

    /// <summary>
    /// tpoを生成します。
    /// </summary>
    public TPOFile()
    {
    }

    /// <summary>
    /// 体型
    /// </summary>
    public IProportion Proportion { get { return proportion; } set { proportion = value; }}

    /// <summary>
    /// TPONodeに変形係数を設定します。
    /// </summary>
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
    /// Tpo.Tmoに含まれるモーション行列値を変形します。
    /// </summary>
    public void Transform()
    {
        if (tmo.frames == null)
            return;

        int frame_count = tmo.frames.Length;
        for (int i = 0; i < frame_count; i++)
        {
            Transform(i, frame_count);
        }
    }

    /// <summary>
    /// 指定番号のフレームに含まれるモーション行列値を変形します。
    /// </summary>
    /// <param name="i">フレーム番号</param>
    /// <param name="frame_count">フレーム数</param>
    public void Transform(int i, int frame_count)
    {
        if (tmo.frames == null)
            return;

        int matrix_count = tmo.frames[i].matrices.Length;
        int frame_mid = frame_count / 2;
        if (frame_mid != 0)
        {
            for (int j = 0; j < matrix_count; j++)
            {
                TPONode node = nodes[j];
                Debug.Assert(node != null, "node should not be null j=" + j.ToString());
                TMOMat mat = tmo.frames[i].matrices[j];//変形対象モーション行列
                float power;
                if ((i / frame_mid) % 2 == 0)
                    power = (i % frame_mid) / (float)frame_mid;
                else
                    power = (frame_mid - (i % frame_mid) - 1) / (float)frame_mid;
                node.Transform(mat, ratio * power);
            }
        }
        else
        {
            for (int j = 0; j < matrix_count; j++)
            {
                TPONode node = nodes[j];
                Debug.Assert(node != null, "node should not be null j=" + j.ToString());
                TMOMat mat = tmo.frames[i].matrices[j];//変形対象モーション行列
                node.Transform(mat, ratio);
            }
        }
    }
}

    /// <summary>
    /// 体型変更操作
    /// </summary>
public class TPOCommand
{
    /// <summary>
    /// 操作タイプ
    /// </summary>
    public enum Type {
        /// <summary>
        /// 拡大
        /// </summary>
        Scale,
        /// <summary>
        /// 拡大（子boneはそれぞれ縮小）
        /// </summary>
        Scale1,
        /// <summary>
        /// 縮小
        /// </summary>
        Scale0,
        /// <summary>
        /// X軸回転
        /// </summary>
        RotateX,
        /// <summary>
        /// Y軸回転
        /// </summary>
        RotateY,
        /// <summary>
        /// Z軸回転
        /// </summary>
        RotateZ,
        /// <summary>
        /// 移動
        /// </summary>
        Move
    };
    internal Type type;
    internal Vector3 v;
    internal float angle;
}

    /// <summary>
    /// boneを扱います。
    /// </summary>
public class TPONode
{
    private int id;
    private string name;
    private string sname;
    
    internal List<TPONode> children = new List<TPONode>();
    internal TPONode parent;

    /// <summary>
    /// ID
    /// </summary>
    public int ID { get { return id; } }
    /// <summary>
    /// 名称
    /// </summary>
    public string Name { get { return name; } }
    /// <summary>
    /// 名称（短い形式）
    /// </summary>
    public string ShortName { get { return sname; } }

    internal List<TPOCommand> command_list = new List<TPOCommand>();

    /// <summary>
    /// 変形操作を追加します。
    /// </summary>
    /// <param name="command"></param>
    public void AddCommand(TPOCommand command)
    {
        command_list.Add(command);
    }
    
    /// <summary>
    /// 変形操作を追加します。
    /// </summary>
    /// <param name="type"></param>
    /// <param name="v"></param>
    public void AddCommand(TPOCommand.Type type, Vector3 v)
    {
        TPOCommand command = new TPOCommand();
        command.type = type;
        command.v = v;
        command_list.Add(command);
    }
    
    /// <summary>
    /// 変形操作を追加します。
    /// </summary>
    /// <param name="type"></param>
    /// <param name="angle"></param>
    public void AddCommand(TPOCommand.Type type, float angle)
    {
        TPOCommand command = new TPOCommand();
        command.type = type;
        command.angle = angle;
        command_list.Add(command);
    }

    /// <summary>
    /// TPONodeを生成します。
    /// </summary>
    /// <param name="id"></param>
    /// <param name="name"></param>
    public TPONode(int id, string name)
    {
        this.id = id;
        this.name = name;
        this.sname = this.name.Substring(this.name.LastIndexOf('|') + 1);
    }

    /// <summary>
    /// モーション行列を指定比率で変形します。
    /// </summary>
    /// <param name="mat">モーション行列</param>
    /// <param name="ratio">比率</param>
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
                    mat.Move(command.v * ratio);
                    break;
            }
        }
    }

    /// <summary>
    /// 拡大します。
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    public void Scale(float x, float y, float z)
    {
        AddCommand(TPOCommand.Type.Scale, new Vector3(x, y, z));
    }

    /// <summary>
    /// 拡大します。同時に子boneはそれぞれ縮小します。
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    public void Scale1(float x, float y, float z)
    {
        AddCommand(TPOCommand.Type.Scale1, new Vector3(x, y, z));

        foreach (TPONode child in children)
            child.Scale0(x, y, z);
    }

    /// <summary>
    /// 縮小します。
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    public void Scale0(float x, float y, float z)
    {
        AddCommand(TPOCommand.Type.Scale0, new Vector3(x, y, z));
    }

    /// <summary>
    /// X軸回転を行います。
    /// </summary>
    /// <param name="angle"></param>
    public void RotateX(float angle)
    {
        AddCommand(TPOCommand.Type.RotateX, angle);
    }

    /// <summary>
    /// Y軸回転を行います。
    /// </summary>
    /// <param name="angle"></param>
    public void RotateY(float angle)
    {
        AddCommand(TPOCommand.Type.RotateY, angle);
    }

    /// <summary>
    /// Z軸回転を行います。
    /// </summary>
    /// <param name="angle"></param>
    public void RotateZ(float angle)
    {
        AddCommand(TPOCommand.Type.RotateZ, angle);
    }

    /// <summary>
    /// 移動します。
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    public void Move(float x, float y, float z)
    {
        AddCommand(TPOCommand.Type.Move, new Vector3(x, y, z));
    }
}
}
