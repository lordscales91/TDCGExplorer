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
            this.textBoxTagNameServer = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioButtonBehaviorText = new System.Windows.Forms.RadioButton();
            this.radioButtonBehaviorImage = new System.Windows.Forms.RadioButton();
            this.radioButtonBehaviorServer = new System.Windows.Forms.RadioButton();
            this.radioButtonBehaviorNone = new System.Windows.Forms.RadioButton();
            this.label9 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.textBoxModRegexp = new System.Windows.Forms.TextBox();
            this.textBoxSaveFile = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.checkBoxCameraReset = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // OKButton
            // 
            this.OKButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.OKButton.Location = new System.Drawing.Point(447, 384);
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
            this.label1.Size = new System.Drawing.Size(85, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "ARCSディレクトリ";
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
            this.DialoogCancelButton.Location = new System.Drawing.Point(528, 384);
            this.DialoogCancelButton.Name = "DialoogCancelButton";
            this.DialoogCancelButton.Size = new System.Drawing.Size(75, 23);
            this.DialoogCancelButton.TabIndex = 3;
            this.DialoogCancelButton.Text = "Cancel";
            this.DialoogCancelButton.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 67);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(73, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "ZIPディレクトリ";
            // 
            // tbZipDirectory
            // 
            this.tbZipDirectory.Location = new System.Drawing.Point(102, 64);
            this.tbZipDirectory.Name = "tbZipDirectory";
            this.tbZipDirectory.Size = new System.Drawing.Size(502, 20);
            this.tbZipDirectory.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 119);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(77, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "mod ref. server";
            // 
            // tbModRefServer
            // 
            this.tbModRefServer.Location = new System.Drawing.Point(102, 116);
            this.tbModRefServer.Name = "tbModRefServer";
            this.tbModRefServer.Size = new System.Drawing.Size(502, 20);
            this.tbModRefServer.TabIndex = 7;
            // 
            // tbZipRegexp
            // 
            this.tbZipRegexp.Location = new System.Drawing.Point(102, 194);
            this.tbZipRegexp.Name = "tbZipRegexp";
            this.tbZipRegexp.Size = new System.Drawing.Size(502, 20);
            this.tbZipRegexp.TabIndex = 9;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 197);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(72, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "ZIP正規表現";
            // 
            // tbArcnamesServer
            // 
            this.tbArcnamesServer.Location = new System.Drawing.Point(101, 142);
            this.tbArcnamesServer.Name = "tbArcnamesServer";
            this.tbArcnamesServer.Size = new System.Drawing.Size(502, 20);
            this.tbArcnamesServer.TabIndex = 11;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(11, 145);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(85, 13);
            this.label5.TabIndex = 10;
            this.label5.Text = "arcnames server";
            // 
            // tbWorkPath
            // 
            this.tbWorkPath.Location = new System.Drawing.Point(102, 90);
            this.tbWorkPath.Name = "tbWorkPath";
            this.tbWorkPath.Size = new System.Drawing.Size(502, 20);
            this.tbWorkPath.TabIndex = 13;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 93);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(60, 13);
            this.label6.TabIndex = 12;
            this.label6.Text = "ZIP展開先";
            // 
            // checkBoxLookupModRef
            // 
            this.checkBoxLookupModRef.AutoSize = true;
            this.checkBoxLookupModRef.Location = new System.Drawing.Point(109, 370);
            this.checkBoxLookupModRef.Name = "checkBoxLookupModRef";
            this.checkBoxLookupModRef.Size = new System.Drawing.Size(310, 17);
            this.checkBoxLookupModRef.TabIndex = 14;
            this.checkBoxLookupModRef.Text = "常に3DCG Mods Reference Serverから最新情報を取得する";
            this.checkBoxLookupModRef.UseVisualStyleBackColor = true;
            // 
            // textBoxTagNameServer
            // 
            this.textBoxTagNameServer.Location = new System.Drawing.Point(101, 168);
            this.textBoxTagNameServer.Name = "textBoxTagNameServer";
            this.textBoxTagNameServer.Size = new System.Drawing.Size(502, 20);
            this.textBoxTagNameServer.TabIndex = 19;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(11, 171);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(85, 13);
            this.label8.TabIndex = 18;
            this.label8.Text = "tagnames server";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radioButtonBehaviorText);
            this.groupBox1.Controls.Add(this.radioButtonBehaviorImage);
            this.groupBox1.Controls.Add(this.radioButtonBehaviorServer);
            this.groupBox1.Controls.Add(this.radioButtonBehaviorNone);
            this.groupBox1.Location = new System.Drawing.Point(102, 246);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(269, 118);
            this.groupBox1.TabIndex = 20;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "ZIP選択時の動作";
            // 
            // radioButtonBehaviorText
            // 
            this.radioButtonBehaviorText.AutoSize = true;
            this.radioButtonBehaviorText.Location = new System.Drawing.Point(7, 89);
            this.radioButtonBehaviorText.Name = "radioButtonBehaviorText";
            this.radioButtonBehaviorText.Size = new System.Drawing.Size(244, 17);
            this.radioButtonBehaviorText.TabIndex = 3;
            this.radioButtonBehaviorText.TabStop = true;
            this.radioButtonBehaviorText.Text = "最初に見つけたテキストもしくは画像を表示する";
            this.radioButtonBehaviorText.UseVisualStyleBackColor = true;
            // 
            // radioButtonBehaviorImage
            // 
            this.radioButtonBehaviorImage.AutoSize = true;
            this.radioButtonBehaviorImage.Location = new System.Drawing.Point(7, 66);
            this.radioButtonBehaviorImage.Name = "radioButtonBehaviorImage";
            this.radioButtonBehaviorImage.Size = new System.Drawing.Size(244, 17);
            this.radioButtonBehaviorImage.TabIndex = 2;
            this.radioButtonBehaviorImage.TabStop = true;
            this.radioButtonBehaviorImage.Text = "最初に見つけた画像もしくはテキストを表示する";
            this.radioButtonBehaviorImage.UseVisualStyleBackColor = true;
            // 
            // radioButtonBehaviorServer
            // 
            this.radioButtonBehaviorServer.AutoSize = true;
            this.radioButtonBehaviorServer.Location = new System.Drawing.Point(7, 43);
            this.radioButtonBehaviorServer.Name = "radioButtonBehaviorServer";
            this.radioButtonBehaviorServer.Size = new System.Drawing.Size(234, 17);
            this.radioButtonBehaviorServer.TabIndex = 1;
            this.radioButtonBehaviorServer.TabStop = true;
            this.radioButtonBehaviorServer.Text = "3DCG Mods Reference Serverにアクセスする";
            this.radioButtonBehaviorServer.UseVisualStyleBackColor = true;
            // 
            // radioButtonBehaviorNone
            // 
            this.radioButtonBehaviorNone.AutoSize = true;
            this.radioButtonBehaviorNone.Location = new System.Drawing.Point(7, 20);
            this.radioButtonBehaviorNone.Name = "radioButtonBehaviorNone";
            this.radioButtonBehaviorNone.Size = new System.Drawing.Size(75, 17);
            this.radioButtonBehaviorNone.TabIndex = 0;
            this.radioButtonBehaviorNone.TabStop = true;
            this.radioButtonBehaviorNone.Text = "何もしない";
            this.radioButtonBehaviorNone.UseVisualStyleBackColor = true;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(12, 371);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(77, 13);
            this.label9.TabIndex = 21;
            this.label9.Text = "起動時の設定";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(12, 223);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(80, 13);
            this.label7.TabIndex = 15;
            this.label7.Text = "MOD正規表現";
            // 
            // textBoxModRegexp
            // 
            this.textBoxModRegexp.Location = new System.Drawing.Point(102, 220);
            this.textBoxModRegexp.Name = "textBoxModRegexp";
            this.textBoxModRegexp.Size = new System.Drawing.Size(502, 20);
            this.textBoxModRegexp.TabIndex = 16;
            // 
            // textBoxSaveFile
            // 
            this.textBoxSaveFile.Location = new System.Drawing.Point(102, 38);
            this.textBoxSaveFile.Name = "textBoxSaveFile";
            this.textBoxSaveFile.Size = new System.Drawing.Size(502, 20);
            this.textBoxSaveFile.TabIndex = 23;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(12, 41);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(86, 13);
            this.label10.TabIndex = 22;
            this.label10.Text = "TDCGディレクトリ";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(12, 394);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(56, 13);
            this.label11.TabIndex = 25;
            this.label11.Text = "カメラ設定";
            // 
            // checkBoxCameraReset
            // 
            this.checkBoxCameraReset.AutoSize = true;
            this.checkBoxCameraReset.Location = new System.Drawing.Point(109, 393);
            this.checkBoxCameraReset.Name = "checkBoxCameraReset";
            this.checkBoxCameraReset.Size = new System.Drawing.Size(229, 17);
            this.checkBoxCameraReset.TabIndex = 24;
            this.checkBoxCameraReset.Text = "新規TSOロード時、カメラ設定をリセットする";
            this.checkBoxCameraReset.UseVisualStyleBackColor = true;
            // 
            // EditSystemDatabase
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(615, 419);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.checkBoxCameraReset);
            this.Controls.Add(this.textBoxSaveFile);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.groupBox1);
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
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "EditSystemDatabase";
            this.Text = "初期設定";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
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
        private System.Windows.Forms.TextBox textBoxTagNameServer;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton radioButtonBehaviorText;
        private System.Windows.Forms.RadioButton radioButtonBehaviorImage;
        private System.Windows.Forms.RadioButton radioButtonBehaviorServer;
        private System.Windows.Forms.RadioButton radioButtonBehaviorNone;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox textBoxModRegexp;
        private System.Windows.Forms.TextBox textBoxSaveFile;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.CheckBox checkBoxCameraReset;
    }
}