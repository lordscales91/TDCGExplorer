using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Security.Cryptography;
/*
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
*/
using ArchiveLib;

namespace TAHdecrypt
{
    public static class TAHDump
    {
        public static MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();

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
            foreach (TAHEntry ent in tah.EntrySet.Entries)
            {
                if (ent.FileName!=null && Path.GetExtension(ent.FileName).ToLower() == ".tso")
                {
                    byte[] data = TAHUtil.ReadEntryData(tah.Reader, ent);
                    byte[] hash = md5.ComputeHash(data);
                    StringBuilder sb = new StringBuilder();
                    foreach (byte b in hash)
                        sb.Append(b.ToString("x2"));
                    string md5sum = sb.ToString();
                    Console.WriteLine("{0} {1}", md5sum, ent.FileName);
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
                    if (Path.GetExtension(entry.FileName).ToLower() == ".tah")
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
            TAHEntry.ReadExternalFileList();
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
