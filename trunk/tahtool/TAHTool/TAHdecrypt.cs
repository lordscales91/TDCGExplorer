using System;
using System.Collections.Generic;
using System.IO;

    class TAHdecrypt
    {
        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                DumpFiles(args[0]);
                //decrypt_TAH_archive(args[0]);
            }
        }

        static int DumpFiles(string source_file)
        {
            Decrypter decrypter = new Decrypter();
            decrypter.Load(source_file);

            string base_path = Path.GetFileNameWithoutExtension(source_file);

            Console.WriteLine("file_name\toffset\tlength\tflag");
            entry_meta_info info;
            while (decrypter.FindNext(out info))
            {
                Console.WriteLine("{0}\t{1}\t{2}\t{3}", info.file_name, info.offset, info.length, info.flag);

                byte[] data_output;
                decrypter.ExtractResource(ref info, out data_output);

                string file_name = info.file_name;

                //flag & 0x1 = 1‚È‚çno path
                if (info.flag % 2 == 1)
                {
                    string ext;
                    string magic = System.Text.Encoding.ASCII.GetString(data_output, 0, 4);
                    switch (magic)
                    {
                        case "8BPS":
                            ext = ".psd";
                            break;
                        case "TMO1":
                            ext = ".tmo";
                            break;
                        case "TSO1":
                            ext = ".tso";
                            break;
                        case "OggS":
                            ext = ".ogg";
                            break;
                        case "BBBB":
                            ext = ".tbn";
                            break;
                        default:
                            ext = ".cgfx";
                            break;
                    }
                    file_name += ext;
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

        static int decrypt_TAH_archive(string source_file)
        {
            string dest_path = "";
            string[] sep = new string[1];
            sep[0] = "\\";
            string[] file_path = source_file.Split(sep, System.StringSplitOptions.RemoveEmptyEntries);
            string file_name = file_path[file_path.Length - 1];
            if (file_name.Substring(file_name.Length - 3, 3).ToLower().CompareTo("tah") == 0)
            {
                //decrypt TAH archive
                string folder_name = file_name.Substring(0, file_name.LastIndexOf("."));
                for (int i = 0; i < file_path.Length - 1; i++)
                {
                    dest_path += file_path[i] + "\\";
                }
                dest_path += folder_name;
                System.Console.Out.WriteLine(dest_path);
                Decrypter decrypter = new Decrypter();
                return decrypter.decrypt_archive(source_file, dest_path);
            }
            else
            {
                //encrypt to TAH archive
                return -1;
            }
        }

    }
