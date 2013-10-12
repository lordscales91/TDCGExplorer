// based on TDCGMan
// Modified by N765/Konoa

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Diagnostics;

namespace TAHCompact
{
    public class LZSSWindow
    {
        public const int WINDOW_BITS = 12;
        public const int LENGTH_BITS = 4;
        public const int WINDOW_MAX = 1 << WINDOW_BITS;
        public const int WINDOW_MASK = WINDOW_MAX - 1;
        public const int LENGTH_MASK = (1 << LENGTH_BITS) - 1;
        public const int LENGTH_MIN = 3;
        public const int LENGTH_MAX = LENGTH_MIN + LENGTH_MASK;

        protected byte[] window;
        protected int current;
        protected int fill;

        public LZSSWindow()
        {
            window = new byte[WINDOW_MAX];
            current = 0;
            fill = 0;
        }

        public byte CurrentByte
        {
            get { return window[current]; }
            set { window[current] = value; }
        }

        public byte this[int index]
        {
            get { return window[(current + index) & WINDOW_MASK]; }
            set { window[(current + index) & WINDOW_MASK] = value; }
        }

        public byte GetAbs(int index)
        {
            return window[index & WINDOW_MASK];
        }

        public void Advance()
        {
            Advance(1);
        }

        public void Advance(int n)
        {
            current = (current + n) & WINDOW_MASK;
            fill = fill + n;
        }

        public int CalcHash(int offset)
        {
            return CalcHash(this[offset], this[offset + 1], this[offset + 2]);
        }

        // 24ビットを12ビットに圧縮
        public static int CalcHash(int b1, int b2, int b3)
        {
            b2 <<= 4;
            b3 = ((b3 & 15) << 8) | (b3 >> 4);
            int hash = b1 ^ b2 ^ b3;
            return hash;
        }

        public void Push(byte b)
        {
            CurrentByte = b;
            Advance();
        }

        public void Put(int offset, byte b)
        {
            this[offset] = b;
        }
    }
    public class LZSSDeflateWindow : LZSSWindow
    {
        public const int HASH_MAX = 4096;

        private LinkedList<int>[] codehash;
        private LinkedListNode<int>[] nodes;

        public LZSSDeflateWindow()
        {
            codehash = new LinkedList<int>[HASH_MAX];
            nodes = new LinkedListNode<int>[WINDOW_MAX];

            for (int i = 0; i < WINDOW_MAX; ++i)
                nodes[i] = new LinkedListNode<int>(i);
        }

        public LinkedListNode<int> GetNode(int off)
        {
            return nodes[(current + off) & WINDOW_MASK];
        }

        public LinkedList<int> GetHashedList(int hash)
        {
            if (codehash[hash] != null)
                return codehash[hash];

            return codehash[hash] = new LinkedList<int>();
        }

        public LinkedList<int> FindHashedList(int hash)
        {
            return codehash[hash];
        }

        public void UpdateHash(int off)
        {
            LinkedListNode<int> node = GetNode(off);
            if(node.List!=null) node.List.Remove(node); // まだリンクされていないnodeの場合null参照になる.
            GetHashedList(CalcHash(off)).AddLast(GetNode(off));
        }

        public void Back(byte b)
        {
            current = (current - 1) & WINDOW_MASK; --fill;
            Push(b);
            current = (current - 1) & WINDOW_MASK; --fill;
        }

        public new byte Push(byte b)
        {
            byte old = CurrentByte;
            CurrentByte = b;

            UpdateHash(-2);
            UpdateHash(-1);
            UpdateHash(0);
            Advance();

            return old;
        }

        public void Push(byte[] b, int off, int len)
        {
            for (int i = 0; i < len; ++i)
                base.Put(i, b[off + i]);

            for (int i = -2; i < len; ++i)
                UpdateHash(i);

            Advance(len);
        }

        public bool FindMatch(byte[] data, int begin, ref int matchoff, ref int matchlen)
        {
            return FindMatch(data, begin, data.Length - begin, ref matchoff, ref matchlen);
        }

        public bool FindMatch(byte[] data, int begin, int length, ref int matchoff, ref int matchlen)
        {
            if (length < LENGTH_MIN)
                return false;

            // ハッシュを計算して先頭３バイトが一致するか確認する
            int hash = CalcHash(data[begin + 0], data[begin + 1], data[begin + 2]);
            LinkedList<int> hashLinkedList = FindHashedList(hash);

            // ハッシュが無い、空の場合一致なしとする
            if (hashLinkedList == null || hashLinkedList.Count == 0)
                return false;

            // 最長マッチを求める
            int matchPosition = 0;
            int matchLength = LENGTH_MIN - 1;
            int matchLimitLength = Math.Min(length, LENGTH_MAX);

            foreach (int hashSpecifiedOffset in hashLinkedList)
            {
                int j = 0;

                if (window[(hashSpecifiedOffset + matchLength) & WINDOW_MASK] != data[begin + matchLength])
                    continue;

                for (j = 0; j < matchLimitLength; ++j)
                    if (j + hashSpecifiedOffset >= fill || window[(j + hashSpecifiedOffset) & WINDOW_MASK] != data[begin + j])
                        break;

                // N765/Konoa 辞書を上書きするオフセットはスキップする
                int distance = (current - hashSpecifiedOffset) & WINDOW_MASK;
                if (distance <= j) continue;

                if (j > matchLength)
                {
                    matchPosition = hashSpecifiedOffset;
                    matchLength = j;

                    if (matchLength == matchLimitLength)
                        break;
                }
            }

            if (matchLength < LENGTH_MIN)
                return false;

            matchoff = (matchPosition - 16) & WINDOW_MASK;
            //matchoff    = (current - mpos) & WINDOW_MASK;
            matchlen = matchLength;

            return true;
        }
    }

    // 圧縮
    public class LZSSDeflate
    {
        private LZSSDeflateWindow window = new LZSSDeflateWindow();
        //private MemoryStream        ms;
        private Stream s;

        public LZSSDeflate(Stream s)
        {
            //this.ms = ms;
            this.s = s;
        }

        public void Deflate(byte[] data)
        {
            int off = 0, len = 0;
            long elempos = 0;
            int elems = 0;
            int elemdata = 0;

            s.WriteByte(0);

            for (int i = 0; i < data.Length; )
            {
                if (window.FindMatch(data, i, ref off, ref len))
                {   // マッチ
#if true           // さらに最適なマッチがないか１バイト先をマッチさせて見る
                    int off2 = 0;
                    int len2 = 0;

                    // 仮にLiteralをひとつおく
                    byte old = window.Push(data[i]);

                    if (window.FindMatch(data, i + 1, ref off2, ref len2))
                    {
                        if (len2 > len + 1) // より長くマッチ
                        {   //
                            window.Back(old);
                            goto LABLE_LITREAL;
                        }
                    }

                    window.Back(old);
#endif

                    s.WriteByte((byte)(off & 255));
                    s.WriteByte((byte)(((off & 0x0F00) >> 4) | ((byte)(len - 3))));

                    window.Push(data, i, len);
                    i += len;
                    goto LABEL_DONE;
                }

                // リテラル
            LABLE_LITREAL:
                if (TAHUtil.debug)
                    System.Diagnostics.Debug.WriteLine("Literal: " + data[i].ToString("X").PadLeft(2, '0'));

                elemdata |= 1 << elems;
                s.WriteByte(data[i]);
                window.Push(data[i++]);

            LABEL_DONE:
                ++elems;

                if (elems == 8)
                {   //
                    elempos = FlushElem(elempos, elemdata);
                    elems = 0;
                    elemdata = 0;
                }
            }

            if (elems != 0)
                FlushElem(elempos, elemdata);

            s.Seek(-1, SeekOrigin.Current);
            s.SetLength(s.Position);
            s.Flush();
        }

        public long FlushElem(long pos, int elem)
        {
            long save = s.Position;
            s.Seek(pos, SeekOrigin.Begin);
            s.WriteByte((byte)elem);
            s.Seek(save + 1, SeekOrigin.Begin);
            return save;
        }
    }

    // 解凍
    public class LZSSInflate
    {
        private MemoryStream ms;
        private LZSSWindow window = new LZSSWindow();

        public int InflatedSize { get { return (int)ms.Position; } }

        public LZSSInflate(MemoryStream ms)
        {
            this.ms = ms;
        }

        public void Inflate(Stream s)
        {
            using (BinaryReader br = new BinaryReader(s))
            {
                int elemdata = br.ReadByte();
                int elems = 0;

                byte b;

                try
                {
                    for (; ; )
                    {
                        if ((elemdata & (1 << elems)) == 0)
                        {   // マッチ
                            int off = br.ReadByte();
                            int len = br.ReadByte();
                            off |= (len & 0xF0) << 4;
                            len = (len & 15) + 3;

                            off += 16;

                            for (int j = 0; j < len; ++j)
                            {
                                ms.WriteByte(b = window.GetAbs(off + j));
                                window.Push(b);

                                if (ms.Length == ms.Position)
                                    break;
                            }
                        }
                        else
                        {   // リテラル
                            ms.WriteByte(b = br.ReadByte());
                            window.Push(b);

                            if (TAHUtil.debug)
                                System.Diagnostics.Debug.WriteLine("Literal:" + b.ToString("X").PadLeft(2, '0'));
                        }

                        if (ms.Position == ms.Length)
                            break;

                        ++elems;

                        if (elems == 8)
                        {
                            elems = 0;
                            elemdata = br.ReadByte();
                        }
                    }
                }
                catch (EndOfStreamException)
                {
                }
            }
        }

        public void Inflate(byte[] data)
        {
            using (MemoryStream stream = new MemoryStream(data, false))
            {
                Inflate(stream);
            }
        }
    }
}

