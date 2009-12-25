using System;
using System.Collections.Generic;
using System.Diagnostics;
//using System.Drawing;
using System.Threading;
//using System.ComponentModel;
using System.Windows.Forms;
using System.IO;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace TDCG
{
    /// <summary>
    /// �t�B�M���A
    /// </summary>
public class Figure : IDisposable
{
    /// <summary>
    /// �t�B�M���A���ێ����Ă���tso���X�g
    /// </summary>
    public List<TSOFile> TSOList = new List<TSOFile>();

    /// <summary>
    /// �X���C�_�ό`�s��
    /// </summary>
    public SlideMatrices slide_matrices = new SlideMatrices();

    Vector3 center = Vector3.Empty;
    /// <summary>
    /// ���S���W
    /// </summary>
    public Vector3 Center
    {
        get { return center; }
    }

    Vector3 translation = Vector3.Empty;
    /// <summary>
    /// �ړ��ψ�
    /// </summary>
    public Vector3 Translation
    {
        get { return translation; }
    }

    TMOFile tmo = null;
    /// <summary>
    /// tmo
    /// </summary>
    public TMOFile Tmo
    {
        get { return tmo; }
        set
        {
            tmo = value;
            ResetFrameIndex();
            SetCenterToHips();
            tpo_list.Tmo = tmo;
        }
    }

    internal Dictionary<TSONode, TMONode> nodemap;

    private MatrixStack matrixStack = null;
    private int frame_index = 0;
    private int current_frame_index = 0;

    /// <summary>
    /// �̌^�X�N���v�g�̃��X�g
    /// </summary>
    public static ProportionList ProportionList { get; set; }

    TPOFileList tpo_list = new TPOFileList();
    /// <summary>
    /// TPO�t�@�C���̃��X�g
    /// </summary>
    public TPOFileList TPOList { get { return tpo_list; } }

    /// <summary>
    /// �̌^���V�s�̃t�@�C����
    /// </summary>
    /// <returns></returns>
    public static string GetTPOConfigPath()
    {
        return Path.Combine(Application.StartupPath, @"TPOConfig.xml");
    }

    /// <summary>
    /// �t�B�M���A�𐶐����܂��B
    /// </summary>
    public Figure()
    {
        tmo = new TMOFile();
        nodemap = new Dictionary<TSONode, TMONode>();
        matrixStack = new MatrixStack();

        tpo_list.SetProportionList(ProportionList);

        string config_file = GetTPOConfigPath();
        if (File.Exists(config_file))
        {
            TPOConfig config = TPOConfig.Load(config_file);
            tpo_list.SetRatiosFromConfig(config);
        }
    }

    /// <summary>
    /// �̌^�ό`���s���܂��B
    /// </summary>
    public void TransformTpo()
    {
        if (tmo.frames == null)
            return;

        tpo_list.Transform();
    }

    /// <summary>
    /// �̌^�ό`���s���܂��B
    /// </summary>
    /// <param name="frame_index">�t���[���ԍ�</param>
    public void TransformTpo(int frame_index)
    {
        if (tmo.frames == null)
            return;

        tpo_list.Transform(frame_index);
    }
    
    /// <summary>
    /// �t�B�M���A���ړ����܂��i���΍��W�j�B
    /// </summary>
    /// <param name="dx">X���ψ�</param>
    /// <param name="dy">Y���ψ�</param>
    /// <param name="dz">Z���ψ�</param>
    public void Move(float dx, float dy, float dz)
    {
        Move(new Vector3(dx, dy, dz));
    }

    /// <summary>
    /// �t�B�M���A���ړ����܂��i���΍��W�j�B
    /// </summary>
    /// <param name="delta">�ψ�</param>
    public void Move(Vector3 delta)
    {
        center += delta;
        translation += delta;
        UpdateBoneMatrices(true);
    }

    /// <summary>
    /// �w��ʒu�ɂ���tso�̈ʒu�����ւ��܂��B�`�揇��ύX���܂��B
    /// </summary>
    /// <param name="aidx">���X�g��̈ʒua</param>
    /// <param name="bidx">���X�g��̈ʒub</param>
    public void SwapAt(int aidx, int bidx)
    {
        Debug.Assert(aidx < bidx);
        TSOFile a = TSOList[aidx];
        TSOFile b = TSOList[bidx];
        TSOList.RemoveAt(bidx);
        TSOList.RemoveAt(aidx);
        TSOList.Insert(aidx, b);
        TSOList.Insert(bidx, a);
    }

    /// <summary>
    /// nodemap��bone�s����X�V���܂��B
    /// tmo���ǂݍ��܂�Ă��Ȃ��ꍇ�͐擪��tso���琶�����܂��B
    /// </summary>
    public void UpdateNodeMapAndBoneMatrices()
    {
        if (tmo.frames == null)
            if (TSOList.Count != 0)
            {
                Tmo = TSOList[0].GenerateTMO();
                TransformTpo();
            }

        nodemap.Clear();
        if (tmo.frames != null)
        foreach (TSOFile tso in TSOList)
            AddNodeMap(tso);

        UpdateBoneMatrices(true);
    }

    /// <summary>
    /// tso�ɑ΂���nodemap��ǉ����܂��B
    /// </summary>
    /// <param name="tso">tso</param>
    protected void AddNodeMap(TSOFile tso)
    {
        foreach (TSONode tso_node in tso.nodes)
        {
            TMONode tmo_node;
            if (tmo.nodemap.TryGetValue(tso_node.Path, out tmo_node))
                nodemap.Add(tso_node, tmo_node);
        }
    }

    /// <summary>
    /// �t���[���ԍ���0�ɐݒ肵�܂��B
    /// </summary>
    protected void ResetFrameIndex()
    {
        frame_index = 0;
        current_frame_index = 0;
    }

    /// <summary>
    /// ���S�_����bone�̈ʒu�ɐݒ肵�܂��B
    /// </summary>
    protected void SetCenterToHips()
    {
        if (tmo.frames == null)
            return;

        TMONode tmo_node;
        if (tmo.nodemap.TryGetValue("|W_Hips", out tmo_node))
        {
            Debug.Assert(tmo_node.matrices.Count > 0);
            Matrix m = tmo_node.matrices[0].m;
            center = new Vector3(m.M41, m.M42, m.M43);
        }
    }

    /// <summary>
    /// ���̃t���[���ɐi�݂܂��B
    /// </summary>
    public void NextTMOFrame()
    {
        if (tmo.frames != null)
        {
            frame_index++;
            if (frame_index >= tmo.frames.Length)
                frame_index = 0;
        }
    }

    /// <summary>
    /// ���݂̃t���[���𓾂܂��B
    /// </summary>
    /// <returns>���݂�tmo frame</returns>
    protected TMOFrame GetTMOFrame()
    {
        if (tmo.frames != null)
        {
            Debug.Assert(current_frame_index >= 0 && current_frame_index < tmo.frames.Length);
            return tmo.frames[current_frame_index];
        }
        return null;
    }

    /// <summary>
    /// ���݂̃t���[���ԍ��𓾂܂��B
    /// </summary>
    /// <returns></returns>
    public int GetFrameIndex()
    {
        return current_frame_index;
    }

    /// <summary>
    /// tso��TSOList�ɒǉ����܂��B
    /// </summary>
    /// <param name="tso">tso</param>
    public void AddTSO(TSOFile tso)
    {
        if (tmo.frames != null)
            AddNodeMap(tso);

        TSOList.Add(tso);
    }

    /// <summary>
    /// bone�s����X�V���܂��B
    /// ������tmo frame�𖳎����܂��B
    /// </summary>
    public void UpdateBoneMatricesWithoutTMOFrame()
    {
        UpdateBoneMatrices(tmo, null);
    }

    /// <summary>
    /// bone�s����X�V���܂��B
    /// </summary>
    public void UpdateBoneMatrices()
    {
        UpdateBoneMatrices(false);
    }

    /// <summary>
    /// bone�s����X�V���܂��B
    /// </summary>
    /// <param name="forced">false�̏ꍇframe index�ɕύX�Ȃ���΍X�V���܂���B</param>
    public void UpdateBoneMatrices(bool forced)
    {
        if (!forced && frame_index == current_frame_index)
            return;
        current_frame_index = frame_index;

        TMOFrame tmo_frame = GetTMOFrame();

        UpdateBoneMatrices(tmo, tmo_frame);
    }
    
    /// <summary>
    /// bone�s����X�V���܂��B
    /// </summary>
    protected void UpdateBoneMatrices(TMOFile tmo, TMOFrame tmo_frame)
    {
        if (tmo.nodes == null)
            return;

        //�擪node��root�Ƃ݂Ȃ�
        TMONode tmo_node = tmo.nodes[0];

        //�o���X���C�_�ɂ��ό`
        Matrix local = Matrix.Scaling(slide_matrices.Local);

        //�ړ��ψʂ�ݒ�
        local.M41 = translation.X;
        local.M42 = translation.Y;
        local.M43 = translation.Z;

        matrixStack.LoadMatrix(local);
        UpdateBoneMatrices(tmo_node, tmo_frame);
    }

    void Scale1(ref Matrix m, Vector3 scaling)
    {
        m.M11 *= scaling.X;
        m.M12 *= scaling.X;
        m.M13 *= scaling.X;
        m.M21 *= scaling.Y;
        m.M22 *= scaling.Y;
        m.M23 *= scaling.Y;
        m.M31 *= scaling.Z;
        m.M32 *= scaling.Z;
        m.M33 *= scaling.Z;
    }

    void Scale1(ref Matrix m, Matrix scaling)
    {
        m.M11 *= scaling.M11;
        m.M12 *= scaling.M11;
        m.M13 *= scaling.M11;
        m.M21 *= scaling.M22;
        m.M22 *= scaling.M22;
        m.M23 *= scaling.M22;
        m.M31 *= scaling.M33;
        m.M32 *= scaling.M33;
        m.M33 *= scaling.M33;
    }

    void Transform1(ref Matrix m, Matrix transformation)
    {
        m = transformation;
    }

    void Transform2(ref Matrix m, Matrix transformation)
    {
        Vector3 t = TMOMat.DecomposeMatrix(ref transformation);
        m = transformation * m;
    }

    void Translate2(ref Matrix m, Matrix transformation)
    {
        Vector3 t = TMOMat.DecomposeMatrix(ref transformation);
        m.M41 = t.X + m.M41;
        m.M42 = t.Y + m.M42;
        m.M43 = t.Z + m.M43;
    }

    /// <summary>
    /// bone�s����X�V���܂��B
    /// </summary>
    protected void UpdateBoneMatrices(TMONode tmo_node, TMOFrame tmo_frame)
    {
        matrixStack.Push();

        if (tmo_frame != null)
        {
            // TMO animation
            tmo_node.TransformationMatrix = tmo_frame.matrices[tmo_node.ID].m;
        }
        Matrix m = tmo_node.TransformationMatrix;

        if (slide_matrices.Flat())
        {
            // �X���C�_�s��Ƃ͕ʂ�Chichi���ێ�����factor��scaling���s���B
            // �����������ł�scaling��translation��ύX���Ă͂Ȃ�Ȃ�����
            // scaling��ł��������Z��translation�ɓK�p����B
            Vector3 scaling = slide_matrices.Chichi;
            switch (tmo_node.Name)
            {
            case "Chichi_Left1":
                m *= slide_matrices.ChichiL1;
                m.M41 /= scaling.X;
                m.M42 /= scaling.Y;
                m.M43 /= scaling.Z;
                m *= Matrix.Scaling(scaling);
                break;
            case "Chichi_Left2":
                m *= slide_matrices.ChichiL2;
                m.M41 /= scaling.X;
                m.M42 /= scaling.Y;
                m.M43 /= scaling.Z;
                break;
            case "Chichi_Left3":
                m *= slide_matrices.ChichiL3;
                m.M41 /= scaling.X;
                m.M42 /= scaling.Y;
                m.M43 /= scaling.Z;
                break;
            case "Chichi_Left4":
                m *= slide_matrices.ChichiL4;
                m.M41 /= scaling.X;
                m.M42 /= scaling.Y;
                m.M43 /= scaling.Z;
                break;
            case "Chichi_Left5":
                m *= slide_matrices.ChichiL5;
                m.M41 /= scaling.X;
                m.M42 /= scaling.Y;
                m.M43 /= scaling.Z;
                break;
            }
        }

        switch (tmo_node.Name)
        {
            case "face_oya":
                Scale1(ref m, slide_matrices.FaceOya);
                break;
            case "eyeline_sita_L":
            case "L_eyeline_oya_L":
            case "Me_Right_Futi":
                m *= slide_matrices.EyeR;
                break;
            case "eyeline_sita_R":
            case "R_eyeline_oya_R":
            case "Me_Left_Futi":
                m *= slide_matrices.EyeL;
                break;

        }
        matrixStack.MultiplyMatrixLocal(m);
        m = matrixStack.Top;


        // �X���C�_�ɂ��̌^�ύX
        // ����scaling��translation��ύX���Ă͂Ȃ�Ȃ�����
        // matrixStack�ɓK�p���Ȃ��B
        switch (tmo_node.Name)
        {
            case "W_Spine_Dummy":
                Scale1(ref m, slide_matrices.SpineDummy);
                break;
            case "W_Spine1":
            case "W_Spine2":
                Scale1(ref m, slide_matrices.Spine1);
                break;

            case "W_LeftHips_Dummy":
            case "W_RightHips_Dummy":
                Scale1(ref m, slide_matrices.HipsDummy);
                break;
            case "W_LeftUpLeg":
            case "W_RightUpLeg":
                Scale1(ref m, slide_matrices.UpLeg);
                break;
            case "W_LeftUpLegRoll":
            case "W_RightUpLegRoll":
            case "W_LeftLeg":
            case "W_RightLeg":
                Scale1(ref m, slide_matrices.UpLegRoll);
                break;
            case "W_LeftLegRoll":
            case "W_RightLegRoll":
            case "W_LeftFoot":
            case "W_RightFoot":
            case "W_LeftToeBase":
            case "W_RightToeBase":
                Scale1(ref m, slide_matrices.LegRoll);
                break;

            case "W_LeftArm_Dummy":
            case "W_RightArm_Dummy":
                Scale1(ref m, slide_matrices.ArmDummy);
                break;
            case "W_LeftArm":
            case "W_RightArm":
            case "W_LeftArmRoll":
            case "W_RightArmRoll":
            case "W_LeftForeArm":
            case "W_RightForeArm":
            case "W_LeftForeArmRoll":
            case "W_RightForeArmRoll":
                Scale1(ref m, slide_matrices.Arm);
                break;

        }

        if (!slide_matrices.Flat())
        {
            switch (tmo_node.Name)
            {
                /*
                case "Chichi_Right1":
                case "Chichi_Right2":
                case "Chichi_Right3":
                case "Chichi_Right4":
                case "Chichi_Right5":
                */
                case "Chichi_Left1":
                case "Chichi_Left2":
                case "Chichi_Left3":
                case "Chichi_Left4":
                case "Chichi_Left5":
                    Scale1(ref m, slide_matrices.Chichi);
                    break;
            }
        }

        tmo_node.combined_matrix = m;

        foreach (TMONode child_node in tmo_node.children)
            UpdateBoneMatrices(child_node, tmo_frame);

        matrixStack.Pop();
    }

    /// <summary>
    /// TSOFile���w��device��ŊJ���܂��B
    /// </summary>
    /// <param name="device">device</param>
    /// <param name="effect">effect</param>
    public void OpenTSOFile(Device device, Effect effect)
    {
        foreach (TSOFile tso in TSOList)
            tso.Open(device, effect);
    }

    /// <summary>
    /// �w�胂�[�V�����t���[���ɐi�݂܂��B
    /// </summary>
    public void SetFrameIndex(int frame_index)
    {
        Debug.Assert(frame_index >= 0);
        if (tmo.frames != null)
        {
            if (frame_index >= tmo.frames.Length)
                this.frame_index = 0;
            else
                this.frame_index = frame_index;
        }
    }

    /// <summary>
    /// ����object��j�����܂��B
    /// </summary>
    public void Dispose()
    {
        foreach (TSOFile tso in TSOList)
            tso.Dispose();
    }
}
}
