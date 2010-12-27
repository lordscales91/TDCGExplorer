namespace TSOEditor
{
    partial class Form2
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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tpGeneral = new System.Windows.Forms.TabPage();
            this.btnTSOSave = new System.Windows.Forms.Button();
            this.lvTSOFiles = new System.Windows.Forms.ListView();
            this.chTSOName = new System.Windows.Forms.ColumnHeader();
            this.tpNodes = new System.Windows.Forms.TabPage();
            this.lvNodes = new System.Windows.Forms.ListView();
            this.chNodeName = new System.Windows.Forms.ColumnHeader();
            this.tpTextures = new System.Windows.Forms.TabPage();
            this.btnTexSave = new System.Windows.Forms.Button();
            this.btnTexLoad = new System.Windows.Forms.Button();
            this.pbTexThumbnail = new System.Windows.Forms.PictureBox();
            this.lvTextures = new System.Windows.Forms.ListView();
            this.chTexName = new System.Windows.Forms.ColumnHeader();
            this.chTexFileName = new System.Windows.Forms.ColumnHeader();
            this.tpSubScripts = new System.Windows.Forms.TabPage();
            this.lvSubScripts = new System.Windows.Forms.ListView();
            this.chSubName = new System.Windows.Forms.ColumnHeader();
            this.chSubFileName = new System.Windows.Forms.ColumnHeader();
            this.tpMeshes = new System.Windows.Forms.TabPage();
            this.lvMeshes = new System.Windows.Forms.ListView();
            this.chMeshName = new System.Windows.Forms.ColumnHeader();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.tabControl1.SuspendLayout();
            this.tpGeneral.SuspendLayout();
            this.tpNodes.SuspendLayout();
            this.tpTextures.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbTexThumbnail)).BeginInit();
            this.tpSubScripts.SuspendLayout();
            this.tpMeshes.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tpGeneral);
            this.tabControl1.Controls.Add(this.tpNodes);
            this.tabControl1.Controls.Add(this.tpTextures);
            this.tabControl1.Controls.Add(this.tpSubScripts);
            this.tabControl1.Controls.Add(this.tpMeshes);
            this.tabControl1.Location = new System.Drawing.Point(12, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(610, 429);
            this.tabControl1.TabIndex = 0;
            // 
            // tpGeneral
            // 
            this.tpGeneral.Controls.Add(this.btnTSOSave);
            this.tpGeneral.Controls.Add(this.lvTSOFiles);
            this.tpGeneral.Location = new System.Drawing.Point(4, 22);
            this.tpGeneral.Name = "tpGeneral";
            this.tpGeneral.Padding = new System.Windows.Forms.Padding(3);
            this.tpGeneral.Size = new System.Drawing.Size(602, 403);
            this.tpGeneral.TabIndex = 4;
            this.tpGeneral.Text = "General";
            this.tpGeneral.UseVisualStyleBackColor = true;
            // 
            // btnTSOSave
            // 
            this.btnTSOSave.Location = new System.Drawing.Point(186, 6);
            this.btnTSOSave.Name = "btnTSOSave";
            this.btnTSOSave.Size = new System.Drawing.Size(75, 23);
            this.btnTSOSave.TabIndex = 2;
            this.btnTSOSave.Text = "&Save";
            this.btnTSOSave.UseVisualStyleBackColor = true;
            this.btnTSOSave.Click += new System.EventHandler(this.btnTSOSave_Click);
            // 
            // lvTSOFiles
            // 
            this.lvTSOFiles.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chTSOName});
            this.lvTSOFiles.Location = new System.Drawing.Point(6, 6);
            this.lvTSOFiles.Name = "lvTSOFiles";
            this.lvTSOFiles.Size = new System.Drawing.Size(174, 391);
            this.lvTSOFiles.TabIndex = 1;
            this.lvTSOFiles.UseCompatibleStateImageBehavior = false;
            this.lvTSOFiles.View = System.Windows.Forms.View.Details;
            this.lvTSOFiles.SelectedIndexChanged += new System.EventHandler(this.lvTSOFiles_SelectedIndexChanged);
            // 
            // chTSOName
            // 
            this.chTSOName.Text = "Name";
            // 
            // tpNodes
            // 
            this.tpNodes.Controls.Add(this.lvNodes);
            this.tpNodes.Location = new System.Drawing.Point(4, 22);
            this.tpNodes.Name = "tpNodes";
            this.tpNodes.Padding = new System.Windows.Forms.Padding(3);
            this.tpNodes.Size = new System.Drawing.Size(602, 403);
            this.tpNodes.TabIndex = 0;
            this.tpNodes.Text = "nodes";
            this.tpNodes.UseVisualStyleBackColor = true;
            // 
            // lvNodes
            // 
            this.lvNodes.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chNodeName});
            this.lvNodes.Location = new System.Drawing.Point(6, 6);
            this.lvNodes.Name = "lvNodes";
            this.lvNodes.Size = new System.Drawing.Size(174, 381);
            this.lvNodes.TabIndex = 0;
            this.lvNodes.UseCompatibleStateImageBehavior = false;
            this.lvNodes.View = System.Windows.Forms.View.Details;
            this.lvNodes.SelectedIndexChanged += new System.EventHandler(this.lvNodes_SelectedIndexChanged);
            // 
            // chNodeName
            // 
            this.chNodeName.Text = "Name";
            // 
            // tpTextures
            // 
            this.tpTextures.Controls.Add(this.btnTexSave);
            this.tpTextures.Controls.Add(this.btnTexLoad);
            this.tpTextures.Controls.Add(this.pbTexThumbnail);
            this.tpTextures.Controls.Add(this.lvTextures);
            this.tpTextures.Location = new System.Drawing.Point(4, 22);
            this.tpTextures.Name = "tpTextures";
            this.tpTextures.Padding = new System.Windows.Forms.Padding(3);
            this.tpTextures.Size = new System.Drawing.Size(602, 403);
            this.tpTextures.TabIndex = 1;
            this.tpTextures.Text = "textures";
            this.tpTextures.UseVisualStyleBackColor = true;
            // 
            // btnTexSave
            // 
            this.btnTexSave.Location = new System.Drawing.Point(269, 269);
            this.btnTexSave.Name = "btnTexSave";
            this.btnTexSave.Size = new System.Drawing.Size(75, 23);
            this.btnTexSave.TabIndex = 3;
            this.btnTexSave.Text = "&Save";
            this.btnTexSave.UseVisualStyleBackColor = true;
            this.btnTexSave.Click += new System.EventHandler(this.btnTexSave_Click);
            // 
            // btnTexLoad
            // 
            this.btnTexLoad.Location = new System.Drawing.Point(187, 269);
            this.btnTexLoad.Name = "btnTexLoad";
            this.btnTexLoad.Size = new System.Drawing.Size(75, 23);
            this.btnTexLoad.TabIndex = 2;
            this.btnTexLoad.Text = "&Load";
            this.btnTexLoad.UseVisualStyleBackColor = true;
            this.btnTexLoad.Click += new System.EventHandler(this.btnTexLoad_Click);
            // 
            // pbTexThumbnail
            // 
            this.pbTexThumbnail.Location = new System.Drawing.Point(186, 6);
            this.pbTexThumbnail.Name = "pbTexThumbnail";
            this.pbTexThumbnail.Size = new System.Drawing.Size(256, 256);
            this.pbTexThumbnail.TabIndex = 1;
            this.pbTexThumbnail.TabStop = false;
            // 
            // lvTextures
            // 
            this.lvTextures.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chTexName,
            this.chTexFileName});
            this.lvTextures.Location = new System.Drawing.Point(6, 6);
            this.lvTextures.Name = "lvTextures";
            this.lvTextures.Size = new System.Drawing.Size(174, 381);
            this.lvTextures.TabIndex = 0;
            this.lvTextures.UseCompatibleStateImageBehavior = false;
            this.lvTextures.View = System.Windows.Forms.View.Details;
            this.lvTextures.SelectedIndexChanged += new System.EventHandler(this.lvTextures_SelectedIndexChanged);
            // 
            // chTexName
            // 
            this.chTexName.Text = "Name";
            // 
            // chTexFileName
            // 
            this.chTexFileName.Text = "File";
            // 
            // tpSubScripts
            // 
            this.tpSubScripts.Controls.Add(this.lvSubScripts);
            this.tpSubScripts.Location = new System.Drawing.Point(4, 22);
            this.tpSubScripts.Name = "tpSubScripts";
            this.tpSubScripts.Size = new System.Drawing.Size(602, 403);
            this.tpSubScripts.TabIndex = 2;
            this.tpSubScripts.Text = "sub scripts";
            this.tpSubScripts.UseVisualStyleBackColor = true;
            // 
            // lvSubScripts
            // 
            this.lvSubScripts.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chSubName,
            this.chSubFileName});
            this.lvSubScripts.Location = new System.Drawing.Point(6, 6);
            this.lvSubScripts.Name = "lvSubScripts";
            this.lvSubScripts.Size = new System.Drawing.Size(174, 381);
            this.lvSubScripts.TabIndex = 0;
            this.lvSubScripts.UseCompatibleStateImageBehavior = false;
            this.lvSubScripts.View = System.Windows.Forms.View.Details;
            this.lvSubScripts.SelectedIndexChanged += new System.EventHandler(this.lvSubScripts_SelectedIndexChanged);
            // 
            // chSubName
            // 
            this.chSubName.Text = "Name";
            // 
            // chSubFileName
            // 
            this.chSubFileName.Text = "File";
            // 
            // tpMeshes
            // 
            this.tpMeshes.Controls.Add(this.lvMeshes);
            this.tpMeshes.Location = new System.Drawing.Point(4, 22);
            this.tpMeshes.Name = "tpMeshes";
            this.tpMeshes.Size = new System.Drawing.Size(602, 403);
            this.tpMeshes.TabIndex = 3;
            this.tpMeshes.Text = "meshes";
            this.tpMeshes.UseVisualStyleBackColor = true;
            // 
            // lvMeshes
            // 
            this.lvMeshes.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chMeshName});
            this.lvMeshes.Location = new System.Drawing.Point(6, 6);
            this.lvMeshes.Name = "lvMeshes";
            this.lvMeshes.Size = new System.Drawing.Size(174, 381);
            this.lvMeshes.TabIndex = 0;
            this.lvMeshes.UseCompatibleStateImageBehavior = false;
            this.lvMeshes.View = System.Windows.Forms.View.Details;
            this.lvMeshes.SelectedIndexChanged += new System.EventHandler(this.lvMeshes_SelectedIndexChanged);
            // 
            // chMeshName
            // 
            this.chMeshName.Text = "Name";
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // Form2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(634, 453);
            this.Controls.Add(this.tabControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "Form2";
            this.Text = "tso editor";
            this.tabControl1.ResumeLayout(false);
            this.tpGeneral.ResumeLayout(false);
            this.tpNodes.ResumeLayout(false);
            this.tpTextures.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbTexThumbnail)).EndInit();
            this.tpSubScripts.ResumeLayout(false);
            this.tpMeshes.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tpNodes;
        private System.Windows.Forms.ListView lvNodes;
        private System.Windows.Forms.TabPage tpTextures;
        private System.Windows.Forms.TabPage tpSubScripts;
        private System.Windows.Forms.TabPage tpMeshes;
        private System.Windows.Forms.ColumnHeader chNodeName;
        private System.Windows.Forms.ListView lvTextures;
        private System.Windows.Forms.ColumnHeader chTexName;
        private System.Windows.Forms.ListView lvSubScripts;
        private System.Windows.Forms.ColumnHeader chSubName;
        private System.Windows.Forms.ListView lvMeshes;
        private System.Windows.Forms.ColumnHeader chMeshName;
        private System.Windows.Forms.TabPage tpGeneral;
        private System.Windows.Forms.ColumnHeader chTexFileName;
        private System.Windows.Forms.ColumnHeader chSubFileName;
        private System.Windows.Forms.PictureBox pbTexThumbnail;
        private System.Windows.Forms.ListView lvTSOFiles;
        private System.Windows.Forms.ColumnHeader chTSOName;
        private System.Windows.Forms.Button btnTexSave;
        private System.Windows.Forms.Button btnTexLoad;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.Button btnTSOSave;
    }
}

