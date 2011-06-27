using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

using TDCG;
using TDCGUtils;
using TSOView;

namespace Tso2Pmd
{
    public partial class MainForm : Form
    {
        TemplateList template_list = null;
        Viewer viewer = null;
        TSOForm view_form = null;
        TransTso2Pmd t2p = null;
        ClippingVertexForm clip_form = null;

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            // お待ちくださいダイアログを表示
            ProgressDialog pd = new ProgressDialog();
            pd.Show();
            pd.Message = "Tso2Pmdを起動しています。";

            //try
            //{

                // テンプレートリストを初期化
                template_list = new TemplateList();
                pd.Value += 15;

                // Viewerクラスを初期化
                viewer = new Viewer();
                pd.Value += 15;

                // Tso2Pmdクラスを初期化
                t2p = new TransTso2Pmd();
                pd.Value += 15;

                // スプリクトを読みとる
                if (!template_list.Load())
                {
                    pd.Dispose();
                    this.Dispose();
                }
                pd.Value += 30;

                // T2POptionControlの初期化
                t2POptionControl1.Initialize(ref viewer, template_list);
                pd.Value += 20;

                // ビューアーフォームの初期化
                view_form = new TSOForm(viewer, this);

                // お待ちくださいダイアログを閉じる
                pd.Value = pd.Maximum;
                System.Threading.Thread.Sleep(1000);
                pd.Dispose();

            /*}
            catch
            {
                // お待ちくださいダイアログを閉じる
                pd.Dispose();

                MessageBox.Show("Tso2Pmdを正常に起動できませんでした。\nProportionファルダや表情フォルダに、\n不正なファイルが含まれていないか確認してください。");

                this.Dispose();
            }*/
        }

        private void panel1_DragEnter(object sender, DragEventArgs e)
        {
            //コントロール内にカーソルがドラッグされたとき、表示されるものを選択
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                // Ctrlを押している状態を判別しない
                e.Effect = DragDropEffects.Copy;

                /*// Ctrlを押している状態なら、+を表示する
                if ((e.KeyState & 8) == 8)
                    e.Effect = DragDropEffects.Copy;
                else
                    e.Effect = DragDropEffects.Move;*/
            }
            else
            {
                //ファイル以外は受け付けない
                e.Effect = DragDropEffects.None;
            }

            panel1.BackColor = Color.DarkTurquoise;
        }

        private void panel1_DragLeave(object sender, EventArgs e)
        {
            panel1.BackColor = Color.LightCyan;
        }

        public void panel1_DragDrop(object sender, DragEventArgs e)
        {
            //ドロップされたデータがファイルか調べる
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] FileName
                    = (string[])e.Data.GetData(DataFormats.FileDrop);

                string name = FileName[0];

                // 拡張子が".tdcgsav.png"or".tso"であるか確認する
                if ((System.IO.Path.GetExtension(name) == ".png" &&
                    System.IO.Path.GetExtension(
                        System.IO.Path.GetFileNameWithoutExtension(name)) == ".tdcgsav")
                    || System.IO.Path.GetExtension(name) == ".tso")
                {
                    // お待ちくださいダイアログを表示
                    ProgressDialog pd = new ProgressDialog();
                    pd.Show(this);
                    pd.Message = "ファイルを読み込んでいます。";

                    //try {

                        //ドロップされたファイルを読み込む
                        viewer.ClearFigureList();
                        viewer.LoadAnyFile(name, false);

                        // フォームより各パラメータを得て、設定
                        SetupFigure(name);

                        // お待ちくださいダイアログを閉じる
                        pd.Value = pd.Maximum;
                        System.Threading.Thread.Sleep(1000);
                        pd.Dispose();

                    /*}
                    catch
                    {
                        // お待ちくださいダイアログを閉じる
                        pd.Dispose();

                        MessageBox.Show("ファイルを読み込むことができませんでした。\nファイルが正常であるか確認してください。");
                    }*/
                }
                else
                {
                    MessageBox.Show("拡張子が.tdcgsav.pngか、.tsoである必要があります。");
                    return;
                }
            }

            panel1.BackColor = Color.LightCyan;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // フォームを表示
            view_form.Show();

            button2.Enabled = false;
        }

        // TSOFormが閉じた時（実際はHide）
        public void TSOForm_Hiding()
        {
            button2.Enabled = true;
        }

        private void button_Clipping_Click(object sender, EventArgs e)
        {
            // ClippingVertexFormを表示する
            clip_form.Show();

            button_Clipping.Enabled = false;
        }

        // ClippingVertexが閉じた時（実際はHide）
        public void ClippingVertexForm_Hiding()
        {
            button_Clipping.Enabled = true;
        }

        // フォームより各パラメータを得て、設定
        public void SetupFigure(string file_name)
        {
            // Figureをセット
            Figure fig;
            viewer.TryGetFigure(out fig);
            t2p.Figure = fig;

            // カテゴリをセット
            List<string> category;
            if (System.IO.Path.GetExtension(file_name) == ".png")
            {
                PNGFileUtils png_u = new PNGFileUtils();
                category = png_u.GetCategoryList(file_name);
            }
            else
            {
                category = new List<string>();
                category.Add("カテゴリーなし");
            }
            t2p.Category = category;

            // コントロールより、体型リストをセットアップ
            t2POptionControl1.SetupTPOListRatio();

            // ファイル名をセット
            t2POptionControl1.SetFileName(file_name);

            // ClippingVertexFormクラスのインスタンスを作成する
            if (clip_form != null && clip_form.Visible == true)
            {
                clip_form.Hide();
                clip_form = new ClippingVertexForm(viewer, t2p, category, this);
                clip_form.Show();
            }
            else
            {
                clip_form = new ClippingVertexForm(viewer, t2p, category, this);
            }
            button_Clipping.Enabled = true;

            button_Trans.Enabled = true;
        }

        private void button_Trans_Click(object sender, EventArgs e)
        {
            // お待ちくださいダイアログを表示
            ProgressDialog pd = new ProgressDialog();
            pd.Show(this);
            pd.Message = "ファイルを変換しています。";

            //try {

                string em;

                // コントロールより、オプションをセットアップ
                if ((em = t2POptionControl1.SetupOption(t2p)) != "")
                {
                    pd.Dispose();
                    MessageBox.Show(em);
                    return;
                }

                // 変換
                if ((em = t2p.Figure2PmdFileData()) != "")
                {
                    pd.Dispose();
                    MessageBox.Show(em);
                    return;
                }

                // 出力フォルダのパスを得る
                string file_path = t2POptionControl1.GetOutputFilePath();

                // PMDファイルを出力
                t2p.Pmd.Save(file_path + "/" + t2POptionControl1.GetModelName() + ".pmd");

                // マテリアル関係のファイルを出力
                t2p.OutputMaterialFile(file_path, t2POptionControl1.GetModelName());

                // 体型レシピを出力
                t2POptionControl1.SaveTPOConfig(Application.StartupPath);
                t2POptionControl1.SaveTPOConfig(file_path);

                // お待ちくださいダイアログを閉じる
                pd.Value = pd.Maximum;
                System.Threading.Thread.Sleep(1000);
                pd.Dispose();

                // にっこりさせる
                t2p.NikkoriFace();

                // メッセージボックスを表示する
                MessageBox.Show("変換を完了しました。");

                // 初期の表情にする
                t2p.DefaultFace();

            /*}
            catch
            {
                // お待ちくださいダイアログを閉じる
                pd.Dispose(); 

                MessageBox.Show("エラーにより、変換できませんでした。");
            }*/
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (viewer != null) viewer.Dispose();

            if (view_form != null)
            {
                view_form.Dispose_flag = true;
                view_form.Dispose();
            }

            if (clip_form != null)
            {
                clip_form.Dispose_flag = true;
                clip_form.Dispose();
            }
        }

    }
}
