﻿namespace TPOEditor
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
            this.gvTPOCommands = new System.Windows.Forms.DataGridView();
            this.X = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Y = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Z = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.gvPortions = new System.Windows.Forms.DataGridView();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.tpoFileBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.typeDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tpoCommandBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.tpoNodeBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.ProportionName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.shortNameDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.gvTPONodes)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvTPOCommands)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvPortions)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tpoFileBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tpoCommandBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tpoNodeBindingSource)).BeginInit();
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
            this.shortNameDataGridViewTextBoxColumn});
            this.gvTPONodes.DataSource = this.tpoNodeBindingSource;
            this.gvTPONodes.Location = new System.Drawing.Point(12, 168);
            this.gvTPONodes.Name = "gvTPONodes";
            this.gvTPONodes.RowTemplate.Height = 21;
            this.gvTPONodes.Size = new System.Drawing.Size(240, 383);
            this.gvTPONodes.TabIndex = 0;
            this.gvTPONodes.SelectionChanged += new System.EventHandler(this.gvTPONodes_SelectionChanged);
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
            // gvPortions
            // 
            this.gvPortions.AllowUserToAddRows = false;
            this.gvPortions.AutoGenerateColumns = false;
            this.gvPortions.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.gvPortions.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gvPortions.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ProportionName});
            this.gvPortions.DataSource = this.tpoFileBindingSource;
            this.gvPortions.Location = new System.Drawing.Point(12, 12);
            this.gvPortions.Name = "gvPortions";
            this.gvPortions.RowTemplate.Height = 21;
            this.gvPortions.Size = new System.Drawing.Size(240, 150);
            this.gvPortions.TabIndex = 2;
            this.gvPortions.SelectionChanged += new System.EventHandler(this.gvPortions_SelectionChanged);
            // 
            // timer1
            // 
            this.timer1.Interval = 30;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // tpoFileBindingSource
            // 
            this.tpoFileBindingSource.DataSource = typeof(TDCG.TPOFile);
            // 
            // typeDataGridViewTextBoxColumn
            // 
            this.typeDataGridViewTextBoxColumn.DataPropertyName = "Type";
            this.typeDataGridViewTextBoxColumn.HeaderText = "Type";
            this.typeDataGridViewTextBoxColumn.Name = "typeDataGridViewTextBoxColumn";
            this.typeDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // tpoCommandBindingSource
            // 
            this.tpoCommandBindingSource.DataSource = typeof(TDCG.TPOCommand);
            // 
            // tpoNodeBindingSource
            // 
            this.tpoNodeBindingSource.DataSource = typeof(TDCG.TPONode);
            // 
            // ProportionName
            // 
            this.ProportionName.DataPropertyName = "ProportionName";
            this.ProportionName.HeaderText = "Proportion";
            this.ProportionName.Name = "ProportionName";
            this.ProportionName.ReadOnly = true;
            // 
            // shortNameDataGridViewTextBoxColumn
            // 
            this.shortNameDataGridViewTextBoxColumn.DataPropertyName = "ShortName";
            this.shortNameDataGridViewTextBoxColumn.HeaderText = "ShortName";
            this.shortNameDataGridViewTextBoxColumn.Name = "shortNameDataGridViewTextBoxColumn";
            this.shortNameDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 563);
            this.Controls.Add(this.gvPortions);
            this.Controls.Add(this.gvTPOCommands);
            this.Controls.Add(this.gvTPONodes);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.gvTPONodes)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvTPOCommands)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvPortions)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tpoFileBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tpoCommandBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tpoNodeBindingSource)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.BindingSource tpoNodeBindingSource;
        private System.Windows.Forms.BindingSource tpoCommandBindingSource;
        private System.Windows.Forms.DataGridView gvTPONodes;
        private System.Windows.Forms.DataGridView gvTPOCommands;
        private System.Windows.Forms.DataGridViewTextBoxColumn typeDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn vDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn angleDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn X;
        private System.Windows.Forms.DataGridViewTextBoxColumn Y;
        private System.Windows.Forms.DataGridViewTextBoxColumn Z;
        private System.Windows.Forms.DataGridView gvPortions;
        private System.Windows.Forms.BindingSource tpoFileBindingSource;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.DataGridViewTextBoxColumn ProportionName;
        private System.Windows.Forms.DataGridViewTextBoxColumn shortNameDataGridViewTextBoxColumn;
    }
}

