using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

using TDCG;
using TDCGUtils;

namespace Vmd2Tmo
{
    public partial class Form1 : Form
    {
        Viewer viewer = null;
        Vmd2Tmo v2t = null;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                // Viewerクラスを初期化
                viewer = new Viewer(false);

                // Tso2Pmdクラスを初期化
                v2t = new Vmd2Tmo();
            }
            catch
            {
                MessageBox.Show("Vmd2Tmoを正常に起動できませんでした。\nProportionファルダや表情フォルダに、\n不正なファイルが含まれていないか確認してください。");

                this.Dispose();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            // お待ちくださいダイアログを表示
            ProgressDialog pd = new ProgressDialog();
            pd.Show();
            pd.Message = "ファイルを変換しています。";

            try
            {
                // MMDPlayerを設定する
                v2t.initialize(referFileName2.FileName, referFileName3.FileName);

                // ヘビーセーブファイルを開く
                viewer.ClearFigureList(); // 前回読み込んだフィギュアを消去
                List<Figure> fig_list = viewer.LoadPNGFile(Path.Combine(Application.StartupPath, @"default.tdcgsav.png"));
                Figure fig = fig_list[0]; // 名前が長いので省略
                fig.UpdateNodeMapAndBoneMatrices();

                // 初期ポーズのTMOを得ておく
                v2t.SetInitPose(fig);

                // offsetをセット
                TMOFile tmo = fig.Tmo; // 名前が長いので省略
                v2t.Offset_position = tmo.nodes[0].Translation;

                // 初期フレームのポーズを得る
                v2t.GetRotation(ref tmo);
                tmo.SaveTransformationMatrixToFrameAdd();

                // 全フレームをtmoにコピー
                float fDiffTime = 30.0f / 60.0f;
                for (float iTime = 0.0f; iTime < v2t.Vmd.getMaxFrame(); iTime += fDiffTime)
                {
                    v2t.FrameMove(fDiffTime);
                    v2t.GetRotation(ref tmo);
                    tmo.SaveTransformationMatrixToFrameAdd(); // FrameにボーンのMatrixを保存

                    if ((int)(iTime / fDiffTime) % 10 == 0)
                    {
                        pd.Value = (int)((iTime / v2t.Vmd.getMaxFrame()) * (pd.Maximum - pd.Minimum));
                        this.Update();
                        System.Threading.Thread.Sleep(10);
                    }
                }

                // nodeのmatricesを、frameのmatricesにそっくりコピー
                tmo.NodeMatrices2FrameMatrices(2);

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
                    sfd.FileName = "新しいファイル.tmo";
                    //[ファイルの種類]に表示される選択肢を指定
                    sfd.Filter = "*.tmo|*.tmo|すべてのファイル|*.*";
                    //[ファイルの種類]ではじめに選択される選択肢
                    sfd.FilterIndex = 1;
                    //ダイアログボックスを閉じる前に現在のディレクトリを復元するようにする
                    sfd.RestoreDirectory = true;

                    if (sfd.ShowDialog() != DialogResult.OK) return;

                    tmo.Save(sfd.FileName);
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
