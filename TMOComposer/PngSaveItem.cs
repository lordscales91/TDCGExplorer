using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TMOComposer
{
    public class PngSaveItem
    {
        public string File { get; set; }
        //public int FigureIndex { get; set; }
        public TMOAnim tmoanim;

        public PngSaveItem()
        {
            tmoanim = new TMOAnim();
        }

        public void UpdateID(int i)
        {
            tmoanim.UpdateID(i);
        }
    }
}
