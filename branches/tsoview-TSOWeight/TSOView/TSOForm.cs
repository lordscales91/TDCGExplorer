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

    internal WeightViewer viewer = null;
    internal FigureForm fig_form = null;
    
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
                    viewer.Camera.SetCenter(fig.Center);
            };
            viewer.FigureEvent += delegate(object sender, EventArgs e)
            {
                Figure fig;
                if (viewer.TryGetFigure(out fig))
                    fig_form.SetFigure(fig);
                else
                    fig_form.Clear();
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
        this.SuspendLayout();
        // 
        // timer1
        // 
        this.timer1.Interval = 16;
        this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
        // 
        // TSOForm
        // 
        this.ClientSize = new System.Drawing.Size(284, 263);
        this.Name = "TSOForm";
        this.ResumeLayout(false);

    }
}
}
