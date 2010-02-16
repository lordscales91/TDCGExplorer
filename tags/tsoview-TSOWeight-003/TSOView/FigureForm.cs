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
    /// �t�B�M���A���������t�H�[��
    /// </summary>
public partial class FigureForm : Form
{
    /// <summary>
    /// �t�B�M���A���t�H�[���𐶐����܂��B
    /// </summary>
    public FigureForm()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Esc�������Ɣ����܂��B
    /// </summary>
    /// <param name="e">�C�x���g����</param>
    protected override void OnKeyPress(System.Windows.Forms.KeyPressEventArgs e)
    {
        if ((int)(byte)e.KeyChar == (int)System.Windows.Forms.Keys.Escape)
            this.Dispose(); // Esc was pressed
    }

    private Figure fig = null;
    private TSOFile tso = null;
    private Shader shader = null;

    /// <summary>
    /// �t�B�M���A�����폜���܂��B
    /// </summary>
    public void Clear()
    {
        gvShaderParams.DataSource = null;
        this.shader = null;
        lvSubScripts.Items.Clear();
        this.tso = null;
        lvTSOFiles.Items.Clear();
        this.fig = null;
    }

    /// <summary>
    /// �t�B�M���A��UI�ɐݒ肵�܂��B
    /// </summary>
    /// <param name="fig">�t�B�M���A</param>
    public void SetFigure(Figure fig)
    {
        this.fig = fig;

        this.tbSlideArm.Value = (int)(fig.slide_matrices.ArmRatio * (float)tbSlideArm.Maximum);
        this.tbSlideLeg.Value = (int)(fig.slide_matrices.LegRatio * (float)tbSlideLeg.Maximum);
        this.tbSlideWaist.Value = (int)(fig.slide_matrices.WaistRatio * (float)tbSlideWaist.Maximum);
        this.tbSlideBust.Value = (int)(fig.slide_matrices.BustRatio * (float)tbSlideBust.Maximum);
        this.tbSlideTall.Value = (int)(fig.slide_matrices.TallRatio * (float)tbSlideTall.Maximum);
        this.tbSlideEye.Value = (int)(fig.slide_matrices.EyeRatio * (float)tbSlideEye.Maximum);

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

    /// <summary>
    /// tso��UI�ɐݒ肵�܂��B
    /// </summary>
    /// <param name="tso">tso</param>
    public void SetTSOFile(TSOFile tso)
    {
        this.tso = tso;
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

    /// <summary>
    /// �V�F�[�_�ݒ��UI�ɐݒ肵�܂��B
    /// </summary>
    /// <param name="shader">�V�F�[�_�ݒ�</param>
    public void SetShader(Shader shader)
    {
        this.shader = shader;
        gvShaderParams.DataSource = shader.shader_parameters;
    }

    private void btnDump_Click(object sender, EventArgs e)
    {
        if (shader == null)
            return;
        Console.WriteLine("-- dump shader parameters --");
        foreach (ShaderParameter param in shader.shader_parameters)
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
        SetFigure(fig);
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
        SetFigure(fig);
        ListViewItem li = lvTSOFiles.Items[li_idx_next];
        li.Selected = true;
    }

    private void lvTSOFiles_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (lvTSOFiles.SelectedItems.Count == 0)
            return;
        ListViewItem li = lvTSOFiles.SelectedItems[0];
        TSOFile tso = li.Tag as TSOFile;
        SetTSOFile(tso);
    }

    private void lvSubScripts_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (lvSubScripts.SelectedItems.Count == 0)
            return;
        ListViewItem li = lvSubScripts.SelectedItems[0];
        TSOSubScript sub_script = li.Tag as TSOSubScript;
        SetShader(sub_script.shader);
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
