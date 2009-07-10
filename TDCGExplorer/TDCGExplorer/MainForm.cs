using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using TDCG;

namespace TDCGExplorer
{
    public partial class MainForm : Form
    {
        private Viewer viewer = null;
        private bool fInitialTmoLoad = false;
        private TreeNode lastSelectTreeNode = null;

        private Color lastSelectTreeNodeColor = Color.Transparent;
        private TreeNode lastSelectTreeNodeForColor = null;

        public MainForm()
        {
            InitializeComponent();
        }

        public TabControl TabControlMainView
        {
            get { return tabMainView; }
            set { }
        }

        public ListBox ListBoxMainView
        {
            get { return listBoxMainListBox; }
            set { }
        }

        public Viewer Viewer
        {
            get { return viewer; }
            set { }
        }

        // スレッド実行時はエラーにする.
        private bool threadCheck()
        {
            if (TDCGExplorer.fThreadRun == true)
            {
                MessageBox.Show("Database Processing Now!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return true;
            }
            return false;
        }

        // 終了チェック.
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (threadCheck() == true) return;
            Close();
        }

        // dbを構築する.
        private void createFromArcsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (threadCheck() == true) return;
            TDCGExplorer.CreateNewArcsDatabase();
        }

        // 起動時処理.
        private void MainForm_Load(object sender, EventArgs e)
        {
            TDCGExplorer.IfReadyDbDisplayArcsDB();

            string windowRect = TDCGExplorer.SystemDB.window_rectangle;
            string[] rect = windowRect.Split(',');
            Left = int.Parse(rect[0]);
            Top = int.Parse(rect[1]);
            Size = new Size( int.Parse(rect[2]) , int.Parse(rect[3]));

            string splitterDistance = TDCGExplorer.SystemDB.splitter_distance;
            string[] distance = splitterDistance.Split(',');
            splitContainerV.SplitterDistance = int.Parse(distance[0]);
            splitContainerH.SplitterDistance = int.Parse(distance[1]);
            splitContainerWithView.SplitterDistance = int.Parse(distance[2]);

            viewer = null;

            listBoxMainListBox.HorizontalScrollbar = true;
        }

        // invokeの為のdelegate
        private delegate void displayFromArcsHander();

        // 非同期で呼び出されるメソッド
        private void asyncDlgDisplayFromArcs()
        {
            //TDCGExplorer.DisplayArcsDB(treeViewArcs);
            DisplayDB();
        }

        // 非同期でツリー表示を更新する.
        public void asyncDisplayFromArcs()
        {
            Invoke(new displayFromArcsHander(asyncDlgDisplayFromArcs));
        }

        // タイマー
        private void MainTimer_Tick(object sender, EventArgs e)
        {
            toolStripStatusLabel.Text = TDCGExplorer.GetToolTips();
            // TSOViewの表示を更新する.
            try
            {
                if (viewer != null)
                {
                    viewer.FrameMove();
                    viewer.Render();
                }
            }
            catch(Exception ex)
            {
                viewer.Dispose();
                viewer = null;
                TDCGExplorer.SetToolTips("error:" + ex.Message);
                Debug.WriteLine(ex.Message);
            }
        }

        // ウインドウが閉じる時の後始末.
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (threadCheck() == true){
                e.Cancel = true;
            }
            string widnowRect = Left.ToString() + "," + Top.ToString() + "," + Size.Width.ToString() + ","+Size.Height.ToString();
            TDCGExplorer.SystemDB.window_rectangle = widnowRect;

            string splitterDistance = splitContainerV.SplitterDistance.ToString()+","+
                splitContainerH.SplitterDistance.ToString()+","+
                splitContainerWithView.SplitterDistance.ToString();
            TDCGExplorer.SystemDB.splitter_distance = splitterDistance;

            if (viewer != null)
            {
                viewer.Dispose();
                viewer = null;
            }
        }
 
        // ツリーを展開する.
        private void expandAllToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            try
            {
                TreeNode node = (TreeNode)lastSelectTreeNode;
                node.ExpandAll();
            }
            catch (Exception exception)
            {
                TDCGExplorer.SetToolTips("error:" + exception.Message);
                Debug.WriteLine(exception.Message);
            }
        }

        // タブを閉じる.
        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (tabMainView.SelectedTab != null) tabMainView.SelectedTab.Dispose();
        }

        // ZIPファイルを展開する
        private void extractZipToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            try
            {
                ZipTreeNode node = (ZipTreeNode)lastSelectTreeNode;
                if (TDCGExplorer.InstallZipFile(node))
                {
                    //MessageBox.Show("Extracted on work directory", "Extract", MessageBoxButtons.OK);
                }
                else
                {
                    MessageBox.Show("Error occured.", "Extract", MessageBoxButtons.OK);
                }
            }
            catch (System.InvalidCastException ex)
            {
                MessageBox.Show("この操作は圧縮ファイルにのみ実行できます", "エラー", MessageBoxButtons.OK);
                Debug.WriteLine(ex.Message);
            }
            catch (Exception exception)
            {
                TDCGExplorer.SetToolTips("error:" + exception.Message);
                Debug.WriteLine(exception.Message);
            }
            Cursor.Current = Cursors.Default;
        }

        // TSOビューワをリセットする
        private void resetTSOViewerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (viewer!=null)
            {
                viewer.ClearFigureList();
                viewer.Render();
                viewer.Dispose();
                viewer = null;
            }
        }

        // 初期TMOを読み込む
        public void doInitialTmoLoad()
        {
            if (viewer != null)
            {
                if (fInitialTmoLoad == false)
                {
                    viewer.LoadTMOFile("default.tmo");
                    fInitialTmoLoad = true;
                }
            }
        }

        // モーション開始
        private void switchToMortionEnabledToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (viewer != null)
            {
                viewer.SwitchMotionEnabled();
            }
        }

        // TSOビューを生成
        public void makeTSOViwer()
        {
            if (viewer == null)
            {
                viewer = new Viewer();
                viewer.InitializeApplication(splitContainerWithView.Panel2);
                fInitialTmoLoad = false;
            }
        }

        public void clearTSOViewer()
        {
            if (viewer != null)
            {
                viewer.ClearFigureList();
                fInitialTmoLoad = false;
            }
        }

        // 初期設定を行う.
        private void editSystemDatabaseToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            if (threadCheck() == true) return;
            TDCGExplorer.EditSystemDatabase();
        }

        // アノテーションを編集.
        private void EditAnnotationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                ZipTreeNode node = (ZipTreeNode)lastSelectTreeNode;
                node.DoEditAnnotation();
            }
            catch (System.InvalidCastException ex)
            {
                MessageBox.Show("この操作は圧縮ファイルにのみ実行できます", "エラー", MessageBoxButtons.OK);
                Debug.WriteLine(ex.Message);
            }
            catch (Exception exception)
            {
                TDCGExplorer.SetToolTips("error:" + exception.Message);
                Debug.WriteLine(exception.Message);
            }
        }

        // 検索
        private void FindItemToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FindDialog dialog = new FindDialog();
            dialog.Owner = this;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                Cursor.Current = Cursors.WaitCursor;
                foreach (TreeNode node in treeViewArcs.Nodes)
                    TDCGExplorer.FindTreeNode(node, dialog.text);
                foreach (TreeNode node in treeViewZips.Nodes)
                    TDCGExplorer.FindTreeNode(node, dialog.text);
                foreach (TreeNode node in treeViewInstalled.Nodes)
                    TDCGExplorer.FindTreeNode(node, dialog.text);
                foreach (TreeNode node in treeViewCollision.Nodes)
                    TDCGExplorer.FindTreeNode(node, dialog.text);
                foreach (TreeNode node in treeViewTag.Nodes)
                    TDCGExplorer.FindTreeNode(node, dialog.text);
                foreach (TreeNode node in treeViewSaveFile.Nodes)
                    TDCGExplorer.FindTreeNode(node, dialog.text);
                Cursor.Current = Cursors.Default;
            }
        }

        // ZIPファイルを展開する
        private void extractZipFileToolStripMenuItem1_Click_1(object sender, EventArgs e)
        {
            extractZipToolStripMenuItem_Click(sender, e);
        }

        // リストアイテムが選択された.
        private void listBoxMainListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                int index = listBoxMainListBox.SelectedIndex;
                if (index >= 0)
                {
                    LbGenItem item = (LbGenItem)listBoxMainListBox.Items[index];
                    item.DoClick();
                    listBoxMainListBox.Focus();
                }
            }
            catch (Exception exception)
            {
                TDCGExplorer.SetToolTips("error:" + exception.Message);
                Debug.WriteLine(exception.Message);
            }
        }

        // 新規タブでページを開く.
        private void NewTabPageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                TabPage tabPage = new TabPage();
                tabPage.Text = "新しいタブ";
                tabMainView.Controls.Add(tabPage);
                tabMainView.SelectTab(tabMainView.Controls.Count - 1);
            }
            catch (Exception exception)
            {
                TDCGExplorer.SetToolTips("error:" + exception.Message);
                Debug.WriteLine(exception.Message);
            }
        }

        // タブを取得する。無い時は新規に作る.
        private TabPage GetCurrentTagPage()
        {
            if (tabMainView.TabPages.Count == 0)
            {
                TabPage tabPage = new TabPage();
                tabMainView.Controls.Add(tabPage);
                tabMainView.SelectTab(tabMainView.Controls.Count - 1);
            }
            return tabMainView.SelectedTab;
        }

        // タブにコントロールを配置する.
        public void AssignTagPageControl(Control control)
        {
            TabPage currentPage = GetCurrentTagPage();
            control.ClientSize = currentPage.Size;
            currentPage.Text = control.Text;
            Control lastControl = null;
            if (currentPage.Controls.Count > 0) lastControl = currentPage.Controls[0];
            currentPage.Controls.Add(control);
            if (lastControl != null) lastControl.Dispose();
        }

        // アノテーションを表示する
        private void EditAnnotationToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            EditAnnotationToolStripMenuItem_Click(sender, e);
        }

        // ツリーを展開する
        private void ExpandTreeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            expandAllToolStripMenuItem1_Click(sender, e);
        }

        // 新規タブを追加する.
        private void NewTabToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            NewTabPageToolStripMenuItem_Click(sender, e);
        }

        // タブを閉じる.
        private void CloseTabToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TabPage tab = TabControlMainView.SelectedTab;
            if (tab != null) tab.Dispose();
        }

        // ツリー表示をクリアする.
        public void ClearTreeBox()
        {
            lastSelectTreeNode = null;
            lastSelectTreeNodeColor = Color.Transparent;
            treeViewArcs.Nodes.Clear();
            treeViewCollision.Nodes.Clear();
            treeViewZips.Nodes.Clear();
            treeViewInstalled.Nodes.Clear();
            treeViewTag.Nodes.Clear();
            treeViewSaveFile.Nodes.Clear();
            // リストボックスの中身を消去する.
            ListBoxClear();
            // ページを消去する.
            tabMainView.TabPages.Clear();
        }

        // リストボックスを消去する.
        public void ListBoxClear()
        {
            listBoxMainListBox.Items.Clear();
        }

        // フォーカスを失った時にハイライトするツリーノードを覚えておく.
        public void SetSelectedNode(TreeNode node)
        {
            lastSelectTreeNode = node;
        }

        // フォーカスが戻ったので色を戻す.
        private void TreeViewForcusEnter(object sender, EventArgs e)
        {
            ResetColor();
        }

        // フォーカスを失ったのでハイライトする.
        private void TrewViewForcusLeave(object sender, EventArgs e)
        {
            SetColor(lastSelectTreeNode);
        }

        // データベースのツリー表示を構築する.
        public void DisplayDB()
        {
            try
            {
                ClearTreeBox();
                TDCGExplorer.MakeArcsTreeView(treeViewArcs );
                TDCGExplorer.MakeZipsTreeView(treeViewZips);
                TDCGExplorer.MakeCollisionTreeView(treeViewCollision);
                TDCGExplorer.MakeInstalledArcsTreeView(treeViewInstalled);
                TDCGExplorer.MakeTagTreeView(treeViewTag);
                TDCGExplorer.MakeSavefileTreeView(treeViewSaveFile);

                // 初期表示に戻す
                tabControlTreeView.SelectTab(0);
                treeViewArcs.Focus();
                treeViewArcs.SelectedNode = treeViewArcs.Nodes[0];
                TreeView_AfterSelect(treeViewArcs, null);
            }
            catch (Exception e)
            {
                TDCGExplorer.fThreadRun = false;
                TDCGExplorer.SetToolTips("Error occured : " + e.Message);
            }
        }

        private void SetColor(TreeNode node)
        {
            // 以前に設定していた色は戻す.
            ResetColor();
            if (node != null)
            {
                // 新しい場所の色を覚えておく.
                lastSelectTreeNodeForColor = node;
                lastSelectTreeNodeColor = node.BackColor;

                node.BackColor = Color.LightBlue;
            }
        }

        private void ResetColor()
        {
            if (lastSelectTreeNodeForColor != null)
            {
                lastSelectTreeNodeForColor.BackColor = lastSelectTreeNodeColor;
                lastSelectTreeNodeForColor = null;
            }
        }

        // ツリーで選択されたら.
        private void TreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            TreeView treeView = (TreeView)sender;
            try
            {
                TahGenTreeNode node = (TahGenTreeNode)treeView.SelectedNode;
                node.DoTvTreeSelect();
            }
            catch (Exception exception)
            {
                ListBoxClear();
                Debug.WriteLine(exception.Message);
            }
            SetSelectedNode(treeView.SelectedNode);
        }

        // ツリーで選択されたら.
        private void treeViewZips_AfterSelect(object sender, TreeViewEventArgs e)
        {
            TreeView_AfterSelect(sender, e);
        }

        // ツリーで選択されたら.
        private void treeViewArcs_AfterSelect(object sender, TreeViewEventArgs e)
        {
            TreeView_AfterSelect(sender, e);
        }

        // ツリーで選択されたら.
        private void treeViewInstalled_AfterSelect(object sender, TreeViewEventArgs e)
        {
            TreeView_AfterSelect(sender, e);
        }

        // ツリーで選択されたら.
        private void treeViewCollision_AfterSelect(object sender, TreeViewEventArgs e)
        {
            TreeView_AfterSelect(sender, e);
        }

        // ツリーで選択されたら.
        private void treeViewTag_AfterSelect(object sender, TreeViewEventArgs e)
        {
            TreeView_AfterSelect(sender, e);
        }

        // セーブファイルツリー
        private void treeViewSaveFile_AfterSelect(object sender, TreeViewEventArgs e)
        {
            TreeView_AfterSelect(sender, e);
        }

        // フォーカスを獲得した場合
        private void treeViewArcs_Enter(object sender, EventArgs e)
        {
            TreeViewForcusEnter(sender, e);
        }

        // フォーカスを獲得した場合
        private void treeViewZips_Enter(object sender, EventArgs e)
        {
            TreeViewForcusEnter(sender, e);
        }

        // フォーカスを獲得した場合
        private void treeViewInstalled_Enter(object sender, EventArgs e)
        {
            TreeViewForcusEnter(sender, e);
        }

        // フォーカスを獲得した場合
        private void treeViewCollision_Enter(object sender, EventArgs e)
        {
            TreeViewForcusEnter(sender, e);
        }

        // フォーカスを獲得した場合
        private void treeViewTag_Enter(object sender, EventArgs e)
        {
            TreeViewForcusEnter(sender, e);
        }

        private void treeViewSaveFile_Enter(object sender, EventArgs e)
        {
            TreeViewForcusEnter(sender, e);
        }

        // フォーカスを失った場合
        private void treeViewArcs_Leave(object sender, EventArgs e)
        {
            TrewViewForcusLeave(sender, e);
        }

        // フォーカスを失った場合
        private void treeViewZips_Leave(object sender, EventArgs e)
        {
            TrewViewForcusLeave(sender, e);
        }

        // フォーカスを失った場合
        private void treeViewInstalled_Leave(object sender, EventArgs e)
        {
            TrewViewForcusLeave(sender, e);
        }

        // フォーカスを失った場合
        private void treeViewCollision_Leave(object sender, EventArgs e)
        {
            TrewViewForcusLeave(sender, e);
        }

        // フォーカスを失った場合
        private void treeViewTag_Leave(object sender, EventArgs e)
        {
            TrewViewForcusLeave(sender, e);
        }

        private void treeViewSaveFile_Leave(object sender, EventArgs e)
        {
            TrewViewForcusLeave(sender, e);
        }

        // マウスがコントロールにはったらフォーカスを設定する(以下全部)
        private void listBoxMainListBox_MouseEnter(object sender, EventArgs e)
        {
            Control obj = (Control)sender;
            obj.Focus();
        }

        private void TreeViewArcs_MouseEnter(object sender, EventArgs e)
        {
            Control obj = (Control)sender;
            obj.Focus();
        }

        private void treeViewZips_MouseEnter(object sender, EventArgs e)
        {
            Control obj = (Control)sender;
            obj.Focus();
        }

        private void treeViewInstalled_MouseEnter(object sender, EventArgs e)
        {
            Control obj = (Control)sender;
            obj.Focus();
        }

        private void treeViewCollision_MouseEnter(object sender, EventArgs e)
        {
            Control obj = (Control)sender;
            obj.Focus();
        }

        private void treeViewTag_MouseEnter(object sender, EventArgs e)
        {
            Control obj = (Control)sender;
            obj.Focus();
        }

        private void treeViewSaveFile_MouseEnter(object sender, EventArgs e)
        {
            Control obj = (Control)sender;
            obj.Focus();
        }

        // 前提zipファイルを展開する.
        private void ExtractPreferZipToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            try
            {
                ZipTreeNode node = (ZipTreeNode)lastSelectTreeNode;
                TDCGExplorer.InstallPreferZip(node);
            }
            catch (System.InvalidCastException ex)
            {
                MessageBox.Show("この操作は圧縮ファイルにのみ実行できます", "エラー", MessageBoxButtons.OK);
                Debug.WriteLine(ex.Message);
            }
            catch (Exception exception)
            {
                TDCGExplorer.SetToolTips("error:" + exception.Message);
                Debug.WriteLine(exception.Message);
            }
            Cursor.Current = Cursors.Default;
        }

        // 前提zipファイルを展開する.
        private void ExtractPreferZipMainMenuToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExtractPreferZipToolStripMenuItem_Click(sender, e);
        }

        // 関連情報を取得する.
        private void downloadLatestDBZipToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            if (threadCheck() == true) return;
            if (TDCGExplorer.DownloadArcNamesZipFromServer() == true)
            {
                TDCGExplorer.GetArcNamesZipInfo();
            }
            else
            {
                MessageBox.Show("Internet access failure. Please check firewall.", "Download", MessageBoxButtons.OK);
                return;
            }

            if (TDCGExplorer.DownloadTagNamesZipFromServer() == true)
            {
                TDCGExplorer.GetTagNamesZipInfo();
                DisplayDB();
            }
            else
            {
                MessageBox.Show("Internet access failure. Please check firewall.", "Download", MessageBoxButtons.OK);
                return;
            }
            DisplayDB();
            MessageBox.Show("3DCG MODS Referenceサーバから最新情報を取得しました", "Download", MessageBoxButtons.OK);
        }

        private void LookupMODRefToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                ZipTreeNode node = (ZipTreeNode) lastSelectTreeNode;
                AssignTagPageControl(new MODRefPage(node.Entry));
            }
            catch (System.InvalidCastException ex)
            {
                MessageBox.Show("この操作は圧縮ファイルにのみ実行できます", "エラー", MessageBoxButtons.OK);
                Debug.WriteLine(ex.Message);
            }
            catch (Exception exception)
            {
                TDCGExplorer.SetToolTips("error:" + exception.Message);
                Debug.WriteLine(exception.Message);
            }
        }

        private void TahDecryptToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                int index = listBoxMainListBox.SelectedIndex;
                if (index >= 0)
                {
                    LbGenItem item = (LbGenItem)listBoxMainListBox.Items[index];
                    item.DoExtract();
                    listBoxMainListBox.Focus();
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show("ファイル展開エラー:"+exception.Message, "エラー", MessageBoxButtons.OK);
            }
        }

        // タブが切り替わったら選択中の内容を復元する.
        private void tabControlTreeContainor_SelectedIndexChanged(object sender, EventArgs e)
        {
            TreeView [] nodeArray = { treeViewArcs,
                                      treeViewZips,
                                      treeViewInstalled,
                                      treeViewCollision,
                                      treeViewTag,
                                      treeViewSaveFile };

            int index = tabControlTreeView.SelectedIndex;

            TreeNode node = null;

            if (index >= 0 && index <= 5)
            {
                node = nodeArray[index].SelectedNode;
                if (node != null)
                {
                    ResetColor();
                    SetSelectedNode(node);
                    SetColor(node);
                    try
                    {
                        ((TahGenTreeNode)node).DoTvTreeSelect();
                    }
                    catch (Exception ex)
                    {
                        ListBoxClear();
                        Debug.WriteLine(ex.Message);
                    }
                }
                else
                {
                    node = nodeArray[index].Nodes[0];
                    if (node != null)
                    {
                        ResetColor();
                        nodeArray[index].SelectedNode = node;
                        SetColor(node);
                    }
                    else
                    {
                        ListBoxClear();
                    }
                }
            }
        }
    }
}
