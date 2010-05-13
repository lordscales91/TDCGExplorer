using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace TDCG.TSOHair
{
    public class TSOHairPart
    {
        Regex re = null;
        string text_pattern = null;

        public string Name { get; set; }
        public string TextPattern
        {
            get
            {
                return text_pattern;
            }
            set
            {
                text_pattern = value;
                re = new Regex(text_pattern, RegexOptions.IgnoreCase);
            }
        }
        public string SubPath { get; set; }
        public string TexPath { get; set; }

        public Regex GetTextRegex()
        {
            return re;
        }
    }
}
