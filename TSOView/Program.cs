using System;
using System.Collections.Generic;
using System.Diagnostics;
//using System.Drawing;
//using System.Threading;
//using System.ComponentModel;
using System.Windows.Forms;
using System.IO;
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

        string config_file = Path.Combine(Application.StartupPath, @"config.xml");
        if (File.Exists(config_file))
            config = TSOConfig.Load(config_file);
        else
            config = new TSOConfig();

        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new TSOForm(config, args));
    }
}
}
