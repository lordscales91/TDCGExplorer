using System;
using System.Collections.Generic;
using System.IO;

namespace TAHTool
{
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

        List<string> file_names = new List<string>();

        public int Count
        {
            get
            {
                return file_names.Count;
            }
        }

        public void Add(string file)
        {
            file_names.Add(file);
        }

        public string SourcePath { get; set; }

        int build_file_indices(string source_path, out string[] file_index, out file_entry[] all_compressed_files)
        {
            Dictionary<string, List<string>> dir_entries = new Dictionary<string, List<string>>();

            foreach (string file in file_names)
            {
                string dirname = Path.GetDirectoryName(file).Replace(@"\", "/");
                string basename = Path.GetFileName(file);
                if (!dir_entries.ContainsKey(dirname))
                    dir_entries[dirname] = new List<string>();
                dir_entries[dirname].Add(basename);
            }

            //�S�f�B���N�g����
            List<string> directories = new List<string>();

            foreach (string dirname in dir_entries.Keys)
            {
                directories.Add(dirname);
            }
            if (directories.Contains(source_path))
                directories.Remove(source_path);

            //�S�t�@�C����
            all_compressed_files = new file_entry[file_names.Count];

            //file entry��p�ӂ���
            UInt32 act_file = 0;
            //�S�t�@�C���� + �S�f�B���N�g�����isource_path �͏����j
            file_index = new string[file_names.Count + dir_entries.Count];

            //���݂�file index���w�� idx
            UInt32 index_pos = 0;

            if (dir_entries.ContainsKey(source_path))
            {
                //int i = 0;

                string act_file_index_path = "";

                //���݂̃f�B���N�g����ɂ���S�t�@�C�����i�f�B���N�g���͊܂܂Ȃ��j�ɂ��ČJ��Ԃ�
                foreach (string file in dir_entries[source_path])
                {
                    //�������Ή�
                    try
                    {
                        //�������Ȃ̂Ńt�@�C�����͂Ȃ�
                        //<idx>_<hash>.<ext>
                        string fparts0 = file;
                        string[] ary_1 = fparts0.Split(new string[] { "_" }, StringSplitOptions.RemoveEmptyEntries);
                        string fparts1 = ary_1[ary_1.Length - 1];//= <hash>.<ext>
                        string fparts2 = fparts1.Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries)[0];//<hash> before dot
                        //this should be the string of the hash value
                        //hash�l���T����
                        all_compressed_files[act_file].hash_value = System.UInt32.Parse(fparts2);
                    }
                    //�������łȂ�����
                    catch (Exception)
                    {
                        file_index[index_pos] = file;
                        //entry �t�@�C�������T����
                        all_compressed_files[act_file].file_name = act_file_index_path + file;
                        index_pos++;
                    }

                    //���t�@�C�������T����
                    all_compressed_files[act_file].true_file_name = source_path + "/" + file;
                    act_file++;
                }
            }

            for (int i = 0; i < directories.Count; i++)
            {
                string act_file_index_path = "";
                {
                    //�f�B���N�g�������T����
                    //������ source_path + 1 ������΂�
                    //���� '/' �ŏI���
                    file_index[index_pos] = directories[i].Substring(source_path.Length + 1) + @"/";
                    act_file_index_path = file_index[index_pos];
                    index_pos++;
                }
                //���݂̃f�B���N�g����ɂ���S�t�@�C�����i�f�B���N�g���͊܂܂Ȃ��j�ɂ��ČJ��Ԃ�
                foreach (string file in dir_entries[directories[i]])
                {
                    file_index[index_pos] = file;
                    //entry �t�@�C�������T����
                    all_compressed_files[act_file].file_name = act_file_index_path + file;
                    index_pos++;

                    //���t�@�C�������T����
                    all_compressed_files[act_file].true_file_name = directories[i] + "/" + file;
                    act_file++;
                }
            }
            return 0;
        }

        public delegate Stream GetFileEntryStreamHandler(string true_file_name);
        public GetFileEntryStreamHandler GetFileEntryStream;

        int version = 0x10;
        public int Version { get { return version; } set { version = value;  } }

        //entry���
        byte[] b_file_index = null;
        UInt32 b_file_index_count = 0;
        //entry���i���k��j
        byte[] compressed_file_index = null;
        UInt32 compressed_file_index_length = 0;

        byte[] compressed_file_index_s = null;

        int build_compressed_file_indices(string[] file_index)
        {
            //entry���

            //�Sentry��񒷂�
            b_file_index_count = 0;
            for (int i = 0; i < file_index.Length; i++)
            {
                if (file_index[i] != null)
                {
                    //+1��null�I�[
                    b_file_index_count += (UInt32)(file_index[i].Length + 1);
                }
            }

            //�f�B���N�g���� ('/' �I�[) + �t�@�C���� (basename) ��null�I�[��1��Ɋi�[����
            b_file_index = new byte[b_file_index_count + 3];//savety margin for encryption...
            b_file_index.Initialize();

            UInt32 b_file_index_pos = 0;
            for (int i = 0; i < file_index.Length; i++)
            {
                if (file_index[i] != null)
                {
                    byte[] partial_index = System.Text.Encoding.ASCII.GetBytes(file_index[i]);
                    Array.Copy(partial_index, 0, b_file_index, (int)b_file_index_pos, partial_index.Length);
                    //+1��null�I�[
                    b_file_index_pos += (UInt32)(partial_index.Length + 1);
                }
            }
            //-- entry���i�[����! --

            Compression.encrypt(ref b_file_index, b_file_index_count, ref compressed_file_index, ref compressed_file_index_length);
            //-- entry��񈳏k����! --

            //xxx: copy����K�v�͂��邩???
            compressed_file_index_s = new byte[compressed_file_index_length];
            Array.Copy(compressed_file_index, 0, compressed_file_index_s, 0, compressed_file_index_length);

            return 0;
        }

        public int Save(string file_path_name)
        {
            string[] file_index;
            file_entry[] all_compressed_files;

            build_file_indices(SourcePath, out file_index, out all_compressed_files);
            build_compressed_file_indices(file_index);

            UInt32 all_files_count = (UInt32)all_compressed_files.Length;

            //now everything is set up for writing the tah file...
            BinaryWriter writer = new BinaryWriter(File.Create(file_path_name));
            writer.Write(System.Text.Encoding.ASCII.GetBytes("TAH2"));
            writer.Write(all_files_count);
            writer.Write(((UInt32)version));
            writer.Write(((UInt32)0));

            //+4�� b_file_index_count (Uint32) �i�[�̈�
            UInt32 offset = 16 + 8 * all_files_count + compressed_file_index_length + 4;
            //writer needs this defined offset for adding length lists of the compressed data later on
            writer.BaseStream.Seek(offset, SeekOrigin.Begin);
            //�S�t�@�C���ɂ��ČJ��Ԃ��i�f�B���N�g���͊܂܂Ȃ��j
            for (int i = 0; i < all_compressed_files.Length; i++)
            {
                try
                {
                    //data���t�@�C������ǂ�
                    Stream input_stream = GetFileEntryStream(all_compressed_files[i].true_file_name);
                    BinaryReader reader = new BinaryReader(input_stream);
                    byte[] data_input = reader.ReadBytes((int)reader.BaseStream.Length);
                    //���k�O�������T����
                    all_compressed_files[i].uncompressed_length = (UInt32)reader.BaseStream.Length;
                    reader.Close();
                    //-- data�ǂݍ��݊���! --

                    ////xxx: copy����K�v�͂��邩???
                    byte[] encrypt_data_input = new byte[data_input.Length + 3]; //with safety margin for encryption
                    Array.Copy(data_input, 0, encrypt_data_input, 0, (int)data_input.Length);
                    byte[] compressed_data = null;
                    UInt32 compressed_length = 0;
                    Compression.encrypt(ref encrypt_data_input, (UInt32)data_input.Length, ref compressed_data, ref compressed_length);
                    //-- data���k����! --

                    //���k�㒷�����T����
                    all_compressed_files[i].compressed_length = compressed_length;
                    ////xxx: copy����K�v�͂��邩???
                    all_compressed_files[i].compressed_data = new byte[compressed_length];
                    Array.Copy(compressed_data, 0, all_compressed_files[i].compressed_data, 0, (int)compressed_length);

                    writer.Write(all_compressed_files[i].uncompressed_length);
                    writer.Write(all_compressed_files[i].compressed_data);
                    writer.Flush();
                    //-- data�����o������! --

                    if (i > 0)
                    {
                        //data�͂����s�v�Ȃ̂ō폜
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
}