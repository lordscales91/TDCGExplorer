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
using Direct3D=Microsoft.DirectX.Direct3D;
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
    internal int keyCameraSlerp = (int)Keys.C;
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

    private int cam_frame_index = 0;

    public TSOForm(TSOConfig config)
    {
        this.ClientSize = config.ClientSize;
        this.Text = "TSOView";
        this.AllowDrop = true;

        for (int i = 0; i < keysEnabled.Length; i++)
        {
            keysEnabled[i] = true;
        }
        this.KeyDown += new KeyEventHandler(form_OnKeyDown);
        this.KeyUp += new KeyEventHandler(form_OnKeyUp);
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
            viewer.SwitchShadowEnabled();
        }
        if (keysEnabled[keySprite] && keys[keySprite])
        {
            keysEnabled[keySprite] = false;
            viewer.SwitchSpriteEnabled();
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
        if (keysEnabled[keyCameraSlerp] && keys[keyCameraSlerp])
        {
            keysEnabled[keyCameraSlerp] = false;
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
            keyL = 1.0f;
        if (keys[(int)Keys.Right])
            keyR = 1.0f;
        if (keys[(int)Keys.PageUp])
            keyU = 1.0f;
        if (keys[(int)Keys.PageDown])
            keyD = 1.0f;
        if (keys[(int)Keys.Up])
            keyPush = 1.0f;
        if (keys[(int)Keys.Down])
            keyPull = 1.0f;
        if (keys[(int)Keys.A])
            keyZRol = -1.0f;
        if (keys[(int)Keys.D])
            keyZRol = +1.0f;

        camera.Move(keyL - keyR, keyD - keyU, keyPush - keyPull);
        camera.RotZ(Geometry.DegreeToRadian(keyZRol));
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
}
}
