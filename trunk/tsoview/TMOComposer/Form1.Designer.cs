﻿namespace TMOComposer
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
            this.lvPoses = new System.Windows.Forms.ListView();
            this.ilPoses = new System.Windows.Forms.ImageList(this.components);
            this.btnGetPoses = new System.Windows.Forms.Button();
            this.gvTMOAnimItems = new System.Windows.Forms.DataGridView();
            this.btnAnimate = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.btnUp = new System.Windows.Forms.Button();
            this.btnDown = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnRec = new System.Windows.Forms.Button();
            this.gvFigures = new System.Windows.Forms.DataGridView();
            this.btnAdd = new System.Windows.Forms.Button();
            this.pngSaveItemBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.poseFileDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lengthDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tmoAnimItemBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.fileDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.gvTMOAnimItems)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvFigures)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pngSaveItemBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tmoAnimItemBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // lvPoses
            // 
            this.lvPoses.LargeImageList = this.ilPoses;
            this.lvPoses.Location = new System.Drawing.Point(796, 12);
            this.lvPoses.Name = "lvPoses";
            this.lvPoses.Size = new System.Drawing.Size(200, 710);
            this.lvPoses.TabIndex = 0;
            this.lvPoses.UseCompatibleStateImageBehavior = false;
            this.lvPoses.DoubleClick += new System.EventHandler(this.lvPoses_DoubleClick);
            this.lvPoses.KeyDown += new System.Windows.Forms.KeyEventHandler(this.listView1_KeyDown);
            // 
            // ilPoses
            // 
            this.ilPoses.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.ilPoses.ImageSize = new System.Drawing.Size(128, 128);
            this.ilPoses.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // btnGetPoses
            // 
            this.btnGetPoses.Location = new System.Drawing.Point(921, 728);
            this.btnGetPoses.Name = "btnGetPoses";
            this.btnGetPoses.Size = new System.Drawing.Size(75, 23);
            this.btnGetPoses.TabIndex = 1;
            this.btnGetPoses.Text = "&Get poses";
            this.btnGetPoses.UseVisualStyleBackColor = true;
            this.btnGetPoses.Click += new System.EventHandler(this.btnGetPoses_Click);
            // 
            // gvTMOAnimItems
            // 
            this.gvTMOAnimItems.AllowUserToAddRows = false;
            this.gvTMOAnimItems.AutoGenerateColumns = false;
            this.gvTMOAnimItems.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.gvTMOAnimItems.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gvTMOAnimItems.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.poseFileDataGridViewTextBoxColumn,
            this.lengthDataGridViewTextBoxColumn});
            this.gvTMOAnimItems.DataSource = this.tmoAnimItemBindingSource;
            this.gvTMOAnimItems.Location = new System.Drawing.Point(12, 168);
            this.gvTMOAnimItems.Name = "gvTMOAnimItems";
            this.gvTMOAnimItems.RowTemplate.Height = 21;
            this.gvTMOAnimItems.Size = new System.Drawing.Size(200, 554);
            this.gvTMOAnimItems.TabIndex = 2;
            // 
            // btnAnimate
            // 
            this.btnAnimate.Location = new System.Drawing.Point(137, 728);
            this.btnAnimate.Name = "btnAnimate";
            this.btnAnimate.Size = new System.Drawing.Size(75, 23);
            this.btnAnimate.TabIndex = 4;
            this.btnAnimate.Text = "&Animate";
            this.btnAnimate.UseVisualStyleBackColor = true;
            this.btnAnimate.Click += new System.EventHandler(this.btnAnimate_Click);
            // 
            // timer1
            // 
            this.timer1.Interval = 30;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // btnUp
            // 
            this.btnUp.Location = new System.Drawing.Point(218, 168);
            this.btnUp.Name = "btnUp";
            this.btnUp.Size = new System.Drawing.Size(50, 23);
            this.btnUp.TabIndex = 5;
            this.btnUp.Text = "&Up";
            this.btnUp.UseVisualStyleBackColor = true;
            this.btnUp.Click += new System.EventHandler(this.btnUp_Click);
            // 
            // btnDown
            // 
            this.btnDown.Location = new System.Drawing.Point(218, 197);
            this.btnDown.Name = "btnDown";
            this.btnDown.Size = new System.Drawing.Size(50, 23);
            this.btnDown.TabIndex = 6;
            this.btnDown.Text = "&Down";
            this.btnDown.UseVisualStyleBackColor = true;
            this.btnDown.Click += new System.EventHandler(this.btnDown_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.Location = new System.Drawing.Point(218, 226);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(50, 23);
            this.btnDelete.TabIndex = 7;
            this.btnDelete.Text = "&Del";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnRec
            // 
            this.btnRec.Location = new System.Drawing.Point(218, 12);
            this.btnRec.Name = "btnRec";
            this.btnRec.Size = new System.Drawing.Size(50, 23);
            this.btnRec.TabIndex = 8;
            this.btnRec.Text = "&Rec";
            this.btnRec.UseVisualStyleBackColor = true;
            this.btnRec.Click += new System.EventHandler(this.btnRec_Click);
            // 
            // gvFigures
            // 
            this.gvFigures.AllowUserToAddRows = false;
            this.gvFigures.AllowUserToDeleteRows = false;
            this.gvFigures.AutoGenerateColumns = false;
            this.gvFigures.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.gvFigures.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gvFigures.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.fileDataGridViewTextBoxColumn});
            this.gvFigures.DataSource = this.pngSaveItemBindingSource;
            this.gvFigures.Location = new System.Drawing.Point(12, 12);
            this.gvFigures.Name = "gvFigures";
            this.gvFigures.ReadOnly = true;
            this.gvFigures.RowTemplate.Height = 21;
            this.gvFigures.Size = new System.Drawing.Size(200, 150);
            this.gvFigures.TabIndex = 9;
            this.gvFigures.SelectionChanged += new System.EventHandler(this.gvFigures_SelectionChanged);
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(218, 41);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(50, 23);
            this.btnAdd.TabIndex = 10;
            this.btnAdd.Text = "&Add";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // pngSaveItemBindingSource
            // 
            this.pngSaveItemBindingSource.DataSource = typeof(TMOComposer.PngSaveItem);
            // 
            // poseFileDataGridViewTextBoxColumn
            // 
            this.poseFileDataGridViewTextBoxColumn.DataPropertyName = "PoseFile";
            this.poseFileDataGridViewTextBoxColumn.HeaderText = "PoseFile";
            this.poseFileDataGridViewTextBoxColumn.Name = "poseFileDataGridViewTextBoxColumn";
            // 
            // lengthDataGridViewTextBoxColumn
            // 
            this.lengthDataGridViewTextBoxColumn.DataPropertyName = "Length";
            this.lengthDataGridViewTextBoxColumn.HeaderText = "Length";
            this.lengthDataGridViewTextBoxColumn.Name = "lengthDataGridViewTextBoxColumn";
            // 
            // tmoAnimItemBindingSource
            // 
            this.tmoAnimItemBindingSource.DataSource = typeof(TMOComposer.TMOAnimItem);
            // 
            // fileDataGridViewTextBoxColumn
            // 
            this.fileDataGridViewTextBoxColumn.DataPropertyName = "File";
            this.fileDataGridViewTextBoxColumn.HeaderText = "SaveFile";
            this.fileDataGridViewTextBoxColumn.Name = "fileDataGridViewTextBoxColumn";
            this.fileDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1008, 763);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.gvFigures);
            this.Controls.Add(this.btnRec);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.btnDown);
            this.Controls.Add(this.btnUp);
            this.Controls.Add(this.btnAnimate);
            this.Controls.Add(this.gvTMOAnimItems);
            this.Controls.Add(this.btnGetPoses);
            this.Controls.Add(this.lvPoses);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.gvTMOAnimItems)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvFigures)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pngSaveItemBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tmoAnimItemBindingSource)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView lvPoses;
        private System.Windows.Forms.ImageList ilPoses;
        private System.Windows.Forms.Button btnGetPoses;
        private System.Windows.Forms.DataGridView gvTMOAnimItems;
        private System.Windows.Forms.Button btnAnimate;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Button btnUp;
        private System.Windows.Forms.Button btnDown;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.BindingSource tmoAnimItemBindingSource;
        private System.Windows.Forms.DataGridViewTextBoxColumn poseFileDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn lengthDataGridViewTextBoxColumn;
        private System.Windows.Forms.Button btnRec;
        private System.Windows.Forms.DataGridView gvFigures;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.BindingSource pngSaveItemBindingSource;
        private System.Windows.Forms.DataGridViewTextBoxColumn fileDataGridViewTextBoxColumn;
    }
}

