namespace TMOComposer
{
    partial class TmoAnimItemForm
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
            this.lbPoseFile = new System.Windows.Forms.Label();
            this.tbPoseFile = new System.Windows.Forms.TextBox();
            this.lbLength = new System.Windows.Forms.Label();
            this.tbLength = new System.Windows.Forms.TextBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.tbFaceFile = new System.Windows.Forms.TextBox();
            this.lbFaceFile = new System.Windows.Forms.Label();
            this.btnOpenFaces = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lbPoseFile
            // 
            this.lbPoseFile.AutoSize = true;
            this.lbPoseFile.Location = new System.Drawing.Point(12, 9);
            this.lbPoseFile.Name = "lbPoseFile";
            this.lbPoseFile.Size = new System.Drawing.Size(49, 12);
            this.lbPoseFile.TabIndex = 0;
            this.lbPoseFile.Text = "PoseFile";
            // 
            // tbPoseFile
            // 
            this.tbPoseFile.Location = new System.Drawing.Point(12, 24);
            this.tbPoseFile.Name = "tbPoseFile";
            this.tbPoseFile.Size = new System.Drawing.Size(200, 19);
            this.tbPoseFile.TabIndex = 1;
            // 
            // lbLength
            // 
            this.lbLength.AutoSize = true;
            this.lbLength.Location = new System.Drawing.Point(10, 46);
            this.lbLength.Name = "lbLength";
            this.lbLength.Size = new System.Drawing.Size(39, 12);
            this.lbLength.TabIndex = 2;
            this.lbLength.Text = "Length";
            // 
            // tbLength
            // 
            this.tbLength.Location = new System.Drawing.Point(12, 61);
            this.tbLength.Name = "tbLength";
            this.tbLength.Size = new System.Drawing.Size(100, 19);
            this.tbLength.TabIndex = 3;
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(197, 228);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "&Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(116, 228);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 5;
            this.btnOK.Text = "&OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // tbFaceFile
            // 
            this.tbFaceFile.Location = new System.Drawing.Point(12, 98);
            this.tbFaceFile.Name = "tbFaceFile";
            this.tbFaceFile.Size = new System.Drawing.Size(200, 19);
            this.tbFaceFile.TabIndex = 7;
            // 
            // lbFaceFile
            // 
            this.lbFaceFile.AutoSize = true;
            this.lbFaceFile.Location = new System.Drawing.Point(12, 83);
            this.lbFaceFile.Name = "lbFaceFile";
            this.lbFaceFile.Size = new System.Drawing.Size(49, 12);
            this.lbFaceFile.TabIndex = 6;
            this.lbFaceFile.Text = "FaceFile";
            // 
            // btnOpenFaces
            // 
            this.btnOpenFaces.Location = new System.Drawing.Point(218, 96);
            this.btnOpenFaces.Name = "btnOpenFaces";
            this.btnOpenFaces.Size = new System.Drawing.Size(50, 23);
            this.btnOpenFaces.TabIndex = 8;
            this.btnOpenFaces.Text = "Open...";
            this.btnOpenFaces.UseVisualStyleBackColor = true;
            this.btnOpenFaces.Click += new System.EventHandler(this.btnOpenFaces_Click);
            // 
            // TmoAnimItemForm
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(284, 263);
            this.Controls.Add(this.btnOpenFaces);
            this.Controls.Add(this.tbFaceFile);
            this.Controls.Add(this.lbFaceFile);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.tbLength);
            this.Controls.Add(this.lbLength);
            this.Controls.Add(this.tbPoseFile);
            this.Controls.Add(this.lbPoseFile);
            this.Name = "TmoAnimItemForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "TmoAnimItemForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lbPoseFile;
        private System.Windows.Forms.TextBox tbPoseFile;
        private System.Windows.Forms.Label lbLength;
        private System.Windows.Forms.TextBox tbLength;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.TextBox tbFaceFile;
        private System.Windows.Forms.Label lbFaceFile;
        private System.Windows.Forms.Button btnOpenFaces;
    }
}