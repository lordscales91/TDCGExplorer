namespace TMOComposer
{
    partial class SaveListForm
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
            this.btnGetSaves = new System.Windows.Forms.Button();
            this.lvSaves = new System.Windows.Forms.ListView();
            this.ilSaves = new System.Windows.Forms.ImageList(this.components);
            this.SuspendLayout();
            // 
            // btnGetSaves
            // 
            this.btnGetSaves.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnGetSaves.Location = new System.Drawing.Point(697, 528);
            this.btnGetSaves.Name = "btnGetSaves";
            this.btnGetSaves.Size = new System.Drawing.Size(75, 23);
            this.btnGetSaves.TabIndex = 13;
            this.btnGetSaves.Text = "&Get saves";
            this.btnGetSaves.UseVisualStyleBackColor = true;
            this.btnGetSaves.Click += new System.EventHandler(this.btnGetSaves_Click);
            // 
            // lvSaves
            // 
            this.lvSaves.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lvSaves.LargeImageList = this.ilSaves;
            this.lvSaves.Location = new System.Drawing.Point(12, 12);
            this.lvSaves.Name = "lvSaves";
            this.lvSaves.Size = new System.Drawing.Size(760, 510);
            this.lvSaves.TabIndex = 12;
            this.lvSaves.UseCompatibleStateImageBehavior = false;
            this.lvSaves.DoubleClick += new System.EventHandler(this.lvSaves_DoubleClick);
            // 
            // ilSaves
            // 
            this.ilSaves.ColorDepth = System.Windows.Forms.ColorDepth.Depth24Bit;
            this.ilSaves.ImageSize = new System.Drawing.Size(128, 256);
            this.ilSaves.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // SaveListForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 563);
            this.Controls.Add(this.btnGetSaves);
            this.Controls.Add(this.lvSaves);
            this.Name = "SaveListForm";
            this.Text = "SaveListForm";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnGetSaves;
        private System.Windows.Forms.ListView lvSaves;
        private System.Windows.Forms.ImageList ilSaves;
    }
}