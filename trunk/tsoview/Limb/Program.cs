using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.ComponentModel;
using System.Windows.Forms;
using System.IO;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using Direct3D=Microsoft.DirectX.Direct3D;

namespace Limb
{

public static class Program
{
    [STAThread]
    static void Main(string[] args) 
    {
        Debug.Listeners.Add(new TextWriterTraceListener(Console.Out));

        using (Viewer viewer = new Viewer())
        using (ViewerForm form = new ViewerForm())
        {
            form.viewer = viewer;

            if (viewer.InitializeApplication(form))
            {
                form.Show();

                while (form.Created)
                {
                    form.FrameMove();
                    viewer.Render();
                    Application.DoEvents();
                }
            }
        }
    }
}

public class ViewerForm : Form
{
    // ÉLÅ[ì¸óÕÇï€éù
    internal bool[] keys = new bool[256];
    internal bool[] keysEnabled = new bool[256];

    internal Viewer viewer = null;

    public ViewerForm()
    {
        this.FormBorderStyle = FormBorderStyle.FixedSingle;
        this.ClientSize = new Size(640, 480);
        this.Text = "Real-time inverse kinematics techniques for anthropomorphic limbs";

        this.MaximumSize = this.Size;
        this.MinimumSize = this.Size;

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
        float keyL = 0.0f;
        float keyR = 0.0f;
        float keyU = 0.0f;
        float keyD = 0.0f;
        float keyPush = 0.0f;
        float keyPull = 0.0f;
        float keySwivel = 0.0f;

        if (keys[(int)Keys.Left])
            keyL = 0.02f;
        if (keys[(int)Keys.Right])
            keyR = 0.02f;
        if (keys[(int)Keys.PageUp])
            keyU = 0.02f;
        if (keys[(int)Keys.PageDown])
            keyD = 0.02f;
        if (keys[(int)Keys.Up])
            keyPush = 0.02f;
        if (keys[(int)Keys.Down])
            keyPull = 0.02f;
        if (keys[(int)Keys.A])
            keySwivel = -2.0f;
        if (keys[(int)Keys.D])
            keySwivel = +2.0f;

        viewer.MoveTarget(keyL - keyR, keyD - keyU, keyPush - keyPull);
        viewer.MoveSwivel(keySwivel);
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


public class Viewer : IDisposable
{
    internal Device device;
    internal Effect effect;

    Vector3 target = new Vector3(1.0f, 1.0f, 1.0f);
    float swivel = 0.0f;

    private Vector3 eye;
    private Matrix view;
    private Matrix proj;
    MatrixStack stack = new MatrixStack();
    Mesh cylinder = null;
    Mesh sphere = null;

    public void MoveTarget(float dz, float dy, float dx)
    {
        target.X += dx;
        target.Y += dy;
        target.Z += dz;
    }

    public void MoveSwivel(float delta)
    {
        swivel += delta;
    }

    public bool InitializeApplication(Control control)
    {
        PresentParameters pp = new PresentParameters();
        try
        {
            pp.Windowed = true;
            pp.SwapEffect = SwapEffect.Discard;
            pp.BackBufferFormat = Format.X8R8G8B8;
            pp.BackBufferCount = 1;
            pp.EnableAutoDepthStencil = true;
            pp.AutoDepthStencilFormat = DepthFormat.D16;

            int adapter_ordinal = Manager.Adapters.Default.Adapter;

            int ret, quality;
            if (Manager.CheckDeviceMultiSampleType(adapter_ordinal, DeviceType.Hardware, pp.BackBufferFormat, pp.Windowed, MultiSampleType.FourSamples, out ret, out quality))
            {
                pp.MultiSample = MultiSampleType.FourSamples;
                pp.MultiSampleQuality = quality - 1;
            }

            CreateFlags flags = CreateFlags.SoftwareVertexProcessing;
            Caps caps = Manager.GetDeviceCaps(adapter_ordinal, DeviceType.Hardware);
            if (caps.DeviceCaps.SupportsHardwareTransformAndLight)
                flags = CreateFlags.HardwareVertexProcessing;
            if (caps.DeviceCaps.SupportsPureDevice)
                flags |= CreateFlags.PureDevice;
            device = new Device(adapter_ordinal, DeviceType.Hardware, control.Handle, flags, pp);
        }
        catch (DirectXException ex)
        {
            Console.WriteLine("Error: " + ex);
            return false;
        }

        string effect_file = Path.Combine(Application.StartupPath, @"tmps.fx");
        if (! File.Exists(effect_file))
        {
            Console.WriteLine("File not found: " + effect_file);
            return false;
        }
        using (FileStream effect_stream = File.OpenRead(effect_file))
        {
            string compile_error;
            effect = Effect.FromStream(device, effect_stream, null, ShaderFlags.None, null, out compile_error);
            if (compile_error != null)
            {
                Console.WriteLine(compile_error);
                return false;
            }
        }
        effect.Technique = "PhongShader";

        eye = new Vector3(5,0,0);
        view = Matrix.LookAtLH( eye, new Vector3(0,0,0), new Vector3(0,1,0));
        proj = Matrix.PerspectiveFovLH( Geometry.DegreeToRadian(45), (float)device.Viewport.Width / (float)device.Viewport.Height, 1.0f, 100.0f );

        effect.SetValue("g_vEyePos", new Vector4(eye.X, eye.Y, eye.Z, 1));

        cylinder = Mesh.Cylinder(device, 0.1f, 0.01f, 1.0f, 8, 6);
        sphere = Mesh.Sphere(device, 0.12f, 8, 6);

        return true;
    }

    public void DrawMeshSub(Mesh mesh, Matrix world, Vector4 color)
    {
        Matrix wvp = world * view * proj;
        effect.SetValue("g_mWorld", world);
        effect.SetValue("g_mWorldIT", Matrix.TransposeMatrix(Matrix.Invert(world)));
        effect.SetValue("g_mWorldViewProj", wvp);

        effect.SetValue("g_vSurfColor", color);

        int npass = effect.Begin(0);
        for (int ipass = 0; ipass < npass; ipass++)
        {
            effect.BeginPass(ipass);
            mesh.DrawSubset(0);
            effect.EndPass();
        }
        effect.End();
    }

    public void Render()
    {
        Matrix R1;
        Matrix Ry;
        Matrix R2;

        Solver solver = new Solver(1.0f, 1.0f);
        Matrix target_m = Matrix.Translation(target);
        solver.Solve(out R1, out Ry, out R2, ref target_m, swivel);

        device.BeginScene();
        device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.CornflowerBlue, 1.0f, 0);

        // R1ä÷êﬂ
        stack.LoadIdentity();
        stack.MultiplyMatrixLocal(R1);
        stack.TranslateLocal(0.0f, 0.0f, 0.5f);
        DrawMeshSub(cylinder, stack.Top, new Vector4(0.5f, 0.0f, 0.0f, 1.0f));

        // Ryä÷êﬂ
        stack.TranslateLocal(0.0f, 0.0f, 0.5f);
        stack.MultiplyMatrixLocal(Ry);
        stack.TranslateLocal(0.0f, 0.0f, 0.5f);
        DrawMeshSub(cylinder, stack.Top, new Vector4(0.0f, 0.5f, 0.0f, 1.0f));

        // R2ä÷êﬂ
        stack.TranslateLocal(0.0f, 0.0f, 0.5f);
        stack.MultiplyMatrixLocal(R2);
        stack.TranslateLocal(0.0f, 0.0f, 0.5f);
        DrawMeshSub(cylinder, stack.Top, new Vector4(0.0f, 0.0f, 0.5f, 1.0f));

        stack.LoadIdentity();
        stack.Translate(target);
        DrawMeshSub(sphere, stack.Top, new Vector4(0.5f, 0.5f, 0.0f, 1.0f));

        device.EndScene();

        device.Present();
        Thread.Sleep(1);
    }

    public void Dispose()
    {
        if (sphere != null)
            sphere.Dispose();
        if (cylinder != null)
            cylinder.Dispose();
        if (effect != null)
            effect.Dispose();
        if (device != null)
            device.Dispose();
    }
}
}
