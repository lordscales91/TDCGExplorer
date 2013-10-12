using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.IO;

namespace TAHCompact
{
    public class TAHUtil
    {
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
            try
            {
                DoDecrypt(input, output);
                return output;
            }
            catch (Exception e)
            {
                DbgPrint(e.ToString());
                throw;
            }
        }

        [System.Diagnostics.Conditional("DECRYPT_DEBUG")]
        public static void DbgPrint(string s)
        {
            System.Diagnostics.Debug.WriteLine(s);
        }
    }
}

