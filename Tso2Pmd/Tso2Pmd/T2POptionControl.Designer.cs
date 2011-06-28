namespace Tso2Pmd
{
    partial class T2POptionControl
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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.label8 = new System.Windows.Forms.Label();
            this.textBox_ModelName = new System.Windows.Forms.TextBox();
            this.textBox_Comment = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.radioButton3 = new System.Windows.Forms.RadioButton();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.label2 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.textBox_Folder = new System.Windows.Forms.TextBox();
            this.button_Folder = new System.Windows.Forms.Button();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.label1 = new System.Windows.Forms.Label();
            this.cbUseSpheremap = new System.Windows.Forms.CheckBox();
            this.cbUniqueMaterial = new System.Windows.Forms.CheckBox();
            this.cbUseEdge = new System.Windows.Forms.CheckBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.checkedListBox1 = new System.Windows.Forms.CheckedListBox();
            this.rbOneBone = new System.Windows.Forms.RadioButton();
            this.rbHumanBone = new System.Windows.Forms.RadioButton();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.physicsControl1 = new Tso2Pmd.PhysicsControl();
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.taikeiControl1 = new Tso2Pmd.TaikeiControl();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.tabPage5.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage5);
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(361, 229);
            this.tabControl1.TabIndex = 18;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.label8);
            this.tabPage1.Controls.Add(this.textBox_ModelName);
            this.tabPage1.Controls.Add(this.textBox_Comment);
            this.tabPage1.Controls.Add(this.label7);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(353, 203);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "モデル名＆コメント";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(30, 64);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(126, 12);
            this.label8.TabIndex = 12;
            this.label8.Text = "コメント（127文字以内） ：";
            // 
            // textBox_ModelName
            // 
            this.textBox_ModelName.Location = new System.Drawing.Point(32, 33);
            this.textBox_ModelName.Name = "textBox_ModelName";
            this.textBox_ModelName.Size = new System.Drawing.Size(143, 19);
            this.textBox_ModelName.TabIndex = 10;
            this.textBox_ModelName.Text = "カスタム少女";
            // 
            // textBox_Comment
            // 
            this.textBox_Comment.Location = new System.Drawing.Point(32, 79);
            this.textBox_Comment.Multiline = true;
            this.textBox_Comment.Name = "textBox_Comment";
            this.textBox_Comment.Size = new System.Drawing.Size(290, 99);
            this.textBox_Comment.TabIndex = 11;
            this.textBox_Comment.Text = "PolyMo用モデルデータ : カスタム少女\r\n(物理演算対応モデル)\r\n\r\nモデル編集者 : ---\r\nMOD作成者 : ---\r\nデータ変換 : Tso2Pm" +
                "d Ver. 0.2.4\r\nCopyright : TechArts3D & ---";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(30, 18);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(118, 12);
            this.label7.TabIndex = 12;
            this.label7.Text = "モデル名(9文字以内) ：";
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.radioButton3);
            this.tabPage2.Controls.Add(this.radioButton2);
            this.tabPage2.Controls.Add(this.radioButton1);
            this.tabPage2.Controls.Add(this.label2);
            this.tabPage2.Controls.Add(this.label4);
            this.tabPage2.Controls.Add(this.textBox_Folder);
            this.tabPage2.Controls.Add(this.button_Folder);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(353, 203);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "出力フォルダ";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // radioButton3
            // 
            this.radioButton3.AutoSize = true;
            this.radioButton3.Location = new System.Drawing.Point(29, 74);
            this.radioButton3.Name = "radioButton3";
            this.radioButton3.Size = new System.Drawing.Size(91, 16);
            this.radioButton3.TabIndex = 10;
            this.radioButton3.Text = "フォルダを指定";
            this.radioButton3.UseVisualStyleBackColor = true;
            this.radioButton3.CheckedChanged += new System.EventHandler(this.radioButton3_CheckedChanged);
            // 
            // radioButton2
            // 
            this.radioButton2.AutoSize = true;
            this.radioButton2.Location = new System.Drawing.Point(29, 52);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(174, 16);
            this.radioButton2.TabIndex = 9;
            this.radioButton2.Text = "セーブファイルが存在するフォルダ";
            this.radioButton2.UseVisualStyleBackColor = true;
            this.radioButton2.CheckedChanged += new System.EventHandler(this.radioButton2_CheckedChanged);
            // 
            // radioButton1
            // 
            this.radioButton1.AutoSize = true;
            this.radioButton1.Checked = true;
            this.radioButton1.Location = new System.Drawing.Point(29, 30);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(270, 16);
            this.radioButton1.TabIndex = 8;
            this.radioButton1.TabStop = true;
            this.radioButton1.Text = "セーブファイルが存在するフォルダにサブフォルダを作成";
            this.radioButton1.UseVisualStyleBackColor = true;
            this.radioButton1.CheckedChanged += new System.EventHandler(this.radioButton1_CheckedChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.Color.Red;
            this.label2.Location = new System.Drawing.Point(37, 156);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(189, 12);
            this.label2.TabIndex = 7;
            this.label2.Text = "＊ ファイルは確認なしで上書きされます";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(46, 99);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(74, 12);
            this.label4.TabIndex = 2;
            this.label4.Text = "出力フォルダ ：";
            // 
            // textBox_Folder
            // 
            this.textBox_Folder.Location = new System.Drawing.Point(48, 114);
            this.textBox_Folder.Name = "textBox_Folder";
            this.textBox_Folder.Size = new System.Drawing.Size(192, 19);
            this.textBox_Folder.TabIndex = 1;
            // 
            // button_Folder
            // 
            this.button_Folder.Location = new System.Drawing.Point(246, 112);
            this.button_Folder.Name = "button_Folder";
            this.button_Folder.Size = new System.Drawing.Size(75, 23);
            this.button_Folder.TabIndex = 5;
            this.button_Folder.Text = "参照";
            this.button_Folder.UseVisualStyleBackColor = true;
            this.button_Folder.Click += new System.EventHandler(this.button_Folder_Click);
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.label1);
            this.tabPage4.Controls.Add(this.cbUseSpheremap);
            this.tabPage4.Controls.Add(this.cbUniqueMaterial);
            this.tabPage4.Controls.Add(this.cbUseEdge);
            this.tabPage4.Controls.Add(this.groupBox4);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(353, 203);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "オプション1";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(247, 58);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(84, 48);
            this.label1.TabIndex = 26;
            this.label1.Text = "＊　組み合わせ\r\nによって、\r\n不具合が生じる\r\n場合があります。";
            // 
            // cbUseSpheremap
            // 
            this.cbUseSpheremap.AutoSize = true;
            this.cbUseSpheremap.Checked = true;
            this.cbUseSpheremap.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbUseSpheremap.Location = new System.Drawing.Point(28, 126);
            this.cbUseSpheremap.Name = "cbUseSpheremap";
            this.cbUseSpheremap.Size = new System.Drawing.Size(303, 16);
            this.cbUseSpheremap.TabIndex = 34;
            this.cbUseSpheremap.Text = "スフィアマップを利用して、Toonテキスチャの色とびを補完する";
            this.cbUseSpheremap.UseVisualStyleBackColor = true;
            // 
            // cbUniqueMaterial
            // 
            this.cbUniqueMaterial.AutoSize = true;
            this.cbUniqueMaterial.Checked = true;
            this.cbUniqueMaterial.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbUniqueMaterial.Location = new System.Drawing.Point(28, 170);
            this.cbUniqueMaterial.Name = "cbUniqueMaterial";
            this.cbUniqueMaterial.Size = new System.Drawing.Size(236, 16);
            this.cbUniqueMaterial.TabIndex = 33;
            this.cbUniqueMaterial.Text = "同じ属性をもつ材質を一つの材質に統合する";
            this.cbUniqueMaterial.UseVisualStyleBackColor = true;
            // 
            // cbUseEdge
            // 
            this.cbUseEdge.AutoSize = true;
            this.cbUseEdge.Location = new System.Drawing.Point(28, 148);
            this.cbUseEdge.Name = "cbUseEdge";
            this.cbUseEdge.Size = new System.Drawing.Size(118, 16);
            this.cbUseEdge.TabIndex = 32;
            this.cbUseEdge.Text = "輪郭線＆影の表示";
            this.cbUseEdge.UseVisualStyleBackColor = true;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.checkedListBox1);
            this.groupBox4.Controls.Add(this.rbOneBone);
            this.groupBox4.Controls.Add(this.rbHumanBone);
            this.groupBox4.Location = new System.Drawing.Point(22, 17);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(213, 96);
            this.groupBox4.TabIndex = 31;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "ボーン構造";
            // 
            // checkedListBox1
            // 
            this.checkedListBox1.CheckOnClick = true;
            this.checkedListBox1.FormattingEnabled = true;
            this.checkedListBox1.Location = new System.Drawing.Point(94, 40);
            this.checkedListBox1.Name = "checkedListBox1";
            this.checkedListBox1.ScrollAlwaysVisible = true;
            this.checkedListBox1.Size = new System.Drawing.Size(107, 46);
            this.checkedListBox1.TabIndex = 25;
            this.checkedListBox1.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.checkedListBox1_ItemCheck);
            // 
            // rbOneBone
            // 
            this.rbOneBone.AutoSize = true;
            this.rbOneBone.Location = new System.Drawing.Point(12, 18);
            this.rbOneBone.Name = "rbOneBone";
            this.rbOneBone.Size = new System.Drawing.Size(58, 16);
            this.rbOneBone.TabIndex = 1;
            this.rbOneBone.Text = "1ボーン";
            this.rbOneBone.UseVisualStyleBackColor = true;
            this.rbOneBone.CheckedChanged += new System.EventHandler(this.radioButton_Bone1_CheckedChanged);
            // 
            // rbHumanBone
            // 
            this.rbHumanBone.AutoSize = true;
            this.rbHumanBone.Checked = true;
            this.rbHumanBone.Location = new System.Drawing.Point(12, 40);
            this.rbHumanBone.Name = "rbHumanBone";
            this.rbHumanBone.Size = new System.Drawing.Size(76, 16);
            this.rbHumanBone.TabIndex = 0;
            this.rbHumanBone.TabStop = true;
            this.rbHumanBone.Text = "人型ボーン";
            this.rbHumanBone.UseVisualStyleBackColor = true;
            this.rbHumanBone.CheckedChanged += new System.EventHandler(this.radioButton_Bone0_CheckedChanged);
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.physicsControl1);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(353, 203);
            this.tabPage3.TabIndex = 4;
            this.tabPage3.Text = "オプション2";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // physicsControl1
            // 
            this.physicsControl1.Location = new System.Drawing.Point(15, 5);
            this.physicsControl1.Name = "physicsControl1";
            this.physicsControl1.Size = new System.Drawing.Size(323, 193);
            this.physicsControl1.TabIndex = 0;
            // 
            // tabPage5
            // 
            this.tabPage5.Controls.Add(this.taikeiControl1);
            this.tabPage5.Location = new System.Drawing.Point(4, 22);
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage5.Size = new System.Drawing.Size(353, 203);
            this.tabPage5.TabIndex = 5;
            this.tabPage5.Text = "オプション3";
            this.tabPage5.UseVisualStyleBackColor = true;
            // 
            // taikeiControl1
            // 
            this.taikeiControl1.Location = new System.Drawing.Point(11, 10);
            this.taikeiControl1.Name = "taikeiControl1";
            this.taikeiControl1.Size = new System.Drawing.Size(330, 183);
            this.taikeiControl1.TabIndex = 0;
            // 
            // T2POptionControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tabControl1);
            this.Name = "T2POptionControl";
            this.Size = new System.Drawing.Size(361, 229);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.tabPage4.ResumeLayout(false);
            this.tabPage4.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.tabPage5.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox textBox_ModelName;
        private System.Windows.Forms.TextBox textBox_Comment;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.RadioButton radioButton3;
        private System.Windows.Forms.RadioButton radioButton2;
        private System.Windows.Forms.RadioButton radioButton1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBox_Folder;
        private System.Windows.Forms.Button button_Folder;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.TabPage tabPage5;
        private TaikeiControl taikeiControl1;
        private System.Windows.Forms.CheckBox cbUseSpheremap;
        private System.Windows.Forms.CheckBox cbUniqueMaterial;
        private System.Windows.Forms.CheckBox cbUseEdge;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.RadioButton rbOneBone;
        private System.Windows.Forms.RadioButton rbHumanBone;
        private PhysicsControl physicsControl1;
        private System.Windows.Forms.CheckedListBox checkedListBox1;
        private System.Windows.Forms.Label label1;
    }
}
