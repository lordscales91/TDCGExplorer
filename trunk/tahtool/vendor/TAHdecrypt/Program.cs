using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TAHdecrypt
{
    class Program
    {
        static void Main(string[] args)
        {
            string source_file = "";
            long last_key_event = System.DateTime.Now.Ticks;
            System.Console.Out.WriteLine("Simply Drag and Drop a TAH file onto this decrypter.");
            System.Console.Out.WriteLine("The uncrompressed data will then be written to a");
            System.Console.Out.WriteLine("subfolder that is named like the TAH file.");
            if (args.Length > 0)
            {
                if (decrypt_TAH_archive(args[0]) >= 0)
                {
                    return;
                }
            }
            while (true)
            {
                ConsoleKeyInfo CKeyInfo = System.Console.ReadKey(true);
                if (((source_file.Length == 1) && (last_key_event + 10000 < System.DateTime.Now.Ticks)) || (last_key_event + 10000000 < System.DateTime.Now.Ticks))
                {
                    //last event seems not to be Drag and Drop but user input
                    source_file = "";
                }
                last_key_event = System.DateTime.Now.Ticks;
                source_file += CKeyInfo.KeyChar;
                if ((source_file.Length > 1) && source_file[source_file.Length - 1].Equals('"'))
                {
                    System.Console.Out.WriteLine(source_file.Substring(1, source_file.Length - 2));
                    if (decrypt_TAH_archive(source_file) >= 0)
                    {
                        return;
                    }
                }
                else if ((source_file.Length > 1) && !System.Console.KeyAvailable)
                {
                    System.Console.Out.WriteLine(source_file);
                    if (decrypt_TAH_archive(source_file) >= 0)
                    {
                        return;
                    }
                }
            }
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
                if (System.IO.Directory.Exists(source_file))
                {
                    //launch encrypt routine from here...
                    Decrypter myDecrypter = new Decrypter();
                    for (int i = 0; i < file_path.Length - 1; i++)
                    {
                        dest_path += file_path[i] + "\\";
                    }
                    System.Console.Out.WriteLine(dest_path);
                    return myDecrypter.encrypt_archive(file_name + ".tah", dest_path, source_file);
                }
                return -1;
            }
        }
    }

    class MT19937ar
    {
        //A C# adaption
        /* 
          A C-program for MT19937, with initialization improved 2002/1/26.
          Coded by Takuji Nishimura and Makoto Matsumoto.

          Before using, initialize the state by using init_genrand(seed)  
          or init_by_array(init_key, key_length).

          Copyright (C) 1997 - 2002, Makoto Matsumoto and Takuji Nishimura,
          All rights reserved.                          

          Redistribution and use in source and binary forms, with or without
          modification, are permitted provided that the following conditions
          are met:

            1. Redistributions of source code must retain the above copyright
               notice, this list of conditions and the following disclaimer.

            2. Redistributions in binary form must reproduce the above copyright
               notice, this list of conditions and the following disclaimer in the
               documentation and/or other materials provided with the distribution.

            3. The names of its contributors may not be used to endorse or promote 
               products derived from this software without specific prior written 
               permission.

          THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
          "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
          LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
          A PARTICULAR PURPOSE ARE DISCLAIMED.  IN NO EVENT SHALL THE COPYRIGHT OWNER OR
          CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
          EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
          PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
          PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
          LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
          NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
          SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.


          Any feedback is very welcome.
          http://www.math.sci.hiroshima-u.ac.jp/~m-mat/MT/emt.html
          email: m-mat @ math.sci.hiroshima-u.ac.jp (remove space)
       */

        /* Period parameters */
        static short N = 624;
        static short M = 397;
        static uint MATRIX_A = 0x9908b0dfU;   /* constant vector a */
        static uint UPPER_MASK = 0x80000000U; /* most significant w-r bits */
        static uint LOWER_MASK = 0x7fffffffU; /* least significant r bits */
        private static uint[] mag01 = { 0x0, MATRIX_A };

        static uint[] mt = new uint[N]; /* the array for the state vector  */
        static short mti = (short)(N + (short)(1)); /* mti==N+1 means mt[N] is not initialized */

        public void init_genrand(uint s)
        {
            //we do not want arithmetic overflow checks here,
            //since it is part of what this code does
            unchecked
            {
                mt[0] = s & 0xffffffffU;
                for (mti = 1; mti < N; mti++)
                {
                    mt[mti] =
                    (1812433253U * (mt[mti - 1] ^ (mt[mti - 1] >> 30)) + (uint)mti);
                    /* See Knuth TAOCP Vol2. 3rd Ed. P.106 for multiplier. */
                    /* In the previous versions, MSBs of the seed affect   */
                    /* only MSBs of the array mt[].                        */
                    /* 2002/01/09 modified by Makoto Matsumoto             */
                    mt[mti] &= 0xffffffffU;
                    /* for >32 bit machines */
                }
            }
        }


        public void init_by_array(uint[] init_key, short key_length)
        {
            //we do not want arithmetic overflow checks here,
            //since it is part of what this code does
            unchecked
            {
                UInt32 i, j, k;
                init_genrand(19650218U);
                i = 1; j = 0;
                k = (uint)(N > key_length ? N : key_length);
                for (; k > 0; k--)
                {
                    mt[i] = (mt[i] ^ ((mt[i - 1] ^ (mt[i - 1] >> 30)) * 1664525U))
                      + init_key[j] + j; /* non linear */
                    mt[i] &= 0xffffffffU; /* for WORDSIZE > 32 machines */
                    i++; j++;
                    if (i >= N) { mt[0] = mt[N - 1]; i = 1; }
                    if (j >= key_length) j = 0;
                }
                for (k = (uint)N - 1; k > 0; k--)
                {
                    mt[i] = (mt[i] ^ ((mt[i - 1] ^ (mt[i - 1] >> 30)) * 1566083941U))
                      - i; /* non linear */
                    mt[i] &= 0xffffffffU; /* for WORDSIZE > 32 machines */
                    i++;
                    if (i >= N) { mt[0] = mt[N - 1]; i = 1; }
                }

                mt[0] = 0x80000000U; /* MSB is 1; assuring non-zero initial array */
            }
        }

        /* generates a random number on [0,0xffffffff]-interval */
        public UInt32 genrand_int32()
        {
            //we do not want arithmetic overflow checks here,
            //since it is part of what this code does
            unchecked
            {
                uint y;
                /* mag01[x] = x * MATRIX_A  for x=0,1 */
                if (mti >= N) /* generate N words at one time */
                {
                    short kk;

                    if (mti == N + 1) /* if sgenrand() has not been called, */
                    {
                        init_genrand(5489U); /* a default initial seed is used */
                    }

                    for (kk = 0; kk < N - M; kk++)
                    {
                        y = (mt[kk] & UPPER_MASK) | (mt[kk + 1] & LOWER_MASK);
                        mt[kk] = mt[kk + M] ^ (y >> 1) ^ mag01[y & 0x1];
                    }

                    for (; kk < N - 1; kk++)
                    {
                        y = (mt[kk] & UPPER_MASK) | (mt[kk + 1] & LOWER_MASK);
                        mt[kk] = mt[kk + (M - N)] ^ (y >> 1) ^ mag01[y & 0x1];
                    }

                    y = (mt[N - 1] & UPPER_MASK) | (mt[0] & LOWER_MASK);
                    mt[N - 1] = mt[M - 1] ^ (y >> 1) ^ mag01[y & 0x1U];

                    mti = 0;
                }

                y = mt[mti++];
                y ^= (y >> 11);
                y ^= (y << 7) & 0x9d2c5680U;
                y ^= (y << 15) & 0xefc60000U;
                y ^= (y >> 18);

                //return ((double)y / 0xffffffffU); /* reals */
                return y;
                /* for integer generation */
            }
        }
    }

    class Decrypter
    {
        /*Many thanks to the author of the original TAH decrypter for crass.
         *I just ripped and converted parts of the code from C++ to C#.
         *I'ld like to contact and credit the author of this code but could
         *not yet find him/her. (Idk japanese/chinese)
         *I believe I have to thank ³Õh¹«Ù\ though I could be wrong... Am I right?*/

        static MT19937ar myMT19937ar = new MT19937ar();

        System.IO.BinaryReader reader;

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
            public entry_meta_info[] directory_meta_infos;
        }

        public int encrypt_archive(string file_name, string dest_path, string source_path)
        {
            //check if file already exists... if yes rename it
            string file_path_name = dest_path + file_name;
            try
            {
                if (System.IO.File.Exists(file_path_name))
                {
                    System.IO.File.Move(file_path_name, file_path_name + ".bak");
                }
            }
            catch (Exception)
            {
                System.Console.Out.WriteLine("Error: Could not rename existing TAH file. Possibly there is already a file with the '.bak' ending from a previous session. Please do something about it. TAHdecrypter will not overwrite existing data and therefore aborts here.");
                return -1;
            }
            //read in files from source path, do not compress them now.
            string[] directories = System.IO.Directory.GetDirectories(source_path, "*", System.IO.SearchOption.AllDirectories);
            string[] compress_directories = new string[directories.Length + 1];
            directories.CopyTo(compress_directories, 1);
            compress_directories[0] = source_path;
            UInt32 all_files_count = 0;
            for (int i = 0; i < compress_directories.Length; i++)
            {
                string[] str_files = System.IO.Directory.GetFiles(compress_directories[i]);
                all_files_count += (UInt32)str_files.Length;
            }
            tah_file tah_output_data = new tah_file();
            tah_output_data.all_compressed_files = new file_entry[all_files_count];
            UInt32 act_file = 0;
            string[] file_index = new string[all_files_count + compress_directories.Length - 1];
            UInt32 index_pos = 0;
            for (int i = 0; i < compress_directories.Length; i++)
            {
                string[] str_files = System.IO.Directory.GetFiles(compress_directories[i]);
                string act_file_index_path = "";
                if (i > 0)
                {
                    file_index[index_pos] = compress_directories[i].Substring(source_path.Length + 1) + "\\";
                    file_index[index_pos] = file_index[index_pos].Replace("\\", "/");
                    act_file_index_path = file_index[index_pos];
                    index_pos++;
                }
                for (int j = 0; j < str_files.Length; j++)
                {
                    if (i > 0)
                    {
                        file_index[index_pos] = str_files[j].Split(new string[] { "\\" }, StringSplitOptions.RemoveEmptyEntries)[str_files[j].Split(new string[] { "\\" }, StringSplitOptions.RemoveEmptyEntries).Length - 1];
                        tah_output_data.all_compressed_files[act_file].file_name = act_file_index_path + file_index[index_pos];
                        index_pos++;
                    }
                    else
                    {
                        try
                        {
                            string fparts0 = str_files[j].Split(new string[] { "\\" }, StringSplitOptions.RemoveEmptyEntries)[str_files[j].Split(new string[] { "\\" }, StringSplitOptions.RemoveEmptyEntries).Length - 1];
                            string fparts1 = fparts0.Split(new string[] { "_" }, StringSplitOptions.RemoveEmptyEntries)[fparts0.Split(new string[] { "_" }, StringSplitOptions.RemoveEmptyEntries).Length - 1];
                            string fparts2 = fparts1.Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries)[0];
                            //this should be the string of the hash value
                            tah_output_data.all_compressed_files[act_file].hash_value = System.UInt32.Parse(fparts2);
                        }
                        catch (Exception)
                        {
                            file_index[index_pos] = str_files[j].Split(new string[] { "\\" }, StringSplitOptions.RemoveEmptyEntries)[str_files[j].Split(new string[] { "\\" }, StringSplitOptions.RemoveEmptyEntries).Length - 1];
                            tah_output_data.all_compressed_files[act_file].file_name = act_file_index_path + file_index[index_pos];
                            index_pos++;
                        }
                    }
                    try
                    {
                        tah_output_data.all_compressed_files[act_file].true_file_name = str_files[j];
                        act_file++;
                    }
                    catch (Exception ex)
                    {
                        System.Console.Out.WriteLine(ex);
                        return -1;
                    }
                }
            }
            byte[] b_file_index;
            UInt32 b_file_index_count = 0;
            for (int i = 0; i < file_index.Length; i++)
            {
                if (file_index[i] != null)
                {
                    b_file_index_count += (UInt32)(file_index[i].Length + 1);
                }
            }
            b_file_index = new byte[b_file_index_count + 3];//savety margin for encryption...
            b_file_index.Initialize();
            UInt32 b_file_index_pos = 0;
            for (int i = 0; i < file_index.Length; i++)
            {
                if (file_index[i] != null)
                {
                    byte[] partial_index = System.Text.Encoding.ASCII.GetBytes(file_index[i]);
                    Copy(partial_index, 0, b_file_index, (int)b_file_index_pos, partial_index.Length);
                    b_file_index_pos += (UInt32)(partial_index.Length + 1);
                }
            }
            byte[] compressed_file_index = null;
            UInt32 compressed_file_index_length = 0;
            encrypt(ref b_file_index, b_file_index_count, ref compressed_file_index, ref compressed_file_index_length);
            byte[] compressed_file_index_s = new byte[compressed_file_index_length];
            Copy(compressed_file_index, 0, compressed_file_index_s, 0, (int)compressed_file_index_length);
            //now everything is set up for writing the tah file...
            System.IO.BinaryWriter writer = new System.IO.BinaryWriter(System.IO.File.Create(dest_path + "\\" + file_name));
            writer.Write(System.Text.Encoding.ASCII.GetBytes("TAH2"));
            writer.Write(all_files_count);
            writer.Write(((UInt32)1));
            writer.Write(((UInt32)0));
            UInt32 offset = 16 + 8 * all_files_count + compressed_file_index_length + 4;
            //writer needs this defined offset for adding length lists of the compressed data later on
            writer.BaseStream.Seek(offset, System.IO.SeekOrigin.Begin);
            for (int i = 0; i < tah_output_data.all_compressed_files.Length; i++)
            {
                try
                {
                    System.IO.BinaryReader reader = new System.IO.BinaryReader(System.IO.File.OpenRead(tah_output_data.all_compressed_files[i].true_file_name));
                    byte[] data_input = reader.ReadBytes((int)reader.BaseStream.Length);
                    tah_output_data.all_compressed_files[i].uncompressed_length = (UInt32)reader.BaseStream.Length;
                    byte[] encrypt_data_input = new byte[data_input.Length + 3]; //with safety margin for encryption
                    Copy(data_input, 0, encrypt_data_input, 0, (int)data_input.Length);
                    byte[] compressed_data = null;
                    UInt32 compressed_length = 0;
                    encrypt(ref encrypt_data_input, (UInt32)data_input.Length, ref compressed_data, ref compressed_length);
                    tah_output_data.all_compressed_files[i].compressed_length = compressed_length;
                    tah_output_data.all_compressed_files[i].compressed_data = new byte[compressed_length];
                    Copy(compressed_data, 0, tah_output_data.all_compressed_files[i].compressed_data, 0, (int)compressed_length);
                    System.Console.Out.WriteLine(String.Format("Compressing File: {0}", tah_output_data.all_compressed_files[i].true_file_name));
                    reader.Close();
                    writer.Write(tah_output_data.all_compressed_files[i].uncompressed_length);
                    writer.Write(tah_output_data.all_compressed_files[i].compressed_data);
                    writer.Flush();
                    if (i > 0)
                    {
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

            writer.BaseStream.Seek(16, System.IO.SeekOrigin.Begin);

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
                reader = new System.IO.BinaryReader(System.IO.File.OpenRead(source_file));
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

        // modified:
        // (c)2002 Jonathan Bennett (jon@hiddensoft.com)

        /*LZSS compressor*/
        public static void encrypt(ref byte[] data_input, UInt32 input_length,
                            ref byte[] data_output, ref UInt32 output_length)
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

        public static void encrypt_loop(ref byte[] data_input, UInt32 input_length,
                            ref byte[] data_output, ref UInt32 output_length, ref UInt32 nInputPos, ref UInt32 nOutputPos, ref byte[] rnd, ref UInt32 seed)
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
        public static void decrypt(ref byte[] data_input, UInt32 input_length,
                            ref byte[] data_output, UInt32 output_length)
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

        public static int extract_TAH_directory(ref System.IO.BinaryReader file_reader,
                                            ref directory_meta_info dir_meta_info)
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

            UInt32 input_length = index_buffer[0].offset - /*sizeof(header)*/ 16 - index_buffer_size;
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

            entry_meta_info[] directory_meta_info_buffer = new entry_meta_info[tah_header.index_entry_count];

            byte[] str_file_path = new byte[output_data.Length];
            output_data.CopyTo(str_file_path, 0);
            byte[] file_path = new byte[MAX_PATH];
            int act_str_pos = 0;
            while (str_file_path.Length > act_str_pos)
            {
                int pos_local = 0;
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
                if (str_file_path[act_str_pos + pos_local] != 0x00)
                {
                    int i;
                    for (i = 0; str_file_path[act_str_pos + i] != 0x00; i++)
                    {
                        file_path[i] = str_file_path[i + act_str_pos];
                    }
                    file_path[i] = 0x00;
                }
                else
                {
                    byte[] str_path = new byte[MAX_PATH];
                    uint str_path_offset = 0;
                    while (file_path[str_path_offset] != 0x00)
                    {
                        str_path_offset++;
                    }
                    file_path.CopyTo(str_path, 0);
                    int i;
                    for (i = 0; str_file_path[act_str_pos + i] != 0x00; i++)
                    {
                        str_path[i + str_path_offset] = str_file_path[act_str_pos + i];
                    }
                    str_path[i + str_path_offset] = 0x00;

                    UInt32 hash_key = gen_hash_key_for_string(ref str_path);
                    UInt32 h;
                    for (h = 0; h < tah_header.index_entry_count; h++)
                    {
                        if (directory_meta_info_buffer[h].file_name == null)
                        {
                            if (hash_key == index_buffer[h].hash_name)
                            {
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
                if (directory_meta_info_buffer[i].file_name == null)
                {
                    directory_meta_info_buffer[i].file_name = System.Text.Encoding.ASCII.GetBytes(i.ToString("00000000") + "_" + index_buffer[i].hash_name.ToString());
                    directory_meta_info_buffer[i].flag ^= 0x1;
                }
                directory_meta_info_buffer[i].offset = index_buffer[i].offset;
            }

            for (UInt32 i = 0; i < tah_header.index_entry_count - 1; i++)
            {
                directory_meta_info_buffer[i].length = index_buffer[i + 1].offset - index_buffer[i].offset;
            }
            directory_meta_info_buffer[tah_header.index_entry_count - 1].length = arc_size - index_buffer[tah_header.index_entry_count - 1].offset;

            dir_meta_info.index_entry_count = tah_header.index_entry_count;
            dir_meta_info.directory_meta_infos = directory_meta_info_buffer;

            return 0;
        }

        static int extract_TAH_resource(ref System.IO.BinaryReader file_reader,
                                            string dest_path,
                                            ref directory_meta_info dir_meta_info)
        {
            //read in names.txt file when it exists.
            ext_file_list external_files = new ext_file_list();
            if (System.IO.File.Exists("names.txt"))
            {
                System.IO.StreamReader reader = new System.IO.StreamReader(System.IO.File.OpenRead("names.txt"));
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
            //now proceed with decrypting
            for (int i = 0; i < dir_meta_info.index_entry_count; i++)
            {
                UInt32 data_input_length = dir_meta_info.directory_meta_infos[i].length - 4;
                byte[] data_input = new byte[data_input_length];
                UInt32 data_output_length;

                file_reader.BaseStream.Position = dir_meta_info.directory_meta_infos[i].offset;

                try
                {
                    data_output_length = System.BitConverter.ToUInt32(file_reader.ReadBytes(4), 0);
                    data_input = file_reader.ReadBytes((int)data_input_length);
                }
                catch (Exception)
                {
                    System.Console.Out.WriteLine("Error: Cannot read out compressed data of the archive.");
                    return -1;
                }

                byte[] data_output = new byte[data_output_length];

                try
                {
                    decrypt(ref data_input, data_input_length, ref data_output, data_output_length);
                }
                catch (Exception)
                {
                    System.Console.Out.WriteLine("Error: Failed to decrypt data. Possible error in archive.");
                    return -1;
                }

                string write_file_str = dest_path;
                int tcnt = 0;
                while ((dir_meta_info.directory_meta_infos[i].file_name[tcnt] != 0x00) && (tcnt < (dir_meta_info.directory_meta_infos[i].file_name.Length - 1))) { tcnt++; }
                write_file_str += "/" + System.Text.Encoding.ASCII.GetString(dir_meta_info.directory_meta_infos[i].file_name, 0, tcnt + 1);
                //write_file_str =  write_file_str.Replace("/", "\\");

                bool filename_found_in_list = false;

                UInt32 hashkey = 0;
                try
                {
                    hashkey = UInt32.Parse(write_file_str.Substring(write_file_str.LastIndexOf("_") + 1));
                }
                catch (Exception) { }

                if (external_files.files != null)
                {
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
                    if (!System.IO.Directory.Exists(dest_path))
                    {
                        System.IO.Directory.CreateDirectory(dest_path);
                    }
                    int l;
                    test_directory = dest_path;
                    for (l = 0; l < dir_parts.Length - 1; l++)
                    {
                        test_directory += "/" + dir_parts[l];
                        if (!System.IO.Directory.Exists(test_directory))
                        {
                            System.IO.Directory.CreateDirectory(test_directory);
                        }
                    }
                    //Does the file already exist?
                    if (System.IO.File.Exists(write_file_str))
                    {
                        System.IO.File.Delete(write_file_str);
                    }
                }
                catch (Exception)
                {
                    System.Console.Out.WriteLine("Error: Cannot prepare destination directory for file writing.");
                    return -1;
                }

                if (dir_meta_info.directory_meta_infos[i].flag % 2 == 1)
                {
                    //had no path name encoded in tah file
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
                    System.IO.BinaryWriter file_writer = new System.IO.BinaryWriter(System.IO.File.Create(write_file_str));
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
                reader = new System.IO.BinaryReader(System.IO.File.OpenRead(source_file));
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
                files[count] = System.Text.Encoding.ASCII.GetString(directory_meta_infos.directory_meta_infos[count].file_name);
            }
            return files;
        }

    }
}
