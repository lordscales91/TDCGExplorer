using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Tso2MqoGui
{
    static class Program
    {
        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static int Main(string[] args)
        {
            if(args.Length != 0)
            {   // バッチで処理する
                try
                {
                    string  tso = null;
                    string  mqo = null;
                    string  rtso= null;

                    foreach(string i in args)
                    {
                        string  o   = i.ToLower();

                        switch(o)
                        {
                        default:
                                 if(o.StartsWith("-tso:"))  tso = o.Substring(5).Trim('\r', '\n');
                            else if(o.StartsWith("-mqo:"))  mqo = o.Substring(5).Trim('\r', '\n');
                            else if(o.StartsWith("-ref:"))  rtso= o.Substring(5).Trim('\r', '\n');
                            else throw new ArgumentException("Invalid option: " + i);
                            break;
                        }
                    }

                    TSOGenerator        gen     = new TSOGenerator();
                    TSOGenerateConfig   config  = new TSOGenerateConfig();
                    config.cui                  = true;
                    config.ShowMaterials       = false;

                    if(mqo  == null) throw new ArgumentException("「-mso:ファイル名」の形式で入力Mqoファイル名を指定してください");
                    if(tso  == null) throw new ArgumentException("「-tso:ファイル名」の形式で出力Tsoファイル名を指定してください");
                    if(rtso == null) throw new ArgumentException("「-ref:ファイル名」の形式で参照Tsoファイル名を指定してください");

                    gen.GenerateRefBone(mqo, rtso, tso, config);
                } catch(ArgumentException e)
                {
                    System.Diagnostics.Debug.WriteLine(e.Message);
                    System.Console.Out.WriteLine(e.Message);
                    System.Console.Out.Flush();
                    return 1;
                } catch(Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e.Message);
                    System.Console.Out.WriteLine(e.Message);
                    System.Console.Out.Flush();
                    return 1;
                }

                return 0;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());

            return 0;
        }
    }
}