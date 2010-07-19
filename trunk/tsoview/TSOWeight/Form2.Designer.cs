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
            this.lbRot = new System.Windows.Forms.Label();
            this.btnRotZ = new TSOWeight.NotSelectableButton();
            this.btnRotY = new TSOWeight.NotSelectableButton();
            this.btnRotX = new TSOWeight.NotSelectableButton();
            this.btnTraZ = new TSOWeight.NotSelectableButton();
            this.btnTraY = new TSOWeight.NotSelectableButton();
            this.btnTraX = new TSOWeight.NotSelectableButton();
            this.lbTra = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lbRot
            // 
            this.lbRot.Location = new System.Drawing.Point(58, 9);
            this.lbRot.Name = "lbRot";
            this.lbRot.Size = new System.Drawing.Size(40, 12);
            this.lbRot.TabIndex = 0;
            this.lbRot.Text = "回転";
            this.lbRot.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // btnRotZ
            // 
            this.btnRotZ.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.btnRotZ.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRotZ.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.btnRotZ.ForeColor = System.Drawing.Color.Blue;
            this.btnRotZ.Location = new System.Drawing.Point(58, 86);
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
            this.btnRotY.Location = new System.Drawing.Point(58, 55);
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
            this.btnRotX.Location = new System.Drawing.Point(58, 24);
            this.btnRotX.Name = "btnRotX";
            this.btnRotX.Size = new System.Drawing.Size(40, 25);
            this.btnRotX.TabIndex = 1;
            this.btnRotX.Text = "X";
            this.btnRotX.UseVisualStyleBackColor = true;
            this.btnRotX.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Control_MouseMove);
            this.btnRotX.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Control_MouseDown);
            this.btnRotX.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Control_MouseUp);
            // 
            // btnTraZ
            // 
            this.btnTraZ.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.btnTraZ.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTraZ.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.btnTraZ.ForeColor = System.Drawing.Color.Blue;
            this.btnTraZ.Location = new System.Drawing.Point(12, 86);
            this.btnTraZ.Name = "btnTraZ";
            this.btnTraZ.Size = new System.Drawing.Size(40, 25);
            this.btnTraZ.TabIndex = 7;
            this.btnTraZ.Text = "Z";
            this.btnTraZ.UseVisualStyleBackColor = true;
            this.btnTraZ.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Control_MouseMove);
            this.btnTraZ.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Control_MouseDown);
            this.btnTraZ.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Control_MouseUp);
            // 
            // btnTraY
            // 
            this.btnTraY.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.btnTraY.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTraY.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.btnTraY.ForeColor = System.Drawing.Color.Green;
            this.btnTraY.Location = new System.Drawing.Point(12, 55);
            this.btnTraY.Name = "btnTraY";
            this.btnTraY.Size = new System.Drawing.Size(40, 25);
            this.btnTraY.TabIndex = 6;
            this.btnTraY.Text = "Y";
            this.btnTraY.UseVisualStyleBackColor = true;
            this.btnTraY.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Control_MouseMove);
            this.btnTraY.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Control_MouseDown);
            this.btnTraY.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Control_MouseUp);
            // 
            // btnTraX
            // 
            this.btnTraX.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.btnTraX.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTraX.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.btnTraX.ForeColor = System.Drawing.Color.Red;
            this.btnTraX.Location = new System.Drawing.Point(12, 24);
            this.btnTraX.Name = "btnTraX";
            this.btnTraX.Size = new System.Drawing.Size(40, 25);
            this.btnTraX.TabIndex = 5;
            this.btnTraX.Text = "X";
            this.btnTraX.UseVisualStyleBackColor = true;
            this.btnTraX.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Control_MouseMove);
            this.btnTraX.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Control_MouseDown);
            this.btnTraX.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Control_MouseUp);
            // 
            // lbTra
            // 
            this.lbTra.Location = new System.Drawing.Point(12, 9);
            this.lbTra.Name = "lbTra";
            this.lbTra.Size = new System.Drawing.Size(40, 12);
            this.lbTra.TabIndex = 4;
            this.lbTra.Text = "移動";
            this.lbTra.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // Form2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 263);
            this.Controls.Add(this.btnTraZ);
            this.Controls.Add(this.btnTraY);
            this.Controls.Add(this.btnTraX);
            this.Controls.Add(this.lbTra);
            this.Controls.Add(this.btnRotZ);
            this.Controls.Add(this.btnRotY);
            this.Controls.Add(this.btnRotX);
            this.Controls.Add(this.lbRot);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "Form2";
            this.Text = "Operation";
            this.TopMost = true;
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Control_MouseUp);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Control_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Control_MouseMove);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lbRot;
        private NotSelectableButton btnRotX;
        private NotSelectableButton btnRotY;
        private NotSelectableButton btnRotZ;
        private NotSelectableButton btnTraZ;
        private NotSelectableButton btnTraY;
        private NotSelectableButton btnTraX;
        private System.Windows.Forms.Label lbTra;

    }
}
