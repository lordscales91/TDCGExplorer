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
        Form3 form3 = null;

        public void SetForm3(Form3 form3)
        {
            this.form3 = form3;
        }

        public TmoAnimItemForm()
        {
            InitializeComponent();
        }
        
        TMOAnimItem item;

        public void SetTmoAnimItem(TMOAnimItem item)
        {
            this.item = item;
            tbPoseFile.Text = item.PoseFile;
            tbLength.Text = item.Length.ToString();
            tbFaceFile.Text = item.FaceFile;
            tbAccel.Value = (int)(item.Accel*10);
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            item.PoseFile = tbPoseFile.Text;
            item.Length = int.Parse(tbLength.Text);
            item.FaceFile = tbFaceFile.Text;
            item.Accel = tbAccel.Value*0.1f;
        }

        private void btnOpenFaces_Click(object sender, EventArgs e)
        {
            if (form3.ShowDialog(this) == DialogResult.OK)
            {
                if (form3.File == null)
                    return;

                tbFaceFile.Text = form3.File;
            }
        }
    }
}
