using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TDCG
{
    struct _W
    {
        public UInt32 X;
        public UInt32 T;
    }

    class _C
    {
        public UInt32 IP;
        public UInt32 BlockN;

        public _C(UInt32 ip, UInt32 BN)
        {
            IP = ip;
            BlockN = BN;
        }
    }

    /// <summary>
    /// TBNファイルを扱います。
    /// </summary>
    public class TBNFile
    {
        const UInt32 T_Integer = 0x01000000;
        const UInt32 T_Pointer = 0x02000000;
        const UInt32 T_Float = 0x04000000;
        const UInt32 T_String = 0x08000000;
        const UInt32 T_Code = 0x10000000;
        const UInt32 T_Trail = 0x20000000;
        const UInt32 T_Tail = 0x40000000;
        const UInt32 T_Label = 0x80000000;

        UInt32 BlockC = 1;
        _W[] W;
        Queue<_C> Cs;

        /// <summary>
        /// バイナリ値として読み取ります。
        /// </summary>
        protected BinaryReader reader;

        /// <summary>
        /// 指定パスから読み込みます。
        /// </summary>
        /// <param name="source_file">パス</param>
        public void Load(string source_file)
        {
            using (Stream source_stream = File.OpenRead(source_file))
                Load(source_stream);
        }

        /// <summary>
        /// 指定ストリームから読み込みます。
        /// </summary>
        /// <param name="source_stream">ストリーム</param>
        public void Load(Stream source_stream)
        {
            this.reader = new BinaryReader(source_stream, System.Text.Encoding.Default);

            byte[] magic = reader.ReadBytes(4);

            if (magic[0] != (byte)'B'
            || magic[1] != (byte)'B'
            || magic[2] != (byte)'B'
            || magic[3] != (byte)'B')
                throw new Exception("File is not TBN");

            float opt1 = reader.ReadSingle();
            //Console.WriteLine(opt1);

            uint WC = reader.ReadUInt32();
            W = new _W[WC];
            W[0].X = 0x42424242;
            W[0].T = T_Label | T_Integer | 1;
            W[1].X = 0x3f800000;
            W[1].T = T_Float | 1;
            W[2].X = WC;
            W[2].T = T_Integer | 1;
            for (uint i = 3; i < WC; i++)
            {
                W[i].X = reader.ReadUInt32();
                W[i].T = (i <= 8) ? (T_Integer | 1) : 0;
            }

            Cs = new Queue<_C>(1024);
            Cs.Enqueue(new _C(9, 0));
            SearchCodeBlock(true);
            SearchUnknownBlock();
        }

        /// <summary>
        /// 指定パスに保存します。
        /// </summary>
        /// <param name="dest_file">パス</param>
        public void Save(string dest_file)
        {
            using (Stream dest_stream = File.Create(dest_file))
                Save(dest_stream);
        }

        /// <summary>
        /// 指定ストリームに保存します。
        /// </summary>
        /// <param name="dest_stream">ストリーム</param>
        public void Save(Stream dest_stream)
        {
            BinaryWriter bw = new BinaryWriter(dest_stream);

            for (uint i = 0; i < W.Length; i++)
            {
                bw.Write(W[i].X);
            }
        }

        bool IsSJISChar(UInt32 c)
        {
            if (0 == c) return (true);
            if (0x0a == c) return (true);
            if (0x7f == c) return (false);
            if ((0x20 <= c) && (0xfc >= c)) return (true);
            return (false);
        }

        bool IsSubStr(UInt32 X)
        {
            if (0 == (X >> 24))
            {
                if (0 == ((X >> 16) & 0xff))
                {
                    if (0 == ((X >> 8) & 0xff))
                    {
                        return (IsSJISChar(X & 0xff));
                    }
                    else
                    {
                        if (0 != (X & 0xff)) return (IsSJISChar((X >> 8) & 0xff) && IsSJISChar(X & 0xff));
                    }
                }
                else
                {
                    if ((0 != ((X >> 8) & 0xff)) && (0 != (X & 0xff)))
                        return (IsSJISChar((X >> 16) & 0xff) && IsSJISChar((X >> 8) & 0xff) && IsSJISChar(X & 0xff));
                }
            }
            else
            {
                if ((0 != ((X >> 16) & 0xff)) && (0 != ((X >> 8) & 0xff)) && (0 != (X & 0xff)))
                    return (IsSJISChar((X >> 24) & 0xff) && IsSJISChar((X >> 16) & 0xff) && IsSJISChar((X >> 8) & 0xff) && IsSJISChar(X & 0xff));
            }
            return (false);
        }

        void SearchStringBlock(UInt32 i)
        {
            if (!IsSubStr(W[i].X)) return;
            UInt32 j;
            for (j = i + 1; j < W.Length; j++)
            {
                if ((0 != W[j].T) || !IsSubStr(W[j].X)) break;
                if ((0 == W[j].X) && (0 == W[j - 1].X) && (0 == (W[j - 2].X >> 24))) { j -= 1; break; }
            }
            while (0 != (W[j - 1].X >> 24)) j--;
            BlockC++;
            bool head = true;
            for (UInt32 k = i; k < j; k++)
            {
                if (head) W[k].T = T_Label;
                W[k].T |= T_String | BlockC;
                head = false;
                if (0 == (W[k].X >> 24)) head = true;
            }
        }

        void SearchUnknownBlock()
        {
            for (UInt32 i = 1; i < W.Length; i++)
            {
                if ((0 != W[i].X) && (0 == W[i].T))
                {
                    UInt32 t = BlockC;
                    Cs.Enqueue(new _C(i, 0));
                    SearchCodeBlock(false);
                    if (0 != W[i].T)
                    {
                        SearchCodeBlock(true);
                        continue;
                    }
                    Cs.Clear();
                    BlockC = t;
                    SearchStringBlock(i);
                    if ((W[i].T & 0x00ffffff) != (W[i - 1].T & 0x00ffffff))
                        W[i].T |= T_Label;
                }
            }
        }

        void SearchCodeBlock(bool loop)
        {
            if (0 == Cs.Count) return;
            do
            {
                _C C = (_C)Cs.Dequeue();
                if (0 == C.BlockN) C.BlockN = ++BlockC;
                W[C.IP].T |= T_Label | C.BlockN;
                UInt32 i;
                for (i = C.IP; i < W.Length; i++)
                {
                    if (0 != (W[i].T & ~T_Label & 0xff000000)) break;

                    W[i].T |= T_Code | C.BlockN;

                    UInt32 x = W[i].X;						//	1
                    UInt32 bit04 = x & 15;					//	2
                    UInt32 bit43 = (x >> 4) & 7;			//	4
                    UInt32 bit73 = (x >> 7) & 7;			//	8
                    UInt32 bitA3 = (x >> 10) & 7;			//	10
                    UInt32 bitD3 = (x >> 13) & 7;			//	20
                    UInt32 bitG3 = (x >> 16) & 7;			//	40
                    UInt32 bitGG = (x >> 16);				//	80
                    UInt32 bitJ3 = (x >> 19) & 7;			//	100
                    UInt32 bitJD = (x >> 19);				//	200
                    UInt32 bitM3 = (x >> 22) & 7;			//	400
                    UInt32 bitMA = (x >> 22);				//	800
                    UInt32 bitP7 = (x >> 25);				//	1000

                    if (0 == bit73) goto UnknownOP;

                    switch (bit43)
                    {
                        case 0:
                            switch (bit04)
                            {
                                case 0:
                                    switch (bitA3)
                                    {
                                        case 1:
                                        case 2:
                                        case 3:
                                        case 4:
                                        case 5:
                                        case 7:
                                            if (0 != bitD3) goto UnknownOP;
                                            break;
                                        default:
                                            goto UnknownOP;
                                    };
                                    break;
                                case 1:
                                    switch (bitA3)
                                    {
                                        case 1:
                                        case 2:
                                        case 3:
                                        case 4:
                                        case 5:
                                            if (0 != bitD3) goto UnknownOP;
                                            break;
                                        case 7:
                                            if (0 != bitD3) goto UnknownOP;
                                            if ((1 == (bitGG & 1)) && (7 == bit73)) { W[i].T |= T_Tail; goto BLOCK_END; }
                                            break;
                                        default:
                                            goto UnknownOP;
                                    };
                                    break;
                                default:
                                    goto UnknownOP;
                            };
                            break;
                        case 1:
                            switch (bit04)
                            {
                                case 0:
                                    if ((0 != bitA3) || (0 != bitD3)) goto UnknownOP;
                                    break;
                                case 1:
                                    switch (bitA3)
                                    {
                                        case 1:
                                        case 2:
                                            break;
                                        case 7:
                                            if (0 == bitD3)
                                            {
                                                Cs.Enqueue(new _C(i + 1 + bitGG + ((0x8000 <= bitGG) ? 0xffff0000 : 0), C.BlockN));
                                                if (7 == bit73) { W[i].T |= T_Tail; goto BLOCK_END; }
                                            }
                                            break;
                                        default:
                                            goto UnknownOP;
                                    };
                                    break;
                                case 2:
                                    switch (bitA3)
                                    {
                                        case 1:
                                        case 2:
                                            break;
                                        case 7:
                                            if (0 == bitD3) Cs.Enqueue(new _C(i + 1 + bitGG + ((0x8000 <= bitGG) ? 0xffff0000 : 0), 0));
                                            break;
                                        default:
                                            goto UnknownOP;
                                    };
                                    break;
                                case 3:
                                    if ((0 != bitA3) || (0 != bitD3)) goto UnknownOP;
                                    break;
                                case 4:
                                    if ((0 != bitA3) || (0 != bitD3)) goto UnknownOP;
                                    Cs.Enqueue(new _C(i + 1 + bitGG + ((0x8000 <= bitGG) ? 0xffff0000 : 0), 0));
                                    break;
                                default:
                                    goto UnknownOP;
                            };
                            break;
                        case 2:
                            switch (bit04)
                            {
                                case 0:
                                    switch (bitA3)
                                    {
                                        case 1:
                                        case 2:
                                        case 3:
                                        case 4:
                                        case 5:
                                            if (0 != bitP7) goto UnknownOP;
                                            break;
                                        case 7:
                                            if (0 != bitP7) goto UnknownOP;
                                            if (7 == bit73) { W[i].T |= T_Tail; goto BLOCK_END; }
                                            break;
                                        default:
                                            goto UnknownOP;
                                    };
                                    break;
                                case 1:
                                    switch (bitA3)
                                    {
                                        case 1:
                                        case 2:
                                        case 3:
                                        case 4:
                                        case 5:
                                        case 7:
                                            if (0 != bitP7) goto UnknownOP;
                                            break;
                                        default:
                                            goto UnknownOP;
                                    };
                                    break;
                                default:
                                    goto UnknownOP;
                            };
                            break;
                        case 3:
                            switch (bit04)
                            {
                                case 0:
                                case 1:
                                case 2:
                                    switch (bitA3)
                                    {
                                        case 1:
                                        case 2:
                                        case 3:
                                        case 4:
                                            if (0 != bitMA) goto UnknownOP;
                                            break;
                                        case 7:
                                            if (0 != bitMA) goto UnknownOP;
                                            if ((0 == bitD3) && (7 == bit73)) { W[i].T |= T_Tail; goto BLOCK_END; }
                                            break;
                                        default:
                                            goto UnknownOP;
                                    };
                                    break;
                                case 3:
                                    switch (bitA3)
                                    {
                                        case 1:
                                        case 2:
                                        case 3:
                                            if (0 != bitMA) goto UnknownOP;
                                            break;
                                        case 7:
                                            if (0 != bitMA) goto UnknownOP;
                                            if ((0 == bitD3) && (7 == bit73)) { W[i].T |= T_Tail; goto BLOCK_END; }
                                            break;
                                        default:
                                            goto UnknownOP;
                                    };
                                    break;
                                case 4:
                                case 7:
                                    switch (bitA3)
                                    {
                                        case 2:
                                            if (0 != bitMA) goto UnknownOP;
                                            break;
                                        default:
                                            goto UnknownOP;
                                    };
                                    break;
                                case 5:
                                case 12:
                                    switch (bitA3)
                                    {
                                        case 1:
                                        case 2:
                                            if (0 != bitMA) goto UnknownOP;
                                            break;
                                        default:
                                            goto UnknownOP;
                                    };
                                    break;
                                case 8:
                                    switch (bitA3)
                                    {
                                        case 1:
                                        case 2:
                                        case 3:
                                        case 7:
                                            if (0 != bitJD) goto UnknownOP;
                                            break;
                                        default:
                                            goto UnknownOP;
                                    };
                                    break;
                                case 9:
                                    switch (bitA3)
                                    {
                                        case 2:
                                        case 3:
                                        case 4:
                                            if (0 != bitJD) goto UnknownOP;
                                            break;
                                        default:
                                            goto UnknownOP;
                                    };
                                    break;
                                default:
                                    goto UnknownOP;
                            };
                            break;
                        case 4:
                            switch (bit04)
                            {
                                case 0:
                                    switch (bitA3)
                                    {
                                        case 1:
                                        case 2:
                                        case 3:
                                            switch (bitM3)
                                            {
                                                case 1:
                                                case 2:
                                                case 3:
                                                case 7:
                                                    if ((0 != bitJ3) || (0 != bitP7)) goto UnknownOP;
                                                    break;
                                                default:
                                                    goto UnknownOP;
                                            };
                                            break;
                                        case 7:
                                            switch (bitM3)
                                            {
                                                case 1:
                                                case 2:
                                                case 3:
                                                case 7:
                                                    if ((0 != bitJ3) || (0 != bitP7)) goto UnknownOP;
                                                    if ((0 == bitD3) && (7 == bit73)) { W[i].T |= T_Tail; goto BLOCK_END; }
                                                    break;
                                                default:
                                                    goto UnknownOP;
                                            };
                                            break;
                                        default:
                                            goto UnknownOP;
                                    };
                                    break;
                                case 1:
                                    switch (bitA3)
                                    {
                                        case 3:
                                            switch (bitM3)
                                            {
                                                case 4:
                                                case 5:
                                                    if (0 != bitP7) goto UnknownOP;
                                                    break;
                                                default:
                                                    goto UnknownOP;
                                            };
                                            break;
                                        case 4:
                                        case 5:
                                            if (0 != bitMA) goto UnknownOP;
                                            break;
                                        default:
                                            goto UnknownOP;
                                    };
                                    break;
                                case 2:
                                    if ((0 != bitA3) || (0 != bitMA)) goto UnknownOP;
                                    break;
                                default:
                                    goto UnknownOP;
                            };
                            break;
                        case 5:
                            if (W.Length < i + bitGG) goto UnknownOP;
                            switch (bit04)
                            {
                                case 0:
                                    switch (bitA3)
                                    {
                                        case 1:
                                        case 2:
                                            for (UInt32 j = i + 1; j <= i + bitGG; j++) W[j].T = T_Code | T_Trail | T_Integer | C.BlockN;
                                            break;
                                        case 3:
                                            for (UInt32 j = i + 1; j <= i + bitGG; j++) W[j].T = T_Code | T_Trail | T_Float | C.BlockN;
                                            break;
                                        case 7:
                                            for (UInt32 j = i + 1; j <= i + bitGG; j++) W[j].T = T_Code | T_Trail | ((0 == bitD3) ? T_Pointer : T_Integer) | C.BlockN;
                                            if (0 == bitD3)
                                            {
                                                Cs.Enqueue(new _C(W[i + bitGG].X, C.BlockN));
                                                if (7 == bit73) { W[i + bitGG].T |= T_Tail; goto BLOCK_END; }
                                            }
                                            break;
                                        default:
                                            goto UnknownOP;
                                    };
                                    break;
                                case 1:
                                case 4:
                                    if ((0 != bitA3) || (0 != bitD3)) goto UnknownOP;
                                    for (UInt32 j = i + 1; j <= i + bitGG; j++) W[j].T = T_Code | T_Trail | C.BlockN;
                                    break;
                                case 2:
                                case 3:
                                    if ((0 != bitA3) || (0 != bitD3)) goto UnknownOP;
                                    for (UInt32 j = i + 1; j <= i + bitGG; j++) W[j].T = T_Code | T_Trail | ((0 == bitD3) ? T_Pointer : T_Integer) | C.BlockN;
                                    Cs.Enqueue(new _C(W[i + bitGG].X, 0));
                                    break;
                                default:
                                    goto UnknownOP;
                            };
                            i += bitGG;
                            break;
                        default:
                            goto UnknownOP;
                    };
                }
            BLOCK_END:
                continue;
            UnknownOP:
                if (2 > i - C.IP)
                    for (UInt32 j = C.IP; j <= i; j++) W[j].T = 0;
                W[i].T = 0;
                break;
            } while (loop && (0 < Cs.Count));
        }

        int ReadBytes(byte[] b, ref uint i)
        {
            int k = 0;
            while (k < b.Length)
            {
                byte[] buf = BitConverter.GetBytes(W[i].X);

                if (buf[0] == 0) break;
                b[k++] = buf[0];

                if (buf[1] == 0) break;
                b[k++] = buf[1];

                if (buf[2] == 0) break;
                b[k++] = buf[2];

                if (buf[3] == 0) break;
                b[k++] = buf[3];

                i++;
            }
            return k;
        }

        int WriteBytes(byte[] b, ref uint i)
        {
            int k = 0;
            while (k < b.Length)
            {
                byte[] buf = BitConverter.GetBytes(W[i].X);

                for (int j = 0; j < 4; j++)
                {
                    if (k < b.Length)
                        buf[j] = b[k++];
                }

                W[i].X = BitConverter.ToUInt32(buf, 0);
                i++;
            }
            return k;
        }

        string DW2Str(ref UInt32 i)
        {
            Byte[] b = new Byte[4096];
            int k = ReadBytes(b, ref i);

            int bu, cu;
            bool comp;
            Char[] c = new Char[4096];
            Encoding.GetEncoding(932).GetDecoder().Convert(b, 0, k, c, 0, 4096, true, out bu, out cu, out comp);
            StringBuilder sb = new StringBuilder();
            for (UInt32 j = 0; j < cu; j++)
            {
                sb.Append(c[j]);
            }
            return sb.ToString();
        }

        string DW2StrDump(ref UInt32 i)
        {
            Byte[] b = new Byte[4096];
            int k = ReadBytes(b, ref i);

            int bu, cu;
            bool comp;
            Char[] c = new Char[4096];
            Encoding.GetEncoding(932).GetDecoder().Convert(b, 0, k, c, 0, 4096, true, out bu, out cu, out comp);
            StringBuilder sb = new StringBuilder();
            for (UInt32 j = 0; j < cu; j++)
            {
                if (0x20 > c[j])
                    sb.Append("\\x" + ((int)c[j]).ToString("X2"));
                else if ('\\' == c[j])
                    sb.Append("\\\\");
                else if ('"' == c[j])
                    sb.Append("\\\"");
                else if (0x100 <= c[j])
                    sb.Append(c[j]);
                else
                    sb.Append(c[j]);
            }
            return sb.ToString();
        }

        /// <summary>
        /// 文字列を書き出します。
        /// </summary>
        public void Dump()
        {
            for (uint i = 0; i < W.Length; i++)
            {
                if (0 != (W[i].T & T_String))
                {
                    Console.Write("_" + i.ToString("X8"));
                    Console.WriteLine(":      DATA \"" + DW2StrDump(ref i) + "\"");
                }
            }
        }

        /// <summary>
        /// 文字列リストを得ます。
        /// </summary>
        /// <returns>文字列リスト</returns>
        public List<string> GetStringList()
        {
            List<string> ret = new List<string>();
            for (uint i = 0; i < W.Length; i++)
            {
                if (0 != (W[i].T & T_String))
                {
                    ret.Add(DW2Str(ref i));
                }
            }
            return ret;
        }

        /// <summary>
        /// 文字列の辞書を得ます。
        /// </summary>
        /// <returns>文字列の辞書</returns>
        public Dictionary<uint, string> GetStringDictionary()
        {
            Dictionary<uint, string> ret = new Dictionary<uint, string>();
            for (uint i = 0; i < W.Length; i++)
            {
                if (0 != (W[i].T & T_String))
                {
                    ret.Add(i, DW2Str(ref i));
                }
            }
            return ret;
        }

        /// <summary>
        /// 文字列を設定します。
        /// </summary>
        public void SetString(uint i, string str)
        {
            Byte[] b = new Byte[4096];
            Encoding enc = Encoding.GetEncoding("Shift_JIS");
            b = enc.GetBytes(str);
            WriteBytes(b, ref i);
        }
    }
}
