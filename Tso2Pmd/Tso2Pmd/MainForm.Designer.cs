namespace Tso2Pmd
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.button_Clipping = new System.Windows.Forms.Button();
            this.button_Trans = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.t2POptionControl1 = new Tso2Pmd.T2POptionControl();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // button_Clipping
            // 
            this.button_Clipping.Enabled = false;
            this.button_Clipping.Location = new System.Drawing.Point(198, 322);
            this.button_Clipping.Name = "button_Clipping";
            this.button_Clipping.Size = new System.Drawing.Size(177, 34);
            this.button_Clipping.TabIndex = 23;
            this.button_Clipping.Text = "変換するメッシュを選択";
            this.button_Clipping.UseVisualStyleBackColor = true;
            this.button_Clipping.Click += new System.EventHandler(this.button_Clipping_Click);
            // 
            // button_Trans
            // 
            this.button_Trans.Enabled = false;
            this.button_Trans.Location = new System.Drawing.Point(12, 362);
            this.button_Trans.Name = "button_Trans";
            this.button_Trans.Size = new System.Drawing.Size(363, 45);
            this.button_Trans.TabIndex = 22;
            this.button_Trans.Text = "変換を実行（TSO -> PMD）";
            this.button_Trans.UseVisualStyleBackColor = true;
            this.button_Trans.Click += new System.EventHandler(this.button_Trans_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(12, 322);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(177, 34);
            this.button2.TabIndex = 21;
            this.button2.Text = "ビューアーを表示";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // panel1
            // 
            this.panel1.AllowDrop = true;
            this.panel1.BackColor = System.Drawing.Color.LightCyan;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.label3);
            this.panel1.Location = new System.Drawing.Point(12, 247);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(363, 69);
            this.panel1.TabIndex = 20;
            this.panel1.DragDrop += new System.Windows.Forms.DragEventHandler(this.panel1_DragDrop);
            this.panel1.DragLeave += new System.EventHandler(this.panel1_DragLeave);
            this.panel1.DragEnter += new System.Windows.Forms.DragEventHandler(this.panel1_DragEnter);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("MS UI Gothic", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label3.ForeColor = System.Drawing.Color.Black;
            this.label3.Location = new System.Drawing.Point(47, 14);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(267, 39);
            this.label3.TabIndex = 0;
            this.label3.Text = "ここにヘビーセーブデータか\r\nTSOファイル（通常女MOD，男MOD，背景）を\r\nドラッグ＆ドロップしてください";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // t2POptionControl1
            // 
            this.t2POptionControl1.Location = new System.Drawing.Point(12, 12);
            this.t2POptionControl1.Name = "t2POptionControl1";
            this.t2POptionControl1.Size = new System.Drawing.Size(363, 229);
            this.t2POptionControl1.TabIndex = 0;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(386, 419);
            this.Controls.Add(this.button_Clipping);
            this.Controls.Add(this.button_Trans);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.t2POptionControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.Text = "Tso2Pmd";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private T2POptionControl t2POptionControl1;
        private System.Windows.Forms.Button button_Clipping;
        private System.Windows.Forms.Button button_Trans;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label3;

    }
}