using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
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

        TSOConfig tso_config;

        string tso_config_file = Path.Combine(Application.StartupPath, @"config.xml");
        if (File.Exists(tso_config_file))
            tso_config = TSOConfig.Load(tso_config_file);
        else
            tso_config = new TSOConfig();

        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new TSOForm(tso_config, args));
    }
}
}
