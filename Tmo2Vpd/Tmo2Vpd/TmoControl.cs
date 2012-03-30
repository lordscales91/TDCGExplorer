using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using TDCG;
using TDCGUtils;

namespace Tmo2Vpd
{
    public partial class TmoControl : UserControl
    {
        public TmoControl()
        {
            InitializeComponent();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            // お待ちくださいダイアログを表示
            ProgressDialog pd = new ProgressDialog();
            pd.Show();
            pd.Message = "ファイルを変換しています。";

            try
            {
                // お待ちくださいダイアログを閉じる
                pd.Value = pd.Maximum;
                System.Threading.Thread.Sleep(1000);
                pd.Dispose();

                // 変換し、保存する
                using (SaveFileDialog sfd = new SaveFileDialog())
                {
                    //タイトルを設定する
                    sfd.Title = "保存先のファイルを選択してください";
                    //はじめのファイル名を指定する
                    sfd.FileName = "新しいファイル.vpd";
                    //[ファイルの種類]に表示される選択肢を指定
                    sfd.Filter = "*.vpd|*.vpd|すべてのファイル|*.*";
                    //[ファイルの種類]ではじめに選択される選択肢
                    sfd.FilterIndex = 1;
                    //ダイアログボックスを閉じる前に現在のディレクトリを復元するようにする
                    sfd.RestoreDirectory = true;

                    if (sfd.ShowDialog() != DialogResult.OK) return;

                    //tmo.Save(sfd.FileName);
                }

                // メッセージボックスを表示する
                MessageBox.Show("変換を完了しました。");
            }
            catch
            {
                // お待ちくださいダイアログを閉じる
                pd.Dispose();

                MessageBox.Show("エラーにより、変換できませんでした。");
            }
        }
    }
}
