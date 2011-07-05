using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TDCGUtils
{
    public partial class ReferFileName : UserControl
    {
        string file_name = "";
        string filter = ".png";
        bool enabled = true;

        public string Title { set {groupBox1.Text = value; } get { return groupBox1.Text; } }
        public string FileName { set { file_name = value; } get { return file_name; } }
        public string Filter { set { filter = value; } get { return filter; } }
        public bool UserEnabled
        {
            set
            {
                enabled = value;
                textBox1.Enabled = value;
                button1.Enabled = value;
                panel1.AllowDrop = value;
            }
            get { return enabled; }
        }

        public ReferFileName()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                //タイトル
                ofd.Title = "開くファイルを選択してください";
                //[ファイルの種類]に表示される選択肢を指定
                ofd.Filter = "*" + filter + "|*" + filter + "|すべてのファイル|*.*";
                //[ファイルの種類]ではじめに選択される選択肢
                ofd.FilterIndex = 1;
                //ダイアログボックスを閉じる前に現在のディレクトリを復元するようにする
                ofd.RestoreDirectory = true;
                //ファイルの複数選択を可とするか？
                ofd.Multiselect = false;

                //ダイアログを開く
                if (ofd.ShowDialog() != DialogResult.OK)
                {
                    return;
                }
                textBox1.Text = ofd.FileName;
                file_name = ofd.FileName;
            }
        }

        private void panel1_DragDrop(object sender, DragEventArgs e)
        {
            //ドロップされたデータがファイルか調べる
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] fileName
                    = (string[])e.Data.GetData(DataFormats.FileDrop);

                // 拡張子が適正であるか確認する
                if (System.IO.Path.GetExtension(fileName[0]) == filter)
                {
                    textBox1.Text = fileName[0];
                    file_name = fileName[0];
                }
                else
                {
                    MessageBox.Show("拡張子が適正ではありません。");
                }
            }
        }

        private void panel1_DragEnter(object sender, DragEventArgs e)
        {
            //コントロール内にドラッグされたとき実行される
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                //ドラッグされたデータ形式を調べ、ファイルのときはコピーとする
                e.Effect = DragDropEffects.Copy;
            else
                //ファイル以外は受け付けない
                e.Effect = DragDropEffects.None;
        }
    }
}
