using System;
using System.Collections.Generic;
using System.Diagnostics;
//using System.Drawing;
//using System.Threading;
//using System.ComponentModel;
using System.Windows.Forms;
using System.IO;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using TDCG;
using Tso2Pmd;

namespace TSOView
{

public class TSOForm : Form
{
    // キー入力を保持
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

    internal Viewer viewer = null;
    internal FigureForm fig_form = null;
    internal MainForm owner_form = null;
    
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

    public TSOForm(Viewer viewer, MainForm owner_form)
    {
        InitializeComponent();

        TSOConfig tso_config = new TSOConfig();
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

        this.viewer = viewer;
        this.fig_form = new FigureForm();
        this.owner_form = owner_form;

        if (viewer.InitializeApplication(this, true))
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

            this.timer1.Enabled = true;

            viewer.Camera.Translation = new Vector3(0.0f, 0.0f, +50.0f);
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
        //コントロール内にカーソルがドラッグされたとき、表示されるものを選択
        if (e.Data.GetDataPresent(DataFormats.FileDrop))
        {
            // Ctrlを押している状態を判別しない
            e.Effect = DragDropEffects.Copy;

            /*// Ctrlを押している状態なら、+を表示する
            if ((e.KeyState & 8) == 8)
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.Move;*/
        }
        else
        {
            //ファイル以外は受け付けない
            e.Effect = DragDropEffects.None;
        }
    }

    private void form_OnDragDrop(object sender, DragEventArgs e)
    {
        owner_form.panel1_DragDrop(sender, e);
    }

    private void timer1_Tick(object sender, EventArgs e)
    {
        this.FrameMove();
        viewer.FrameMove();
        viewer.Render();
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
        this.ShowIcon = false;
        this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.TSOForm_FormClosing);
        this.ResumeLayout(false);

    }

    private bool dispose_flag = false;
    public bool Dispose_flag { set { dispose_flag = value; } get { return dispose_flag; } }

    private void TSOForm_FormClosing(object sender, FormClosingEventArgs e)
    {
        if (dispose_flag == false)
        {
            owner_form.TSOForm_Hiding();
            this.Hide();
            e.Cancel = true;
        }

    }
}
}
