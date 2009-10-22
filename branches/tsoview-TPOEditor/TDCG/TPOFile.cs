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
    /// TPO�t�@�C�����X�g
    public List<TPOFile> files = new List<TPOFile>();

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
            return files[i];
        }
    }

    /// <summary>
    /// �v�f��
    /// </summary>
    public int Count
    {
        get
        {
            return files.Count;
        }
    }
    
    /// <summary>
    /// tpo��ǉ����܂��B
    /// </summary>
    /// <param name="tpo"></param>
    public void Add(TPOFile tpo)
    {
        tpo.Tmo = tmo;
        files.Add(tpo);
    }

    /// <summary>
    /// ���X�g���������܂��B
    /// </summary>
    public void Clear()
    {
        files.Clear();
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
        foreach (TPOFile tpo in files)
            tpo.Transform();
    }

    /// <summary>
    /// �w��ԍ��̃t���[���Ɋ܂܂�郂�[�V�����s��l��ό`���܂��B
    /// </summary>
    /// <param name="i">�t���[���ԍ�</param>
    public void Transform(int i)
    {
        LoadMatrix();
        foreach (TPOFile tpo in files)
            tpo.Transform(i);
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

            foreach (TPOFile tpo in files)
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
    /// �̌^�̖��O�𓾂܂��B
    /// </summary>
    public string ProportionName
    {
        get { return proportion.ToString(); }
    }

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
            Transform(i);
        }
    }

    /// <summary>
    /// �w��ԍ��̃t���[���Ɋ܂܂�郂�[�V�����s��l��ό`���܂��B
    /// </summary>
    /// <param name="i">�t���[���ԍ�</param>
    public void Transform(int i)
    {
        if (tmo.frames == null)
            return;

        int matrix_count = tmo.frames[i].matrices.Length;
        for (int j = 0; j < matrix_count; j++)
        {
            TPONode node = nodes[j];
            Debug.Assert(node != null, "node should not be null j=" + j.ToString());
            TMOMat mat = tmo.frames[i].matrices[j];//�ό`�Ώۃ��[�V�����s��
            node.Transform(mat, ratio);
        }
    }
}

/// <summary>
/// ����^�C�v
/// </summary>
public enum TPOCommandType
{
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
    /// ��]
    /// </summary>
    Rotate,
    /// <summary>
    /// �ړ�
    /// </summary>
    Move
};

/// <summary>
/// �̌^�ύX����
/// </summary>
public class TPOCommand
{
    internal TPOCommandType type;
    internal float x;
    internal float y;
    internal float z;

    /// ����^�C�v
    public TPOCommandType Type { get { return type; } }
    /// X���W�ψ�
    public float X { get { return x; } }
    /// Y���W�ψ�
    public float Y { get { return y; } }
    /// Z���W�ψ�
    public float Z { get { return z; } }

    /// �ψʃx�N�g���𓾂܂��B
    public Vector3 GetVector3()
    {
        return new Vector3(x, y, z);
    }
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

    /// �ό`���샊�X�g
    public List<TPOCommand> commands = new List<TPOCommand>();

    /// <summary>
    /// �ό`�����ǉ����܂��B
    /// </summary>
    /// <param name="command"></param>
    public void AddCommand(TPOCommand command)
    {
        commands.Add(command);
    }
    
    /// <summary>
    /// �ό`�����ǉ����܂��B
    /// </summary>
    /// <param name="type"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    public void AddCommand(TPOCommandType type, float x, float y, float z)
    {
        TPOCommand command = new TPOCommand();
        command.type = type;
        command.x = x;
        command.y = y;
        command.z = z;
        commands.Add(command);
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
        foreach (TPOCommand command in commands)
        {
            Matrix scaling = Matrix.Identity;
            switch (command.type)
            {
                case TPOCommandType.Scale:
                case TPOCommandType.Scale1:
                case TPOCommandType.Scale0:
                    Vector3 v;
                    v.X = (float)Math.Pow(command.X, ratio);
                    v.Y = (float)Math.Pow(command.Y, ratio);
                    v.Z = (float)Math.Pow(command.Z, ratio);
                    scaling = Matrix.Scaling(v);
                    break;
            }
            switch (command.type)
            {
                case TPOCommandType.Scale:
                    mat.Scale(scaling);
                    break;
                case TPOCommandType.Scale1:
                    mat.Scale1(scaling);
                    break;
                case TPOCommandType.Scale0:
                    mat.Scale0(scaling);
                    break;
                case TPOCommandType.Rotate:
                    mat.RotateX(command.X * ratio);
                    mat.RotateY(command.Y * ratio);
                    mat.RotateZ(command.Z * ratio);
                    break;
                case TPOCommandType.Move:
                    mat.Move(command.GetVector3() * ratio);
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
        AddCommand(TPOCommandType.Scale, x, y, z);
    }

    /// <summary>
    /// �g�債�܂��B�����Ɏqbone�͂��ꂼ��k�����܂��B
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    public void Scale1(float x, float y, float z)
    {
        AddCommand(TPOCommandType.Scale1, x, y, z);

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
        AddCommand(TPOCommandType.Scale0, x, y, z);
    }

    /// <summary>
    /// X����]���s���܂��B
    /// </summary>
    /// <param name="angle"></param>
    public void RotateX(float angle)
    {
        TPOCommand rotation_command = FindRotationCommand();
        if (rotation_command != null)
            rotation_command.x = angle;
        else
            AddCommand(TPOCommandType.Rotate, angle, 0, 0);
    }

    /// <summary>
    /// Y����]���s���܂��B
    /// </summary>
    /// <param name="angle"></param>
    public void RotateY(float angle)
    {
        TPOCommand rotation_command = FindRotationCommand();
        if (rotation_command != null)
            rotation_command.y = angle;
        else
            AddCommand(TPOCommandType.Rotate, 0, angle, 0);
    }

    /// <summary>
    /// Z����]���s���܂��B
    /// </summary>
    /// <param name="angle"></param>
    public void RotateZ(float angle)
    {
        TPOCommand rotation_command = FindRotationCommand();
        if (rotation_command != null)
            rotation_command.z = angle;
        else
            AddCommand(TPOCommandType.Rotate, 0, 0, angle);
    }

    /// <summary>
    /// �ړ����܂��B
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    public void Move(float x, float y, float z)
    {
        AddCommand(TPOCommandType.Move, x, y, z);
    }

    public TPOCommand FindScalingCommand()
    {
        TPOCommand scaling_command = null;
        foreach (TPOCommand command in commands)
        {
            switch (command.Type)
            {
                case TPOCommandType.Scale:
                case TPOCommandType.Scale1:
                    scaling_command = command;
                    break;
            }
        }
        return scaling_command;
    }

    private TPOCommand FindInverseScalingCommand()
    {
        TPOCommand scaling_command = null;
        foreach (TPOCommand command in commands)
        {
            switch (command.Type)
            {
                case TPOCommandType.Scale0:
                    scaling_command = command;
                    break;
            }
        }
        return scaling_command;
    }

    public TPOCommand LastScalingOrInverseScalingCommand()
    {
        TPOCommand scaling_command = null;
        foreach (TPOCommand command in commands)
        {
            switch (command.Type)
            {
                case TPOCommandType.Scale:
                case TPOCommandType.Scale1:
                case TPOCommandType.Scale0:
                    scaling_command = command;
                    break;
            }
        }
        return scaling_command;
    }

    public TPOCommand FindRotationCommand()
    {
        TPOCommand rotation_command = null;
        foreach (TPOCommand command in commands)
        {
            switch (command.Type)
            {
                case TPOCommandType.Rotate:
                    rotation_command = command;
                    break;
            }
        }
        return rotation_command;
    }

    public TPOCommand LastScalingOrInverseScalingOrRotationCommand()
    {
        TPOCommand scaling_or_rotation_command = null;
        foreach (TPOCommand command in commands)
        {
            switch (command.Type)
            {
                case TPOCommandType.Scale:
                case TPOCommandType.Scale1:
                case TPOCommandType.Scale0:
                case TPOCommandType.Rotate:
                    scaling_or_rotation_command = command;
                    break;
            }
        }
        return scaling_or_rotation_command;
    }

    public TPOCommand FindTranslationCommand()
    {
        TPOCommand translation_command = null;
        foreach (TPOCommand command in commands)
        {
            switch (command.Type)
            {
                case TPOCommandType.Move:
                    translation_command = command;
                    break;
            }
        }
        return translation_command;
    }

    public Vector3 GetScaling(out bool inv_scale_on_children)
    {
        inv_scale_on_children = false;
        TPOCommand scaling_command = FindScalingCommand();
        if (scaling_command != null)
        {
            if (scaling_command.Type == TPOCommandType.Scale1)
                inv_scale_on_children = true;
            return scaling_command.GetVector3();
        }
        else
            return new Vector3(1, 1, 1);
    }

    public Vector3 GetAngle()
    {
        TPOCommand rotation_command = FindRotationCommand();
        if (rotation_command != null)
        {
            Vector3 angle;
            angle.X = Geometry.RadianToDegree(rotation_command.X);
            angle.Y = Geometry.RadianToDegree(rotation_command.Y);
            angle.Z = Geometry.RadianToDegree(rotation_command.Z);
            return angle;
        }
        else
            return new Vector3(0, 0, 0);
    }

    public Vector3 GetTranslation()
    {
        TPOCommand translation_command = FindTranslationCommand();
        if (translation_command != null)
            return translation_command.GetVector3();
        else
            return new Vector3(0, 0, 0);
    }

    public void SetScaling(Vector3 scaling, bool inv_scale_on_children)
    {
        if (scaling == new Vector3(1, 1, 1))
        {
            RemoveScaling();
            return;
        }
        TPOCommand scaling_command = FindScalingCommand();
        if (scaling_command == null)
        {
            scaling_command = new TPOCommand();
            scaling_command.type = TPOCommandType.Scale;
            commands.Insert(0, scaling_command);
        }
        if (inv_scale_on_children)
            scaling_command.type = TPOCommandType.Scale1;
        else
            scaling_command.type = TPOCommandType.Scale;

        scaling_command.x = scaling.X;
        scaling_command.y = scaling.Y;
        scaling_command.z = scaling.Z;

        if (inv_scale_on_children)
        {
            foreach (TPONode child in children)
                child.SetInverseScaling(scaling);
        }
        else
        {
            foreach (TPONode child in children)
                child.RemoveInverseScaling();
        }
    }

    public void SetInverseScaling(Vector3 scaling)
    {
        TPOCommand scaling_command = FindInverseScalingCommand();
        if (scaling_command == null)
        {
            scaling_command = new TPOCommand();
            scaling_command.type = TPOCommandType.Scale0;
            commands.Insert(0, scaling_command);
        }
        scaling_command.x = scaling.X;
        scaling_command.y = scaling.Y;
        scaling_command.z = scaling.Z;
    }

    public void SetAngle(Vector3 angle)
    {
        if (angle == new Vector3(0, 0, 0))
        {
            RemoveAngle();
            return;
        }
        TPOCommand rotation_command = FindRotationCommand();
        if (rotation_command == null)
        {
            rotation_command = new TPOCommand();
            rotation_command.type = TPOCommandType.Rotate;

            int idx = 0;
            TPOCommand scaling_command = LastScalingOrInverseScalingCommand();
            if (scaling_command != null)
                idx = commands.IndexOf(scaling_command) + 1;

            commands.Insert(idx, rotation_command);
        }
        rotation_command.x = Geometry.DegreeToRadian(angle.X);
        rotation_command.y = Geometry.DegreeToRadian(angle.Y);
        rotation_command.z = Geometry.DegreeToRadian(angle.Z);
    }

    public void SetTranslation(Vector3 translation)
    {
        if (translation == new Vector3(0, 0, 0))
        {
            RemoveTranslation();
            return;
        }
        TPOCommand translation_command = FindTranslationCommand();
        if (translation_command == null)
        {
            translation_command = new TPOCommand();
            translation_command.type = TPOCommandType.Move;

            int idx = 0;
            TPOCommand scaling_or_rotation_command = LastScalingOrInverseScalingOrRotationCommand();
            if (scaling_or_rotation_command != null)
                idx = commands.IndexOf(scaling_or_rotation_command) + 1;

            commands.Insert(idx, translation_command);
        }
        translation_command.x = translation.X;
        translation_command.y = translation.Y;
        translation_command.z = translation.Z;
    }

    public void RemoveScaling()
    {
        TPOCommand scaling_command = FindScalingCommand();
        if (scaling_command != null)
        {
            commands.Remove(scaling_command);

            if (scaling_command.Type == TPOCommandType.Scale1)
            {
                foreach (TPONode child in children)
                    child.RemoveInverseScaling();
            }
        }
    }

    public void RemoveInverseScaling()
    {
        TPOCommand scaling_command = FindInverseScalingCommand();
        if (scaling_command != null)
            commands.Remove(scaling_command);
    }

    public void RemoveAngle()
    {
        TPOCommand rotation_command = FindRotationCommand();
        if (rotation_command != null)
            commands.Remove(rotation_command);
    }

    public void RemoveTranslation()
    {
        TPOCommand translation_command = FindTranslationCommand();
        if (translation_command != null)
            commands.Remove(translation_command);
    }
}
}
