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
    /// �t�B�M���A���������t�H�[��
    /// </summary>
public class FigureForm : Form
{
    Button btn1;
    Button btnUp;
    Button btnDown;
    ListView lv_fig;
    ListView lv;
    DataGridView dg;

    /// <summary>
    /// �t�B�M���A���t�H�[���𐶐����܂��B
    /// </summary>
    public FigureForm()
    {
        this.ClientSize = new Size(800, 600);
        this.Text = "TSOGrid";
        //this.AllowDrop = true;
        this.FormClosing += new FormClosingEventHandler(form_FormClosing);

        btn1 = new Button();
        btn1.Location = new Point(10, 10);
        btn1.Text = "Dump";
        btn1.Click += new EventHandler(btn1_Click);
        this.Controls.Add(btn1);

        btnUp = new Button();
        btnUp.Location = new Point(95, 10);
        btnUp.Text = "&Up";
        btnUp.Click += new EventHandler(btnUp_Click);
        this.Controls.Add(btnUp);

        btnDown = new Button();
        btnDown.Location = new Point(180, 10);
        btnDown.Text = "&Down";
        btnDown.Click += new EventHandler(btnDown_Click);
        this.Controls.Add(btnDown);

        lv_fig = new ListView();
        lv_fig.Bounds = new Rectangle(new Point(10, 40), new Size(100, 200));
        lv_fig.View = View.Details;
        lv_fig.FullRowSelect = true;
        lv_fig.HideSelection = false;
        lv_fig.MultiSelect = false;
        lv_fig.GridLines = true;

        lv_fig.Columns.Add("Name", -2, HorizontalAlignment.Left);
        lv_fig.SelectedIndexChanged += lv_fig_SelectedIndexChanged;

        this.Controls.Add(lv_fig);

        lv = new ListView();
        lv.Bounds = new Rectangle(new Point(120, 40), new Size(300, 200));
        lv.View = View.Details;
        lv.FullRowSelect = true;
        lv.HideSelection = false;
        lv.MultiSelect = false;
        lv.GridLines = true;

        lv.Columns.Add("Name", -2, HorizontalAlignment.Left);
        lv.Columns.Add("File", -2, HorizontalAlignment.Left);
        lv.SelectedIndexChanged += lv_SelectedIndexChanged;

        this.Controls.Add(lv);

        dg = new DataGridView();
        dg.Bounds = new Rectangle(new Point(10, 250), new Size(410, 250));
        dg.EditMode = DataGridViewEditMode.EditOnEnter;
        dg.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        this.Controls.Add(dg);
    }

    /// <summary>
    /// �t�H�[�������Ƃ��j�������ɉB���܂��B
    /// </summary>
    /// <param name="sender">sender</param>
    /// <param name="e">�C�x���g����</param>
    protected void form_FormClosing(object sender, FormClosingEventArgs e)
    {
        if (e.CloseReason != CloseReason.FormOwnerClosing)
        {
            this.Hide();
            e.Cancel = true;
        }
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
        dg.DataSource = null;
        this.shader = null;
        lv.Items.Clear();
        this.tso = null;
        lv_fig.Items.Clear();
        this.fig = null;
    }

    /// <summary>
    /// �t�B�M���A��UI�ɐݒ肵�܂��B
    /// </summary>
    /// <param name="fig">�t�B�M���A</param>
    public void SetFigure(Figure fig)
    {
        this.fig = fig;
        lv_fig.Items.Clear();
        for (int i = 0; i < fig.TSOList.Count; i++)
        {
            TSOFile tso = fig.TSOList[i];
            ListViewItem li = new ListViewItem("TSO #" + i.ToString());
            li.Tag = tso;
            lv_fig.Items.Add(li);
        }
        lv_fig.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
    }

    /// <summary>
    /// tso��UI�ɐݒ肵�܂��B
    /// </summary>
    /// <param name="tso">tso</param>
    public void SetTSOFile(TSOFile tso)
    {
        this.tso = tso;
        lv.Items.Clear();
        foreach (TSOSubScript sub_script in tso.sub_scripts)
        {
            ListViewItem li = new ListViewItem(sub_script.Name);
            li.SubItems.Add(sub_script.File);
            li.Tag = sub_script;
            lv.Items.Add(li);
        }
        lv.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
    }

    /// <summary>
    /// �V�F�[�_�ݒ��UI�ɐݒ肵�܂��B
    /// </summary>
    /// <param name="shader">�V�F�[�_�ݒ�</param>
    public void SetShader(Shader shader)
    {
        this.shader = shader;
        dg.DataSource = shader.shader_parameters;
    }

    /// <summary>
    /// btn1���N���b�N�����Ƃ��ɌĂяo����܂��B
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btn1_Click(object sender, EventArgs e)
    {
        if (shader == null)
            return;
        Console.WriteLine("-- dump shader parameters --");
        foreach (ShaderParameter param in shader.shader_parameters)
            Console.WriteLine("Name {0} F1 {1} F2 {2} F3 {3} F4 {4}", param.Name, param.F1, param.F2, param.F3, param.F4);
    }

    /// <summary>
    /// btnUp���N���b�N�����Ƃ��ɌĂяo����܂��B
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnUp_Click(object sender, EventArgs e)
    {
        if (lv_fig.SelectedItems.Count == 0)
            return;
        int li_idx = lv_fig.SelectedIndices[0];
        int li_idx_prev = li_idx-1;
        if (li_idx_prev < 0)
            return;
        fig.SwapAt(li_idx_prev, li_idx);
        SetFigure(fig);
        ListViewItem li = lv_fig.Items[li_idx_prev];
        li.Selected = true;
    }

    /// <summary>
    /// btnDown���N���b�N�����Ƃ��ɌĂяo����܂��B
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnDown_Click(object sender, EventArgs e)
    {
        if (lv_fig.SelectedItems.Count == 0)
            return;
        int li_idx = lv_fig.SelectedIndices[0];
        int li_idx_next = li_idx+1;
        if (li_idx_next > lv_fig.Items.Count-1)
            return;
        fig.SwapAt(li_idx, li_idx_next);
        SetFigure(fig);
        ListViewItem li = lv_fig.Items[li_idx_next];
        li.Selected = true;
    }

    /// <summary>
    /// lv_fig�̑I���C���f�b�N�X���ύX���ꂽ�Ƃ��ɌĂяo����܂��B
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void lv_fig_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (lv_fig.SelectedItems.Count == 0)
            return;
        ListViewItem li = lv_fig.SelectedItems[0];
        TSOFile tso = li.Tag as TSOFile;
        SetTSOFile(tso);
    }

    /// <summary>
    /// lv�̑I���C���f�b�N�X���ύX���ꂽ�Ƃ��ɌĂяo����܂��B
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void lv_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (lv.SelectedItems.Count == 0)
            return;
        ListViewItem li = lv.SelectedItems[0];
        TSOSubScript sub_script = li.Tag as TSOSubScript;
        SetShader(sub_script.shader);
    }
}
}
