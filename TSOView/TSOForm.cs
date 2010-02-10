using System;
using System.Collections.Generic;
using System.Diagnostics;
//using System.Drawing;
//using System.Threading;
//using System.ComponentModel;
using System.Windows.Forms;
using System.IO;
using CSScriptLibrary;
using TDCG;

namespace TSOView
{

public class TSOForm : Form
{
    // ÉLÅ[ì¸óÕÇï€éù
    internal bool[] keys = new bool[256];
    internal bool[] keysEnabled = new bool[256];

    internal int keySave        = (int)Keys.Return;
    internal int keyMotion      = (int)Keys.Space;
    internal int keyShadow      = (int)Keys.S;
    internal int keySprite      = (int)Keys.Z;
    internal int keyFigure      = (int)Keys.Tab;
    internal int keyDelete      = (int)Keys.Delete;
    internal int keyCameraReset = (int)Keys.D0;
    internal int keyFigureForm = (int)Keys.G;

    WeightViewer viewer = null;
    FigureForm fig_form = null;
    string save_path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\TechArts3D\TDCG";
    
    private SimpleCamera camera
    {
        get {
            return viewer.Camera;
        }
        set {
            viewer.Camera = value;
        }
    }

    private Timer timer1;
    private DataGridView gvSkinWeights;
    private Button btnShowFigureForm;
    private DataGridViewTextBoxColumn Column1;
    private DataGridViewTextBoxColumn Column2;
    private System.ComponentModel.IContainer components;

    public TSOForm(TSOConfig tso_config, string[] args)
    {
        InitializeComponent();
        this.ClientSize = tso_config.ClientSize;
        this.Text = "TSOView";
        this.AllowDrop = true;

        for (int i = 0; i < keysEnabled.Length; i++)
        {
            keysEnabled[i] = true;
        }
        this.KeyDown += new KeyEventHandler(form_OnKeyDown);
        this.KeyUp += new KeyEventHandler(form_OnKeyUp);

        this.DragDrop += new DragEventHandler(form_OnDragDrop);
        this.DragOver += new DragEventHandler(form_OnDragOver);

        this.viewer = new WeightViewer();
        this.fig_form = new FigureForm();

        if (viewer.InitializeApplication(this))
        {
            viewer.FigureEvent += delegate(object sender, EventArgs e)
            {
                Figure fig;
                if (viewer.TryGetFigure(out fig))
                    fig_form.Figure = fig;
                else
                    fig_form.Clear();
            };
            viewer.VertexEvent += delegate(object sender, EventArgs e)
            {
                Vertex selected_vertex = viewer.selected_vertex_mesh.vertices[viewer.selected_vertex_id];
                gvSkinWeights.Rows.Clear();
                foreach (SkinWeight skin_weight in selected_vertex.skin_weights)
                {
                    gvSkinWeights.Rows.Add(new string[] { skin_weight.bone_index.ToString(), skin_weight.weight.ToString("F4") });
                }
            };
            fig_form.NodeEvent += delegate(object sender, EventArgs e)
            {
                viewer.selected_node = fig_form.selected_node;
            };
            fig_form.FrameEvent += delegate(object sender, EventArgs e)
            {
                viewer.selected_frame = fig_form.selected_frame;
            };
            foreach (string arg in args)
                viewer.LoadAnyFile(arg, true);
            if (viewer.FigureList.Count == 0)
                viewer.LoadAnyFile(Path.Combine(save_path, "system.tdcgsav.png"), true);
            viewer.Camera.SetTranslation(0.0f, +18.0f, +10.0f);

            string script_file = Path.Combine(Application.StartupPath, "Script.cs");
            if (File.Exists(script_file))
            {
                var script = CSScript.Load(script_file).CreateInstance("TDCG.Script").AlignToInterface<IScript>();
                script.Hello(viewer);
            }

            this.timer1.Enabled = true;
        }
    }

    private void form_OnKeyDown(object sender, KeyEventArgs e)
    {
        if ((int)e.KeyCode < keys.Length)
        {
            keys[(int)e.KeyCode] = true;
        }
    }

    private void form_OnKeyUp(object sender, KeyEventArgs e)
    {
        if ((int)e.KeyCode < keys.Length)
        {
            keys[(int)e.KeyCode] = false;
            keysEnabled[(int)e.KeyCode] = true;
        }
    }

    static float DegreeToRadian(float angle)
    {
        return (float)(Math.PI * angle / 180.0);
    }

    public void FrameMove()
    {
        if (keysEnabled[keySave] && keys[keySave])
        {
            keysEnabled[keySave] = false;
            viewer.SaveToBitmap("sample.bmp");
        }
        if (keysEnabled[keyMotion] && keys[keyMotion])
        {
            keysEnabled[keyMotion] = false;
            viewer.SwitchMotionEnabled();
        }
        if (keysEnabled[keyShadow] && keys[keyShadow])
        {
            keysEnabled[keyShadow] = false;
            viewer.SwitchShadowShown();
        }
        if (keysEnabled[keySprite] && keys[keySprite])
        {
            keysEnabled[keySprite] = false;
            viewer.SwitchSpriteShown();
        }
        if (keysEnabled[keyFigure] && keys[keyFigure])
        {
            keysEnabled[keyFigure] = false;
            viewer.NextFigure();
        }
        if (keysEnabled[keyDelete] && keys[keyDelete])
        {
            keysEnabled[keyDelete] = false;

            if (keys[(int)Keys.ControlKey])
                viewer.ClearFigureList();
            else
                viewer.RemoveSelectedFigure();
        }
        if (keysEnabled[keyCameraReset] && keys[keyCameraReset])
        {
            keysEnabled[keyCameraReset] = false;
            camera.Reset();
            Figure fig;
            if (viewer.TryGetFigure(out fig))
                camera.SetCenter(fig.Center);
        }
        if (keysEnabled[keyFigureForm] && keys[keyFigureForm])
        {
            keys[keyFigureForm] = false;
            keysEnabled[keyFigureForm] = true;
            // stale KeyUp event
            fig_form.Show();
            fig_form.Activate();
        }

        float keyL = 0.0f;
        float keyR = 0.0f;
        float keyU = 0.0f;
        float keyD = 0.0f;
        float keyPush = 0.0f;
        float keyPull = 0.0f;
        float keyZRol = 0.0f;

        if (keys[(int)Keys.Left])
            keyL = 2.0f;
        if (keys[(int)Keys.Right])
            keyR = 2.0f;
        if (keys[(int)Keys.PageUp])
            keyU = 2.0f;
        if (keys[(int)Keys.PageDown])
            keyD = 2.0f;
        if (keys[(int)Keys.Up])
            keyPush = 1.0f;
        if (keys[(int)Keys.Down])
            keyPull = 1.0f;
        if (keys[(int)Keys.A])
            keyZRol = -2.0f;
        if (keys[(int)Keys.D])
            keyZRol = +2.0f;

        camera.Move(keyR - keyL, keyU - keyD, keyPull - keyPush);
        camera.RotZ(DegreeToRadian(keyZRol));
    }

    private void form_OnDragOver(object sender, DragEventArgs e)
    {
        if (e.Data.GetDataPresent(DataFormats.FileDrop))
        {
            if ((e.KeyState & 8) == 8)
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.Move;
        }
    }

    private void form_OnDragDrop(object sender, DragEventArgs e)
    {
        if (e.Data.GetDataPresent(DataFormats.FileDrop))
        {
            foreach (string src in (string[])e.Data.GetData(DataFormats.FileDrop))
                viewer.LoadAnyFile(src, (e.KeyState & 8) == 8);
        }
    }

    private void timer1_Tick(object sender, EventArgs e)
    {
        this.FrameMove();
        viewer.FrameMove();
        viewer.Render();
    }

    public void Render()
    {
        /*
        int height = 24;
        for (int i = 0; i < keys.Length; i++)
        {
            if (keys[i])
            {
                font.DrawText(null, ((Keys)i).ToString(), 0, height, Color.Black);
                height += 24;
            }
        }
        */
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

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            viewer.Dispose();
        }
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        this.components = new System.ComponentModel.Container();
        this.timer1 = new System.Windows.Forms.Timer(this.components);
        this.gvSkinWeights = new System.Windows.Forms.DataGridView();
        this.btnShowFigureForm = new System.Windows.Forms.Button();
        this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
        this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
        ((System.ComponentModel.ISupportInitialize)(this.gvSkinWeights)).BeginInit();
        this.SuspendLayout();
        // 
        // timer1
        // 
        this.timer1.Interval = 16;
        this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
        // 
        // gvSkinWeights
        // 
        this.gvSkinWeights.AllowUserToAddRows = false;
        this.gvSkinWeights.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        this.gvSkinWeights.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.Column2});
        this.gvSkinWeights.Location = new System.Drawing.Point(12, 41);
        this.gvSkinWeights.Name = "gvSkinWeights";
        this.gvSkinWeights.RowTemplate.Height = 21;
        this.gvSkinWeights.Size = new System.Drawing.Size(260, 150);
        this.gvSkinWeights.TabIndex = 0;
        // 
        // btnShowFigureForm
        // 
        this.btnShowFigureForm.Location = new System.Drawing.Point(12, 12);
        this.btnShowFigureForm.Name = "btnShowFigureForm";
        this.btnShowFigureForm.Size = new System.Drawing.Size(260, 23);
        this.btnShowFigureForm.TabIndex = 1;
        this.btnShowFigureForm.Text = "show FigureForm";
        this.btnShowFigureForm.UseVisualStyleBackColor = true;
        this.btnShowFigureForm.Click += new System.EventHandler(this.btnShowFigureForm_Click);
        // 
        // Column1
        // 
        this.Column1.HeaderText = "BoneIndex";
        this.Column1.Name = "Column1";
        // 
        // Column2
        // 
        this.Column2.HeaderText = "Weight";
        this.Column2.Name = "Column2";
        // 
        // TSOForm
        // 
        this.ClientSize = new System.Drawing.Size(284, 263);
        this.Controls.Add(this.btnShowFigureForm);
        this.Controls.Add(this.gvSkinWeights);
        this.Name = "TSOForm";
        ((System.ComponentModel.ISupportInitialize)(this.gvSkinWeights)).EndInit();
        this.ResumeLayout(false);

    }

    private void btnShowFigureForm_Click(object sender, EventArgs e)
    {
        fig_form.Show();
        fig_form.Activate();
    }
}
}
