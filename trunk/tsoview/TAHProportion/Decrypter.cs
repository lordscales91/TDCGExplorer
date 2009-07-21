using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace TAHTool
{
    public class TAHEntry
    {
        public UInt32 hash_name;
        public UInt32 offset;
        public string file_name;
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
            public UInt32 index_entry_count;
            public UInt32 unknown; //1
            public UInt32 reserved; //0
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

        public void extract_TAH_directory()
        {
            Entries = null;

            UInt32 arc_size = (UInt32)reader.BaseStream.Length;

            header tah_header = new header();

            byte[] magic = reader.ReadBytes(4);

            if (magic[0] != (byte)'T' || magic[1] != (byte)'A' || magic[2] != (byte)'H' || magic[3] != (byte)'2')
                throw new Exception("File is not TAH");

            tah_header.index_entry_count = reader.ReadUInt32();
            tah_header.unknown = reader.ReadUInt32();
            tah_header.reserved = reader.ReadUInt32();

            UInt32 index_buffer_size = tah_header.index_entry_count * 8; //sizeof(index_entry) == 8
            Entries = new TAHEntry[tah_header.index_entry_count];

            for (int i = 0; i < tah_header.index_entry_count; i++)
            {
                Entries[i] = new TAHEntry();

                Entries[i].hash_name = reader.ReadUInt32();
                Entries[i].offset = reader.ReadUInt32();
            }

            UInt32 output_length = reader.ReadUInt32();

            //entry情報の読み出し長さ
            UInt32 input_length = Entries[0].offset - /*sizeof(header)*/ 16 - index_buffer_size;
            //entry情報の読み出しバッファ
            byte[] data_input = new byte[input_length];

            data_input = reader.ReadBytes((int)input_length);
            //-- entry情報の読み込み完了! --

            byte[] output_data = new byte[output_length];

            Decompression.decrypt(ref data_input, input_length, ref output_data, output_length);
            //-- entry情報の復号完了! --

            build_TAHEntries(output_data, arc_size);
        }

        public void build_TAHEntries(byte[] str_file_path, UInt32 arc_size)
        {
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
                    for (h = 0; h < Entries.Length; h++)
                    {
                        //名無しで
                        if (Entries[h].file_name == null)
                        {
                            //hashが一致する
                            if (hash_key == Entries[h].hash_name)
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

            for (UInt32 i = 0; i < Entries.Length; i++)
            {
                //file_nameが見つからなかった場合
                if (Entries[i].file_name == null)
                {
                    //names.txt を検索
                    int pos = Array.BinarySearch(external_files.hashkeys, Entries[i].hash_name);
                    if (pos < 0) // not found
                    {
                        //ファイル名は <i>_<hash>にする
                        Entries[i].file_name = i.ToString("00000000") + "_" + Entries[i].hash_name.ToString();
                        //file_nameが見つからなかったflag on
                        Entries[i].flag ^= 0x1;
                    }
                    else
                    {
                        Entries[i].file_name = external_files.files[pos];
                    }
                }
                //オフセットを設定
                //Entries[i].offset = index_buffer[i].offset;
            }

            for (UInt32 i = 0; i < Entries.Length - 1; i++)
            {
                //data読み込み長さを設定
                //読み込み長さは現在entryオフセットと次のentryオフセットとの差である
                Entries[i].length = Entries[i + 1].offset - Entries[i].offset;
            }
            //最終entry data読み込み長さを設定
            Entries[Entries.Length - 1].length = arc_size - Entries[Entries.Length - 1].offset;
        }

        static void build_ext_file_list(out ext_file_list external_files)
        {
            //read in names.txt file when it exists.
            external_files = new ext_file_list();
            string names_path = Path.Combine(Application.StartupPath, @"names.txt");
            if (File.Exists(names_path))
            {
                StreamReader reader = new StreamReader(File.OpenRead(names_path));
                List<string> known_files = new List<string>();
                System.Console.Out.WriteLine("Reading \"names.txt\" at " + Application.StartupPath + ".");
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
            if (reader != null)
                reader.Close();
        }
    }
}
