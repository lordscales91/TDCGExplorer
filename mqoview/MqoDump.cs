using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace mqoview
{
    public static class MqoDump
    {
        public static void Main()
        {
            Debug.Listeners.Add(new TextWriterTraceListener(Console.Out));
            MqoFile mqo = new MqoFile();
            mqo.Load(@"D:\TechArts3D\wc\1\1.mqo");
            mqo.Dump();
        }
    }
}