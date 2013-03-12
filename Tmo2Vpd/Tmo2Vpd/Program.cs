using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.IO;

using TSOView;

namespace Tmo2Vpd
{
    static class Program
    {
        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main()
        {
            TSOConfig tso_config;

            string tso_config_file = Path.Combine(Application.StartupPath, @"config.xml");
            if (File.Exists(tso_config_file))
                tso_config = TSOConfig.Load(tso_config_file);
            else
                tso_config = new TSOConfig();

            string[] str = { Path.Combine(Application.StartupPath, @"default.tdcgsav.png") };

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new TSOForm(tso_config, str));
        }
    }
}
