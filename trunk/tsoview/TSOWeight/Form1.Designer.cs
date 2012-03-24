namespace TSOWeight
{
    partial class Form1
    {
        /// <summary>
        /// 必要なデザイナ変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースが破棄される場合 true、破棄されない場合は false です。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                viewer.Dispose();
            }
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナで生成されたコード

        /// <summary>
        /// デザイナ サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディタで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.FileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fileNewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fileOpenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.fileSaveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fileSaveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.filePrintToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.filePreviewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.fileExitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.EditToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editUndoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editRedoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.editCutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editCopyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editPasteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.editSelectAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ViewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.modeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toonToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.heatToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.wireToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cameraCenterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cameraResetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cameraSelectedVertexToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cameraSelectedBoneToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.meshToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.meshAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.meshSelectedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.vertexToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.vertexAllToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.vertexCcwToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.vertexNoneToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.boneToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.boneAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.boneNoneToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.HelpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpContentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpIndexToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpSearchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.helpVersionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lbTSOFiles = new System.Windows.Forms.Label();
            this.lvTSOFiles = new System.Windows.Forms.ListView();
            this.columnHeader6 = new System.Windows.Forms.ColumnHeader();
            this.lbMeshes = new System.Windows.Forms.Label();
            this.lvMeshes = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnAssign = new System.Windows.Forms.Button();
            this.btnReduce = new System.Windows.Forms.Button();
            this.lbWeightCaption = new System.Windows.Forms.Label();
            this.lbRadiusCaption = new System.Windows.Forms.Label();
            this.lvSkinWeights = new System.Windows.Forms.ListView();
            this.columnHeader4 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader5 = new System.Windows.Forms.ColumnHeader();
            this.lbSkinWeights = new System.Windows.Forms.Label();
            this.tbWeight = new System.Windows.Forms.TrackBar();
            this.btnGain = new System.Windows.Forms.Button();
            this.tbRadius = new System.Windows.Forms.TrackBar();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.edWeight = new System.Windows.Forms.TextBox();
            this.edRadius = new System.Windows.Forms.TextBox();
            this.menuStrip1.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbWeight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbRadius)).BeginInit();
            this.SuspendLayout();
            // 
            // timer1
            // 
            this.timer1.Interval = 30;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.FileToolStripMenuItem,
            this.EditToolStripMenuItem,
            this.ViewToolStripMenuItem,
            this.HelpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1008, 26);
            this.menuStrip1.TabIndex = 20;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // FileToolStripMenuItem
            // 
            this.FileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileNewToolStripMenuItem,
            this.fileOpenToolStripMenuItem,
            this.toolStripSeparator,
            this.fileSaveToolStripMenuItem,
            this.fileSaveAsToolStripMenuItem,
            this.toolStripSeparator1,
            this.filePrintToolStripMenuItem,
            this.filePreviewToolStripMenuItem,
            this.toolStripSeparator2,
            this.fileExitToolStripMenuItem});
            this.FileToolStripMenuItem.Name = "FileToolStripMenuItem";
            this.FileToolStripMenuItem.Size = new System.Drawing.Size(85, 22);
            this.FileToolStripMenuItem.Text = "ファイル(&F)";
            // 
            // fileNewToolStripMenuItem
            // 
            this.fileNewToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("fileNewToolStripMenuItem.Image")));
            this.fileNewToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.fileNewToolStripMenuItem.Name = "fileNewToolStripMenuItem";
            this.fileNewToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.fileNewToolStripMenuItem.Size = new System.Drawing.Size(201, 22);
            this.fileNewToolStripMenuItem.Text = "新規作成(&N)";
            this.fileNewToolStripMenuItem.Click += new System.EventHandler(this.fileNewToolStripMenuItem_Click);
            // 
            // fileOpenToolStripMenuItem
            // 
            this.fileOpenToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("fileOpenToolStripMenuItem.Image")));
            this.fileOpenToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.fileOpenToolStripMenuItem.Name = "fileOpenToolStripMenuItem";
            this.fileOpenToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.fileOpenToolStripMenuItem.Size = new System.Drawing.Size(201, 22);
            this.fileOpenToolStripMenuItem.Text = "開く(&O)";
            this.fileOpenToolStripMenuItem.Click += new System.EventHandler(this.fileOpenToolStripMenuItem_Click);
            // 
            // toolStripSeparator
            // 
            this.toolStripSeparator.Name = "toolStripSeparator";
            this.toolStripSeparator.Size = new System.Drawing.Size(198, 6);
            // 
            // fileSaveToolStripMenuItem
            // 
            this.fileSaveToolStripMenuItem.Enabled = false;
            this.fileSaveToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("fileSaveToolStripMenuItem.Image")));
            this.fileSaveToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.fileSaveToolStripMenuItem.Name = "fileSaveToolStripMenuItem";
            this.fileSaveToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.fileSaveToolStripMenuItem.Size = new System.Drawing.Size(201, 22);
            this.fileSaveToolStripMenuItem.Text = "上書き保存(&S)";
            // 
            // fileSaveAsToolStripMenuItem
            // 
            this.fileSaveAsToolStripMenuItem.Name = "fileSaveAsToolStripMenuItem";
            this.fileSaveAsToolStripMenuItem.Size = new System.Drawing.Size(201, 22);
            this.fileSaveAsToolStripMenuItem.Text = "名前を付けて保存(&A)";
            this.fileSaveAsToolStripMenuItem.Click += new System.EventHandler(this.fileSaveAsToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(198, 6);
            // 
            // filePrintToolStripMenuItem
            // 
            this.filePrintToolStripMenuItem.Enabled = false;
            this.filePrintToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("filePrintToolStripMenuItem.Image")));
            this.filePrintToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.filePrintToolStripMenuItem.Name = "filePrintToolStripMenuItem";
            this.filePrintToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.P)));
            this.filePrintToolStripMenuItem.Size = new System.Drawing.Size(201, 22);
            this.filePrintToolStripMenuItem.Text = "印刷(&P)";
            // 
            // filePreviewToolStripMenuItem
            // 
            this.filePreviewToolStripMenuItem.Enabled = false;
            this.filePreviewToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("filePreviewToolStripMenuItem.Image")));
            this.filePreviewToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.filePreviewToolStripMenuItem.Name = "filePreviewToolStripMenuItem";
            this.filePreviewToolStripMenuItem.Size = new System.Drawing.Size(201, 22);
            this.filePreviewToolStripMenuItem.Text = "印刷プレビュー(&V)";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(198, 6);
            // 
            // fileExitToolStripMenuItem
            // 
            this.fileExitToolStripMenuItem.Name = "fileExitToolStripMenuItem";
            this.fileExitToolStripMenuItem.Size = new System.Drawing.Size(201, 22);
            this.fileExitToolStripMenuItem.Text = "終了(&X)";
            this.fileExitToolStripMenuItem.Click += new System.EventHandler(this.fileExitToolStripMenuItem_Click);
            // 
            // EditToolStripMenuItem
            // 
            this.EditToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.editUndoToolStripMenuItem,
            this.editRedoToolStripMenuItem,
            this.toolStripSeparator3,
            this.editCutToolStripMenuItem,
            this.editCopyToolStripMenuItem,
            this.editPasteToolStripMenuItem,
            this.toolStripSeparator4,
            this.editSelectAllToolStripMenuItem});
            this.EditToolStripMenuItem.Name = "EditToolStripMenuItem";
            this.EditToolStripMenuItem.Size = new System.Drawing.Size(61, 22);
            this.EditToolStripMenuItem.Text = "編集(&E)";
            // 
            // editUndoToolStripMenuItem
            // 
            this.editUndoToolStripMenuItem.Name = "editUndoToolStripMenuItem";
            this.editUndoToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Z)));
            this.editUndoToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            this.editUndoToolStripMenuItem.Text = "元に戻す(&U)";
            this.editUndoToolStripMenuItem.Click += new System.EventHandler(this.editUndoToolStripMenuItem_Click);
            // 
            // editRedoToolStripMenuItem
            // 
            this.editRedoToolStripMenuItem.Name = "editRedoToolStripMenuItem";
            this.editRedoToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Y)));
            this.editRedoToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            this.editRedoToolStripMenuItem.Text = "やり直し(&R)";
            this.editRedoToolStripMenuItem.Click += new System.EventHandler(this.editRedoToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(187, 6);
            // 
            // editCutToolStripMenuItem
            // 
            this.editCutToolStripMenuItem.Enabled = false;
            this.editCutToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("editCutToolStripMenuItem.Image")));
            this.editCutToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.editCutToolStripMenuItem.Name = "editCutToolStripMenuItem";
            this.editCutToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.X)));
            this.editCutToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            this.editCutToolStripMenuItem.Text = "切り取り(&T)";
            // 
            // editCopyToolStripMenuItem
            // 
            this.editCopyToolStripMenuItem.Enabled = false;
            this.editCopyToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("editCopyToolStripMenuItem.Image")));
            this.editCopyToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.editCopyToolStripMenuItem.Name = "editCopyToolStripMenuItem";
            this.editCopyToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.editCopyToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            this.editCopyToolStripMenuItem.Text = "コピー(&C)";
            // 
            // editPasteToolStripMenuItem
            // 
            this.editPasteToolStripMenuItem.Enabled = false;
            this.editPasteToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("editPasteToolStripMenuItem.Image")));
            this.editPasteToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.editPasteToolStripMenuItem.Name = "editPasteToolStripMenuItem";
            this.editPasteToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
            this.editPasteToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            this.editPasteToolStripMenuItem.Text = "貼り付け(&P)";
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(187, 6);
            // 
            // editSelectAllToolStripMenuItem
            // 
            this.editSelectAllToolStripMenuItem.Enabled = false;
            this.editSelectAllToolStripMenuItem.Name = "editSelectAllToolStripMenuItem";
            this.editSelectAllToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            this.editSelectAllToolStripMenuItem.Text = "すべて選択(&A)";
            // 
            // ViewToolStripMenuItem
            // 
            this.ViewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.modeToolStripMenuItem,
            this.cameraCenterToolStripMenuItem,
            this.meshToolStripMenuItem,
            this.vertexToolStripMenuItem,
            this.boneToolStripMenuItem});
            this.ViewToolStripMenuItem.Name = "ViewToolStripMenuItem";
            this.ViewToolStripMenuItem.Size = new System.Drawing.Size(62, 22);
            this.ViewToolStripMenuItem.Text = "表示(&V)";
            // 
            // modeToolStripMenuItem
            // 
            this.modeToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toonToolStripMenuItem,
            this.heatToolStripMenuItem,
            this.wireToolStripMenuItem});
            this.modeToolStripMenuItem.Name = "modeToolStripMenuItem";
            this.modeToolStripMenuItem.Size = new System.Drawing.Size(178, 22);
            this.modeToolStripMenuItem.Text = "表示形式(&S)";
            // 
            // toonToolStripMenuItem
            // 
            this.toonToolStripMenuItem.Name = "toonToolStripMenuItem";
            this.toonToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.D1)));
            this.toonToolStripMenuItem.Size = new System.Drawing.Size(187, 22);
            this.toonToolStripMenuItem.Text = "トゥーン(&T)";
            this.toonToolStripMenuItem.Click += new System.EventHandler(this.toonToolStripMenuItem_Click);
            // 
            // heatToolStripMenuItem
            // 
            this.heatToolStripMenuItem.Name = "heatToolStripMenuItem";
            this.heatToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.D2)));
            this.heatToolStripMenuItem.Size = new System.Drawing.Size(187, 22);
            this.heatToolStripMenuItem.Text = "ウェイト(&H)";
            this.heatToolStripMenuItem.Click += new System.EventHandler(this.heatToolStripMenuItem_Click);
            // 
            // wireToolStripMenuItem
            // 
            this.wireToolStripMenuItem.Name = "wireToolStripMenuItem";
            this.wireToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.D3)));
            this.wireToolStripMenuItem.Size = new System.Drawing.Size(187, 22);
            this.wireToolStripMenuItem.Text = "ワイヤー(&W)";
            this.wireToolStripMenuItem.Click += new System.EventHandler(this.wireToolStripMenuItem_Click);
            // 
            // cameraCenterToolStripMenuItem
            // 
            this.cameraCenterToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cameraResetToolStripMenuItem,
            this.cameraSelectedVertexToolStripMenuItem,
            this.cameraSelectedBoneToolStripMenuItem});
            this.cameraCenterToolStripMenuItem.Name = "cameraCenterToolStripMenuItem";
            this.cameraCenterToolStripMenuItem.Size = new System.Drawing.Size(178, 22);
            this.cameraCenterToolStripMenuItem.Text = "視点の回転中心(&C)";
            // 
            // cameraResetToolStripMenuItem
            // 
            this.cameraResetToolStripMenuItem.Name = "cameraResetToolStripMenuItem";
            this.cameraResetToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D1)));
            this.cameraResetToolStripMenuItem.Size = new System.Drawing.Size(200, 22);
            this.cameraResetToolStripMenuItem.Text = "初期位置(&R)";
            this.cameraResetToolStripMenuItem.Click += new System.EventHandler(this.cameraResetToolStripMenuItem_Click);
            // 
            // cameraSelectedVertexToolStripMenuItem
            // 
            this.cameraSelectedVertexToolStripMenuItem.Name = "cameraSelectedVertexToolStripMenuItem";
            this.cameraSelectedVertexToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D2)));
            this.cameraSelectedVertexToolStripMenuItem.Size = new System.Drawing.Size(200, 22);
            this.cameraSelectedVertexToolStripMenuItem.Text = "選択頂点(&V)";
            this.cameraSelectedVertexToolStripMenuItem.Click += new System.EventHandler(this.cameraSelectedVertexToolStripMenuItem_Click);
            // 
            // cameraSelectedBoneToolStripMenuItem
            // 
            this.cameraSelectedBoneToolStripMenuItem.Name = "cameraSelectedBoneToolStripMenuItem";
            this.cameraSelectedBoneToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D3)));
            this.cameraSelectedBoneToolStripMenuItem.Size = new System.Drawing.Size(200, 22);
            this.cameraSelectedBoneToolStripMenuItem.Text = "選択ボーン(&B)";
            this.cameraSelectedBoneToolStripMenuItem.Click += new System.EventHandler(this.cameraSelectedBoneToolStripMenuItem_Click);
            // 
            // meshToolStripMenuItem
            // 
            this.meshToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.meshAllToolStripMenuItem,
            this.meshSelectedToolStripMenuItem});
            this.meshToolStripMenuItem.Name = "meshToolStripMenuItem";
            this.meshToolStripMenuItem.Size = new System.Drawing.Size(178, 22);
            this.meshToolStripMenuItem.Text = "メッシュ(&M)";
            // 
            // meshAllToolStripMenuItem
            // 
            this.meshAllToolStripMenuItem.Name = "meshAllToolStripMenuItem";
            this.meshAllToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.A)));
            this.meshAllToolStripMenuItem.Size = new System.Drawing.Size(256, 22);
            this.meshAllToolStripMenuItem.Text = "全てのメッシュを表示(&A)";
            this.meshAllToolStripMenuItem.Click += new System.EventHandler(this.meshAllToolStripMenuItem_Click);
            // 
            // meshSelectedToolStripMenuItem
            // 
            this.meshSelectedToolStripMenuItem.Name = "meshSelectedToolStripMenuItem";
            this.meshSelectedToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.S)));
            this.meshSelectedToolStripMenuItem.Size = new System.Drawing.Size(256, 22);
            this.meshSelectedToolStripMenuItem.Text = "選択メッシュのみ表示(&S)";
            this.meshSelectedToolStripMenuItem.Click += new System.EventHandler(this.meshSelectedToolStripMenuItem_Click);
            // 
            // vertexToolStripMenuItem
            // 
            this.vertexToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.vertexAllToolStripMenuItem1,
            this.vertexCcwToolStripMenuItem,
            this.vertexNoneToolStripMenuItem});
            this.vertexToolStripMenuItem.Name = "vertexToolStripMenuItem";
            this.vertexToolStripMenuItem.Size = new System.Drawing.Size(178, 22);
            this.vertexToolStripMenuItem.Text = "頂点(&V)";
            // 
            // vertexAllToolStripMenuItem1
            // 
            this.vertexAllToolStripMenuItem1.Name = "vertexAllToolStripMenuItem1";
            this.vertexAllToolStripMenuItem1.Size = new System.Drawing.Size(190, 22);
            this.vertexAllToolStripMenuItem1.Text = "全ての頂点を表示(&A)";
            this.vertexAllToolStripMenuItem1.Click += new System.EventHandler(this.vertexAllToolStripMenuItem1_Click);
            // 
            // vertexCcwToolStripMenuItem
            // 
            this.vertexCcwToolStripMenuItem.Name = "vertexCcwToolStripMenuItem";
            this.vertexCcwToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            this.vertexCcwToolStripMenuItem.Text = "表面頂点のみ表示(&S)";
            this.vertexCcwToolStripMenuItem.Click += new System.EventHandler(this.vertexCcwToolStripMenuItem_Click);
            // 
            // vertexNoneToolStripMenuItem
            // 
            this.vertexNoneToolStripMenuItem.Name = "vertexNoneToolStripMenuItem";
            this.vertexNoneToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            this.vertexNoneToolStripMenuItem.Text = "なし(&N)";
            this.vertexNoneToolStripMenuItem.Click += new System.EventHandler(this.vertexNoneToolStripMenuItem_Click);
            // 
            // boneToolStripMenuItem
            // 
            this.boneToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.boneAllToolStripMenuItem,
            this.boneNoneToolStripMenuItem});
            this.boneToolStripMenuItem.Name = "boneToolStripMenuItem";
            this.boneToolStripMenuItem.Size = new System.Drawing.Size(178, 22);
            this.boneToolStripMenuItem.Text = "ボーン(&B)";
            // 
            // boneAllToolStripMenuItem
            // 
            this.boneAllToolStripMenuItem.Name = "boneAllToolStripMenuItem";
            this.boneAllToolStripMenuItem.Size = new System.Drawing.Size(202, 22);
            this.boneAllToolStripMenuItem.Text = "全てのボーンを表示(&A)";
            this.boneAllToolStripMenuItem.Click += new System.EventHandler(this.boneAllToolStripMenuItem_Click);
            // 
            // boneNoneToolStripMenuItem
            // 
            this.boneNoneToolStripMenuItem.Name = "boneNoneToolStripMenuItem";
            this.boneNoneToolStripMenuItem.Size = new System.Drawing.Size(202, 22);
            this.boneNoneToolStripMenuItem.Text = "なし(&N)";
            this.boneNoneToolStripMenuItem.Click += new System.EventHandler(this.boneNoneToolStripMenuItem_Click);
            // 
            // HelpToolStripMenuItem
            // 
            this.HelpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.helpContentToolStripMenuItem,
            this.helpIndexToolStripMenuItem,
            this.helpSearchToolStripMenuItem,
            this.toolStripSeparator5,
            this.helpVersionToolStripMenuItem});
            this.HelpToolStripMenuItem.Name = "HelpToolStripMenuItem";
            this.HelpToolStripMenuItem.Size = new System.Drawing.Size(75, 22);
            this.HelpToolStripMenuItem.Text = "ヘルプ(&H)";
            // 
            // helpContentToolStripMenuItem
            // 
            this.helpContentToolStripMenuItem.Enabled = false;
            this.helpContentToolStripMenuItem.Name = "helpContentToolStripMenuItem";
            this.helpContentToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            this.helpContentToolStripMenuItem.Text = "内容(&C)";
            // 
            // helpIndexToolStripMenuItem
            // 
            this.helpIndexToolStripMenuItem.Enabled = false;
            this.helpIndexToolStripMenuItem.Name = "helpIndexToolStripMenuItem";
            this.helpIndexToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            this.helpIndexToolStripMenuItem.Text = "インデックス(&I)";
            // 
            // helpSearchToolStripMenuItem
            // 
            this.helpSearchToolStripMenuItem.Enabled = false;
            this.helpSearchToolStripMenuItem.Name = "helpSearchToolStripMenuItem";
            this.helpSearchToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            this.helpSearchToolStripMenuItem.Text = "検索(&S)";
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(187, 6);
            // 
            // helpVersionToolStripMenuItem
            // 
            this.helpVersionToolStripMenuItem.Enabled = false;
            this.helpVersionToolStripMenuItem.Name = "helpVersionToolStripMenuItem";
            this.helpVersionToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            this.helpVersionToolStripMenuItem.Text = "バージョン情報(&A)...";
            // 
            // lbTSOFiles
            // 
            this.lbTSOFiles.Location = new System.Drawing.Point(12, 1);
            this.lbTSOFiles.Name = "lbTSOFiles";
            this.lbTSOFiles.Size = new System.Drawing.Size(120, 12);
            this.lbTSOFiles.TabIndex = 0;
            this.lbTSOFiles.Text = "TSOファイル";
            // 
            // lvTSOFiles
            // 
            this.lvTSOFiles.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader6});
            this.lvTSOFiles.FullRowSelect = true;
            this.lvTSOFiles.GridLines = true;
            this.lvTSOFiles.HideSelection = false;
            this.lvTSOFiles.Location = new System.Drawing.Point(14, 16);
            this.lvTSOFiles.MultiSelect = false;
            this.lvTSOFiles.Name = "lvTSOFiles";
            this.lvTSOFiles.Size = new System.Drawing.Size(174, 120);
            this.lvTSOFiles.TabIndex = 1;
            this.lvTSOFiles.UseCompatibleStateImageBehavior = false;
            this.lvTSOFiles.View = System.Windows.Forms.View.Details;
            this.lvTSOFiles.SelectedIndexChanged += new System.EventHandler(this.lvTSOFiles_SelectedIndexChanged);
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "名前";
            // 
            // lbMeshes
            // 
            this.lbMeshes.Location = new System.Drawing.Point(14, 139);
            this.lbMeshes.Name = "lbMeshes";
            this.lbMeshes.Size = new System.Drawing.Size(120, 12);
            this.lbMeshes.TabIndex = 2;
            this.lbMeshes.Text = "メッシュ";
            // 
            // lvMeshes
            // 
            this.lvMeshes.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.lvMeshes.FullRowSelect = true;
            this.lvMeshes.GridLines = true;
            this.lvMeshes.HideSelection = false;
            this.lvMeshes.Location = new System.Drawing.Point(14, 154);
            this.lvMeshes.MultiSelect = false;
            this.lvMeshes.Name = "lvMeshes";
            this.lvMeshes.Size = new System.Drawing.Size(174, 120);
            this.lvMeshes.TabIndex = 3;
            this.lvMeshes.UseCompatibleStateImageBehavior = false;
            this.lvMeshes.View = System.Windows.Forms.View.Details;
            this.lvMeshes.SelectedIndexChanged += new System.EventHandler(this.lvMeshes_SelectedIndexChanged);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "名前";
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Controls.Add(this.edRadius);
            this.panel1.Controls.Add(this.edWeight);
            this.panel1.Controls.Add(this.btnAssign);
            this.panel1.Controls.Add(this.btnReduce);
            this.panel1.Controls.Add(this.lvTSOFiles);
            this.panel1.Controls.Add(this.lbTSOFiles);
            this.panel1.Controls.Add(this.lbWeightCaption);
            this.panel1.Controls.Add(this.lbMeshes);
            this.panel1.Controls.Add(this.lvMeshes);
            this.panel1.Controls.Add(this.lbRadiusCaption);
            this.panel1.Controls.Add(this.lvSkinWeights);
            this.panel1.Controls.Add(this.lbSkinWeights);
            this.panel1.Controls.Add(this.tbWeight);
            this.panel1.Controls.Add(this.btnGain);
            this.panel1.Controls.Add(this.tbRadius);
            this.panel1.Location = new System.Drawing.Point(808, 25);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(200, 681);
            this.panel1.TabIndex = 22;
            // 
            // btnAssign
            // 
            this.btnAssign.Location = new System.Drawing.Point(14, 557);
            this.btnAssign.Name = "btnAssign";
            this.btnAssign.Size = new System.Drawing.Size(84, 23);
            this.btnAssign.TabIndex = 14;
            this.btnAssign.Text = "代入(&A)";
            this.btnAssign.UseVisualStyleBackColor = true;
            this.btnAssign.Click += new System.EventHandler(this.btnAssign_Click);
            // 
            // btnReduce
            // 
            this.btnReduce.Location = new System.Drawing.Point(104, 528);
            this.btnReduce.Name = "btnReduce";
            this.btnReduce.Size = new System.Drawing.Size(84, 23);
            this.btnReduce.TabIndex = 13;
            this.btnReduce.Text = "減算(&R)";
            this.btnReduce.UseVisualStyleBackColor = true;
            this.btnReduce.Click += new System.EventHandler(this.btnReduce_Click);
            // 
            // lbWeightCaption
            // 
            this.lbWeightCaption.Location = new System.Drawing.Point(14, 421);
            this.lbWeightCaption.Name = "lbWeightCaption";
            this.lbWeightCaption.Size = new System.Drawing.Size(120, 12);
            this.lbWeightCaption.TabIndex = 6;
            this.lbWeightCaption.Text = "強度";
            // 
            // lbRadiusCaption
            // 
            this.lbRadiusCaption.Location = new System.Drawing.Point(14, 476);
            this.lbRadiusCaption.Name = "lbRadiusCaption";
            this.lbRadiusCaption.Size = new System.Drawing.Size(120, 12);
            this.lbRadiusCaption.TabIndex = 9;
            this.lbRadiusCaption.Text = "半径";
            // 
            // lvSkinWeights
            // 
            this.lvSkinWeights.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader4,
            this.columnHeader5});
            this.lvSkinWeights.FullRowSelect = true;
            this.lvSkinWeights.GridLines = true;
            this.lvSkinWeights.HideSelection = false;
            this.lvSkinWeights.Location = new System.Drawing.Point(14, 292);
            this.lvSkinWeights.MultiSelect = false;
            this.lvSkinWeights.Name = "lvSkinWeights";
            this.lvSkinWeights.Size = new System.Drawing.Size(174, 120);
            this.lvSkinWeights.TabIndex = 5;
            this.lvSkinWeights.UseCompatibleStateImageBehavior = false;
            this.lvSkinWeights.View = System.Windows.Forms.View.Details;
            this.lvSkinWeights.SelectedIndexChanged += new System.EventHandler(this.lvSkinWeights_SelectedIndexChanged);
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "ボーン";
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "ウェイト";
            // 
            // lbSkinWeights
            // 
            this.lbSkinWeights.Location = new System.Drawing.Point(14, 277);
            this.lbSkinWeights.Name = "lbSkinWeights";
            this.lbSkinWeights.Size = new System.Drawing.Size(120, 12);
            this.lbSkinWeights.TabIndex = 4;
            this.lbSkinWeights.Text = "頂点ウェイト";
            // 
            // tbWeight
            // 
            this.tbWeight.AutoSize = false;
            this.tbWeight.Location = new System.Drawing.Point(16, 443);
            this.tbWeight.Maximum = 200;
            this.tbWeight.Name = "tbWeight";
            this.tbWeight.Size = new System.Drawing.Size(172, 24);
            this.tbWeight.TabIndex = 8;
            this.tbWeight.TickStyle = System.Windows.Forms.TickStyle.None;
            this.tbWeight.Value = 4;
            this.tbWeight.ValueChanged += new System.EventHandler(this.tbWeight_ValueChanged);
            // 
            // btnGain
            // 
            this.btnGain.Location = new System.Drawing.Point(14, 528);
            this.btnGain.Name = "btnGain";
            this.btnGain.Size = new System.Drawing.Size(84, 23);
            this.btnGain.TabIndex = 12;
            this.btnGain.Text = "加算(&G)";
            this.btnGain.UseVisualStyleBackColor = true;
            this.btnGain.Click += new System.EventHandler(this.btnGain_Click);
            // 
            // tbRadius
            // 
            this.tbRadius.AutoSize = false;
            this.tbRadius.Location = new System.Drawing.Point(16, 498);
            this.tbRadius.Maximum = 40;
            this.tbRadius.Name = "tbRadius";
            this.tbRadius.Size = new System.Drawing.Size(172, 24);
            this.tbRadius.TabIndex = 11;
            this.tbRadius.TickStyle = System.Windows.Forms.TickStyle.None;
            this.tbRadius.Value = 20;
            this.tbRadius.ValueChanged += new System.EventHandler(this.tbRadius_ValueChanged);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Location = new System.Drawing.Point(0, 709);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1008, 22);
            this.statusStrip1.TabIndex = 23;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // edWeight
            // 
            this.edWeight.Location = new System.Drawing.Point(104, 418);
            this.edWeight.Name = "edWeight";
            this.edWeight.Size = new System.Drawing.Size(84, 19);
            this.edWeight.TabIndex = 7;
            this.edWeight.Text = "0.020";
            // 
            // edRadius
            // 
            this.edRadius.Location = new System.Drawing.Point(104, 473);
            this.edRadius.Name = "edRadius";
            this.edRadius.Size = new System.Drawing.Size(84, 19);
            this.edRadius.TabIndex = 10;
            this.edRadius.Text = "0.500";
            // 
            // Form1
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1008, 731);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "TSOWeight";
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.Form1_DragDrop);
            this.DragOver += new System.Windows.Forms.DragEventHandler(this.Form1_DragOver);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbWeight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbRadius)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem FileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fileNewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fileOpenToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator;
        private System.Windows.Forms.ToolStripMenuItem fileSaveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fileSaveAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem filePrintToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem filePreviewToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem fileExitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem EditToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editUndoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editRedoToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem editCutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editCopyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editPasteToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem editSelectAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ViewToolStripMenuItem;
        private System.Windows.Forms.Label lbMeshes;
        private System.Windows.Forms.ListView lvMeshes;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lbTSOFiles;
        private System.Windows.Forms.ListView lvTSOFiles;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.Label lbWeightCaption;
        private System.Windows.Forms.Label lbRadiusCaption;
        private System.Windows.Forms.Button btnGain;
        private System.Windows.Forms.TrackBar tbRadius;
        private System.Windows.Forms.TrackBar tbWeight;
        private System.Windows.Forms.ListView lvSkinWeights;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.Label lbSkinWeights;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripMenuItem cameraCenterToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cameraResetToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cameraSelectedVertexToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cameraSelectedBoneToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem modeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toonToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem heatToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem wireToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem meshToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem vertexToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem meshAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem vertexAllToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem vertexCcwToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem vertexNoneToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem meshSelectedToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem HelpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpContentToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpIndexToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpSearchToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripMenuItem helpVersionToolStripMenuItem;
        private System.Windows.Forms.Button btnReduce;
        private System.Windows.Forms.ToolStripMenuItem boneToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem boneAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem boneNoneToolStripMenuItem;
        private System.Windows.Forms.Button btnAssign;
        private System.Windows.Forms.TextBox edWeight;
        private System.Windows.Forms.TextBox edRadius;
    }
}

