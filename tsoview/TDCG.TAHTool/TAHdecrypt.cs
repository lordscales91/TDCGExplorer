using System;
using System.Collections.Generic;
using System.IO;

namespace TDCG.TAHTool
{
    public class TAHdecrypt
    {
        public static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                DumpFiles(args[0]);
            }
        }

        public static int DumpFiles(string source_file)
        {
            Decrypter decrypter = new Decrypter();
            decrypter.Open(source_file);

            string base_path = Path.GetFileNameWithoutExtension(source_file);

            Console.WriteLine("file_name\toffset\tlength\tflag");
            foreach (TAHEntry entry in decrypter.Entries)
            {
                Console.WriteLine("{0}\t{1}\t{2}\t{3}", entry.file_name, entry.offset, entry.length, entry.flag);

                byte[] data_output;
                decrypter.ExtractResource(entry, out data_output);

                string file_name = entry.file_name;

                //flag & 0x1 = 1ならno path
                if (entry.flag % 2 == 1)
                {
                    file_name += TAHFileUtils.GetExtensionFromMagic(data_output);
                }
                string dest_file_name = Path.Combine(base_path, file_name);
                Directory.CreateDirectory(Path.GetDirectoryName(dest_file_name));

                BinaryWriter file_writer = new BinaryWriter(File.Create(dest_file_name));
                file_writer.Write(data_output);
                file_writer.Close();
            }
            decrypter.Close();

            return 0;
        }
    }
}