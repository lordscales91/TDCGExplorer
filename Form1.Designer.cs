namespace tso2mqo
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
            this.cbCopyTSO = new System.Windows.Forms.CheckBox();
            this.cbMakeSub = new System.Windows.Forms.CheckBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.cbShowMaterials = new System.Windows.Forms.CheckBox();
            this.gbBone = new System.Windows.Forms.GroupBox();
            this.bDeselectAll = new System.Windows.Forms.Button();
            this.bRefresh = new System.Windows.Forms.Button();
            this.bSelectAll = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.bAssign = new System.Windows.Forms.Button();
            this.tvBone = new System.Windows.Forms.TreeView();
            this.lvObject = new System.Windows.Forms.ListView();
            this.chObjects1 = new System.Windows.Forms.ColumnHeader();
            this.chObjects2 = new System.Windows.Forms.ColumnHeader();
            this.label6 = new System.Windows.Forms.Label();
            this.bExpOk = new System.Windows.Forms.Button();
            this.bRefTsoEx = new System.Windows.Forms.Button();
            this.bRefTso = new System.Windows.Forms.Button();
            this.rb1Bone = new System.Windows.Forms.RadioButton();
            this.rbAutoBone = new System.Windows.Forms.RadioButton();
            this.tbTsoEx = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.tbTso = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.bRefMqoIn = new System.Windows.Forms.Button();
            this.tbMqoIn = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.bRefMergeTso = new System.Windows.Forms.Button();
            this.tbMergeTso = new System.Windows.Forms.TextBox();
            this.bMerge = new System.Windows.Forms.Button();
            this.label9 = new System.Windows.Forms.Label();
            this.bMergeReset = new System.Windows.Forms.Button();
            this.bMergeDel = new System.Windows.Forms.Button();
            this.bMergeAdd = new System.Windows.Forms.Button();
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
            this.label2.Size = new System.Drawing.Size(476, 178);
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
            this.tabPage2.Controls.Add(this.bExpOk);
            this.tabPage2.Controls.Add(this.bRefTsoEx);
            this.tabPage2.Controls.Add(this.bRefTso);
            this.tabPage2.Controls.Add(this.rb1Bone);
            this.tabPage2.Controls.Add(this.rbAutoBone);
            this.tabPage2.Controls.Add(this.tbTsoEx);
            this.tabPage2.Controls.Add(this.label5);
            this.tabPage2.Controls.Add(this.tbTso);
            this.tabPage2.Controls.Add(this.label4);
            this.tabPage2.Controls.Add(this.bRefMqoIn);
            this.tabPage2.Controls.Add(this.tbMqoIn);
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
            this.gbBone.Controls.Add(this.bDeselectAll);
            this.gbBone.Controls.Add(this.bRefresh);
            this.gbBone.Controls.Add(this.bSelectAll);
            this.gbBone.Controls.Add(this.label7);
            this.gbBone.Controls.Add(this.bAssign);
            this.gbBone.Controls.Add(this.tvBone);
            this.gbBone.Controls.Add(this.lvObject);
            this.gbBone.Controls.Add(this.label6);
            this.gbBone.Location = new System.Drawing.Point(6, 103);
            this.gbBone.Name = "gbBone";
            this.gbBone.Size = new System.Drawing.Size(476, 137);
            this.gbBone.TabIndex = 20;
            this.gbBone.TabStop = false;
            this.gbBone.Text = "ボーン指定";
            // 
            // bDeselectAll
            // 
            this.bDeselectAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.bDeselectAll.Location = new System.Drawing.Point(123, 108);
            this.bDeselectAll.Name = "bDeselectAll";
            this.bDeselectAll.Size = new System.Drawing.Size(110, 23);
            this.bDeselectAll.TabIndex = 27;
            this.bDeselectAll.Text = "すべて非選択";
            this.bDeselectAll.UseVisualStyleBackColor = true;
            this.bDeselectAll.Click += new System.EventHandler(this.bDeselectAll_Click);
            // 
            // bRefresh
            // 
            this.bRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bRefresh.Location = new System.Drawing.Point(272, 108);
            this.bRefresh.Name = "bRefresh";
            this.bRefresh.Size = new System.Drawing.Size(198, 23);
            this.bRefresh.TabIndex = 26;
            this.bRefresh.Text = "オブジェクトとボーンの一覧を更新";
            this.bRefresh.UseVisualStyleBackColor = true;
            this.bRefresh.Click += new System.EventHandler(this.bRefresh_Click);
            // 
            // bSelectAll
            // 
            this.bSelectAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.bSelectAll.Location = new System.Drawing.Point(6, 108);
            this.bSelectAll.Name = "bSelectAll";
            this.bSelectAll.Size = new System.Drawing.Size(110, 23);
            this.bSelectAll.TabIndex = 25;
            this.bSelectAll.Text = "すべて選択";
            this.bSelectAll.UseVisualStyleBackColor = true;
            this.bSelectAll.Click += new System.EventHandler(this.bSelectAll_Click);
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
            // bAssign
            // 
            this.bAssign.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.bAssign.Location = new System.Drawing.Point(239, 30);
            this.bAssign.Name = "bAssign";
            this.bAssign.Size = new System.Drawing.Size(27, 72);
            this.bAssign.TabIndex = 23;
            this.bAssign.Text = "←割り当て";
            this.bAssign.UseVisualStyleBackColor = true;
            this.bAssign.Click += new System.EventHandler(this.bAssign_Click);
            // 
            // tvBone
            // 
            this.tvBone.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tvBone.HideSelection = false;
            this.tvBone.Location = new System.Drawing.Point(272, 30);
            this.tvBone.Name = "tvBone";
            this.tvBone.Size = new System.Drawing.Size(198, 72);
            this.tvBone.TabIndex = 22;
            // 
            // lvObject
            // 
            this.lvObject.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.lvObject.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chObjects1,
            this.chObjects2});
            this.lvObject.FullRowSelect = true;
            this.lvObject.HideSelection = false;
            this.lvObject.Location = new System.Drawing.Point(6, 30);
            this.lvObject.Name = "lvObject";
            this.lvObject.Size = new System.Drawing.Size(227, 72);
            this.lvObject.TabIndex = 21;
            this.lvObject.UseCompatibleStateImageBehavior = false;
            this.lvObject.View = System.Windows.Forms.View.Details;
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
            // bExpOk
            // 
            this.bExpOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bExpOk.Location = new System.Drawing.Point(363, 246);
            this.bExpOk.Name = "bExpOk";
            this.bExpOk.Size = new System.Drawing.Size(113, 23);
            this.bExpOk.TabIndex = 17;
            this.bExpOk.Text = "Tsoファイル作成";
            this.bExpOk.UseVisualStyleBackColor = true;
            this.bExpOk.Click += new System.EventHandler(this.bOk_Click);
            // 
            // bRefTsoEx
            // 
            this.bRefTsoEx.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bRefTsoEx.Location = new System.Drawing.Point(407, 54);
            this.bRefTsoEx.Name = "bRefTsoEx";
            this.bRefTsoEx.Size = new System.Drawing.Size(75, 23);
            this.bRefTsoEx.TabIndex = 10;
            this.bRefTsoEx.Text = "Ref";
            this.bRefTsoEx.UseVisualStyleBackColor = true;
            this.bRefTsoEx.Click += new System.EventHandler(this.bRefTsoEx_Click);
            // 
            // bRefTso
            // 
            this.bRefTso.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bRefTso.Location = new System.Drawing.Point(407, 29);
            this.bRefTso.Name = "bRefTso";
            this.bRefTso.Size = new System.Drawing.Size(75, 23);
            this.bRefTso.TabIndex = 9;
            this.bRefTso.Text = "Ref";
            this.bRefTso.UseVisualStyleBackColor = true;
            this.bRefTso.Click += new System.EventHandler(this.bRefTso_Click);
            // 
            // rb1Bone
            // 
            this.rb1Bone.AutoSize = true;
            this.rb1Bone.Location = new System.Drawing.Point(170, 81);
            this.rb1Bone.Name = "rb1Bone";
            this.rb1Bone.Size = new System.Drawing.Size(119, 16);
            this.rb1Bone.TabIndex = 8;
            this.rb1Bone.Text = "１ボーンに割り当てる";
            this.rb1Bone.UseVisualStyleBackColor = true;
            this.rb1Bone.CheckedChanged += new System.EventHandler(this.radioButton2_CheckedChanged);
            // 
            // rbAutoBone
            // 
            this.rbAutoBone.AutoSize = true;
            this.rbAutoBone.Checked = true;
            this.rbAutoBone.Location = new System.Drawing.Point(8, 81);
            this.rbAutoBone.Name = "rbAutoBone";
            this.rbAutoBone.Size = new System.Drawing.Size(156, 16);
            this.rbAutoBone.TabIndex = 7;
            this.rbAutoBone.TabStop = true;
            this.rbAutoBone.Text = "自動的にボーンを割り当てる";
            this.rbAutoBone.UseVisualStyleBackColor = true;
            this.rbAutoBone.CheckedChanged += new System.EventHandler(this.radioButton1_CheckedChanged);
            // 
            // tbTsoEx
            // 
            this.tbTsoEx.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tbTsoEx.Location = new System.Drawing.Point(65, 56);
            this.tbTsoEx.Name = "tbTsoEx";
            this.tbTsoEx.Size = new System.Drawing.Size(336, 19);
            this.tbTsoEx.TabIndex = 6;
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
            // tbTso
            // 
            this.tbTso.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tbTso.Location = new System.Drawing.Point(65, 31);
            this.tbTso.Name = "tbTso";
            this.tbTso.Size = new System.Drawing.Size(336, 19);
            this.tbTso.TabIndex = 4;
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
            // bRefMqoIn
            // 
            this.bRefMqoIn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bRefMqoIn.Location = new System.Drawing.Point(407, 4);
            this.bRefMqoIn.Name = "bRefMqoIn";
            this.bRefMqoIn.Size = new System.Drawing.Size(75, 23);
            this.bRefMqoIn.TabIndex = 2;
            this.bRefMqoIn.Text = "Ref";
            this.bRefMqoIn.UseVisualStyleBackColor = true;
            this.bRefMqoIn.Click += new System.EventHandler(this.bRefMqoIn_Click);
            // 
            // tbMqoIn
            // 
            this.tbMqoIn.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tbMqoIn.Location = new System.Drawing.Point(65, 6);
            this.tbMqoIn.Name = "tbMqoIn";
            this.tbMqoIn.Size = new System.Drawing.Size(336, 19);
            this.tbMqoIn.TabIndex = 1;
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
            this.tabPage3.Controls.Add(this.bRefMergeTso);
            this.tabPage3.Controls.Add(this.tbMergeTso);
            this.tabPage3.Controls.Add(this.bMerge);
            this.tabPage3.Controls.Add(this.label9);
            this.tabPage3.Controls.Add(this.bMergeReset);
            this.tabPage3.Controls.Add(this.bMergeDel);
            this.tabPage3.Controls.Add(this.bMergeAdd);
            this.tabPage3.Controls.Add(this.label8);
            this.tabPage3.Controls.Add(this.tvMerge);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(488, 274);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Merge";
            // 
            // bRefMergeTso
            // 
            this.bRefMergeTso.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bRefMergeTso.Location = new System.Drawing.Point(410, 220);
            this.bRefMergeTso.Name = "bRefMergeTso";
            this.bRefMergeTso.Size = new System.Drawing.Size(75, 23);
            this.bRefMergeTso.TabIndex = 8;
            this.bRefMergeTso.Text = "Ref";
            this.bRefMergeTso.UseVisualStyleBackColor = true;
            this.bRefMergeTso.Click += new System.EventHandler(this.bRefMergeTso_Click);
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
            // bMerge
            // 
            this.bMerge.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bMerge.Location = new System.Drawing.Point(410, 249);
            this.bMerge.Name = "bMerge";
            this.bMerge.Size = new System.Drawing.Size(75, 23);
            this.bMerge.TabIndex = 6;
            this.bMerge.Text = "実行";
            this.bMerge.UseVisualStyleBackColor = true;
            this.bMerge.Click += new System.EventHandler(this.bMerge_Click);
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
            // bMergeReset
            // 
            this.bMergeReset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.bMergeReset.Location = new System.Drawing.Point(167, 193);
            this.bMergeReset.Name = "bMergeReset";
            this.bMergeReset.Size = new System.Drawing.Size(75, 23);
            this.bMergeReset.TabIndex = 4;
            this.bMergeReset.Text = "リセット";
            this.bMergeReset.UseVisualStyleBackColor = true;
            this.bMergeReset.Click += new System.EventHandler(this.bMergeReset_Click);
            // 
            // bMergeDel
            // 
            this.bMergeDel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.bMergeDel.Location = new System.Drawing.Point(86, 193);
            this.bMergeDel.Name = "bMergeDel";
            this.bMergeDel.Size = new System.Drawing.Size(75, 23);
            this.bMergeDel.TabIndex = 3;
            this.bMergeDel.Text = "削除";
            this.bMergeDel.UseVisualStyleBackColor = true;
            this.bMergeDel.Click += new System.EventHandler(this.bMergeDel_Click);
            // 
            // bMergeAdd
            // 
            this.bMergeAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.bMergeAdd.Location = new System.Drawing.Point(5, 193);
            this.bMergeAdd.Name = "bMergeAdd";
            this.bMergeAdd.Size = new System.Drawing.Size(75, 23);
            this.bMergeAdd.TabIndex = 2;
            this.bMergeAdd.Text = "追加";
            this.bMergeAdd.UseVisualStyleBackColor = true;
            this.bMergeAdd.Click += new System.EventHandler(this.bMergeAdd_Click);
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
            this.Text = "Tso2MqoGui v0.31";
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
        private System.Windows.Forms.Button bRefMqoIn;
        private System.Windows.Forms.TextBox tbMqoIn;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button bRefTsoEx;
        private System.Windows.Forms.Button bRefTso;
        private System.Windows.Forms.RadioButton rb1Bone;
        private System.Windows.Forms.RadioButton rbAutoBone;
        private System.Windows.Forms.TextBox tbTsoEx;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox tbTso;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button bExpOk;
        private System.Windows.Forms.CheckBox cbCopyTSO;
        private System.Windows.Forms.GroupBox gbBone;
        private System.Windows.Forms.Button bRefresh;
        private System.Windows.Forms.Button bSelectAll;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button bAssign;
        private System.Windows.Forms.TreeView tvBone;
        private System.Windows.Forms.ListView lvObject;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ColumnHeader chObjects1;
        private System.Windows.Forms.ColumnHeader chObjects2;
        private System.Windows.Forms.Button bDeselectAll;
        private System.Windows.Forms.CheckBox cbShowMaterials;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.Button bMergeReset;
        private System.Windows.Forms.Button bMergeDel;
        private System.Windows.Forms.Button bMergeAdd;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TreeView tvMerge;
        private System.Windows.Forms.Button bRefMergeTso;
        private System.Windows.Forms.TextBox tbMergeTso;
        private System.Windows.Forms.Button bMerge;
        private System.Windows.Forms.Label label9;
    }
}

