using System;
using System.Collections.Generic;
using System.Diagnostics;
//using System.Drawing;
//using System.Threading;
//using System.ComponentModel;
using System.Windows.Forms;
using System.IO;
//using Microsoft.DirectX;
//using Microsoft.DirectX.Direct3D;
//using Direct3D=Microsoft.DirectX.Direct3D;
using CSScriptLibrary;
using TAHdecrypt;

namespace TSOView
{

public class TSOForm : Form
{
    public TSOForm(TSOConfig config)
    {
        this.ClientSize = config.ClientSize;
        this.Text = "TSOView";
        this.AllowDrop = true;
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

public interface IScript
{
    void Hello(TSOSample sample);
}

static class TSOView
{
    [STAThread]
    static void Main(string[] args) 
    {
        Debug.Listeners.Add(new TextWriterTraceListener(Console.Out));

        TSOConfig config;

        if (File.Exists(@"config.xml"))
            config = TSOConfig.Load(@"config.xml");
        else
            config = new TSOConfig();

        using (TSOSample sample = new TSOSample())
        using (TSOFigureForm fig_form = new TSOFigureForm())
        using (TSOForm form = new TSOForm(config))
        {
            sample.fig_form = fig_form;

            if (sample.InitializeApplication(form))
            {
                foreach (string arg in args)
                    sample.LoadAnyFile(arg);

                var script = CSScript.Load("Script.cs").CreateInstance("TAHdecrypt.Script").AlignToInterface<IScript>();
                script.Hello(sample);

                form.Show();
                long wait = (long)(10000000.0f/60.0f);
                long nextTicks = DateTime.Now.Ticks;
                // While the form is still valid, render and process messages
                while (form.Created)
                {
                    if (DateTime.Now.Ticks >= nextTicks)
                    {
                        sample.FrameMove();
                        if (DateTime.Now.Ticks < nextTicks + wait)
                            sample.Render();
                        nextTicks += wait;
                    }
                    Application.DoEvents();
                }
            }

        }
    }
}
}
