using System;
using System.IO;
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
        private bool fNeedCameraReset = true;
        private TreeNode lastSelectTreeNode = null;

        private Color lastSelectTreeNodeColor = Color.Transparent;
        private TreeNode lastSelectTreeNodeForColor = null;

        private Rectangle normalsize;

        private bool closeSplash = true;

        public MainForm()
        {
            InitializeComponent();
        }

        public TabControl TabControlMainView
        {
            get { return tabMainView; }
        }

        public ListBox ListBoxMainView
        {
            get { return listBoxMainListBox; }
        }

        public Viewer Viewer
        {
            get { return viewer; }
        }

        public TreeView ArcsTreeView
        {
            get { return treeViewArcs; }
        }

        public TreeView ZipsTreeView
        {
            get { return treeViewZips; }
        }

        public TreeView SaveFileTreeView
        {
            get { return treeViewSaveFile; }
        }

        public PictureBox PictureBox
        {
            get { return pictureBoxImage; }
        }

        // スレッド実行時はエラーにする.
        private bool threadCheck()
        {
            if (TDCGExplorer.BusyTest()) return true;
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

            // この処理がWindows x64ではたまに失敗するらしい.
            try
            {
                string windowRect = TDCGExplorer.SystemDB.window_rectangle;
                string[] rect = windowRect.Split(',');
                Left = int.Parse(rect[0]);
                Top = int.Parse(rect[1]);
                Size = new Size(int.Parse(rect[2]), int.Parse(rect[3]));
                normalsize = new Rectangle(Location, Size);

                string splitterDistance = TDCGExplorer.SystemDB.splitter_distance;
                string[] distance = splitterDistance.Split(',');
                splitContainerV.SplitterDistance = int.Parse(distance[0]);
                splitContainerH.SplitterDistance = int.Parse(distance[1]);
                splitContainerWithView.SplitterDistance = int.Parse(distance[2]);
            }
            catch (Exception)
            {
            }

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
            catch(Exception)
            {
                viewer.Dispose();
                viewer = null;
            }
            if (closeSplash)
            {
                closeSplash = false;
                SplashForm.CloseSplash();
            }
        }

        // ウインドウが閉じる時の後始末.
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (threadCheck() == true){
                e.Cancel = true;
            }
            string widnowRect = normalsize.Left.ToString() + "," + normalsize.Top.ToString() + "," + normalsize.Width.ToString() + ","+ normalsize.Height.ToString();
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
            if (TDCGExplorer.BusyTest()) return;
            if (tabMainView.SelectedTab != null) tabMainView.SelectedTab.Dispose();
        }

        // ZIPファイルを展開する
        private void extractZipToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            try
            {
                GenericZipTreeNode node = (GenericZipTreeNode)lastSelectTreeNode;
                if (TDCGExplorer.InstallZipFile(node))
                {
                    MessageBox.Show("ファイルを展開しました", "展開成功", MessageBoxButtons.OK);
                }
                else
                {
                    MessageBox.Show("Error extractZipToolStripMenuItem_Click.", "Extract", MessageBoxButtons.OK);
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
                TDCGExplorer.ResetDefaultPose(); // ポーズをデフォルトに戻す.
                TDCGExplorer.FigureLoad = false;
            }
        }

        // 初期TMOを読み込む
        public void doInitialTmoLoad()
        {
            if (viewer != null)
            {
                if (fInitialTmoLoad == false)
                {
                    viewer.LoadTMOFile(TDCGExplorer.defaultpose);
                    if (TDCGExplorer.SystemDB.initialize_camera == true || fNeedCameraReset == true)
                    {
                        doResetCamera();
                        fNeedCameraReset = false;
                    }
                    fInitialTmoLoad = true;
                }
            }
        }

        public void setNeedCameraReset()
        {
            fNeedCameraReset = true;
        }

        public void doResetCamera()
        {
            if (viewer != null)
            {
                viewer.Camera.Reset();
                TSOCameraAutoCenter camera = new TSOCameraAutoCenter(viewer);
                camera.SetCenter(TDCGExplorer.SystemDB.cameracenter);
                camera.TranslateToBone(TDCGExplorer.SystemDB.cameracenter,TDCGExplorer.SystemDB.translateto);
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
                // 次回カメラをリセットする.
                fNeedCameraReset = true;
            }
        }

        public void clearTSOViewer()
        {
            if (viewer != null)
            {
                viewer.ClearFigureList();
                fInitialTmoLoad = false;
                viewer.BackColor = Color.LightGray; // 背景色は灰色に.
            }
        }

        // 初期設定を行う.
        private void editSystemDatabaseToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            if (threadCheck() == true) return;
            TDCGExplorer.EditSystemDatabase();
            DisplayDB();
        }

        // アノテーションを編集.
        private void EditAnnotationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                GenericZipTreeNode node = lastSelectTreeNode as GenericZipTreeNode;
                if (node != null)
                {
                    node.DoEditAnnotation();
                }
                else
                {
                    MessageBox.Show("この操作は圧縮ファイルにのみ実行できます", "エラー", MessageBoxButtons.OK);
                }
            }
            catch (Exception exception)
            {
                TDCGExplorer.SetToolTips("error:" + exception.Message);
                Debug.WriteLine(exception.Message);
            }
        }
#if false
        // 検索
        private void FindItemToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SimpleTextDialog dialog = new SimpleTextDialog();
            dialog.Owner = this;
            dialog.dialogtext = "検索";
            dialog.labeltext = "検索文字列";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string text = dialog.textfield;
                Cursor.Current = Cursors.WaitCursor;
                foreach (TreeNode node in treeViewArcs.Nodes)
                    TDCGExplorer.FindTreeNode(node, text);
                foreach (TreeNode node in treeViewZips.Nodes)
                    TDCGExplorer.FindTreeNode(node, text);
                foreach (TreeNode node in treeViewInstalled.Nodes)
                    TDCGExplorer.FindTreeNode(node, text);
                foreach (TreeNode node in treeViewCollision.Nodes)
                    TDCGExplorer.FindTreeNode(node, text);
                foreach (TreeNode node in treeViewTag.Nodes)
                    TDCGExplorer.FindTreeNode(node, text);
                foreach (TreeNode node in treeViewSaveFile.Nodes)
                    TDCGExplorer.FindTreeNode(node, text);
                Cursor.Current = Cursors.Default;
            }
        }
#endif
        // ZIPファイルを展開する
        private void extractZipFileToolStripMenuItem1_Click_1(object sender, EventArgs e)
        {
            extractZipToolStripMenuItem_Click(sender, e);
        }

        private int lastSelectedListBoxIndex = -1;

        // リストアイテムが選択された.
        private void listBoxMainListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (TDCGExplorer.BusyTest()==true) return;

            try
            {
                int index = listBoxMainListBox.SelectedIndex;
                if (index >= 0)
                {
                    if (index == lastSelectedListBoxIndex) return;

                    lastSelectedListBoxIndex = index;

                    LbGenItem item = listBoxMainListBox.Items[index] as LbGenItem;
                    if (item != null)
                    {
                        item.DoClick();
                        listBoxMainListBox.Focus();
                    }
                }
            }
            catch (Exception exception)
            {
                TDCGExplorer.SetToolTips("error:" + exception.Message);
                Debug.WriteLine(exception.Message);
            }
        }

        public void NewTab()
        {
            TabPage tabPage = new TabPage();
            tabPage.Text = "新しいタブ";
            tabMainView.Controls.Add(tabPage);
            tabMainView.SelectTab(tabMainView.Controls.Count - 1);
        }

        // 新規タブでページを開く.
        private void NewTabPageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewTab();
        }

        // タブを取得する。無い時は新規に作る.
        private TabPage GetCurrentTagPage()
        {
            if (tabMainView.TabPages.Count == 0)
            {
                NewTab();
            }
            // 一番最後のタブを割り当てる.
            tabMainView.SelectTab(tabMainView.Controls.Count - 1);
            return tabMainView.SelectedTab;
        }

        // タブにコントロールを配置する.
        public void AssignTagPageControl(Control control)
        {
            TabPage currentPage = null;
            if (TDCGExplorer.SystemDB.alwaysnewtab == true)
            {
                Type type = control.GetType();
                foreach (TabPage page in tabMainView.TabPages)
                {
                    if (page.Controls.Count > 0)
                    {
                        if (page.Controls[0].GetType() == type) currentPage = page; // 同じ属性のページ.
                    }
                    else
                    {
                        currentPage = page; // 空のページ.
                        break;
                    }
                }
                // 該当ページが無かったら新規ページを作成する.
                if (currentPage == null)
                {
                    NewTab();
                    currentPage = GetCurrentTagPage();
                }
                tabMainView.SelectedTab = currentPage;
            }
            else
            {
                currentPage = GetCurrentTagPage();
            }
            control.ClientSize = currentPage.Size;
            currentPage.Text = control.Text;
            Control lastControl = null;
            if (currentPage.Controls.Count > 0) lastControl = currentPage.Controls[0];
            currentPage.Controls.Add(control);
            if (lastControl != null)
            {
                lastControl.Dispose();
                currentPage.Controls.Remove(lastControl);
            }
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
            // ページを消去する.
#if false
            tabMainView.TabPages.Clear();
#endif
            while (TabControlMainView.TabPages.Count != 0)
            {
                TabControlMainView.TabPages[0].Dispose();
            }

            // リストボックスの中身を消去する.
            ListBoxClear();
            // ツリーを消去する
            lastSelectTreeNode = null;
            lastSelectTreeNodeColor = Color.Transparent;
            treeViewArcs.Nodes.Clear();
            treeViewCollision.Nodes.Clear();
            treeViewZips.Nodes.Clear();
            treeViewInstalled.Nodes.Clear();
            treeViewTag.Nodes.Clear();
            treeViewSaveFile.Nodes.Clear();
        }

        // リストボックスを消去する.
        public void ListBoxClear()
        {
            listBoxMainListBox.Items.Clear();
            lastSelectedListBoxIndex = -1;
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
                TDCGExplorer.MakeArcsTreeView(treeViewArcs);
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
            catch (Exception)
            {
            }
        }
#if false
        public void UpdateSaveFileTree()
        {
            treeViewSaveFile.Nodes.Clear();
            TDCGExplorer.MakeSavefileTreeView(treeViewSaveFile);
        }
#endif
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
            TreeView treeView = sender as TreeView;
            if (treeView != null)
            {
                GenericTahTreeNode node = treeView.SelectedNode as GenericTahTreeNode;
                if (node != null)
                {
                    node.DoTvTreeSelect();
                }
                else
                {
                    ListBoxClear();
                }
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
                GenericZipTreeNode node = lastSelectTreeNode as GenericZipTreeNode;
                if (node != null)
                {
                    TDCGExplorer.InstallPreferZip(node);
                }
                else
                {
                    MessageBox.Show("この操作は圧縮ファイルにのみ実行できます", "エラー", MessageBoxButtons.OK);
                }
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
            if (TDCGExplorer.BusyTest()) return;

            try
            {
                GenericZipTreeNode node = lastSelectTreeNode as GenericZipTreeNode;
                if (node != null)
                {
                    AssignTagPageControl(new MODRefPage(node.Entry));
                }
                else
                {
                    MessageBox.Show("この操作は圧縮ファイルにのみ実行できます", "エラー", MessageBoxButtons.OK);
                }
            }
            catch (Exception exception)
            {
                TDCGExplorer.SetToolTips("error:" + exception.Message);
                Debug.WriteLine(exception.Message);
            }
        }

        private void TahDecryptToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (TDCGExplorer.BusyTest()) return;

            try
            {
                int index = listBoxMainListBox.SelectedIndex;
                if (index >= 0)
                {
                    LbGenItem item = listBoxMainListBox.Items[index] as LbGenItem;
                    if (item != null)
                    {
                        item.DoTahEdit();
                        listBoxMainListBox.Focus();
                    }
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
                        if (threadCheck() == false) ((GenericTahTreeNode)node).DoTvTreeSelect();
                    }
                    catch (Exception ex)
                    {
                        ListBoxClear();
                        Debug.WriteLine(ex.Message);
                    }
                }
                else
                {
                    if (nodeArray[index].Nodes.Count > 0)
                    {
                        node = nodeArray[index].Nodes[0];
                        if (node != null)
                        {
                            ResetColor();
                            nodeArray[index].SelectedNode = node;
                            SetColor(node);
                            return;
                        }
                    }
                    ListBoxClear();
                }
            }
        }

        private void LookupModRefToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LookupMODRefToolStripMenuItem_Click(sender, e);
        }

        // TAHファイルを直接展開する.
        private void ExtractTahFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (TDCGExplorer.BusyTest()) return;

            try
            {
                int index = listBoxMainListBox.SelectedIndex;
                if (index >= 0)
                {
                    LbGenItem item = listBoxMainListBox.Items[index] as LbGenItem;
                    if (item != null)
                    {
                        item.DoDecrypt();
                        listBoxMainListBox.Focus();
                    }
                }
            }
            catch (Exception exception)
            {
                TDCGExplorer.SetToolTips("error:" + exception.Message);
                Debug.WriteLine(exception.Message);
            }
        }

        private void deleteTahEditorFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (TDCGExplorer.BusyTest()) return;

            try
            {
                int index = listBoxMainListBox.SelectedIndex;
                if (index >= 0)
                {
                    LbGenItem item = listBoxMainListBox.Items[index] as LbGenItem;
                    if (item != null)
                    {
                        item.DoDeleteTahEdit();
                        listBoxMainListBox.Focus();
                    }
                }
            }
            catch (Exception exception)
            {
                TDCGExplorer.SetToolTips("error:" + exception.Message);
                Debug.WriteLine(exception.Message);
            }

        }

        // 最新の情報を表示する.
        private void displayUpdateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (threadCheck() == true) return;
            DisplayDB();
        }

        public void SetTahContextMenu(bool flagIsTah)
        {
            if (flagIsTah == true)
            {
                listBoxMainListBox.ContextMenuStrip = ListBoxContextMenuStripTahFile;
            }
            else
            {
                listBoxMainListBox.ContextMenuStrip = contextMenuStripSaveFile;
            }
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            NewTabPageToolStripMenuItem_Click(sender, e);
        }

        // ファイルをドラッグドロップされた→TAHページを作成してTAH梱包の準備をする.
        private void MainForm_DragDrop(object sender, DragEventArgs e)
        {
            if (TDCGExplorer.BusyTest()) return;

            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files.Length == 0) return;

            TDCGExplorer.FileDrop(files);
        }

        private void MainForm_DragEnter(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(DataFormats.FileDrop)) return;
            e.Effect = DragDropEffects.Copy;
        }

        public void AddTimer(EventHandler handler)
        {
            MainTimer.Tick += handler;
        }

        public void DeleteTimer(EventHandler handler)
        {
            MainTimer.Tick -= handler;
        }

        private void OpenFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                TreeView treeView = tabControlTreeView.SelectedTab.Controls[0] as TreeView;
                if (treeView != null)
                {
                    GenericTahTreeNode node = treeView.SelectedNode as GenericTahTreeNode;
                    if (node != null) node.DoExploreNode();
                }
            }
            catch (Exception)
            {
            }
        }

        private void OpenFolderCXToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFolderToolStripMenuItem_Click(sender, e);
        }

        // 00番が見つからないMODを探索する.
        private void findNoBaseModToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (TDCGExplorer.BusyTest() == true) return;
            AssignTagPageControl(new FindBaseModPage());
        }

        public void SelectArcsTreeNode(TreeNode node)
        {
            ResetColor();
            tabControlTreeView.SelectedIndex = 0;
            treeViewArcs.SelectedNode = node;
            SetColor(node);
        }

        public void SelectZipsTreeNode(TreeNode node)
        {
            ResetColor();
            tabControlTreeView.SelectedIndex = 1;
            treeViewZips.SelectedNode = node;
            SetColor(node);
        }

        private void ShowManualToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //System.Diagnostics.Process.Start(@"iexplore.exe", Path.Combine(Directory.GetCurrentDirectory(), @"manual.mht"));
            try
            {
                System.Diagnostics.Process.Start(Path.Combine(Directory.GetCurrentDirectory(), @"manual\\manual.html"));
            }
            catch (Exception)
            {
            }
        }

        private void ShowVersionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string cr = "\n";
            string appVersion = "TDCGExplorer Version " + TDCGExplorer.CONST_APPVERSION;
            string sysDbVersion = "System.db Version " + TDCGExplorer.SystemDB.appversion;
            string arcsDbVersion = "Arcs.db Version " + TDCGExplorer.ArcsDB["version"];
            string directXVersion = "DirectX Version " + TDCGExplorer.GetDirectXVersion();
            string copyright = TDCGExplorer.CONST_COPYRIGHT;
            MessageBox.Show(appVersion+cr+sysDbVersion+cr+arcsDbVersion+cr+directXVersion+cr+cr+copyright , "Version", MessageBoxButtons.OK);
        }

        private void OpenArchiveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TDCGExplorer.ExplorerPath(TDCGExplorer.SystemDB.work_path);
        }

        private void ToolStripMenuItemRename_Click(object sender, EventArgs e)
        {
            if (TDCGExplorer.BusyTest()==true) return;

            try
            {
                int index = listBoxMainListBox.SelectedIndex;
                if (index >= 0)
                {
                    LbSaveFileItem item = listBoxMainListBox.Items[index] as LbSaveFileItem;
                    if (item != null)
                    {
                        item.Rename();
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        private void toolStripMenuItemTouch_Click(object sender, EventArgs e)
        {
            if (TDCGExplorer.BusyTest() == true) return;

            try
            {
                int index = listBoxMainListBox.SelectedIndex;
                if (index >= 0)
                {
                    LbSaveFileItem item = listBoxMainListBox.Items[index] as LbSaveFileItem;
                    if (item != null)
                    {
                        item.Touch();
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        private void toolStripMenuItemTouchAll_Click(object sender, EventArgs e)
        {
            if (TDCGExplorer.BusyTest() == true) return;

            try
            {
                SimpleTextDialog dialog = new SimpleTextDialog();
                dialog.Owner = TDCGExplorer.MainFormWindow;
                dialog.dialogtext = "タイムスタンプの変更";
                dialog.labeltext = "日時";
                dialog.textfield = DateTime.Now.ToString();

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    DateTime newtime = DateTime.Parse(dialog.textfield);
                    foreach (Object itemobject in ListBoxMainView.Items)
                    {
                        LbSaveFileItem item = itemobject as LbSaveFileItem;
                        if (item != null)
                        {
                            item.SetDate(newtime);
                        }
                    }
                }
            }
            catch (Exception)
            {
            }

        }

        private void MainForm_ResizeEnd(object sender, EventArgs e)
        {
            normalsize = new Rectangle(Location, Size);
        }

        private void listBoxMainListBox_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                SaveFilePage savefile = tabMainView.SelectedTab.Controls[0] as SaveFilePage;
                if (savefile != null) savefile.DisplayTso();

                PoseFilePage posefile = tabMainView.SelectedTab.Controls[0] as PoseFilePage;
                if (posefile != null) posefile.DisplayPose();
            }
            catch (Exception)
            {
            }
        }

        private void OpenAllTAHToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                GenericFilesTreeNode node = lastSelectTreeNode as GenericFilesTreeNode;
                if (node != null)
                {
                    //node.DoEditAnnotation();
                    string startwith = node.FullPath.Substring(TDCGExplorer.SystemDB.arcs_path.Length+1);
                    TDCGExplorer.TAHCompaction(startwith);
                }
                else
                {
                    MessageBox.Show("この操作はTAHディレクトリのみ実行できます", "エラー", MessageBoxButtons.OK);
                }
            }
            catch (Exception exception)
            {
                TDCGExplorer.SetToolTips("error:" + exception.Message);
                Debug.WriteLine(exception.Message);
            }
        }

        private void labelSearchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SimpleTextDialog dialog = new SimpleTextDialog();
            dialog.Owner = this;
            dialog.dialogtext = "検索";
            dialog.labeltext = "検索文字列";
            dialog.checkboxenable = true;
            dialog.checkboxtext = "アーカイブも検索する";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string text = dialog.textfield;
                AssignTagPageControl(new FindItemPage(text, dialog.checkboxchecked));
            }
        }

        private void tahfilesearchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SimpleTextDialog dialog = new SimpleTextDialog();
            dialog.Owner = this;
            dialog.dialogtext = "検索";
            dialog.labeltext = "検索文字列";
            dialog.checkboxenable = true;
            dialog.checkboxtext = "アーカイブも検索する";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string text = dialog.textfield;
                AssignTagPageControl(new FindItemPage(text, dialog.checkboxchecked, true));
            }
        }
    }
}
