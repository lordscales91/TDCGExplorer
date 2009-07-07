using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

//using System.Windows.Forms;
using TDCGExplorer;

namespace System.Windows.Forms
{
    class TextPageControl : ZipFilePageControl
    {
        private System.Windows.Forms.TextBox textBox;

        public TextPageControl(GenTahInfo tahInfo)
            : base(tahInfo)
        {
            InitializeComponent();
            ExtractFile();
            textBox.SelectionStart = 0;
            textBox.SelectionLength = 0;

            TDCGExplorer.TDCGExplorer.SetToolTips(tahInfo.path);
        }

        private void InitializeComponent()
        {
            this.textBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // textBox
            // 
            this.textBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox.BackColor = System.Drawing.SystemColors.Window;
            this.textBox.Location = new System.Drawing.Point(0, 0);
            this.textBox.Multiline = true;
            this.textBox.Name = "textBox";
            this.textBox.ReadOnly = true;
            this.textBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox.Size = new System.Drawing.Size(0, 0);
            this.textBox.TabIndex = 0;
            // 
            // TextPageControl
            // 
            this.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.Controls.Add(this.textBox);
            this.Resize += new System.EventHandler(this.TextPage_Resize);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        public override void BindingStream(MemoryStream ms)
        {
            StreamReader reader = new StreamReader(ms, System.Text.Encoding.GetEncoding("Shift_JIS"));
            textBox.Text = reader.ReadToEnd();
        }

        private void TextPage_Resize(object sender, EventArgs e)
        {
            textBox.Size = Size;
        }
    }
}
