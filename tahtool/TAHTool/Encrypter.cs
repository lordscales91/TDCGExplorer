using System;
using System.Collections.Generic;
using System.IO;

    class Encrypter
    {
        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                string source_file = args[0];
                //encrypt to TAH archive
                if (Directory.Exists(source_file))
                {
                    //launch encrypt routine from here...
                    Encrypter myEncrypter = new Encrypter();
                    myEncrypter.encrypt_archive(source_file + ".tah", source_file);
                }
            }
        }

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

        public struct LZSS_Hash
        {
            public UInt32[] nPos; //4 entries max (no linked lists)
        }
        public static LZSS_Hash[] m_HashTable;

        public struct tah_file
        {
            public header tah_header;
            public file_entry[] all_compressed_files;
        }

        public struct file_entry
        {
            public byte[] compressed_data;
            public string true_file_name;
            public UInt32 compressed_length;
            public UInt32 uncompressed_length;
            public string file_name;
            public UInt32 hash_value; //only for entries with file_name == null
        }

        public struct header
        {
            public UInt32 id; //TAH2 (843596116)
            public UInt32 index_entry_count;
            public UInt32 unknown; //1
            public UInt32 reserved; //0
        }

        static unsafe void Copy(byte[] src, int srcIndex, byte[] dst, int dstIndex, int count)
        {
            if (src == null || srcIndex < 0 ||
                dst == null || dstIndex < 0 || count < 0)
            {
                throw new ArgumentException();
            }
            int srcLen = src.Length;
            int dstLen = dst.Length;
            if (srcLen - srcIndex < count ||
                dstLen - dstIndex < count)
            {
                throw new ArgumentException();
            }


            // The following fixed statement pins the location of
            // the src and dst objects in memory so that they will
            // not be moved by garbage collection.          
            fixed (byte* pSrc = src, pDst = dst)
            {
                byte* ps = pSrc + srcIndex;
                byte* pd = pDst + dstIndex;

                // Loop over the count in blocks of 4 bytes, copying an
                // integer (4 bytes) at a time:
                for (int n = 0; n < count / 4; n++)
                {
                    *((int*)pd) = *((int*)ps);
                    pd += 4;
                    ps += 4;
                }

                // Complete the copy by moving any bytes that weren't
                // moved in blocks of 4:
                for (int n = 0; n < count % 4; n++)
                {
                    *pd = *ps;
                    pd++;
                    ps++;
                }
            }
        }

        List<string> files = new List<string>();

        void add_file(string file)
        {
            files.Add(file);
            //Console.WriteLine("add {0}", file);
        }

        int build_file_indices(string source_path, out string[] file_index, out tah_file tah_output_data, out UInt32 all_files_count)
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
            all_files_count = (uint)files.Count;

            tah_output_data = new tah_file();
            tah_output_data.all_compressed_files = new file_entry[all_files_count];

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
                            tah_output_data.all_compressed_files[act_file].hash_value = System.UInt32.Parse(fparts2);
                        }
                        //名無しでなかった
                        catch (Exception)
                        {
                            file_index[index_pos] = file;
                            //entry ファイル名を控える
                            tah_output_data.all_compressed_files[act_file].file_name = act_file_index_path + file_index[index_pos];
                            index_pos++;
                        }
                    }
                    try
                    {
                        //実ファイル名を控える
                        tah_output_data.all_compressed_files[act_file].true_file_name = Path.Combine(source_path, file);
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
                        tah_output_data.all_compressed_files[act_file].file_name = act_file_index_path + file_index[index_pos];
                        index_pos++;
                    }
                    try
                    {
                        //実ファイル名を控える
                        tah_output_data.all_compressed_files[act_file].true_file_name = Path.Combine(directories[i], file);
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
            tah_file tah_output_data;
            UInt32 all_files_count;

            build_file_indices(source_path, out file_index, out tah_output_data, out all_files_count);
            build_compressed_file_indices(file_index);
            return write(file_path_name, ref tah_output_data, ref all_files_count);
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
                    Copy(partial_index, 0, b_file_index, (int)b_file_index_pos, partial_index.Length);
                    //+1はnull終端
                    b_file_index_pos += (UInt32)(partial_index.Length + 1);
                }
            }
            //-- entry情報格納完了! --

            encrypt(ref b_file_index, b_file_index_count, ref compressed_file_index, ref compressed_file_index_length);
            //-- entry情報圧縮完了! --

            //xxx: copyする必要はあるか???
            compressed_file_index_s = new byte[compressed_file_index_length];
            Copy(compressed_file_index, 0, compressed_file_index_s, 0, (int)compressed_file_index_length);

            return 0;
        }

        int write(string file_path_name, ref tah_file tah_output_data, ref UInt32 all_files_count)
        {
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
            for (int i = 0; i < tah_output_data.all_compressed_files.Length; i++)
            {
                try
                {
                    //dataをファイルから読む
                    Stream input_stream = get_file_entry_stream(ref tah_output_data.all_compressed_files[i]);
                    BinaryReader reader = new BinaryReader(input_stream);
                    byte[] data_input = reader.ReadBytes((int)reader.BaseStream.Length);
                    //圧縮前長さを控える
                    tah_output_data.all_compressed_files[i].uncompressed_length = (UInt32)reader.BaseStream.Length;
                    reader.Close();
                    //-- data読み込み完了! --

                    ////xxx: copyする必要はあるか???
                    byte[] encrypt_data_input = new byte[data_input.Length + 3]; //with safety margin for encryption
                    Copy(data_input, 0, encrypt_data_input, 0, (int)data_input.Length);
                    byte[] compressed_data = null;
                    UInt32 compressed_length = 0;
                    encrypt(ref encrypt_data_input, (UInt32)data_input.Length, ref compressed_data, ref compressed_length);
                    //-- data圧縮完了! --

                    //圧縮後長さを控える
                    tah_output_data.all_compressed_files[i].compressed_length = compressed_length;
                    ////xxx: copyする必要はあるか???
                    tah_output_data.all_compressed_files[i].compressed_data = new byte[compressed_length];
                    Copy(compressed_data, 0, tah_output_data.all_compressed_files[i].compressed_data, 0, (int)compressed_length);

                    System.Console.Out.WriteLine(String.Format("Compressing File: {0} {1}", tah_output_data.all_compressed_files[i].true_file_name, tah_output_data.all_compressed_files[i].uncompressed_length));

                    writer.Write(tah_output_data.all_compressed_files[i].uncompressed_length);
                    writer.Write(tah_output_data.all_compressed_files[i].compressed_data);
                    writer.Flush();
                    //-- data書き出し完了! --

                    if (i > 0)
                    {
                        //dataはもう不要なので削除
                        tah_output_data.all_compressed_files[i - 1].compressed_data = new byte[] { };
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
                if (tah_output_data.all_compressed_files[i].file_name == null)
                {
                    writer.Write(tah_output_data.all_compressed_files[i].hash_value);
                }
                else
                {
                    byte[] fname = System.Text.Encoding.ASCII.GetBytes(tah_output_data.all_compressed_files[i].file_name);
                    byte[] fname2 = new byte[fname.Length + 1];
                    fname2.Initialize();
                    fname.CopyTo(fname2, 0);
                    writer.Write(gen_hash_key_for_string(ref fname2));
                }
                writer.Write(offset);
                offset += tah_output_data.all_compressed_files[i].compressed_length + 4;
            }
            writer.Write(b_file_index_count);
            writer.Write(compressed_file_index_s);
            writer.Flush();
            writer.Close();
            return 0;
        }

        // modified:
        // (c)2002 Jonathan Bennett (jon@hiddensoft.com)

        /*LZSS compressor*/
        public static void encrypt(ref byte[] data_input, UInt32 input_length, ref byte[] data_output, ref UInt32 output_length)
        {

            // Set up initial values
            UInt32 m_nDataStreamPos = 0;				// We are at the start of the input data
            UInt32 m_nCompressedStreamPos = 0;			// We are at the start of the compressed data
            HS_LZSS_OUTPUT_BUFFER = 0;
            HS_LZSS_OUTPUT_BUFFER_POS = 0;
            data_output = new byte[1024 * 1024]; //1MB buffer

            //for encryption...
            uint[] init_key = new uint[4];
            byte[] rnd = new byte[1024];

            init_key[0] = (input_length | 0x80) >> 5;
            init_key[1] = (input_length << 9) | 6;
            init_key[2] = (input_length << 6) | 4;
            init_key[3] = (input_length | 0x48) >> 3;
            myMT19937ar.init_by_array(init_key, 4);
            for (UInt32 i = 0; i < 1024; i++)
            {
                rnd[i] = (byte)(((uint)myMT19937ar.genrand_int32()) >> (int)(i % 7));
            }

            UInt32 seed = (((input_length / 1000) % 10) + ((input_length / 100) % 10) + ((input_length / 10) % 10) + (input_length % 10)) & 0x31A;


            // If the input file is too small then there is a chance of 
            // buffer overrun, so just abort
            if (input_length < 32)
            {
                data_output = data_input;
                return;
            }

            // Initialize our hash table
            HashTableInit();

            //
            // Jump to our main compression function
            //

            encrypt_loop(ref data_input, input_length, ref data_output, ref output_length, ref m_nDataStreamPos, ref m_nCompressedStreamPos, ref rnd, ref seed);
        }

        public static void encrypt_loop(ref byte[] data_input, UInt32 input_length, ref byte[] data_output, ref UInt32 output_length, ref UInt32 nInputPos, ref UInt32 nOutputPos, ref byte[] rnd, ref UInt32 seed)
        {
            UInt32 nOffset1, nLen1;					// n Offset values
            UInt32 nOffset2, nLen2;					// n+1 Offset values (lazy evaluation)
            UInt32 nDataPos1;						// N data pos temp values
            UInt32 nTempDataPos;
            UInt32 nHash;
            UInt32 nTempUINT;
            UInt32 nWPos;

            nOffset1 = 0;
            nOffset2 = 0;
            nLen1 = 0;
            nLen2 = 0;

            // Loop around until there is no more data
            int flag_loop = 0;
            while (nInputPos < input_length)
            {
                // Store where we are before we go crazy with lazy evaluation stuff
                nTempDataPos = nInputPos;

                // Match 
                FindMatches(ref nOffset1, ref nLen1, ref nInputPos, ref data_input); // Search for matches for current position
                nDataPos1 = nInputPos;			// Store the data pos if we use this match

                // Match + 1
                nInputPos = nTempDataPos + 1;	// Reset the stream pointer for a match at +1

                // Check that we are not at the end of the data for doing a match at +1
                if (nInputPos < input_length)
                    FindMatches(ref nOffset2, ref nLen2, ref nInputPos, ref data_input); // Search for matches for current position +1
                else
                {
                    // We will overrun the end of the data buffer by doing a second match
                    // fool the routine into thinking that 2nd match was blank
                    nOffset2 = 0;
                    nLen2 = 0;
                }

                if (nInputPos > 32860)
                {
                    int dummy = 0;
                }
                // Are there ANY good matches?
                if ((nOffset1 != 0) || (nOffset2 != 0))
                {
                    // Which was the best match? N or N+1?
                    if (nLen1 >= nLen2)
                    {
                        // Let's use match 1
                        nInputPos = nDataPos1;	// Restore the data pos for this match
                        //WriteBitsToDataOutput(1, 1, ref data_output, ref nOutputPos);
                        WriteBitsToDataOutput(0, 1, ref data_output, ref nOutputPos, flag_loop % 8, ref rnd, ref seed);
                        WriteBitsToDataOutput(inverse_byte_order(nOffset1 - HS_LZSS_MINMATCHLEN, nInputPos, nLen1), HS_LZSS_WINBITS, ref data_output, ref nOutputPos, -1, ref rnd, ref seed);
                        WriteBitsToDataOutput(nLen1 - HS_LZSS_MINMATCHLEN, HS_LZSS_MATCHBITS, ref data_output, ref nOutputPos, -1, ref rnd, ref seed);
                        flag_loop++;
                    }
                    else
                    {
                        // Remember m_nDataStreamPos WILL ALREADY BE VALID for match +1
                        // Let's use match 2 (a little more work required)
                        // First, store the byte at N as a literal
                        //WriteBitsToDataOutput(0, 1, ref data_output, ref nOutputPos);
                        WriteBitsToDataOutput(1, 1, ref data_output, ref nOutputPos, flag_loop % 8, ref rnd, ref seed);
                        WriteBitsToDataOutput(data_input[nTempDataPos], 8, ref data_output, ref nOutputPos, -1, ref rnd, ref seed);
                        flag_loop++;

                        // Then store match 2
                        //m_nDataStreamPos = nDataPos2;	// Restore the data pos for this match
                        //WriteBitsToDataOutput(1, 1, ref data_output, ref nOutputPos);
                        WriteBitsToDataOutput(0, 1, ref data_output, ref nOutputPos, flag_loop % 8, ref rnd, ref seed);
                        WriteBitsToDataOutput(inverse_byte_order(nOffset2 - HS_LZSS_MINMATCHLEN, nInputPos, nLen2), HS_LZSS_WINBITS, ref data_output, ref nOutputPos, -1, ref rnd, ref seed);
                        WriteBitsToDataOutput(nLen2 - HS_LZSS_MINMATCHLEN, HS_LZSS_MATCHBITS, ref data_output, ref nOutputPos, -1, ref rnd, ref seed);
                        flag_loop++;
                    }
                }
                else
                {
                    // No matches, just store the literal byte
                    nInputPos = nTempDataPos;
                    //WriteBitsToDataOutput(0, 1, ref data_output, ref nOutputPos);
                    WriteBitsToDataOutput(1, 1, ref data_output, ref nOutputPos, flag_loop % 8, ref rnd, ref seed);
                    WriteBitsToDataOutput(data_input[nInputPos++], 8, ref data_output, ref nOutputPos, -1, ref rnd, ref seed);
                    flag_loop++;
                }

                // We have skipped forwards either 1 byte or xxx bytes (if matched) we must now
                // add entries in the hash table for all the entries we've skipped

                if (nTempDataPos < HS_LZSS_WINLEN)
                    nWPos = 0;
                else
                    nWPos = nTempDataPos - HS_LZSS_WINLEN;

                nTempUINT = nInputPos - nTempDataPos;	// How many bytes to hash?
                while ((nTempUINT > 0) && (nTempDataPos + 2 < data_input.Length))
                {
                    nTempUINT--;
                    nHash = (UInt32)((40543 * ((((data_input[nTempDataPos] << 4) ^ data_input[nTempDataPos + 1]) << 4) ^ data_input[nTempDataPos + 2])) >> 4) & 0xFFF;
                    HashTableAdd(nHash, nTempDataPos, nWPos);
                    nTempDataPos++;

                }  // End while

            } // End while
            //there might be data left in the output bit bufer HS_LZSS_OUTPUT_BUFFER... write it out into the data_output buffer
            if (data_output.Length <= nOutputPos + 1)
            {
                //running out of preallocated memory in the data_output array
                //add 1MB to buffer
                byte[] tmp_data = new byte[data_output.Length + 1024 * 1024];
                data_output.CopyTo(tmp_data, 0);
                data_output = tmp_data;
            }
            //flushing remaining data in the buffers
            if (HS_LZSS_OUTPUT_BUFFER_POS != 0)
            {
                while (HS_LZSS_OUTPUT_BUFFER_POS < 8)
                {
                    // Make room for another bit (shift left once)
                    HS_LZSS_OUTPUT_BUFFER = (byte)(HS_LZSS_OUTPUT_BUFFER << 1);
                    // Update how many bits we are using (add 1)
                    HS_LZSS_OUTPUT_BUFFER_POS++;
                }
                seed = (seed + 1) & 0x3ff;
                data_output[nOutputPos++] = (byte)((HS_LZSS_OUTPUT_BUFFER) ^ rnd[seed]);
            }
            if (flag_loop % 8 != 0)
            {
                while ((flag_loop % 8) != 0)
                {
                    HS_LZSS_OUTPUT_BUFFER_FLAG = (byte)(HS_LZSS_OUTPUT_BUFFER_FLAG >> (byte)0x01);
                    HS_LZSS_OUTPUT_BUFFER_FLAG |= (byte)(0x80);
                    flag_loop++;
                }
                data_output[HS_LZSS_OUTPUT_BUFFER_FLAG_POS] = (byte)(HS_LZSS_OUTPUT_BUFFER_FLAG ^ rnd[HS_LZSS_OUTPUT_BUFFER_FLAG_SEED]);
            }
            // We've now written out everything... reset the buffer
            HS_LZSS_OUTPUT_BUFFER_POS = 0;
            HS_LZSS_OUTPUT_BUFFER_FLAG_POS = 0;
            // empty HS_LZSS_OUTPUT_BUFFER
            HS_LZSS_OUTPUT_BUFFER = 0;
            HS_LZSS_OUTPUT_BUFFER_FLAG = 0;
            output_length = nOutputPos;
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

        public static UInt32 inverse_byte_order(UInt32 nValue, UInt32 nInputPos, UInt32 nLen)
        {
            UInt32 ret_value = 0;
            //the decrypter does not move backwards in the
            //search window when retrieving copy data,
            //but moves forward from the offset at 4080.
            //So any backward slide number given in nValue
            //must be transformed into an absolute file
            //offset.
            nValue = nInputPos - (nValue + HS_LZSS_MINMATCHLEN + nLen);
            //prepare nValue for decrypter win offset...
            nValue += 4080;
            nValue &= 4095;
            //assume 12bit data length
            ret_value |= (UInt32)(nValue & 0xFF);
            ret_value <<= 4;
            ret_value |= (UInt32)((nValue >> 8) & 0xF);
            return ret_value;
        }

        public static void WriteBitsToDataOutput(UInt32 nValue, UInt32 nNumBits, ref byte[] bOutputData, ref UInt32 nPosOutput, int flag_loop, ref byte[] rnd, ref UInt32 seed)
        {
            if (flag_loop != -1)
            {
                if (flag_loop == 0)
                {
                    HS_LZSS_OUTPUT_BUFFER_FLAG_POS = nPosOutput;
                    nPosOutput++;
                    seed = (seed + 1) & 0x3ff;
                    HS_LZSS_OUTPUT_BUFFER_FLAG_SEED = seed;
                    HS_LZSS_OUTPUT_BUFFER_FLAG = 0x00;
                    HS_LZSS_OUTPUT_BUFFER_FLAG |= (byte)(nValue * 0x80);
                }
                else if (flag_loop == 7)
                {
                    HS_LZSS_OUTPUT_BUFFER_FLAG = (byte)(HS_LZSS_OUTPUT_BUFFER_FLAG >> (byte)0x01);
                    HS_LZSS_OUTPUT_BUFFER_FLAG |= (byte)(nValue * 0x80);
                    bOutputData[HS_LZSS_OUTPUT_BUFFER_FLAG_POS] = (byte)(HS_LZSS_OUTPUT_BUFFER_FLAG ^ rnd[HS_LZSS_OUTPUT_BUFFER_FLAG_SEED]);
                }
                else
                {
                    HS_LZSS_OUTPUT_BUFFER_FLAG = (byte)(HS_LZSS_OUTPUT_BUFFER_FLAG >> (byte)0x01);
                    HS_LZSS_OUTPUT_BUFFER_FLAG |= (byte)(nValue * 0x80);
                }
            }
            else
            {
                while (nNumBits > 0)
                {
                    nNumBits--;

                    // Make room for another bit (shift left once)
                    HS_LZSS_OUTPUT_BUFFER = (byte)(HS_LZSS_OUTPUT_BUFFER << 1);

                    // Merge (OR) our value into the temporary long
                    HS_LZSS_OUTPUT_BUFFER = (byte)(HS_LZSS_OUTPUT_BUFFER | ((nValue >> (int)nNumBits) & 0x00000001));

                    // Update how many bits we are using (add 1)
                    HS_LZSS_OUTPUT_BUFFER_POS++;

                    // Now check if we have filled our temporary long with bits (32bits)
                    if (HS_LZSS_OUTPUT_BUFFER_POS == 8)
                    {
                        if (bOutputData.Length <= nPosOutput + 1)
                        {
                            //running out of preallocated memory in the data_output array
                            //add 1MB to buffer
                            byte[] tmp_data = new byte[bOutputData.Length + 1024 * 1024];
                            bOutputData.CopyTo(tmp_data, 0);
                            bOutputData = tmp_data;
                        }
                        seed = (seed + 1) & 0x3ff;
                        bOutputData[nPosOutput++] = (byte)((HS_LZSS_OUTPUT_BUFFER) ^ rnd[seed]);

                        // We've now written out 8 bits
                        HS_LZSS_OUTPUT_BUFFER_POS = 0;
                        HS_LZSS_OUTPUT_BUFFER = 0x00;
                    }

                } // End while
            }

        }

        public static void FindMatches(ref UInt32 nOffset, ref UInt32 nLen, ref UInt32 nInputPos, ref byte[] bInputData)
        {
            UInt32 nTempWPos, nWPos, nDPos;	// Temp Window and Data position markers
            UInt32 nTempLen;					// Temp vars 
            UInt32 nBestOffset, nBestLen;		// Stores the best match so far
            UInt32 nHash;

            // Reset all variables
            nBestOffset = 0;
            nBestLen = HS_LZSS_MINMATCHLEN - 1;

            // Get our window start position, if the window would take us beyond
            // the start of the file, just use 0
            if (nInputPos < HS_LZSS_WINLEN)
                nWPos = 0;
            else
                nWPos = nInputPos - HS_LZSS_WINLEN;

            // Generate a hash of the next three chars
            nHash = (UInt32)((40543 * ((((bInputData[nInputPos] << 4) ^ bInputData[nInputPos + 1]) << 4) ^ bInputData[nInputPos + 2])) >> 4) & 0xFFF;

            // Main loop

            for (int i = 0; i < 4; i++)
            {
                if (m_HashTable[nHash].nPos != null)
                {
                    nTempWPos = m_HashTable[nHash].nPos[i];
                    if ((nTempWPos < nWPos) && (m_HashTable[nHash].nPos[i] != HS_LZSS_HASHTABLE_EMPTY_ENTRY))
                    {
                        //remove it
                        m_HashTable[nHash].nPos[i] = HS_LZSS_HASHTABLE_EMPTY_ENTRY;
                    }
                    else if (m_HashTable[nHash].nPos[i] != HS_LZSS_HASHTABLE_EMPTY_ENTRY)
                    {
                        nDPos = nInputPos;
                        nTempLen = 0;
                        while ((bInputData[nTempWPos] == bInputData[nDPos]) && (nTempWPos < nInputPos) &&
                                (nDPos < bInputData.Length - 1) && (nTempLen < HS_LZSS_MATCHLEN))
                        {
                            nTempLen++; nTempWPos++; nDPos++;
                        }
                        // See if this match was better than previous match
                        if (nTempLen > nBestLen)
                        {
                            nBestLen = nTempLen;
                            nBestOffset = nInputPos - m_HashTable[nHash].nPos[i];
                        }
                    }
                }
                else
                {
                    break;
                }
            }


            // Setup our return values of bestoffset and bestlen, bestoffset will be 0
            // if no good matches were match
            nOffset = nBestOffset;
            nLen = nBestLen;

            // Update our data stream pointer if we had a good match
            if (nOffset != 0)
                nInputPos = nInputPos + nLen;

        }

        public static void HashTableInit()
        {
            m_HashTable = new LZSS_Hash[HS_LZSS_HASHTABLE_SIZE];
            m_HashTable.Initialize();
        }

        public static void HashTableAdd(UInt32 nHash, UInt32 nPos, UInt32 nTooOldPos)
        {
            if (m_HashTable[nHash].nPos != null)
            {
                //search for the first empty entry (HS_LZSS_HASHTABLE_EMPTY_ENTRY)
                for (int i = 0; i < 4; i++)
                {
                    if ((m_HashTable[nHash].nPos[i] == HS_LZSS_HASHTABLE_EMPTY_ENTRY) || (m_HashTable[nHash].nPos[i] < nTooOldPos))
                    {
                        m_HashTable[nHash].nPos[i] = nPos;
                        break;
                    }
                    else if (i == 3)
                    {
                        //full, so replace this entry
                        m_HashTable[nHash].nPos[i] = nPos;
                        break;
                    }
                }
            }
            else
            {
                //first entry...
                m_HashTable[nHash].nPos = new UInt32[4];
                m_HashTable[nHash].nPos[0] = nPos;
                m_HashTable[nHash].nPos[1] = HS_LZSS_HASHTABLE_EMPTY_ENTRY;
                m_HashTable[nHash].nPos[2] = HS_LZSS_HASHTABLE_EMPTY_ENTRY;
                m_HashTable[nHash].nPos[3] = HS_LZSS_HASHTABLE_EMPTY_ENTRY;
            }
        }
    }
