using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;

namespace Tso2MqoGui
{
    public partial class Form1 : Form
    {
        public string   OutPath
        {
            get { return tbPath.Text; }
            set { tbPath.Text= value; }
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            RegistryKey reg = Application.UserAppDataRegistry.CreateSubKey("Config");
            OutPath = (string)reg.GetValue("OutPath", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
            tabControl1.SelectedIndex = (int)reg.GetValue("TabPage", 0);
            tbMqoFile.Text = (string)reg.GetValue("MqoIn", "");
            tbTsoFileRef.Text = (string)reg.GetValue("Tso", "");
            tbTsoFile.Text = (string)reg.GetValue("TsoEx", "");
            tbMergeTso.Text = (string)reg.GetValue("MergeTso", "");
            rbRefBone.Checked = (int)reg.GetValue("RefBone", 1) == 1;
            rbOneBone.Checked = (int)reg.GetValue("OneBone", 0) == 1;
            rbBoneNone.Checked = (int)reg.GetValue("BoneNone", 1) == 1;
            rbBoneRokDeBone.Checked = (int)reg.GetValue("BoneRokDeBone", 0) == 1;
            cbMakeSub.Checked = (int)reg.GetValue("MakeSub", 1) == 1;
            cbCopyTSO.Checked = (int)reg.GetValue("CopyTSO", 1) == 1;
            cbShowMaterials.Checked = (int)reg.GetValue("ShowMaterials", 0) == 1;

            reg             = Application.UserAppDataRegistry.CreateSubKey("Form1");
            Bounds          = new Rectangle(
                (int)reg.GetValue("Left",     0),
                (int)reg.GetValue("Top",      0),
                (int)reg.GetValue("Width",  640),
                (int)reg.GetValue("Height", 320));

            EnableControlStuff();

            Config  config  = Config.Instance;
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            RegistryKey reg = Application.UserAppDataRegistry.CreateSubKey("Config");
            reg.SetValue("OutPath", OutPath);
            reg.SetValue("TabPage", tabControl1.SelectedIndex);
            reg.SetValue("MqoIn", tbMqoFile.Text);
            reg.SetValue("Tso", tbTsoFileRef.Text);
            reg.SetValue("TsoEx", tbTsoFile.Text);
            reg.SetValue("MergeTso", tbMergeTso.Text);
            reg.SetValue("RefBone", rbRefBone.Checked ? 1 : 0);
            reg.SetValue("OneBone", rbOneBone.Checked ? 1 : 0);
            reg.SetValue("BoneNone", rbBoneNone.Checked ? 1 : 0);
            reg.SetValue("BoneRokDeBone", rbBoneRokDeBone.Checked ? 1 : 0);
            reg.SetValue("MakeSub", cbMakeSub.Checked ? 1 : 0);
            reg.SetValue("CopyTSO", cbCopyTSO.Checked ? 1 : 0);
            reg.SetValue("ShowMaterials", cbShowMaterials.Checked ? 1 : 0);

            reg= Application.UserAppDataRegistry.CreateSubKey("Form1");

            if((this.WindowState & FormWindowState.Minimized) == FormWindowState.Minimized)
            {
                reg.SetValue("Top",    RestoreBounds.Top);
                reg.SetValue("Left",   RestoreBounds.Left);
                reg.SetValue("Width",  RestoreBounds.Width);
                reg.SetValue("Height", RestoreBounds.Height);
            } else
            {
                reg.SetValue("Top",    Top);
                reg.SetValue("Left",   Left);
                reg.SetValue("Width",  Width);
                reg.SetValue("Height", Height);
            }

            Config.Save();
        }

        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                if(!e.Data.GetDataPresent(DataFormats.FileDrop))
                    return;

                string[]    files   = (string[])e.Data.GetData(DataFormats.FileDrop);

                if(files.Length == 0)
                    return;

                switch(tabControl1.SelectedIndex)
                {
                case 0:
                    foreach(string i in files)
                    {
                        if(Path.GetExtension(i).ToUpper() == ".TSO")
                            OpenTSOFile(i);
                    }

                    break;

                case 1:
                    switch (Path.GetExtension(files[0]).ToUpper())
                    {
                        case ".TSO": tbTsoFileRef.Text = files[0]; break;
                        case ".MQO": tbMqoFile.Text = files[0]; break;
                    }

                    break;

                case 2:
                    AddMergeTso(files);
                    break;
                }
            } catch(Exception ex)
            {
                Util.ProcessError(ex);
            }
        }

        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            if(!e.Data.GetDataPresent(DataFormats.FileDrop))
                return;

            e.Effect    = DragDropEffects.Copy;
        }

        private void tbMergeTso_DragDrop(object sender, DragEventArgs e)
        {
            if(!e.Data.GetDataPresent(DataFormats.FileDrop))
                return;

            string[]    files   = (string[])e.Data.GetData(DataFormats.FileDrop);

            switch(Path.GetExtension(files[0]).ToUpper())
            {
            case ".TSO":    tbMergeTso.Text= files[0];  break;
            }
        }

        private void tbMergeTso_DragEnter(object sender, DragEventArgs e)
        {
            if(!e.Data.GetDataPresent(DataFormats.FileDrop))
                return;

            e.Effect    = DragDropEffects.Copy;
        }

        private void OpenTSOFile(string file)
        {
            string dir = OutPath;

            if (cbMakeSub.Checked)
            {
                dir = Path.Combine(dir, Path.GetFileNameWithoutExtension(file));

                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);
            }

            string mqo_path = Path.Combine(dir, Path.ChangeExtension(Path.GetFileName(file), ".mqo"));
            string importinfo_path = Path.Combine(dir, Path.ChangeExtension(Path.GetFileName(file), ".xml"));

            try
            {
                label2.BackColor = Color.Tomato;
                label2.ForeColor = Color.White;
                label2.Text = "Processing";
                label2.Invalidate();
                label2.Update();

                // モデル、テクスチャの作成
                using (MqoWriter mqo = new MqoWriter(mqo_path))
                {
                    TSOFile tso = new TSOFile(file);
                    tso.ReadAll();

                    if (rbBoneRokDeBone.Checked) mqo.BoneMode = MqoBoneMode.RokDeBone;

                    mqo.Write(tso);
                    mqo.Close();

                    ImportInfo ii = new ImportInfo();

                    // テクスチャ情報
                    foreach (TSOTex i in tso.textures)
                        ii.textures.Add(new ImportTextureInfo(i));

                    // エフェクトの作成
                    foreach (TSOEffect i in tso.effects)
                    {
                        ii.effects.Add(new ImportEffectInfo(i));
                        File.WriteAllText(Path.Combine(dir, i.Name), i.code, Encoding.Default);
                    }

                    // マテリアルの作成
                    foreach (TSOMaterial i in tso.materials)
                    {
                        ii.materials.Add(new ImportMaterialInfo(i));
                        File.WriteAllText(Path.Combine(dir, i.Name), i.code, Encoding.Default);
                    }

                    ImportInfo.Save(importinfo_path, ii);
                }

                if (cbCopyTSO.Checked)
                {
                    string tso_path = Path.Combine(dir, Path.GetFileName(file));

                    if (file != tso_path)
                        File.Copy(file, tso_path, true);
                }
            }
            finally
            {
                label2.BackColor = SystemColors.Control;
                label2.BackColor = label2.Parent.BackColor;
                label2.ForeColor = SystemColors.ControlText;
                label2.Text = "Drop TSO File Here!";
            }
        }

        private void OpenMQOFile(string file)
        {
            TSOGeneratorConfig config = new TSOGeneratorConfig();
            config.ShowMaterials = cbShowMaterials.Checked;

            if (rbRefBone.Checked)
            {
                TSOGeneratorRefBone gen = new TSOGeneratorRefBone(config);
                gen.Generate(file, tbTsoFileRef.Text, tbTsoFile.Text);
            }
            else
            if (rbOneBone.Checked)
            {
                TSOGeneratorOneBone gen = new TSOGeneratorOneBone(config);

                foreach (ListViewItem i in lvObjects.Items)
                {
                    if (i.SubItems[1].Text == "")
                    {
                        MessageBox.Show("すべてのオブジェクトにボーンを設定してください");
                        return;
                    }

                    gen.ObjectBoneNames.Add(i.SubItems[0].Text, i.SubItems[1].Text);
                }

                gen.Generate(file, tbTsoFileRef.Text, tbTsoFile.Text);
            }
            else
            {
            }
        }
#region tso->mqo UI
        private void button1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dlg = new FolderBrowserDialog();
            dlg.SelectedPath    = OutPath;

            if(dlg.ShowDialog() == DialogResult.OK)
                OutPath = dlg.SelectedPath;
        }
#endregion
#region mqo->tso UI
        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            EnableControlStuff();
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            EnableControlStuff();
        }

        private void EnableControlStuff()
        {
            gbBone.Enabled  = rbOneBone.Checked;
        }

        private void BuildBoneTree(TreeNodeCollection nodes, TSONode node)
        {
            TreeNode    tn  = nodes.Add(node.ShortName);
            tn.Tag          = node;

            if(node.children != null)
            foreach(TSONode i in node.children)
                BuildBoneTree(tn.Nodes, i);
        }

        private void SaveAssign()
        {
            foreach(ListViewItem i in lvObjects.Items)
            {
                string  obj = i.SubItems[0].Text;
                string  bone= i.SubItems[1].Text;

                if(Config.Instance.object_bone_map.ContainsKey(obj))
                        Config.Instance.object_bone_map[obj]    = bone;
                else    Config.Instance.object_bone_map.Add(obj, bone);
            }
        }

        private void btnMqoFile_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog dlg = new OpenFileDialog();
                dlg.Filter = "Metasequoia File (*.mqo)|*.mqo";
                dlg.FileName = tbMqoFile.Text;

                if (dlg.ShowDialog() == DialogResult.OK)
                    tbMqoFile.Text = dlg.FileName;
            }
            catch (Exception ex)
            {
                Util.ProcessError(ex);
            }
        }

        private void btnTsoFileRef_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog dlg = new OpenFileDialog();
                dlg.Filter = "TSO File (*.tso)|*.tso";
                dlg.FileName = tbTsoFileRef.Text;

                if (dlg.ShowDialog() == DialogResult.OK)
                    tbTsoFileRef.Text = dlg.FileName;
            }
            catch (Exception ex)
            {
                Util.ProcessError(ex);
            }
        }

        private void btnTsoFile_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog dlg = new SaveFileDialog();
                dlg.Filter = "TSO File (*.tso)|*.tso";
                dlg.FileName = tbTsoFile.Text;

                if (dlg.ShowDialog() == DialogResult.OK)
                    tbTsoFile.Text = dlg.FileName;
            }
            catch (Exception ex)
            {
                Util.ProcessError(ex);
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                // 一旦現状を保存
                SaveAssign();

                // オブジェクト
                MqoFile mqo = new MqoFile();
                mqo.Load(tbMqoFile.Text);
                lvObjects.Items.Clear();

                foreach (MqoObject i in mqo.Objects)
                {
                    ListViewItem item = lvObjects.Items.Add(i.name);
                    item.Tag = i;
                    string bone;

                    if (Config.Instance.object_bone_map.TryGetValue(i.name, out bone))
                        item.SubItems.Add(bone);
                    else item.SubItems.Add("");
                }

                // ボーン構造
                TSOFile tso = new TSOFile(tbTsoFileRef.Text);
                tso.ReadAll();
                tvBones.Visible = false;
                tvBones.Nodes.Clear();
                BuildBoneTree(tvBones.Nodes, tso.nodes[0]);
                tvBones.ExpandAll();
                tvBones.Nodes[0].EnsureVisible();
            }
            catch (Exception ex)
            {
                Util.ProcessError(ex);
            }
            finally
            {
                tvBones.Visible = true;
            }
        }

        private void btnSelectAll_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem i in lvObjects.Items)
                i.Selected = true;
        }

        private void btnDeselectAll_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem i in lvObjects.Items)
                i.Selected = false;
        }

        private void btnAssign_Click(object sender, EventArgs e)
        {
            try
            {
                TreeNode n = tvBones.SelectedNode;

                if (n == null)
                {
                    MessageBox.Show("割り当てるボーンを選択してください");
                    return;
                }

                foreach (ListViewItem i in lvObjects.SelectedItems)
                    i.SubItems[1].Text = n.Text;

                SaveAssign();
            }
            catch (Exception ex)
            {
                Util.ProcessError(ex);
            }
        }

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            Color c = tabPage2.BackColor;

            try
            {
                tabPage2.BackColor = Color.Tomato;
                tabPage2.Update();
                string file = tbMqoFile.Text;
                OpenMQOFile(file);
            }
            catch (Exception ex)
            {
                Util.ProcessError(ex);
            }
            finally
            {
                tabPage2.BackColor = c;
            }
        }
#endregion
#region Merge UI
        private void AddMergeTso(string[] files)
        {
            foreach(string i in files)
            {
                if(Path.GetExtension(files[0]).ToUpper() != ".TSO")
                    continue;

                if(tvMerge.Nodes.Find(i, false).Length == 0)
                {
                    TreeNode    node= tvMerge.Nodes.Add(i);
                    node.Name       = i;
                    node.Checked    = true;

                    TSOFile     tso = new TSOFile(i);
                    tso.ReadAll();

                    foreach(TSOMesh j in tso.meshes)
                    {
                        TreeNode    mesh= node.Nodes.Add(j.Name);
                        mesh.Name       = j.Name;
                        mesh.Checked    = true;
                    }
                }
            }
        }

        private void btnMerge_Click(object sender, EventArgs e)
        {
            Color c = tabPage2.BackColor;

            try
            {
                tabPage2.BackColor = Color.Tomato;
                List<TSOMesh> meshes = new List<TSOMesh>();
                Dictionary<string, Pair<TSOMaterial, int>> materialmap = new Dictionary<string, Pair<TSOMaterial, int>>();
                Dictionary<string, TSOTex> textures = new Dictionary<string, TSOTex>();
                TSOFile last = null;

                foreach (TreeNode i in tvMerge.Nodes)
                {
                    TSOFile tso = new TSOFile(i.Text);
                    last = tso;
                    ulong mtls = 0;
                    ulong mask = 1;
                    tso.ReadAll();

                    foreach (TSOMesh j in tso.meshes)
                    {
                        TreeNode[] found = i.Nodes.Find(j.Name, false);

                        if (found.Length == 0 || !found[0].Checked)
                            continue;

                        foreach (TSOSubMesh k in j.sub)
                            mtls |= 1ul << k.spec;

                        meshes.Add(j);
                    }

                    foreach (TSOMaterial j in tso.materials)
                    {
                        if ((mask & mtls) != 0)
                        {
                            if (!materialmap.ContainsKey(j.Name))
                            {
                                Pair<TSOMaterial, int> value = new Pair<TSOMaterial, int>(j, materialmap.Count);
                                materialmap.Add(j.Name, value);

                                if (!textures.ContainsKey(j.ColorTex))
                                {
                                    TSOTex tex = tso.texturemap[j.ColorTex];
                                    textures.Add(tex.Name, tex);
                                }

                                if (!textures.ContainsKey(j.ShadeTex))
                                {
                                    TSOTex tex = tso.texturemap[j.ShadeTex];
                                    textures.Add(tex.Name, tex);
                                }
                            }
                        }

                        mask <<= 1;
                    }
                }

                using (FileStream fs = File.OpenWrite(tbMergeTso.Text))
                {
                    fs.SetLength(0);

                    List<TSOTex> texlist = new List<TSOTex>(textures.Values);
                    TSOMaterial[] mtllist = new TSOMaterial[materialmap.Count];

                    foreach (var i in materialmap.Values)
                        mtllist[i.Second] = i.First;

                    foreach (TSOMesh i in meshes)
                    {
                        foreach (TSOSubMesh j in i.sub)
                        {
                            TSOMaterial mtl = i.file.materials[j.spec];
                            j.spec = materialmap[mtl.Name].Second;
                        }
                    }

                    foreach (TSOTex i in texlist)
                        TSOFile.ExchangeChannel(i.data, i.depth);

                    BinaryWriter bw = new BinaryWriter(fs);
                    TSOWriter.WriteHeader(bw);
                    TSOWriter.Write(bw, last.nodes);
                    TSOWriter.Write(bw, texlist.ToArray());
                    TSOWriter.Write(bw, last.effects);
                    TSOWriter.Write(bw, mtllist);
                    TSOWriter.Write(bw, meshes.ToArray());
                }
            }
            catch (Exception ex)
            {
                Util.ProcessError(ex);
            }
            finally
            {
                tabPage2.BackColor = c;
            }
        }

        private void btnMergeAdd_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog  dlg = new OpenFileDialog();
                dlg.Filter      = "TSO File(*.tso)|*.tso";
                dlg.Multiselect = true;

                if(dlg.ShowDialog() == DialogResult.OK)
                    AddMergeTso(dlg.FileNames);
            } catch(Exception ex)
            {
                Util.ProcessError(ex);
            }
        }

        private void btnMergeDel_Click(object sender, EventArgs e)
        {
            if (tvMerge.SelectedNode != null && tvMerge.SelectedNode.Level == 0)
                tvMerge.SelectedNode.Remove();
        }

        private void btnMergeReset_Click(object sender, EventArgs e)
        {
            tvMerge.Nodes.Clear();
        }

        private void btnRefMergeTso_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog dlg = new SaveFileDialog();
                dlg.Filter = "TSO File (*.tso)|*.tso";
                dlg.FileName = tbMergeTso.Text;

                if (dlg.ShowDialog() == DialogResult.OK)
                    tbMergeTso.Text = dlg.FileName;
            }
            catch (Exception ex)
            {
                Util.ProcessError(ex);
            }
        }

        public static bool  bTvMerge_AfterCheck = false;

        private void tvMerge_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if(bTvMerge_AfterCheck)
                return;

            bTvMerge_AfterCheck = true;

            try
            {
                if(e.Node.Level == 0)
                {
                    foreach(TreeNode i in e.Node.Nodes)
                        i.Checked   = e.Node.Checked;
                } else
                {
                    bool    check   = false;
                  //bool    uncheck = false;

                    foreach(TreeNode i in e.Node.Parent.Nodes)
                    if(i.Checked)   check   = true;
                  //else            uncheck = true;

                    e.Node.Parent.Checked   = check;
                }
            } finally
            {
                bTvMerge_AfterCheck = false;
            }
        }
#endregion
    }

    public class Util
    {
        public static void ProcessError(Exception e)
        {
            MessageBox.Show(e.ToString());
        }
    }
}
