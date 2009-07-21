using System;
using System.Collections.Generic;
using System.IO;

    public class TAHEntry
    {
        public string file_name;
        public UInt32 offset;
        public UInt32 length;
        public UInt32 flag; //at bit 0x1: no path info in tah file 1 otherwise 0
    }

    class Decrypter
    {
        BinaryReader reader;

        static UInt32 MAX_PATH = 260;

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

        public void extract_TAH_directory()
        {
            Entries = null;

            UInt32 arc_size = (UInt32)reader.BaseStream.Length;

            header tah_header = new header();

            tah_header.id = reader.ReadUInt32();
            tah_header.index_entry_count = reader.ReadUInt32();
            tah_header.unknown = reader.ReadUInt32();
            tah_header.reserved = reader.ReadUInt32();

            if (!file_header_match_with_TAH(tah_header.id))
            {
                throw new ApplicationException("Wrong file format. Please use a TAH archive as input file.");
            }

            UInt32 index_buffer_size = tah_header.index_entry_count * 8; //sizeof(index_entry) == 8
            index_entry[] index_buffer = new index_entry[tah_header.index_entry_count];

            for (int i = 0; i < tah_header.index_entry_count; i++)
            {
                index_buffer[i].hash_name = reader.ReadUInt32();
                index_buffer[i].offset = reader.ReadUInt32();
            }

            UInt32 output_length = reader.ReadUInt32();

            //entry情報の読み出し長さ
            UInt32 input_length = index_buffer[0].offset - /*sizeof(header)*/ 16 - index_buffer_size;
            //entry情報の読み出しバッファ
            byte[] data_input = new byte[input_length];

            data_input = reader.ReadBytes((int)input_length);
            //-- entry情報の読み込み完了! --

            byte[] output_data = new byte[output_length];

            Decompression.decrypt(ref data_input, input_length, ref output_data, output_length);
            //-- entry情報の復号完了! --

            build_TAHEntrys(output_data, index_buffer, arc_size);
        }

        public void build_TAHEntrys(byte[] str_file_path, index_entry[] index_buffer, UInt32 arc_size)
        {
            int index_entry_count = index_buffer.Length;

            Entries = new TAHEntry[index_entry_count];

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
                    int str_path_offset = 0;
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
                    for (h = 0; h < index_entry_count; h++)
                    {
                        //名無しで
                        if (Entries[h].file_name == null)
                        {
                            //hashが一致する
                            if (hash_key == index_buffer[h].hash_name)
                            {
                                //file_nameとしてcopy
                                Entries[h].file_name = System.Text.Encoding.GetEncoding(932).GetString(str_path, 0, i + str_path_offset);
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

            for (UInt32 i = 0; i < index_entry_count; i++)
            {
                //file_nameが見つからなかった場合
                if (Entries[i].file_name == null)
                {
                    //names.txt を検索
                    int pos = Array.BinarySearch(external_files.hashkeys, index_buffer[i].hash_name);
                    if (pos < 0) // not found
                    {
                        //ファイル名は <i>_<hash>にする
                        Entries[i].file_name = i.ToString("00000000") + "_" + index_buffer[i].hash_name.ToString();
                        //file_nameが見つからなかったflag on
                        Entries[i].flag ^= 0x1;
                    }
                    else
                    {
                        Entries[i].file_name = external_files.files[pos];
                    }
                }
                //オフセットを設定
                Entries[i].offset = index_buffer[i].offset;
            }

            for (UInt32 i = 0; i < index_entry_count - 1; i++)
            {
                //data読み込み長さを設定
                //読み込み長さは現在entryオフセットと次のentryオフセットとの差である
                Entries[i].length = index_buffer[i + 1].offset - index_buffer[i].offset;
            }
            //最終entry data読み込み長さを設定
            Entries[index_entry_count - 1].length = arc_size - index_buffer[index_entry_count - 1].offset;
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

        public int ExtractResource(TAHEntry entry, out byte[] data_output)
        {
            //data読み込み長さ
            //-4はdata書き出し長さ格納領域 (UInt32) を減じている
            UInt32 data_input_length = entry.length - 4;
            //data読み込みバッファ
            byte[] data_input = new byte[data_input_length];
            UInt32 data_output_length;

            reader.BaseStream.Position = entry.offset;

            try
            {
                //data書き出し長さ
                data_output_length = reader.ReadUInt32();
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
                Decompression.decrypt(ref data_input, data_input_length, ref data_output, data_output_length);
            }
            catch (Exception)
            {
                System.Console.Out.WriteLine("Error: Failed to decrypt data. Possible error in archive.");
                return -1;
            }
            //-- data復号完了! --
            return 0;
        }

        public string[] GetFiles(string source_file)
        {
            reader = new BinaryReader(File.OpenRead(source_file));
            extract_TAH_directory();
            reader.Close();

            //
            //Entries.collect file_name
            //
            string[] files = new string[Entries.Length];
            for (int count = 0; count < Entries.Length; count++)
            {
                files[count] = Entries[count].file_name;
            }
            return files;
        }

        public TAHEntry[] Entries { get; set; }

        public void Load(string source_file)
        {
            reader = new BinaryReader(File.OpenRead(source_file));
            extract_TAH_directory();
        }

        public void Close()
        {
            reader.Close();
        }
    }
