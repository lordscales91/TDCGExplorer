namespace TMOKinect
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
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.cbLimitRotation = new System.Windows.Forms.CheckBox();
            this.cbFloor = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // timer1
            // 
            this.timer1.Interval = 16;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // cbLimitRotation
            // 
            this.cbLimitRotation.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cbLimitRotation.Location = new System.Drawing.Point(572, 12);
            this.cbLimitRotation.Name = "cbLimitRotation";
            this.cbLimitRotation.Size = new System.Drawing.Size(100, 16);
            this.cbLimitRotation.TabIndex = 13;
            this.cbLimitRotation.Text = "&Limit Rotation";
            this.cbLimitRotation.UseVisualStyleBackColor = true;
            this.cbLimitRotation.CheckedChanged += new System.EventHandler(this.cbLimitRotation_CheckedChanged);
            // 
            // cbFloor
            // 
            this.cbFloor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cbFloor.Location = new System.Drawing.Point(572, 34);
            this.cbFloor.Name = "cbFloor";
            this.cbFloor.Size = new System.Drawing.Size(100, 16);
            this.cbFloor.TabIndex = 14;
            this.cbFloor.Text = "&Floor";
            this.cbFloor.UseVisualStyleBackColor = true;
            this.cbFloor.CheckedChanged += new System.EventHandler(this.cbFloor_CheckedChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 563);
            this.Controls.Add(this.cbFloor);
            this.Controls.Add(this.cbLimitRotation);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.CheckBox cbLimitRotation;
        private System.Windows.Forms.CheckBox cbFloor;
    }
}

