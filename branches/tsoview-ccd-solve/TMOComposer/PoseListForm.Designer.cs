namespace TMOComposer
{
    partial class PoseListForm
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
            this.btnGetPoses = new System.Windows.Forms.Button();
            this.lvPoses = new System.Windows.Forms.ListView();
            this.ilPoses = new System.Windows.Forms.ImageList(this.components);
            this.SuspendLayout();
            // 
            // btnGetPoses
            // 
            this.btnGetPoses.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnGetPoses.Location = new System.Drawing.Point(697, 528);
            this.btnGetPoses.Name = "btnGetPoses";
            this.btnGetPoses.Size = new System.Drawing.Size(75, 23);
            this.btnGetPoses.TabIndex = 17;
            this.btnGetPoses.Text = "&Get poses";
            this.btnGetPoses.UseVisualStyleBackColor = true;
            this.btnGetPoses.Click += new System.EventHandler(this.btnGetPoses_Click);
            // 
            // lvPoses
            // 
            this.lvPoses.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lvPoses.Location = new System.Drawing.Point(12, 12);
            this.lvPoses.Name = "lvPoses";
            this.lvPoses.Size = new System.Drawing.Size(760, 510);
            this.lvPoses.TabIndex = 16;
            this.lvPoses.UseCompatibleStateImageBehavior = false;
            this.lvPoses.DoubleClick += new System.EventHandler(this.lvPoses_DoubleClick);
            // 
            // ilPoses
            // 
            this.ilPoses.ColorDepth = System.Windows.Forms.ColorDepth.Depth24Bit;
            this.ilPoses.ImageSize = new System.Drawing.Size(128, 128);
            this.ilPoses.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // PoseListForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 563);
            this.Controls.Add(this.btnGetPoses);
            this.Controls.Add(this.lvPoses);
            this.Name = "PoseListForm";
            this.Text = "PoseListForm";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnGetPoses;
        private System.Windows.Forms.ListView lvPoses;
        private System.Windows.Forms.ImageList ilPoses;
    }
}