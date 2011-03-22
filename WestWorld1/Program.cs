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

            //create a miner
            Miner bob = new Miner(0);

            //create his wife
            MinersWife elsa = new MinersWife(1);

            //register them with the entity manager
            EntityManager.Instance.RegisterEntity(bob);
            EntityManager.Instance.RegisterEntity(elsa);

            //run Bob and Elsa through a few Update calls
            for (int i = 0; i < 20; i++)
            {
                bob.Update();
                elsa.Update();
                MessageDispatcher.Instance.DispatchDelayedMessages();
                System.Threading.Thread.Sleep(500);
            }
        }
    }
}
