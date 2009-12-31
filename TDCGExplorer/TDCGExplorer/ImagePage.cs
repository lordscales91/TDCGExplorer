using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using ArchiveLib;
using System.Drawing;
using TDCGExplorer;

namespace System.Windows.Forms
{
    class ImagePageControl : ZipFilePageControl
    {
        private Panel panel;
        private PictureBox pictureBox;

        public ImagePageControl(GenericTahInfo tahInfo) : base(tahInfo)
        {
            InitializeComponent();
            ExtractFile();
            TDCGExplorer.TDCGExplorer.SetToolTips(tahInfo.path + " : " + TextResource.ZoomByClick);
        }

        private void InitializeComponent()
        {
            this.pictureBox = new System.Windows.Forms.PictureBox();
            this.panel = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            this.panel.SuspendLayout();
            this.SuspendLayout();
            // 
            // pictureBox
            // 
            this.pictureBox.Location = new System.Drawing.Point(0, 0);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size(100, 100);
            this.pictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox.TabIndex = 0;
            this.pictureBox.TabStop = false;
            this.pictureBox.Click += new System.EventHandler(this.pictureBox_Click);
            this.pictureBox.MouseEnter += new System.EventHandler(this.pictureBox_MouseEnter);
            // 
            // panel
            // 
            this.panel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panel.AutoScroll = true;
            this.panel.Controls.Add(this.pictureBox);
            this.panel.Location = new System.Drawing.Point(0, 0);
            this.panel.Name = "panel";
            this.panel.Size = new System.Drawing.Size(0, 0);
            this.panel.TabIndex = 0;
            // 
            // ImagePageControl
            // 
            this.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.Controls.Add(this.panel);
            this.Resize += new System.EventHandler(this.ImagePageControl_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
            this.panel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        public override void BindingStream(MemoryStream ms)
        {
            try
            {
                // 全部読み出す.
                using(Bitmap bitmap = new Bitmap(ms))
                using(MemoryStream basebmp = new MemoryStream())
                {
                    bitmap.Save(basebmp, System.Drawing.Imaging.ImageFormat.Bmp);
                    Bitmap newbmp = new Bitmap(basebmp);
                    pictureBox.Image = (Image)newbmp;
                }
            }
            catch (Exception e)
            {
                TDCGExplorer.TDCGExplorer.SetToolTips(TextResource.Error+" : " + e.Message);
            }
        }

        private void pictureBox_MouseEnter(object sender, EventArgs e)
        {
            Control obj = (Control)sender;
            obj.Focus();
        }

        private void pictureBox_Click(object sender, EventArgs e)
        {
            if (pictureBox.SizeMode == PictureBoxSizeMode.Zoom)
            {
                pictureBox.SizeMode = PictureBoxSizeMode.AutoSize;
                pictureBox.Size = new Size(pictureBox.Image.Width, pictureBox.Image.Height);
            }
            else
            {
                pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
                pictureBox.Size = new Size(ClientRectangle.Width, ClientRectangle.Height);
            }
        }

        private void ImagePageControl_Resize(object sender, EventArgs e)
        {
            if (pictureBox.SizeMode == PictureBoxSizeMode.Zoom)
            {
                pictureBox.Size = new Size(ClientRectangle.Width, ClientRectangle.Height);
            }
            else
            {
                pictureBox.Size = new Size(pictureBox.Image.Width, pictureBox.Image.Height);
            }

        }
    }
}
