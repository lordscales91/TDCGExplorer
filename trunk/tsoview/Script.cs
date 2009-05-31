using System;

namespace TAHdecrypt
{

public class Script
{
    public void Hello(TSOSample sample)
    {
        Console.WriteLine("TSOFigureList.Count {0}", sample.TSOFigureList.Count);
        sample.LoadTMOFile(@"teni-1.tmo");
        sample.LoadTMOFile(30, @"teni-2.tmo");
        sample.LoadTMOFile(60, @"teni-3.tmo");
        sample.LoadTMOFile(90, @"teni-4.tmo");
    }
}
}
