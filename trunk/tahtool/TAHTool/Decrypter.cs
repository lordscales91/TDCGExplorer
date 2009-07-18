using System;
using System.Collections.Generic;
using System.IO;

    class Decrypter
    {
        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                DumpFiles(args[0]);
            }
        }

        static int DumpFiles(string source_file)
        {
            Decrypter myDecrypter = new Decrypter();
            myDecrypter.Load(source_file);

            string base_path = Path.GetFileNameWithoutExtension(source_file);

            Console.WriteLine("file_name\toffset\tlength\tflag");
            entry_meta_info info;
            while (myDecrypter.FindNext(out info))
            {
                string file_name = System.Text.Encoding.ASCII.GetString(info.file_name);
                Console.WriteLine("{0}\t{1}\t{2}\t{3}", file_name, info.offset, info.length, info.flag);

                byte[] data_output;
                myDecrypter.ExtractResource(ref info, out data_output);

                string dest_file_name = Path.Combine(base_path, file_name);
                Directory.CreateDirectory(Path.GetDirectoryName(dest_file_name));

                BinaryWriter file_writer = new BinaryWriter(File.Create(dest_file_name));
                file_writer.Write(data_output);
                file_writer.Close();
            }
            myDecrypter.Close();

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
                Decrypter myDecrypter = new Decrypter();
                return myDecrypter.decrypt_archive(source_file, dest_path);
            }
            else
            {
                //encrypt to TAH archive
                return -1;
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

        public struct ext_file_list
        {
            public string[] files;
            public UInt32[] hashkeys;
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

        public struct index_entry
        {
            public UInt32 hash_name;
            public UInt32 offset;
        }

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

        static unsafe void Copy(byte[] src, int srcIndex,
            byte[] dst, int dstIndex, int count)
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

            for (UInt32 i = 0; i < tah_header.index_entry_count; i++)
            {
                //file_nameが見つからなかった場合
                if (directory_meta_info_buffer[i].file_name == null)
                {
                    //ファイル名の先頭はhash値にする
                    directory_meta_info_buffer[i].file_name = System.Text.Encoding.ASCII.GetBytes(i.ToString("00000000") + "_" + index_buffer[i].hash_name.ToString());
                    //file_nameが見つからなかったflag on
                    directory_meta_info_buffer[i].flag ^= 0x1;
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

        int ExtractResource(ref entry_meta_info ent_meta_info, out byte[] data_output)
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
            ext_file_list external_files;
            build_ext_file_list(out external_files);

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

                bool filename_found_in_list = false;

                UInt32 hashkey = 0;
                try
                {
                    hashkey = UInt32.Parse(write_file_str.Substring(write_file_str.LastIndexOf("_") + 1));
                }
                catch (Exception) { }

                //names.txt があるなら
                if (external_files.files != null)
                {
                    //names.txt を検索
                    int pos = Array.BinarySearch(external_files.hashkeys, hashkey);
                    if (pos >= 0)
                    {
                        write_file_str = write_file_str.Substring(0, write_file_str.LastIndexOf("/"));
                        write_file_str += "/" + external_files.files[pos];
                        filename_found_in_list = true;
                    }
                }

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

                //flag = 1ならno path
                if (dir_meta_info.entry_meta_infos[i].flag % 2 == 1)
                {
                    //had no path name encoded in tah file
                    //names.txt で見つからなかった場合
                    if (!filename_found_in_list)
                    {
                        if (System.Text.Encoding.ASCII.GetString(data_output, 0, 4).Contains("8BPS"))
                        {
                            write_file_str += ".psd";
                        }
                        else if (System.Text.Encoding.ASCII.GetString(data_output, 0, 4).Contains("TMO1"))
                        {
                            write_file_str += ".tmo";
                        }
                        else if (System.Text.Encoding.ASCII.GetString(data_output, 0, 4).Contains("TSO1"))
                        {
                            write_file_str += ".tso";
                        }
                        else if (System.Text.Encoding.ASCII.GetString(data_output, 0, 4).Contains("OggS"))
                        {
                            write_file_str += ".ogg";
                        }
                        else if (System.Text.Encoding.ASCII.GetString(data_output, 0, 4).Contains("BBBB"))
                        {
                            write_file_str += ".tbn";
                        }
                        else
                        {
                            write_file_str += ".cgfx";
                        }
                    }
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
