using System;
using System.Drawing;
using System.Windows.Forms;

namespace TAHdecrypt
{
public class TSOForm : Form
{
    Button btn1;
    ListView lv;
    DataGridView dg;

    public TSOForm()
    {
        this.ClientSize = new System.Drawing.Size(800, 600);
        this.Text = "TSOGrid";
        //this.AllowDrop = true;

        btn1 = new Button();
        btn1.Location = new Point(10, 10);
        btn1.Text = "&Dump";
        btn1.Click += new EventHandler(btn1_Click);
        this.Controls.Add(btn1);

        lv = new ListView();
        lv.Bounds = new Rectangle(new Point(10, 40), new Size(300, 200));
        lv.View = View.Details;
        lv.FullRowSelect = true;
        lv.HideSelection = false;
        lv.GridLines = true;

        lv.Columns.Add("Name", -2, HorizontalAlignment.Left);
        lv.Columns.Add("File", -2, HorizontalAlignment.Left);
        lv.SelectedIndexChanged += lv_SelectedIndexChanged;

        this.Controls.Add(lv);

        dg = new DataGridView();
        dg.Bounds = new Rectangle(new Point(10, 250), new Size(300, 200));
        dg.EditMode = DataGridViewEditMode.EditOnEnter;
        dg.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        this.Controls.Add(dg);
    }

    protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
    {
        //this.Render(); // Render on painting
    }

    protected override void OnKeyPress(System.Windows.Forms.KeyPressEventArgs e)
    {
        if ((int)(byte)e.KeyChar == (int)System.Windows.Forms.Keys.Escape)
            this.Dispose(); // Esc was pressed
    }

    private TSOFile tso = null;
    private Shader shader = null;

    public void SetTSOFile(TSOFile tso)
    {
        this.tso = tso;
        foreach (TSOSubScript sub_script in tso.sub_scripts)
        {
            ListViewItem li = new ListViewItem(sub_script.Name);
            li.SubItems.Add(sub_script.File);
            li.Tag = sub_script;
            lv.Items.Add(li);
        }
    }

    public void SetShader(Shader shader)
    {
        this.shader = shader;
        dg.DataSource = shader.shader_parameters;
    }

    protected void btn1_Click(object sender, EventArgs e)
    {
        if (shader == null)
            return;
        Console.WriteLine("-- dump shader parameters --");
        foreach (ShaderParameter param in shader.shader_parameters)
            Console.WriteLine("Name {0} F1 {1}", param.Name, param.F1);
    }

    protected void lv_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (lv.SelectedItems.Count == 0)
            return;
        ListViewItem li = lv.SelectedItems[0];
        TSOSubScript sub_script = li.Tag as TSOSubScript;
        SetShader(sub_script.shader);
    }
}

static class TSOGrid
{
    static void Main(string[] args)
    {
        if (args.Length != 1)
        {
            Console.WriteLine("Usage: TSOGrid <tso file>");
            return;
        }
        string source_file = args[0];
        TSOFile tso = new TSOFile();
        tso.Load(source_file);
        //tso.Dump();
        using (TSOForm form = new TSOForm())
        {
            form.SetTSOFile(tso);
            form.Show();

            while (form.Created)
            {
                Application.DoEvents();
            }
        }
    }
}
}
