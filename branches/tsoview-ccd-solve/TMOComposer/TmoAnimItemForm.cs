using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TMOComposer
{
    public partial class TmoAnimItemForm : Form
    {
        FaceListForm faceListForm = null;

        public void SetFaceListForm(FaceListForm form)
        {
            this.faceListForm = form;
        }

        public TmoAnimItemForm()
        {
            InitializeComponent();
        }
        
        TMOAnimItem item;

        public void SetTmoAnimItem(TMOAnimItem item)
        {
            this.item = item;
            tbPoseFile.Text = "";
            tbLength.Text = item.Length.ToString();
            tbFaceFile.Text = "";
            accelSlider1.Accel = item.Accel;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            item.LoadPoseFile(tbPoseFile.Text);
            item.Length = int.Parse(tbLength.Text);
            item.CopyFaceFile(tbFaceFile.Text);
            item.Accel = accelSlider1.Accel;
        }

        private void btnOpenFaces_Click(object sender, EventArgs e)
        {
            if (faceListForm.ShowDialog(this) == DialogResult.OK)
            {
                if (faceListForm.File == null)
                    return;

                tbFaceFile.Text = faceListForm.File;
            }
        }
    }
}
