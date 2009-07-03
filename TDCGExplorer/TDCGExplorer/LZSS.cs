using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TDCGExplorer
{
    public class LZSSWindow
    {
#if true
        public const int WINDOW_BITS = 12;
        public const int LENGTH_BITS = 4;
#else        
        public const int    WINDOW_BITS = 13;
        public const int    LENGTH_BITS = 3;
#endif
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

        //public int FillCount    { get { return fill; } }

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

        public virtual void Reset()
        {
#if false
			window.Fill(0);
#else
            Array.Clear(window, 0, window.Length);
#endif
            current = 0;
            fill = 0;
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
#if true

            b2 <<= 4;
            b3 = ((b3 & 15) << 8) | (b3 >> 4);
            int hash = b1 ^ b2 ^ b3;
#else
            int hash=(  b1               << 4)
                  ^  (((b2 +  53) & 255) << 2)
                  ^  (((b3 + 149) & 255) << 0);
		  
#endif
            /*
            System.Diagnostics.Debug.WriteLine(
                string.Format("{0} {1} {2} hash: {3}",
                    b1.ToString("X").PadLeft(2, '0'),
                    b2.ToString("X").PadLeft(2, '0'),
                    b3.ToString("X").PadLeft(2, '0'),
                    hash.ToString("X")));
            */
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

        public void Dump()
        {
            Dump(window);
        }

        public static void Dump(byte[] b)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < b.Length; ++i)
            {
                switch (i & 15)
                {
#if false
				case 0: sb.Append(i.ToFormattedString("X", 3, '0') + ": " + b[i].ToString("X").PadLeft(2, '0')); break;
#else
                    case 0: sb.Append(String.Format("{0:X3}", i) + ": " + b[i].ToString("X").PadLeft(2, '0')); break;
#endif
                    case 15: sb.AppendLine(" " + b[i].ToString("X").PadLeft(2, '0')); break;
                    default: sb.Append(" " + b[i].ToString("X").PadLeft(2, '0')); break;
                }

                if (sb.Length > 480)
                {
                    System.Diagnostics.Debug.Write(sb.ToString());
                    sb.Length = 0;
                }
            }

            if (sb.Length > 0)
            {
                System.Diagnostics.Debug.Write(sb.ToString());
                sb.Length = 0;
            }
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

        public override void Reset()
        {
#if false
			foreach (var i in nodes)
				i.Remove();
#else
            foreach (LinkedListNode<int> i in nodes)
            {
                LinkedList<int> list = i.List;
                list.Remove(i);
            }
#endif
            base.Reset();
        }

        public LinkedListNode<int> CurrentNode
        {
            get { return nodes[current]; }
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
#if false
			GetNode(off).Remove();
#else
            LinkedListNode<int> node = GetNode(off);
            node.List.Remove(node);
#endif
            //System.Diagnostics.Debug.Write("Rehash: ");
            //GetHashedList(CalcHash(off)).AddFirst(GetNode(off));
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
            if (length < 3)
                return false;

            // ハッシュを計算して先頭３バイトが一致するか確認する
            //System.Diagnostics.Debug.Write("Source: ");

            int hash = CalcHash(data[begin + 0], data[begin + 1], data[begin + 2]);
            LinkedList<int> ll = FindHashedList(hash);

            // ハッシュが無い、空の場合一致なしとする
            if (ll == null || ll.Count == 0)
                return false;

            // 最長マッチを求める
            int mpos = 0;
            int mlen = LENGTH_MIN - 1;
            int l = Math.Min(length, LENGTH_MAX);

            foreach (int i in ll)
            {
                int off = i, j = 0;

                if (window[(off + mlen) & WINDOW_MASK] != data[begin + mlen])
                    continue;

                for (j = 0; j < l; ++j)
                    if (j + off >= fill || window[(j + off) & WINDOW_MASK] != data[begin + j])
                        break;

                if (j > mlen)
                {
                    mpos = off;
                    mlen = j;

                    if (mlen == l)
                        break;
                }
            }

            if (mlen < LENGTH_MIN)
                return false;

            matchoff = (mpos - 16) & WINDOW_MASK;
            //matchoff    = (current - mpos) & WINDOW_MASK;
            matchlen = mlen;

            return true;
        }
    }

    // 圧縮
    public class LZSSDeflate
    {
        private LZSSDeflateWindow window = new LZSSDeflateWindow();
        //private MemoryStream        ms;
        private Stream s;

        public LZSSDeflate()
        {
            //ms      = new MemoryStream();
            s = new MemoryStream();
        }

        //public LZSSDeflate(MemoryStream ms)
        public LZSSDeflate(Stream s)
        {
            //this.ms = ms;
            this.s = s;
        }

        public void Reset()
        {
            window.Reset();
            //ms.SetLength(0);
            //ms.SetLength(0);
        }

        private void PushLiteral()
        {
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
#if true            // さらに最適なマッチがないか１バイト先をマッチさせて見る
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
                    if (TAHUtil.debug)
                    {
                        window.Dump();
                        System.Diagnostics.Debug.WriteLine("Match: offset=" + ((off + 16) & 4095) + ", length=" + len);
                        System.Diagnostics.Debug.WriteLine("Position: " + i);

                        for (int j = 0; j < len; ++j)
                            System.Diagnostics.Debug.Write(" " + data[i + j].ToString("X").PadLeft(2, '0'));

                        System.Diagnostics.Debug.WriteLine("");
                    }

                    //elemdata|=1 << elems;
#if false
                    int bits= (off << LZSSWindow.LENGTH_BITS) + (len - LZSSWindow.LENGTH_MIN);

                    s.WriteByte((byte)(bits & 255));
                    s.WriteByte((byte)(bits >> 8));
#else
                    s.WriteByte((byte)(off & 255));
                    s.WriteByte((byte)(((off & 0x0F00) >> 4) | ((byte)(len - 3))));
#endif

                    //for(int j= 0; j < len; ++j)
                    //    System.Diagnostics.Debug.Write((char)window[-off+j]);

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

        public byte[] GetDeflatedBytes()
        {
            return s is MemoryStream ? (s as MemoryStream).ToArray() : null;
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
        //private int             inflated_size   = 0;

        //public int InflatedSize { get { return inflated_size; } }
        public int InflatedSize { get { return (int)ms.Position; } }

        public LZSSInflate()
        {
            this.ms = new MemoryStream();
        }

        public LZSSInflate(MemoryStream ms)
        {
            this.ms = ms;
        }

        public void Inflate(Stream s)
        {
            BinaryReader br = new BinaryReader(s);
            int elemdata = br.ReadByte();
            int elems = 0;
            byte[] buf = new byte[LZSSWindow.LENGTH_MAX + 1];
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
                        //bits    |=br.ReadByte() << 8;
                        //int off = LZSSWindow.WINDOW_MAX - (bits >> LZSSWindow.LENGTH_BITS);
                        //int len = (bits & LZSSWindow.LENGTH_MASK) + LZSSWindow.LENGTH_MIN;

                        if (TAHUtil.debug)
                        {
                            System.Diagnostics.Debug.WriteLine("Match: offset=" + off + ", length=" + len);
                            System.Diagnostics.Debug.WriteLine("Position: " + ms.Position);
                        }

                        //off = 4096 - off;
                        off += 16;

#if false
                        for(int j= 0; j < len; ++j)
                            ms.WriteByte(buf[j]= window.GetAbs(off+j));
                          //ms.WriteByte(buf[j]= window[off+j]);

                        for(int j= 0; j < len; ++j)
                            window.Push(buf[j]);
#else
                        for (int j = 0; j < len; ++j)
                        {
                            ms.WriteByte(buf[j] = window.GetAbs(off + j));
                            window.Push(buf[j]);

                            if (ms.Length == ms.Position)
                                break;
                        }
#endif
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
                /*            } finally
                            {
                                inflated_size   = (int)ms.Position;
                */
            }
        }

        public void Inflate(byte[] data)
        {
#if true
            Inflate(new MemoryStream(data, false));
#else
            int     elemdata= data[0];
            int     elems   = 0;
            byte[]  buf     = new byte[LZSSWindow.LENGTH_MAX + 1];

            for(int i= 1; i < data.Length; )
            {
                if((elemdata & (1 << elems)) == 0)
                {   // リテラル
                    ms.WriteByte(data[i]);
                    window.Push(data[i++]);
                } else
                {   // マッチ
                    int bits= data[i+0] | (data[i+1] << 8);
                    int off = LZSSWindow.WINDOW_MAX - (bits >> LZSSWindow.LENGTH_BITS);
                    int len = (bits & LZSSWindow.LENGTH_MASK) + LZSSWindow.LENGTH_MIN;
                    i       +=2;

                    for(int j= 0; j < len; ++j)
                        ms.WriteByte(buf[j]= window[off+j]);

                    for(int j= 0; j < len; ++j)
                        window.Push(buf[j]);
                }

                ++elems;

                if(elems == 8 && i < data.Length)
                {
                    elems   = 0;
                    elemdata= data[i++];
                }
            }
#endif
        }

        public byte[] GetInflatedBytes()
        {
            return ms.ToArray();
        }
    }
}

