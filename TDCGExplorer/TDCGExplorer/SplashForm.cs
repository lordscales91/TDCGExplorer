using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TDCGExplorer
{
    public partial class SplashForm : Form
    {
        public SplashForm()
        {
            StartPosition = FormStartPosition.CenterScreen;

            InitializeComponent();

            Bitmap bitmap = new Bitmap("title.jpg");
            pictureBox.Image = bitmap;
        }

        //Splashフォーム
        private static SplashForm _form = null;
        //Splashを表示するスレッド
        private static System.Threading.Thread _thread = null;
        //lock用のオブジェクト
        private static readonly object syncObject = new object();

        /// <summary>
        /// Splashフォーム
        /// </summary>
        public static SplashForm Form
        {
            get { return _form; }
        }

        /// <summary>
        /// Splashフォームを表示する
        /// </summary>
        /// <param name="mainForm">メインフォーム</param>
        public static void ShowSplash()
        {
            if (_form != null || _thread != null)
                return;

            //スレッドの作成
            _thread = new System.Threading.Thread(
                new System.Threading.ThreadStart(StartThread));
            _thread.Name = "SplashForm";
            _thread.IsBackground = true;
            _thread.ApartmentState = System.Threading.ApartmentState.STA;
            //スレッドの開始
            _thread.Start();
        }

        /// <summary>
        /// Splashフォームを消す
        /// </summary>
        public static void CloseSplash()
        {
            lock (syncObject)
            {
                if (_form != null && _form.IsDisposed == false)
                {
                    //Splashフォームを閉じる
                    //Invokeが必要か調べる
                    if (_form.InvokeRequired)
                        _form.Invoke(new MethodInvoker(_form.Close));
                    else
                        _form.Close();
                }


                _form = null;
                _thread = null;
            }
        }

        //スレッドで開始するメソッド
        private static void StartThread()
        {
            //Splashフォームを作成
            _form = new SplashForm();
            //Splashフォームをクリックして閉じられるようにする
            _form.Click += new EventHandler(_form_Click);
            //Splashフォームを表示する
            Application.Run(_form);
        }

        //Splashフォームがクリックされた時
        private static void _form_Click(object sender, EventArgs e)
        {
            //Splashフォームを閉じる
            CloseSplash();
        }
    }
}
