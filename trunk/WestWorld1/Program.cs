using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace WestWorld1
{
    class Program
    {
        public static void Main()
        {
            TextWriterTraceListener tr1 = new TextWriterTraceListener(System.Console.Out);
            Debug.Listeners.Add(tr1);

            Miner miner = new Miner(0);
            System.Console.WriteLine("ID:{0}", miner.ID);
            for (int i = 0; i < 20; i++)
            {
                miner.Update();
            }
        }
    }
}
