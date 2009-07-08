namespace TDCGExplorer
{
    partial class EditSystemDatabase
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
            this.OKButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.tbArcsDirectory = new System.Windows.Forms.TextBox();
            this.DialoogCancelButton = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.tbZipDirectory = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.tbModRefServer = new System.Windows.Forms.TextBox();
            this.tbZipRegexp = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.tbArcnamesServer = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.tbWorkPath = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.checkBoxLookupModRef = new System.Windows.Forms.CheckBox();
            this.textBoxModRegexp = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.textBoxTagNameServer = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // OKButton
            // 
            this.OKButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.OKButton.Location = new System.Drawing.Point(448, 220);
            this.OKButton.Name = "OKButton";
            this.OKButton.Size = new System.Drawing.Size(75, 23);
            this.OKButton.TabIndex = 0;
            this.OKButton.Text = "OK";
            this.OKButton.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(76, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "arcsディレクトリ";
            // 
            // tbArcsDirectory
            // 
            this.tbArcsDirectory.Location = new System.Drawing.Point(102, 12);
            this.tbArcsDirectory.Name = "tbArcsDirectory";
            this.tbArcsDirectory.Size = new System.Drawing.Size(502, 20);
            this.tbArcsDirectory.TabIndex = 2;
            // 
            // DialoogCancelButton
            // 
            this.DialoogCancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.DialoogCancelButton.Location = new System.Drawing.Point(529, 220);
            this.DialoogCancelButton.Name = "DialoogCancelButton";
            this.DialoogCancelButton.Size = new System.Drawing.Size(75, 23);
            this.DialoogCancelButton.TabIndex = 3;
            this.DialoogCancelButton.Text = "Cancel";
            this.DialoogCancelButton.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 41);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(73, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "ZIPディレクトリ";
            // 
            // tbZipDirectory
            // 
            this.tbZipDirectory.Location = new System.Drawing.Point(102, 38);
            this.tbZipDirectory.Name = "tbZipDirectory";
            this.tbZipDirectory.Size = new System.Drawing.Size(502, 20);
            this.tbZipDirectory.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 93);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(77, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "mod ref. server";
            // 
            // tbModRefServer
            // 
            this.tbModRefServer.Location = new System.Drawing.Point(102, 90);
            this.tbModRefServer.Name = "tbModRefServer";
            this.tbModRefServer.Size = new System.Drawing.Size(502, 20);
            this.tbModRefServer.TabIndex = 7;
            // 
            // tbZipRegexp
            // 
            this.tbZipRegexp.Location = new System.Drawing.Point(102, 168);
            this.tbZipRegexp.Name = "tbZipRegexp";
            this.tbZipRegexp.Size = new System.Drawing.Size(502, 20);
            this.tbZipRegexp.TabIndex = 9;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 171);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(72, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "ZIP正規表現";
            // 
            // tbArcnamesServer
            // 
            this.tbArcnamesServer.Location = new System.Drawing.Point(101, 116);
            this.tbArcnamesServer.Name = "tbArcnamesServer";
            this.tbArcnamesServer.Size = new System.Drawing.Size(502, 20);
            this.tbArcnamesServer.TabIndex = 11;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(11, 119);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(85, 13);
            this.label5.TabIndex = 10;
            this.label5.Text = "arcnames server";
            // 
            // tbWorkPath
            // 
            this.tbWorkPath.Location = new System.Drawing.Point(102, 64);
            this.tbWorkPath.Name = "tbWorkPath";
            this.tbWorkPath.Size = new System.Drawing.Size(502, 20);
            this.tbWorkPath.TabIndex = 13;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 67);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(60, 13);
            this.label6.TabIndex = 12;
            this.label6.Text = "ZIP展開先";
            // 
            // checkBoxLookupModRef
            // 
            this.checkBoxLookupModRef.AutoSize = true;
            this.checkBoxLookupModRef.Location = new System.Drawing.Point(102, 220);
            this.checkBoxLookupModRef.Name = "checkBoxLookupModRef";
            this.checkBoxLookupModRef.Size = new System.Drawing.Size(177, 17);
            this.checkBoxLookupModRef.TabIndex = 14;
            this.checkBoxLookupModRef.Text = "ZIP関連情報の自動照会を行う";
            this.checkBoxLookupModRef.UseVisualStyleBackColor = true;
            // 
            // textBoxModRegexp
            // 
            this.textBoxModRegexp.Location = new System.Drawing.Point(102, 194);
            this.textBoxModRegexp.Name = "textBoxModRegexp";
            this.textBoxModRegexp.Size = new System.Drawing.Size(502, 20);
            this.textBoxModRegexp.TabIndex = 16;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(12, 197);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(80, 13);
            this.label7.TabIndex = 15;
            this.label7.Text = "MOD正規表現";
            // 
            // textBoxTagNameServer
            // 
            this.textBoxTagNameServer.Location = new System.Drawing.Point(101, 142);
            this.textBoxTagNameServer.Name = "textBoxTagNameServer";
            this.textBoxTagNameServer.Size = new System.Drawing.Size(502, 20);
            this.textBoxTagNameServer.TabIndex = 19;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(11, 145);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(85, 13);
            this.label8.TabIndex = 18;
            this.label8.Text = "tagnames server";
            // 
            // EditSystemDatabase
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(615, 253);
            this.Controls.Add(this.textBoxTagNameServer);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.textBoxModRegexp);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.checkBoxLookupModRef);
            this.Controls.Add(this.tbWorkPath);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.tbArcnamesServer);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.tbZipRegexp);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.tbModRefServer);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.tbZipDirectory);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.DialoogCancelButton);
            this.Controls.Add(this.tbArcsDirectory);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.OKButton);
            this.Name = "EditSystemDatabase";
            this.Text = "初期設定";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button OKButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbArcsDirectory;
        private System.Windows.Forms.Button DialoogCancelButton;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbZipDirectory;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbModRefServer;
        private System.Windows.Forms.TextBox tbZipRegexp;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox tbArcnamesServer;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox tbWorkPath;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.CheckBox checkBoxLookupModRef;
        private System.Windows.Forms.TextBox textBoxModRegexp;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox textBoxTagNameServer;
        private System.Windows.Forms.Label label8;
    }
}