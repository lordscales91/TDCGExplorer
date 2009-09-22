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
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            item.PoseFile = tbPoseFile.Text;
            item.Length = int.Parse(tbLength.Text);
            item.FaceFile = tbFaceFile.Text;
        }
    }
}
