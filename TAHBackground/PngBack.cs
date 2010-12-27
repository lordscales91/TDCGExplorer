using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using TDCG;
using TDCG.TAHTool;

namespace TAHBackground
{
    public class TSOData
    {
        internal uint opt1;
        internal byte[] ftso;
    }

    public class TSOFigure
    {
        internal List<TSOData> TSOList = new List<TSOData>();
        internal TMOFile tmo = null;
    }

    public class PngBack
    {
        TBNFile tbn = null;
        public TBNFile Tbn { get { return tbn; } }

        PSDFile psd = null;
        public PSDFile Psd { get { return psd; } }

        PNGFile png = null;
        public PNGFile Png { get { return png; } }

        TSOFigure fig = null;
        public TSOFigure Figure { get { return fig; } }

        public void Load(Stream tbn_stream, Stream psd_stream)
        {
            tbn = new TBNFile();
            tbn.Load(tbn_stream);

            psd = new PSDFile();
            psd.Load(psd_stream);

            png = CreatePNGFileFromPSDFile(psd);

            fig = new TSOFigure();
        }

        public static string GetTBNPathFromPSDPath(string psd_path)
        {
            return @"script/backgrounds/" + Path.GetFileNameWithoutExtension(psd_path) + @".tbn";
        }

        public static List<string> GetTSOPathListFromTBNFile(TBNFile tbn)
        {
            List<string> ret = new List<string>();
            List<string> strings = tbn.GetStringList();
            Regex re_tsofile = new Regex(@"\.tso$");
            foreach (string str in strings)
            {
                if (re_tsofile.IsMatch(str))
                    ret.Add(str);
            }
            return ret;
        }

        static PNGFile CreatePNGFileFromPSDFile(PSDFile psd)
        {
            MemoryStream ms = new MemoryStream();
            psd.Bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            ms.Seek(0, SeekOrigin.Begin);

            PNGFile png = new PNGFile();
            png.Load(ms);

            return png;
        }

        public void AddTSOFile(byte[] ftso)
        {
            TSOData tso = new TSOData();
            tso.opt1 = 0;
            tso.ftso = ftso;
            fig.TSOList.Add(tso);
        }

        public void Save(string dest_file)
        {
            png.WriteTaOb += delegate(BinaryWriter bw)
            {
                PNGWriter pw = new PNGWriter(bw);
                WriteHsav(pw);
            };
            png.Save(dest_file);
        }
        
        protected void WriteHsav(PNGWriter pw)
        {
            pw.WriteTDCG();
            pw.WriteHSAV();
            foreach (TSOData tso in fig.TSOList)
                pw.WriteFTSO(tso.opt1, tso.ftso);
        }
    }
}
