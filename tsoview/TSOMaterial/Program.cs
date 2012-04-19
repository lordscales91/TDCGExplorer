using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TDCG;

namespace TSOMaterial
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("TSOMaterial.exe <tso file>");
                return;
            }
            string source_file = args[0];

            if (Path.GetExtension(source_file).ToLower() == ".tso")
            {
                string dest_path = Path.GetDirectoryName(source_file);
                string file_name = Path.GetFileName(source_file);
                string folder_name = Path.GetFileNameWithoutExtension(source_file);
                dest_path = Path.Combine(dest_path, folder_name);
                Console.WriteLine("Extract File: " + source_file);

                Program program = new Program();

                program.Extract(source_file, dest_path);
            }
            else if (Directory.Exists(source_file))
            {
                Console.WriteLine("Compose File: " + source_file);

                Program program = new Program();

                program.Compose(source_file);
            }
        }
        
        public int Extract(string source_file, string dest_path)
        {
            try
            {
                Directory.CreateDirectory(dest_path);
            }
            catch (Exception)
            {
                Console.WriteLine("Error: Cannot prepare destination directory for file writing.");
                return -1;
            }
            TSOFile tso = new TSOFile();
            tso.Load(source_file);

            foreach (TSOTex tex in tso.textures)
            {
                string name = tex.Name;
                string file = tex.FileName.Trim('"');
                Console.WriteLine(file);
                using (BinaryWriter bw = new BinaryWriter(File.Create(Path.Combine(dest_path, file))))
                {
                    switch (Path.GetExtension(file).ToLower())
                    {
                        case ".bmp":
                            tex.SaveBMP(bw);
                            break;
                        case ".tga":
                            tex.SaveTGA(bw);
                            break;
                    }
                }
            }

            foreach (TSOScript scr in tso.scripts)
            {
                string name = scr.Name;
                Console.WriteLine(name);
                scr.Save(Path.Combine(dest_path, name));
            }

            foreach (TSOSubScript scr in tso.sub_scripts)
            {
                string name = scr.Name;
                string file = scr.FileName.Trim('"');
                Console.WriteLine(name);
                scr.Save(Path.Combine(dest_path, name));
            }

            return 0;
        }

        public int Compose(string dest_path)
        {
            string dest_file = dest_path + @".tso";
            if (!File.Exists(dest_file))
            {
                Console.WriteLine("File not found: " + dest_file);
                return -1;
            }
            TSOFile tso = new TSOFile();
            tso.Load(dest_file);

            foreach (TSOTex tex in tso.textures)
            {
                string name = tex.Name;
                string file = tex.FileName.Trim('"');
                string path = Path.Combine(dest_path, file);
                if (!File.Exists(path))
                {
                    Console.WriteLine("File not found: " + path);
                    continue;
                }
                Console.WriteLine(file);
                using (BinaryReader br = new BinaryReader(File.OpenRead(path)))
                {
                    switch (Path.GetExtension(file).ToLower())
                    {
                        case ".bmp":
                            tex.LoadBMP(br);
                            break;
                        case ".tga":
                            tex.LoadTGA(br);
                            break;
                    }
                }
            }

            foreach (TSOScript scr in tso.scripts)
            {
                string name = scr.Name;
                string path = Path.Combine(dest_path, name);
                if (!File.Exists(path))
                {
                    Console.WriteLine("File not found: " + path);
                    continue;
                }
                Console.WriteLine(name);
                scr.Load(path);
            }

            foreach (TSOSubScript scr in tso.sub_scripts)
            {
                string name = scr.Name;
                string file = scr.FileName.Trim('"');
                string path = Path.Combine(dest_path, name);
                if (!File.Exists(path))
                {
                    Console.WriteLine("File not found: " + path);
                    continue;
                }
                Console.WriteLine(name);
                scr.Load(path);
            }

            {
                int i = 1;
                string backup_file = null;
                while (true)
                {
                    backup_file = dest_path + @"." + i.ToString() + @".tso";
                    if (!File.Exists(backup_file))
                        break;
                    i++;
                }
                Console.WriteLine("Rename File: " + dest_file + " to " + backup_file);
                File.Move(dest_file, backup_file);
            }
            Console.WriteLine("Save File: " + dest_file);
            tso.Save(dest_file);

            return 0;
        }
    }
}
