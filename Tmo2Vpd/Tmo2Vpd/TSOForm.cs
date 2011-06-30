using System;
using System.Collections.Generic;
using System.Diagnostics;
//using System.Drawing;
//using System.Threading;
//using System.ComponentModel;
using System.Windows.Forms;
using System.IO;
using CSScriptLibrary;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using TDCG;
using TDCGUtils;

namespace TSOView
{

public class TSOForm : Form
{
    // 繧ｭ繝ｼ蜈･蜉帙ｒ菫晄戟
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
    internal TransBoneRotation tbr = null;

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
    private Button button1;
    private Label label1;
    private System.ComponentModel.IContainer components;

    public TSOForm(TSOConfig tso_config, string[] args)
    {
        InitializeComponent();
        this.ClientSize = tso_config.ClientSize;
        this.AllowDrop = true;

        for (int i = 0; i < keysEnabled.Length; i++)
        {
            keysEnabled[i] = true;
        }
        this.KeyDown += new KeyEventHandler(form_OnKeyDown);
        this.KeyUp += new KeyEventHandler(form_OnKeyUp);

        this.DragDrop += new DragEventHandler(form_OnDragDrop);
        this.DragOver += new DragEventHandler(form_OnDragOver);

        this.viewer = new Viewer(false);
        this.fig_form = new FigureForm();
        this.tbr = new TransBoneRotation();

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
            foreach (string arg in args)
                viewer.LoadAnyFile(arg, true);

            // 初期ポーズのTMOを得ておく
            Figure fig_forPose;
            viewer.TryGetFigure(out fig_forPose);
            tbr.SetInitPose(fig_forPose);

            /*string script_file = Path.Combine(Application.StartupPath, "Script.cs");
            if (File.Exists(script_file))
            {
                var script = CSScript.Load(script_file).CreateInstance("TDCG.Script").AlignToInterface<IScript>();
                script.Hello(viewer);
            }*/

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
        if (e.Data.GetDataPresent(DataFormats.FileDrop))
            e.Effect = DragDropEffects.Move;
    }

    private void form_OnDragDrop(object sender, DragEventArgs e)
    {
        //ドロップされたデータがファイルか調べる
        if (e.Data.GetDataPresent(DataFormats.FileDrop))
        {
            string[] FileName
                = (string[])e.Data.GetData(DataFormats.FileDrop);

            string name = FileName[0];

            // 拡張子が".tdcgpose.png"or".tmo"であるか確認する
            if ((System.IO.Path.GetExtension(name) == ".png" &&
                System.IO.Path.GetExtension(
                    System.IO.Path.GetFileNameWithoutExtension(name)) == ".tdcgpose")
                || System.IO.Path.GetExtension(name) == ".tmo")
            {
                try
                {
                    //ドロップされたファイルを読み込む
                    viewer.LoadAnyFile(name, false);
                }
                catch
                {
                    MessageBox.Show("ファイルを読み込むことができませんでした。\nファイルが正常であるか確認してください。");
                }
            }
            else
            {
                MessageBox.Show("拡張子が.tdcgpose.pngか、.tmoである必要があります。");
                return;
            }
        }
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
        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TSOForm));
        this.timer1 = new System.Windows.Forms.Timer(this.components);
        this.button1 = new System.Windows.Forms.Button();
        this.label1 = new System.Windows.Forms.Label();
        this.SuspendLayout();
        // 
        // timer1
        // 
        this.timer1.Interval = 16;
        this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
        // 
        // button1
        // 
        this.button1.Location = new System.Drawing.Point(12, 17);
        this.button1.Name = "button1";
        this.button1.Size = new System.Drawing.Size(81, 48);
        this.button1.TabIndex = 0;
        this.button1.Text = "ポーズを変換";
        this.button1.UseVisualStyleBackColor = true;
        this.button1.Click += new System.EventHandler(this.button1_Click);
        // 
        // label1
        // 
        this.label1.AutoSize = true;
        this.label1.Location = new System.Drawing.Point(109, 17);
        this.label1.Name = "label1";
        this.label1.Size = new System.Drawing.Size(278, 48);
        this.label1.TabIndex = 1;
        this.label1.Text = "＜使用方法＞\r\n① ポーズファイル(.tdcgpose.png)(.tmo)をドラッグ＆ドロップ\r\n② ”ポーズを変換”ボタンを押すと、変換されます\r\n③ MMD" +
            "上にて、IKをOFFにして、使用ください";
        // 
        // TSOForm
        // 
        this.ClientSize = new System.Drawing.Size(284, 263);
        this.Controls.Add(this.label1);
        this.Controls.Add(this.button1);
        this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
        this.Name = "TSOForm";
        this.Text = "Tmo2Vpd";
        this.ResumeLayout(false);
        this.PerformLayout();

    }

    private void button1_Click(object sender, EventArgs e)
    {
        Figure fig;
        viewer.TryGetFigure(out fig);

        // 変換し、保存する
        using (SaveFileDialog sfd = new SaveFileDialog())
        {
            //タイトルを設定する
            sfd.Title = "保存先のファイルを選択してください";
            //はじめのファイル名を指定する
            sfd.FileName = "新しいファイル.vpd";
            //[ファイルの種類]に表示される選択肢を指定
            sfd.Filter = "*.vpd|*.vpd|すべてのファイル|*.*";
            //[ファイルの種類]ではじめに選択される選択肢
            sfd.FilterIndex = 1;
            //ダイアログボックスを閉じる前に現在のディレクトリを復元するようにする
            sfd.RestoreDirectory = true;

            if (sfd.ShowDialog() != DialogResult.OK) return;

            tbr.SaveVpd(fig.Tmo, sfd.FileName);
        }
    }
}
}
