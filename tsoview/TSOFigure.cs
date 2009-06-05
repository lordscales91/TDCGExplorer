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
    internal Vector3 center = Vector3.Empty; //中心点
    internal Vector3 translation = Vector3.Empty; //位置

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

    //指定位置にあるtsoの位置を入れ替えます。描画順を変更します。
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

    //nodemapとbone行列を更新します。
    public void UpdateNodeMapAndBoneMatrices()
    {
        nodemap.Clear();
        if (tmo.frames != null)
        foreach (TSOFile tso in TSOList)
            AddNodeMap(tso);

        UpdateBoneMatrices(true);
    }

    //TSOFileに対するnodemapを追加します。
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

    //TMOFileを変更したときに呼ぶ必要があります。
    //frame indexと中心点を設定します。
    protected void UpdateTMO()
    {
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

    //現在のmotion frameを得ます。
    protected TMOFrame GetTMOFrame()
    {
        if (tmo.frames != null)
            return tmo.frames[current_frame_index];
        return null;
    }

    //TSOFileをTSOListに追加します。
    public void AddTSO(TSOFile tso)
    {
        if (tmo.frames != null)
            AddNodeMap(tso);

        current_frame_index = frame_index;

        TMOFrame tmo_frame = GetTMOFrame();
        UpdateBoneMatrices(tso, tmo_frame);

        TSOList.Add(tso);
    }

    //bone行列を更新します。
    //forcedがfalseの場合frame indexに変更なければ更新しません。
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

    public List<TSOFile> LoadTSOFile(string source_file)
    {
        List<TSOFile> tso_list = new List<TSOFile>();
        try
        {
            if (Path.GetExtension(source_file).ToUpper() == ".TSO")
            {
                TSOFile tso = new TSOFile();
                tso.Load(source_file);
                tso_list.Add(tso);
            }
            else if (Directory.Exists(source_file))
            {
                string[] files = Directory.GetFiles(source_file, "*.TSO");
                foreach (string file in files)
                {
                    TSOFile tso = new TSOFile();
                    tso.Load(file);
                    tso_list.Add(tso);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex);
        }
        return tso_list;
    }

    public void LoadTMOFile(string source_file)
    {
        if (File.Exists(source_file))
        try
        {
            tmo.Load(source_file);
            UpdateTMO();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex);
        }
    }

    public void OpenTSOFile(Device device, Effect effect)
    {
        foreach (TSOFile tso in TSOList)
            tso.Open(device, effect);
    }

    public void Dispose()
    {
        foreach (TSOFile tso in TSOList)
            tso.Dispose();
    }
}
}
