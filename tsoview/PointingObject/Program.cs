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

namespace PointingObject
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
            if (viewer.InitializeApplication(form))
            {
                form.Show();

                while (form.Created)
                {
                    viewer.Render();
                    Application.DoEvents();
                }
            }
        }
    }
}

public class ViewerForm : Form
{
    public ViewerForm()
    {
        this.ClientSize = new Size(640, 480);
        this.Text = "PointingObject";
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

        device.RenderState.Lighting = true;

        eye = new Vector3(0,0,-5);
        view = Matrix.LookAtLH( eye, new Vector3(0,0,0), new Vector3(0,1,0));
        proj = Matrix.PerspectiveFovLH( Geometry.DegreeToRadian(45), (float)device.Viewport.Width / (float)device.Viewport.Height, 1.0f, 1000.0f );

        device.Transform.View = view;
        device.Transform.Projection = proj;

        mesh = Mesh.Box(device, 10.0f, 10.0f, 10.0f);

        return true;
    }

    Vector4[] colors = new Vector4[3]{
        new Vector4(1.0f, 0.0f, 0.0f, 1.0f),
        new Vector4(0.0f, 1.0f, 0.0f, 1.0f),
        new Vector4(0.0f, 0.0f, 1.0f, 1.0f)
    };
    private Vector3 eye;
    private Matrix view;
    private Matrix proj;
    private Mesh mesh = null;
    private float tick = 0.0f;

    public void Render()
    {
        device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.CornflowerBlue, 1.0f, 0);

        device.BeginScene();

        for (int i = 0; i < 3; i++)
        {
            float rad = Geometry.DegreeToRadian(i*50);

            Matrix world = Matrix.Identity*
                Matrix.RotationX(Geometry.DegreeToRadian(tick*(i+1)/3))*
                Matrix.RotationY(Geometry.DegreeToRadian(tick*(i+1)/2))*
                Matrix.Translation( 8*(float)Math.Sin(rad + tick/100/(i+1)), 12*(float)Math.Cos(rad + tick/100/(i+1)), 50*(float)(1+Math.Sin(rad)));

            //device.Transform.World = world;

            Matrix wvp = world * view * proj;
            effect.SetValue("g_mWorld", world);
            effect.SetValue("g_mWorldIT", Matrix.TransposeMatrix(Matrix.Invert(world)));
            effect.SetValue("g_vEyePos", new Vector4(eye.X, eye.Y, eye.Z, 1));
            effect.SetValue("g_mWorldViewProj", wvp);

            effect.SetValue("g_vSurfColor", colors[i]);

            int npass = effect.Begin(0);
            for (int ipass = 0; ipass < npass; ipass++)
            {
                effect.BeginPass(ipass);
                mesh.DrawSubset(0);
                effect.EndPass();
            }
            effect.End();
        }
        device.EndScene();

        device.Present();
        Thread.Sleep(1);
        tick += 1.0f;
    }

    public void Dispose()
    {
        if (mesh != null)
            mesh.Dispose();
        if (effect != null)
            effect.Dispose();
        if (device != null)
            device.Dispose();
    }
}
}
