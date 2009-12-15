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
    private TrackBar tbSlide;
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
        
        Debug.Assert(fig.RatioList.Count == 7);
        this.tbSlide.Value = (int)(fig.RatioList[0] * 10);

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
        this.tbSlide = new System.Windows.Forms.TrackBar();
        ((System.ComponentModel.ISupportInitialize)(this.gvShaderParams)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.tbSlide)).BeginInit();
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
        // tbSlide
        // 
        this.tbSlide.Location = new System.Drawing.Point(604, 218);
        this.tbSlide.Name = "tbSlide";
        this.tbSlide.Size = new System.Drawing.Size(104, 45);
        this.tbSlide.TabIndex = 6;
        this.tbSlide.VisibleChanged += new System.EventHandler(this.slider_VisibleChanged);
        // 
        // FigureForm
        // 
        this.ClientSize = new System.Drawing.Size(784, 563);
        this.Controls.Add(this.tbSlide);
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
        ((System.ComponentModel.ISupportInitialize)(this.tbSlide)).EndInit();
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

    private void FigureForm_FormClosing(object sender, FormClosingEventArgs e)
    {
        if (e.CloseReason != CloseReason.FormOwnerClosing)
        {
            this.Hide();
            e.Cancel = true;
        }
    }

    private void slider_VisibleChanged(object sender, EventArgs e)
    {
        if (fig == null)
            return;

        Debug.Assert(fig.RatioList.Count == 7);
        fig.RatioList[0] = tbSlide.Value * 0.1f;
    }
}
}
