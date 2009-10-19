using System;
using System.Collections.Generic;
using System.IO;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using TDCG;

namespace PNGNodeCopy
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

        internal byte[] lgta;
        internal byte[] figu;
    }

    class Program
    {
        static void Main(string[] args) 
        {
            if (args.Length < 2)
            {
                System.Console.WriteLine("Usage: PNGNodeCopy <source png> <motion png> [node name]");
                return;
            }

            string source_file = args[0];
            string motion_file = args[1];
            string node_name = "W_Hips";

            if (args.Length > 2)
                node_name = args[2];

            Program program = new Program();
            program.CopyNode(source_file, motion_file, node_name);
        }

        internal List<TSOFigure> TSOFigureList = new List<TSOFigure>();

        byte[] cami;

        public bool CopyNode(string source_file, string motion_file, string node_name)
        {
            List<TSOFigure> fig_list = new List<TSOFigure>();

            Console.WriteLine("Load File: " + source_file);
            PNGFile source = new PNGFile();
            string source_type = "";

            try
            {
                TSOFigure fig = null;
                TMOFile tmo = null;

                source.Hsav += delegate(string type)
                {
                    source_type = type;

                    fig = new TSOFigure();
                    fig_list.Add(fig);
                };
                source.Pose += delegate(string type)
                {
                    source_type = type;
                };
                source.Scne += delegate(string type)
                {
                    source_type = type;
                };
                source.Cami += delegate(Stream dest, int extract_length)
                {
                    cami = new byte[extract_length];
                    dest.Read(cami, 0, extract_length);
                };
                source.Lgta += delegate(Stream dest, int extract_length)
                {
                    byte[] lgta = new byte[extract_length];
                    dest.Read(lgta, 0, extract_length);

                    fig = new TSOFigure();
                    fig.lgta = lgta;
                    fig_list.Add(fig);
                };
                source.Ftmo += delegate(Stream dest, int extract_length)
                {
                    tmo = new TMOFile();
                    tmo.Load(dest);
                    fig.tmo = tmo;
                };
                source.Figu += delegate(Stream dest, int extract_length)
                {
                    byte[] figu = new byte[extract_length];
                    dest.Read(figu, 0, extract_length);

                    fig.figu = figu;
                };
                source.Ftso += delegate(Stream dest, int extract_length, byte[] opt1)
                {
                    byte[] ftso = new byte[extract_length];
                    dest.Read(ftso, 0, extract_length);

                    TSOData tso = new TSOData();
                    tso.opt1 = BitConverter.ToUInt32(opt1, 0);
                    tso.ftso = ftso;
                    fig.TSOList.Add(tso);
                };

                source.Load(source_file);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }

            foreach (TSOFigure fig in fig_list)
            {
                TSOFigureList.Add(fig);
            }

            Console.WriteLine("Load File: " + motion_file);
            PNGFile motion = new PNGFile();
            TMOFile motion_tmo = null;
            try
            {
                motion.Ftmo += delegate(Stream dest, int extract_length)
                {
                    motion_tmo = new TMOFile();
                    motion_tmo.Load(dest);
                };

                motion.Load(motion_file);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }

            if (motion_tmo != null)
                foreach (TSOFigure fig in TSOFigureList)
                    if (fig.tmo != null)
                        fig.tmo.CopyNodeFrom(motion_tmo, node_name);

            string dest_file = source_file + ".tmp";
            Console.WriteLine("Save File: " + dest_file);
            source.WriteTaOb += delegate(BinaryWriter bw)
            {
                PNGWriter pw = new PNGWriter(bw);
                switch (source_type)
                {
                case "HSAV":
                    WriteHsav(pw);
                    break;
                case "POSE":
                    WritePose(pw);
                    break;
                case "SCNE":
                    WriteScne(pw);
                    break;
                }
            };
            source.Save(dest_file);

            File.Delete(source_file);
            File.Move(dest_file, source_file);
            Console.WriteLine("updated " + source_file);

            return true;
        }

        protected void WriteHsav(PNGWriter pw)
        {
            pw.WriteTDCG();
            pw.WriteHSAV();
            foreach (TSOFigure fig in TSOFigureList)
                foreach (TSOData tso in fig.TSOList)
                    pw.WriteFTSO(tso.opt1, tso.ftso);
        }

        protected void WritePose(PNGWriter pw)
        {
            pw.WriteTDCG();
            pw.WritePOSE();
            pw.WriteCAMI(cami);
            foreach (TSOFigure fig in TSOFigureList)
            {
                pw.WriteLGTA(fig.lgta);
                pw.WriteFTMO(fig.tmo);
            }
        }

        protected void WriteScne(PNGWriter pw)
        {
            pw.WriteTDCG();
            pw.WriteSCNE(FigureCount());
            pw.WriteCAMI(cami);
            foreach (TSOFigure fig in TSOFigureList)
            {
                pw.WriteLGTA(fig.lgta);
                pw.WriteFTMO(fig.tmo);
                pw.WriteFIGU(fig.figu);
                foreach (TSOData tso in fig.TSOList)
                    pw.WriteFTSO(tso.opt1, tso.ftso);
            }
        }

        protected int FigureCount()
        {
            return TSOFigureList.Count;
        }
    }
}
