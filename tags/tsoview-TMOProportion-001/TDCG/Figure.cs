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
    internal TMOFile tmo = null;

    internal Vector3 center = Vector3.Empty;
    /// <summary>
    /// ���S���W
    /// </summary>
    public Vector3 Center
    {
        get { return center; }
    }

    internal Vector3 translation = Vector3.Empty;
    /// <summary>
    /// �ړ��ψ�
    /// </summary>
    public Vector3 Translation
    {
        get { return translation; }
    }

    /// <summary>
    /// tmo
    /// </summary>
    public TMOFile Tmo
    {
        get { return tmo; }
        set
        {
            tmo = value;
            UpdateTMO();
        }
    }

    internal Dictionary<TSONode, TMONode> nodemap;

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
    /// </summary>
    public void UpdateNodeMapAndBoneMatrices()
    {
        nodemap.Clear();
        if (tmo.frames != null)
        foreach (TSOFile tso in TSOList)
            AddNodeMap(tso);

        UpdateBoneMatrices(true);
    }

    /// <summary>
    /// TSOFile�ɑ΂���nodemap��ǉ����܂��B
    /// </summary>
    /// <param name="tso">tso</param>
    protected void AddNodeMap(TSOFile tso)
    {
        foreach (TSONode tso_node in tso.nodes)
        {
            TMONode tmo_node;
            if (tmo.nodemap.TryGetValue(tso_node.Name, out tmo_node))
                nodemap.Add(tso_node, tmo_node);
        }
    }

    private MatrixStack matrixStack = null;
    private int frame_index = 0;
    private int current_frame_index = -1;

    /// <summary>
    /// �t�B�M���A�𐶐����܂��B
    /// </summary>
    public Figure()
    {
        tmo = new TMOFile();
        nodemap = new Dictionary<TSONode, TMONode>();
        matrixStack = new MatrixStack();
    }
    
    /// TMOFile��ύX�����Ƃ��ɌĂԕK�v������܂��B
    /// tmo frame index�ƒ��S�_��ݒ肵�܂��B
    protected void UpdateTMO()
    {
        Debug.Assert(tmo != null);
        frame_index = 0;
        current_frame_index = 0;

        TMONode tmo_node;
        if (tmo.nodemap.TryGetValue("|W_Hips", out tmo_node))
        {
            Debug.Assert(tmo_node.frame_matrices.Count > 0);
            Matrix m = tmo_node.frame_matrices[0].m;
            center = new Vector3(m.M41, m.M42, -m.M43);
        }
    }

    /// <summary>
    /// ����tmo frame�ɐi�݂܂��B
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
    /// ���݂�tmo frame�𓾂܂��B
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

    public int GetFrameIndex()
    {
        return current_frame_index;
    }

    /// <summary>
    /// TSOFile��TSOList�ɒǉ����܂��B
    /// </summary>
    /// <param name="tso">tso</param>
    public void AddTSO(TSOFile tso)
    {
        if (tmo.frames != null)
            AddNodeMap(tso);

        current_frame_index = frame_index;

        TMOFrame tmo_frame = GetTMOFrame();
        UpdateBoneMatrices(tso, tmo_frame);

        TSOList.Add(tso);
    }

    /// <summary>
    /// bone�s����X�V���܂��B������tmo�𖳎����܂��B
    /// </summary>
    public void UpdateBoneMatricesWithoutTMO()
    {
        foreach (TSOFile tso in TSOList)
            UpdateBoneMatricesWithoutTMO(tso);
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
        foreach (TSOFile tso in TSOList)
            UpdateBoneMatrices(tso, tmo_frame);
    }
    
    /// <summary>
    /// bone�s����X�V���܂��B������tmo�𖳎����܂��B
    /// </summary>
    protected void UpdateBoneMatricesWithoutTMO(TSOFile tso)
    {
        matrixStack.LoadMatrix(Matrix.Translation(translation));
        UpdateBoneMatricesWithoutTMO(tso.nodes[0]);
    }

    /// <summary>
    /// bone�s����X�V���܂��B
    /// </summary>
    protected void UpdateBoneMatrices(TSOFile tso, TMOFrame tmo_frame)
    {
        matrixStack.LoadMatrix(Matrix.Translation(translation));
        UpdateBoneMatrices(tso.nodes[0], tmo_frame);
    }

    /// <summary>
    /// bone�s����X�V���܂��B������tmo�𖳎����܂��B
    /// </summary>
    protected void UpdateBoneMatricesWithoutTMO(TSONode tso_node)
    {
        matrixStack.Push();

        matrixStack.MultiplyMatrixLocal(tso_node.TransformationMatrix);
        tso_node.combined_matrix = matrixStack.Top;

        foreach (TSONode child_node in tso_node.child_nodes)
            UpdateBoneMatricesWithoutTMO(child_node);

        matrixStack.Pop();
    }

    /// <summary>
    /// bone�s����X�V���܂��B
    /// </summary>
    protected void UpdateBoneMatrices(TSONode tso_node, TMOFrame tmo_frame)
    {
        matrixStack.Push();

        if (tmo_frame != null)
        {
            // TMO animation
            TMONode tmo_node;
            if (nodemap.TryGetValue(tso_node, out tmo_node))
                tso_node.TransformationMatrix = tmo_frame.matrices[tmo_node.ID].m;
        }

        matrixStack.MultiplyMatrixLocal(tso_node.TransformationMatrix);
        tso_node.combined_matrix = matrixStack.Top;

        foreach (TSONode child_node in tso_node.child_nodes)
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

    private FigureMotion motion = new FigureMotion();
    
    /// <summary>
    /// �t�B�M���A���[�V����
    /// </summary>
    public FigureMotion Motion
    {
        get { return motion; }
    }

    /// <summary>
    /// �t�B�M���A���[�V������ݒ肵�܂��B
    /// </summary>
    /// <param name="frame_index">�t���[���ԍ�</param>
    /// <param name="tmo">tmo</param>
    public void SetMotion(int frame_index, TMOFile tmo)
    {
        motion.Add(frame_index, tmo);
    }

    /// <summary>
    /// ���̃��[�V�����t���[���ɐi�݂܂��B
    /// </summary>
    public void NextFrame()
    {
        if (motion.Count != 0)
        {
            TMOFile tmo = motion.GetTMO();
            if (tmo != Tmo)
            {
                Tmo = tmo;
                UpdateNodeMapAndBoneMatrices();
            }
            motion.NextFrame();
        }
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