using System;
using System.Collections.Generic;
using System.Diagnostics;
//using System.Drawing;
using System.Threading;
//using System.ComponentModel;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;
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

    /// <summary>
    /// スライダ変形行列
    /// </summary>
    public SlideMatrices slide_matrices = new SlideMatrices();

    Vector3 center = Vector3.Empty;
    /// <summary>
    /// 中心座標
    /// </summary>
    public Vector3 Center
    {
        get { return center; }
    }

    Vector3 translation = Vector3.Empty;
    /// <summary>
    /// 移動変位
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

    /// tso nodeからtmo nodeを導出する辞書
    public Dictionary<TSONode, TMONode> nodemap;

    private MatrixStack matrixStack = null;
    private int frame_index = 0;
    private int current_frame_index = 0;

    /// <summary>
    /// 体型スクリプトのリスト
    /// </summary>
    public static ProportionList ProportionList { get; set; }

    TPOFileList tpo_list = new TPOFileList();
    /// <summary>
    /// TPOファイルのリスト
    /// </summary>
    public TPOFileList TPOList { get { return tpo_list; } }

    /// <summary>
    /// 体型レシピのファイル名
    /// </summary>
    /// <returns></returns>
    public static string GetTPOConfigPath()
    {
        return Path.Combine(Application.StartupPath, @"TPOConfig.xml");
    }

    /// <summary>
    /// フィギュアを生成します。
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
    /// 体型変形を行います。
    /// </summary>
    public void TransformTpo()
    {
        if (tmo.frames == null)
            return;

        tpo_list.Transform();
    }

    /// <summary>
    /// 体型変形を行います。
    /// </summary>
    /// <param name="frame_index">フレーム番号</param>
    public void TransformTpo(int frame_index)
    {
        if (tmo.frames == null)
            return;

        tpo_list.Transform(frame_index);
    }
    
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
    /// tmoが読み込まれていない場合は先頭のtsoからtmoを生成します。
    /// </summary>
    public void UpdateNodeMapAndBoneMatrices()
    {
        if (tmo.frames == null)
            RegenerateTMO();

        nodemap.Clear();
        if (tmo.frames != null)
        foreach (TSOFile tso in TSOList)
            AddNodeMap(tso);

        UpdateBoneMatrices(true);
    }

    /// <summary>
    /// 先頭のtsoからtmoを生成します。
    /// </summary>
    public void RegenerateTMO()
    {
        if (TSOList.Count != 0)
        {
            Tmo = TSOList[0].GenerateTMO();
            TransformTpo();
        }
    }

    /// <summary>
    /// tsoに対するnodemapを追加します。
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
    /// フレーム番号を0に設定します。
    /// </summary>
    protected void ResetFrameIndex()
    {
        frame_index = 0;
        current_frame_index = 0;
    }

    /// <summary>
    /// 中心点を腰boneの位置に設定します。
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
    /// 次のフレームに進みます。
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
    /// 現在のフレームを得ます。
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
    /// 現在のフレーム番号を得ます。
    /// </summary>
    /// <returns></returns>
    public int GetFrameIndex()
    {
        return current_frame_index;
    }

    /// <summary>
    /// tsoをTSOListに追加します。
    /// </summary>
    /// <param name="tso">tso</param>
    public void AddTSO(TSOFile tso)
    {
        if (tmo.frames != null)
            AddNodeMap(tso);

        TSOList.Add(tso);
    }

    /// <summary>
    /// bone行列を更新します。
    /// ただしtmo frameを無視します。
    /// </summary>
    public void UpdateBoneMatricesWithoutTMOFrame()
    {
        UpdateBoneMatrices(tmo, null);
    }

    /// <summary>
    /// bone行列を更新します。
    /// </summary>
    public void UpdateBoneMatrices()
    {
        UpdateBoneMatrices(false);
    }

    /// <summary>
    /// bone行列を更新します。
    /// </summary>
    /// <param name="forced">falseの場合frame indexに変更なければ更新しません。</param>
    public void UpdateBoneMatrices(bool forced)
    {
        if (!forced && frame_index == current_frame_index)
            return;
        current_frame_index = frame_index;

        TMOFrame tmo_frame = GetTMOFrame();

        UpdateBoneMatrices(tmo, tmo_frame);
    }
    
    /// <summary>
    /// bone行列を更新します。
    /// </summary>
    protected void UpdateBoneMatrices(TMOFile tmo, TMOFrame tmo_frame)
    {
        if (tmo.nodes == null)
            return;

        if (tmo.w_hips_node != null)
        {
            //姉妹スライダによる変形
            Matrix local = Matrix.Scaling(slide_matrices.Local);

            //移動変位を設定
            local.M41 = translation.X;
            local.M42 = translation.Y;
            local.M43 = translation.Z;

            matrixStack.LoadMatrix(local);
            UpdateBoneMatrices(tmo.w_hips_node, tmo_frame);
        }
        foreach (TMONode tmo_node in tmo.root_nodes_except_w_hips)
        {
            //移動変位を設定
            Matrix local = Matrix.Translation(translation);

            matrixStack.LoadMatrix(local);
            UpdateBoneMatricesWithoutSlideMatrices(tmo_node, tmo_frame);
        }
    }

    static Regex re_chichi = new Regex(@"\AChichi");

    /// <summary>
    /// bone行列を更新します。
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

        bool is_chichi = re_chichi.IsMatch(tmo_node.Name);

        if (is_chichi)
        {
            if (slide_matrices.Flat())
                slide_matrices.TransformChichiFlat(tmo_node, ref m);
        }
        else
            slide_matrices.TransformFace(tmo_node, ref m);

        matrixStack.MultiplyMatrixLocal(m);
        m = matrixStack.Top;

        if (is_chichi)
        {
            if (! slide_matrices.Flat())
                slide_matrices.ScaleChichi(ref m);
        }
        else
            slide_matrices.Scale(tmo_node, ref m);

        tmo_node.combined_matrix = m;

        foreach (TMONode child_node in tmo_node.children)
            UpdateBoneMatrices(child_node, tmo_frame);

        matrixStack.Pop();
    }

    /// <summary>
    /// bone行列を更新します（体型変更なし）。
    /// </summary>
    protected void UpdateBoneMatricesWithoutSlideMatrices(TMONode tmo_node, TMOFrame tmo_frame)
    {
        matrixStack.Push();

        if (tmo_frame != null)
        {
            // TMO animation
            tmo_node.TransformationMatrix = tmo_frame.matrices[tmo_node.ID].m;
        }
        Matrix m = tmo_node.TransformationMatrix;

        matrixStack.MultiplyMatrixLocal(m);
        m = matrixStack.Top;

        tmo_node.combined_matrix = m;

        foreach (TMONode child_node in tmo_node.children)
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

    /// <summary>
    /// スキン変形行列の配列を得ます。
    /// </summary>
    /// <param name="sub_mesh">サブメッシュ</param>
    /// <returns>スキン変形行列の配列</returns>
    public Matrix[] ClipBoneMatrices(TSOSubMesh sub_mesh)
    {
        Matrix[] clipped_boneMatrices = new Matrix[sub_mesh.maxPalettes];

        for (int numPalettes = 0; numPalettes < sub_mesh.maxPalettes; numPalettes++)
        {
            TSONode tso_node = sub_mesh.GetBone(numPalettes);
            TMONode tmo_node;
            if (nodemap.TryGetValue(tso_node, out tmo_node))
                clipped_boneMatrices[numPalettes] = tso_node.offset_matrix * tmo_node.combined_matrix;
        }
        return clipped_boneMatrices;
    }

    /// <summary>
    /// 内部objectを破棄します。
    /// </summary>
    public void Dispose()
    {
        foreach (TSOFile tso in TSOList)
            tso.Dispose();
    }
}
}
