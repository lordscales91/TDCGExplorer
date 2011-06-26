using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

using TDCG;
using TDCGUtils;

namespace Tso2Pmd
{
    public partial class TaikeiControl : UserControl
    {
        Viewer viewer;
        Dictionary<string, float> pro_ratio;

        public TaikeiControl()
        {
            InitializeComponent();
        }

        public void Initialize(ref Viewer viewer)
        {
            this.viewer = viewer;

            // 特殊体型メニューを設定
            foreach (IProportion ip in ProportionList.Instance.items)
            {
                if (ip.ToString().IndexOf("TDCG.Proportion.AAA") < 0)
                    listBox1.Items.Add(ip.ToString());
            }

            // 割合を初期状態にする
            InitProRatio();

            // TPOConfig.xmlを読み込む
            LoadTPOConfig();
        }

        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            label_TaikeiValue.Text = (trackBar1.Value * 5.0f).ToString("000") + "%";
            pro_ratio[listBox1.SelectedItem.ToString()]
                = (float)trackBar1.Value * 0.05f;

            SetupTPOListRatio();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            trackBar1.Value = (int)(pro_ratio[listBox1.SelectedItem.ToString()] * 20.0f);
        }

        // 割合を初期状態にする
        public void InitProRatio()
        {
            pro_ratio = new Dictionary<string, float>();

            foreach (IProportion ip in ProportionList.Instance.items)
                pro_ratio.Add(ip.ToString(), 0.0f);

            pro_ratio["TDCG.Proportion.AAA_PMDInitPose"] = 1.0f;
            pro_ratio["TDCG.Proportion.AAA_PMDInitPoseM"] = 1.0f;

            SetupTPOListRatio();
            if (listBox1.SelectedIndex < 0) listBox1.SelectedIndex = 0;
            trackBar1.Value = (int)(pro_ratio[listBox1.SelectedItem.ToString()] * 20.0f);
        }

        // pro_ratioより、fig.TPOListへ、体型レシピを更新
        public void SetupTPOListRatio()
        {
            Figure fig;
            if (viewer.TryGetFigure(out fig))
            {
                foreach (KeyValuePair<string, float> kvp in pro_ratio)
                {
                    if (kvp.Key == "TDCG.Proportion.Shift_Y")
                        fig.TPOList[kvp.Key].Ratio = 0.0f;
                    else
                        fig.TPOList[kvp.Key].Ratio = kvp.Value;
                }

                fig.TransformTpo(fig.GetFrameIndex());
                fig.UpdateBoneMatrices(true);

                // つま先と地面の位置の差分だけシフトして、足の裏が地面に着くようにする
                if (fig.TPOList.Tmo.FindNodeByName("W_LeftToes_End") != null)
                {
                    float toes_y = fig.TPOList.Tmo.FindNodeByName("W_LeftToes_End").combined_matrix.M42;
                    fig.TPOList["TDCG.Proportion.AAA_WHipsYShift"].Ratio += -toes_y + 0.5f;
                }

                fig.TPOList["TDCG.Proportion.Shift_Y"].Ratio
                    = pro_ratio["TDCG.Proportion.Shift_Y"];

                fig.TransformTpo(fig.GetFrameIndex());
                fig.UpdateBoneMatrices(true);
            }
        }

        // TPOConfig.xmlを読み込む
        private void LoadTPOConfig()
        {
            LoadTPOConfig(Path.Combine(Application.StartupPath, @"TPOConfig.xml"));
        }

        // TPOConfig.xmlを読み込む
        private void LoadTPOConfig(string config_file)
        {
            if (File.Exists(config_file))
            {
                TPOConfig config = TPOConfig.Load(config_file);
                foreach (Proportion pro in config.Proportions)
                {
                    if (pro_ratio.ContainsKey(pro.ClassName) == true)
                        pro_ratio[pro.ClassName] = pro.Ratio;
                }

                SetupTPOListRatio();
                trackBar1.Value = (int)(pro_ratio[listBox1.SelectedItem.ToString()] * 20.0f);
            }
        }

        // TPOConfig.xmlを書き出す
        public void SaveTPOConfig(string path)
        {
            TPOConfig config = new TPOConfig();

            Figure fig;
            if (viewer.TryGetFigure(out fig))
            {
                config.Proportions = new Proportion[fig.TPOList.Count];
                for (int i = 0; i < fig.TPOList.Count; i++)
                    config.Proportions[i] = new Proportion();

                for (int i = 0; i < fig.TPOList.Count; i++)
                {
                    TPOFile tpo = fig.TPOList[i];
                    Proportion portion = config.Proportions[i];
                    portion.ClassName = tpo.ProportionName;
                    portion.Ratio = tpo.Ratio;
                }
            }
            config.Save(Path.Combine(path, @"TPOConfig.xml"));
        }

        private void button1_Click(object sender, EventArgs e)
        {
            InitProRatio();
        }

        private void listBox1_DragEnter(object sender, DragEventArgs e)
        {
            //コントロール内にドラッグされたとき実行される
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                //ドラッグされたデータ形式を調べ、ファイルのときはコピーとする
                e.Effect = DragDropEffects.Copy;
            else
                //ファイル以外は受け付けない
                e.Effect = DragDropEffects.None;
        }

        private void listBox1_DragDrop(object sender, DragEventArgs e)
        {
            //ドロップされたデータがファイルか調べる
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] fileName
                    = (string[])e.Data.GetData(DataFormats.FileDrop);

                // 拡張子が適正であるか確認する
                if (System.IO.Path.GetExtension(fileName[0]) == ".xml")
                {
                    try
                    {
                        LoadTPOConfig(fileName[0]);
                    }
                    catch
                    {
                        MessageBox.Show("ファイルが読み込めません。");
                    }
                }
                else
                {
                    MessageBox.Show("拡張子が適正ではありません。");
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            trackBar1.Value++;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            trackBar1.Value--;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            trackBar1.Value = 0;
        }
    }
}
