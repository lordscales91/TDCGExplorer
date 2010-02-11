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
            this.cbBoneHeatingView.Text = "Heat";
            this.cbBoneHeatingView.UseVisualStyleBackColor = true;
            this.cbBoneHeatingView.CheckedChanged += new System.EventHandler(this.cbBoneHeatingView_CheckedChanged);
            // 
            // Form1
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 563);
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
    }
}

