namespace TMOComposer
{
    partial class FaceListForm
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
            this.btnGetFaces = new System.Windows.Forms.Button();
            this.lvFaces = new System.Windows.Forms.ListView();
            this.ilFaces = new System.Windows.Forms.ImageList(this.components);
            this.SuspendLayout();
            // 
            // btnGetFaces
            // 
            this.btnGetFaces.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnGetFaces.Location = new System.Drawing.Point(697, 528);
            this.btnGetFaces.Name = "btnGetFaces";
            this.btnGetFaces.Size = new System.Drawing.Size(75, 23);
            this.btnGetFaces.TabIndex = 15;
            this.btnGetFaces.Text = "&Get faces";
            this.btnGetFaces.UseVisualStyleBackColor = true;
            this.btnGetFaces.Click += new System.EventHandler(this.btnGetFaces_Click);
            // 
            // lvFaces
            // 
            this.lvFaces.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lvFaces.LargeImageList = this.ilFaces;
            this.lvFaces.Location = new System.Drawing.Point(12, 12);
            this.lvFaces.Name = "lvFaces";
            this.lvFaces.Size = new System.Drawing.Size(760, 510);
            this.lvFaces.TabIndex = 14;
            this.lvFaces.UseCompatibleStateImageBehavior = false;
            this.lvFaces.DoubleClick += new System.EventHandler(this.lvFaces_DoubleClick);
            // 
            // ilFaces
            // 
            this.ilFaces.ColorDepth = System.Windows.Forms.ColorDepth.Depth24Bit;
            this.ilFaces.ImageSize = new System.Drawing.Size(128, 128);
            this.ilFaces.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // FaceListForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 563);
            this.Controls.Add(this.btnGetFaces);
            this.Controls.Add(this.lvFaces);
            this.Name = "FaceListForm";
            this.Text = "FaceListForm";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnGetFaces;
        private System.Windows.Forms.ListView lvFaces;
        private System.Windows.Forms.ImageList ilFaces;
    }
}