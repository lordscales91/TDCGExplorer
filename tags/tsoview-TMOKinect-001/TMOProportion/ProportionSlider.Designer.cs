﻿namespace TMOProportion
{
    partial class ProportionSlider
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
            this.lbClassName = new System.Windows.Forms.Label();
            this.tbRatio = new System.Windows.Forms.TrackBar();
            this.lbRatio = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.tbRatio)).BeginInit();
            this.SuspendLayout();
            // 
            // lbClassName
            // 
            this.lbClassName.AutoSize = true;
            this.lbClassName.Location = new System.Drawing.Point(3, 3);
            this.lbClassName.Name = "lbClassName";
            this.lbClassName.Size = new System.Drawing.Size(35, 12);
            this.lbClassName.TabIndex = 0;
            this.lbClassName.Text = "label1";
            // 
            // tbRatio
            // 
            this.tbRatio.Location = new System.Drawing.Point(3, 18);
            this.tbRatio.Maximum = 20;
            this.tbRatio.Name = "tbRatio";
            this.tbRatio.Size = new System.Drawing.Size(176, 45);
            this.tbRatio.TabIndex = 1;
            this.tbRatio.ValueChanged += new System.EventHandler(this.tbRatio_ValueChanged);
            // 
            // lbRatio
            // 
            this.lbRatio.AutoSize = true;
            this.lbRatio.Location = new System.Drawing.Point(156, 3);
            this.lbRatio.Name = "lbRatio";
            this.lbRatio.Size = new System.Drawing.Size(25, 12);
            this.lbRatio.TabIndex = 2;
            this.lbRatio.Text = "0.00";
            // 
            // ProportionSlider
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lbRatio);
            this.Controls.Add(this.tbRatio);
            this.Controls.Add(this.lbClassName);
            this.Name = "ProportionSlider";
            this.Size = new System.Drawing.Size(182, 64);
            ((System.ComponentModel.ISupportInitialize)(this.tbRatio)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.Label lbClassName;
        public System.Windows.Forms.TrackBar tbRatio;
        private System.Windows.Forms.Label lbRatio;

    }
}
