namespace Vmd2Tmo
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.button3 = new System.Windows.Forms.Button();
            this.referFileName3 = new TDCGUtils.ReferFileName();
            this.referFileName2 = new TDCGUtils.ReferFileName();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(54, 144);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(241, 36);
            this.button3.TabIndex = 23;
            this.button3.Text = "変換 ( .vmd -> .tmo )";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // referFileName3
            // 
            this.referFileName3.FileName = "";
            this.referFileName3.Filter = ".vmd";
            this.referFileName3.Location = new System.Drawing.Point(11, 78);
            this.referFileName3.Name = "referFileName3";
            this.referFileName3.Size = new System.Drawing.Size(328, 60);
            this.referFileName3.TabIndex = 30;
            this.referFileName3.Title = "VMDファイル（ドラッグ＆ドロップ可）";
            this.referFileName3.UserEnabled = true;
            // 
            // referFileName2
            // 
            this.referFileName2.FileName = "";
            this.referFileName2.Filter = ".pmd";
            this.referFileName2.Location = new System.Drawing.Point(11, 12);
            this.referFileName2.Name = "referFileName2";
            this.referFileName2.Size = new System.Drawing.Size(328, 60);
            this.referFileName2.TabIndex = 29;
            this.referFileName2.Title = "PMDファイル（ドラッグ＆ドロップ可）";
            this.referFileName2.UserEnabled = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 194);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(319, 24);
            this.label1.TabIndex = 31;
            this.label1.Text = "＊ PMDファイルには、3Dカスタム少女上で動かしたいモデルのヘビー\r\n　　セーブデータを、Tso2Pmdで変換したものを指定してください。";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(349, 236);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.referFileName3);
            this.Controls.Add(this.referFileName2);
            this.Controls.Add(this.button3);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "Vmd2Tmo";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button3;
        private TDCGUtils.ReferFileName referFileName2;
        private TDCGUtils.ReferFileName referFileName3;
        private System.Windows.Forms.Label label1;
    }
}

