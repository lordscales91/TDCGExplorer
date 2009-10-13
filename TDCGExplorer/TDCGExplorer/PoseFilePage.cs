using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TDCGExplorer;
using System.Data;
using System.Drawing;
using System.IO;
using System.Diagnostics;
using Microsoft.DirectX;
using System.Drawing.Imaging;

namespace System.Windows.Forms
{
    class PoseFilePage : ZipFilePageControl
    {
        private ContextMenuStrip contextMenuStripPoseSaveData;
        private System.ComponentModel.IContainer components;
        private string filename;
        private ToolStripMenuItem toolStripMenuItemThumbs;
        private ToolStripMenuItem toolStripMenuItemClose;
        private TreeView PoseTreeView;
        private ToolStripMenuItem toolStripMenuItemSaveTmo;
        private ToolStripMenuItem toolStripMenuItemMakeTahFile;
        private PNGPoseData posedata;

        // zipファイルの中から
        public PoseFilePage(GenericTahInfo tahInfo) : base(tahInfo)
        {
            filename = Path.GetFileName(tahInfo.path);
            Cursor.Current = Cursors.WaitCursor;
            try
            {
                InitializeComponent();
                ExtractFile();
                TDCGExplorer.TDCGExplorer.SetToolTips(Text);
            }
            catch (System.InvalidCastException ex)
            {
                Debug.WriteLine(ex.Message);
            }
            Cursor.Current = Cursors.Default;
        }
        // ファイルから直接読み出す.
        public PoseFilePage(string path)
        {
            Cursor.Current = Cursors.WaitCursor;
            try
            {
                InitializeComponent();
                Text = Path.GetFileName(path);
                filename = Path.GetFileName(path);
                TDCGExplorer.TDCGExplorer.SetLastAccessFile = path;
                using (FileStream fs = File.OpenRead(path))
                {
                    Byte[] buffer;
                    BinaryReader reader = new BinaryReader(fs, System.Text.Encoding.Default);
                    buffer = reader.ReadBytes((int)fs.Length);
                    using (MemoryStream ms = new MemoryStream(buffer))
                    {
                        BindingStream(ms);
                    }
                }
                TDCGExplorer.TDCGExplorer.SetToolTips(Text);
            }
            catch (System.InvalidCastException ex)
            {
                Debug.WriteLine(ex.Message);
            }
            Cursor.Current = Cursors.Default;
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.contextMenuStripPoseSaveData = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItemThumbs = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemSaveTmo = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemMakeTahFile = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemClose = new System.Windows.Forms.ToolStripMenuItem();
            this.PoseTreeView = new System.Windows.Forms.TreeView();
            this.contextMenuStripPoseSaveData.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextMenuStripPoseSaveData
            // 
            this.contextMenuStripPoseSaveData.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemThumbs,
            this.toolStripMenuItemSaveTmo,
            this.toolStripMenuItemMakeTahFile,
            this.toolStripMenuItemClose});
            this.contextMenuStripPoseSaveData.Name = "contextMenuStripSaveData";
            this.contextMenuStripPoseSaveData.Size = new System.Drawing.Size(265, 92);
            // 
            // toolStripMenuItemThumbs
            // 
            this.toolStripMenuItemThumbs.Name = "toolStripMenuItemThumbs";
            this.toolStripMenuItemThumbs.Size = new System.Drawing.Size(264, 22);
            this.toolStripMenuItemThumbs.Text = "サムネイルを作成する";
            this.toolStripMenuItemThumbs.Click += new System.EventHandler(this.toolStripMenuItemThumbs_Click);
            // 
            // toolStripMenuItemSaveTmo
            // 
            this.toolStripMenuItemSaveTmo.Name = "toolStripMenuItemSaveTmo";
            this.toolStripMenuItemSaveTmo.Size = new System.Drawing.Size(264, 22);
            this.toolStripMenuItemSaveTmo.Text = "選択されたtmoファイルを保存する";
            this.toolStripMenuItemSaveTmo.Click += new System.EventHandler(this.toolStripMenuItemSaveTmo_Click);
            // 
            // toolStripMenuItemMakeTahFile
            // 
            this.toolStripMenuItemMakeTahFile.Name = "toolStripMenuItemMakeTahFile";
            this.toolStripMenuItemMakeTahFile.Size = new System.Drawing.Size(264, 22);
            this.toolStripMenuItemMakeTahFile.Text = "選択されたtahファイルを作成する";
            this.toolStripMenuItemMakeTahFile.Click += new System.EventHandler(this.toolStripMenuItemMakeTahFile_Click);
            // 
            // toolStripMenuItemClose
            // 
            this.toolStripMenuItemClose.Name = "toolStripMenuItemClose";
            this.toolStripMenuItemClose.Size = new System.Drawing.Size(264, 22);
            this.toolStripMenuItemClose.Text = "閉じる";
            this.toolStripMenuItemClose.Click += new System.EventHandler(this.toolStripMenuItemClose_Click);
            // 
            // PoseTreeView
            // 
            this.PoseTreeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.PoseTreeView.ContextMenuStrip = this.contextMenuStripPoseSaveData;
            this.PoseTreeView.LineColor = System.Drawing.Color.Empty;
            this.PoseTreeView.Location = new System.Drawing.Point(0, 0);
            this.PoseTreeView.Name = "PoseTreeView";
            this.PoseTreeView.Size = new System.Drawing.Size(121, 97);
            this.PoseTreeView.TabIndex = 0;
            // 
            // PoseFilePage
            // 
            this.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.Controls.Add(this.PoseTreeView);
            this.Resize += new System.EventHandler(this.PoseTreeeView_Resize);
            this.Enter += new System.EventHandler(this.PoseFilePage_Enter);
            this.contextMenuStripPoseSaveData.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        public override void BindingStream(MemoryStream ms)
        {
            ms.Seek(0, SeekOrigin.Begin);
            Bitmap savefilebitmap = new Bitmap(ms);

            if (TDCGExplorer.TDCGExplorer.MainFormWindow.Viewer == null) TDCGExplorer.TDCGExplorer.MainFormWindow.makeTSOViwer();
            TDCG.Viewer viewer = TDCGExplorer.TDCGExplorer.MainFormWindow.Viewer;

            PNGPOSEStream posestream = new PNGPOSEStream();
            ms.Seek(0, SeekOrigin.Begin);
            posedata = posestream.LoadStream(ms);

            TDCGExplorer.TDCGExplorer.MainFormWindow.PictureBox.Image = savefilebitmap;
            TDCGExplorer.TDCGExplorer.MainFormWindow.PictureBox.Width = savefilebitmap.Width;
            TDCGExplorer.TDCGExplorer.MainFormWindow.PictureBox.Height = savefilebitmap.Height;

            if (posedata.scene == false)
            {
                // キャラが無い時はデフォ子
                if (TDCGExplorer.TDCGExplorer.FigureLoad == false)
                {
                    viewer.AddFigureFromPNGFile("default.tdcgsav.png", false);
                    TDCGExplorer.TDCGExplorer.FigureLoad = true;
                }
                using (MemoryStream tmo = new MemoryStream(posedata.figures[0].tmo.data))
                {
                    viewer.LoadTMOFile(tmo);
                }
            }
            else
            {
                // 全部ロードする.
                ms.Seek(0, SeekOrigin.Begin);
                viewer.AddFigureFromPNGStream(ms, false);
                TDCGExplorer.TDCGExplorer.FigureLoad = false;
            }
            List<float> camera = posedata.GetCamera();
            Vector3 eye = new Vector3(camera[0], camera[1], camera[2]);
            Vector3 ypr = new Vector3(camera[5], camera[4], camera[6]);
            Matrix m = Matrix.RotationYawPitchRoll(ypr.Y, ypr.X, ypr.Z) * Matrix.Translation(eye.X, eye.Y, eye.Z);
            viewer.Camera.Reset();
            viewer.Camera.Translation = new Vector3(-m.M41, -m.M42, m.M43);
            viewer.Camera.Angle = ypr;

            TDCGExplorer.TDCGExplorer.MainFormWindow.setNeedCameraReset();

            // データ階層ツリーを構築する.

            MakeTreeView();
        }

        private void PoseTreeeView_Resize(object sender, EventArgs e)
        {
            PoseTreeView.Size = Size;
        }

        private void toolStripMenuItemThumbs_Click(object sender, EventArgs e)
        {
            if (TDCGExplorer.TDCGExplorer.BusyTest() == true) return;
            ChangeThumb();
            //TDCGExplorer.TDCGExplorer.MainFormWindow.UpdateSaveFileTree();
        }

        private void ChangeThumb()
        {
            if (TDCGExplorer.TDCGExplorer.BusyTest() == true) return;

            // サーフェースからbitmapを作る.
            Bitmap orgbitmap = TDCGExplorer.TDCGExplorer.MainFormWindow.Viewer.GetBitmap();
            // アイコン用bitmapを作る.
            Bitmap savefilebitmap = new Bitmap(128, 128, PixelFormat.Format24bppRgb);

            // 縮小する.
            int x1, y1, x2, y2;
            if (orgbitmap.Width > orgbitmap.Height)
            {// 横に長い
                int div = orgbitmap.Width - orgbitmap.Height;
                x1 = div / 2;
                x2 = orgbitmap.Width - div / 2;
                y1 = 0;
                y2 = orgbitmap.Height;
            }
            else
            {// 縦に長い
                int div = orgbitmap.Height - orgbitmap.Width;
                x1 = 0;
                x2 = orgbitmap.Width;
                y1 = div / 2;
                y2 = orgbitmap.Height - div / 2;
            }
            Graphics srcG = Graphics.FromImage(orgbitmap);
            Graphics dstG = Graphics.FromImage(savefilebitmap);
            Rectangle dstRect = new Rectangle(0, 0, 128, 128);
            dstG.DrawImage(orgbitmap, dstRect, x1, y1, x2 - x1, y2 - y1, GraphicsUnit.Pixel);

            try
            {
                // ヘビーセーブ形式で保存する.
                if (savefilebitmap == null) return;
                // まずPNG形式のデータを作る.
                MemoryStream basepng = new MemoryStream();
                savefilebitmap.Save(basepng, System.Drawing.Imaging.ImageFormat.Png);
                // PNGFileクラスにデータを取り込む.
                PNGPOSEStream pngstream = new PNGPOSEStream();
                basepng.Seek(0, SeekOrigin.Begin);
                PNGFile png = pngstream.GetPNG(basepng);
                //POSEデータを設定する.
                pngstream.PoseData = posedata;
                // 保存先を決める.
                string savefile_dir = TDCGExplorer.TDCGExplorer.SystemDB.posefile_savedirectory;
                string savefile_name = Path.GetFileNameWithoutExtension(filename) + ".png";

                SaveFileDialog dialog = new SaveFileDialog();
                dialog.FileName = filename;
                dialog.InitialDirectory = TDCGExplorer.TDCGExplorer.SystemDB.posefile_savedirectory;
                dialog.Filter = "PNGファイル(*.tdcgpose.png)|*.tdcgpose.png";
                dialog.FilterIndex = 0;
                dialog.Title = "保存先のファイルを選択してください";
                dialog.RestoreDirectory = true;
                dialog.OverwritePrompt = true;
                dialog.CheckPathExists = true;

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    string destpath = Path.Combine(savefile_dir, dialog.FileName);
                    // 保存先をオープン.
                    File.Delete(destpath);
                    using (Stream output = File.Create(destpath))
                    {
                        // PNGを出力する.
                        pngstream.SavePNGFile(png, output);
                    }

                    TDCGExplorer.TDCGExplorer.MainFormWindow.PictureBox.Image = savefilebitmap;
                    TDCGExplorer.TDCGExplorer.MainFormWindow.PictureBox.Width = savefilebitmap.Width;
                    TDCGExplorer.TDCGExplorer.MainFormWindow.PictureBox.Height = savefilebitmap.Height;
                    // ファイルを追加する.
                    TDCGExplorer.TDCGExplorer.AddFileTree(destpath);
                }
            }
            catch (Exception ex)
            {
                TDCGExplorer.TDCGExplorer.SetToolTips("ファイルセーブエラー:" + ex.Message);
            }
        }

        private void PoseFilePage_Enter(object sender, EventArgs e)
        {
            Control obj = (Control)sender;
            obj.Focus();
        }

        private void MakeTreeView()
        {
            PoseDataNode posenode = new PoseDataNode(filename);
            PoseTreeView.Nodes.Add(posenode);
            // tahを展開する.
            if (posedata.scene)
            {
                PoseDataCameraNode camera = new PoseDataCameraNode("カメラ", posedata.camera);
                posenode.Nodes.Add(camera);
                foreach (TDCGExplorer.PNGPoseFigureData data in posedata.figures)
                {
                    PoseDataFigureNode figure = new PoseDataFigureNode("Figure", data);
                    posenode.Nodes.Add(figure);
                    PoseDataLightNode light = new PoseDataLightNode("ライト", data.light);
                    figure.Nodes.Add(light);
                    PoseDataTMONode tmo = new PoseDataTMONode("TMO", data.tmo);
                    figure.Nodes.Add(tmo);
                    PoseDataTSONode tso = new PoseDataTSONode("TSO", data.tsos);
                    figure.Nodes.Add(tso);
                }
            }
            else
            {
                PoseDataCameraNode camera = new PoseDataCameraNode("カメラ", posedata.camera);
                PoseDataLightNode light = new PoseDataLightNode("ライト", posedata.figures[0].light);
                PoseDataTMONode tmo = new PoseDataTMONode("TMO", posedata.figures[0].tmo);
                posenode.Nodes.Add(camera);
                posenode.Nodes.Add(light);
                posenode.Nodes.Add(tmo);
            }
            posenode.ExpandAll();
        }

        private void toolStripMenuItemSaveTmo_Click(object sender, EventArgs e)
        {
            if (TDCGExplorer.TDCGExplorer.BusyTest() == true) return;

            PoseDataTMONode node = PoseTreeView.SelectedNode as PoseDataTMONode;
            if (node == null)
            {
                MessageBox.Show("この操作はTMOにのみ実行できます", "エラー", MessageBoxButtons.OK);
                return;
            }

            try
            {
                SaveFileDialog dialog = new SaveFileDialog();
                dialog.FileName = filename + ".tmo";
                dialog.InitialDirectory = TDCGExplorer.TDCGExplorer.SystemDB.tahpath;
                dialog.Filter = "TMOファイル(*.tmo)|*.tmo";
                dialog.FilterIndex = 0;
                dialog.Title = "保存先のファイルを選択してください";
                dialog.RestoreDirectory = true;
                dialog.OverwritePrompt = true;
                dialog.CheckPathExists = true;

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    string tmofilename = dialog.FileName;
                    File.Delete(tmofilename);

                    using(MemoryStream ms = new MemoryStream(node.tmo.data))
                    using (Stream fileStream = File.Create(tmofilename))
                    {
                        BufferedStream bufferedDataStream = new BufferedStream(ms);
                        BufferedStream bufferedFileStream = new BufferedStream(fileStream);
                        CopyStream(bufferedDataStream, bufferedFileStream);

                        bufferedFileStream.Flush();
                        bufferedFileStream.Close();
                        bufferedDataStream.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                TDCGExplorer.TDCGExplorer.SetToolTips("エラー:" + ex.Message);
            }

        }

        private static void CopyStream(Stream input, Stream output)
        {
            byte[] buf = new byte[1024];
            int len;
            while ((len = input.Read(buf, 0, buf.Length)) > 0)
            {
                output.Write(buf, 0, len);
            }
        }

        private void toolStripMenuItemMakeTahFile_Click(object sender, EventArgs e)
        {
            if (TDCGExplorer.TDCGExplorer.BusyTest() == true) return;

            PoseDataTSONode node = PoseTreeView.SelectedNode as PoseDataTSONode;
            if (node == null)
            {
                MessageBox.Show("この操作はTSOにのみ実行できます", "エラー", MessageBoxButtons.OK);
                return;
            }

            try
            {
                SimpleTextDialog dialog = new SimpleTextDialog();
                dialog.Owner = TDCGExplorer.TDCGExplorer.MainFormWindow;
                dialog.dialogtext = "TAH形式の保存";
                dialog.labeltext = "ファイル名";
                dialog.textfield = filename;

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    // 新規TAHを作成する.
                    string dbfilename = LBFileTahUtl.GetTahDbPath(dialog.textfield);
                    if (File.Exists(dbfilename))
                    {
                        MessageBox.Show("既にデータベースファイルがあります。\n" + dbfilename + "\n削除してから操作してください。", "エラー", MessageBoxButtons.OK);
                        return;
                    }

                    // 常に新規タブで.
                    TAHEditor editor = new TAHEditor(dbfilename, null);
                    editor.SetInformation(filename + ".tah", 1);
                    editor.makeTAHFile(filename, node.tso);
                    TDCGExplorer.TDCGExplorer.MainFormWindow.AssignTagPageControl(editor);
                    editor.SelectAll();
                }
            }
            catch (Exception ex)
            {
                TDCGExplorer.TDCGExplorer.SetToolTips("エラー:" + ex.Message);
            }

        }

        private void toolStripMenuItemClose_Click(object sender, EventArgs e)
        {
            if (TDCGExplorer.TDCGExplorer.BusyTest()) return;
            Parent.Dispose();
        }

    }

    // topノード
    class PoseDataNode : TreeNode
    {
        public PoseDataNode(string text)
        {
            Text = text;
        }

        // 共通ユーティリティ関数
        public List<float> ByteToFloat(Byte[] data)
        {
            List<float> retval = new List<float>();
            for (int offset = 0; offset < data.Length; offset += sizeof(float))
            {
                float flo = BitConverter.ToSingle(data, offset);
                retval.Add(flo);
            }
            return retval;
        }
    }

    class PoseDataCameraNode : PoseDataNode
    {
        public byte[] camera;
        public PoseDataCameraNode(string text, Byte[] data)
            : base(text)
        {
            camera = data;
        }
    }

    class PoseDataFigureNode : PoseDataNode
    {
        public PNGPoseFigureData figure;
        public PoseDataFigureNode(string text, PNGPoseFigureData data)
            : base(text)
        {
            figure = data;
        }
    }

    class PoseDataLightNode : PoseDataNode
    {
        public PNGPoseLight light;
        public PoseDataLightNode(string text, PNGPoseLight data)
            : base(text)
        {
            light = data;
        }
    }

    class PoseDataTMONode : PoseDataNode
    {
        public PNGPoseTmoData tmo;
        public PoseDataTMONode(string text, PNGPoseTmoData data)
            : base(text)
        {
            tmo = data;
        }
    }

    class PoseDataTSONode : PoseDataNode
    {
        public List<PNGTsoData> tso;
        public PoseDataTSONode(string text, List<PNGTsoData> data)
            : base(text)
        {
            tso = data;
        }
    }
}
