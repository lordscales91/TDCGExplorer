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

        private void InitializeComponent()
        {
            this.btnDump = new System.Windows.Forms.Button();
            this.btnUp = new System.Windows.Forms.Button();
            this.btnDown = new System.Windows.Forms.Button();
            this.lvTSOFiles = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.lvSubScripts = new System.Windows.Forms.ListView();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
            this.gvShaderParams = new System.Windows.Forms.DataGridView();
            this.tbSlideEye = new System.Windows.Forms.TrackBar();
            this.tbSlideLeg = new System.Windows.Forms.TrackBar();
            this.tbSlideArm = new System.Windows.Forms.TrackBar();
            this.tbSlideWaist = new System.Windows.Forms.TrackBar();
            this.tbSlideBust = new System.Windows.Forms.TrackBar();
            this.lbSlideEye = new System.Windows.Forms.Label();
            this.lbSlideLeg = new System.Windows.Forms.Label();
            this.lbSlideArm = new System.Windows.Forms.Label();
            this.lbSlideWaist = new System.Windows.Forms.Label();
            this.lbSlideBust = new System.Windows.Forms.Label();
            this.lbSlideTall = new System.Windows.Forms.Label();
            this.tbSlideTall = new System.Windows.Forms.TrackBar();
            ((System.ComponentModel.ISupportInitialize)(this.gvShaderParams)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbSlideEye)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbSlideLeg)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbSlideArm)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbSlideWaist)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbSlideBust)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbSlideTall)).BeginInit();
            this.SuspendLayout();
            // 
            // btnDump
            // 
            this.btnDump.Location = new System.Drawing.Point(523, 528);
            this.btnDump.Name = "btnDump";
            this.btnDump.Size = new System.Drawing.Size(75, 23);
            this.btnDump.TabIndex = 0;
            this.btnDump.Text = "&Dump";
            this.btnDump.UseVisualStyleBackColor = true;
            this.btnDump.Click += new System.EventHandler(this.btnDump_Click);
            // 
            // btnUp
            // 
            this.btnUp.Location = new System.Drawing.Point(198, 12);
            this.btnUp.Name = "btnUp";
            this.btnUp.Size = new System.Drawing.Size(50, 23);
            this.btnUp.TabIndex = 1;
            this.btnUp.Text = "&Up";
            this.btnUp.UseVisualStyleBackColor = true;
            this.btnUp.Click += new System.EventHandler(this.btnUp_Click);
            // 
            // btnDown
            // 
            this.btnDown.Location = new System.Drawing.Point(198, 41);
            this.btnDown.Name = "btnDown";
            this.btnDown.Size = new System.Drawing.Size(50, 23);
            this.btnDown.TabIndex = 2;
            this.btnDown.Text = "&Down";
            this.btnDown.UseVisualStyleBackColor = true;
            this.btnDown.Click += new System.EventHandler(this.btnDown_Click);
            // 
            // lvTSOFiles
            // 
            this.lvTSOFiles.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
                this.columnHeader1});
            this.lvTSOFiles.FullRowSelect = true;
            this.lvTSOFiles.GridLines = true;
            this.lvTSOFiles.HideSelection = false;
            this.lvTSOFiles.Location = new System.Drawing.Point(12, 12);
            this.lvTSOFiles.MultiSelect = false;
            this.lvTSOFiles.Name = "lvTSOFiles";
            this.lvTSOFiles.Size = new System.Drawing.Size(180, 200);
            this.lvTSOFiles.TabIndex = 3;
            this.lvTSOFiles.UseCompatibleStateImageBehavior = false;
            this.lvTSOFiles.View = System.Windows.Forms.View.Details;
            this.lvTSOFiles.SelectedIndexChanged += new System.EventHandler(this.lvTSOFiles_SelectedIndexChanged);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Name";
            // 
            // lvSubScripts
            // 
            this.lvSubScripts.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
                this.columnHeader2,
                this.columnHeader3});
            this.lvSubScripts.FullRowSelect = true;
            this.lvSubScripts.GridLines = true;
            this.lvSubScripts.HideSelection = false;
            this.lvSubScripts.Location = new System.Drawing.Point(12, 218);
            this.lvSubScripts.MultiSelect = false;
            this.lvSubScripts.Name = "lvSubScripts";
            this.lvSubScripts.Size = new System.Drawing.Size(180, 304);
            this.lvSubScripts.TabIndex = 4;
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
            // gvShaderParams
            // 
            this.gvShaderParams.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.gvShaderParams.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gvShaderParams.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.gvShaderParams.Location = new System.Drawing.Point(198, 218);
            this.gvShaderParams.Name = "gvShaderParams";
            this.gvShaderParams.RowTemplate.Height = 21;
            this.gvShaderParams.Size = new System.Drawing.Size(400, 304);
            this.gvShaderParams.TabIndex = 5;
            // 
            // tbSlideEye
            // 
            this.tbSlideEye.Location = new System.Drawing.Point(604, 473);
            this.tbSlideEye.Maximum = 20;
            this.tbSlideEye.Name = "tbSlideEye";
            this.tbSlideEye.Size = new System.Drawing.Size(168, 45);
            this.tbSlideEye.TabIndex = 6;
            this.tbSlideEye.ValueChanged += new System.EventHandler(this.tbSlideEye_ValueChanged);
            // 
            // tbSlideLeg
            // 
            this.tbSlideLeg.Location = new System.Drawing.Point(604, 269);
            this.tbSlideLeg.Maximum = 20;
            this.tbSlideLeg.Name = "tbSlideLeg";
            this.tbSlideLeg.Size = new System.Drawing.Size(168, 45);
            this.tbSlideLeg.TabIndex = 7;
            this.tbSlideLeg.ValueChanged += new System.EventHandler(this.tbSlideLeg_ValueChanged);
            // 
            // tbSlideArm
            // 
            this.tbSlideArm.Location = new System.Drawing.Point(604, 218);
            this.tbSlideArm.Maximum = 20;
            this.tbSlideArm.Name = "tbSlideArm";
            this.tbSlideArm.Size = new System.Drawing.Size(168, 45);
            this.tbSlideArm.TabIndex = 8;
            this.tbSlideArm.ValueChanged += new System.EventHandler(this.tbSlideArm_ValueChanged);
            // 
            // tbSlideWaist
            // 
            this.tbSlideWaist.Location = new System.Drawing.Point(604, 320);
            this.tbSlideWaist.Maximum = 20;
            this.tbSlideWaist.Name = "tbSlideWaist";
            this.tbSlideWaist.Size = new System.Drawing.Size(168, 45);
            this.tbSlideWaist.TabIndex = 9;
            this.tbSlideWaist.ValueChanged += new System.EventHandler(this.tbSlideWaist_ValueChanged);
            // 
            // tbSlideBust
            // 
            this.tbSlideBust.Location = new System.Drawing.Point(604, 371);
            this.tbSlideBust.Maximum = 20;
            this.tbSlideBust.Name = "tbSlideBust";
            this.tbSlideBust.Size = new System.Drawing.Size(168, 45);
            this.tbSlideBust.TabIndex = 10;
            this.tbSlideBust.ValueChanged += new System.EventHandler(this.tbSlideBust_ValueChanged);
            // 
            // lbSlideEye
            // 
            this.lbSlideEye.AutoSize = true;
            this.lbSlideEye.Location = new System.Drawing.Point(604, 458);
            this.lbSlideEye.Name = "lbSlideEye";
            this.lbSlideEye.Size = new System.Drawing.Size(24, 12);
            this.lbSlideEye.TabIndex = 11;
            this.lbSlideEye.Text = "Eye";
            // 
            // lbSlideLeg
            // 
            this.lbSlideLeg.AutoSize = true;
            this.lbSlideLeg.Location = new System.Drawing.Point(604, 254);
            this.lbSlideLeg.Name = "lbSlideLeg";
            this.lbSlideLeg.Size = new System.Drawing.Size(23, 12);
            this.lbSlideLeg.TabIndex = 12;
            this.lbSlideLeg.Text = "Leg";
            // 
            // lbSlideArm
            // 
            this.lbSlideArm.AutoSize = true;
            this.lbSlideArm.Location = new System.Drawing.Point(605, 206);
            this.lbSlideArm.Name = "lbSlideArm";
            this.lbSlideArm.Size = new System.Drawing.Size(26, 12);
            this.lbSlideArm.TabIndex = 13;
            this.lbSlideArm.Text = "Arm";
            // 
            // lbSlideWaist
            // 
            this.lbSlideWaist.AutoSize = true;
            this.lbSlideWaist.Location = new System.Drawing.Point(604, 305);
            this.lbSlideWaist.Name = "lbSlideWaist";
            this.lbSlideWaist.Size = new System.Drawing.Size(33, 12);
            this.lbSlideWaist.TabIndex = 14;
            this.lbSlideWaist.Text = "Waist";
            // 
            // lbSlideBust
            // 
            this.lbSlideBust.AutoSize = true;
            this.lbSlideBust.Location = new System.Drawing.Point(604, 356);
            this.lbSlideBust.Name = "lbSlideBust";
            this.lbSlideBust.Size = new System.Drawing.Size(29, 12);
            this.lbSlideBust.TabIndex = 15;
            this.lbSlideBust.Text = "Bust";
            // 
            // lbSlideTall
            // 
            this.lbSlideTall.AutoSize = true;
            this.lbSlideTall.Location = new System.Drawing.Point(604, 407);
            this.lbSlideTall.Name = "lbSlideTall";
            this.lbSlideTall.Size = new System.Drawing.Size(24, 12);
            this.lbSlideTall.TabIndex = 17;
            this.lbSlideTall.Text = "Tall";
            // 
            // tbSlideTall
            // 
            this.tbSlideTall.Location = new System.Drawing.Point(604, 422);
            this.tbSlideTall.Maximum = 20;
            this.tbSlideTall.Name = "tbSlideTall";
            this.tbSlideTall.Size = new System.Drawing.Size(168, 45);
            this.tbSlideTall.TabIndex = 16;
            this.tbSlideTall.ValueChanged += new System.EventHandler(this.tbSlideTall_ValueChanged);
            // 
            // FigureForm
            // 
            this.ClientSize = new System.Drawing.Size(784, 563);
            this.Controls.Add(this.lbSlideEye);
            this.Controls.Add(this.tbSlideEye);
            this.Controls.Add(this.lbSlideTall);
            this.Controls.Add(this.tbSlideTall);
            this.Controls.Add(this.lbSlideBust);
            this.Controls.Add(this.lbSlideWaist);
            this.Controls.Add(this.lbSlideArm);
            this.Controls.Add(this.lbSlideLeg);
            this.Controls.Add(this.tbSlideBust);
            this.Controls.Add(this.tbSlideWaist);
            this.Controls.Add(this.tbSlideArm);
            this.Controls.Add(this.tbSlideLeg);
            this.Controls.Add(this.gvShaderParams);
            this.Controls.Add(this.lvSubScripts);
            this.Controls.Add(this.lvTSOFiles);
            this.Controls.Add(this.btnDown);
            this.Controls.Add(this.btnUp);
            this.Controls.Add(this.btnDump);
            this.Name = "FigureForm";
            this.Text = "TSOGrid";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FigureForm_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.gvShaderParams)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbSlideEye)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbSlideLeg)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbSlideArm)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbSlideWaist)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbSlideBust)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbSlideTall)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnDump;
        private System.Windows.Forms.Button btnUp;
        private System.Windows.Forms.Button btnDown;
        private System.Windows.Forms.ListView lvTSOFiles;
        private System.Windows.Forms.ListView lvSubScripts;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.TrackBar tbSlideEye;
        private System.Windows.Forms.TrackBar tbSlideLeg;
        private System.Windows.Forms.TrackBar tbSlideArm;
        private System.Windows.Forms.TrackBar tbSlideWaist;
        private System.Windows.Forms.TrackBar tbSlideBust;
        private System.Windows.Forms.Label lbSlideEye;
        private System.Windows.Forms.Label lbSlideLeg;
        private System.Windows.Forms.Label lbSlideArm;
        private System.Windows.Forms.Label lbSlideWaist;
        private System.Windows.Forms.Label lbSlideBust;
        private System.Windows.Forms.Label lbSlideTall;
        private System.Windows.Forms.TrackBar tbSlideTall;
        private System.Windows.Forms.DataGridView gvShaderParams;
    }
}
