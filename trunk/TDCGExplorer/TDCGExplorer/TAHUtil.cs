using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace TDCGExplorer
{
    public class TAHUtil
    {
        //private static bool     debug   = true;
        public static bool debug = false;

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

        public static void WriteString(BinaryWriter bw, string s)
        {
            byte[] b = Encoding.Default.GetBytes(s);
            bw.Write(b, 0, b.Length);
            bw.Write((byte)0);
        }

        public static string ReadString(BinaryReader br)
        {
            List<byte> buf = new List<byte>(200);
            byte b;

            while ((b = br.ReadByte()) != 0)
                buf.Add(b);

            return Encoding.Default.GetString(buf.ToArray());
        }

        public static string ReadString(byte[] buf, int offset)
        {
            int begin = offset;

            for (; offset < buf.Length; ++offset)
                if (buf[offset] == 0)
                    break;

            return Encoding.Default.GetString(buf, begin, offset - begin);
        }

        public static byte[] ReadEntryData(BinaryReader br, TAHEntry e)
        {
            br.BaseStream.Seek(e.DataOffset, SeekOrigin.Begin);
            byte[] output = new byte[br.ReadInt32()];
            byte[] input = br.ReadBytes(e.Length - 4);

            TAHUtil.Decrypt(input, output);

            return output;
        }

        public static byte[] ReadRawEntryData(BinaryReader br, TAHEntry e, out uint len)
        {
            br.BaseStream.Seek(e.DataOffset, SeekOrigin.Begin);
            len = br.ReadUInt32();
            return br.ReadBytes(e.Length - 4);
        }

        public static byte[] DoDecrypt(byte[] input, byte[] output)
        {
            TAHCryptStream s = new TAHCryptStream(new MemoryStream(input, false), output.Length);
            LZSSInflate lzss = new LZSSInflate(new MemoryStream(output, true));
            lzss.Inflate(s);

            if (lzss.InflatedSize != output.Length)
                throw new InvalidDataException();

            return output;
        }

        public static byte[] DoEncrypt(byte[] input)
        {
            MemoryStream ms = new MemoryStream();
            TAHCryptStream s = new TAHCryptStream(ms, input.Length);
            LZSSDeflate lzss = new LZSSDeflate(s);
            lzss.Deflate(input);

            return ms.ToArray();
        }

        public static byte[] Encrypt(byte[] input)
        {
            return DoEncrypt(input);
        }

        public static byte[] Decrypt(byte[] input, byte[] output)
        {
            //debug   = true;

            try
            {
#if DECRYPT_DEBUG
                byte[]  output2= new byte[output.Length];

                if(debug)
                    TAHUtil.DecryptOld(input.Clone() as byte[], output2);
#endif
                DoDecrypt(input, output);
#if DECRYPT_DEBUG
                if(debug)
                    for(int i= 0; i < output.Length; ++i)
                        System.Diagnostics.Debug.Assert(output2[i] == output[i]);
#endif
#if ENCRYPT_DEBUG
                byte[]  encrypted   = DoEncrypt(output);
                byte[]  encrypted2  = new byte[output.Length];
                uint    length      = (uint)encrypted2.Length;
              //TAHdecrypt.Decrypter.encrypt(ref output, (uint)output.Length, ref encrypted2, ref length);
                byte[]  decrypted   = new byte[output.Length];
                DoDecrypt(encrypted, decrypted);
                if(debug)
                    for(int i= 0; i < output.Length; ++i)
                        System.Diagnostics.Debug.Assert(decrypted[i] == output[i]);
#endif
                return output;
            }
            catch (Exception e)
            {
                DbgPrint(e.ToString());
                throw;
            }
        }

        public static void DecryptOld(byte[] input, byte[] output)
        {
            //const UInt32 win_size   = 4096;
            const int win_mask = 4095;
            int pos_win = 0xff0;
            int pos_out = 0;
            int pos_in = 0;
            byte[] win = new byte[4096];
            ushort flag = 0;
            uint[] init_key = new uint[4];
            byte[] rnd = new byte[1024];

            init_key[0] = ((uint)output.Length | 0x80) >> 5;
            init_key[1] = ((uint)output.Length << 9) | 0x06;
            init_key[2] = ((uint)output.Length << 6) | 0x04;
            init_key[3] = ((uint)output.Length | 0x48) >> 3;

            mt19937.init_by_array(init_key, 4);

            for (int i = 0; i < 1024; ++i)
            {
                rnd[i] = (byte)((mt19937.genrand_int32()) >> (int)(i % 7));

                if (debug)
                    DbgPrint(rnd[i].ToString("X").PadLeft(2, '0'));
            }

            int seed = (((output.Length / 1000) % 10)
                        + ((output.Length / 100) % 10)
                        + ((output.Length / 10) % 10)
                        + ((output.Length) % 10)) & 0x31A;
            ++seed;

            if (debug)
                DbgPrint("seed: " + seed.ToString("X").PadLeft(8, '0'));

            for (int i = 0; i < input.Length; ++i)
            {
                //byte    before  = input[i];
                input[i] = (byte)(input[i] ^ rnd[seed + i & 1023]);

                //if(debug)
                //{
                //    DbgPrint(before  .ToString("X").PadLeft(2, '0')
                //        +">"+input[i].ToString("X").PadLeft(2, '0'));
                //}
            }

            try
            {
                win.Initialize();

                while (pos_out < output.Length)
                {
                    flag >>= 1;

                    if ((flag & 0x0100) == 0)
                    {
                        flag = (ushort)(input[pos_in++] | 0xff00);

                        //if(debug)
                        //    DbgPrint("GetFlag: " + flag.ToString("X").PadLeft(4, '0'));
                    }

                    if ((flag & 1) != 0)
                    {
                        if (debug)
                            DbgPrint("Literal:" + input[pos_in].ToString("X").PadLeft(2, '0'));

                        output[pos_out++] =
                        win[pos_win++] = input[pos_in++];
                        pos_win &= win_mask;
                        continue;
                    }

                    int offset = input[pos_in++];
                    int length = input[pos_in++];
                    offset |= (length & 0xf0) << 4;
                    length = (length & 0x0f) + 3;

                    if (debug)
                    {
                        DbgPrint("Match: offset=" + ((offset + 16) & 4095) + ", length=" + length);
                        DbgPrint("Position: " + pos_out);
                    }

                    for (int i = 0; i < length; ++i)
                    {
                        if (debug)
                            System.Diagnostics.Debug.Write(" " + win[(offset + i) & win_mask].ToString("X").PadLeft(2, '0'));

                        output[pos_out++] =
                        win[pos_win++] = win[(offset + i) & win_mask];
                        pos_win &= win_mask;

                        if (pos_out >= output.Length)
                            return;
                    }

                    if (debug)
                        System.Diagnostics.Debug.WriteLine("");
                }
            }
            finally
            {
                for (int i = 0; i < input.Length; ++i)
                    input[i] = (byte)(input[i] ^ rnd[seed + i & 1023]);
            }
        }

        [System.Diagnostics.Conditional("DECRYPT_DEBUG")]
        public static void DbgPrint(string s)
        {
            System.Diagnostics.Debug.WriteLine(s);
        }
    }
}
