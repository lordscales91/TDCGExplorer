using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace tso2pmx
{
    class Program
    {
        static void Main(string[] args)
        {
            PmxFile pmx = new PmxFile();
            pmx.Save("out.pmx");
        }
    }
}
