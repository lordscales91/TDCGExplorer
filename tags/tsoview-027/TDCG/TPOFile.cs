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
    /// TPO�t�@�C���������܂��B
    /// </summary>
public class TPOFile
{
    private float ratio = 0.0f;
    private TMOFile tmo = null;
    private IProportion proportion = null;

    /// <summary>
    /// bone�z��
    /// </summary>
    public TPONode[] nodes;

    private Dictionary<string, TPONode> nodemap;

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
            nodemap = null;
            nodes = null;

            tmo = value;

            if (tmo == null)
                return;

            if (tmo.nodes == null)
                return;

            int node_count = tmo.nodes.Length;
            nodes = new TPONode[node_count];

            for (int i = 0; i < node_count; i++)
            {
                nodes[i] = new TPONode(i, tmo.nodes[i].Name);
            }

            GenerateNodemapAndTree();

            ExecuteProportion();
        }
    }

    void GenerateNodemapAndTree()
    {
        nodemap = new Dictionary<string, TPONode>();

        for (int i = 0; i < nodes.Length; i++)
        {
            nodemap.Add(nodes[i].Name, nodes[i]);
        }

        for (int i = 0; i < nodes.Length; i++)
        {
            int index = nodes[i].Name.LastIndexOf('|');
            if (index <= 0)
                continue;
            string pname = nodes[i].Name.Substring(0, index);
            nodes[i].parent = nodemap[pname];
            nodes[i].parent.children.Add(nodes[i]);
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
    /// �S�Ẵt���[���Ɋ܂܂�郂�[�V�����s��l��ό`���܂��B
    /// </summary>
    public void Transform()
    {
        if (ratio == 0)
            return;

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
    /// <param name="frame_index">�t���[���ԍ�</param>
    public void Transform(int frame_index)
    {
        if (ratio == 0)
            return;

        if (tmo.frames == null)
            return;

        TMOFrame frame = tmo.frames[frame_index];
        Debug.Assert(frame.matrices.Length == nodes.Length);
        for (int j = 0; j < frame.matrices.Length; j++)
        {
            TPONode node = nodes[j];
            TMOMat mat = frame.matrices[j];
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

    /// �g�呀��𓾂܂��B
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

    /// �k������𓾂܂��B
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

    /// �g�呀��܂��͏k������𓾂܂��B
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

    /// ��]����𓾂܂��B
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

    /// �g�呀��܂��͏k������܂��͉�]����𓾂܂��B
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

    /// �ړ�����𓾂܂��B
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

    /// �g��ψʂ𓾂܂��B
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

    /// ��]�p�𓾂܂��B
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

    /// �ړ��ψʂ𓾂܂��B
    public Vector3 GetTranslation()
    {
        TPOCommand translation_command = FindTranslationCommand();
        if (translation_command != null)
            return translation_command.GetVector3();
        else
            return new Vector3(0, 0, 0);
    }

    /// �g��ψʂ�ݒ肵�܂��B
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

    /// �k���ψʂ�ݒ肵�܂��B
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

    /// ��]�p��ݒ肵�܂��B
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

    /// �ړ��ψʂ�ݒ肵�܂��B
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

    /// �g�呀����폜���܂��B
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

    /// �k��������폜���܂��B
    public void RemoveInverseScaling()
    {
        TPOCommand scaling_command = FindInverseScalingCommand();
        if (scaling_command != null)
            commands.Remove(scaling_command);
    }

    /// ��]������폜���܂��B
    public void RemoveAngle()
    {
        TPOCommand rotation_command = FindRotationCommand();
        if (rotation_command != null)
            commands.Remove(rotation_command);
    }

    /// �ړ�������폜���܂��B
    public void RemoveTranslation()
    {
        TPOCommand translation_command = FindTranslationCommand();
        if (translation_command != null)
            commands.Remove(translation_command);
    }
}
}
