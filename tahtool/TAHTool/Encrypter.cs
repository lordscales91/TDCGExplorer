using System;
using System.Collections.Generic;
using System.IO;

    class Encrypter
    {
        public struct file_entry
        {
            public byte[] compressed_data;
            public string true_file_name;
            public UInt32 compressed_length;
            public UInt32 uncompressed_length;
            public string file_name;
            public UInt32 hash_value; //only for entries with file_name == null
        }

        List<string> files = new List<string>();

        void add_file(string file)
        {
            files.Add(file);
            //Console.WriteLine("add {0}", file);
        }

        int build_file_indices(string source_path, out string[] file_index, out file_entry[] all_compressed_files)
        {
            Dictionary<string, List<string>> dir_entries = new Dictionary<string, List<string>>();

            foreach (string file in files)
            {
                string dirname = Path.GetDirectoryName(file);
                string basename = Path.GetFileName(file);
                if (! dir_entries.ContainsKey(dirname))
                    dir_entries[dirname] = new List<string>(); 
                dir_entries[dirname].Add(basename);
                //Console.WriteLine("dirname {0} basename {1}", dirname, basename);
            }

            //全ディレクトリ名
            List<string> directories = new List<string>();

            foreach (string dirname in dir_entries.Keys)
            {
                directories.Add(dirname);
            }
            if (directories.Contains(source_path))
                directories.Remove(source_path);

            //全ファイル数
            int all_files_count = files.Count;
            all_compressed_files = new file_entry[all_files_count];

            //file entryを用意する
            UInt32 act_file = 0;
            //全ファイル数 + 全ディレクトリ数（source_path は除く）
            file_index = new string[all_files_count + dir_entries.Count];

            //現在のfile indexを指す idx
            UInt32 index_pos = 0;

            if (dir_entries.ContainsKey(source_path))
            {
                //int i = 0;

                string act_file_index_path = "";

                //現在のディレクトリ上にある全ファイル名（ディレクトリは含まない）について繰り返す
                foreach (string file in dir_entries[source_path])
                {
                    {
                        //名無し対応
                        try
                        {
                            //名無しなのでファイル名はない
                            //<idx>_<hash>.<ext>
                            string fparts0 = file;
                            string[] ary_1 = fparts0.Split(new string[] { "_" }, StringSplitOptions.RemoveEmptyEntries);
                            string fparts1 = ary_1[ary_1.Length - 1];//= <hash>.<ext>
                            string fparts2 = fparts1.Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries)[0];//<hash> before dot
                            //this should be the string of the hash value
                            //hash値を控える
                            all_compressed_files[act_file].hash_value = System.UInt32.Parse(fparts2);
                        }
                        //名無しでなかった
                        catch (Exception)
                        {
                            file_index[index_pos] = file;
                            //entry ファイル名を控える
                            all_compressed_files[act_file].file_name = act_file_index_path + file_index[index_pos];
                            index_pos++;
                        }
                    }
                    try
                    {
                        //実ファイル名を控える
                        all_compressed_files[act_file].true_file_name = Path.Combine(source_path, file);
                        act_file++;
                    }
                    catch (Exception ex)
                    {
                        System.Console.Out.WriteLine(ex);
                        return -1;
                    }
                }
            }

            for (int i = 0; i < directories.Count; i++)
            {
                string act_file_index_path = "";
                {
                    //ディレクトリ名を控える
                    //ただし source_path + 1 文字飛ばす
                    //いつも '/' で終わる
                    file_index[index_pos] = directories[i].Substring(source_path.Length + 1) + "\\";
                    file_index[index_pos] = file_index[index_pos].Replace("\\", "/");
                    act_file_index_path = file_index[index_pos];
                    index_pos++;
                }
                //現在のディレクトリ上にある全ファイル名（ディレクトリは含まない）について繰り返す
                foreach (string file in dir_entries[directories[i]])
                {
                    {
                        file_index[index_pos] = file;
                        //entry ファイル名を控える
                        all_compressed_files[act_file].file_name = act_file_index_path + file_index[index_pos];
                        index_pos++;
                    }
                    try
                    {
                        //実ファイル名を控える
                        all_compressed_files[act_file].true_file_name = Path.Combine(directories[i], file);
                        act_file++;
                    }
                    catch (Exception ex)
                    {
                        System.Console.Out.WriteLine(ex);
                        return -1;
                    }
                }
            }
            return 0;
        }

        public Stream get_file_entry_stream(ref file_entry compressed_file)
        {
            return File.OpenRead(compressed_file.true_file_name);
        }

        //entry情報
        byte[] b_file_index = null;
        UInt32 b_file_index_count = 0;
        //entry情報（圧縮後）
        byte[] compressed_file_index = null;
        UInt32 compressed_file_index_length = 0;

        byte[] compressed_file_index_s = null;

        public int encrypt_archive(string file_path_name, string source_path)
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
                System.Console.Out.WriteLine("Error: Could not rename existing TAH file. Possibly there is already a file with the '.bak' ending from a previous session. Please do something about it. TAHdecrypter will not overwrite existing data and therefore aborts here.");
                return -1;
            }

            //read in files from source path, do not compress them now.
            //全ディレクトリ名
            string[] directories = Directory.GetDirectories(source_path, "*", SearchOption.AllDirectories);

            {
                string dirname = source_path;
                string[] files = Directory.GetFiles(dirname);
                foreach (string file in files)
                    add_file(file);
            }
            for (int i = 0; i < directories.Length; i++)
            {
                string dirname = directories[i];
                string[] files = Directory.GetFiles(dirname);
                foreach (string file in files)
                    add_file(file);
            }

            string[] file_index;
            file_entry[] all_compressed_files;

            build_file_indices(source_path, out file_index, out all_compressed_files);
            build_compressed_file_indices(file_index);
            return write(file_path_name, all_compressed_files);
        }

        int build_compressed_file_indices(string[] file_index)
        {
            //entry情報

            //全entry情報長さ
            b_file_index_count = 0;
            for (int i = 0; i < file_index.Length; i++)
            {
                if (file_index[i] != null)
                {
                    //+1はnull終端
                    b_file_index_count += (UInt32)(file_index[i].Length + 1);
                }
            }

            //ディレクトリ名 ('/' 終端) + ファイル名 (basename) をnull終端で1列に格納する
            b_file_index = new byte[b_file_index_count + 3];//savety margin for encryption...
            b_file_index.Initialize();

            UInt32 b_file_index_pos = 0;
            for (int i = 0; i < file_index.Length; i++)
            {
                if (file_index[i] != null)
                {
                    byte[] partial_index = System.Text.Encoding.ASCII.GetBytes(file_index[i]);
                    Array.Copy(partial_index, 0, b_file_index, (int)b_file_index_pos, partial_index.Length);
                    //+1はnull終端
                    b_file_index_pos += (UInt32)(partial_index.Length + 1);
                }
            }
            //-- entry情報格納完了! --

            Compression.encrypt(ref b_file_index, b_file_index_count, ref compressed_file_index, ref compressed_file_index_length);
            //-- entry情報圧縮完了! --

            //xxx: copyする必要はあるか???
            compressed_file_index_s = new byte[compressed_file_index_length];
            Array.Copy(compressed_file_index, 0, compressed_file_index_s, 0, compressed_file_index_length);

            return 0;
        }

        int write(string file_path_name, file_entry[] all_compressed_files)
        {
            UInt32 all_files_count = (UInt32)all_compressed_files.Length;

            //now everything is set up for writing the tah file...
            BinaryWriter writer = new BinaryWriter(File.Create(file_path_name));
            writer.Write(System.Text.Encoding.ASCII.GetBytes("TAH2"));
            writer.Write(all_files_count);
            writer.Write(((UInt32)1));//TAH version
            writer.Write(((UInt32)0));

            //+4は b_file_index_count (Uint32) 格納領域
            UInt32 offset = 16 + 8 * all_files_count + compressed_file_index_length + 4;
            //writer needs this defined offset for adding length lists of the compressed data later on
            writer.BaseStream.Seek(offset, SeekOrigin.Begin);
            //全ファイルについて繰り返し（ディレクトリは含まない）
            for (int i = 0; i < all_compressed_files.Length; i++)
            {
                try
                {
                    //dataをファイルから読む
                    Stream input_stream = get_file_entry_stream(ref all_compressed_files[i]);
                    BinaryReader reader = new BinaryReader(input_stream);
                    byte[] data_input = reader.ReadBytes((int)reader.BaseStream.Length);
                    //圧縮前長さを控える
                    all_compressed_files[i].uncompressed_length = (UInt32)reader.BaseStream.Length;
                    reader.Close();
                    //-- data読み込み完了! --

                    ////xxx: copyする必要はあるか???
                    byte[] encrypt_data_input = new byte[data_input.Length + 3]; //with safety margin for encryption
                    Array.Copy(data_input, 0, encrypt_data_input, 0, (int)data_input.Length);
                    byte[] compressed_data = null;
                    UInt32 compressed_length = 0;
                    Compression.encrypt(ref encrypt_data_input, (UInt32)data_input.Length, ref compressed_data, ref compressed_length);
                    //-- data圧縮完了! --

                    //圧縮後長さを控える
                    all_compressed_files[i].compressed_length = compressed_length;
                    ////xxx: copyする必要はあるか???
                    all_compressed_files[i].compressed_data = new byte[compressed_length];
                    Array.Copy(compressed_data, 0, all_compressed_files[i].compressed_data, 0, (int)compressed_length);

                    System.Console.Out.WriteLine(String.Format("Compressing File: {0} {1}", all_compressed_files[i].true_file_name, all_compressed_files[i].uncompressed_length));

                    writer.Write(all_compressed_files[i].uncompressed_length);
                    writer.Write(all_compressed_files[i].compressed_data);
                    writer.Flush();
                    //-- data書き出し完了! --

                    if (i > 0)
                    {
                        //dataはもう不要なので削除
                        all_compressed_files[i - 1].compressed_data = new byte[] { };
                    }
                }
                catch (Exception ex)
                {
                    System.Console.Out.WriteLine(ex);
                    return -1;
                }
            }
            //now resetting file offset to write file index table

            writer.BaseStream.Seek(16, SeekOrigin.Begin);

            for (int i = 0; i < all_files_count; i++)
            {
                if (all_compressed_files[i].file_name == null)
                {
                    writer.Write(all_compressed_files[i].hash_value);
                }
                else
                {
                    byte[] fname = System.Text.Encoding.ASCII.GetBytes(all_compressed_files[i].file_name);
                    byte[] fname2 = new byte[fname.Length + 1];
                    fname2.Initialize();
                    fname.CopyTo(fname2, 0);
                    writer.Write(gen_hash_key_for_string(ref fname2));
                }
                writer.Write(offset);
                offset += all_compressed_files[i].compressed_length + 4;
            }
            writer.Write(b_file_index_count);
            writer.Write(compressed_file_index_s);
            writer.Flush();
            writer.Close();
            return 0;
        }

        public static UInt32 gen_hash_key_for_string(ref byte[] strg)
        {
            UInt32 key = 0xC8A4E57AU;
            UInt32 i;

            //lower string
            string tstrg = System.Text.Encoding.ASCII.GetString(strg);
            tstrg = tstrg.ToLower();
            byte[] tbstrg = System.Text.Encoding.ASCII.GetBytes(tstrg);

            for (i = 0; tbstrg[i] != 0x00; i++)
            {
                key = key << 19 | key >> 13;
                key = key ^ (uint)tbstrg[i];
            }

            return (uint)(key ^ (((tbstrg[i - 1] & 0x1A) != 0x00 ? -1 : 0)));
        }

    }
