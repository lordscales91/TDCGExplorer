using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace ShaderTest
{
class Program : IDisposable
{
    static void Main(string[] args)
    {
        using (Program program = new Program())
        using (TMOForm form = new TMOForm())
        if (program.InitializeApplication(form))
        {
            Console.WriteLine("Test succeeded.");
            /*
            // While the form is still valid, render and process messages
            while (form.Created)
            {
                Application.DoEvents();
            }
            */
        }
        else
        {
            Console.WriteLine("Test failed.");
        }
    }

    internal Device device;
    internal Effect effect;

    public bool InitializeApplication(TMOForm form)
    {
        PresentParameters pp = new PresentParameters();
        try
        {
            // Set up the structure used to create the D3DDevice. Since we are now
            pp.Windowed = true;
            pp.SwapEffect = SwapEffect.Discard;
            pp.BackBufferFormat = Format.X8R8G8B8;
            pp.BackBufferCount = 1;
            pp.EnableAutoDepthStencil = true;
            pp.AutoDepthStencilFormat = DepthFormat.D16;

            int ret, quality;
            if (Manager.CheckDeviceMultiSampleType((int)Manager.Adapters.Default.Adapter, DeviceType.Hardware, pp.BackBufferFormat, pp.Windowed, MultiSampleType.FourSamples, out ret, out quality))
            {
                pp.MultiSample = MultiSampleType.FourSamples;
                pp.MultiSampleQuality = quality - 1;
            }

            // Create the D3DDevice
            device = new Device(0, DeviceType.Hardware, form.Handle, CreateFlags.HardwareVertexProcessing, pp);

        }
        catch (DirectXException ex)
        {
            Console.WriteLine("Error: " + ex);
            return false;
        }
        string effect_file = @"toonshader.cgfx";
        if (! File.Exists(effect_file))
        {
            Console.WriteLine("File not found: " + effect_file);
            return false;
        }
        string compile_error;
        effect = Effect.FromFile(device, effect_file, null, ShaderFlags.None, null, out compile_error);
        if (compile_error != null)
        {
            Console.WriteLine(compile_error);
            return false;
        }

        Console.WriteLine("Parameters:");
        int nparam = effect.Description.Parameters;
        for (int i = 0; i < nparam; i++)
        {
            EffectHandle param = effect.GetParameter(null, i);
            DumpParameter(param, i);
        }

        Console.WriteLine("Techniques:");
        int ntech = effect.Description.Techniques;
        for (int i = 0; i < ntech; i++)
        {
            EffectHandle tech = effect.GetTechnique(i);
            DumpTechnique(tech, i);
        }

        Console.WriteLine("Valid techniques:");
        {
            EffectHandle tech = null;
            while ((tech = effect.FindNextValidTechnique(tech)) != null)
            {
                TechniqueDescription td = effect.GetTechniqueDescription(tech);
                Console.WriteLine("\t{0}", td.Name);
            }
        }
        return true;
    }

    public void DumpParameter(EffectHandle param, int index)
    {
        ParameterDescription pd = effect.GetParameterDescription(param);

        if (pd.Semantic != null)
            Console.WriteLine("{0}\t{1} {2} : {3}", index, pd.Type, pd.Name, pd.Semantic);
        else
            Console.WriteLine("{0}\t{1} {2}", index, pd.Type, pd.Name);

        int nannotation = pd.Annotations;
        if (nannotation > 0)
        {
            Console.WriteLine("\tAnnotations:");
            for (int i = 0; i < nannotation; i++)
            {
                EffectHandle annotation = effect.GetAnnotation(param, i);
                ParameterDescription ad = effect.GetParameterDescription(annotation);
                Console.WriteLine("\t{0}\t{1} {2}", i, ad.Type, ad.Name);
            }
        }
    }

    public void DumpTechnique(EffectHandle tech, int index)
    {
        TechniqueDescription td = effect.GetTechniqueDescription(tech);

        Console.WriteLine("{0}\t{1}", index, td.Name);

        int nannotation = td.Annotations;
        if (nannotation > 0)
        {
            Console.WriteLine("\tAnnotations:");
            for (int i = 0; i < nannotation; i++)
            {
                EffectHandle annotation = effect.GetAnnotation(tech, i);
                ParameterDescription ad = effect.GetParameterDescription(annotation);
                Console.WriteLine("\t{0,3} {1} {2}", i, ad.Type, ad.Name);
            }
        }
    }

    public void Dispose()
    {
        if (effect != null)
            effect.Dispose();
        if (device != null)
            device.Dispose();
    }
}

public class TMOForm : Form
{
    public TMOForm()
    {
        this.ClientSize = new System.Drawing.Size(640, 480);
        this.Text = "TMOView";
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
