namespace TMOKinect
{
    partial class AccelSlider
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

        #region コンポーネント デザイナで生成されたコード

        /// <summary> 
        /// デザイナ サポートに必要なメソッドです。このメソッドの内容を 
        /// コード エディタで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.tbAccel = new System.Windows.Forms.TrackBar();
            this.label1 = new System.Windows.Forms.Label();
            this.lbAccel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.tbAccel)).BeginInit();
            this.SuspendLayout();
            // 
            // tbAccel
            // 
            this.tbAccel.Location = new System.Drawing.Point(3, 30);
            this.tbAccel.Name = "tbAccel";
            this.tbAccel.Size = new System.Drawing.Size(200, 45);
            this.tbAccel.TabIndex = 12;
            this.tbAccel.Value = 5;
            this.tbAccel.ValueChanged += new System.EventHandler(this.tbAccel_ValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(-2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(34, 12);
            this.label1.TabIndex = 11;
            this.label1.Text = "Accel";
            // 
            // lbAccel
            // 
            this.lbAccel.AutoSize = true;
            this.lbAccel.Location = new System.Drawing.Point(209, 30);
            this.lbAccel.Name = "lbAccel";
            this.lbAccel.Size = new System.Drawing.Size(19, 12);
            this.lbAccel.TabIndex = 13;
            this.lbAccel.Text = "0.5";
            // 
            // AccelSlider
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lbAccel);
            this.Controls.Add(this.tbAccel);
            this.Controls.Add(this.label1);
            this.Name = "AccelSlider";
            this.Size = new System.Drawing.Size(250, 75);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.AccelSlider_Paint);
            ((System.ComponentModel.ISupportInitialize)(this.tbAccel)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TrackBar tbAccel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lbAccel;
    }
}
