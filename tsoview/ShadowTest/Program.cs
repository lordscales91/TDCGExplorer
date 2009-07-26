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

namespace ShadowTest
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
        this.FormBorderStyle = FormBorderStyle.FixedSingle;
        this.ClientSize = new Size(640, 480);
        this.Text = "ShadowTest";

        this.MaximumSize = this.Size;
        this.MinimumSize = this.Size;
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

            device.DeviceResizing += new CancelEventHandler(CancelResize);
        }
        catch (DirectXException ex)
        {
            Console.WriteLine("Error: " + ex);
            return false;
        }
        return true;
    }

    private void CancelResize(object sender, CancelEventArgs e)
    {
        Console.WriteLine("CancelResize");
        e.Cancel = true;
    }

    public void Render()
    {
        device.BeginScene();

        device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.CornflowerBlue, 1.0f, 0);

        device.EndScene();

        device.Present();
        Thread.Sleep(30);
    }

    public void Dispose()
    {
        if (device != null)
            device.Dispose();
    }
}
}
