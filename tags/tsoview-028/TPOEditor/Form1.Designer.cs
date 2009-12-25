namespace TPOEditor
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

        /// <summary>
        /// デザイナ サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディタで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.gvTPONodes = new System.Windows.Forms.DataGridView();
            this.nameDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tpoNodeBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.gvTPOCommands = new System.Windows.Forms.DataGridView();
            this.typeDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.X = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Y = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Z = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tpoCommandBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.gvPortions = new System.Windows.Forms.DataGridView();
            this.proportionNameDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tpoFileBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.gvCommands = new System.Windows.Forms.DataGridView();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cbInverseScaleOnChildren = new System.Windows.Forms.CheckBox();
            this.btnSave = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.gvTPONodes)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tpoNodeBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvTPOCommands)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tpoCommandBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvPortions)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tpoFileBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvCommands)).BeginInit();
            this.SuspendLayout();
            // 
            // gvTPONodes
            // 
            this.gvTPONodes.AllowUserToAddRows = false;
            this.gvTPONodes.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.gvTPONodes.AutoGenerateColumns = false;
            this.gvTPONodes.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.gvTPONodes.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gvTPONodes.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.nameDataGridViewTextBoxColumn});
            this.gvTPONodes.DataSource = this.tpoNodeBindingSource;
            this.gvTPONodes.Location = new System.Drawing.Point(12, 168);
            this.gvTPONodes.Name = "gvTPONodes";
            this.gvTPONodes.RowTemplate.Height = 21;
            this.gvTPONodes.Size = new System.Drawing.Size(240, 383);
            this.gvTPONodes.TabIndex = 0;
            this.gvTPONodes.SelectionChanged += new System.EventHandler(this.gvTPONodes_SelectionChanged);
            // 
            // nameDataGridViewTextBoxColumn
            // 
            this.nameDataGridViewTextBoxColumn.DataPropertyName = "Name";
            this.nameDataGridViewTextBoxColumn.HeaderText = "Name";
            this.nameDataGridViewTextBoxColumn.Name = "nameDataGridViewTextBoxColumn";
            this.nameDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // tpoNodeBindingSource
            // 
            this.tpoNodeBindingSource.DataSource = typeof(TDCG.TPONode);
            // 
            // gvTPOCommands
            // 
            this.gvTPOCommands.AllowUserToAddRows = false;
            this.gvTPOCommands.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.gvTPOCommands.AutoGenerateColumns = false;
            this.gvTPOCommands.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.gvTPOCommands.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gvTPOCommands.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.typeDataGridViewTextBoxColumn,
            this.X,
            this.Y,
            this.Z});
            this.gvTPOCommands.DataSource = this.tpoCommandBindingSource;
            this.gvTPOCommands.Location = new System.Drawing.Point(532, 12);
            this.gvTPOCommands.Name = "gvTPOCommands";
            this.gvTPOCommands.RowTemplate.Height = 21;
            this.gvTPOCommands.Size = new System.Drawing.Size(240, 150);
            this.gvTPOCommands.TabIndex = 1;
            // 
            // typeDataGridViewTextBoxColumn
            // 
            this.typeDataGridViewTextBoxColumn.DataPropertyName = "Type";
            this.typeDataGridViewTextBoxColumn.HeaderText = "Type";
            this.typeDataGridViewTextBoxColumn.Name = "typeDataGridViewTextBoxColumn";
            this.typeDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // X
            // 
            this.X.DataPropertyName = "X";
            this.X.HeaderText = "X";
            this.X.Name = "X";
            this.X.ReadOnly = true;
            // 
            // Y
            // 
            this.Y.DataPropertyName = "Y";
            this.Y.HeaderText = "Y";
            this.Y.Name = "Y";
            this.Y.ReadOnly = true;
            // 
            // Z
            // 
            this.Z.DataPropertyName = "Z";
            this.Z.HeaderText = "Z";
            this.Z.Name = "Z";
            this.Z.ReadOnly = true;
            // 
            // tpoCommandBindingSource
            // 
            this.tpoCommandBindingSource.DataSource = typeof(TDCG.TPOCommand);
            // 
            // gvPortions
            // 
            this.gvPortions.AllowUserToAddRows = false;
            this.gvPortions.AutoGenerateColumns = false;
            this.gvPortions.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.gvPortions.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gvPortions.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.proportionNameDataGridViewTextBoxColumn});
            this.gvPortions.DataSource = this.tpoFileBindingSource;
            this.gvPortions.Location = new System.Drawing.Point(12, 12);
            this.gvPortions.Name = "gvPortions";
            this.gvPortions.RowTemplate.Height = 21;
            this.gvPortions.Size = new System.Drawing.Size(240, 150);
            this.gvPortions.TabIndex = 2;
            this.gvPortions.SelectionChanged += new System.EventHandler(this.gvPortions_SelectionChanged);
            // 
            // proportionNameDataGridViewTextBoxColumn
            // 
            this.proportionNameDataGridViewTextBoxColumn.DataPropertyName = "ProportionName";
            this.proportionNameDataGridViewTextBoxColumn.HeaderText = "ProportionName";
            this.proportionNameDataGridViewTextBoxColumn.Name = "proportionNameDataGridViewTextBoxColumn";
            this.proportionNameDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // tpoFileBindingSource
            // 
            this.tpoFileBindingSource.DataSource = typeof(TDCG.TPOFile);
            // 
            // timer1
            // 
            this.timer1.Interval = 30;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // gvCommands
            // 
            this.gvCommands.AllowUserToAddRows = false;
            this.gvCommands.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.gvCommands.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.gvCommands.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gvCommands.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.Column2,
            this.Column3,
            this.Column4});
            this.gvCommands.Location = new System.Drawing.Point(532, 168);
            this.gvCommands.Name = "gvCommands";
            this.gvCommands.RowTemplate.Height = 21;
            this.gvCommands.Size = new System.Drawing.Size(240, 150);
            this.gvCommands.TabIndex = 3;
            this.gvCommands.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.gvCommands_MouseWheel);
            this.gvCommands.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.gvCommands_CellEndEdit);
            // 
            // Column1
            // 
            this.Column1.HeaderText = "Type";
            this.Column1.Name = "Column1";
            // 
            // Column2
            // 
            this.Column2.HeaderText = "X";
            this.Column2.Name = "Column2";
            // 
            // Column3
            // 
            this.Column3.HeaderText = "Y";
            this.Column3.Name = "Column3";
            // 
            // Column4
            // 
            this.Column4.HeaderText = "Z";
            this.Column4.Name = "Column4";
            // 
            // cbInverseScaleOnChildren
            // 
            this.cbInverseScaleOnChildren.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cbInverseScaleOnChildren.AutoSize = true;
            this.cbInverseScaleOnChildren.Location = new System.Drawing.Point(532, 324);
            this.cbInverseScaleOnChildren.Name = "cbInverseScaleOnChildren";
            this.cbInverseScaleOnChildren.Size = new System.Drawing.Size(152, 16);
            this.cbInverseScaleOnChildren.TabIndex = 4;
            this.cbInverseScaleOnChildren.Text = "&inverse scale on children";
            this.cbInverseScaleOnChildren.UseVisualStyleBackColor = true;
            this.cbInverseScaleOnChildren.CheckedChanged += new System.EventHandler(this.cbInverseScaleOnChildren_CheckedChanged);
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.Location = new System.Drawing.Point(697, 528);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 5;
            this.btnSave.Text = "&Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 563);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.cbInverseScaleOnChildren);
            this.Controls.Add(this.gvCommands);
            this.Controls.Add(this.gvPortions);
            this.Controls.Add(this.gvTPOCommands);
            this.Controls.Add(this.gvTPONodes);
            this.Name = "Form1";
            this.Text = "tpo editor";
            ((System.ComponentModel.ISupportInitialize)(this.gvTPONodes)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tpoNodeBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvTPOCommands)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tpoCommandBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvPortions)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tpoFileBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvCommands)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.BindingSource tpoNodeBindingSource;
        private System.Windows.Forms.BindingSource tpoCommandBindingSource;
        private System.Windows.Forms.DataGridView gvTPONodes;
        private System.Windows.Forms.DataGridView gvTPOCommands;
        private System.Windows.Forms.DataGridViewTextBoxColumn typeDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn X;
        private System.Windows.Forms.DataGridViewTextBoxColumn Y;
        private System.Windows.Forms.DataGridViewTextBoxColumn Z;
        private System.Windows.Forms.DataGridView gvPortions;
        private System.Windows.Forms.BindingSource tpoFileBindingSource;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.DataGridViewTextBoxColumn nameDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn proportionNameDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridView gvCommands;
        private System.Windows.Forms.CheckBox cbInverseScaleOnChildren;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column3;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column4;
        private System.Windows.Forms.Button btnSave;
    }
}

