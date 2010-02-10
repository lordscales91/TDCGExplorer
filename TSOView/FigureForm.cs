using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
//using System.ComponentModel;
using System.Windows.Forms;
using System.IO;
using TDCG;

namespace TSOView
{
    /// <summary>
    /// フィギュア情報を扱うフォーム
    /// </summary>
public partial class FigureForm : Form
{
    /// <summary>
    /// フィギュア情報フォームを生成します。
    /// </summary>
    public FigureForm()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Escを押すと抜けます。
    /// </summary>
    /// <param name="e">イベント引数</param>
    protected override void OnKeyPress(System.Windows.Forms.KeyPressEventArgs e)
    {
        if ((int)(byte)e.KeyChar == (int)System.Windows.Forms.Keys.Escape)
            this.Dispose(); // Esc was pressed
    }

    private Figure fig = null;
    public TSOFile selected_tso = null;
    public Shader selected_shader = null;

    /// <summary>
    /// フィギュア情報を削除します。
    /// </summary>
    public void Clear()
    {
        gvShaderParams.DataSource = null;
        selected_shader = null;
        lvSubScripts.Items.Clear();
        selected_tso = null;
        lvTSOFiles.Items.Clear();
        this.fig = null;
    }

    /// <summary>
    /// フィギュア
    /// </summary>
    public Figure Figure
    {
        get
        {
            return fig;
        }
        set
        {
            fig = value;
            AssignFigure(fig);
        }
    }

    void AssignFigure(Figure fig)
    {
        this.tbSlideArm.Value = (int)(fig.slide_matrices.ArmRatio * (float)tbSlideArm.Maximum);
        this.tbSlideLeg.Value = (int)(fig.slide_matrices.LegRatio * (float)tbSlideLeg.Maximum);
        this.tbSlideWaist.Value = (int)(fig.slide_matrices.WaistRatio * (float)tbSlideWaist.Maximum);
        this.tbSlideBust.Value = (int)(fig.slide_matrices.BustRatio * (float)tbSlideBust.Maximum);
        this.tbSlideTall.Value = (int)(fig.slide_matrices.TallRatio * (float)tbSlideTall.Maximum);
        this.tbSlideEye.Value = (int)(fig.slide_matrices.EyeRatio * (float)tbSlideEye.Maximum);

        AssignTSOFiles(fig);
    }

    private void AssignTSOFiles(Figure fig)
    {
        lvTSOFiles.Items.Clear();
        for (int i = 0; i < fig.TSOList.Count; i++)
        {
            TSOFile tso = fig.TSOList[i];
            ListViewItem li = new ListViewItem("TSO #" + i.ToString());
            li.Tag = tso;
            lvTSOFiles.Items.Add(li);
        }
        lvTSOFiles.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
    }

    private void AssignTSOFile(TSOFile tso)
    {
        AssignSubScripts(tso);
        AssignNodes(tso);
        AssignFrames(tso);
    }

    private void AssignSubScripts(TSOFile tso)
    {
        lvSubScripts.Items.Clear();
        foreach (TSOSubScript sub_script in tso.sub_scripts)
        {
            ListViewItem li = new ListViewItem(sub_script.Name);
            li.SubItems.Add(sub_script.File);
            li.Tag = sub_script;
            lvSubScripts.Items.Add(li);
        }
        lvSubScripts.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
    }

    private void AssignNodes(TSOFile tso)
    {
        lvNodes.Items.Clear();
        foreach (TSONode node in tso.nodes)
        {
            ListViewItem li = new ListViewItem(node.Name);
            li.Tag = node;
            lvNodes.Items.Add(li);
        }
        lvNodes.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
    }

    private void AssignFrames(TSOFile tso)
    {
        lvFrames.Items.Clear();
        foreach (TSOFrame frame in tso.frames)
        {
            ListViewItem li = new ListViewItem(frame.name);
            li.Tag = frame;
            lvFrames.Items.Add(li);
        }
        lvFrames.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
    }

    /// <summary>
    /// シェーダ設定をUIに設定します。
    /// </summary>
    /// <param name="shader">シェーダ設定</param>
    public void SetShader(Shader shader)
    {
        selected_shader = shader;
        gvShaderParams.DataSource = shader.shader_parameters;
    }

    private void btnDump_Click(object sender, EventArgs e)
    {
        if (selected_shader == null)
            return;
        Console.WriteLine("-- dump shader parameters --");
        foreach (ShaderParameter param in selected_shader.shader_parameters)
            Console.WriteLine("Name {0} F1 {1} F2 {2} F3 {3} F4 {4}", param.Name, param.F1, param.F2, param.F3, param.F4);
    }

    private void btnUp_Click(object sender, EventArgs e)
    {
        if (lvTSOFiles.SelectedItems.Count == 0)
            return;
        int li_idx = lvTSOFiles.SelectedIndices[0];
        int li_idx_prev = li_idx - 1;
        if (li_idx_prev < 0)
            return;
        fig.SwapAt(li_idx_prev, li_idx);
        AssignFigure(fig);
        ListViewItem li = lvTSOFiles.Items[li_idx_prev];
        li.Selected = true;
    }

    private void btnDown_Click(object sender, EventArgs e)
    {
        if (lvTSOFiles.SelectedItems.Count == 0)
            return;
        int li_idx = lvTSOFiles.SelectedIndices[0];
        int li_idx_next = li_idx + 1;
        if (li_idx_next > lvTSOFiles.Items.Count - 1)
            return;
        fig.SwapAt(li_idx, li_idx_next);
        AssignFigure(fig);
        ListViewItem li = lvTSOFiles.Items[li_idx_next];
        li.Selected = true;
    }

    private void lvTSOFiles_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (lvTSOFiles.SelectedItems.Count == 0)
        {
            selected_tso = null;
            return;
        }
        ListViewItem li = lvTSOFiles.SelectedItems[0];
        TSOFile tso = li.Tag as TSOFile;
        selected_tso = tso;
        AssignTSOFile(tso);
    }

    private void lvSubScripts_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (lvSubScripts.SelectedItems.Count == 0)
            return;
        ListViewItem li = lvSubScripts.SelectedItems[0];
        TSOSubScript sub_script = li.Tag as TSOSubScript;
        SetShader(sub_script.shader);
    }

    /// <summary>
    /// フレーム選択時に呼び出されるハンドラ
    /// </summary>
    public event EventHandler FrameEvent;

    /// <summary>
    /// 選択中のフレーム
    /// </summary>
    public TSOFrame selected_frame = null;

    private void lvFrames_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (lvFrames.SelectedItems.Count == 0)
        {
            selected_frame = null;
            return;
        }
        ListViewItem li = lvFrames.SelectedItems[0];
        TSOFrame frame = li.Tag as TSOFrame;
        selected_frame = frame;
        if (FrameEvent != null)
            FrameEvent(this, EventArgs.Empty);
    }

    public void SetFrame(TSOFrame frame)
    {
        foreach (TSOMesh mesh in frame.meshes)
            foreach (int bone_index in mesh.bone_indices)
                ;
    }

    /// <summary>
    /// ノード選択時に呼び出されるハンドラ
    /// </summary>
    public event EventHandler NodeEvent;

    /// <summary>
    /// 選択中のノード
    /// </summary>
    public TSONode selected_node = null;

    private void lvNodes_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (lvNodes.SelectedItems.Count == 0)
        {
            selected_node = null;
            return;
        }
        ListViewItem li = lvNodes.SelectedItems[0];
        TSONode node = li.Tag as TSONode;
        selected_node = node;
        if (NodeEvent != null)
            NodeEvent(this, EventArgs.Empty);
    }

    private void FigureForm_FormClosing(object sender, FormClosingEventArgs e)
    {
        if (e.CloseReason != CloseReason.FormOwnerClosing)
        {
            this.Hide();
            e.Cancel = true;
        }
    }

    private void tbSlideArm_ValueChanged(object sender, EventArgs e)
    {
        if (fig == null)
            return;

        fig.slide_matrices.ArmRatio = tbSlideArm.Value / (float)tbSlideArm.Maximum;
        fig.UpdateBoneMatrices(true);
    }

    private void tbSlideLeg_ValueChanged(object sender, EventArgs e)
    {
        if (fig == null)
            return;

        fig.slide_matrices.LegRatio = tbSlideLeg.Value / (float)tbSlideLeg.Maximum;
        fig.UpdateBoneMatrices(true);
    }

    private void tbSlideWaist_ValueChanged(object sender, EventArgs e)
    {
        if (fig == null)
            return;

        fig.slide_matrices.WaistRatio = tbSlideWaist.Value / (float)tbSlideWaist.Maximum;
        fig.UpdateBoneMatrices(true);
    }

    private void tbSlideBust_ValueChanged(object sender, EventArgs e)
    {
        if (fig == null)
            return;

        fig.slide_matrices.BustRatio = tbSlideBust.Value / (float)tbSlideBust.Maximum;
        fig.UpdateBoneMatrices(true);
    }

    private void tbSlideEye_ValueChanged(object sender, EventArgs e)
    {
        if (fig == null)
            return;

        fig.slide_matrices.EyeRatio = tbSlideEye.Value / (float)tbSlideEye.Maximum;
        fig.UpdateBoneMatrices(true);
    }

    private void tbSlideTall_ValueChanged(object sender, EventArgs e)
    {
        if (fig == null)
            return;

        fig.slide_matrices.TallRatio = tbSlideTall.Value / (float)tbSlideTall.Maximum;
        fig.UpdateBoneMatrices(true);
    }
}
}
