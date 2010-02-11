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
            this.lvFrames = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.lvMeshes = new System.Windows.Forms.ListView();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.lbFrames = new System.Windows.Forms.Label();
            this.lbMeshes = new System.Windows.Forms.Label();
            this.lbBoneIndices = new System.Windows.Forms.Label();
            this.lvBoneIndices = new System.Windows.Forms.ListView();
            this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
            this.cbBoneHeatingView = new System.Windows.Forms.CheckBox();
            this.lbCamera = new System.Windows.Forms.Label();
            this.btnCenter = new System.Windows.Forms.Button();
            this.lbSkinWeights = new System.Windows.Forms.Label();
            this.lvSkinWeights = new System.Windows.Forms.ListView();
            this.columnHeader4 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader5 = new System.Windows.Forms.ColumnHeader();
            this.lbWeight = new System.Windows.Forms.Label();
            this.tbWeight = new System.Windows.Forms.TrackBar();
            this.tbRadius = new System.Windows.Forms.TrackBar();
            this.lbRadius = new System.Windows.Forms.Label();
            this.btnDraw = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.lbTSOFiles = new System.Windows.Forms.Label();
            this.lvTSOFiles = new System.Windows.Forms.ListView();
            this.columnHeader6 = new System.Windows.Forms.ColumnHeader();
            ((System.ComponentModel.ISupportInitialize)(this.tbWeight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbRadius)).BeginInit();
            this.SuspendLayout();
            // 
            // lvFrames
            // 
            this.lvFrames.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.lvFrames.FullRowSelect = true;
            this.lvFrames.GridLines = true;
            this.lvFrames.HideSelection = false;
            this.lvFrames.Location = new System.Drawing.Point(12, 24);
            this.lvFrames.MultiSelect = false;
            this.lvFrames.Name = "lvFrames";
            this.lvFrames.Size = new System.Drawing.Size(120, 120);
            this.lvFrames.TabIndex = 0;
            this.lvFrames.UseCompatibleStateImageBehavior = false;
            this.lvFrames.View = System.Windows.Forms.View.Details;
            this.lvFrames.SelectedIndexChanged += new System.EventHandler(this.lvFrames_SelectedIndexChanged);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Name";
            // 
            // timer1
            // 
            this.timer1.Interval = 30;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // lvMeshes
            // 
            this.lvMeshes.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader2});
            this.lvMeshes.FullRowSelect = true;
            this.lvMeshes.GridLines = true;
            this.lvMeshes.HideSelection = false;
            this.lvMeshes.Location = new System.Drawing.Point(12, 162);
            this.lvMeshes.MultiSelect = false;
            this.lvMeshes.Name = "lvMeshes";
            this.lvMeshes.Size = new System.Drawing.Size(120, 120);
            this.lvMeshes.TabIndex = 2;
            this.lvMeshes.UseCompatibleStateImageBehavior = false;
            this.lvMeshes.View = System.Windows.Forms.View.Details;
            this.lvMeshes.SelectedIndexChanged += new System.EventHandler(this.lvMeshes_SelectedIndexChanged);
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Name";
            // 
            // lbFrames
            // 
            this.lbFrames.Location = new System.Drawing.Point(12, 9);
            this.lbFrames.Name = "lbFrames";
            this.lbFrames.Size = new System.Drawing.Size(120, 12);
            this.lbFrames.TabIndex = 3;
            this.lbFrames.Text = "Frames";
            // 
            // lbMeshes
            // 
            this.lbMeshes.Location = new System.Drawing.Point(12, 147);
            this.lbMeshes.Name = "lbMeshes";
            this.lbMeshes.Size = new System.Drawing.Size(120, 12);
            this.lbMeshes.TabIndex = 4;
            this.lbMeshes.Text = "Meshes";
            // 
            // lbBoneIndices
            // 
            this.lbBoneIndices.Location = new System.Drawing.Point(12, 285);
            this.lbBoneIndices.Name = "lbBoneIndices";
            this.lbBoneIndices.Size = new System.Drawing.Size(120, 12);
            this.lbBoneIndices.TabIndex = 6;
            this.lbBoneIndices.Text = "Bone indices";
            // 
            // lvBoneIndices
            // 
            this.lvBoneIndices.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader3});
            this.lvBoneIndices.FullRowSelect = true;
            this.lvBoneIndices.GridLines = true;
            this.lvBoneIndices.HideSelection = false;
            this.lvBoneIndices.Location = new System.Drawing.Point(12, 300);
            this.lvBoneIndices.MultiSelect = false;
            this.lvBoneIndices.Name = "lvBoneIndices";
            this.lvBoneIndices.Size = new System.Drawing.Size(120, 240);
            this.lvBoneIndices.TabIndex = 5;
            this.lvBoneIndices.UseCompatibleStateImageBehavior = false;
            this.lvBoneIndices.View = System.Windows.Forms.View.Details;
            this.lvBoneIndices.SelectedIndexChanged += new System.EventHandler(this.lvBoneIndices_SelectedIndexChanged);
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Name";
            // 
            // cbBoneHeatingView
            // 
            this.cbBoneHeatingView.AutoSize = true;
            this.cbBoneHeatingView.Location = new System.Drawing.Point(138, 300);
            this.cbBoneHeatingView.Name = "cbBoneHeatingView";
            this.cbBoneHeatingView.Size = new System.Drawing.Size(48, 16);
            this.cbBoneHeatingView.TabIndex = 7;
            this.cbBoneHeatingView.Text = "&Heat";
            this.cbBoneHeatingView.UseVisualStyleBackColor = true;
            this.cbBoneHeatingView.CheckedChanged += new System.EventHandler(this.cbBoneHeatingView_CheckedChanged);
            // 
            // lbCamera
            // 
            this.lbCamera.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lbCamera.Location = new System.Drawing.Point(652, 9);
            this.lbCamera.Name = "lbCamera";
            this.lbCamera.Size = new System.Drawing.Size(120, 12);
            this.lbCamera.TabIndex = 8;
            this.lbCamera.Text = "Camera";
            // 
            // btnCenter
            // 
            this.btnCenter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCenter.Location = new System.Drawing.Point(652, 24);
            this.btnCenter.Name = "btnCenter";
            this.btnCenter.Size = new System.Drawing.Size(120, 23);
            this.btnCenter.TabIndex = 9;
            this.btnCenter.Text = "&Center";
            this.btnCenter.UseVisualStyleBackColor = true;
            this.btnCenter.Click += new System.EventHandler(this.btnCenter_Click);
            // 
            // lbSkinWeights
            // 
            this.lbSkinWeights.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lbSkinWeights.Location = new System.Drawing.Point(652, 50);
            this.lbSkinWeights.Name = "lbSkinWeights";
            this.lbSkinWeights.Size = new System.Drawing.Size(120, 12);
            this.lbSkinWeights.TabIndex = 10;
            this.lbSkinWeights.Text = "Skin weights";
            // 
            // lvSkinWeights
            // 
            this.lvSkinWeights.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lvSkinWeights.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader4,
            this.columnHeader5});
            this.lvSkinWeights.FullRowSelect = true;
            this.lvSkinWeights.GridLines = true;
            this.lvSkinWeights.HideSelection = false;
            this.lvSkinWeights.Location = new System.Drawing.Point(652, 65);
            this.lvSkinWeights.MultiSelect = false;
            this.lvSkinWeights.Name = "lvSkinWeights";
            this.lvSkinWeights.Size = new System.Drawing.Size(120, 120);
            this.lvSkinWeights.TabIndex = 11;
            this.lvSkinWeights.UseCompatibleStateImageBehavior = false;
            this.lvSkinWeights.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Bone";
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "Weight";
            // 
            // lbWeight
            // 
            this.lbWeight.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lbWeight.Location = new System.Drawing.Point(652, 188);
            this.lbWeight.Name = "lbWeight";
            this.lbWeight.Size = new System.Drawing.Size(120, 12);
            this.lbWeight.TabIndex = 12;
            this.lbWeight.Text = "Weight";
            // 
            // tbWeight
            // 
            this.tbWeight.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.tbWeight.Location = new System.Drawing.Point(652, 203);
            this.tbWeight.Name = "tbWeight";
            this.tbWeight.Size = new System.Drawing.Size(120, 45);
            this.tbWeight.TabIndex = 13;
            this.tbWeight.Value = 2;
            this.tbWeight.ValueChanged += new System.EventHandler(this.tbWeight_ValueChanged);
            // 
            // tbRadius
            // 
            this.tbRadius.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.tbRadius.Location = new System.Drawing.Point(652, 266);
            this.tbRadius.Name = "tbRadius";
            this.tbRadius.Size = new System.Drawing.Size(120, 45);
            this.tbRadius.TabIndex = 15;
            this.tbRadius.Value = 5;
            this.tbRadius.ValueChanged += new System.EventHandler(this.tbRadius_ValueChanged);
            // 
            // lbRadius
            // 
            this.lbRadius.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lbRadius.Location = new System.Drawing.Point(652, 251);
            this.lbRadius.Name = "lbRadius";
            this.lbRadius.Size = new System.Drawing.Size(120, 12);
            this.lbRadius.TabIndex = 14;
            this.lbRadius.Text = "Radius";
            // 
            // btnDraw
            // 
            this.btnDraw.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDraw.Location = new System.Drawing.Point(652, 317);
            this.btnDraw.Name = "btnDraw";
            this.btnDraw.Size = new System.Drawing.Size(120, 23);
            this.btnDraw.TabIndex = 16;
            this.btnDraw.Text = "&Draw";
            this.btnDraw.UseVisualStyleBackColor = true;
            this.btnDraw.Click += new System.EventHandler(this.btnDraw_Click);
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.Location = new System.Drawing.Point(652, 528);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(120, 23);
            this.btnSave.TabIndex = 17;
            this.btnSave.Text = "&Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // lbTSOFiles
            // 
            this.lbTSOFiles.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lbTSOFiles.Location = new System.Drawing.Point(652, 387);
            this.lbTSOFiles.Name = "lbTSOFiles";
            this.lbTSOFiles.Size = new System.Drawing.Size(120, 12);
            this.lbTSOFiles.TabIndex = 19;
            this.lbTSOFiles.Text = "TSO files";
            // 
            // lvTSOFiles
            // 
            this.lvTSOFiles.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lvTSOFiles.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader6});
            this.lvTSOFiles.FullRowSelect = true;
            this.lvTSOFiles.GridLines = true;
            this.lvTSOFiles.HideSelection = false;
            this.lvTSOFiles.Location = new System.Drawing.Point(652, 402);
            this.lvTSOFiles.MultiSelect = false;
            this.lvTSOFiles.Name = "lvTSOFiles";
            this.lvTSOFiles.Size = new System.Drawing.Size(120, 120);
            this.lvTSOFiles.TabIndex = 18;
            this.lvTSOFiles.UseCompatibleStateImageBehavior = false;
            this.lvTSOFiles.View = System.Windows.Forms.View.Details;
            this.lvTSOFiles.SelectedIndexChanged += new System.EventHandler(this.lvTSOFiles_SelectedIndexChanged);
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "Name";
            // 
            // Form1
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 563);
            this.Controls.Add(this.lbTSOFiles);
            this.Controls.Add(this.lvTSOFiles);
            this.Controls.Add(this.lbWeight);
            this.Controls.Add(this.lbRadius);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnDraw);
            this.Controls.Add(this.tbRadius);
            this.Controls.Add(this.tbWeight);
            this.Controls.Add(this.lvSkinWeights);
            this.Controls.Add(this.lbSkinWeights);
            this.Controls.Add(this.btnCenter);
            this.Controls.Add(this.lbCamera);
            this.Controls.Add(this.cbBoneHeatingView);
            this.Controls.Add(this.lbBoneIndices);
            this.Controls.Add(this.lvBoneIndices);
            this.Controls.Add(this.lbMeshes);
            this.Controls.Add(this.lbFrames);
            this.Controls.Add(this.lvMeshes);
            this.Controls.Add(this.lvFrames);
            this.Name = "Form1";
            this.Text = "TSOWeight";
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.Form1_DragDrop);
            this.DragOver += new System.Windows.Forms.DragEventHandler(this.Form1_DragOver);
            ((System.ComponentModel.ISupportInitialize)(this.tbWeight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbRadius)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView lvFrames;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.ListView lvMeshes;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.Label lbFrames;
        private System.Windows.Forms.Label lbMeshes;
        private System.Windows.Forms.Label lbBoneIndices;
        private System.Windows.Forms.ListView lvBoneIndices;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.CheckBox cbBoneHeatingView;
        private System.Windows.Forms.Label lbCamera;
        private System.Windows.Forms.Button btnCenter;
        private System.Windows.Forms.Label lbSkinWeights;
        private System.Windows.Forms.ListView lvSkinWeights;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.Label lbWeight;
        private System.Windows.Forms.TrackBar tbWeight;
        private System.Windows.Forms.TrackBar tbRadius;
        private System.Windows.Forms.Label lbRadius;
        private System.Windows.Forms.Button btnDraw;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Label lbTSOFiles;
        private System.Windows.Forms.ListView lvTSOFiles;
        private System.Windows.Forms.ColumnHeader columnHeader6;
    }
}

