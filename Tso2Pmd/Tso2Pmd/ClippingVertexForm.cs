using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using TDCG;

namespace Tso2Pmd
{
    public partial class ClippingVertexForm : Form
    {
        Figure fig;
        int group_flag = 0;
        List<int> verCount = new List<int>();
        List<string> opt;
        List<bool> meshes_flag = new List<bool>();

        internal MainForm ownerForm;

        public ClippingVertexForm(Viewer viewer, TransTso2Pmd t2p, List<string> opt, MainForm ownerForm)
        {
            InitializeComponent();

            this.ownerForm = ownerForm;

            viewer.TryGetFigure(out this.fig);
            this.opt = opt;

            SettingItems();
            AllChecked(true);

            viewer.visible_meshes_flag = meshes_flag;
            t2p.UseMeshes = meshes_flag;
        }

        // -----------------------------------------------------
        // リストボックスのチェックが変更される前に実行されるイベント
        // -----------------------------------------------------
        private void checkedListBox1_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            // ビューアーに表示されるメッシュを指定するフラグの更新
            UpdataMeshesFlag(e);

            // 合計の頂点数を再計算
            int clip_verCount = 0;
            for (int i = 0; i < checkedListBox1.CheckedIndices.Count; i++)
                clip_verCount += verCount[checkedListBox1.CheckedIndices[i]];

            // チェックが変更された後の状態を反映する
            if (e.NewValue == CheckState.Checked)
                clip_verCount += verCount[e.Index];
            else
                clip_verCount -= verCount[e.Index];

            // 合計頂点数を表示
            label_VerCount.Text = clip_verCount.ToString();

            // 頂点数が上限を超えていたら文字を赤色にする
            if (clip_verCount < 65535)
                label_VerCount.ForeColor = Color.Black;
            else
                label_VerCount.ForeColor = Color.Red;
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked == true)
            {
                group_flag = 0;
                label1.Text = "カテゴリ - 材質名 : 頂点数";
                SettingItems();
                AllChecked(true);
            }
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2.Checked == true)
            {
                group_flag = 1;
                label1.Text = "カテゴリ - メッシュ名 : 頂点数";
                SettingItems();
                AllChecked(true);
            }
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton3.Checked == true)
            {
                group_flag = 2;
                label1.Text = "カテゴリ - メッシュ名 -サブメッシュ番号 (材質名) : 頂点数";
                SettingItems();
                AllChecked(true);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            AllChecked(true);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            AllChecked(false);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < checkedListBox1.Items.Count; i++)
                checkedListBox1.SetItemChecked(i, 
                    !(checkedListBox1.GetItemChecked(i)));
        }
 
        private void AllChecked(bool state)
        {
            for (int i = 0; i < checkedListBox1.Items.Count; i++)
                checkedListBox1.SetItemChecked(i, state);
        }

        private void ClippingVertexForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!IsDisposed)
            {
                ownerForm.ClippingVertexForm_Hiding();
                this.Hide();
                e.Cancel = true;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            ownerForm.ClippingVertexForm_Hiding();
            this.Hide();
        }

        private void SettingItems()
        {
            // 前回カウントした情報をクリア
            checkedListBox1.Items.Clear();
            verCount.Clear();

            // 頂点数をチェック
            int n_tso = 0;
            foreach (TSOFile tso in fig.TSOList)
            {
                switch (group_flag)
                {
                    case 0:
                        for (int script_num = 0; script_num < tso.sub_scripts.Length; script_num++)
                        {
                            int verCount_inScript = 0;

                            foreach (TSOMesh mesh in tso.meshes)
                            foreach (TSOSubMesh sub_mesh in mesh.sub_meshes)
                            {
                                if (sub_mesh.spec == script_num)
                                    verCount_inScript += sub_mesh.vertices.Length;
                            }

                            checkedListBox1.Items.Add(
                                opt[n_tso] + " - " + tso.sub_scripts[script_num].Name
                                + " : " + verCount_inScript.ToString());
                            verCount.Add(verCount_inScript);
                        }
                        break;

                    case 1:
                        foreach (TSOMesh mesh in tso.meshes)
                        {
                            int verCount_inFrame = 0;

                            foreach (TSOSubMesh sub_mesh in mesh.sub_meshes)
                            {
                                verCount_inFrame += sub_mesh.vertices.Length;
                            }

                            checkedListBox1.Items.Add(
                                opt[n_tso] + " - " + mesh.Name
                                + " : " + verCount_inFrame.ToString());
                            verCount.Add(verCount_inFrame);
                        }
                        break;

                    case 2:
                        int verCount_inTso = 0;

                        foreach (TSOMesh mesh in tso.meshes)
                        {
                            int mesh_num = 0;

                            foreach (TSOSubMesh sub_mesh in mesh.sub_meshes)
                            {
                                verCount_inTso += sub_mesh.vertices.Length;

                                checkedListBox1.Items.Add(
                                    opt[n_tso] + " - " + mesh.Name + " -" + mesh_num.ToString()
                                    + " (" + tso.sub_scripts[sub_mesh.spec].Name + ") : "
                                    + sub_mesh.vertices.Length);
                                verCount.Add(sub_mesh.vertices.Length);

                                mesh_num++;
                            }
                        }
                        break;
                }

                n_tso++;
            }
        }

        // ビューアーに表示されるメッシュを指定するフラグの更新
        private void UpdataMeshesFlag(ItemCheckEventArgs e)
        {
            meshes_flag.Clear();

            int checkList_index = 0;
            int tso_num = 0;
            foreach (TSOFile tso in fig.TSOList)
            {
                switch (group_flag)
                {
                    case 0:
                        for (int script_num = 0; script_num < tso.sub_scripts.Length; script_num++)
                        {
                            bool flag;
                            if (checkList_index == e.Index)
                                flag = !(checkedListBox1.CheckedIndices.IndexOf(checkList_index++) >= 0);// チェックが変更された後の状態を反映する
                            else
                                flag = (checkedListBox1.CheckedIndices.IndexOf(checkList_index++) >= 0);

                            foreach (TSOMesh mesh in tso.meshes)
                            foreach (TSOSubMesh sub_mesh in mesh.sub_meshes)
                            {
                                if (sub_mesh.spec == script_num)
                                    meshes_flag.Add(flag);
                            }
                        }
                        break;

                    case 1:
                        for (int script_num = 0; script_num < tso.sub_scripts.Length; script_num++)
                        {
                            int tmp_checkList_index = checkList_index;
                            foreach (TSOMesh mesh in tso.meshes)
                            {
                                bool flag;
                                if (checkList_index == e.Index)
                                    flag = !(checkedListBox1.CheckedIndices.IndexOf(checkList_index++) >= 0);// チェックが変更された後の状態を反映する
                                else
                                    flag = (checkedListBox1.CheckedIndices.IndexOf(checkList_index++) >= 0);

                                foreach (TSOSubMesh sub_mesh in mesh.sub_meshes)
                                {
                                    if (sub_mesh.spec == script_num)
                                        meshes_flag.Add(flag);
                                }
                            }
                            if (script_num < tso.sub_scripts.Length - 1)
                                checkList_index = tmp_checkList_index;
                        }
                        break;

                    case 2:
                        for (int script_num = 0; script_num < tso.sub_scripts.Length; script_num++)
                        {
                            int tmp_checkList_index = checkList_index;
                            foreach (TSOMesh mesh in tso.meshes)
                            foreach (TSOSubMesh sub_mesh in mesh.sub_meshes)
                            {
                                bool flag;
                                if (checkList_index == e.Index)
                                    flag = !(checkedListBox1.CheckedIndices.IndexOf(checkList_index++) >= 0);// チェックが変更された後の状態を反映する
                                else
                                    flag = (checkedListBox1.CheckedIndices.IndexOf(checkList_index++) >= 0);

                                if (sub_mesh.spec == script_num)
                                    meshes_flag.Add(flag);
                            }
                            if (script_num < tso.sub_scripts.Length - 1)
                                checkList_index = tmp_checkList_index;
                        }
                        break;
                }
                tso_num++;
            }
        }

   }
}
