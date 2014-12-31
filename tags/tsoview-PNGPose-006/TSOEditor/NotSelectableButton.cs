using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TSOEditor
{
    class NotSelectableButton : Button
    {
        public NotSelectableButton()
        {
            this.SetStyle(ControlStyles.Selectable, false);
        }
    }
}
