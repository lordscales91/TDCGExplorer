using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TDCGExplorer
{
    public partial class TbnSelectForm : Form
    {
        public TbnSelectForm()
        {
            InitializeComponent();
        }

        // チェックボックスの状態をコピーする.
        public TDCGTbnCreateInfo getResult()
        {
            TDCGTbnCreateInfo info = new TDCGTbnCreateInfo();
            for (int i = 0; i < 30; i++)
                info.tbnFlags[i] = checkedListBoxTbns.GetItemChecked(i);
            return info;
        }
    }

    public class TDCGTbnCreateInfo
    {
        public bool[] tbnFlags = new bool[30];
    }
}
