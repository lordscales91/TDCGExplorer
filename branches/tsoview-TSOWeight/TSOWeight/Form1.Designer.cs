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
            this.lvFrames.Size = new System.Drawing.Size(180, 120);
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
            this.lvMeshes.Size = new System.Drawing.Size(180, 120);
            this.lvMeshes.TabIndex = 2;
            this.lvMeshes.UseCompatibleStateImageBehavior = false;
            this.lvMeshes.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Name";
            // 
            // lbFrames
            // 
            this.lbFrames.Location = new System.Drawing.Point(14, 9);
            this.lbFrames.Name = "lbFrames";
            this.lbFrames.Size = new System.Drawing.Size(178, 12);
            this.lbFrames.TabIndex = 3;
            this.lbFrames.Text = "Frames";
            // 
            // lbMeshes
            // 
            this.lbMeshes.Location = new System.Drawing.Point(12, 147);
            this.lbMeshes.Name = "lbMeshes";
            this.lbMeshes.Size = new System.Drawing.Size(180, 12);
            this.lbMeshes.TabIndex = 4;
            this.lbMeshes.Text = "Meshes";
            // 
            // Form1
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 563);
            this.Controls.Add(this.lbMeshes);
            this.Controls.Add(this.lbFrames);
            this.Controls.Add(this.lvMeshes);
            this.Controls.Add(this.lvFrames);
            this.Name = "Form1";
            this.Text = "TSOWeight";
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.Form1_DragDrop);
            this.DragOver += new System.Windows.Forms.DragEventHandler(this.Form1_DragOver);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView lvFrames;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.ListView lvMeshes;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.Label lbFrames;
        private System.Windows.Forms.Label lbMeshes;
    }
}

