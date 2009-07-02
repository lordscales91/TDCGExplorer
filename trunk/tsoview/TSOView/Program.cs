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
using TDCG;

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
    void Hello(Viewer viewer);
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

        using (Viewer viewer = new Viewer())
        using (FigureForm fig_form = new FigureForm())
        using (TSOForm form = new TSOForm(config))
        {
            viewer.fig_form = fig_form;

            if (viewer.InitializeApplication(form))
            {
                foreach (string arg in args)
                    viewer.LoadAnyFile(arg);

                var script = CSScript.Load(Path.Combine(Application.StartupPath, "Script.cs")).CreateInstance("TDCG.Script").AlignToInterface<IScript>();
                script.Hello(viewer);

                form.Show();
                long wait = (long)(10000000.0f/60.0f);
                long nextTicks = DateTime.Now.Ticks;
                // While the form is still valid, render and process messages
                while (form.Created)
                {
                    if (DateTime.Now.Ticks >= nextTicks)
                    {
                        viewer.FrameMove();
                        if (DateTime.Now.Ticks < nextTicks + wait)
                            viewer.Render();
                        nextTicks += wait;
                    }
                    Application.DoEvents();
                }
            }

        }
    }
}
}
