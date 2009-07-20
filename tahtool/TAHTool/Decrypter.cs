using System;
using System.Collections.Generic;
using System.IO;

        public struct entry_meta_info
        {
            public byte[] file_name;
            public UInt32 offset;
            public UInt32 length;
            public UInt32 flag; //at bit 0x1: no path info in tah file 1 otherwise 0
        }

        public struct directory_meta_info
        {
            public UInt32 index_entry_count;
            public entry_meta_info[] entry_meta_infos;
        }

    class Decrypter
    {
        /*Many thanks to the author of the original TAH decrypter for crass.
         *I just ripped and converted parts of the code from C++ to C#.
         *I'ld like to contact and credit the author of this code but could
         *not yet find him/her. (Idk japanese/chinese)
         *I believe I have to thank ﾂｳﾃ閉拮ﾂｹﾂｫﾃ兔 though I could be wrong... Am I right?*/

        static MT19937ar myMT19937ar = new MT19937ar();

        BinaryReader reader;

        static UInt32 MAX_PATH = 260;

        // Window sizing related stuff compressor
        public static uint HS_LZSS_MINMATCHLEN = 3;
        public static uint HS_LZSS_WINBITS = 12;
        public static uint HS_LZSS_WINLEN = 4091; //otherwise there will be data loops
        public static uint HS_LZSS_MATCHBITS = 4;
        public static uint HS_LZSS_MATCHLEN = 18;
        public static uint HS_LZSS_HASHTABLE_SIZE = 4096;
        public static uint HS_LZSS_HASHTABLE_EMPTY_ENTRY = 0xFFFFFFFF;
        public static byte HS_LZSS_OUTPUT_BUFFER = 0;
        public static UInt32 HS_LZSS_OUTPUT_BUFFER_POS = 0;
        public static byte HS_LZSS_OUTPUT_BUFFER_FLAG = 0;
        public static UInt32 HS_LZSS_OUTPUT_BUFFER_FLAG_POS = 0;
        public static UInt32 HS_LZSS_OUTPUT_BUFFER_FLAG_SEED = 0;

        public struct ext_file_list
        {
            public string[] files;
            public UInt32[] hashkeys;
        }

        public struct header
        {
            public UInt32 id; //TAH2 (843596116)
            public UInt32 index_entry_count;
            public UInt32 unknown; //1
            public UInt32 reserved; //0
        }

        public struct index_entry
        {
            public UInt32 hash_name;
            public UInt32 offset;
        }

        public int decrypt_archive(string source_file, string dest_path)
        {
            directory_meta_info directory_meta_infos = new directory_meta_info();
            try
            {
                reader = new BinaryReader(File.OpenRead(source_file));
            }
            catch (Exception)
            {
                System.Console.Out.WriteLine("Error: This file cannot be read or does not exist.");
                return -1;
            }
            int ret = 0;
            ret = extract_TAH_directory(ref reader, ref directory_meta_infos);
            if (ret >= 0)
            {
                ret = extract_TAH_resource(ref reader, dest_path, ref directory_meta_infos);
            }
            reader.Close();
            return ret;
        }

        /*LZSS decompressor*/
        public static void decrypt(ref byte[] data_input, UInt32 input_length, ref byte[] data_output, UInt32 output_length)
        {
            UInt32 act_output_length = 0;
            UInt32 act_byte = 0;
            UInt32 nActWindowByte = 0xff0;
            const UInt32 win_size = 4096;
            byte[] win = new byte[4096];
            ushort flag = 0;
            uint[] init_key = new uint[4];
            byte[] rnd = new byte[1024];

            init_key[0] = (output_length | 0x80) >> 5;
            init_key[1] = (output_length << 9) | 6;
            init_key[2] = (output_length << 6) | 4;
            init_key[3] = (output_length | 0x48) >> 3;
            myMT19937ar.init_by_array(init_key, 4);
            for (UInt32 i = 0; i < 1024; i++)
            {
                rnd[i] = (byte)(((uint)myMT19937ar.genrand_int32()) >> (int)(i % 7));
            }

            UInt32 seed = (((output_length / 1000) % 10) + ((output_length / 100) % 10) + ((output_length / 10) % 10) + (output_length % 10)) & 0x31A;
            win.Initialize();

            while (act_output_length < output_length)
            {
                flag >>= 1;
                if (act_output_length >= 32860)
                {
                    int dummy = 0;
                }
                if ((flag & 0x0100) == 0)
                {
                    seed = (seed + 1) & 0x3ff;
                    flag = (ushort)((((ushort)(data_input[act_byte++] ^ rnd[seed]))) | 0xff00);
                }

                seed = (seed + 1) & 0x3ff;

                if ((flag & 1) != 0)
                {
                    byte data;

                    data = (byte)(data_input[act_byte++] ^ rnd[seed]);
                    data_output[act_output_length++] = data;
                    win[nActWindowByte++] = data;
                    nActWindowByte &= win_size - 1;
                }
                else
                {
                    UInt32 copy_bytes, win_offset;
                    UInt32 i;

                    win_offset = (UInt32)(data_input[act_byte++] ^ rnd[seed]);
                    seed = (seed + 1) & 0x3ff;
                    copy_bytes = (UInt32)(data_input[act_byte++] ^ rnd[seed]);
                    win_offset |= (copy_bytes & 0xf0) << 4;
                    copy_bytes &= 0x0f;
                    copy_bytes += 3;

                    for (i = 0; i < copy_bytes; i++)
                    {
                        byte data;

                        data = win[(win_offset + i) & (win_size - 1)];
                        data_output[act_output_length++] = data;
                        win[nActWindowByte++] = data;
                        nActWindowByte &= win_size - 1;
                        if (act_output_length >= output_length)
                        {
                            return;
                        }
                    }
                }
            }

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

        /* TAH Procedures*/

        public static bool file_header_match_with_TAH(UInt32 id)
        {
            if (id == 843596116)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static int extract_TAH_directory(ref BinaryReader file_reader, ref directory_meta_info dir_meta_info)
        {
            header tah_header;
            UInt32 arc_size;

            if (file_reader.BaseStream.Length > 16) //sizeof(header) == 16
            {
                arc_size = (UInt32)file_reader.BaseStream.Length;
            }
            else
            {
                return -1;
            }

            tah_header = new header();

            try
            {
                tah_header.id = System.BitConverter.ToUInt32(file_reader.ReadBytes(4), 0);
                tah_header.index_entry_count = System.BitConverter.ToUInt32(file_reader.ReadBytes(4), 0);
                tah_header.unknown = System.BitConverter.ToUInt32(file_reader.ReadBytes(4), 0);
                tah_header.reserved = System.BitConverter.ToUInt32(file_reader.ReadBytes(4), 0);
            }
            catch (Exception)
            {
                System.Console.Out.WriteLine("Error: Cannot read out header from the archive.");
                return -1;
            }

            if (!file_header_match_with_TAH(tah_header.id))
            {
                System.Console.Out.WriteLine("Error: Wrong file format. Please use a TAH archive as input file.");
                return -1;
            }

            UInt32 index_buffer_size = tah_header.index_entry_count * 8; //sizeof(index_entry) == 8
            index_entry[] index_buffer = new index_entry[tah_header.index_entry_count];

            try
            {
                for (int i = 0; i < tah_header.index_entry_count; i++)
                {
                    index_buffer[i].hash_name = System.BitConverter.ToUInt32(file_reader.ReadBytes(4), 0);
                    index_buffer[i].offset = System.BitConverter.ToUInt32(file_reader.ReadBytes(4), 0);
                }
            }
            catch (Exception)
            {
                System.Console.Out.WriteLine("Error: Cannot read out index of the archive.");
                return -1;
            }

            UInt32 output_length;

            try
            {
                output_length = System.BitConverter.ToUInt32(file_reader.ReadBytes(4), 0);
            }
            catch (Exception)
            {
                System.Console.Out.WriteLine("Error: Cannot read out data size information of the archive.");
                return -1;
            }

            //entry情報の読み出し長さ
            UInt32 input_length = index_buffer[0].offset - /*sizeof(header)*/ 16 - index_buffer_size;
            //entry情報の読み出しバッファ
            byte[] data_input = new byte[input_length];

            try
            {
                data_input = file_reader.ReadBytes((int)input_length);
            }
            catch (Exception)
            {
                System.Console.Out.WriteLine("Error: Cannot read out compressed data of the archive.");
                return -1;
            }
            //-- entry情報の読み込み完了! --

            byte[] output_data = new byte[output_length];

            try
            {
                decrypt(ref data_input, input_length, ref output_data, output_length);
            }
            catch (Exception)
            {
                System.Console.Out.WriteLine("Error: Failed to decrypt data. Possible error in archive.");
                return -1;
            }
            //-- entry情報の復号完了! --

            entry_meta_info[] directory_meta_info_buffer = new entry_meta_info[tah_header.index_entry_count];

            byte[] str_file_path = new byte[output_data.Length];
            output_data.CopyTo(str_file_path, 0);//xxx: copyして持つ必要はあるか???

            byte[] file_path = new byte[MAX_PATH];
            int act_str_pos = 0;
            while (str_file_path.Length > act_str_pos)
            {
                int pos_local = 0;
                //0x00か0x2Fに達するまで回す
                while (str_file_path[act_str_pos + pos_local] != 0x00)
                {
                    if (str_file_path[act_str_pos + pos_local] == 0x2F) // '/'
                    {
                        break;
                    }
                    else
                    {
                        pos_local++;
                    }
                }
                //0x00でない（つまり0x2Fである）場合 = ディレクトリ名
                if (str_file_path[act_str_pos + pos_local] != 0x00)
                {
                    //0x00に達するまで回す
                    int i;
                    for (i = 0; str_file_path[act_str_pos + i] != 0x00; i++)
                    {
                        file_path[i] = str_file_path[i + act_str_pos];
                    }
                    file_path[i] = 0x00;
                }
                //0x00である場合 = ファイル名
                else
                {
                    byte[] str_path = new byte[MAX_PATH];

                    //0x00の位置まで先頭から回す = ディレクトリ名
                    uint str_path_offset = 0;
                    while (file_path[str_path_offset] != 0x00)
                    {
                        str_path_offset++;
                    }
                    file_path.CopyTo(str_path, 0);

                    //actが0x00に達するまで回す
                    int i;
                    for (i = 0; str_file_path[act_str_pos + i] != 0x00; i++)
                    {
                        str_path[i + str_path_offset] = str_file_path[act_str_pos + i];
                    }
                    str_path[i + str_path_offset] = 0x00;

                    //str_pathからhashを作る
                    UInt32 hash_key = gen_hash_key_for_string(ref str_path);

                    //index entryでhashを先頭から検索
                    UInt32 h;
                    for (h = 0; h < tah_header.index_entry_count; h++)
                    {
                        //名無しで
                        if (directory_meta_info_buffer[h].file_name == null)
                        {
                            //hashが一致する
                            if (hash_key == index_buffer[h].hash_name)
                            {
                                //i + str_path_offset以降をfile_nameとしてcopy
                                directory_meta_info_buffer[h].file_name = new byte[i + str_path_offset]; int k = 0;
                                while (k < (i + str_path_offset)) { directory_meta_info_buffer[h].file_name[k] = str_path[k]; k++; }
                                break;
                            }
                        }
                    }
                }
                while (str_file_path[act_str_pos] != 0x00)
                {
                    act_str_pos++;
                }
                act_str_pos += 1;
            }

            ext_file_list external_files;
            build_ext_file_list(out external_files);

            for (UInt32 i = 0; i < tah_header.index_entry_count; i++)
            {
                //file_nameが見つからなかった場合
                if (directory_meta_info_buffer[i].file_name == null)
                {
                    //names.txt を検索
                    int pos = Array.BinarySearch(external_files.hashkeys, index_buffer[i].hash_name);
                    if (pos < 0) // not found
                    {
                        //ファイル名の先頭はhash値にする
                        directory_meta_info_buffer[i].file_name = System.Text.Encoding.ASCII.GetBytes(i.ToString("00000000") + "_" + index_buffer[i].hash_name.ToString() );
                        //file_nameが見つからなかったflag on
                        directory_meta_info_buffer[i].flag ^= 0x1;
                    }
                    else
                    {
                        directory_meta_info_buffer[i].file_name = System.Text.Encoding.ASCII.GetBytes(external_files.files[pos]);
                    }
                }
                //オフセットを設定
                directory_meta_info_buffer[i].offset = index_buffer[i].offset;
            }

            for (UInt32 i = 0; i < tah_header.index_entry_count - 1; i++)
            {
                //data読み込み長さを設定
                //読み込み長さは現在entryオフセットと次のentryオフセットとの差である
                directory_meta_info_buffer[i].length = index_buffer[i + 1].offset - index_buffer[i].offset;
            }
            //最終entry data読み込み長さを設定
            directory_meta_info_buffer[tah_header.index_entry_count - 1].length = arc_size - index_buffer[tah_header.index_entry_count - 1].offset;

            dir_meta_info.index_entry_count = tah_header.index_entry_count;
            dir_meta_info.entry_meta_infos = directory_meta_info_buffer;

            return 0;
        }

        static void build_ext_file_list(out ext_file_list external_files)
        {
            //read in names.txt file when it exists.
            external_files = new ext_file_list();
            if (File.Exists("names.txt"))
            {
                StreamReader reader = new StreamReader(File.OpenRead("names.txt"));
                List<string> known_files = new List<string>();
                System.Console.Out.WriteLine("Reading \"names.txt\" at " + System.Environment.CurrentDirectory.ToString() + ".");
                while (reader.BaseStream.Position < reader.BaseStream.Length)
                {
                    known_files.Add(reader.ReadLine());
                }
                //map a list of hash keys to the file...
                external_files.files = known_files.ToArray();
                if (external_files.files != null)
                {
                    external_files.hashkeys = new UInt32[external_files.files.Length];
                    for (int i = 0; i < external_files.files.Length; i++)
                    {
                        byte[] byte_string = System.Text.Encoding.ASCII.GetBytes(external_files.files[i] + "\0");
                        external_files.hashkeys[i] = gen_hash_key_for_string(ref byte_string);
                    }
                }
                //sorting for faster look up...
                Array.Sort(external_files.hashkeys, external_files.files);
            }
            else
            {
                System.Console.Out.WriteLine("Could not find \"names.txt\" at " + System.Environment.CurrentDirectory.ToString() + ".");
                System.Console.Out.WriteLine("Press any Key to continue with data extraction without correct file names.");
                System.Console.ReadKey();
            }
        }

        public int ExtractResource(ref entry_meta_info ent_meta_info, out byte[] data_output)
        {
            //data読み込み長さ
            //-4はdata書き出し長さ格納領域 (UInt32) を減じている
            UInt32 data_input_length = ent_meta_info.length - 4;
            //data読み込みバッファ
            byte[] data_input = new byte[data_input_length];
            UInt32 data_output_length;

            reader.BaseStream.Position = ent_meta_info.offset;

            try
            {
                //data書き出し長さ
                data_output_length = System.BitConverter.ToUInt32(reader.ReadBytes(4), 0);
                data_input = reader.ReadBytes((int)data_input_length);
            }
            catch (Exception)
            {
                System.Console.Out.WriteLine("Error: Cannot read out compressed data of the archive.");
                data_output = new byte[0];
                return -1;
            }
            //-- data読み込み（復号前）完了! --

            //data書き出しバッファ
            data_output = new byte[data_output_length];

            try
            {
                decrypt(ref data_input, data_input_length, ref data_output, data_output_length);
            }
            catch (Exception)
            {
                System.Console.Out.WriteLine("Error: Failed to decrypt data. Possible error in archive.");
                return -1;
            }
            //-- data復号完了! --
            return 0;
        }

        static int extract_TAH_resource_0(ref BinaryReader file_reader, ref entry_meta_info ent_meta_info, out byte[] data_output)
        {
            //data読み込み長さ
            //-4はdata書き出し長さ格納領域 (UInt32) を減じている
            UInt32 data_input_length = ent_meta_info.length - 4;
            //data読み込みバッファ
            byte[] data_input = new byte[data_input_length];
            UInt32 data_output_length;

            file_reader.BaseStream.Position = ent_meta_info.offset;

            try
            {
                //data書き出し長さ
                data_output_length = System.BitConverter.ToUInt32(file_reader.ReadBytes(4), 0);
                data_input = file_reader.ReadBytes((int)data_input_length);
            }
            catch (Exception)
            {
                System.Console.Out.WriteLine("Error: Cannot read out compressed data of the archive.");
                data_output = new byte[0];
                return -1;
            }
            //-- data読み込み（復号前）完了! --

            //data書き出しバッファ
            data_output = new byte[data_output_length];

            try
            {
                decrypt(ref data_input, data_input_length, ref data_output, data_output_length);
            }
            catch (Exception)
            {
                System.Console.Out.WriteLine("Error: Failed to decrypt data. Possible error in archive.");
                return -1;
            }
            //-- data復号完了! --
            return 0;
        }

        static int extract_TAH_resource(ref BinaryReader file_reader, string dest_path, ref directory_meta_info dir_meta_info)
        {
            //now proceed with decrypting
            for (int i = 0; i < dir_meta_info.index_entry_count; i++)
            {
                //data書き出しバッファ
                byte[] data_output;

                int ret;
                ret = extract_TAH_resource_0(ref file_reader, ref dir_meta_info.entry_meta_infos[i], out data_output);
                if (ret < 0)
                    return ret;

                //書き出しファイル名
                string write_file_str = dest_path;
                int tcnt = 0;
                while ((dir_meta_info.entry_meta_infos[i].file_name[tcnt] != 0x00) && (tcnt < (dir_meta_info.entry_meta_infos[i].file_name.Length - 1))) { tcnt++; }
                write_file_str += "/" + System.Text.Encoding.ASCII.GetString(dir_meta_info.entry_meta_infos[i].file_name, 0, tcnt + 1);
                //write_file_str =  write_file_str.Replace("/", "\\");

                UInt32 hashkey = 0;
                try
                {
                    hashkey = UInt32.Parse(write_file_str.Substring(write_file_str.LastIndexOf("_") + 1));
                }
                catch (Exception) { }

                try
                {
                    string test_directory = write_file_str.Substring(write_file_str.IndexOf("/"));
                    string[] sep = new string[1];
                    sep[0] = "/";
                    string[] dir_parts = test_directory.Split(sep, System.StringSplitOptions.RemoveEmptyEntries);

                    //test base level directory
                    if (!Directory.Exists(dest_path))
                    {
                        Directory.CreateDirectory(dest_path);
                    }
                    int l;
                    test_directory = dest_path;
                    for (l = 0; l < dir_parts.Length - 1; l++)
                    {
                        test_directory += "/" + dir_parts[l];
                        if (!Directory.Exists(test_directory))
                        {
                            Directory.CreateDirectory(test_directory);
                        }
                    }
                    //Does the file already exist?
                    if (File.Exists(write_file_str))
                    {
                        File.Delete(write_file_str);
                    }
                }
                catch (Exception)
                {
                    System.Console.Out.WriteLine("Error: Cannot prepare destination directory for file writing.");
                    return -1;
                }

                //flag & 0x1 = 1ならno path
                if (dir_meta_info.entry_meta_infos[i].flag % 2 == 1)
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
                    write_file_str += ext;
                }

                try
                {
                    BinaryWriter file_writer = new BinaryWriter(File.Create(write_file_str));
                    file_writer.Write(data_output);
                    System.Console.Out.WriteLine(write_file_str);
                    file_writer.Close();
                }
                catch (Exception)
                {
                    System.Console.Out.WriteLine("Error: Cannot write decompressed file.");
                    return -1;
                }
            }
            return 0;
        }

        public string[] GetFiles(string source_file)
        {
            directory_meta_info directory_meta_infos = new directory_meta_info();
            try
            {
                reader = new BinaryReader(File.OpenRead(source_file));
            }
            catch (Exception)
            {
                System.Console.Out.WriteLine("Error: This file cannot be read or does not exist.");
                return null;
            }
            int ret = 0;
            ret = extract_TAH_directory(ref reader, ref directory_meta_infos);
            reader.Close();
            string[] files = new string[directory_meta_infos.index_entry_count];
            for (int count = 0; count < directory_meta_infos.index_entry_count; count++)
            {
                files[count] = System.Text.Encoding.ASCII.GetString(directory_meta_infos.entry_meta_infos[count].file_name);
            }
            return files;
        }

        directory_meta_info m_directory_meta_infos;
        int m_count = 0;

        public void Load(string source_file)
        {
            m_directory_meta_infos = new directory_meta_info();
            m_count = 0;
            try
            {
                reader = new BinaryReader(File.OpenRead(source_file));
            }
            catch (Exception)
            {
                System.Console.Out.WriteLine("Error: This file cannot be read or does not exist.");
                return;
            }
            int ret = 0;
            ret = extract_TAH_directory(ref reader, ref m_directory_meta_infos);
        }

        public void Close()
        {
            reader.Close();
        }

        public bool FindFirst(out entry_meta_info info)
        {
            m_count = 0;
            return FindNext(out info);
        }

        public bool FindNext(out entry_meta_info info)
        {
            bool exist_p = m_count < m_directory_meta_infos.index_entry_count;
            if (exist_p)
                info = m_directory_meta_infos.entry_meta_infos[m_count++];
            else
                info = new entry_meta_info();
            return exist_p;
        }
    }
