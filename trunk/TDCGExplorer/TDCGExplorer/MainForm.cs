using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        private Viewer viewer;
        private bool fInitialTmoLoad = false;

        public MainForm()
        {
            InitializeComponent();
        }

        public TabControl TabControlMainView
        {
            get { return tabMainView; }
            set { }
        }

        public TreeView TabTreeMainView
        {
            get { return tvMainTree; }
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
            TDCGExplorer.IfReadyDbDisplayArcsDB(tvMainTree);

            string windowRect = TDCGExplorer.GetSystemDatabase().window_rectangle;
            string[] rect = windowRect.Split(',');
            Left = int.Parse(rect[0]);
            Top = int.Parse(rect[1]);
            Size = new Size( int.Parse(rect[2]) , int.Parse(rect[3]));

            string splitterDistance = TDCGExplorer.GetSystemDatabase().splitter_distance;
            string[] distance = splitterDistance.Split(',');
            splitContainerV.SplitterDistance = int.Parse(distance[0]);
            splitContainerH.SplitterDistance = int.Parse(distance[1]);
            splitContainerWithView.SplitterDistance = int.Parse(distance[2]);

            viewer = null;
        }

        // invokeの為のdelegate
        private delegate void displayFromArcsHander();

        // 非同期で呼び出されるメソッド
        private void asyncDlgDisplayFromArcs()
        {
            TDCGExplorer.DisplayArcsDB(tvMainTree);
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
            TDCGExplorer.GetSystemDatabase().window_rectangle = widnowRect;

            string splitterDistance = splitContainerV.SplitterDistance.ToString()+","+
                splitContainerH.SplitterDistance.ToString()+","+
                splitContainerWithView.SplitterDistance.ToString();
            TDCGExplorer.GetSystemDatabase().splitter_distance = splitterDistance;

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
                TreeNode node = (TreeNode)tvMainTree.SelectedNode;
                SuspendLayout();
                node.ExpandAll();
                ResumeLayout();
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
            }
        }

        // タブを閉じる.
        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tabMainView.SelectedTab.Dispose();
        }

        // ツリーで選択されたら.
        private void tvMainTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            try
            {
                TahGenTreeNode node = (TahGenTreeNode)tvMainTree.SelectedNode;
                SuspendLayout();
                node.DoTvTreeSelect();
                ResumeLayout();
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
            }
        }

        // リストボックスがダブルクリックされたら.
        private void listBoxMainListBox_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                int index = listBoxMainListBox.SelectedIndex;
                if (index >= 0)
                {
                    LbGenItem item = (LbGenItem)listBoxMainListBox.Items[index];
                    SuspendLayout();
                    item.DoClick(tabMainView);
                    ResumeLayout();
                }
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
            }
        }

        // arcsnames.zipをダウンロードする.
        private void downloadLatestArcsnameszipToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (threadCheck() == true) return;
            if (TDCGExplorer.DownloadArcNamesZipFromServer() == true)
            {
                TDCGExplorer.GetArcNamesZipInfo();
                TDCGExplorer.DisplayArcsDB(tvMainTree);
                MessageBox.Show("Download success, database updated.", "Download", MessageBoxButtons.OK);
            }
            else
            {
                MessageBox.Show("Internet access failure. Please check firewall.", "Download", MessageBoxButtons.OK);
            }
        }

        // ZIPファイルを展開する
        private void extractZipFileToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            try
            {
                ZipTreeNode node = (ZipTreeNode)tvMainTree.SelectedNode;
                if (TDCGExplorer.InstallZipFile(node))
                {
                    //MessageBox.Show("Extracted on work directory", "Extract", MessageBoxButtons.OK);
                }
                else
                {
                    MessageBox.Show("Error occured.", "Extract", MessageBoxButtons.OK);
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show("Please select zip file.", "Extract", MessageBoxButtons.OK);
                Debug.WriteLine(exception.Message);
            }
        }

        // ZIPファイルを展開する
        private void extractZipToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                ZipTreeNode node = (ZipTreeNode)tvMainTree.SelectedNode;
                if (TDCGExplorer.InstallZipFile(node))
                {
                    //MessageBox.Show("Extracted on work directory", "Extract", MessageBoxButtons.OK);
                }
                else
                {
                    MessageBox.Show("Error occured.", "Extract", MessageBoxButtons.OK);
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show("Please select zip file.", "Extract", MessageBoxButtons.OK);
                Debug.WriteLine(exception.Message);
            }
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

        // 初期設定を行う.
        private void editSystemDatabaseToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            if (threadCheck() == true) return;
            TDCGExplorer.EditSystemDatabase();
        }

        // MODサーバに接続する.
        private void lookupMODRelationshipToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                ZipTreeNode node = (ZipTreeNode)tvMainTree.SelectedNode;
                node.DoLookupServer();
            }
            catch (Exception exception)
            {
                MessageBox.Show("Error occured:"+exception.Message, "Server", MessageBoxButtons.OK);
                Debug.WriteLine(exception.Message);
            }

        }
        // アノテーションを編集.
        private void EditAnntationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                ZipTreeNode node = (ZipTreeNode)tvMainTree.SelectedNode;
                node.DoEditAnnotation();
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
            }
        }
    }
}
