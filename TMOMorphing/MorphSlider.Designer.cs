namespace TMOMorphing
{
    partial class MorphSlider
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
            this.tbRatio = new System.Windows.Forms.TrackBar();
            this.lbGroupName = new System.Windows.Forms.Label();
            this.cbMorphNames = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.tbRatio)).BeginInit();
            this.SuspendLayout();
            // 
            // tbRatio
            // 
            this.tbRatio.Location = new System.Drawing.Point(12, 51);
            this.tbRatio.Name = "tbRatio";
            this.tbRatio.Size = new System.Drawing.Size(121, 45);
            this.tbRatio.TabIndex = 0;
            this.tbRatio.ValueChanged += new System.EventHandler(this.tbRatio_ValueChanged);
            // 
            // lbGroupName
            // 
            this.lbGroupName.AutoSize = true;
            this.lbGroupName.Location = new System.Drawing.Point(10, 10);
            this.lbGroupName.Name = "lbGroupName";
            this.lbGroupName.Size = new System.Drawing.Size(35, 12);
            this.lbGroupName.TabIndex = 1;
            this.lbGroupName.Text = "label1";
            // 
            // cbMorphNames
            // 
            this.cbMorphNames.FormattingEnabled = true;
            this.cbMorphNames.Location = new System.Drawing.Point(12, 25);
            this.cbMorphNames.Name = "cbMorphNames";
            this.cbMorphNames.Size = new System.Drawing.Size(121, 20);
            this.cbMorphNames.TabIndex = 3;
            this.cbMorphNames.SelectedIndexChanged += new System.EventHandler(this.cbMorphNames_SelectedIndexChanged);
            // 
            // MorphSlider
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.cbMorphNames);
            this.Controls.Add(this.lbGroupName);
            this.Controls.Add(this.tbRatio);
            this.Name = "MorphSlider";
            this.Size = new System.Drawing.Size(150, 85);
            ((System.ComponentModel.ISupportInitialize)(this.tbRatio)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TrackBar tbRatio;
        private System.Windows.Forms.Label lbGroupName;
        private System.Windows.Forms.ComboBox cbMorphNames;
    }
}
