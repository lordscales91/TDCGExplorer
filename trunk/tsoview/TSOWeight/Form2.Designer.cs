namespace TSOWeight
{
    partial class Form2
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
            this.label1 = new System.Windows.Forms.Label();
            this.btnRotZ = new TSOWeight.NotSelectableButton();
            this.btnRotY = new TSOWeight.NotSelectableButton();
            this.btnRotX = new TSOWeight.NotSelectableButton();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "回転";
            // 
            // btnRotZ
            // 
            this.btnRotZ.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.btnRotZ.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRotZ.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.btnRotZ.ForeColor = System.Drawing.Color.Blue;
            this.btnRotZ.Location = new System.Drawing.Point(139, 3);
            this.btnRotZ.Name = "btnRotZ";
            this.btnRotZ.Size = new System.Drawing.Size(40, 25);
            this.btnRotZ.TabIndex = 3;
            this.btnRotZ.Text = "Z";
            this.btnRotZ.UseVisualStyleBackColor = true;
            this.btnRotZ.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Control_MouseMove);
            this.btnRotZ.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Control_MouseDown);
            this.btnRotZ.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Control_MouseUp);
            // 
            // btnRotY
            // 
            this.btnRotY.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.btnRotY.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRotY.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.btnRotY.ForeColor = System.Drawing.Color.Green;
            this.btnRotY.Location = new System.Drawing.Point(93, 3);
            this.btnRotY.Name = "btnRotY";
            this.btnRotY.Size = new System.Drawing.Size(40, 25);
            this.btnRotY.TabIndex = 2;
            this.btnRotY.Text = "Y";
            this.btnRotY.UseVisualStyleBackColor = true;
            this.btnRotY.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Control_MouseMove);
            this.btnRotY.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Control_MouseDown);
            this.btnRotY.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Control_MouseUp);
            // 
            // btnRotX
            // 
            this.btnRotX.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.btnRotX.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRotX.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.btnRotX.ForeColor = System.Drawing.Color.Red;
            this.btnRotX.Location = new System.Drawing.Point(47, 3);
            this.btnRotX.Name = "btnRotX";
            this.btnRotX.Size = new System.Drawing.Size(40, 25);
            this.btnRotX.TabIndex = 1;
            this.btnRotX.Text = "X";
            this.btnRotX.UseVisualStyleBackColor = true;
            this.btnRotX.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Control_MouseMove);
            this.btnRotX.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Control_MouseDown);
            this.btnRotX.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Control_MouseUp);
            // 
            // Form2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 263);
            this.Controls.Add(this.btnRotZ);
            this.Controls.Add(this.btnRotY);
            this.Controls.Add(this.btnRotX);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "Form2";
            this.Text = "Operation";
            this.TopMost = true;
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Control_MouseUp);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Control_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Control_MouseMove);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private NotSelectableButton btnRotX;
        private NotSelectableButton btnRotY;
        private NotSelectableButton btnRotZ;

    }
}
