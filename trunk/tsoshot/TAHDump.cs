using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
/*
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
*/
using ArchiveLib;

namespace TDCGSaveViewer
{
    public static class TAHDump
    {
        public static void DumpTAHEntries(string source_file)
        {
            Console.WriteLine("# TAH " + source_file);
            using (FileStream source = File.OpenRead(source_file))
                DumpTAHEntries(source);
        }
        public static void DumpTAHEntries(Stream source)
        {
            TAHFile tah = new TAHFile(source);
            try
            {
                tah.LoadEntries();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex);
                return;
            }
            foreach (string file in tah.Files.Files)
            {
                if (Path.GetExtension(file) == ".tso")
                {
                    Console.WriteLine(file);
                }
            }
            foreach (TAHEntry ent in tah.EntrySet.Entries)
            {
                if (Path.GetExtension(ent.FileName) == ".tso")
                {
                    byte[] data = TAHUtil.ReadEntryData(tah.Reader, ent);
                    BinaryWriter writer = new BinaryWriter(File.Create(ent.FileName), System.Text.Encoding.Default);
                    writer.Write(data);
                    writer.Close();
                }
            }
        }

        public static void DumpArcEntries(string source_file, IArchive arc)
        {
            try
            {
                arc.Open(source_file);
                if (arc == null)
                    return;

                foreach (IArchiveEntry entry in arc)
                {
                    if (Path.GetExtension(entry.FileName) == ".tah")
                    {
                        Console.WriteLine("# TAH in archive " + entry.FileName);
                        using (MemoryStream ms = new MemoryStream((int)entry.Size))
                        {
                            arc.Extract(entry, ms);
                            ms.Seek(0, SeekOrigin.Begin);
                            DumpTAHEntries(ms);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex);
                return;
            }
        }
        public static void DumpZipEntries(string source_file)
        {
            Console.WriteLine("# zip " + source_file);
            IArchive arc = new ZipArchive();
            DumpArcEntries(source_file, arc);
        }
        public static void DumpRarEntries(string source_file)
        {
            Console.WriteLine("# rar " + source_file);
            IArchive arc = new RarArchive();
            DumpArcEntries(source_file, arc);
        }
        public static void DumpLzhEntries(string source_file)
        {
            Console.WriteLine("# lzh " + source_file);
            IArchive arc = new LzhArchive();
            DumpArcEntries(source_file, arc);
        }

        public static void DumpDirEntries(string dir)
        {
            string[] tah_files = Directory.GetFiles(dir, "*.TAH");
            foreach (string file in tah_files)
            {
                DumpTAHEntries(file);
            }
            string[] zip_files = Directory.GetFiles(dir, "*.ZIP");
            foreach (string file in zip_files)
            {
                DumpZipEntries(file);
            }
            string[] rar_files = Directory.GetFiles(dir, "*.RAR");
            foreach (string file in rar_files)
            {
                DumpRarEntries(file);
            }
            string[] lzh_files = Directory.GetFiles(dir, "*.LZH");
            foreach (string file in lzh_files)
            {
                DumpLzhEntries(file);
            }
            string[] entries = Directory.GetDirectories(dir);
            foreach (string entry in entries)
            {
                DumpDirEntries(entry);
            }
        }

        public static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                System.Console.WriteLine("Usage: TAHDump <zip file>");
                return;
            }
            string source_file = args[0];
            try
            {
                string ext = Path.GetExtension(source_file).ToUpper();
                if (ext == ".TAH")
                {
                    DumpTAHEntries(source_file);
                }
                else if (ext == ".ZIP")
                {
                    DumpZipEntries(source_file);
                }
                else if (ext == ".RAR")
                {
                    DumpRarEntries(source_file);
                }
                else if (ext == ".LZH")
                {
                    DumpLzhEntries(source_file);
                }
                else if (Directory.Exists(source_file))
                {
                    DumpDirEntries(source_file);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex);
            }
        }
    }
}
