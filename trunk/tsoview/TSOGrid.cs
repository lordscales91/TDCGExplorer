using System;
using System.Drawing;
using System.Windows.Forms;

namespace TAHdecrypt
{
public class TSOForm : Form
{
    Button btn1;
    PropertyGrid pg;
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

        pg = new PropertyGrid();
        pg.Bounds = new Rectangle(new Point(10, 40), new Size(300, 200));
        this.Controls.Add(pg);

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

    private Shader shader = null;

    public void SetShader(Shader shader)
    {
        this.shader = shader;
        pg.SelectedObject = shader.shader_parameters;
        dg.DataSource = shader.shader_parameters;
    }

    protected void btn1_Click(object sender, EventArgs e)
    {
        Console.WriteLine("-- dump shader parameters --");
        foreach (ShaderParameter param in shader.shader_parameters)
            Console.WriteLine("Name {0} F1 {1}", param.Name, param.F1);
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
            form.SetShader(tso.sub_scripts[0].shader);
            form.Show();

            while (form.Created)
            {
                Application.DoEvents();
            }
        }
    }
}
}
