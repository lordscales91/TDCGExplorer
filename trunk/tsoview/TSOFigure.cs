using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
//using System.ComponentModel;
using System.Windows.Forms;
using System.IO;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using Direct3D=Microsoft.DirectX.Direct3D;

namespace TAHdecrypt
{

public class TSOFigure : IDisposable
{
    internal List<TSOFile> TSOList = new List<TSOFile>();
    internal TMOFile tmo = null;
    internal Vector3 center = Vector3.Empty; //���S�_
    internal Vector3 translation = Vector3.Empty; //�ʒu

    public Vector3 Center
    {
        get { return center; }
    }

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

    public void Move(float dx, float dy, float dz)
    {
        Move(new Vector3(dx, dy, dz));
    }

    public void Move(Vector3 delta)
    {
        center += delta;
        translation += delta;
        UpdateBoneMatrices(true);
    }

    //�w��ʒu�ɂ���tso�̈ʒu�����ւ��܂��B�`�揇��ύX���܂��B
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

    //nodemap��bone�s����X�V���܂��B
    public void UpdateNodeMapAndBoneMatrices()
    {
        nodemap.Clear();
        if (tmo.frames != null)
        foreach (TSOFile tso in TSOList)
            AddNodeMap(tso);

        UpdateBoneMatrices(true);
    }

    //TSOFile�ɑ΂���nodemap��ǉ����܂��B
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

    public TSOFigure()
    {
        tmo = new TMOFile();
        nodemap = new Dictionary<TSONode, TMONode>();
        matrixStack = new MatrixStack();
    }

    //TMOFile��ύX�����Ƃ��ɌĂԕK�v������܂��B
    //frame index�ƒ��S�_��ݒ肵�܂��B
    protected void UpdateTMO()
    {
        Debug.Assert(tmo != null);
        frame_index = 0;
        current_frame_index = 0;

        TMONode tmo_node;
        if (tmo.nodemap.TryGetValue("|W_Hips", out tmo_node))
        {
            Matrix m = tmo_node.frame_matrices[0].m;
            center = new Vector3(m.M41, m.M42, -m.M43) + translation;
        }
    }

    public void NextTMOFrame()
    {
        if (tmo.frames != null)
        {
            frame_index++;
            if (frame_index >= tmo.frames.Length)
                frame_index = 0;
        }
    }

    //���݂�motion frame�𓾂܂��B
    protected TMOFrame GetTMOFrame()
    {
        if (tmo.frames != null)
            return tmo.frames[current_frame_index];
        return null;
    }

    //TSOFile��TSOList�ɒǉ����܂��B
    public void AddTSO(TSOFile tso)
    {
        if (tmo.frames != null)
            AddNodeMap(tso);

        current_frame_index = frame_index;

        TMOFrame tmo_frame = GetTMOFrame();
        UpdateBoneMatrices(tso, tmo_frame);

        TSOList.Add(tso);
    }

    //bone�s����X�V���܂��B
    //forced��false�̏ꍇframe index�ɕύX�Ȃ���΍X�V���܂���B
    public void UpdateBoneMatrices(bool forced)
    {
        if (!forced && frame_index == current_frame_index)
            return;
        current_frame_index = frame_index;

        TMOFrame tmo_frame = GetTMOFrame();
        foreach (TSOFile tso in TSOList)
            UpdateBoneMatrices(tso, tmo_frame);
    }
    public void UpdateBoneMatrices()
    {
        UpdateBoneMatrices(false);
    }

    protected void UpdateBoneMatrices(TSOFile tso, TMOFrame tmo_frame)
    {
        matrixStack.LoadMatrix(Matrix.Translation(translation));
        UpdateBoneMatrices(tso.nodes[0], tmo_frame);
    }

    protected void UpdateBoneMatrices(TSONode tso_node, TMOFrame tmo_frame)
    {
        matrixStack.Push();

        Matrix transform;

        if (tmo_frame != null)
        {
            // TMO animation
            TMONode tmo_node;
            if (nodemap.TryGetValue(tso_node, out tmo_node))
                transform = tmo_frame.matrices[tmo_node.ID].m;
            else
                transform = tso_node.transformation_matrix;
        }
        else
            transform = tso_node.transformation_matrix;

        matrixStack.MultiplyMatrixLocal(transform);
        tso_node.combined_matrix = matrixStack.Top;

        foreach (TSONode child_node in tso_node.child_nodes)
            UpdateBoneMatrices(child_node, tmo_frame);

        matrixStack.Pop();
    }

    public void OpenTSOFile(Device device, Effect effect)
    {
        foreach (TSOFile tso in TSOList)
            tso.Open(device, effect);
    }

    private TSOFigureMotion motion = new TSOFigureMotion();

    public TSOFigureMotion Motion
    {
        get { return motion; }
    }

    public void SetMotion(int frame_index, TMOFile tmo)
    {
        motion.Add(frame_index, tmo);
    }

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

    public void Dispose()
    {
        foreach (TSOFile tso in TSOList)
            tso.Dispose();
    }
}
}
