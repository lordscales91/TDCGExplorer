namespace TDCGExplorer
{
    partial class TbnSelectForm
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
            this.checkedListBoxTbns = new System.Windows.Forms.CheckedListBox();
            this.buttonOk = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // checkedListBoxTbns
            // 
            this.checkedListBoxTbns.FormattingEnabled = true;
            this.checkedListBoxTbns.Items.AddRange(new object[] {
            "A:身体",
            "B:頭髪前",
            "C:頭髪後",
            "D:頭髪生え際",
            "E:瞳",
            "F:ブラ",
            "G:全身下着",
            "H:パンツ",
            "I:靴下",
            "J:上着",
            "K:全身上着",
            "L:上着オプション",
            "M:スカート",
            "N:尻尾",
            "O:靴",
            "P:頭部装備",
            "Q:眼鏡",
            "R:首輪",
            "S:腕装備",
            "T:背中",
            "U:頭髪オプション",
            "V:眼帯",
            "W:タイツ・ガーター",
            "X:手首",
            "Y:リボン",
            "Z:手持ち",
            "0:眉",
            "1:八重歯",
            "2:ほくろ",
            "3:イヤリング"});
            this.checkedListBoxTbns.Location = new System.Drawing.Point(12, 12);
            this.checkedListBoxTbns.Name = "checkedListBoxTbns";
            this.checkedListBoxTbns.Size = new System.Drawing.Size(240, 184);
            this.checkedListBoxTbns.TabIndex = 0;
            // 
            // buttonOk
            // 
            this.buttonOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOk.Location = new System.Drawing.Point(270, 174);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(75, 23);
            this.buttonOk.TabIndex = 2;
            this.buttonOk.Text = "OK";
            this.buttonOk.UseVisualStyleBackColor = true;
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(352, 174);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 3;
            this.buttonCancel.Text = "CANCEL";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // TbnSelectForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(435, 209);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOk);
            this.Controls.Add(this.checkedListBoxTbns);
            this.Name = "TbnSelectForm";
            this.Text = "tbnファイルの生成";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.CheckedListBox checkedListBoxTbns;
        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.Button buttonCancel;

    }
}