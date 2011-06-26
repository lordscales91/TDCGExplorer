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

        TemplateList template_list;
        CorrespondTableList correspondTable_list = new CorrespondTableList();

        public T2POptionControl()
        {
            InitializeComponent();
        }

        public void Initialize(ref Viewer viewer, TemplateList template_list)
        {
            // 出力フォルダ設定
            radioButton1.Checked = true;
            textBox_Folder.Enabled = false;
            button_Folder.Enabled = false;

            // -----------------------------------------------------
            this.template_list = template_list;
            taikeiControl1.Initialize(ref viewer);
            physicsControl1.Initialize(template_list);

            // -----------------------------------------------------
            // ボーンの変換表を読みとる
            correspondTable_list.Load();
            foreach (string name in correspondTable_list.NameList)
                checkedListBox1.Items.Add(name);
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
            t2p.Spheremap_flag = checkBox_Spheremap.Checked;
            t2p.Edge_flag_flag = checkBox_Edge.Checked;
            t2p.Merge_flag = checkBox_Merge.Checked;

            physicsControl1.SetPhysFlag();
            t2p.TemplateList = template_list;
            t2p.CorTableList = correspondTable_list;

            t2p.Bone_flag = radioButton_Bone1.Checked ? 1 : 0; 

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

        private void radioButton_Bone1_CheckedChanged(object sender, EventArgs e)
        {
            checkedListBox1.Enabled = false;
        }

        private void radioButton_Bone0_CheckedChanged(object sender, EventArgs e)
        {
            checkedListBox1.Enabled = true;
        }

        private void checkedListBox1_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (e.NewValue == CheckState.Checked)
                correspondTable_list.Flag[checkedListBox1.Items[e.Index].ToString()] = true;
            else if (e.NewValue == CheckState.Unchecked)
                correspondTable_list.Flag[checkedListBox1.Items[e.Index].ToString()] = false;
        }
    }
}
