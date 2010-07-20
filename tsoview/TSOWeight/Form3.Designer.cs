namespace TSOWeight
{
    partial class Form3
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
            this.lbWeight = new System.Windows.Forms.Label();
            this.lbRadius = new System.Windows.Forms.Label();
            this.lvSkinWeights = new System.Windows.Forms.ListView();
            this.columnHeader4 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader5 = new System.Windows.Forms.ColumnHeader();
            this.lbSkinWeights = new System.Windows.Forms.Label();
            this.tbWeight = new System.Windows.Forms.TrackBar();
            this.btnDraw = new System.Windows.Forms.Button();
            this.tbRadius = new System.Windows.Forms.TrackBar();
            ((System.ComponentModel.ISupportInitialize)(this.tbWeight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbRadius)).BeginInit();
            this.SuspendLayout();
            // 
            // lbWeight
            // 
            this.lbWeight.Location = new System.Drawing.Point(12, 147);
            this.lbWeight.Name = "lbWeight";
            this.lbWeight.Size = new System.Drawing.Size(120, 12);
            this.lbWeight.TabIndex = 31;
            this.lbWeight.Text = "加算ウェイト";
            // 
            // lbRadius
            // 
            this.lbRadius.Location = new System.Drawing.Point(12, 195);
            this.lbRadius.Name = "lbRadius";
            this.lbRadius.Size = new System.Drawing.Size(120, 12);
            this.lbRadius.TabIndex = 33;
            this.lbRadius.Text = "半径";
            // 
            // lvSkinWeights
            // 
            this.lvSkinWeights.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader4,
            this.columnHeader5});
            this.lvSkinWeights.FullRowSelect = true;
            this.lvSkinWeights.GridLines = true;
            this.lvSkinWeights.HideSelection = false;
            this.lvSkinWeights.Location = new System.Drawing.Point(12, 24);
            this.lvSkinWeights.MultiSelect = false;
            this.lvSkinWeights.Name = "lvSkinWeights";
            this.lvSkinWeights.Size = new System.Drawing.Size(134, 120);
            this.lvSkinWeights.TabIndex = 30;
            this.lvSkinWeights.UseCompatibleStateImageBehavior = false;
            this.lvSkinWeights.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "ボーン";
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "ウェイト";
            // 
            // lbSkinWeights
            // 
            this.lbSkinWeights.Location = new System.Drawing.Point(12, 9);
            this.lbSkinWeights.Name = "lbSkinWeights";
            this.lbSkinWeights.Size = new System.Drawing.Size(120, 12);
            this.lbSkinWeights.TabIndex = 29;
            this.lbSkinWeights.Text = "頂点ウェイト";
            // 
            // tbWeight
            // 
            this.tbWeight.Location = new System.Drawing.Point(14, 162);
            this.tbWeight.Maximum = 20;
            this.tbWeight.Name = "tbWeight";
            this.tbWeight.Size = new System.Drawing.Size(132, 45);
            this.tbWeight.TabIndex = 32;
            this.tbWeight.Value = 2;
            // 
            // btnDraw
            // 
            this.btnDraw.Location = new System.Drawing.Point(12, 261);
            this.btnDraw.Name = "btnDraw";
            this.btnDraw.Size = new System.Drawing.Size(134, 23);
            this.btnDraw.TabIndex = 35;
            this.btnDraw.Text = "塗る(&D)";
            this.btnDraw.UseVisualStyleBackColor = true;
            // 
            // tbRadius
            // 
            this.tbRadius.Location = new System.Drawing.Point(14, 210);
            this.tbRadius.Maximum = 20;
            this.tbRadius.Name = "tbRadius";
            this.tbRadius.Size = new System.Drawing.Size(132, 45);
            this.tbRadius.TabIndex = 34;
            this.tbRadius.Value = 5;
            // 
            // Form3
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(158, 313);
            this.Controls.Add(this.lbWeight);
            this.Controls.Add(this.lbRadius);
            this.Controls.Add(this.lvSkinWeights);
            this.Controls.Add(this.lbSkinWeights);
            this.Controls.Add(this.tbWeight);
            this.Controls.Add(this.btnDraw);
            this.Controls.Add(this.tbRadius);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "Form3";
            this.Text = "ウェイト操作";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form3_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.tbWeight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbRadius)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lbWeight;
        private System.Windows.Forms.Label lbRadius;
        private System.Windows.Forms.ListView lvSkinWeights;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.Label lbSkinWeights;
        private System.Windows.Forms.TrackBar tbWeight;
        private System.Windows.Forms.Button btnDraw;
        private System.Windows.Forms.TrackBar tbRadius;
    }
}
