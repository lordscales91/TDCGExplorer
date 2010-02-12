namespace TSOView
{

partial class TSOForm
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
            if (disposing)
            {
                viewer.Dispose();
            }
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナで生成されたコード

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.gvSkinWeights = new System.Windows.Forms.DataGridView();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnShowFigureForm = new System.Windows.Forms.Button();
            this.btnGainSkinWeight = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.gvSkinWeights)).BeginInit();
            this.SuspendLayout();
            // 
            // timer1
            // 
            this.timer1.Interval = 16;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // gvSkinWeights
            // 
            this.gvSkinWeights.AllowUserToAddRows = false;
            this.gvSkinWeights.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gvSkinWeights.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.Column2});
            this.gvSkinWeights.Location = new System.Drawing.Point(12, 41);
            this.gvSkinWeights.Name = "gvSkinWeights";
            this.gvSkinWeights.RowTemplate.Height = 21;
            this.gvSkinWeights.Size = new System.Drawing.Size(260, 150);
            this.gvSkinWeights.TabIndex = 0;
            // 
            // Column1
            // 
            this.Column1.HeaderText = "BoneIndex";
            this.Column1.Name = "Column1";
            // 
            // Column2
            // 
            this.Column2.HeaderText = "Weight";
            this.Column2.Name = "Column2";
            // 
            // btnShowFigureForm
            // 
            this.btnShowFigureForm.Location = new System.Drawing.Point(12, 12);
            this.btnShowFigureForm.Name = "btnShowFigureForm";
            this.btnShowFigureForm.Size = new System.Drawing.Size(260, 23);
            this.btnShowFigureForm.TabIndex = 1;
            this.btnShowFigureForm.Text = "Show FigureForm";
            this.btnShowFigureForm.UseVisualStyleBackColor = true;
            this.btnShowFigureForm.Click += new System.EventHandler(this.btnShowFigureForm_Click);
            // 
            // btnGainSkinWeight
            // 
            this.btnGainSkinWeight.Location = new System.Drawing.Point(12, 197);
            this.btnGainSkinWeight.Name = "btnGainSkinWeight";
            this.btnGainSkinWeight.Size = new System.Drawing.Size(260, 23);
            this.btnGainSkinWeight.TabIndex = 2;
            this.btnGainSkinWeight.Text = "Gain SkinWeight";
            this.btnGainSkinWeight.UseVisualStyleBackColor = true;
            this.btnGainSkinWeight.Click += new System.EventHandler(this.btnGainSkinWeight_Click);
            // 
            // TSOForm
            // 
            this.ClientSize = new System.Drawing.Size(284, 263);
            this.Controls.Add(this.btnGainSkinWeight);
            this.Controls.Add(this.btnShowFigureForm);
            this.Controls.Add(this.gvSkinWeights);
            this.Name = "TSOForm";
            ((System.ComponentModel.ISupportInitialize)(this.gvSkinWeights)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.DataGridView gvSkinWeights;
        private System.Windows.Forms.Button btnShowFigureForm;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.Button btnGainSkinWeight;
    }
}
