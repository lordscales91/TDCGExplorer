using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace TDCG.TAHTool
{
    public class TAHencrypt
    {
        public static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("tah.exe <folder>");
                return;
            }
            string source_file = args[0];

            //encrypt to TAH archive
            if (Directory.Exists(source_file))
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();

                //launch encrypt routine from here...
                encrypt_archive(source_file + ".tah", source_file);

                sw.Stop();
                Console.WriteLine("time: " + sw.Elapsed);
            }
        }

        public static int encrypt_archive(string file_path_name, string source_path)
        {
            //check if file already exists... if yes rename it
            try
            {
                if (File.Exists(file_path_name))
                {
                    File.Move(file_path_name, file_path_name + ".bak");
                }
            }
            catch (IOException)
            {
                System.Console.Out.WriteLine("Error: Could not rename existing TAH file.");
                return -1;
            }
            Encrypter encrypter = new Encrypter();

            //read in files from source path, do not compress them now.
            //全ディレクトリ名
            string[] directories = Directory.GetDirectories(source_path, "*", SearchOption.AllDirectories);

            {
                string dirname = source_path;
                string[] files = Directory.GetFiles(dirname);
                foreach (string file in files)
                    encrypter.Add(file);
            }
            for (int i = 0; i < directories.Length; i++)
            {
                string dirname = directories[i];
                string[] files = Directory.GetFiles(dirname);
                foreach (string file in files)
                    encrypter.Add(file);
            }

            encrypter.SourcePath = source_path;
            encrypter.GetFileEntryStream = delegate(string true_file_name)
            {
                Console.WriteLine("compressing {0}", true_file_name);
                return File.OpenRead(true_file_name);
            };
            encrypter.Save(file_path_name);
            return 0;
        }
    }
}
