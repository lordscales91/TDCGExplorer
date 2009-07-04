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
            this.検索ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.databaseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.createFromArcsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editSystemDatabaseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dCGModsReferenceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.downloadLatestArcsnameszipToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lookupMODRelationshipToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tSOViewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.resetTSOViewerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.switchToMortionEnabledToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tvMainTree = new System.Windows.Forms.TreeView();
            this.cmContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.expandAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.extractZipToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.注訳を入力ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mODの依存関係を表示ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.StatusStrip = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.MainTimer = new System.Windows.Forms.Timer(this.components);
            this.tabMainView = new System.Windows.Forms.TabControl();
            this.tabContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.splitContainerV = new System.Windows.Forms.SplitContainer();
            this.splitContainerH = new System.Windows.Forms.SplitContainer();
            this.splitContainerWithView = new System.Windows.Forms.SplitContainer();
            this.listBoxMainListBox = new System.Windows.Forms.ListBox();
            this.MainMenu.SuspendLayout();
            this.cmContextMenu.SuspendLayout();
            this.StatusStrip.SuspendLayout();
            this.tabContextMenuStrip.SuspendLayout();
            this.splitContainerV.Panel1.SuspendLayout();
            this.splitContainerV.Panel2.SuspendLayout();
            this.splitContainerV.SuspendLayout();
            this.splitContainerH.Panel1.SuspendLayout();
            this.splitContainerH.Panel2.SuspendLayout();
            this.splitContainerH.SuspendLayout();
            this.splitContainerWithView.Panel1.SuspendLayout();
            this.splitContainerWithView.SuspendLayout();
            this.SuspendLayout();
            // 
            // MainMenu
            // 
            this.MainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tDCGExplorerToolStripMenuItem,
            this.databaseToolStripMenuItem,
            this.dCGModsReferenceToolStripMenuItem,
            this.tSOViewToolStripMenuItem});
            this.MainMenu.Location = new System.Drawing.Point(0, 0);
            this.MainMenu.Name = "MainMenu";
            this.MainMenu.Size = new System.Drawing.Size(608, 24);
            this.MainMenu.TabIndex = 1;
            this.MainMenu.Text = "MainMenu";
            // 
            // tDCGExplorerToolStripMenuItem
            // 
            this.tDCGExplorerToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.extractZipFileToolStripMenuItem1,
            this.検索ToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.tDCGExplorerToolStripMenuItem.Name = "tDCGExplorerToolStripMenuItem";
            this.tDCGExplorerToolStripMenuItem.Size = new System.Drawing.Size(67, 20);
            this.tDCGExplorerToolStripMenuItem.Text = "ファイル";
            // 
            // extractZipFileToolStripMenuItem1
            // 
            this.extractZipFileToolStripMenuItem1.Name = "extractZipFileToolStripMenuItem1";
            this.extractZipFileToolStripMenuItem1.Size = new System.Drawing.Size(218, 22);
            this.extractZipFileToolStripMenuItem1.Text = "アーカイブファイルの展開";
            this.extractZipFileToolStripMenuItem1.Click += new System.EventHandler(this.extractZipFileToolStripMenuItem1_Click_1);
            // 
            // 検索ToolStripMenuItem
            // 
            this.検索ToolStripMenuItem.Name = "検索ToolStripMenuItem";
            this.検索ToolStripMenuItem.Size = new System.Drawing.Size(218, 22);
            this.検索ToolStripMenuItem.Text = "検索...";
            this.検索ToolStripMenuItem.Click += new System.EventHandler(this.検索ToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(218, 22);
            this.exitToolStripMenuItem.Text = "終了";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // databaseToolStripMenuItem
            // 
            this.databaseToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.createFromArcsToolStripMenuItem,
            this.editSystemDatabaseToolStripMenuItem});
            this.databaseToolStripMenuItem.Name = "databaseToolStripMenuItem";
            this.databaseToolStripMenuItem.Size = new System.Drawing.Size(91, 20);
            this.databaseToolStripMenuItem.Text = "データベース";
            // 
            // createFromArcsToolStripMenuItem
            // 
            this.createFromArcsToolStripMenuItem.Name = "createFromArcsToolStripMenuItem";
            this.createFromArcsToolStripMenuItem.Size = new System.Drawing.Size(218, 22);
            this.createFromArcsToolStripMenuItem.Text = "データベースの構築・更新";
            this.createFromArcsToolStripMenuItem.Click += new System.EventHandler(this.createFromArcsToolStripMenuItem_Click);
            // 
            // editSystemDatabaseToolStripMenuItem
            // 
            this.editSystemDatabaseToolStripMenuItem.Name = "editSystemDatabaseToolStripMenuItem";
            this.editSystemDatabaseToolStripMenuItem.Size = new System.Drawing.Size(218, 22);
            this.editSystemDatabaseToolStripMenuItem.Text = "初期設定";
            this.editSystemDatabaseToolStripMenuItem.Click += new System.EventHandler(this.editSystemDatabaseToolStripMenuItem_Click_1);
            // 
            // dCGModsReferenceToolStripMenuItem
            // 
            this.dCGModsReferenceToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.downloadLatestArcsnameszipToolStripMenuItem,
            this.lookupMODRelationshipToolStripMenuItem});
            this.dCGModsReferenceToolStripMenuItem.Name = "dCGModsReferenceToolStripMenuItem";
            this.dCGModsReferenceToolStripMenuItem.Size = new System.Drawing.Size(134, 20);
            this.dCGModsReferenceToolStripMenuItem.Text = "3DCG mods reference";
            // 
            // downloadLatestArcsnameszipToolStripMenuItem
            // 
            this.downloadLatestArcsnameszipToolStripMenuItem.Name = "downloadLatestArcsnameszipToolStripMenuItem";
            this.downloadLatestArcsnameszipToolStripMenuItem.Size = new System.Drawing.Size(220, 22);
            this.downloadLatestArcsnameszipToolStripMenuItem.Text = "最新のarcsnames.zipを取得";
            this.downloadLatestArcsnameszipToolStripMenuItem.Click += new System.EventHandler(this.downloadLatestArcsnameszipToolStripMenuItem_Click);
            // 
            // lookupMODRelationshipToolStripMenuItem
            // 
            this.lookupMODRelationshipToolStripMenuItem.Name = "lookupMODRelationshipToolStripMenuItem";
            this.lookupMODRelationshipToolStripMenuItem.Size = new System.Drawing.Size(220, 22);
            this.lookupMODRelationshipToolStripMenuItem.Text = "MODの依存関係を表示";
            this.lookupMODRelationshipToolStripMenuItem.Click += new System.EventHandler(this.lookupMODRelationshipToolStripMenuItem_Click);
            // 
            // tSOViewToolStripMenuItem
            // 
            this.tSOViewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.resetTSOViewerToolStripMenuItem,
            this.switchToMortionEnabledToolStripMenuItem});
            this.tSOViewToolStripMenuItem.Name = "tSOViewToolStripMenuItem";
            this.tSOViewToolStripMenuItem.Size = new System.Drawing.Size(66, 20);
            this.tSOViewToolStripMenuItem.Text = "TSOView";
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
            // tvMainTree
            // 
            this.tvMainTree.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tvMainTree.ContextMenuStrip = this.cmContextMenu;
            this.tvMainTree.Location = new System.Drawing.Point(0, 0);
            this.tvMainTree.Margin = new System.Windows.Forms.Padding(0);
            this.tvMainTree.Name = "tvMainTree";
            this.tvMainTree.Size = new System.Drawing.Size(133, 406);
            this.tvMainTree.TabIndex = 3;
            this.tvMainTree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvMainTree_AfterSelect);
            // 
            // cmContextMenu
            // 
            this.cmContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.expandAllToolStripMenuItem,
            this.extractZipToolStripMenuItem,
            this.注訳を入力ToolStripMenuItem,
            this.mODの依存関係を表示ToolStripMenuItem});
            this.cmContextMenu.Name = "cmContextMenu";
            this.cmContextMenu.Size = new System.Drawing.Size(219, 92);
            // 
            // expandAllToolStripMenuItem
            // 
            this.expandAllToolStripMenuItem.Name = "expandAllToolStripMenuItem";
            this.expandAllToolStripMenuItem.Size = new System.Drawing.Size(218, 22);
            this.expandAllToolStripMenuItem.Text = "ツリーを展開";
            this.expandAllToolStripMenuItem.Click += new System.EventHandler(this.expandAllToolStripMenuItem1_Click);
            // 
            // extractZipToolStripMenuItem
            // 
            this.extractZipToolStripMenuItem.Name = "extractZipToolStripMenuItem";
            this.extractZipToolStripMenuItem.Size = new System.Drawing.Size(218, 22);
            this.extractZipToolStripMenuItem.Text = "アーカイブファイルを展開";
            this.extractZipToolStripMenuItem.Click += new System.EventHandler(this.extractZipToolStripMenuItem_Click);
            // 
            // 注訳を入力ToolStripMenuItem
            // 
            this.注訳を入力ToolStripMenuItem.Name = "注訳を入力ToolStripMenuItem";
            this.注訳を入力ToolStripMenuItem.Size = new System.Drawing.Size(218, 22);
            this.注訳を入力ToolStripMenuItem.Text = "注訳を入力";
            this.注訳を入力ToolStripMenuItem.Click += new System.EventHandler(this.EditAnnotationToolStripMenuItem_Click);
            // 
            // mODの依存関係を表示ToolStripMenuItem
            // 
            this.mODの依存関係を表示ToolStripMenuItem.Name = "mODの依存関係を表示ToolStripMenuItem";
            this.mODの依存関係を表示ToolStripMenuItem.Size = new System.Drawing.Size(218, 22);
            this.mODの依存関係を表示ToolStripMenuItem.Text = "MODの依存関係を表示";
            this.mODの依存関係を表示ToolStripMenuItem.Click += new System.EventHandler(this.LookupModRefToolStripMenuItem_Click);
            // 
            // StatusStrip
            // 
            this.StatusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel});
            this.StatusStrip.Location = new System.Drawing.Point(0, 435);
            this.StatusStrip.Name = "StatusStrip";
            this.StatusStrip.Size = new System.Drawing.Size(608, 22);
            this.StatusStrip.TabIndex = 4;
            this.StatusStrip.Text = "StatusStrip";
            // 
            // toolStripStatusLabel
            // 
            this.toolStripStatusLabel.Name = "toolStripStatusLabel";
            this.toolStripStatusLabel.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never;
            this.toolStripStatusLabel.Size = new System.Drawing.Size(593, 17);
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
            this.tabMainView.Name = "tabMainView";
            this.tabMainView.SelectedIndex = 0;
            this.tabMainView.Size = new System.Drawing.Size(459, 236);
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
            this.splitContainerV.Panel1.Controls.Add(this.tvMainTree);
            // 
            // splitContainerV.Panel2
            // 
            this.splitContainerV.Panel2.Controls.Add(this.splitContainerH);
            this.splitContainerV.Size = new System.Drawing.Size(608, 407);
            this.splitContainerV.SplitterDistance = 135;
            this.splitContainerV.SplitterWidth = 8;
            this.splitContainerV.TabIndex = 6;
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
            this.splitContainerH.Size = new System.Drawing.Size(460, 399);
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
            this.splitContainerWithView.Size = new System.Drawing.Size(449, 152);
            this.splitContainerWithView.SplitterDistance = 213;
            this.splitContainerWithView.SplitterWidth = 8;
            this.splitContainerWithView.TabIndex = 1;
            // 
            // listBoxMainListBox
            // 
            this.listBoxMainListBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.listBoxMainListBox.FormattingEnabled = true;
            this.listBoxMainListBox.Location = new System.Drawing.Point(0, 3);
            this.listBoxMainListBox.Name = "listBoxMainListBox";
            this.listBoxMainListBox.Size = new System.Drawing.Size(210, 147);
            this.listBoxMainListBox.TabIndex = 0;
            this.listBoxMainListBox.DoubleClick += new System.EventHandler(this.listBoxMainListBox_DoubleClick);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(608, 457);
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
            this.splitContainerH.Panel1.ResumeLayout(false);
            this.splitContainerH.Panel2.ResumeLayout(false);
            this.splitContainerH.ResumeLayout(false);
            this.splitContainerWithView.Panel1.ResumeLayout(false);
            this.splitContainerWithView.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip MainMenu;
        private System.Windows.Forms.ToolStripMenuItem tDCGExplorerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.TreeView tvMainTree;
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
        private System.Windows.Forms.ToolStripMenuItem dCGModsReferenceToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem downloadLatestArcsnameszipToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem extractZipToolStripMenuItem;
        private System.Windows.Forms.SplitContainer splitContainerWithView;
        private System.Windows.Forms.ToolStripMenuItem tSOViewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem resetTSOViewerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem switchToMortionEnabledToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem extractZipFileToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem editSystemDatabaseToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem lookupMODRelationshipToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 注訳を入力ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mODの依存関係を表示ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 検索ToolStripMenuItem;

    }
}

