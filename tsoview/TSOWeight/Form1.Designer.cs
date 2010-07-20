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
            this.新規作成NToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.開くOToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.上書き保存SToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.名前を付けて保存AToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.印刷PToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.印刷プレビューVToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.終了XToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.EditToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.元に戻すUToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.やり直しRToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.切り取りTToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.コピーCToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.貼り付けPToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.すべて選択AToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
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
            this.HelpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.内容CToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.インデックスIToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.検索SToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.バージョン情報AToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lbTSOFiles = new System.Windows.Forms.Label();
            this.lvTSOFiles = new System.Windows.Forms.ListView();
            this.columnHeader6 = new System.Windows.Forms.ColumnHeader();
            this.lbBoneIndices = new System.Windows.Forms.Label();
            this.lvBoneIndices = new System.Windows.Forms.ListView();
            this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
            this.lbSubMeshes = new System.Windows.Forms.Label();
            this.lbMeshes = new System.Windows.Forms.Label();
            this.lvSubMeshes = new System.Windows.Forms.ListView();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.lvMeshes = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.panel2 = new System.Windows.Forms.Panel();
            this.btnSave = new System.Windows.Forms.Button();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.tbRadius = new System.Windows.Forms.TrackBar();
            this.btnDraw = new System.Windows.Forms.Button();
            this.tbWeight = new System.Windows.Forms.TrackBar();
            this.lbSkinWeights = new System.Windows.Forms.Label();
            this.columnHeader4 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader5 = new System.Windows.Forms.ColumnHeader();
            this.lvSkinWeights = new System.Windows.Forms.ListView();
            this.lbRadius = new System.Windows.Forms.Label();
            this.lbWeight = new System.Windows.Forms.Label();
            this.menuStrip1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbRadius)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbWeight)).BeginInit();
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
            this.新規作成NToolStripMenuItem,
            this.開くOToolStripMenuItem,
            this.toolStripSeparator,
            this.上書き保存SToolStripMenuItem,
            this.名前を付けて保存AToolStripMenuItem,
            this.toolStripSeparator1,
            this.印刷PToolStripMenuItem,
            this.印刷プレビューVToolStripMenuItem,
            this.toolStripSeparator2,
            this.終了XToolStripMenuItem});
            this.FileToolStripMenuItem.Name = "FileToolStripMenuItem";
            this.FileToolStripMenuItem.Size = new System.Drawing.Size(85, 22);
            this.FileToolStripMenuItem.Text = "ファイル(&F)";
            // 
            // 新規作成NToolStripMenuItem
            // 
            this.新規作成NToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("新規作成NToolStripMenuItem.Image")));
            this.新規作成NToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.新規作成NToolStripMenuItem.Name = "新規作成NToolStripMenuItem";
            this.新規作成NToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.新規作成NToolStripMenuItem.Size = new System.Drawing.Size(201, 22);
            this.新規作成NToolStripMenuItem.Text = "新規作成(&N)";
            this.新規作成NToolStripMenuItem.Click += new System.EventHandler(this.新規作成NToolStripMenuItem_Click);
            // 
            // 開くOToolStripMenuItem
            // 
            this.開くOToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("開くOToolStripMenuItem.Image")));
            this.開くOToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.開くOToolStripMenuItem.Name = "開くOToolStripMenuItem";
            this.開くOToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.開くOToolStripMenuItem.Size = new System.Drawing.Size(201, 22);
            this.開くOToolStripMenuItem.Text = "開く(&O)";
            this.開くOToolStripMenuItem.Click += new System.EventHandler(this.開くOToolStripMenuItem_Click);
            // 
            // toolStripSeparator
            // 
            this.toolStripSeparator.Name = "toolStripSeparator";
            this.toolStripSeparator.Size = new System.Drawing.Size(198, 6);
            // 
            // 上書き保存SToolStripMenuItem
            // 
            this.上書き保存SToolStripMenuItem.Enabled = false;
            this.上書き保存SToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("上書き保存SToolStripMenuItem.Image")));
            this.上書き保存SToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.上書き保存SToolStripMenuItem.Name = "上書き保存SToolStripMenuItem";
            this.上書き保存SToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.上書き保存SToolStripMenuItem.Size = new System.Drawing.Size(201, 22);
            this.上書き保存SToolStripMenuItem.Text = "上書き保存(&S)";
            // 
            // 名前を付けて保存AToolStripMenuItem
            // 
            this.名前を付けて保存AToolStripMenuItem.Name = "名前を付けて保存AToolStripMenuItem";
            this.名前を付けて保存AToolStripMenuItem.Size = new System.Drawing.Size(201, 22);
            this.名前を付けて保存AToolStripMenuItem.Text = "名前を付けて保存(&A)";
            this.名前を付けて保存AToolStripMenuItem.Click += new System.EventHandler(this.名前を付けて保存AToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(198, 6);
            // 
            // 印刷PToolStripMenuItem
            // 
            this.印刷PToolStripMenuItem.Enabled = false;
            this.印刷PToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("印刷PToolStripMenuItem.Image")));
            this.印刷PToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.印刷PToolStripMenuItem.Name = "印刷PToolStripMenuItem";
            this.印刷PToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.P)));
            this.印刷PToolStripMenuItem.Size = new System.Drawing.Size(201, 22);
            this.印刷PToolStripMenuItem.Text = "印刷(&P)";
            // 
            // 印刷プレビューVToolStripMenuItem
            // 
            this.印刷プレビューVToolStripMenuItem.Enabled = false;
            this.印刷プレビューVToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("印刷プレビューVToolStripMenuItem.Image")));
            this.印刷プレビューVToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.印刷プレビューVToolStripMenuItem.Name = "印刷プレビューVToolStripMenuItem";
            this.印刷プレビューVToolStripMenuItem.Size = new System.Drawing.Size(201, 22);
            this.印刷プレビューVToolStripMenuItem.Text = "印刷プレビュー(&V)";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(198, 6);
            // 
            // 終了XToolStripMenuItem
            // 
            this.終了XToolStripMenuItem.Name = "終了XToolStripMenuItem";
            this.終了XToolStripMenuItem.Size = new System.Drawing.Size(201, 22);
            this.終了XToolStripMenuItem.Text = "終了(&X)";
            this.終了XToolStripMenuItem.Click += new System.EventHandler(this.終了XToolStripMenuItem_Click);
            // 
            // EditToolStripMenuItem
            // 
            this.EditToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.元に戻すUToolStripMenuItem,
            this.やり直しRToolStripMenuItem,
            this.toolStripSeparator3,
            this.切り取りTToolStripMenuItem,
            this.コピーCToolStripMenuItem,
            this.貼り付けPToolStripMenuItem,
            this.toolStripSeparator4,
            this.すべて選択AToolStripMenuItem});
            this.EditToolStripMenuItem.Name = "EditToolStripMenuItem";
            this.EditToolStripMenuItem.Size = new System.Drawing.Size(61, 22);
            this.EditToolStripMenuItem.Text = "編集(&E)";
            // 
            // 元に戻すUToolStripMenuItem
            // 
            this.元に戻すUToolStripMenuItem.Name = "元に戻すUToolStripMenuItem";
            this.元に戻すUToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Z)));
            this.元に戻すUToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            this.元に戻すUToolStripMenuItem.Text = "元に戻す(&U)";
            this.元に戻すUToolStripMenuItem.Click += new System.EventHandler(this.元に戻すUToolStripMenuItem_Click);
            // 
            // やり直しRToolStripMenuItem
            // 
            this.やり直しRToolStripMenuItem.Name = "やり直しRToolStripMenuItem";
            this.やり直しRToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Y)));
            this.やり直しRToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            this.やり直しRToolStripMenuItem.Text = "やり直し(&R)";
            this.やり直しRToolStripMenuItem.Click += new System.EventHandler(this.やり直しRToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(187, 6);
            // 
            // 切り取りTToolStripMenuItem
            // 
            this.切り取りTToolStripMenuItem.Enabled = false;
            this.切り取りTToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("切り取りTToolStripMenuItem.Image")));
            this.切り取りTToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.切り取りTToolStripMenuItem.Name = "切り取りTToolStripMenuItem";
            this.切り取りTToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.X)));
            this.切り取りTToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            this.切り取りTToolStripMenuItem.Text = "切り取り(&T)";
            // 
            // コピーCToolStripMenuItem
            // 
            this.コピーCToolStripMenuItem.Enabled = false;
            this.コピーCToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("コピーCToolStripMenuItem.Image")));
            this.コピーCToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.コピーCToolStripMenuItem.Name = "コピーCToolStripMenuItem";
            this.コピーCToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.コピーCToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            this.コピーCToolStripMenuItem.Text = "コピー(&C)";
            // 
            // 貼り付けPToolStripMenuItem
            // 
            this.貼り付けPToolStripMenuItem.Enabled = false;
            this.貼り付けPToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("貼り付けPToolStripMenuItem.Image")));
            this.貼り付けPToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.貼り付けPToolStripMenuItem.Name = "貼り付けPToolStripMenuItem";
            this.貼り付けPToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
            this.貼り付けPToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            this.貼り付けPToolStripMenuItem.Text = "貼り付け(&P)";
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(187, 6);
            // 
            // すべて選択AToolStripMenuItem
            // 
            this.すべて選択AToolStripMenuItem.Enabled = false;
            this.すべて選択AToolStripMenuItem.Name = "すべて選択AToolStripMenuItem";
            this.すべて選択AToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            this.すべて選択AToolStripMenuItem.Text = "すべて選択(&A)";
            // 
            // ViewToolStripMenuItem
            // 
            this.ViewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.modeToolStripMenuItem,
            this.cameraCenterToolStripMenuItem,
            this.meshToolStripMenuItem,
            this.vertexToolStripMenuItem});
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
            this.modeToolStripMenuItem.Text = "表示形式";
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
            this.meshAllToolStripMenuItem.Size = new System.Drawing.Size(238, 22);
            this.meshAllToolStripMenuItem.Text = "全てのメッシュを表示";
            this.meshAllToolStripMenuItem.Click += new System.EventHandler(this.meshAllToolStripMenuItem_Click);
            // 
            // meshSelectedToolStripMenuItem
            // 
            this.meshSelectedToolStripMenuItem.Name = "meshSelectedToolStripMenuItem";
            this.meshSelectedToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.S)));
            this.meshSelectedToolStripMenuItem.Size = new System.Drawing.Size(238, 22);
            this.meshSelectedToolStripMenuItem.Text = "選択メッシュのみ表示";
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
            this.vertexAllToolStripMenuItem1.Size = new System.Drawing.Size(172, 22);
            this.vertexAllToolStripMenuItem1.Text = "全ての頂点を表示";
            this.vertexAllToolStripMenuItem1.Click += new System.EventHandler(this.vertexAllToolStripMenuItem1_Click);
            // 
            // vertexCcwToolStripMenuItem
            // 
            this.vertexCcwToolStripMenuItem.Name = "vertexCcwToolStripMenuItem";
            this.vertexCcwToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.vertexCcwToolStripMenuItem.Text = "表面頂点のみ表示";
            this.vertexCcwToolStripMenuItem.Click += new System.EventHandler(this.vertexCcwToolStripMenuItem_Click);
            // 
            // vertexNoneToolStripMenuItem
            // 
            this.vertexNoneToolStripMenuItem.Name = "vertexNoneToolStripMenuItem";
            this.vertexNoneToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.vertexNoneToolStripMenuItem.Text = "なし";
            this.vertexNoneToolStripMenuItem.Click += new System.EventHandler(this.vertexNoneToolStripMenuItem_Click);
            // 
            // HelpToolStripMenuItem
            // 
            this.HelpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.内容CToolStripMenuItem,
            this.インデックスIToolStripMenuItem,
            this.検索SToolStripMenuItem,
            this.toolStripSeparator5,
            this.バージョン情報AToolStripMenuItem});
            this.HelpToolStripMenuItem.Name = "HelpToolStripMenuItem";
            this.HelpToolStripMenuItem.Size = new System.Drawing.Size(75, 22);
            this.HelpToolStripMenuItem.Text = "ヘルプ(&H)";
            // 
            // 内容CToolStripMenuItem
            // 
            this.内容CToolStripMenuItem.Enabled = false;
            this.内容CToolStripMenuItem.Name = "内容CToolStripMenuItem";
            this.内容CToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            this.内容CToolStripMenuItem.Text = "内容(&C)";
            // 
            // インデックスIToolStripMenuItem
            // 
            this.インデックスIToolStripMenuItem.Enabled = false;
            this.インデックスIToolStripMenuItem.Name = "インデックスIToolStripMenuItem";
            this.インデックスIToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            this.インデックスIToolStripMenuItem.Text = "インデックス(&I)";
            // 
            // 検索SToolStripMenuItem
            // 
            this.検索SToolStripMenuItem.Enabled = false;
            this.検索SToolStripMenuItem.Name = "検索SToolStripMenuItem";
            this.検索SToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            this.検索SToolStripMenuItem.Text = "検索(&S)";
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(187, 6);
            // 
            // バージョン情報AToolStripMenuItem
            // 
            this.バージョン情報AToolStripMenuItem.Enabled = false;
            this.バージョン情報AToolStripMenuItem.Name = "バージョン情報AToolStripMenuItem";
            this.バージョン情報AToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            this.バージョン情報AToolStripMenuItem.Text = "バージョン情報(&A)...";
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.panel1.Controls.Add(this.lbTSOFiles);
            this.panel1.Controls.Add(this.lvTSOFiles);
            this.panel1.Controls.Add(this.lbBoneIndices);
            this.panel1.Controls.Add(this.lvBoneIndices);
            this.panel1.Controls.Add(this.lbSubMeshes);
            this.panel1.Controls.Add(this.lbMeshes);
            this.panel1.Controls.Add(this.lvSubMeshes);
            this.panel1.Controls.Add(this.lvMeshes);
            this.panel1.Location = new System.Drawing.Point(0, 26);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(144, 681);
            this.panel1.TabIndex = 21;
            // 
            // lbTSOFiles
            // 
            this.lbTSOFiles.Location = new System.Drawing.Point(12, 1);
            this.lbTSOFiles.Name = "lbTSOFiles";
            this.lbTSOFiles.Size = new System.Drawing.Size(120, 12);
            this.lbTSOFiles.TabIndex = 31;
            this.lbTSOFiles.Text = "TSOファイル";
            // 
            // lvTSOFiles
            // 
            this.lvTSOFiles.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader6});
            this.lvTSOFiles.FullRowSelect = true;
            this.lvTSOFiles.GridLines = true;
            this.lvTSOFiles.HideSelection = false;
            this.lvTSOFiles.Location = new System.Drawing.Point(12, 16);
            this.lvTSOFiles.MultiSelect = false;
            this.lvTSOFiles.Name = "lvTSOFiles";
            this.lvTSOFiles.Size = new System.Drawing.Size(120, 120);
            this.lvTSOFiles.TabIndex = 30;
            this.lvTSOFiles.UseCompatibleStateImageBehavior = false;
            this.lvTSOFiles.View = System.Windows.Forms.View.Details;
            this.lvTSOFiles.SelectedIndexChanged += new System.EventHandler(this.lvTSOFiles_SelectedIndexChanged);
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "名前";
            // 
            // lbBoneIndices
            // 
            this.lbBoneIndices.Location = new System.Drawing.Point(12, 415);
            this.lbBoneIndices.Name = "lbBoneIndices";
            this.lbBoneIndices.Size = new System.Drawing.Size(120, 12);
            this.lbBoneIndices.TabIndex = 13;
            this.lbBoneIndices.Text = "ボーン参照";
            // 
            // lvBoneIndices
            // 
            this.lvBoneIndices.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader3});
            this.lvBoneIndices.FullRowSelect = true;
            this.lvBoneIndices.GridLines = true;
            this.lvBoneIndices.HideSelection = false;
            this.lvBoneIndices.Location = new System.Drawing.Point(12, 430);
            this.lvBoneIndices.MultiSelect = false;
            this.lvBoneIndices.Name = "lvBoneIndices";
            this.lvBoneIndices.Size = new System.Drawing.Size(120, 248);
            this.lvBoneIndices.TabIndex = 12;
            this.lvBoneIndices.UseCompatibleStateImageBehavior = false;
            this.lvBoneIndices.View = System.Windows.Forms.View.Details;
            this.lvBoneIndices.SelectedIndexChanged += new System.EventHandler(this.lvBoneIndices_SelectedIndexChanged);
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "名前";
            // 
            // lbSubMeshes
            // 
            this.lbSubMeshes.Location = new System.Drawing.Point(12, 277);
            this.lbSubMeshes.Name = "lbSubMeshes";
            this.lbSubMeshes.Size = new System.Drawing.Size(120, 12);
            this.lbSubMeshes.TabIndex = 11;
            this.lbSubMeshes.Text = "サブメッシュ";
            // 
            // lbMeshes
            // 
            this.lbMeshes.Location = new System.Drawing.Point(12, 139);
            this.lbMeshes.Name = "lbMeshes";
            this.lbMeshes.Size = new System.Drawing.Size(120, 12);
            this.lbMeshes.TabIndex = 10;
            this.lbMeshes.Text = "メッシュ";
            // 
            // lvSubMeshes
            // 
            this.lvSubMeshes.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader2});
            this.lvSubMeshes.FullRowSelect = true;
            this.lvSubMeshes.GridLines = true;
            this.lvSubMeshes.HideSelection = false;
            this.lvSubMeshes.Location = new System.Drawing.Point(12, 292);
            this.lvSubMeshes.MultiSelect = false;
            this.lvSubMeshes.Name = "lvSubMeshes";
            this.lvSubMeshes.Size = new System.Drawing.Size(120, 120);
            this.lvSubMeshes.TabIndex = 9;
            this.lvSubMeshes.UseCompatibleStateImageBehavior = false;
            this.lvSubMeshes.View = System.Windows.Forms.View.Details;
            this.lvSubMeshes.SelectedIndexChanged += new System.EventHandler(this.lvMeshes_SelectedIndexChanged);
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "名前";
            // 
            // lvMeshes
            // 
            this.lvMeshes.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.lvMeshes.FullRowSelect = true;
            this.lvMeshes.GridLines = true;
            this.lvMeshes.HideSelection = false;
            this.lvMeshes.Location = new System.Drawing.Point(12, 154);
            this.lvMeshes.MultiSelect = false;
            this.lvMeshes.Name = "lvMeshes";
            this.lvMeshes.Size = new System.Drawing.Size(120, 120);
            this.lvMeshes.TabIndex = 8;
            this.lvMeshes.UseCompatibleStateImageBehavior = false;
            this.lvMeshes.View = System.Windows.Forms.View.Details;
            this.lvMeshes.SelectedIndexChanged += new System.EventHandler(this.lvFrames_SelectedIndexChanged);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "名前";
            // 
            // panel2
            // 
            this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panel2.Controls.Add(this.lbWeight);
            this.panel2.Controls.Add(this.btnSave);
            this.panel2.Controls.Add(this.lbRadius);
            this.panel2.Controls.Add(this.lvSkinWeights);
            this.panel2.Controls.Add(this.lbSkinWeights);
            this.panel2.Controls.Add(this.tbWeight);
            this.panel2.Controls.Add(this.btnDraw);
            this.panel2.Controls.Add(this.tbRadius);
            this.panel2.Location = new System.Drawing.Point(866, 26);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(144, 681);
            this.panel2.TabIndex = 22;
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(10, 655);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(120, 23);
            this.btnSave.TabIndex = 29;
            this.btnSave.Text = "保存(&S)";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Location = new System.Drawing.Point(0, 709);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1008, 22);
            this.statusStrip1.TabIndex = 23;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // tbRadius
            // 
            this.tbRadius.Location = new System.Drawing.Point(12, 553);
            this.tbRadius.Maximum = 20;
            this.tbRadius.Name = "tbRadius";
            this.tbRadius.Size = new System.Drawing.Size(120, 45);
            this.tbRadius.TabIndex = 27;
            this.tbRadius.Value = 5;
            this.tbRadius.ValueChanged += new System.EventHandler(this.tbRadius_ValueChanged);
            // 
            // btnDraw
            // 
            this.btnDraw.Location = new System.Drawing.Point(10, 587);
            this.btnDraw.Name = "btnDraw";
            this.btnDraw.Size = new System.Drawing.Size(120, 23);
            this.btnDraw.TabIndex = 28;
            this.btnDraw.Text = "塗る(&D)";
            this.btnDraw.UseVisualStyleBackColor = true;
            this.btnDraw.Click += new System.EventHandler(this.btnDraw_Click);
            // 
            // tbWeight
            // 
            this.tbWeight.Location = new System.Drawing.Point(12, 505);
            this.tbWeight.Maximum = 20;
            this.tbWeight.Name = "tbWeight";
            this.tbWeight.Size = new System.Drawing.Size(120, 45);
            this.tbWeight.TabIndex = 25;
            this.tbWeight.Value = 2;
            this.tbWeight.ValueChanged += new System.EventHandler(this.tbWeight_ValueChanged);
            // 
            // lbSkinWeights
            // 
            this.lbSkinWeights.Location = new System.Drawing.Point(10, 352);
            this.lbSkinWeights.Name = "lbSkinWeights";
            this.lbSkinWeights.Size = new System.Drawing.Size(120, 12);
            this.lbSkinWeights.TabIndex = 22;
            this.lbSkinWeights.Text = "頂点ウェイト";
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "ボーン";
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "ウェイト";
            // 
            // lvSkinWeights
            // 
            this.lvSkinWeights.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader4,
            this.columnHeader5});
            this.lvSkinWeights.FullRowSelect = true;
            this.lvSkinWeights.GridLines = true;
            this.lvSkinWeights.HideSelection = false;
            this.lvSkinWeights.Location = new System.Drawing.Point(10, 367);
            this.lvSkinWeights.MultiSelect = false;
            this.lvSkinWeights.Name = "lvSkinWeights";
            this.lvSkinWeights.Size = new System.Drawing.Size(120, 120);
            this.lvSkinWeights.TabIndex = 23;
            this.lvSkinWeights.UseCompatibleStateImageBehavior = false;
            this.lvSkinWeights.View = System.Windows.Forms.View.Details;
            this.lvSkinWeights.SelectedIndexChanged += new System.EventHandler(this.lvSkinWeights_SelectedIndexChanged);
            // 
            // lbRadius
            // 
            this.lbRadius.Location = new System.Drawing.Point(10, 538);
            this.lbRadius.Name = "lbRadius";
            this.lbRadius.Size = new System.Drawing.Size(120, 12);
            this.lbRadius.TabIndex = 26;
            this.lbRadius.Text = "半径";
            // 
            // lbWeight
            // 
            this.lbWeight.Location = new System.Drawing.Point(10, 490);
            this.lbWeight.Name = "lbWeight";
            this.lbWeight.Size = new System.Drawing.Size(120, 12);
            this.lbWeight.TabIndex = 24;
            this.lbWeight.Text = "加算ウェイト";
            // 
            // Form1
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1008, 731);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.panel2);
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
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbRadius)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbWeight)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem FileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 新規作成NToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 開くOToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator;
        private System.Windows.Forms.ToolStripMenuItem 上書き保存SToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 名前を付けて保存AToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem 印刷PToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 印刷プレビューVToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem 終了XToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem EditToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 元に戻すUToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem やり直しRToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem 切り取りTToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem コピーCToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 貼り付けPToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem すべて選択AToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ViewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem HelpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 内容CToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem インデックスIToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 検索SToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripMenuItem バージョン情報AToolStripMenuItem;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lbBoneIndices;
        private System.Windows.Forms.ListView lvBoneIndices;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.Label lbSubMeshes;
        private System.Windows.Forms.Label lbMeshes;
        private System.Windows.Forms.ListView lvSubMeshes;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ListView lvMeshes;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label lbTSOFiles;
        private System.Windows.Forms.ListView lvTSOFiles;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.Button btnSave;
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
        private System.Windows.Forms.Label lbWeight;
        private System.Windows.Forms.Label lbRadius;
        private System.Windows.Forms.ListView lvSkinWeights;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.Label lbSkinWeights;
        private System.Windows.Forms.TrackBar tbWeight;
        private System.Windows.Forms.Button btnDraw;
        private System.Windows.Forms.TrackBar tbRadius;
    }
}

