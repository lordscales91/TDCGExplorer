namespace TDCGExplorer
{
    partial class MainForm
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
            this.MainMenu = new System.Windows.Forms.MenuStrip();
            this.tDCGExplorerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.extractZipFileToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.関連アーカイブを調べるToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.前提アーカイブファイルを展開ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.FindDialogToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.EditAnnotationToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.databaseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.createFromArcsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editSystemDatabaseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.downloadLatestArcsnameszipToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tSOViewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.resetTSOViewerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.switchToMortionEnabledToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.WindowMenuToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ExpandTreeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.NewTabToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.CloseTabToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.treeViewArcs = new System.Windows.Forms.TreeView();
            this.cmContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.expandAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.extractZipToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.未インストールの前提アーカイブを展開ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.EditAnnotationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.StatusStrip = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.MainTimer = new System.Windows.Forms.Timer(this.components);
            this.tabMainView = new System.Windows.Forms.TabControl();
            this.tabContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.splitContainerV = new System.Windows.Forms.SplitContainer();
            this.tabControlTreeContainor = new System.Windows.Forms.TabControl();
            this.tabPageArcs = new System.Windows.Forms.TabPage();
            this.tabPageZips = new System.Windows.Forms.TabPage();
            this.treeViewZips = new System.Windows.Forms.TreeView();
            this.tabPageIntalled = new System.Windows.Forms.TabPage();
            this.treeViewInstalled = new System.Windows.Forms.TreeView();
            this.tabPageCollsion = new System.Windows.Forms.TabPage();
            this.treeViewCollision = new System.Windows.Forms.TreeView();
            this.tabPageTag = new System.Windows.Forms.TabPage();
            this.treeViewTag = new System.Windows.Forms.TreeView();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.treeViewSaveFile = new System.Windows.Forms.TreeView();
            this.splitContainerH = new System.Windows.Forms.SplitContainer();
            this.splitContainerWithView = new System.Windows.Forms.SplitContainer();
            this.listBoxMainListBox = new System.Windows.Forms.ListBox();
            this.ListBoxContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.NewTabToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ファイルを展開するToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.MainMenu.SuspendLayout();
            this.cmContextMenu.SuspendLayout();
            this.StatusStrip.SuspendLayout();
            this.tabContextMenuStrip.SuspendLayout();
            this.splitContainerV.Panel1.SuspendLayout();
            this.splitContainerV.Panel2.SuspendLayout();
            this.splitContainerV.SuspendLayout();
            this.tabControlTreeContainor.SuspendLayout();
            this.tabPageArcs.SuspendLayout();
            this.tabPageZips.SuspendLayout();
            this.tabPageIntalled.SuspendLayout();
            this.tabPageCollsion.SuspendLayout();
            this.tabPageTag.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.splitContainerH.Panel1.SuspendLayout();
            this.splitContainerH.Panel2.SuspendLayout();
            this.splitContainerH.SuspendLayout();
            this.splitContainerWithView.Panel1.SuspendLayout();
            this.splitContainerWithView.SuspendLayout();
            this.ListBoxContextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // MainMenu
            // 
            this.MainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tDCGExplorerToolStripMenuItem,
            this.databaseToolStripMenuItem,
            this.tSOViewToolStripMenuItem,
            this.WindowMenuToolStripMenuItem});
            this.MainMenu.Location = new System.Drawing.Point(0, 0);
            this.MainMenu.Name = "MainMenu";
            this.MainMenu.Size = new System.Drawing.Size(817, 24);
            this.MainMenu.TabIndex = 1;
            this.MainMenu.Text = "MainMenu";
            // 
            // tDCGExplorerToolStripMenuItem
            // 
            this.tDCGExplorerToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.extractZipFileToolStripMenuItem1,
            this.関連アーカイブを調べるToolStripMenuItem,
            this.前提アーカイブファイルを展開ToolStripMenuItem,
            this.FindDialogToolStripMenuItem,
            this.EditAnnotationToolStripMenuItem1,
            this.exitToolStripMenuItem});
            this.tDCGExplorerToolStripMenuItem.Name = "tDCGExplorerToolStripMenuItem";
            this.tDCGExplorerToolStripMenuItem.Size = new System.Drawing.Size(67, 20);
            this.tDCGExplorerToolStripMenuItem.Text = "ファイル";
            // 
            // extractZipFileToolStripMenuItem1
            // 
            this.extractZipFileToolStripMenuItem1.Name = "extractZipFileToolStripMenuItem1";
            this.extractZipFileToolStripMenuItem1.Size = new System.Drawing.Size(242, 22);
            this.extractZipFileToolStripMenuItem1.Text = "アーカイブファイルの展開";
            this.extractZipFileToolStripMenuItem1.Click += new System.EventHandler(this.extractZipFileToolStripMenuItem1_Click_1);
            // 
            // 関連アーカイブを調べるToolStripMenuItem
            // 
            this.関連アーカイブを調べるToolStripMenuItem.Name = "関連アーカイブを調べるToolStripMenuItem";
            this.関連アーカイブを調べるToolStripMenuItem.Size = new System.Drawing.Size(242, 22);
            this.関連アーカイブを調べるToolStripMenuItem.Text = "関連アーカイブを調べる";
            this.関連アーカイブを調べるToolStripMenuItem.Click += new System.EventHandler(this.LookupMODRefToolStripMenuItem_Click);
            // 
            // 前提アーカイブファイルを展開ToolStripMenuItem
            // 
            this.前提アーカイブファイルを展開ToolStripMenuItem.Name = "前提アーカイブファイルを展開ToolStripMenuItem";
            this.前提アーカイブファイルを展開ToolStripMenuItem.Size = new System.Drawing.Size(242, 22);
            this.前提アーカイブファイルを展開ToolStripMenuItem.Text = "前提アーカイブファイルを展開";
            this.前提アーカイブファイルを展開ToolStripMenuItem.Click += new System.EventHandler(this.ExtractPreferZipMainMenuToolStripMenuItem_Click);
            // 
            // FindDialogToolStripMenuItem
            // 
            this.FindDialogToolStripMenuItem.Name = "FindDialogToolStripMenuItem";
            this.FindDialogToolStripMenuItem.Size = new System.Drawing.Size(242, 22);
            this.FindDialogToolStripMenuItem.Text = "検索...";
            this.FindDialogToolStripMenuItem.Click += new System.EventHandler(this.FindItemToolStripMenuItem_Click);
            // 
            // EditAnnotationToolStripMenuItem1
            // 
            this.EditAnnotationToolStripMenuItem1.Name = "EditAnnotationToolStripMenuItem1";
            this.EditAnnotationToolStripMenuItem1.Size = new System.Drawing.Size(242, 22);
            this.EditAnnotationToolStripMenuItem1.Text = "注訳を入力...";
            this.EditAnnotationToolStripMenuItem1.Click += new System.EventHandler(this.EditAnnotationToolStripMenuItem1_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(242, 22);
            this.exitToolStripMenuItem.Text = "終了";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // databaseToolStripMenuItem
            // 
            this.databaseToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.createFromArcsToolStripMenuItem,
            this.editSystemDatabaseToolStripMenuItem,
            this.downloadLatestArcsnameszipToolStripMenuItem});
            this.databaseToolStripMenuItem.Name = "databaseToolStripMenuItem";
            this.databaseToolStripMenuItem.Size = new System.Drawing.Size(91, 20);
            this.databaseToolStripMenuItem.Text = "データベース";
            // 
            // createFromArcsToolStripMenuItem
            // 
            this.createFromArcsToolStripMenuItem.Name = "createFromArcsToolStripMenuItem";
            this.createFromArcsToolStripMenuItem.Size = new System.Drawing.Size(242, 22);
            this.createFromArcsToolStripMenuItem.Text = "データベースの構築・更新";
            this.createFromArcsToolStripMenuItem.Click += new System.EventHandler(this.createFromArcsToolStripMenuItem_Click);
            // 
            // editSystemDatabaseToolStripMenuItem
            // 
            this.editSystemDatabaseToolStripMenuItem.Name = "editSystemDatabaseToolStripMenuItem";
            this.editSystemDatabaseToolStripMenuItem.Size = new System.Drawing.Size(242, 22);
            this.editSystemDatabaseToolStripMenuItem.Text = "データベース初期設定";
            this.editSystemDatabaseToolStripMenuItem.Click += new System.EventHandler(this.editSystemDatabaseToolStripMenuItem_Click_1);
            // 
            // downloadLatestArcsnameszipToolStripMenuItem
            // 
            this.downloadLatestArcsnameszipToolStripMenuItem.Name = "downloadLatestArcsnameszipToolStripMenuItem";
            this.downloadLatestArcsnameszipToolStripMenuItem.Size = new System.Drawing.Size(242, 22);
            this.downloadLatestArcsnameszipToolStripMenuItem.Text = "最新のデータベース情報を取得";
            this.downloadLatestArcsnameszipToolStripMenuItem.Click += new System.EventHandler(this.downloadLatestDBZipToolStripMenuItem_Click_1);
            // 
            // tSOViewToolStripMenuItem
            // 
            this.tSOViewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.resetTSOViewerToolStripMenuItem,
            this.switchToMortionEnabledToolStripMenuItem});
            this.tSOViewToolStripMenuItem.Name = "tSOViewToolStripMenuItem";
            this.tSOViewToolStripMenuItem.Size = new System.Drawing.Size(89, 20);
            this.tSOViewToolStripMenuItem.Text = "TSOビューワ";
            // 
            // resetTSOViewerToolStripMenuItem
            // 
            this.resetTSOViewerToolStripMenuItem.Name = "resetTSOViewerToolStripMenuItem";
            this.resetTSOViewerToolStripMenuItem.Size = new System.Drawing.Size(182, 22);
            this.resetTSOViewerToolStripMenuItem.Text = "ビューワをリセット";
            this.resetTSOViewerToolStripMenuItem.Click += new System.EventHandler(this.resetTSOViewerToolStripMenuItem_Click);
            // 
            // switchToMortionEnabledToolStripMenuItem
            // 
            this.switchToMortionEnabledToolStripMenuItem.Name = "switchToMortionEnabledToolStripMenuItem";
            this.switchToMortionEnabledToolStripMenuItem.Size = new System.Drawing.Size(182, 22);
            this.switchToMortionEnabledToolStripMenuItem.Text = "アニメーション開始";
            this.switchToMortionEnabledToolStripMenuItem.Click += new System.EventHandler(this.switchToMortionEnabledToolStripMenuItem_Click);
            // 
            // WindowMenuToolStripMenuItem
            // 
            this.WindowMenuToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ExpandTreeToolStripMenuItem,
            this.NewTabToolStripMenuItem1,
            this.CloseTabToolStripMenuItem});
            this.WindowMenuToolStripMenuItem.Name = "WindowMenuToolStripMenuItem";
            this.WindowMenuToolStripMenuItem.Size = new System.Drawing.Size(79, 20);
            this.WindowMenuToolStripMenuItem.Text = "ウインドウ";
            // 
            // ExpandTreeToolStripMenuItem
            // 
            this.ExpandTreeToolStripMenuItem.Name = "ExpandTreeToolStripMenuItem";
            this.ExpandTreeToolStripMenuItem.Size = new System.Drawing.Size(170, 22);
            this.ExpandTreeToolStripMenuItem.Text = "ツリーを展開";
            this.ExpandTreeToolStripMenuItem.Click += new System.EventHandler(this.ExpandTreeToolStripMenuItem_Click);
            // 
            // NewTabToolStripMenuItem1
            // 
            this.NewTabToolStripMenuItem1.Name = "NewTabToolStripMenuItem1";
            this.NewTabToolStripMenuItem1.Size = new System.Drawing.Size(170, 22);
            this.NewTabToolStripMenuItem1.Text = "新しいタブを開く";
            this.NewTabToolStripMenuItem1.Click += new System.EventHandler(this.NewTabToolStripMenuItem1_Click);
            // 
            // CloseTabToolStripMenuItem
            // 
            this.CloseTabToolStripMenuItem.Name = "CloseTabToolStripMenuItem";
            this.CloseTabToolStripMenuItem.Size = new System.Drawing.Size(170, 22);
            this.CloseTabToolStripMenuItem.Text = "タブを閉じる";
            this.CloseTabToolStripMenuItem.Click += new System.EventHandler(this.CloseTabToolStripMenuItem_Click);
            // 
            // treeViewArcs
            // 
            this.treeViewArcs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.treeViewArcs.ContextMenuStrip = this.cmContextMenu;
            this.treeViewArcs.Location = new System.Drawing.Point(3, 3);
            this.treeViewArcs.Margin = new System.Windows.Forms.Padding(0);
            this.treeViewArcs.Name = "treeViewArcs";
            this.treeViewArcs.Size = new System.Drawing.Size(159, 331);
            this.treeViewArcs.TabIndex = 3;
            this.treeViewArcs.Enter += new System.EventHandler(this.treeViewArcs_Enter);
            this.treeViewArcs.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeViewArcs_AfterSelect);
            this.treeViewArcs.MouseEnter += new System.EventHandler(this.TreeViewArcs_MouseEnter);
            this.treeViewArcs.Leave += new System.EventHandler(this.treeViewArcs_Leave);
            // 
            // cmContextMenu
            // 
            this.cmContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.expandAllToolStripMenuItem,
            this.extractZipToolStripMenuItem,
            this.未インストールの前提アーカイブを展開ToolStripMenuItem,
            this.EditAnnotationToolStripMenuItem});
            this.cmContextMenu.Name = "cmContextMenu";
            this.cmContextMenu.Size = new System.Drawing.Size(243, 92);
            // 
            // expandAllToolStripMenuItem
            // 
            this.expandAllToolStripMenuItem.Name = "expandAllToolStripMenuItem";
            this.expandAllToolStripMenuItem.Size = new System.Drawing.Size(242, 22);
            this.expandAllToolStripMenuItem.Text = "ツリーを展開";
            this.expandAllToolStripMenuItem.Click += new System.EventHandler(this.expandAllToolStripMenuItem1_Click);
            // 
            // extractZipToolStripMenuItem
            // 
            this.extractZipToolStripMenuItem.Name = "extractZipToolStripMenuItem";
            this.extractZipToolStripMenuItem.Size = new System.Drawing.Size(242, 22);
            this.extractZipToolStripMenuItem.Text = "アーカイブファイルを展開";
            this.extractZipToolStripMenuItem.Click += new System.EventHandler(this.extractZipToolStripMenuItem_Click);
            // 
            // 未インストールの前提アーカイブを展開ToolStripMenuItem
            // 
            this.未インストールの前提アーカイブを展開ToolStripMenuItem.Name = "未インストールの前提アーカイブを展開ToolStripMenuItem";
            this.未インストールの前提アーカイブを展開ToolStripMenuItem.Size = new System.Drawing.Size(242, 22);
            this.未インストールの前提アーカイブを展開ToolStripMenuItem.Text = "前提アーカイブファイルを展開";
            this.未インストールの前提アーカイブを展開ToolStripMenuItem.Click += new System.EventHandler(this.ExtractPreferZipToolStripMenuItem_Click);
            // 
            // EditAnnotationToolStripMenuItem
            // 
            this.EditAnnotationToolStripMenuItem.Name = "EditAnnotationToolStripMenuItem";
            this.EditAnnotationToolStripMenuItem.Size = new System.Drawing.Size(242, 22);
            this.EditAnnotationToolStripMenuItem.Text = "注訳を入力";
            this.EditAnnotationToolStripMenuItem.Click += new System.EventHandler(this.EditAnnotationToolStripMenuItem_Click);
            // 
            // StatusStrip
            // 
            this.StatusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel});
            this.StatusStrip.Location = new System.Drawing.Point(0, 435);
            this.StatusStrip.Name = "StatusStrip";
            this.StatusStrip.Size = new System.Drawing.Size(817, 22);
            this.StatusStrip.TabIndex = 4;
            this.StatusStrip.Text = "StatusStrip";
            // 
            // toolStripStatusLabel
            // 
            this.toolStripStatusLabel.Name = "toolStripStatusLabel";
            this.toolStripStatusLabel.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never;
            this.toolStripStatusLabel.Size = new System.Drawing.Size(802, 17);
            this.toolStripStatusLabel.Spring = true;
            this.toolStripStatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // MainTimer
            // 
            this.MainTimer.Enabled = true;
            this.MainTimer.Tick += new System.EventHandler(this.MainTimer_Tick);
            // 
            // tabMainView
            // 
            this.tabMainView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tabMainView.ContextMenuStrip = this.tabContextMenuStrip;
            this.tabMainView.Location = new System.Drawing.Point(0, 0);
            this.tabMainView.Multiline = true;
            this.tabMainView.Name = "tabMainView";
            this.tabMainView.SelectedIndex = 0;
            this.tabMainView.Size = new System.Drawing.Size(622, 236);
            this.tabMainView.TabIndex = 5;
            // 
            // tabContextMenuStrip
            // 
            this.tabContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.closeToolStripMenuItem});
            this.tabContextMenuStrip.Name = "tabContextMenuStrip";
            this.tabContextMenuStrip.Size = new System.Drawing.Size(104, 26);
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.closeToolStripMenuItem.Text = "Close";
            this.closeToolStripMenuItem.Click += new System.EventHandler(this.closeToolStripMenuItem_Click);
            // 
            // splitContainerV
            // 
            this.splitContainerV.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainerV.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitContainerV.Location = new System.Drawing.Point(0, 27);
            this.splitContainerV.Margin = new System.Windows.Forms.Padding(0);
            this.splitContainerV.Name = "splitContainerV";
            // 
            // splitContainerV.Panel1
            // 
            this.splitContainerV.Panel1.Controls.Add(this.tabControlTreeContainor);
            // 
            // splitContainerV.Panel2
            // 
            this.splitContainerV.Panel2.Controls.Add(this.splitContainerH);
            this.splitContainerV.Size = new System.Drawing.Size(817, 407);
            this.splitContainerV.SplitterDistance = 181;
            this.splitContainerV.SplitterWidth = 8;
            this.splitContainerV.TabIndex = 6;
            // 
            // tabControlTreeContainor
            // 
            this.tabControlTreeContainor.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControlTreeContainor.Controls.Add(this.tabPageArcs);
            this.tabControlTreeContainor.Controls.Add(this.tabPageZips);
            this.tabControlTreeContainor.Controls.Add(this.tabPageIntalled);
            this.tabControlTreeContainor.Controls.Add(this.tabPageCollsion);
            this.tabControlTreeContainor.Controls.Add(this.tabPageTag);
            this.tabControlTreeContainor.Controls.Add(this.tabPage1);
            this.tabControlTreeContainor.Location = new System.Drawing.Point(3, 3);
            this.tabControlTreeContainor.Multiline = true;
            this.tabControlTreeContainor.Name = "tabControlTreeContainor";
            this.tabControlTreeContainor.SelectedIndex = 0;
            this.tabControlTreeContainor.Size = new System.Drawing.Size(173, 399);
            this.tabControlTreeContainor.TabIndex = 4;
            // 
            // tabPageArcs
            // 
            this.tabPageArcs.Controls.Add(this.treeViewArcs);
            this.tabPageArcs.Location = new System.Drawing.Point(4, 58);
            this.tabPageArcs.Name = "tabPageArcs";
            this.tabPageArcs.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageArcs.Size = new System.Drawing.Size(165, 337);
            this.tabPageArcs.TabIndex = 0;
            this.tabPageArcs.Text = "Arcs";
            this.tabPageArcs.UseVisualStyleBackColor = true;
            // 
            // tabPageZips
            // 
            this.tabPageZips.Controls.Add(this.treeViewZips);
            this.tabPageZips.Location = new System.Drawing.Point(4, 58);
            this.tabPageZips.Name = "tabPageZips";
            this.tabPageZips.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageZips.Size = new System.Drawing.Size(165, 337);
            this.tabPageZips.TabIndex = 1;
            this.tabPageZips.Text = "アーカイブ";
            this.tabPageZips.UseVisualStyleBackColor = true;
            // 
            // treeViewZips
            // 
            this.treeViewZips.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.treeViewZips.ContextMenuStrip = this.cmContextMenu;
            this.treeViewZips.Location = new System.Drawing.Point(3, 3);
            this.treeViewZips.Margin = new System.Windows.Forms.Padding(0);
            this.treeViewZips.Name = "treeViewZips";
            this.treeViewZips.Size = new System.Drawing.Size(159, 331);
            this.treeViewZips.TabIndex = 4;
            this.treeViewZips.Enter += new System.EventHandler(this.treeViewZips_Enter);
            this.treeViewZips.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeViewZips_AfterSelect);
            this.treeViewZips.MouseEnter += new System.EventHandler(this.treeViewZips_MouseEnter);
            this.treeViewZips.Leave += new System.EventHandler(this.treeViewZips_Leave);
            // 
            // tabPageIntalled
            // 
            this.tabPageIntalled.Controls.Add(this.treeViewInstalled);
            this.tabPageIntalled.Location = new System.Drawing.Point(4, 58);
            this.tabPageIntalled.Name = "tabPageIntalled";
            this.tabPageIntalled.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageIntalled.Size = new System.Drawing.Size(165, 337);
            this.tabPageIntalled.TabIndex = 2;
            this.tabPageIntalled.Text = "インストール済み";
            this.tabPageIntalled.UseVisualStyleBackColor = true;
            // 
            // treeViewInstalled
            // 
            this.treeViewInstalled.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.treeViewInstalled.ContextMenuStrip = this.cmContextMenu;
            this.treeViewInstalled.Location = new System.Drawing.Point(3, 3);
            this.treeViewInstalled.Margin = new System.Windows.Forms.Padding(0);
            this.treeViewInstalled.Name = "treeViewInstalled";
            this.treeViewInstalled.Size = new System.Drawing.Size(159, 331);
            this.treeViewInstalled.TabIndex = 4;
            this.treeViewInstalled.Enter += new System.EventHandler(this.treeViewInstalled_Enter);
            this.treeViewInstalled.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeViewInstalled_AfterSelect);
            this.treeViewInstalled.MouseEnter += new System.EventHandler(this.treeViewInstalled_MouseEnter);
            this.treeViewInstalled.Leave += new System.EventHandler(this.treeViewInstalled_Leave);
            // 
            // tabPageCollsion
            // 
            this.tabPageCollsion.Controls.Add(this.treeViewCollision);
            this.tabPageCollsion.Location = new System.Drawing.Point(4, 58);
            this.tabPageCollsion.Name = "tabPageCollsion";
            this.tabPageCollsion.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageCollsion.Size = new System.Drawing.Size(165, 337);
            this.tabPageCollsion.TabIndex = 3;
            this.tabPageCollsion.Text = "衝突";
            this.tabPageCollsion.UseVisualStyleBackColor = true;
            // 
            // treeViewCollision
            // 
            this.treeViewCollision.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.treeViewCollision.ContextMenuStrip = this.cmContextMenu;
            this.treeViewCollision.Location = new System.Drawing.Point(3, 3);
            this.treeViewCollision.Margin = new System.Windows.Forms.Padding(0);
            this.treeViewCollision.Name = "treeViewCollision";
            this.treeViewCollision.Size = new System.Drawing.Size(159, 331);
            this.treeViewCollision.TabIndex = 4;
            this.treeViewCollision.Enter += new System.EventHandler(this.treeViewCollision_Enter);
            this.treeViewCollision.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeViewCollision_AfterSelect);
            this.treeViewCollision.MouseEnter += new System.EventHandler(this.treeViewCollision_MouseEnter);
            this.treeViewCollision.Leave += new System.EventHandler(this.treeViewCollision_Leave);
            // 
            // tabPageTag
            // 
            this.tabPageTag.Controls.Add(this.treeViewTag);
            this.tabPageTag.Location = new System.Drawing.Point(4, 58);
            this.tabPageTag.Name = "tabPageTag";
            this.tabPageTag.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageTag.Size = new System.Drawing.Size(165, 337);
            this.tabPageTag.TabIndex = 4;
            this.tabPageTag.Text = "タグ";
            this.tabPageTag.UseVisualStyleBackColor = true;
            // 
            // treeViewTag
            // 
            this.treeViewTag.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.treeViewTag.ContextMenuStrip = this.cmContextMenu;
            this.treeViewTag.Location = new System.Drawing.Point(3, 3);
            this.treeViewTag.Margin = new System.Windows.Forms.Padding(0);
            this.treeViewTag.Name = "treeViewTag";
            this.treeViewTag.Size = new System.Drawing.Size(159, 331);
            this.treeViewTag.TabIndex = 5;
            this.treeViewTag.Enter += new System.EventHandler(this.treeViewTag_Enter);
            this.treeViewTag.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeViewTag_AfterSelect);
            this.treeViewTag.MouseEnter += new System.EventHandler(this.treeViewTag_MouseEnter);
            this.treeViewTag.Leave += new System.EventHandler(this.treeViewTag_Leave);
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.treeViewSaveFile);
            this.tabPage1.Location = new System.Drawing.Point(4, 58);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(165, 337);
            this.tabPage1.TabIndex = 5;
            this.tabPage1.Text = "セーブファイル";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // treeViewSaveFile
            // 
            this.treeViewSaveFile.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.treeViewSaveFile.ContextMenuStrip = this.cmContextMenu;
            this.treeViewSaveFile.Location = new System.Drawing.Point(3, 3);
            this.treeViewSaveFile.Margin = new System.Windows.Forms.Padding(0);
            this.treeViewSaveFile.Name = "treeViewSaveFile";
            this.treeViewSaveFile.Size = new System.Drawing.Size(159, 331);
            this.treeViewSaveFile.TabIndex = 6;
            this.treeViewSaveFile.Enter += new System.EventHandler(this.treeViewSaveFile_Enter);
            this.treeViewSaveFile.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeViewSaveFile_AfterSelect);
            this.treeViewSaveFile.MouseEnter += new System.EventHandler(this.treeViewSaveFile_MouseEnter);
            this.treeViewSaveFile.Leave += new System.EventHandler(this.treeViewSaveFile_Leave);
            // 
            // splitContainerH
            // 
            this.splitContainerH.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainerH.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitContainerH.Location = new System.Drawing.Point(0, 0);
            this.splitContainerH.Margin = new System.Windows.Forms.Padding(0);
            this.splitContainerH.Name = "splitContainerH";
            this.splitContainerH.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainerH.Panel1
            // 
            this.splitContainerH.Panel1.Controls.Add(this.splitContainerWithView);
            // 
            // splitContainerH.Panel2
            // 
            this.splitContainerH.Panel2.Controls.Add(this.tabMainView);
            this.splitContainerH.Size = new System.Drawing.Size(623, 399);
            this.splitContainerH.SplitterDistance = 161;
            this.splitContainerH.SplitterWidth = 8;
            this.splitContainerH.TabIndex = 6;
            // 
            // splitContainerWithView
            // 
            this.splitContainerWithView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainerWithView.Location = new System.Drawing.Point(2, 2);
            this.splitContainerWithView.Name = "splitContainerWithView";
            // 
            // splitContainerWithView.Panel1
            // 
            this.splitContainerWithView.Panel1.Controls.Add(this.listBoxMainListBox);
            // 
            // splitContainerWithView.Panel2
            // 
            this.splitContainerWithView.Panel2.BackColor = System.Drawing.SystemColors.ButtonShadow;
            this.splitContainerWithView.Panel2.Margin = new System.Windows.Forms.Padding(4);
            this.splitContainerWithView.Size = new System.Drawing.Size(612, 152);
            this.splitContainerWithView.SplitterDistance = 289;
            this.splitContainerWithView.SplitterWidth = 8;
            this.splitContainerWithView.TabIndex = 1;
            // 
            // listBoxMainListBox
            // 
            this.listBoxMainListBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.listBoxMainListBox.ContextMenuStrip = this.ListBoxContextMenuStrip;
            this.listBoxMainListBox.FormattingEnabled = true;
            this.listBoxMainListBox.Location = new System.Drawing.Point(0, 3);
            this.listBoxMainListBox.Name = "listBoxMainListBox";
            this.listBoxMainListBox.Size = new System.Drawing.Size(286, 147);
            this.listBoxMainListBox.TabIndex = 0;
            this.listBoxMainListBox.SelectedIndexChanged += new System.EventHandler(this.listBoxMainListBox_SelectedIndexChanged);
            this.listBoxMainListBox.MouseEnter += new System.EventHandler(this.listBoxMainListBox_MouseEnter);
            // 
            // ListBoxContextMenuStrip
            // 
            this.ListBoxContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.NewTabToolStripMenuItem,
            this.ファイルを展開するToolStripMenuItem});
            this.ListBoxContextMenuStrip.Name = "contextMenuStrip1";
            this.ListBoxContextMenuStrip.Size = new System.Drawing.Size(183, 70);
            // 
            // NewTabToolStripMenuItem
            // 
            this.NewTabToolStripMenuItem.Name = "NewTabToolStripMenuItem";
            this.NewTabToolStripMenuItem.Size = new System.Drawing.Size(182, 22);
            this.NewTabToolStripMenuItem.Text = "新しいタブを開く";
            this.NewTabToolStripMenuItem.Click += new System.EventHandler(this.NewTabPageToolStripMenuItem_Click);
            // 
            // ファイルを展開するToolStripMenuItem
            // 
            this.ファイルを展開するToolStripMenuItem.Name = "ファイルを展開するToolStripMenuItem";
            this.ファイルを展開するToolStripMenuItem.Size = new System.Drawing.Size(182, 22);
            this.ファイルを展開するToolStripMenuItem.Text = "ファイルを展開する";
            this.ファイルを展開するToolStripMenuItem.Click += new System.EventHandler(this.TahDecryptToolStripMenuItem_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(817, 457);
            this.Controls.Add(this.splitContainerV);
            this.Controls.Add(this.StatusStrip);
            this.Controls.Add(this.MainMenu);
            this.MainMenuStrip = this.MainMenu;
            this.Name = "MainForm";
            this.Text = "TDCGExplorer";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.MainMenu.ResumeLayout(false);
            this.MainMenu.PerformLayout();
            this.cmContextMenu.ResumeLayout(false);
            this.StatusStrip.ResumeLayout(false);
            this.StatusStrip.PerformLayout();
            this.tabContextMenuStrip.ResumeLayout(false);
            this.splitContainerV.Panel1.ResumeLayout(false);
            this.splitContainerV.Panel2.ResumeLayout(false);
            this.splitContainerV.ResumeLayout(false);
            this.tabControlTreeContainor.ResumeLayout(false);
            this.tabPageArcs.ResumeLayout(false);
            this.tabPageZips.ResumeLayout(false);
            this.tabPageIntalled.ResumeLayout(false);
            this.tabPageCollsion.ResumeLayout(false);
            this.tabPageTag.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.splitContainerH.Panel1.ResumeLayout(false);
            this.splitContainerH.Panel2.ResumeLayout(false);
            this.splitContainerH.ResumeLayout(false);
            this.splitContainerWithView.Panel1.ResumeLayout(false);
            this.splitContainerWithView.ResumeLayout(false);
            this.ListBoxContextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip MainMenu;
        private System.Windows.Forms.ToolStripMenuItem tDCGExplorerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.TreeView treeViewArcs;
        private System.Windows.Forms.StatusStrip StatusStrip;
        private System.Windows.Forms.ToolStripMenuItem databaseToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem createFromArcsToolStripMenuItem;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel;
        private System.Windows.Forms.Timer MainTimer;
        private System.Windows.Forms.ContextMenuStrip cmContextMenu;
        private System.Windows.Forms.ToolStripMenuItem expandAllToolStripMenuItem;
        private System.Windows.Forms.TabControl tabMainView;
        private System.Windows.Forms.ContextMenuStrip tabContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
        private System.Windows.Forms.SplitContainer splitContainerV;
        private System.Windows.Forms.SplitContainer splitContainerH;
        private System.Windows.Forms.ListBox listBoxMainListBox;
        private System.Windows.Forms.ToolStripMenuItem extractZipToolStripMenuItem;
        private System.Windows.Forms.SplitContainer splitContainerWithView;
        private System.Windows.Forms.ToolStripMenuItem tSOViewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem resetTSOViewerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem switchToMortionEnabledToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem extractZipFileToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem editSystemDatabaseToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem EditAnnotationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem FindDialogToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip ListBoxContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem NewTabToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem EditAnnotationToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem WindowMenuToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ExpandTreeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem NewTabToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem CloseTabToolStripMenuItem;
        private System.Windows.Forms.TabControl tabControlTreeContainor;
        private System.Windows.Forms.TabPage tabPageArcs;
        private System.Windows.Forms.TabPage tabPageZips;
        private System.Windows.Forms.TabPage tabPageIntalled;
        private System.Windows.Forms.TabPage tabPageCollsion;
        private System.Windows.Forms.TreeView treeViewZips;
        private System.Windows.Forms.TreeView treeViewInstalled;
        private System.Windows.Forms.TreeView treeViewCollision;
        private System.Windows.Forms.ToolStripMenuItem 未インストールの前提アーカイブを展開ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 前提アーカイブファイルを展開ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem downloadLatestArcsnameszipToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 関連アーカイブを調べるToolStripMenuItem;
        private System.Windows.Forms.TabPage tabPageTag;
        private System.Windows.Forms.TreeView treeViewTag;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TreeView treeViewSaveFile;
        private System.Windows.Forms.ToolStripMenuItem ファイルを展開するToolStripMenuItem;

    }
}

