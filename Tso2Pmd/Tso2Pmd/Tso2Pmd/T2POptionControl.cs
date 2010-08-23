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
    public partial class T2POptionControl : UserControl
    {
        string file_name;

        public T2POptionControl()
        {
            InitializeComponent();
        }

        public void Initialize(ref Viewer viewer)
        {
            // 出力フォルダ設定
            radioButton1.Checked = true;
            textBox_Folder.Enabled = false;
            button_Folder.Enabled = false;

            taikeiControl1.Initialize(ref viewer);
        }

        private void button_Folder_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog fbd = new FolderBrowserDialog())
            {
                //最初に選択するフォルダを指定する
                if (textBox_Folder.Text != "")
                    fbd.SelectedPath = textBox_Folder.Text;

                if (fbd.ShowDialog() != DialogResult.OK)
                {
                    return;
                }
                textBox_Folder.Text = fbd.SelectedPath;
            }
        }

        // フォームより各パラメータを得て、設定
        public string SetupOption(TransTso2Pmd t2p)
        {
            if (radioButton_Kami0.Checked == true) t2p.Kami_flag = 0;
            else if (radioButton_Kami1.Checked == true) t2p.Kami_flag = 1;
            else if (radioButton_Kami2.Checked == true) t2p.Kami_flag = 2;

            if (radioButton_Chichi0.Checked == true) t2p.Chichi_flag = 0;
            else if (radioButton_Chichi1.Checked == true) t2p.Chichi_flag = 1;
            else if (radioButton_Chichi2.Checked == true) t2p.Chichi_flag = 2;
            else if (radioButton_Chichi3.Checked == true) t2p.Chichi_flag = 3;

            if (radioButton_Skirt0.Checked == true) t2p.Skirt_flag = 0;
            else if (radioButton_Skirt1.Checked == true) t2p.Skirt_flag = 1;
            else if (radioButton_Skirt2.Checked == true) t2p.Skirt_flag = 2;

            if (radioButton_Bone0.Checked == true) t2p.Bone_flag = 0;
            else if (radioButton_Bone1.Checked == true) t2p.Bone_flag = 1;

            t2p.Spheremap_flag = checkBox_Spheremap.Checked;
            t2p.Edge_flag_flag = checkBox_Edge.Checked;
            t2p.Merge_flag = checkBox_Merge.Checked;

            string em;

            // ヘッダ情報を入力
            if ((em = t2p.InputHeader(textBox_ModelName.Text, textBox_Comment.Text)) != "")
                return em;

            return "";
        }

        public void SetFileName(string file_name)
        {
            this.file_name = file_name;
        }

        // 出力フォルダを得る
        public string GetOutputFilePath()
        {
            if (radioButton1.Checked == true)
            {
                string outputFilePath = System.IO.Path.GetDirectoryName(file_name) + "\\"
                    + System.IO.Path.GetFileNameWithoutExtension(
                        System.IO.Path.GetFileNameWithoutExtension(file_name));
                System.IO.Directory.CreateDirectory(outputFilePath);
                return outputFilePath;
            }
            else if (radioButton2.Checked == true)
            {
                return System.IO.Path.GetDirectoryName(file_name);
            }
            else
            {
                return textBox_Folder.Text;
            }
        }

        public string GetModelName()
        {
            return textBox_ModelName.Text;
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            textBox_Folder.Enabled = false;
            button_Folder.Enabled = false;
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            textBox_Folder.Enabled = false;
            button_Folder.Enabled = false;
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            textBox_Folder.Enabled = true;
            button_Folder.Enabled = true;
        }

        public void SetupTPOListRatio()
        {
            taikeiControl1.SetupTPOListRatio();
        }

        public void SaveTPOConfig(string path)
        {
            taikeiControl1.SaveTPOConfig(path);
        }

    }
}
