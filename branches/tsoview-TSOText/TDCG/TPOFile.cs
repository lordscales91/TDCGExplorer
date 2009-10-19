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
/// TPO�t�@�C���̃��X�g�ł��B
/// </summary>
public class TPOFileList
{
    private List<TPOFile> list = new List<TPOFile>();
    //�������[�V�����s��l��ێ�����t���[���z��
    private TMOFrame[] frames;

    private TMOFile tmo = null;

    /// <summary>
    /// �C���f�N�T
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
    /// �v�f��
    /// </summary>
    public int Count
    {
        get
        {
            return list.Count;
        }
    }
    
    /// <summary>
    /// tpo��ǉ����܂��B
    /// </summary>
    /// <param name="tpo"></param>
    public void Add(TPOFile tpo)
    {
        tpo.Tmo = tmo;
        list.Add(tpo);
    }

    /// <summary>
    /// ���X�g���������܂��B
    /// </summary>
    public void Clear()
    {
        list.Clear();
    }

    /// <summary>
    /// �̌^���X�g��ݒ肵�܂��B
    /// </summary>
    /// <param name="pro_list">�̌^���X�g</param>
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
    /// Tpo.Tmo�Ɋ܂܂�郂�[�V�����s��l��ό`���܂��B
    /// </summary>
    public void Transform()
    {
        LoadMatrix();
        foreach (TPOFile tpo in list)
            tpo.Transform();
    }

    /// <summary>
    /// �w��ԍ��̃t���[���Ɋ܂܂�郂�[�V�����s��l��ό`���܂��B
    /// </summary>
    /// <param name="i">�t���[���ԍ�</param>
    /// <param name="frame_count">�t���[����</param>
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

    //�������[�V�����s��l��ێ�����̈���m�ۂ���B
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
    /// �ޔ����[�V�����s��l��tmo�ɖ߂��܂��B
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
    /// ���[�V�����s��l��tmo����ޔ����܂��B
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
    /// TPO�t�@�C���������܂��B
    /// </summary>
public class TPOFile
{
    private float ratio = 0.0f;
    private TMOFile tmo = null;
    private Dictionary<string, TPONode> nodemap = new Dictionary<string, TPONode>();
    private IProportion proportion = null;

    /// <summary>
    /// bone�z��
    /// </summary>
    public TPONode[] nodes;

    /// <summary>
    /// TPONode�̕ό`�W���ɏ悸��ό`�䗦
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

            //tmo node����name�𓾂Đݒ肷��B
            //������nodemap�ɒǉ�����B
            for (int i = 0; i < node_count; i++)
            {
                string name = tmo.nodes[i].Name;
                nodes[i] = new TPONode(i, name);
                nodemap.Add(name, nodes[i]);
            }

            //�e�q�֌W��ݒ肷��B
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
    /// tpo�𐶐����܂��B
    /// </summary>
    public TPOFile()
    {
    }

    /// <summary>
    /// �̌^
    /// </summary>
    public IProportion Proportion { get { return proportion; } set { proportion = value; }}

    /// <summary>
    /// TPONode�ɕό`�W����ݒ肵�܂��B
    /// </summary>
    public void ExecuteProportion()
    {
        if (proportion == null)
            return;

        Dictionary<string, TPONode> nodemap = new Dictionary<string, TPONode>();
        foreach (TPONode node in nodes)
            nodemap[node.ShortName] = node;

        proportion.Nodes = nodemap;
        //TPONode�ɕό`�W����ݒ肷��B
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
    /// Tpo.Tmo�Ɋ܂܂�郂�[�V�����s��l��ό`���܂��B
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
    /// �w��ԍ��̃t���[���Ɋ܂܂�郂�[�V�����s��l��ό`���܂��B
    /// </summary>
    /// <param name="i">�t���[���ԍ�</param>
    /// <param name="frame_count">�t���[����</param>
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
                TMOMat mat = tmo.frames[i].matrices[j];//�ό`�Ώۃ��[�V�����s��
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
                TMOMat mat = tmo.frames[i].matrices[j];//�ό`�Ώۃ��[�V�����s��
                node.Transform(mat, ratio);
            }
        }
    }
}

    /// <summary>
    /// �̌^�ύX����
    /// </summary>
public class TPOCommand
{
    /// <summary>
    /// ����^�C�v
    /// </summary>
    public enum Type {
        /// <summary>
        /// �g��
        /// </summary>
        Scale,
        /// <summary>
        /// �g��i�qbone�͂��ꂼ��k���j
        /// </summary>
        Scale1,
        /// <summary>
        /// �k��
        /// </summary>
        Scale0,
        /// <summary>
        /// X����]
        /// </summary>
        RotateX,
        /// <summary>
        /// Y����]
        /// </summary>
        RotateY,
        /// <summary>
        /// Z����]
        /// </summary>
        RotateZ,
        /// <summary>
        /// �ړ�
        /// </summary>
        Move
    };
    internal Type type;
    internal Vector3 v;
    internal float angle;
}

    /// <summary>
    /// bone�������܂��B
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
    /// ����
    /// </summary>
    public string Name { get { return name; } }
    /// <summary>
    /// ���́i�Z���`���j
    /// </summary>
    public string ShortName { get { return sname; } }

    internal List<TPOCommand> command_list = new List<TPOCommand>();

    /// <summary>
    /// �ό`�����ǉ����܂��B
    /// </summary>
    /// <param name="command"></param>
    public void AddCommand(TPOCommand command)
    {
        command_list.Add(command);
    }
    
    /// <summary>
    /// �ό`�����ǉ����܂��B
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
    /// �ό`�����ǉ����܂��B
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
    /// TPONode�𐶐����܂��B
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
    /// ���[�V�����s����w��䗦�ŕό`���܂��B
    /// </summary>
    /// <param name="mat">���[�V�����s��</param>
    /// <param name="ratio">�䗦</param>
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
    /// �g�債�܂��B
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    public void Scale(float x, float y, float z)
    {
        AddCommand(TPOCommand.Type.Scale, new Vector3(x, y, z));
    }

    /// <summary>
    /// �g�債�܂��B�����Ɏqbone�͂��ꂼ��k�����܂��B
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
    /// �k�����܂��B
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    public void Scale0(float x, float y, float z)
    {
        AddCommand(TPOCommand.Type.Scale0, new Vector3(x, y, z));
    }

    /// <summary>
    /// X����]���s���܂��B
    /// </summary>
    /// <param name="angle"></param>
    public void RotateX(float angle)
    {
        AddCommand(TPOCommand.Type.RotateX, angle);
    }

    /// <summary>
    /// Y����]���s���܂��B
    /// </summary>
    /// <param name="angle"></param>
    public void RotateY(float angle)
    {
        AddCommand(TPOCommand.Type.RotateY, angle);
    }

    /// <summary>
    /// Z����]���s���܂��B
    /// </summary>
    /// <param name="angle"></param>
    public void RotateZ(float angle)
    {
        AddCommand(TPOCommand.Type.RotateZ, angle);
    }

    /// <summary>
    /// �ړ����܂��B
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
