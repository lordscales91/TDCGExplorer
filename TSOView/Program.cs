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

public interface IScript
{
    void Hello(Viewer viewer);
}

static class Program
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
            form.fig_form = fig_form;
            form.viewer = viewer;

            if (viewer.InitializeApplication(form, true))
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
                        form.FrameMove();
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
