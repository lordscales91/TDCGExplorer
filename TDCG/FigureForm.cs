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
    private Button btnUp;
    private Button btnDown;
    private ListView lvTSOFiles;
    private ColumnHeader columnHeader1;
    private TabControl tabControl1;
    private TabPage tpSliders;
    private Label lbSlideEye;
    private TrackBar tbSlideEye;
    private Label lbSlideTall;
    private TrackBar tbSlideTall;
    private Label lbSlideBust;
    private Label lbSlideWaist;
    private Label lbSlideArm;
    private Label lbSlideLeg;
    private TrackBar tbSlideBust;
    private TrackBar tbSlideWaist;
    private TrackBar tbSlideArm;
    private TrackBar tbSlideLeg;
    private DataGridView gvShaderParams;
    private ListView lvSubScripts;
    private ColumnHeader columnHeader2;
    private ColumnHeader columnHeader3;
    private Button btnDump;
    private TabPage tpFramesAndNodes;
    private ListView lvNodes;
    private ColumnHeader columnHeader5;
    private ListView lvFrames;
    private ColumnHeader columnHeader4;
    private TabPage tpSubScripts;

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
        lvNodes.Items.Clear();
        foreach (TSONode node in tso.nodes)
        {
            ListViewItem li = new ListViewItem(node.Name);
            li.Tag = node;
            lvNodes.Items.Add(li);
        }
        lvNodes.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
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
        this.btnUp = new System.Windows.Forms.Button();
        this.btnDown = new System.Windows.Forms.Button();
        this.lvTSOFiles = new System.Windows.Forms.ListView();
        this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
        this.tabControl1 = new System.Windows.Forms.TabControl();
        this.tpSliders = new System.Windows.Forms.TabPage();
        this.tpSubScripts = new System.Windows.Forms.TabPage();
        this.lbSlideEye = new System.Windows.Forms.Label();
        this.tbSlideEye = new System.Windows.Forms.TrackBar();
        this.lbSlideTall = new System.Windows.Forms.Label();
        this.tbSlideTall = new System.Windows.Forms.TrackBar();
        this.lbSlideBust = new System.Windows.Forms.Label();
        this.lbSlideWaist = new System.Windows.Forms.Label();
        this.lbSlideArm = new System.Windows.Forms.Label();
        this.lbSlideLeg = new System.Windows.Forms.Label();
        this.tbSlideBust = new System.Windows.Forms.TrackBar();
        this.tbSlideWaist = new System.Windows.Forms.TrackBar();
        this.tbSlideArm = new System.Windows.Forms.TrackBar();
        this.tbSlideLeg = new System.Windows.Forms.TrackBar();
        this.gvShaderParams = new System.Windows.Forms.DataGridView();
        this.lvSubScripts = new System.Windows.Forms.ListView();
        this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
        this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
        this.btnDump = new System.Windows.Forms.Button();
        this.tpFramesAndNodes = new System.Windows.Forms.TabPage();
        this.lvNodes = new System.Windows.Forms.ListView();
        this.columnHeader5 = new System.Windows.Forms.ColumnHeader();
        this.lvFrames = new System.Windows.Forms.ListView();
        this.columnHeader4 = new System.Windows.Forms.ColumnHeader();
        this.tabControl1.SuspendLayout();
        this.tpSliders.SuspendLayout();
        this.tpSubScripts.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)(this.tbSlideEye)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.tbSlideTall)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.tbSlideBust)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.tbSlideWaist)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.tbSlideArm)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.tbSlideLeg)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.gvShaderParams)).BeginInit();
        this.tpFramesAndNodes.SuspendLayout();
        this.SuspendLayout();
        // 
        // btnUp
        // 
        this.btnUp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
        this.btnUp.Location = new System.Drawing.Point(86, 518);
        this.btnUp.Name = "btnUp";
        this.btnUp.Size = new System.Drawing.Size(50, 23);
        this.btnUp.TabIndex = 1;
        this.btnUp.Text = "&Up";
        this.btnUp.UseVisualStyleBackColor = true;
        this.btnUp.Click += new System.EventHandler(this.btnUp_Click);
        // 
        // btnDown
        // 
        this.btnDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
        this.btnDown.Location = new System.Drawing.Point(142, 518);
        this.btnDown.Name = "btnDown";
        this.btnDown.Size = new System.Drawing.Size(50, 23);
        this.btnDown.TabIndex = 2;
        this.btnDown.Text = "&Down";
        this.btnDown.UseVisualStyleBackColor = true;
        this.btnDown.Click += new System.EventHandler(this.btnDown_Click);
        // 
        // lvTSOFiles
        // 
        this.lvTSOFiles.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                    | System.Windows.Forms.AnchorStyles.Left)));
        this.lvTSOFiles.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
        this.lvTSOFiles.FullRowSelect = true;
        this.lvTSOFiles.GridLines = true;
        this.lvTSOFiles.HideSelection = false;
        this.lvTSOFiles.Location = new System.Drawing.Point(12, 40);
        this.lvTSOFiles.MultiSelect = false;
        this.lvTSOFiles.Name = "lvTSOFiles";
        this.lvTSOFiles.Size = new System.Drawing.Size(180, 472);
        this.lvTSOFiles.TabIndex = 3;
        this.lvTSOFiles.UseCompatibleStateImageBehavior = false;
        this.lvTSOFiles.View = System.Windows.Forms.View.Details;
        this.lvTSOFiles.SelectedIndexChanged += new System.EventHandler(this.lvTSOFiles_SelectedIndexChanged);
        // 
        // columnHeader1
        // 
        this.columnHeader1.Text = "Name";
        // 
        // tabControl1
        // 
        this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                    | System.Windows.Forms.AnchorStyles.Left)
                    | System.Windows.Forms.AnchorStyles.Right)));
        this.tabControl1.Controls.Add(this.tpSubScripts);
        this.tabControl1.Controls.Add(this.tpFramesAndNodes);
        this.tabControl1.Controls.Add(this.tpSliders);
        this.tabControl1.Location = new System.Drawing.Point(198, 12);
        this.tabControl1.Name = "tabControl1";
        this.tabControl1.SelectedIndex = 0;
        this.tabControl1.Size = new System.Drawing.Size(574, 539);
        this.tabControl1.TabIndex = 20;
        // 
        // tpSliders
        // 
        this.tpSliders.Controls.Add(this.lbSlideEye);
        this.tpSliders.Controls.Add(this.tbSlideEye);
        this.tpSliders.Controls.Add(this.lbSlideTall);
        this.tpSliders.Controls.Add(this.tbSlideTall);
        this.tpSliders.Controls.Add(this.lbSlideBust);
        this.tpSliders.Controls.Add(this.lbSlideWaist);
        this.tpSliders.Controls.Add(this.lbSlideArm);
        this.tpSliders.Controls.Add(this.lbSlideLeg);
        this.tpSliders.Controls.Add(this.tbSlideBust);
        this.tpSliders.Controls.Add(this.tbSlideWaist);
        this.tpSliders.Controls.Add(this.tbSlideArm);
        this.tpSliders.Controls.Add(this.tbSlideLeg);
        this.tpSliders.Location = new System.Drawing.Point(4, 22);
        this.tpSliders.Name = "tpSliders";
        this.tpSliders.Padding = new System.Windows.Forms.Padding(3);
        this.tpSliders.Size = new System.Drawing.Size(566, 513);
        this.tpSliders.TabIndex = 0;
        this.tpSliders.Text = "Sliders";
        this.tpSliders.UseVisualStyleBackColor = true;
        // 
        // tpSubScripts
        // 
        this.tpSubScripts.Controls.Add(this.gvShaderParams);
        this.tpSubScripts.Controls.Add(this.lvSubScripts);
        this.tpSubScripts.Controls.Add(this.btnDump);
        this.tpSubScripts.Location = new System.Drawing.Point(4, 22);
        this.tpSubScripts.Name = "tpSubScripts";
        this.tpSubScripts.Padding = new System.Windows.Forms.Padding(3);
        this.tpSubScripts.Size = new System.Drawing.Size(566, 513);
        this.tpSubScripts.TabIndex = 1;
        this.tpSubScripts.Text = "SubScripts";
        this.tpSubScripts.UseVisualStyleBackColor = true;
        // 
        // lbSlideEye
        // 
        this.lbSlideEye.AutoSize = true;
        this.lbSlideEye.Location = new System.Drawing.Point(6, 321);
        this.lbSlideEye.Name = "lbSlideEye";
        this.lbSlideEye.Size = new System.Drawing.Size(24, 12);
        this.lbSlideEye.TabIndex = 23;
        this.lbSlideEye.Text = "Eye";
        // 
        // tbSlideEye
        // 
        this.tbSlideEye.Location = new System.Drawing.Point(6, 336);
        this.tbSlideEye.Maximum = 20;
        this.tbSlideEye.Name = "tbSlideEye";
        this.tbSlideEye.Size = new System.Drawing.Size(168, 45);
        this.tbSlideEye.TabIndex = 18;
        this.tbSlideEye.ValueChanged += new System.EventHandler(this.tbSlideEye_ValueChanged);
        // 
        // lbSlideTall
        // 
        this.lbSlideTall.AutoSize = true;
        this.lbSlideTall.Location = new System.Drawing.Point(6, 258);
        this.lbSlideTall.Name = "lbSlideTall";
        this.lbSlideTall.Size = new System.Drawing.Size(24, 12);
        this.lbSlideTall.TabIndex = 29;
        this.lbSlideTall.Text = "Tall";
        // 
        // tbSlideTall
        // 
        this.tbSlideTall.Location = new System.Drawing.Point(6, 273);
        this.tbSlideTall.Maximum = 20;
        this.tbSlideTall.Name = "tbSlideTall";
        this.tbSlideTall.Size = new System.Drawing.Size(168, 45);
        this.tbSlideTall.TabIndex = 28;
        this.tbSlideTall.ValueChanged += new System.EventHandler(this.tbSlideTall_ValueChanged);
        // 
        // lbSlideBust
        // 
        this.lbSlideBust.AutoSize = true;
        this.lbSlideBust.Location = new System.Drawing.Point(6, 195);
        this.lbSlideBust.Name = "lbSlideBust";
        this.lbSlideBust.Size = new System.Drawing.Size(29, 12);
        this.lbSlideBust.TabIndex = 27;
        this.lbSlideBust.Text = "Bust";
        // 
        // lbSlideWaist
        // 
        this.lbSlideWaist.AutoSize = true;
        this.lbSlideWaist.Location = new System.Drawing.Point(6, 132);
        this.lbSlideWaist.Name = "lbSlideWaist";
        this.lbSlideWaist.Size = new System.Drawing.Size(33, 12);
        this.lbSlideWaist.TabIndex = 26;
        this.lbSlideWaist.Text = "Waist";
        // 
        // lbSlideArm
        // 
        this.lbSlideArm.AutoSize = true;
        this.lbSlideArm.Location = new System.Drawing.Point(6, 6);
        this.lbSlideArm.Name = "lbSlideArm";
        this.lbSlideArm.Size = new System.Drawing.Size(26, 12);
        this.lbSlideArm.TabIndex = 25;
        this.lbSlideArm.Text = "Arm";
        // 
        // lbSlideLeg
        // 
        this.lbSlideLeg.AutoSize = true;
        this.lbSlideLeg.Location = new System.Drawing.Point(6, 69);
        this.lbSlideLeg.Name = "lbSlideLeg";
        this.lbSlideLeg.Size = new System.Drawing.Size(23, 12);
        this.lbSlideLeg.TabIndex = 24;
        this.lbSlideLeg.Text = "Leg";
        // 
        // tbSlideBust
        // 
        this.tbSlideBust.Location = new System.Drawing.Point(6, 210);
        this.tbSlideBust.Maximum = 20;
        this.tbSlideBust.Name = "tbSlideBust";
        this.tbSlideBust.Size = new System.Drawing.Size(168, 45);
        this.tbSlideBust.TabIndex = 22;
        this.tbSlideBust.ValueChanged += new System.EventHandler(this.tbSlideBust_ValueChanged);
        // 
        // tbSlideWaist
        // 
        this.tbSlideWaist.Location = new System.Drawing.Point(6, 147);
        this.tbSlideWaist.Maximum = 20;
        this.tbSlideWaist.Name = "tbSlideWaist";
        this.tbSlideWaist.Size = new System.Drawing.Size(168, 45);
        this.tbSlideWaist.TabIndex = 21;
        this.tbSlideWaist.ValueChanged += new System.EventHandler(this.tbSlideWaist_ValueChanged);
        // 
        // tbSlideArm
        // 
        this.tbSlideArm.Location = new System.Drawing.Point(6, 21);
        this.tbSlideArm.Maximum = 20;
        this.tbSlideArm.Name = "tbSlideArm";
        this.tbSlideArm.Size = new System.Drawing.Size(168, 45);
        this.tbSlideArm.TabIndex = 20;
        this.tbSlideArm.ValueChanged += new System.EventHandler(this.tbSlideArm_ValueChanged);
        // 
        // tbSlideLeg
        // 
        this.tbSlideLeg.Location = new System.Drawing.Point(6, 84);
        this.tbSlideLeg.Maximum = 20;
        this.tbSlideLeg.Name = "tbSlideLeg";
        this.tbSlideLeg.Size = new System.Drawing.Size(168, 45);
        this.tbSlideLeg.TabIndex = 19;
        this.tbSlideLeg.ValueChanged += new System.EventHandler(this.tbSlideLeg_ValueChanged);
        // 
        // gvShaderParams
        // 
        this.gvShaderParams.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
        this.gvShaderParams.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        this.gvShaderParams.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
        this.gvShaderParams.Location = new System.Drawing.Point(192, 6);
        this.gvShaderParams.Name = "gvShaderParams";
        this.gvShaderParams.RowTemplate.Height = 21;
        this.gvShaderParams.Size = new System.Drawing.Size(368, 472);
        this.gvShaderParams.TabIndex = 8;
        // 
        // lvSubScripts
        // 
        this.lvSubScripts.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader2,
            this.columnHeader3});
        this.lvSubScripts.FullRowSelect = true;
        this.lvSubScripts.GridLines = true;
        this.lvSubScripts.HideSelection = false;
        this.lvSubScripts.Location = new System.Drawing.Point(6, 6);
        this.lvSubScripts.MultiSelect = false;
        this.lvSubScripts.Name = "lvSubScripts";
        this.lvSubScripts.Size = new System.Drawing.Size(180, 472);
        this.lvSubScripts.TabIndex = 7;
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
        // btnDump
        // 
        this.btnDump.Location = new System.Drawing.Point(485, 484);
        this.btnDump.Name = "btnDump";
        this.btnDump.Size = new System.Drawing.Size(75, 23);
        this.btnDump.TabIndex = 6;
        this.btnDump.Text = "&Dump";
        this.btnDump.UseVisualStyleBackColor = true;
        this.btnDump.Click += new System.EventHandler(this.btnDump_Click);
        // 
        // tpFramesAndNodes
        // 
        this.tpFramesAndNodes.Controls.Add(this.lvNodes);
        this.tpFramesAndNodes.Controls.Add(this.lvFrames);
        this.tpFramesAndNodes.Location = new System.Drawing.Point(4, 22);
        this.tpFramesAndNodes.Name = "tpFramesAndNodes";
        this.tpFramesAndNodes.Padding = new System.Windows.Forms.Padding(3);
        this.tpFramesAndNodes.Size = new System.Drawing.Size(566, 513);
        this.tpFramesAndNodes.TabIndex = 2;
        this.tpFramesAndNodes.Text = "Frames and Nodes";
        this.tpFramesAndNodes.UseVisualStyleBackColor = true;
        // 
        // lvNodes
        // 
        this.lvNodes.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader5});
        this.lvNodes.FullRowSelect = true;
        this.lvNodes.GridLines = true;
        this.lvNodes.HideSelection = false;
        this.lvNodes.Location = new System.Drawing.Point(287, 6);
        this.lvNodes.MultiSelect = false;
        this.lvNodes.Name = "lvNodes";
        this.lvNodes.Size = new System.Drawing.Size(274, 472);
        this.lvNodes.TabIndex = 21;
        this.lvNodes.UseCompatibleStateImageBehavior = false;
        this.lvNodes.View = System.Windows.Forms.View.Details;
        this.lvNodes.SelectedIndexChanged += new System.EventHandler(this.lvNodes_SelectedIndexChanged);
        // 
        // columnHeader5
        // 
        this.columnHeader5.Text = "Name";
        // 
        // lvFrames
        // 
        this.lvFrames.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader4});
        this.lvFrames.FullRowSelect = true;
        this.lvFrames.GridLines = true;
        this.lvFrames.HideSelection = false;
        this.lvFrames.Location = new System.Drawing.Point(6, 6);
        this.lvFrames.MultiSelect = false;
        this.lvFrames.Name = "lvFrames";
        this.lvFrames.Size = new System.Drawing.Size(274, 472);
        this.lvFrames.TabIndex = 20;
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
        this.Controls.Add(this.tabControl1);
        this.Controls.Add(this.lvTSOFiles);
        this.Controls.Add(this.btnDown);
        this.Controls.Add(this.btnUp);
        this.Name = "FigureForm";
        this.Text = "TSOGrid";
        this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FigureForm_FormClosing);
        this.tabControl1.ResumeLayout(false);
        this.tpSliders.ResumeLayout(false);
        this.tpSliders.PerformLayout();
        this.tpSubScripts.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)(this.tbSlideEye)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.tbSlideTall)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.tbSlideBust)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.tbSlideWaist)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.tbSlideArm)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.tbSlideLeg)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.gvShaderParams)).EndInit();
        this.tpFramesAndNodes.ResumeLayout(false);
        this.ResumeLayout(false);

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
