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
            if (form3.ShowDialog(this) == DialogResult.OK)
            {
                if (form3.File == null)
                    return;

                tbFaceFile.Text = form3.File;
            }
        }
    }
}
