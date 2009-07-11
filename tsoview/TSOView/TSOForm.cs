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
    internal int keyCameraLoadOrSave1 = (int)Keys.D1;
    internal int keyCameraLoadOrSave2 = (int)Keys.D2;
    internal int keyCameraInterpolation = (int)Keys.C;
    internal int keyFigureForm = (int)Keys.G;

    internal Viewer viewer = null;
    internal FigureForm fig_form = null;

    private Camera camera
    {
        get {
            return viewer.Camera;
        }
        set {
            viewer.Camera = value;
        }
    }

    private Camera cam1 = null;
    private Camera cam2 = null;
    private Timer timer1;
    private System.ComponentModel.IContainer components;

    private int cam_frame_index = 0;

    public TSOForm(TSOConfig config, string[] args)
    {
        InitializeComponent();
        this.ClientSize = config.ClientSize;
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
        Viewer viewer = new Viewer();
        FigureForm fig_form = new FigureForm();
        this.fig_form = fig_form;
        this.viewer = viewer;

        if (viewer.InitializeApplication(this, true))
        {
            viewer.FigureEvent += delegate(object sender, EventArgs e)
            {
                Figure fig;
                if (viewer.TryGetFigure(out fig))
                    fig_form.SetFigure(fig);
                else
                    fig_form.Clear();
            };
            foreach (string arg in args)
                viewer.LoadAnyFile(arg, true);

            var script = CSScript.Load(Path.Combine(Application.StartupPath, "Script.cs")).CreateInstance("TDCG.Script").AlignToInterface<IScript>();
            script.Hello(viewer);

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
            viewer.SaveToBitmap();
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
        if (keysEnabled[keyCameraLoadOrSave1] && keys[keyCameraLoadOrSave1])
        {
            keysEnabled[keyCameraLoadOrSave1] = false;
            if (keys[(int)Keys.ControlKey])
                camera.Save(@"camera1.xml");
            else if (File.Exists(@"camera1.xml"))
            {
                camera = Camera.Load(@"camera1.xml");
                Figure fig;
                if (viewer.TryGetFigure(out fig))
                    camera.SetCenter(fig.Center);
            }
        }
        if (keysEnabled[keyCameraLoadOrSave2] && keys[keyCameraLoadOrSave2])
        {
            keysEnabled[keyCameraLoadOrSave2] = false;
            if (keys[(int)Keys.ControlKey])
                camera.Save(@"camera2.xml");
            else if (File.Exists(@"camera2.xml"))
            {
                camera = Camera.Load(@"camera2.xml");
                Figure fig;
                if (viewer.TryGetFigure(out fig))
                    camera.SetCenter(fig.Center);
            }
        }
        if (keysEnabled[keyCameraInterpolation] && keys[keyCameraInterpolation])
        {
            keysEnabled[keyCameraInterpolation] = false;
            if (cam1 != null && cam2 != null)
            {
                camera = cam2;
                cam_frame_index = 0;
                cam1 = null;
                cam2 = null;
            }
            else
            {
                if (File.Exists(@"camera1.xml"))
                    cam1 = Camera.Load(@"camera1.xml");
                if (File.Exists(@"camera2.xml"))
                    cam2 = Camera.Load(@"camera2.xml");
                if (cam1 != null && cam2 != null)
                    camera = cam1;
            }
            {
                Figure fig;
                if (viewer.TryGetFigure(out fig))
                    camera.SetCenter(fig.Center);
            }
        }
        if (keysEnabled[keyFigureForm] && keys[keyFigureForm])
        {
            keys[keyFigureForm] = false;
            keysEnabled[keyFigureForm] = true;
            // stale KeyUp event
            fig_form.Show();
            fig_form.Activate();
        }

        if (cam1 != null && cam2 != null)
        {
            camera = Camera.Interpolation(cam1, cam2, cam_frame_index/120.0f);
            cam_frame_index++;
            if (cam_frame_index >= 120)
            {
                cam_frame_index = 0;
                Camera cam0 = cam2;
                cam2 = cam1;
                cam1 = cam0;
            }
        }

        float keyL = 0.0f;
        float keyR = 0.0f;
        float keyU = 0.0f;
        float keyD = 0.0f;
        float keyPush = 0.0f;
        float keyPull = 0.0f;
        float keyZRol = 0.0f;

        if (keys[(int)Keys.Left])
            keyL = 0.1f;
        if (keys[(int)Keys.Right])
            keyR = 0.1f;
        if (keys[(int)Keys.PageUp])
            keyU = 0.1f;
        if (keys[(int)Keys.PageDown])
            keyD = 0.1f;
        if (keys[(int)Keys.Up])
            keyPush = 0.1f;
        if (keys[(int)Keys.Down])
            keyPull = 0.1f;
        if (keys[(int)Keys.A])
            keyZRol = -2.0f;
        if (keys[(int)Keys.D])
            keyZRol = +2.0f;

        viewer.MoveTarget(keyL - keyR, keyD - keyU, keyPush - keyPull);
        viewer.MoveSwivel(DegreeToRadian(keyZRol));
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
