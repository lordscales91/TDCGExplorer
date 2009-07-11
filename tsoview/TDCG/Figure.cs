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
    /// フィギュア
    /// </summary>
public class Figure : IDisposable
{
    /// <summary>
    /// フィギュアが保持しているtsoリスト
    /// </summary>
    public List<TSOFile> TSOList = new List<TSOFile>();
    internal TMOFile tmo = null;

    internal Vector3 center = Vector3.Empty;
    /// <summary>
    /// 中心座標
    /// </summary>
    public Vector3 Center
    {
        get { return center; }
    }

    internal Vector3 translation = Vector3.Empty;
    /// <summary>
    /// 移動変位
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
    /// フィギュアを移動します（相対座標）。
    /// </summary>
    /// <param name="dx">X軸変位</param>
    /// <param name="dy">Y軸変位</param>
    /// <param name="dz">Z軸変位</param>
    public void Move(float dx, float dy, float dz)
    {
        Move(new Vector3(dx, dy, dz));
    }

    /// <summary>
    /// フィギュアを移動します（相対座標）。
    /// </summary>
    /// <param name="delta">変位</param>
    public void Move(Vector3 delta)
    {
        center += delta;
        translation += delta;
        UpdateBoneMatrices(true);
    }

    /// <summary>
    /// 指定位置にあるtsoの位置を入れ替えます。描画順を変更します。
    /// </summary>
    /// <param name="aidx">リスト上の位置a</param>
    /// <param name="bidx">リスト上の位置b</param>
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
    /// nodemapとbone行列を更新します。
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
    /// TSOFileに対するnodemapを追加します。
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
    /// フィギュアを生成します。
    /// </summary>
    public Figure()
    {
        tmo = new TMOFile();
        nodemap = new Dictionary<TSONode, TMONode>();
        matrixStack = new MatrixStack();
    }
    
    /// TMOFileを変更したときに呼ぶ必要があります。
    /// tmo frame indexと中心点を設定します。
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

    /// <summary>
    /// 次のtmo frameに進みます。
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
    /// 現在のtmo frameを得ます。
    /// </summary>
    /// <returns>現在のtmo frame</returns>
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
    /// TSOFileをTSOListに追加します。
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

    /// bone行列を更新します。
    /// forcedがfalseの場合frame indexに変更なければ更新しません。
    public void UpdateBoneMatrices(bool forced)
    {
        if (!forced && frame_index == current_frame_index)
            return;
        current_frame_index = frame_index;

        TMOFrame tmo_frame = GetTMOFrame();
        foreach (TSOFile tso in TSOList)
            UpdateBoneMatrices(tso, tmo_frame);
    }
    
    public void UpdateBoneMatricesWithoutTMO()
    {
        foreach (TSOFile tso in TSOList)
            UpdateBoneMatrices(tso);
    }
    
    /// <summary>
    /// bone行列を更新します。
    /// </summary>
    public void UpdateBoneMatrices()
    {
        UpdateBoneMatrices(false);
    }

    protected void UpdateBoneMatrices(TSOFile tso)
    {
        matrixStack.LoadMatrix(Matrix.Translation(translation));
        UpdateBoneMatrices(tso.nodes[0]);
    }

    protected void UpdateBoneMatrices(TSOFile tso, TMOFrame tmo_frame)
    {
        matrixStack.LoadMatrix(Matrix.Translation(translation));
        UpdateBoneMatrices(tso.nodes[0], tmo_frame);
    }

    protected void UpdateBoneMatrices(TSONode tso_node)
    {
        matrixStack.Push();

        matrixStack.MultiplyMatrixLocal(tso_node.TransformationMatrix);
        tso_node.combined_matrix = matrixStack.Top;

        foreach (TSONode child_node in tso_node.child_nodes)
            UpdateBoneMatrices(child_node);

        matrixStack.Pop();
    }

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
    /// TSOFileを指定device上で開きます。
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
    /// フィギュアモーション
    /// </summary>
    public FigureMotion Motion
    {
        get { return motion; }
    }

    /// <summary>
    /// フィギュアモーションを設定します。
    /// </summary>
    /// <param name="frame_index">フレーム番号</param>
    /// <param name="tmo">tmo</param>
    public void SetMotion(int frame_index, TMOFile tmo)
    {
        motion.Add(frame_index, tmo);
    }

    /// <summary>
    /// 次のモーションフレームに進みます。
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
    /// 指定モーションフレームに進みます。
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

    public void Dispose()
    {
        foreach (TSOFile tso in TSOList)
            tso.Dispose();
    }
}
}
