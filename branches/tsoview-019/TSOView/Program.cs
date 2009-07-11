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

        if (File.Exists(@"config.xml"))
            config = TSOConfig.Load(@"config.xml");
        else
            config = new TSOConfig();

        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new TSOForm(config, args));
    }
}
}
