using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace TDCG.TAHTool
{
    /// <summary>
    /// BinaryReaderの拡張メソッドを定義します。
    /// </summary>
    public static class BinaryReaderMethods
    {
        /// <summary>
        /// null終端文字列を読みとります。
        /// </summary>
        /// <returns>文字列</returns>
        public static string ReadCString(this BinaryReader reader)
        {
            StringBuilder string_builder = new StringBuilder();
            while ( true ) {
                char c = reader.ReadChar();
                if (c == 0)
                        break;
                string_builder.Append(c);
            }
            return string_builder.ToString();
        }
    }

    public class TAHEntry
    {
        public UInt32 hash_name;
        public UInt32 offset;
        public string file_name;
        public UInt32 length;
        public UInt32 flag; //at bit 0x1: no path info in tah file 1 otherwise 0
    }

    public class Decrypter
    {
        BinaryReader reader;

        public struct ext_file_list
        {
            public string[] files;
            public UInt32[] hashkeys;
        }

        public struct header
        {
            public UInt32 index_entry_count;
            public UInt32 version; //1
            public UInt32 reserved; //0
        }

        public static UInt32 CalcHash(string s)
        {
            UInt32 key = 0xC8A4E57AU;

            byte[] buf = Encoding.Default.GetBytes(s.ToLower());

            foreach (byte i in buf)
            {
                key = key << 19 | key >> 13;
                key = key ^ (uint)i;
            }

            return (uint)(key ^ (((buf[buf.Length - 1] & 0x1A) != 0x00 ? -1 : 0)));
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
            tah_header.version = reader.ReadUInt32();
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

            TAHCryption.crypt(ref data_input, input_length, output_length);
            Decompression.infrate(ref data_input, input_length, ref output_data, output_length);
            //-- entry情報の復号完了! --

            build_TAHEntries(output_data, arc_size);
        }

        public TAHEntry FindTAHEntryByHash(UInt32 hash)
        {
            foreach (TAHEntry ent in Entries)
            {
                if (ent.hash_name == hash)
                    return ent;
            }
            return null;
        }

        public void build_TAHEntries(byte[] str_file_path, UInt32 arc_size)
        {
            using (BinaryReader br = new BinaryReader(new MemoryStream(str_file_path)))
            {
                try
                {
                    string path = "";
                    while (true)
                    {
                        string name = br.ReadCString();
                        if (name.EndsWith("/"))
                        {
                            path = name;
                        }
                        else
                        {
                            string file_name = path + name;
                            TAHEntry ent = FindTAHEntryByHash(CalcHash(file_name));
                            if (ent != null)
                                ent.file_name = file_name;
                        }
                    }
                }
                catch (EndOfStreamException)
                {
                }
            }

            ext_file_list external_files;
            build_ext_file_list(out external_files);

            for (UInt32 i = 0; i < Entries.Length; i++)
            {
                //file_nameが見つからなかった場合
                if (Entries[i].file_name == null)
                {
                    //names.txt を検索
                    int pos = -1;
                    if (external_files.files != null)
                        pos = Array.BinarySearch(external_files.hashkeys, Entries[i].hash_name);
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
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    known_files.Add(line);
                }
                //map a list of hash keys to the file...
                external_files.files = known_files.ToArray();
                if (external_files.files != null)
                {
                    external_files.hashkeys = new UInt32[external_files.files.Length];
                    for (int i = 0; i < external_files.files.Length; i++)
                    {
                        external_files.hashkeys[i] = CalcHash(external_files.files[i]);
                    }
                }
                //sorting for faster look up...
                Array.Sort(external_files.hashkeys, external_files.files);
            }
        }

        public byte[] ExtractResource(TAHEntry entry)
        {
            //data読み込み長さ
            //-4はdata書き出し長さ格納領域 (UInt32) を減じている
            UInt32 input_length = entry.length - 4;
            //data読み込みバッファ
            byte[] data_input = new byte[input_length];
            UInt32 output_length;

            reader.BaseStream.Position = entry.offset;

            //data書き出し長さ
            output_length = reader.ReadUInt32();
            data_input = reader.ReadBytes((int)input_length);
            //-- data読み込み（復号前）完了! --

            //data書き出しバッファ
            byte[] data_output = new byte[output_length];

            {
                TAHCryption.crypt(ref data_input, input_length, output_length);
                Decompression.infrate(ref data_input, input_length, ref data_output, output_length);
            }
            //-- data復号完了! --
            return data_output;
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

        public void Open(string source_file)
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
