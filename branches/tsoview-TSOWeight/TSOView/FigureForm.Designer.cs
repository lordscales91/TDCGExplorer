namespace TSOView
{
    partial class FigureForm
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
            this.btnUp = new System.Windows.Forms.Button();
            this.btnDown = new System.Windows.Forms.Button();
            this.lvTSOFiles = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tpSubScripts = new System.Windows.Forms.TabPage();
            this.gvShaderParams = new System.Windows.Forms.DataGridView();
            this.lvSubScripts = new System.Windows.Forms.ListView();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
            this.btnDump = new System.Windows.Forms.Button();
            this.tpFramesAndNodes = new System.Windows.Forms.TabPage();
            this.lvNodes = new System.Windows.Forms.ListView();
            this.columnHeader5 = new System.Windows.Forms.ColumnHeader();
            this.lvFrames = new System.Windows.Forms.ListView();
            this.columnHeader4 = new System.Windows.Forms.ColumnHeader();
            this.tpSliders = new System.Windows.Forms.TabPage();
            this.lbSlideEye = new System.Windows.Forms.Label();
            this.tbSlideEye = new System.Windows.Forms.TrackBar();
            this.lbSlideTall = new System.Windows.Forms.Label();
            this.tbSlideTall = new System.Windows.Forms.TrackBar();
            this.lbSlideBust = new System.Windows.Forms.Label();
            this.lbSlideWaist = new System.Windows.Forms.Label();
            this.lbSlideArm = new System.Windows.Forms.Label();
            this.lbSlideLeg = new System.Windows.Forms.Label();
            this.tbSlideBust = new System.Windows.Forms.TrackBar();
            this.tbSlideWaist = new System.Windows.Forms.TrackBar();
            this.tbSlideArm = new System.Windows.Forms.TrackBar();
            this.tbSlideLeg = new System.Windows.Forms.TrackBar();
            this.tabControl1.SuspendLayout();
            this.tpSubScripts.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gvShaderParams)).BeginInit();
            this.tpFramesAndNodes.SuspendLayout();
            this.tpSliders.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbSlideEye)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbSlideTall)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbSlideBust)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbSlideWaist)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbSlideArm)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbSlideLeg)).BeginInit();
            this.SuspendLayout();
            // 
            // btnUp
            // 
            this.btnUp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnUp.Location = new System.Drawing.Point(86, 518);
            this.btnUp.Name = "btnUp";
            this.btnUp.Size = new System.Drawing.Size(50, 23);
            this.btnUp.TabIndex = 1;
            this.btnUp.Text = "&Up";
            this.btnUp.UseVisualStyleBackColor = true;
            this.btnUp.Click += new System.EventHandler(this.btnUp_Click);
            // 
            // btnDown
            // 
            this.btnDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnDown.Location = new System.Drawing.Point(142, 518);
            this.btnDown.Name = "btnDown";
            this.btnDown.Size = new System.Drawing.Size(50, 23);
            this.btnDown.TabIndex = 2;
            this.btnDown.Text = "&Down";
            this.btnDown.UseVisualStyleBackColor = true;
            this.btnDown.Click += new System.EventHandler(this.btnDown_Click);
            // 
            // lvTSOFiles
            // 
            this.lvTSOFiles.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.lvTSOFiles.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
                this.columnHeader1});
            this.lvTSOFiles.FullRowSelect = true;
            this.lvTSOFiles.GridLines = true;
            this.lvTSOFiles.HideSelection = false;
            this.lvTSOFiles.Location = new System.Drawing.Point(12, 40);
            this.lvTSOFiles.MultiSelect = false;
            this.lvTSOFiles.Name = "lvTSOFiles";
            this.lvTSOFiles.Size = new System.Drawing.Size(180, 472);
            this.lvTSOFiles.TabIndex = 3;
            this.lvTSOFiles.UseCompatibleStateImageBehavior = false;
            this.lvTSOFiles.View = System.Windows.Forms.View.Details;
            this.lvTSOFiles.SelectedIndexChanged += new System.EventHandler(this.lvTSOFiles_SelectedIndexChanged);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Name";
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tpFramesAndNodes);
            this.tabControl1.Controls.Add(this.tpSubScripts);
            this.tabControl1.Controls.Add(this.tpSliders);
            this.tabControl1.Location = new System.Drawing.Point(198, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(574, 539);
            this.tabControl1.TabIndex = 20;
            // 
            // tpSubScripts
            // 
            this.tpSubScripts.Controls.Add(this.gvShaderParams);
            this.tpSubScripts.Controls.Add(this.lvSubScripts);
            this.tpSubScripts.Controls.Add(this.btnDump);
            this.tpSubScripts.Location = new System.Drawing.Point(4, 22);
            this.tpSubScripts.Name = "tpSubScripts";
            this.tpSubScripts.Padding = new System.Windows.Forms.Padding(3);
            this.tpSubScripts.Size = new System.Drawing.Size(566, 513);
            this.tpSubScripts.TabIndex = 1;
            this.tpSubScripts.Text = "SubScripts";
            this.tpSubScripts.UseVisualStyleBackColor = true;
            // 
            // gvShaderParams
            // 
            this.gvShaderParams.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.gvShaderParams.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gvShaderParams.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.gvShaderParams.Location = new System.Drawing.Point(192, 6);
            this.gvShaderParams.Name = "gvShaderParams";
            this.gvShaderParams.RowTemplate.Height = 21;
            this.gvShaderParams.Size = new System.Drawing.Size(368, 472);
            this.gvShaderParams.TabIndex = 8;
            // 
            // lvSubScripts
            // 
            this.lvSubScripts.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
                this.columnHeader2,
                this.columnHeader3});
            this.lvSubScripts.FullRowSelect = true;
            this.lvSubScripts.GridLines = true;
            this.lvSubScripts.HideSelection = false;
            this.lvSubScripts.Location = new System.Drawing.Point(6, 6);
            this.lvSubScripts.MultiSelect = false;
            this.lvSubScripts.Name = "lvSubScripts";
            this.lvSubScripts.Size = new System.Drawing.Size(180, 472);
            this.lvSubScripts.TabIndex = 7;
            this.lvSubScripts.UseCompatibleStateImageBehavior = false;
            this.lvSubScripts.View = System.Windows.Forms.View.Details;
            this.lvSubScripts.SelectedIndexChanged += new System.EventHandler(this.lvSubScripts_SelectedIndexChanged);
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Name";
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "File";
            // 
            // btnDump
            // 
            this.btnDump.Location = new System.Drawing.Point(485, 484);
            this.btnDump.Name = "btnDump";
            this.btnDump.Size = new System.Drawing.Size(75, 23);
            this.btnDump.TabIndex = 6;
            this.btnDump.Text = "&Dump";
            this.btnDump.UseVisualStyleBackColor = true;
            this.btnDump.Click += new System.EventHandler(this.btnDump_Click);
            // 
            // tpFramesAndNodes
            // 
            this.tpFramesAndNodes.Controls.Add(this.lvNodes);
            this.tpFramesAndNodes.Controls.Add(this.lvFrames);
            this.tpFramesAndNodes.Location = new System.Drawing.Point(4, 22);
            this.tpFramesAndNodes.Name = "tpFramesAndNodes";
            this.tpFramesAndNodes.Padding = new System.Windows.Forms.Padding(3);
            this.tpFramesAndNodes.Size = new System.Drawing.Size(566, 513);
            this.tpFramesAndNodes.TabIndex = 2;
            this.tpFramesAndNodes.Text = "Frames and Nodes";
            this.tpFramesAndNodes.UseVisualStyleBackColor = true;
            // 
            // lvNodes
            // 
            this.lvNodes.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
                this.columnHeader5});
            this.lvNodes.FullRowSelect = true;
            this.lvNodes.GridLines = true;
            this.lvNodes.HideSelection = false;
            this.lvNodes.Location = new System.Drawing.Point(287, 6);
            this.lvNodes.MultiSelect = false;
            this.lvNodes.Name = "lvNodes";
            this.lvNodes.Size = new System.Drawing.Size(274, 472);
            this.lvNodes.TabIndex = 21;
            this.lvNodes.UseCompatibleStateImageBehavior = false;
            this.lvNodes.View = System.Windows.Forms.View.Details;
            this.lvNodes.SelectedIndexChanged += new System.EventHandler(this.lvNodes_SelectedIndexChanged);
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "Name";
            // 
            // lvFrames
            // 
            this.lvFrames.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
                this.columnHeader4});
            this.lvFrames.FullRowSelect = true;
            this.lvFrames.GridLines = true;
            this.lvFrames.HideSelection = false;
            this.lvFrames.Location = new System.Drawing.Point(6, 6);
            this.lvFrames.MultiSelect = false;
            this.lvFrames.Name = "lvFrames";
            this.lvFrames.Size = new System.Drawing.Size(274, 472);
            this.lvFrames.TabIndex = 20;
            this.lvFrames.UseCompatibleStateImageBehavior = false;
            this.lvFrames.View = System.Windows.Forms.View.Details;
            this.lvFrames.SelectedIndexChanged += new System.EventHandler(this.lvFrames_SelectedIndexChanged);
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Name";
            // 
            // tpSliders
            // 
            this.tpSliders.Controls.Add(this.lbSlideEye);
            this.tpSliders.Controls.Add(this.tbSlideEye);
            this.tpSliders.Controls.Add(this.lbSlideTall);
            this.tpSliders.Controls.Add(this.tbSlideTall);
            this.tpSliders.Controls.Add(this.lbSlideBust);
            this.tpSliders.Controls.Add(this.lbSlideWaist);
            this.tpSliders.Controls.Add(this.lbSlideArm);
            this.tpSliders.Controls.Add(this.lbSlideLeg);
            this.tpSliders.Controls.Add(this.tbSlideBust);
            this.tpSliders.Controls.Add(this.tbSlideWaist);
            this.tpSliders.Controls.Add(this.tbSlideArm);
            this.tpSliders.Controls.Add(this.tbSlideLeg);
            this.tpSliders.Location = new System.Drawing.Point(4, 22);
            this.tpSliders.Name = "tpSliders";
            this.tpSliders.Padding = new System.Windows.Forms.Padding(3);
            this.tpSliders.Size = new System.Drawing.Size(566, 513);
            this.tpSliders.TabIndex = 0;
            this.tpSliders.Text = "Sliders";
            this.tpSliders.UseVisualStyleBackColor = true;
            // 
            // lbSlideEye
            // 
            this.lbSlideEye.AutoSize = true;
            this.lbSlideEye.Location = new System.Drawing.Point(6, 321);
            this.lbSlideEye.Name = "lbSlideEye";
            this.lbSlideEye.Size = new System.Drawing.Size(24, 12);
            this.lbSlideEye.TabIndex = 23;
            this.lbSlideEye.Text = "Eye";
            // 
            // tbSlideEye
            // 
            this.tbSlideEye.Location = new System.Drawing.Point(6, 336);
            this.tbSlideEye.Maximum = 20;
            this.tbSlideEye.Name = "tbSlideEye";
            this.tbSlideEye.Size = new System.Drawing.Size(168, 45);
            this.tbSlideEye.TabIndex = 18;
            this.tbSlideEye.ValueChanged += new System.EventHandler(this.tbSlideEye_ValueChanged);
            // 
            // lbSlideTall
            // 
            this.lbSlideTall.AutoSize = true;
            this.lbSlideTall.Location = new System.Drawing.Point(6, 258);
            this.lbSlideTall.Name = "lbSlideTall";
            this.lbSlideTall.Size = new System.Drawing.Size(24, 12);
            this.lbSlideTall.TabIndex = 29;
            this.lbSlideTall.Text = "Tall";
            // 
            // tbSlideTall
            // 
            this.tbSlideTall.Location = new System.Drawing.Point(6, 273);
            this.tbSlideTall.Maximum = 20;
            this.tbSlideTall.Name = "tbSlideTall";
            this.tbSlideTall.Size = new System.Drawing.Size(168, 45);
            this.tbSlideTall.TabIndex = 28;
            this.tbSlideTall.ValueChanged += new System.EventHandler(this.tbSlideTall_ValueChanged);
            // 
            // lbSlideBust
            // 
            this.lbSlideBust.AutoSize = true;
            this.lbSlideBust.Location = new System.Drawing.Point(6, 195);
            this.lbSlideBust.Name = "lbSlideBust";
            this.lbSlideBust.Size = new System.Drawing.Size(29, 12);
            this.lbSlideBust.TabIndex = 27;
            this.lbSlideBust.Text = "Bust";
            // 
            // lbSlideWaist
            // 
            this.lbSlideWaist.AutoSize = true;
            this.lbSlideWaist.Location = new System.Drawing.Point(6, 132);
            this.lbSlideWaist.Name = "lbSlideWaist";
            this.lbSlideWaist.Size = new System.Drawing.Size(33, 12);
            this.lbSlideWaist.TabIndex = 26;
            this.lbSlideWaist.Text = "Waist";
            // 
            // lbSlideArm
            // 
            this.lbSlideArm.AutoSize = true;
            this.lbSlideArm.Location = new System.Drawing.Point(6, 6);
            this.lbSlideArm.Name = "lbSlideArm";
            this.lbSlideArm.Size = new System.Drawing.Size(26, 12);
            this.lbSlideArm.TabIndex = 25;
            this.lbSlideArm.Text = "Arm";
            // 
            // lbSlideLeg
            // 
            this.lbSlideLeg.AutoSize = true;
            this.lbSlideLeg.Location = new System.Drawing.Point(6, 69);
            this.lbSlideLeg.Name = "lbSlideLeg";
            this.lbSlideLeg.Size = new System.Drawing.Size(23, 12);
            this.lbSlideLeg.TabIndex = 24;
            this.lbSlideLeg.Text = "Leg";
            // 
            // tbSlideBust
            // 
            this.tbSlideBust.Location = new System.Drawing.Point(6, 210);
            this.tbSlideBust.Maximum = 20;
            this.tbSlideBust.Name = "tbSlideBust";
            this.tbSlideBust.Size = new System.Drawing.Size(168, 45);
            this.tbSlideBust.TabIndex = 22;
            this.tbSlideBust.ValueChanged += new System.EventHandler(this.tbSlideBust_ValueChanged);
            // 
            // tbSlideWaist
            // 
            this.tbSlideWaist.Location = new System.Drawing.Point(6, 147);
            this.tbSlideWaist.Maximum = 20;
            this.tbSlideWaist.Name = "tbSlideWaist";
            this.tbSlideWaist.Size = new System.Drawing.Size(168, 45);
            this.tbSlideWaist.TabIndex = 21;
            this.tbSlideWaist.ValueChanged += new System.EventHandler(this.tbSlideWaist_ValueChanged);
            // 
            // tbSlideArm
            // 
            this.tbSlideArm.Location = new System.Drawing.Point(6, 21);
            this.tbSlideArm.Maximum = 20;
            this.tbSlideArm.Name = "tbSlideArm";
            this.tbSlideArm.Size = new System.Drawing.Size(168, 45);
            this.tbSlideArm.TabIndex = 20;
            this.tbSlideArm.ValueChanged += new System.EventHandler(this.tbSlideArm_ValueChanged);
            // 
            // tbSlideLeg
            // 
            this.tbSlideLeg.Location = new System.Drawing.Point(6, 84);
            this.tbSlideLeg.Maximum = 20;
            this.tbSlideLeg.Name = "tbSlideLeg";
            this.tbSlideLeg.Size = new System.Drawing.Size(168, 45);
            this.tbSlideLeg.TabIndex = 19;
            this.tbSlideLeg.ValueChanged += new System.EventHandler(this.tbSlideLeg_ValueChanged);
            // 
            // FigureForm
            // 
            this.ClientSize = new System.Drawing.Size(784, 563);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.lvTSOFiles);
            this.Controls.Add(this.btnDown);
            this.Controls.Add(this.btnUp);
            this.Name = "FigureForm";
            this.Text = "TSOGrid";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FigureForm_FormClosing);
            this.tabControl1.ResumeLayout(false);
            this.tpSubScripts.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gvShaderParams)).EndInit();
            this.tpFramesAndNodes.ResumeLayout(false);
            this.tpSliders.ResumeLayout(false);
            this.tpSliders.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbSlideEye)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbSlideTall)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbSlideBust)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbSlideWaist)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbSlideArm)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbSlideLeg)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnUp;
        private System.Windows.Forms.Button btnDown;
        private System.Windows.Forms.ListView lvTSOFiles;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tpSliders;
        private System.Windows.Forms.Label lbSlideEye;
        private System.Windows.Forms.TrackBar tbSlideEye;
        private System.Windows.Forms.Label lbSlideTall;
        private System.Windows.Forms.TrackBar tbSlideTall;
        private System.Windows.Forms.Label lbSlideBust;
        private System.Windows.Forms.Label lbSlideWaist;
        private System.Windows.Forms.Label lbSlideArm;
        private System.Windows.Forms.Label lbSlideLeg;
        private System.Windows.Forms.TrackBar tbSlideBust;
        private System.Windows.Forms.TrackBar tbSlideWaist;
        private System.Windows.Forms.TrackBar tbSlideArm;
        private System.Windows.Forms.TrackBar tbSlideLeg;
        private System.Windows.Forms.DataGridView gvShaderParams;
        private System.Windows.Forms.ListView lvSubScripts;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.Button btnDump;
        private System.Windows.Forms.TabPage tpFramesAndNodes;
        private System.Windows.Forms.ListView lvNodes;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ListView lvFrames;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.TabPage tpSubScripts;

    }
}
