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
    /// tmo���ǂݍ��܂�Ă��Ȃ��ꍇ�͐擪��tso���琶�����܂��B
    /// </summary>
    public void UpdateNodeMapAndBoneMatrices()
    {
        if (tmo.frames == null)
            if (TSOList.Count != 0)
                Tmo = GenerateTMOFromTSO(TSOList[0]);

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
            if (tmo.nodemap.TryGetValue(tso_node.Name, out tmo_node))
                nodemap.Add(tso_node, tmo_node);
        }
    }

    private MatrixStack matrixStack = null;
    private int frame_index = 0;
    private int current_frame_index = 0;

    /// <summary>
    /// �t�B�M���A�𐶐����܂��B
    /// </summary>
    public Figure()
    {
        tmo = new TMOFile();
        nodemap = new Dictionary<TSONode, TMONode>();
        matrixStack = new MatrixStack();
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
        Debug.Assert(tmo != null);
        TMONode tmo_node;
        if (tmo.nodemap.TryGetValue("|W_Hips", out tmo_node))
        {
            Debug.Assert(tmo_node.frame_matrices.Count > 0);
            Matrix m = tmo_node.frame_matrices[0].m;
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
    /// tso����tmo�𐶐����܂��B
    /// </summary>
    /// <param name="tso">tso</param>
    public static TMOFile GenerateTMOFromTSO(TSOFile tso)
    {
        TMOFile tmo = new TMOFile();

        int node_count = tso.nodes.Length;
        tmo.nodes = new TMONode[node_count];

        for (int i = 0; i < node_count; i++)
        {
            string name = tso.nodes[i].Name;
            tmo.nodes[i] = new TMONode(i, name);
        }

        tmo.GenerateNodemapAndTree();

        int frame_count = 1;
        tmo.frames = new TMOFrame[frame_count];

        for (int i = 0; i < frame_count; i++)
        {
            tmo.frames[i] = new TMOFrame();
            tmo.frames[i].id = i;

            int matrix_count = node_count;
            tmo.frames[i].matrices = new TMOMat[matrix_count];

            for (int j = 0; j < matrix_count; j++)
            {
                TMOMat mat = tmo.frames[i].matrices[j] = new TMOMat();
                mat.m = tso.nodes[j].TransformationMatrix;
                tmo.nodes[j].frame_matrices.Add(mat);
            }
        }

        return tmo;
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
        matrixStack.LoadMatrix(Matrix.Translation(translation));
        UpdateBoneMatrices(tmo.nodes[0], tmo_frame);
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
        matrixStack.MultiplyMatrixLocal(tmo_node.TransformationMatrix);
        tmo_node.combined_matrix = matrixStack.Top;

        foreach (TMONode child_node in tmo_node.child_nodes)
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