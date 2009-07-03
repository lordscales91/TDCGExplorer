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

        // システムデータベースを編集する.
        private void editSystemDatabaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (threadCheck() == true) return;
            TDCGExplorer.EditSystemDatabase();
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

        // tahを表示する.
        private void displayArcsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TDCGExplorer.DisplayArcsDB(tvMainTree);
        }

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

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tabMainView.SelectedTab.Dispose();
        }

        private void tvMainTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            try
            {
                TahGenTreeNode node = (TahGenTreeNode)tvMainTree.SelectedNode;
                SuspendLayout();
                node.DoTvTreeSelect(node, listBoxMainListBox);
                ResumeLayout();
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
            }
        }

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

        private void downloadLatestArcsnameszipToolStripMenuItem_Click(object sender, EventArgs e)
        {
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

        private void switchToMortionEnabledToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (viewer != null)
            {
                viewer.SwitchMotionEnabled();
            }
        }

        private void deleteLastFigureToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (viewer != null)
            {
                int figures = viewer.FigureList.Count;
                if (figures != 0)
                {
                    viewer.SetFigureIndex(figures);
                    viewer.RemoveSelectedFigure();
                }
            }
        }

        public void makeTSOViwer()
        {
            if (viewer == null)
            {
                viewer = new Viewer();
                viewer.InitializeApplication(splitContainerWithView.Panel2);
                fInitialTmoLoad = false;
            }
        }
    }
}
