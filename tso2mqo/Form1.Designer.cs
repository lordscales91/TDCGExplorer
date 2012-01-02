namespace Tso2MqoGui
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
            if(disposing && (components != null))
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
            this.button1 = new System.Windows.Forms.Button();
            this.tbPath = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.rbBoneRokDeBone = new System.Windows.Forms.RadioButton();
            this.rbBoneNone = new System.Windows.Forms.RadioButton();
            this.cbCopyTSO = new System.Windows.Forms.CheckBox();
            this.cbMakeSub = new System.Windows.Forms.CheckBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.cbShowMaterials = new System.Windows.Forms.CheckBox();
            this.gbBone = new System.Windows.Forms.GroupBox();
            this.btnDeselectAll = new System.Windows.Forms.Button();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.btnSelectAll = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.btnAssign = new System.Windows.Forms.Button();
            this.tvBones = new System.Windows.Forms.TreeView();
            this.lvObjects = new System.Windows.Forms.ListView();
            this.chObjects1 = new System.Windows.Forms.ColumnHeader();
            this.chObjects2 = new System.Windows.Forms.ColumnHeader();
            this.label6 = new System.Windows.Forms.Label();
            this.btnGenerate = new System.Windows.Forms.Button();
            this.btnTsoFile = new System.Windows.Forms.Button();
            this.btnTsoFileRef = new System.Windows.Forms.Button();
            this.rbOneBone = new System.Windows.Forms.RadioButton();
            this.rbRefBone = new System.Windows.Forms.RadioButton();
            this.tbTsoFile = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.tbTsoFileRef = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.btnMqoFile = new System.Windows.Forms.Button();
            this.tbMqoFile = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.btnMergeTso = new System.Windows.Forms.Button();
            this.tbMergeTso = new System.Windows.Forms.TextBox();
            this.btnMerge = new System.Windows.Forms.Button();
            this.label9 = new System.Windows.Forms.Label();
            this.btnMergeReset = new System.Windows.Forms.Button();
            this.btnMergeDel = new System.Windows.Forms.Button();
            this.btnMergeAdd = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.tvMerge = new System.Windows.Forms.TreeView();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.gbBone.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(407, 4);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 3;
            this.button1.Text = "&Ref";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // tbPath
            // 
            this.tbPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tbPath.Location = new System.Drawing.Point(52, 6);
            this.tbPath.Name = "tbPath";
            this.tbPath.Size = new System.Drawing.Size(349, 19);
            this.tbPath.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(40, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "&OutDir:";
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label2.Location = new System.Drawing.Point(6, 94);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(476, 177);
            this.label2.TabIndex = 8;
            this.label2.Text = "Drop TSO File Here!";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Location = new System.Drawing.Point(12, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(496, 300);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.Color.LightSteelBlue;
            this.tabPage1.Controls.Add(this.rbBoneRokDeBone);
            this.tabPage1.Controls.Add(this.rbBoneNone);
            this.tabPage1.Controls.Add(this.cbCopyTSO);
            this.tabPage1.Controls.Add(this.cbMakeSub);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Controls.Add(this.label2);
            this.tabPage1.Controls.Add(this.button1);
            this.tabPage1.Controls.Add(this.tbPath);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(488, 274);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Tso->Mqo";
            // 
            // rbBoneRokDeBone
            // 
            this.rbBoneRokDeBone.AutoSize = true;
            this.rbBoneRokDeBone.Location = new System.Drawing.Point(126, 75);
            this.rbBoneRokDeBone.Name = "rbBoneRokDeBone";
            this.rbBoneRokDeBone.Size = new System.Drawing.Size(170, 16);
            this.rbBoneRokDeBone.TabIndex = 7;
            this.rbBoneRokDeBone.TabStop = true;
            this.rbBoneRokDeBone.Text = "RokDeBone形式のボーン作成";
            this.rbBoneRokDeBone.UseVisualStyleBackColor = true;
            // 
            // rbBoneNone
            // 
            this.rbBoneNone.AutoSize = true;
            this.rbBoneNone.Location = new System.Drawing.Point(6, 75);
            this.rbBoneNone.Name = "rbBoneNone";
            this.rbBoneNone.Size = new System.Drawing.Size(114, 16);
            this.rbBoneNone.TabIndex = 6;
            this.rbBoneNone.TabStop = true;
            this.rbBoneNone.Text = "ボーンを作成しない";
            this.rbBoneNone.UseVisualStyleBackColor = true;
            // 
            // cbCopyTSO
            // 
            this.cbCopyTSO.AutoSize = true;
            this.cbCopyTSO.Location = new System.Drawing.Point(6, 53);
            this.cbCopyTSO.Name = "cbCopyTSO";
            this.cbCopyTSO.Size = new System.Drawing.Size(189, 16);
            this.cbCopyTSO.TabIndex = 5;
            this.cbCopyTSO.Text = "TSOファイルを同じ場所にコピーする";
            this.cbCopyTSO.UseVisualStyleBackColor = true;
            // 
            // cbMakeSub
            // 
            this.cbMakeSub.AutoSize = true;
            this.cbMakeSub.Location = new System.Drawing.Point(6, 31);
            this.cbMakeSub.Name = "cbMakeSub";
            this.cbMakeSub.Size = new System.Drawing.Size(183, 16);
            this.cbMakeSub.TabIndex = 4;
            this.cbMakeSub.Text = "ファイル名と同じサブフォルダを作る";
            this.cbMakeSub.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.BackColor = System.Drawing.Color.PeachPuff;
            this.tabPage2.Controls.Add(this.cbShowMaterials);
            this.tabPage2.Controls.Add(this.gbBone);
            this.tabPage2.Controls.Add(this.btnGenerate);
            this.tabPage2.Controls.Add(this.btnTsoFile);
            this.tabPage2.Controls.Add(this.btnTsoFileRef);
            this.tabPage2.Controls.Add(this.rbOneBone);
            this.tabPage2.Controls.Add(this.rbRefBone);
            this.tabPage2.Controls.Add(this.tbTsoFile);
            this.tabPage2.Controls.Add(this.label5);
            this.tabPage2.Controls.Add(this.tbTsoFileRef);
            this.tabPage2.Controls.Add(this.label4);
            this.tabPage2.Controls.Add(this.btnMqoFile);
            this.tabPage2.Controls.Add(this.tbMqoFile);
            this.tabPage2.Controls.Add(this.label3);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(488, 274);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Mqo->Tso";
            // 
            // cbShowMaterials
            // 
            this.cbShowMaterials.AutoSize = true;
            this.cbShowMaterials.Location = new System.Drawing.Point(295, 81);
            this.cbShowMaterials.Name = "cbShowMaterials";
            this.cbShowMaterials.Size = new System.Drawing.Size(144, 16);
            this.cbShowMaterials.TabIndex = 21;
            this.cbShowMaterials.Text = "マテリアル設定を表示する";
            this.cbShowMaterials.UseVisualStyleBackColor = true;
            // 
            // gbBone
            // 
            this.gbBone.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.gbBone.Controls.Add(this.btnDeselectAll);
            this.gbBone.Controls.Add(this.btnRefresh);
            this.gbBone.Controls.Add(this.btnSelectAll);
            this.gbBone.Controls.Add(this.label7);
            this.gbBone.Controls.Add(this.btnAssign);
            this.gbBone.Controls.Add(this.tvBones);
            this.gbBone.Controls.Add(this.lvObjects);
            this.gbBone.Controls.Add(this.label6);
            this.gbBone.Location = new System.Drawing.Point(6, 103);
            this.gbBone.Name = "gbBone";
            this.gbBone.Size = new System.Drawing.Size(476, 137);
            this.gbBone.TabIndex = 20;
            this.gbBone.TabStop = false;
            this.gbBone.Text = "ボーン指定";
            // 
            // btnDeselectAll
            // 
            this.btnDeselectAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnDeselectAll.Location = new System.Drawing.Point(123, 108);
            this.btnDeselectAll.Name = "btnDeselectAll";
            this.btnDeselectAll.Size = new System.Drawing.Size(110, 23);
            this.btnDeselectAll.TabIndex = 27;
            this.btnDeselectAll.Text = "すべて非選択";
            this.btnDeselectAll.UseVisualStyleBackColor = true;
            this.btnDeselectAll.Click += new System.EventHandler(this.btnDeselectAll_Click);
            // 
            // btnRefresh
            // 
            this.btnRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRefresh.Location = new System.Drawing.Point(272, 108);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(198, 23);
            this.btnRefresh.TabIndex = 26;
            this.btnRefresh.Text = "オブジェクトとボーンの一覧を更新";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // btnSelectAll
            // 
            this.btnSelectAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnSelectAll.Location = new System.Drawing.Point(6, 108);
            this.btnSelectAll.Name = "btnSelectAll";
            this.btnSelectAll.Size = new System.Drawing.Size(110, 23);
            this.btnSelectAll.TabIndex = 25;
            this.btnSelectAll.Text = "すべて選択";
            this.btnSelectAll.UseVisualStyleBackColor = true;
            this.btnSelectAll.Click += new System.EventHandler(this.btnSelectAll_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(270, 15);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(55, 12);
            this.label7.TabIndex = 24;
            this.label7.Text = "Tsoボーン:";
            // 
            // btnAssign
            // 
            this.btnAssign.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.btnAssign.Location = new System.Drawing.Point(239, 30);
            this.btnAssign.Name = "btnAssign";
            this.btnAssign.Size = new System.Drawing.Size(27, 72);
            this.btnAssign.TabIndex = 23;
            this.btnAssign.Text = "←割り当て";
            this.btnAssign.UseVisualStyleBackColor = true;
            this.btnAssign.Click += new System.EventHandler(this.btnAssign_Click);
            // 
            // tvBones
            // 
            this.tvBones.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tvBones.HideSelection = false;
            this.tvBones.Location = new System.Drawing.Point(272, 30);
            this.tvBones.Name = "tvBones";
            this.tvBones.Size = new System.Drawing.Size(198, 72);
            this.tvBones.TabIndex = 22;
            // 
            // lvObjects
            // 
            this.lvObjects.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.lvObjects.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chObjects1,
            this.chObjects2});
            this.lvObjects.FullRowSelect = true;
            this.lvObjects.HideSelection = false;
            this.lvObjects.Location = new System.Drawing.Point(6, 30);
            this.lvObjects.Name = "lvObjects";
            this.lvObjects.Size = new System.Drawing.Size(227, 72);
            this.lvObjects.TabIndex = 21;
            this.lvObjects.UseCompatibleStateImageBehavior = false;
            this.lvObjects.View = System.Windows.Forms.View.Details;
            // 
            // chObjects1
            // 
            this.chObjects1.Text = "オブジェクト";
            this.chObjects1.Width = 105;
            // 
            // chObjects2
            // 
            this.chObjects2.Text = "割り当てボーン";
            this.chObjects2.Width = 117;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 15);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(79, 12);
            this.label6.TabIndex = 20;
            this.label6.Text = "Mqoオブジェクト:";
            // 
            // btnGenerate
            // 
            this.btnGenerate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnGenerate.Location = new System.Drawing.Point(363, 246);
            this.btnGenerate.Name = "btnGenerate";
            this.btnGenerate.Size = new System.Drawing.Size(113, 23);
            this.btnGenerate.TabIndex = 17;
            this.btnGenerate.Text = "Tsoファイル作成";
            this.btnGenerate.UseVisualStyleBackColor = true;
            this.btnGenerate.Click += new System.EventHandler(this.btnGenerate_Click);
            // 
            // btnTsoFile
            // 
            this.btnTsoFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnTsoFile.Location = new System.Drawing.Point(407, 54);
            this.btnTsoFile.Name = "btnTsoFile";
            this.btnTsoFile.Size = new System.Drawing.Size(75, 23);
            this.btnTsoFile.TabIndex = 10;
            this.btnTsoFile.Text = "Ref";
            this.btnTsoFile.UseVisualStyleBackColor = true;
            this.btnTsoFile.Click += new System.EventHandler(this.btnTsoFile_Click);
            // 
            // btnTsoFileRef
            // 
            this.btnTsoFileRef.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnTsoFileRef.Location = new System.Drawing.Point(407, 29);
            this.btnTsoFileRef.Name = "btnTsoFileRef";
            this.btnTsoFileRef.Size = new System.Drawing.Size(75, 23);
            this.btnTsoFileRef.TabIndex = 9;
            this.btnTsoFileRef.Text = "Ref";
            this.btnTsoFileRef.UseVisualStyleBackColor = true;
            this.btnTsoFileRef.Click += new System.EventHandler(this.btnTsoFileRef_Click);
            // 
            // rbOneBone
            // 
            this.rbOneBone.AutoSize = true;
            this.rbOneBone.Location = new System.Drawing.Point(170, 81);
            this.rbOneBone.Name = "rbOneBone";
            this.rbOneBone.Size = new System.Drawing.Size(119, 16);
            this.rbOneBone.TabIndex = 8;
            this.rbOneBone.Text = "１ボーンに割り当てる";
            this.rbOneBone.UseVisualStyleBackColor = true;
            this.rbOneBone.CheckedChanged += new System.EventHandler(this.radioButton2_CheckedChanged);
            // 
            // rbRefBone
            // 
            this.rbRefBone.AutoSize = true;
            this.rbRefBone.Checked = true;
            this.rbRefBone.Location = new System.Drawing.Point(8, 81);
            this.rbRefBone.Name = "rbRefBone";
            this.rbRefBone.Size = new System.Drawing.Size(156, 16);
            this.rbRefBone.TabIndex = 7;
            this.rbRefBone.TabStop = true;
            this.rbRefBone.Text = "自動的にボーンを割り当てる";
            this.rbRefBone.UseVisualStyleBackColor = true;
            this.rbRefBone.CheckedChanged += new System.EventHandler(this.radioButton1_CheckedChanged);
            // 
            // tbTsoFile
            // 
            this.tbTsoFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tbTsoFile.Location = new System.Drawing.Point(65, 56);
            this.tbTsoFile.Name = "tbTsoFile";
            this.tbTsoFile.Size = new System.Drawing.Size(336, 19);
            this.tbTsoFile.TabIndex = 6;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 59);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(53, 12);
            this.label5.TabIndex = 5;
            this.label5.Text = "出力TSO:";
            // 
            // tbTsoFileRef
            // 
            this.tbTsoFileRef.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tbTsoFileRef.Location = new System.Drawing.Point(65, 31);
            this.tbTsoFileRef.Name = "tbTsoFileRef";
            this.tbTsoFileRef.Size = new System.Drawing.Size(336, 19);
            this.tbTsoFileRef.TabIndex = 4;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 34);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(53, 12);
            this.label4.TabIndex = 3;
            this.label4.Text = "参照TSO:";
            // 
            // btnMqoFile
            // 
            this.btnMqoFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnMqoFile.Location = new System.Drawing.Point(407, 4);
            this.btnMqoFile.Name = "btnMqoFile";
            this.btnMqoFile.Size = new System.Drawing.Size(75, 23);
            this.btnMqoFile.TabIndex = 2;
            this.btnMqoFile.Text = "Ref";
            this.btnMqoFile.UseVisualStyleBackColor = true;
            this.btnMqoFile.Click += new System.EventHandler(this.btnMqoFile_Click);
            // 
            // tbMqoFile
            // 
            this.tbMqoFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tbMqoFile.Location = new System.Drawing.Point(65, 6);
            this.tbMqoFile.Name = "tbMqoFile";
            this.tbMqoFile.Size = new System.Drawing.Size(336, 19);
            this.tbMqoFile.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(56, 12);
            this.label3.TabIndex = 0;
            this.label3.Text = "入力MQO:";
            // 
            // tabPage3
            // 
            this.tabPage3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(232)))), ((int)(((byte)(216)))));
            this.tabPage3.Controls.Add(this.btnMergeTso);
            this.tabPage3.Controls.Add(this.tbMergeTso);
            this.tabPage3.Controls.Add(this.btnMerge);
            this.tabPage3.Controls.Add(this.label9);
            this.tabPage3.Controls.Add(this.btnMergeReset);
            this.tabPage3.Controls.Add(this.btnMergeDel);
            this.tabPage3.Controls.Add(this.btnMergeAdd);
            this.tabPage3.Controls.Add(this.label8);
            this.tabPage3.Controls.Add(this.tvMerge);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(488, 274);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Merge";
            // 
            // btnMergeTso
            // 
            this.btnMergeTso.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnMergeTso.Location = new System.Drawing.Point(410, 220);
            this.btnMergeTso.Name = "btnMergeTso";
            this.btnMergeTso.Size = new System.Drawing.Size(75, 23);
            this.btnMergeTso.TabIndex = 8;
            this.btnMergeTso.Text = "Ref";
            this.btnMergeTso.UseVisualStyleBackColor = true;
            this.btnMergeTso.Click += new System.EventHandler(this.btnRefMergeTso_Click);
            // 
            // tbMergeTso
            // 
            this.tbMergeTso.AllowDrop = true;
            this.tbMergeTso.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tbMergeTso.Location = new System.Drawing.Point(60, 222);
            this.tbMergeTso.Name = "tbMergeTso";
            this.tbMergeTso.Size = new System.Drawing.Size(344, 19);
            this.tbMergeTso.TabIndex = 7;
            this.tbMergeTso.DragDrop += new System.Windows.Forms.DragEventHandler(this.tbMergeTso_DragDrop);
            this.tbMergeTso.DragEnter += new System.Windows.Forms.DragEventHandler(this.tbMergeTso_DragEnter);
            // 
            // btnMerge
            // 
            this.btnMerge.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnMerge.Location = new System.Drawing.Point(410, 249);
            this.btnMerge.Name = "btnMerge";
            this.btnMerge.Size = new System.Drawing.Size(75, 23);
            this.btnMerge.TabIndex = 6;
            this.btnMerge.Text = "実行";
            this.btnMerge.UseVisualStyleBackColor = true;
            this.btnMerge.Click += new System.EventHandler(this.btnMerge_Click);
            // 
            // label9
            // 
            this.label9.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(3, 225);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(51, 12);
            this.label9.TabIndex = 5;
            this.label9.Text = "出力TSO";
            // 
            // btnMergeReset
            // 
            this.btnMergeReset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnMergeReset.Location = new System.Drawing.Point(167, 193);
            this.btnMergeReset.Name = "btnMergeReset";
            this.btnMergeReset.Size = new System.Drawing.Size(75, 23);
            this.btnMergeReset.TabIndex = 4;
            this.btnMergeReset.Text = "リセット";
            this.btnMergeReset.UseVisualStyleBackColor = true;
            this.btnMergeReset.Click += new System.EventHandler(this.btnMergeReset_Click);
            // 
            // btnMergeDel
            // 
            this.btnMergeDel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnMergeDel.Location = new System.Drawing.Point(86, 193);
            this.btnMergeDel.Name = "btnMergeDel";
            this.btnMergeDel.Size = new System.Drawing.Size(75, 23);
            this.btnMergeDel.TabIndex = 3;
            this.btnMergeDel.Text = "削除";
            this.btnMergeDel.UseVisualStyleBackColor = true;
            this.btnMergeDel.Click += new System.EventHandler(this.btnMergeDel_Click);
            // 
            // btnMergeAdd
            // 
            this.btnMergeAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnMergeAdd.Location = new System.Drawing.Point(5, 193);
            this.btnMergeAdd.Name = "btnMergeAdd";
            this.btnMergeAdd.Size = new System.Drawing.Size(75, 23);
            this.btnMergeAdd.TabIndex = 2;
            this.btnMergeAdd.Text = "追加";
            this.btnMergeAdd.UseVisualStyleBackColor = true;
            this.btnMergeAdd.Click += new System.EventHandler(this.btnMergeAdd_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(3, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(51, 12);
            this.label8.TabIndex = 1;
            this.label8.Text = "入力TSO";
            // 
            // tvMerge
            // 
            this.tvMerge.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tvMerge.CheckBoxes = true;
            this.tvMerge.HideSelection = false;
            this.tvMerge.ItemHeight = 17;
            this.tvMerge.Location = new System.Drawing.Point(3, 15);
            this.tvMerge.Name = "tvMerge";
            this.tvMerge.Size = new System.Drawing.Size(482, 170);
            this.tvMerge.TabIndex = 0;
            this.tvMerge.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.tvMerge_AfterCheck);
            // 
            // Form1
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(520, 324);
            this.Controls.Add(this.tabControl1);
            this.MinimumSize = new System.Drawing.Size(528, 200);
            this.Name = "Form1";
            this.Text = "Tso2MqoGui v0.34";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.Form1_DragDrop);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.Form1_DragEnter);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.gbBone.ResumeLayout(false);
            this.gbBone.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox tbPath;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.CheckBox cbMakeSub;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Button btnMqoFile;
        private System.Windows.Forms.TextBox tbMqoFile;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnTsoFile;
        private System.Windows.Forms.Button btnTsoFileRef;
        private System.Windows.Forms.RadioButton rbOneBone;
        private System.Windows.Forms.RadioButton rbRefBone;
        private System.Windows.Forms.TextBox tbTsoFile;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox tbTsoFileRef;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnGenerate;
        private System.Windows.Forms.CheckBox cbCopyTSO;
        private System.Windows.Forms.RadioButton rbBoneRokDeBone;
        private System.Windows.Forms.RadioButton rbBoneNone;
        private System.Windows.Forms.GroupBox gbBone;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Button btnSelectAll;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button btnAssign;
        private System.Windows.Forms.TreeView tvBones;
        private System.Windows.Forms.ListView lvObjects;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ColumnHeader chObjects1;
        private System.Windows.Forms.ColumnHeader chObjects2;
        private System.Windows.Forms.Button btnDeselectAll;
        private System.Windows.Forms.CheckBox cbShowMaterials;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.Button btnMergeReset;
        private System.Windows.Forms.Button btnMergeDel;
        private System.Windows.Forms.Button btnMergeAdd;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TreeView tvMerge;
        private System.Windows.Forms.Button btnMergeTso;
        private System.Windows.Forms.TextBox tbMergeTso;
        private System.Windows.Forms.Button btnMerge;
        private System.Windows.Forms.Label label9;
    }
}

