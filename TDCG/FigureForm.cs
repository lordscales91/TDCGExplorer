using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
//using System.ComponentModel;
using System.Windows.Forms;
using System.IO;

namespace TDCG
{
    /// <summary>
    /// フィギュア情報を扱うフォーム
    /// </summary>
public class FigureForm : Form
{
    private Button btnDump;
    private Button btnUp;
    private Button btnDown;
    private ListView lvTSOFiles;
    private ListView lvSubScripts;
    private ColumnHeader columnHeader1;
    private ColumnHeader columnHeader2;
    private ColumnHeader columnHeader3;
    private TrackBar tbSlideEye;
    private TrackBar tbSlideLeg;
    private TrackBar tbSlideArm;
    private TrackBar tbSlideWaist;
    private TrackBar tbSlideBust;
    private Label lbSlideEye;
    private Label lbSlideLeg;
    private Label lbSlideArm;
    private Label lbSlideWaist;
    private Label lbSlideBust;
    private Label lbSlideTall;
    private TrackBar tbSlideTall;
    private ListView lvFrames;
    private ColumnHeader columnHeader4;
    private DataGridView gvShaderParams;

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
    private TSOFile tso = null;
    private Shader shader = null;

    /// <summary>
    /// フィギュア情報を削除します。
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
    /// フィギュアをUIに設定します。
    /// </summary>
    /// <param name="fig">フィギュア</param>
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
    /// tsoをUIに設定します。
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
        this.shader = shader;
        gvShaderParams.DataSource = shader.shader_parameters;
    }

    private void InitializeComponent()
    {
        this.btnDump = new System.Windows.Forms.Button();
        this.btnUp = new System.Windows.Forms.Button();
        this.btnDown = new System.Windows.Forms.Button();
        this.lvTSOFiles = new System.Windows.Forms.ListView();
        this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
        this.lvSubScripts = new System.Windows.Forms.ListView();
        this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
        this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
        this.gvShaderParams = new System.Windows.Forms.DataGridView();
        this.tbSlideEye = new System.Windows.Forms.TrackBar();
        this.tbSlideLeg = new System.Windows.Forms.TrackBar();
        this.tbSlideArm = new System.Windows.Forms.TrackBar();
        this.tbSlideWaist = new System.Windows.Forms.TrackBar();
        this.tbSlideBust = new System.Windows.Forms.TrackBar();
        this.lbSlideEye = new System.Windows.Forms.Label();
        this.lbSlideLeg = new System.Windows.Forms.Label();
        this.lbSlideArm = new System.Windows.Forms.Label();
        this.lbSlideWaist = new System.Windows.Forms.Label();
        this.lbSlideBust = new System.Windows.Forms.Label();
        this.lbSlideTall = new System.Windows.Forms.Label();
        this.tbSlideTall = new System.Windows.Forms.TrackBar();
        this.lvFrames = new System.Windows.Forms.ListView();
        this.columnHeader4 = new System.Windows.Forms.ColumnHeader();
        ((System.ComponentModel.ISupportInitialize)(this.gvShaderParams)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.tbSlideEye)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.tbSlideLeg)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.tbSlideArm)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.tbSlideWaist)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.tbSlideBust)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.tbSlideTall)).BeginInit();
        this.SuspendLayout();
        // 
        // btnDump
        // 
        this.btnDump.Location = new System.Drawing.Point(523, 528);
        this.btnDump.Name = "btnDump";
        this.btnDump.Size = new System.Drawing.Size(75, 23);
        this.btnDump.TabIndex = 0;
        this.btnDump.Text = "&Dump";
        this.btnDump.UseVisualStyleBackColor = true;
        this.btnDump.Click += new System.EventHandler(this.btnDump_Click);
        // 
        // btnUp
        // 
        this.btnUp.Location = new System.Drawing.Point(198, 12);
        this.btnUp.Name = "btnUp";
        this.btnUp.Size = new System.Drawing.Size(50, 23);
        this.btnUp.TabIndex = 1;
        this.btnUp.Text = "&Up";
        this.btnUp.UseVisualStyleBackColor = true;
        this.btnUp.Click += new System.EventHandler(this.btnUp_Click);
        // 
        // btnDown
        // 
        this.btnDown.Location = new System.Drawing.Point(198, 41);
        this.btnDown.Name = "btnDown";
        this.btnDown.Size = new System.Drawing.Size(50, 23);
        this.btnDown.TabIndex = 2;
        this.btnDown.Text = "&Down";
        this.btnDown.UseVisualStyleBackColor = true;
        this.btnDown.Click += new System.EventHandler(this.btnDown_Click);
        // 
        // lvTSOFiles
        // 
        this.lvTSOFiles.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
        this.lvTSOFiles.FullRowSelect = true;
        this.lvTSOFiles.GridLines = true;
        this.lvTSOFiles.HideSelection = false;
        this.lvTSOFiles.Location = new System.Drawing.Point(12, 12);
        this.lvTSOFiles.MultiSelect = false;
        this.lvTSOFiles.Name = "lvTSOFiles";
        this.lvTSOFiles.Size = new System.Drawing.Size(180, 200);
        this.lvTSOFiles.TabIndex = 3;
        this.lvTSOFiles.UseCompatibleStateImageBehavior = false;
        this.lvTSOFiles.View = System.Windows.Forms.View.Details;
        this.lvTSOFiles.SelectedIndexChanged += new System.EventHandler(this.lvTSOFiles_SelectedIndexChanged);
        // 
        // columnHeader1
        // 
        this.columnHeader1.Text = "Name";
        // 
        // lvSubScripts
        // 
        this.lvSubScripts.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader2,
            this.columnHeader3});
        this.lvSubScripts.FullRowSelect = true;
        this.lvSubScripts.GridLines = true;
        this.lvSubScripts.HideSelection = false;
        this.lvSubScripts.Location = new System.Drawing.Point(12, 218);
        this.lvSubScripts.MultiSelect = false;
        this.lvSubScripts.Name = "lvSubScripts";
        this.lvSubScripts.Size = new System.Drawing.Size(180, 304);
        this.lvSubScripts.TabIndex = 4;
        this.lvSubScripts.UseCompatibleStateImageBehavior = false;
        this.lvSubScripts.View = System.Windows.Forms.View.Details;
        this.lvSubScripts.SelectedIndexChanged += new System.EventHandler(this.lvSubScripts_SelectedIndexChanged);
        // 
        // columnHeader2
        // 
        this.columnHeader2.Text = "Name";
        // 
        // columnHeader3
        // 
        this.columnHeader3.Text = "File";
        // 
        // gvShaderParams
        // 
        this.gvShaderParams.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
        this.gvShaderParams.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        this.gvShaderParams.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
        this.gvShaderParams.Location = new System.Drawing.Point(198, 218);
        this.gvShaderParams.Name = "gvShaderParams";
        this.gvShaderParams.RowTemplate.Height = 21;
        this.gvShaderParams.Size = new System.Drawing.Size(400, 304);
        this.gvShaderParams.TabIndex = 5;
        // 
        // tbSlideEye
        // 
        this.tbSlideEye.Location = new System.Drawing.Point(604, 473);
        this.tbSlideEye.Maximum = 20;
        this.tbSlideEye.Name = "tbSlideEye";
        this.tbSlideEye.Size = new System.Drawing.Size(168, 45);
        this.tbSlideEye.TabIndex = 6;
        this.tbSlideEye.ValueChanged += new System.EventHandler(this.tbSlideEye_ValueChanged);
        // 
        // tbSlideLeg
        // 
        this.tbSlideLeg.Location = new System.Drawing.Point(604, 269);
        this.tbSlideLeg.Maximum = 20;
        this.tbSlideLeg.Name = "tbSlideLeg";
        this.tbSlideLeg.Size = new System.Drawing.Size(168, 45);
        this.tbSlideLeg.TabIndex = 7;
        this.tbSlideLeg.ValueChanged += new System.EventHandler(this.tbSlideLeg_ValueChanged);
        // 
        // tbSlideArm
        // 
        this.tbSlideArm.Location = new System.Drawing.Point(604, 218);
        this.tbSlideArm.Maximum = 20;
        this.tbSlideArm.Name = "tbSlideArm";
        this.tbSlideArm.Size = new System.Drawing.Size(168, 45);
        this.tbSlideArm.TabIndex = 8;
        this.tbSlideArm.ValueChanged += new System.EventHandler(this.tbSlideArm_ValueChanged);
        // 
        // tbSlideWaist
        // 
        this.tbSlideWaist.Location = new System.Drawing.Point(604, 320);
        this.tbSlideWaist.Maximum = 20;
        this.tbSlideWaist.Name = "tbSlideWaist";
        this.tbSlideWaist.Size = new System.Drawing.Size(168, 45);
        this.tbSlideWaist.TabIndex = 9;
        this.tbSlideWaist.ValueChanged += new System.EventHandler(this.tbSlideWaist_ValueChanged);
        // 
        // tbSlideBust
        // 
        this.tbSlideBust.Location = new System.Drawing.Point(604, 371);
        this.tbSlideBust.Maximum = 20;
        this.tbSlideBust.Name = "tbSlideBust";
        this.tbSlideBust.Size = new System.Drawing.Size(168, 45);
        this.tbSlideBust.TabIndex = 10;
        this.tbSlideBust.ValueChanged += new System.EventHandler(this.tbSlideBust_ValueChanged);
        // 
        // lbSlideEye
        // 
        this.lbSlideEye.AutoSize = true;
        this.lbSlideEye.Location = new System.Drawing.Point(604, 458);
        this.lbSlideEye.Name = "lbSlideEye";
        this.lbSlideEye.Size = new System.Drawing.Size(24, 12);
        this.lbSlideEye.TabIndex = 11;
        this.lbSlideEye.Text = "Eye";
        // 
        // lbSlideLeg
        // 
        this.lbSlideLeg.AutoSize = true;
        this.lbSlideLeg.Location = new System.Drawing.Point(604, 254);
        this.lbSlideLeg.Name = "lbSlideLeg";
        this.lbSlideLeg.Size = new System.Drawing.Size(23, 12);
        this.lbSlideLeg.TabIndex = 12;
        this.lbSlideLeg.Text = "Leg";
        // 
        // lbSlideArm
        // 
        this.lbSlideArm.AutoSize = true;
        this.lbSlideArm.Location = new System.Drawing.Point(605, 206);
        this.lbSlideArm.Name = "lbSlideArm";
        this.lbSlideArm.Size = new System.Drawing.Size(26, 12);
        this.lbSlideArm.TabIndex = 13;
        this.lbSlideArm.Text = "Arm";
        // 
        // lbSlideWaist
        // 
        this.lbSlideWaist.AutoSize = true;
        this.lbSlideWaist.Location = new System.Drawing.Point(604, 305);
        this.lbSlideWaist.Name = "lbSlideWaist";
        this.lbSlideWaist.Size = new System.Drawing.Size(33, 12);
        this.lbSlideWaist.TabIndex = 14;
        this.lbSlideWaist.Text = "Waist";
        // 
        // lbSlideBust
        // 
        this.lbSlideBust.AutoSize = true;
        this.lbSlideBust.Location = new System.Drawing.Point(604, 356);
        this.lbSlideBust.Name = "lbSlideBust";
        this.lbSlideBust.Size = new System.Drawing.Size(29, 12);
        this.lbSlideBust.TabIndex = 15;
        this.lbSlideBust.Text = "Bust";
        // 
        // lbSlideTall
        // 
        this.lbSlideTall.AutoSize = true;
        this.lbSlideTall.Location = new System.Drawing.Point(604, 407);
        this.lbSlideTall.Name = "lbSlideTall";
        this.lbSlideTall.Size = new System.Drawing.Size(24, 12);
        this.lbSlideTall.TabIndex = 17;
        this.lbSlideTall.Text = "Tall";
        // 
        // tbSlideTall
        // 
        this.tbSlideTall.Location = new System.Drawing.Point(604, 422);
        this.tbSlideTall.Maximum = 20;
        this.tbSlideTall.Name = "tbSlideTall";
        this.tbSlideTall.Size = new System.Drawing.Size(168, 45);
        this.tbSlideTall.TabIndex = 16;
        this.tbSlideTall.ValueChanged += new System.EventHandler(this.tbSlideTall_ValueChanged);
        // 
        // lvFrames
        // 
        this.lvFrames.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader4});
        this.lvFrames.FullRowSelect = true;
        this.lvFrames.GridLines = true;
        this.lvFrames.HideSelection = false;
        this.lvFrames.Location = new System.Drawing.Point(254, 12);
        this.lvFrames.MultiSelect = false;
        this.lvFrames.Name = "lvFrames";
        this.lvFrames.Size = new System.Drawing.Size(344, 200);
        this.lvFrames.TabIndex = 18;
        this.lvFrames.UseCompatibleStateImageBehavior = false;
        this.lvFrames.View = System.Windows.Forms.View.Details;
        this.lvFrames.SelectedIndexChanged += new System.EventHandler(this.lvFrames_SelectedIndexChanged);
        // 
        // columnHeader4
        // 
        this.columnHeader4.Text = "Name";
        // 
        // FigureForm
        // 
        this.ClientSize = new System.Drawing.Size(784, 563);
        this.Controls.Add(this.lvFrames);
        this.Controls.Add(this.lbSlideEye);
        this.Controls.Add(this.tbSlideEye);
        this.Controls.Add(this.lbSlideTall);
        this.Controls.Add(this.tbSlideTall);
        this.Controls.Add(this.lbSlideBust);
        this.Controls.Add(this.lbSlideWaist);
        this.Controls.Add(this.lbSlideArm);
        this.Controls.Add(this.lbSlideLeg);
        this.Controls.Add(this.tbSlideBust);
        this.Controls.Add(this.tbSlideWaist);
        this.Controls.Add(this.tbSlideArm);
        this.Controls.Add(this.tbSlideLeg);
        this.Controls.Add(this.gvShaderParams);
        this.Controls.Add(this.lvSubScripts);
        this.Controls.Add(this.lvTSOFiles);
        this.Controls.Add(this.btnDown);
        this.Controls.Add(this.btnUp);
        this.Controls.Add(this.btnDump);
        this.Name = "FigureForm";
        this.Text = "TSOGrid";
        this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FigureForm_FormClosing);
        ((System.ComponentModel.ISupportInitialize)(this.gvShaderParams)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.tbSlideEye)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.tbSlideLeg)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.tbSlideArm)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.tbSlideWaist)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.tbSlideBust)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.tbSlideTall)).EndInit();
        this.ResumeLayout(false);
        this.PerformLayout();

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
