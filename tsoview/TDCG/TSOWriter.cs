using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace TDCG
{
    /// <summary>
    /// TSOFileを書き出すメソッド群
    /// </summary>
    public class TSOWriter
    {
        /// <summary>
        /// 指定ライタ に 'TSO1' を書き出します。
        /// </summary>
        /// <param name="bw">ライタ</param>
        public static void WriteMagic(BinaryWriter bw)
        {
            bw.Write(0x314F5354);
        }

        public static void Write(BinaryWriter bw, byte[] bytes)
        {
            bw.Write(bytes);
        }

        public static void Write(BinaryWriter bw, string s)
        {
            foreach(byte i in Encoding.Default.GetBytes(s))
                bw.Write(i);

            bw.Write((byte)0);
        }

        public static void Write(BinaryWriter bw, TSONode[] items)
        {
            bw.Write(items.Length);

            foreach(var i in items)
                Write(bw, i);

            bw.Write(items.Length);

            foreach(var i in items)
                Write(bw, ref i.transformation_matrix);
        }

        public static void Write(BinaryWriter bw, TSONode item)
        {
            Write(bw, item.Name);
        }

        public static void Write(BinaryWriter bw, TSOTex[] items)
        {
            bw.Write(items.Length);

            foreach(var i in items)
                Write(bw, i);
        }

        public static void Write(BinaryWriter bw, TSOTex item)
        {
            Write(bw, item.name);
            Write(bw, item.file);
            bw.Write(item.width);
            bw.Write(item.height);
            bw.Write(item.depth);
            Write(bw, item.data);
        }

        public static void Write(BinaryWriter bw, ref Matrix m)
        {
            bw.Write(m.M11); bw.Write(m.M12); bw.Write(m.M13); bw.Write(m.M14);
            bw.Write(m.M21); bw.Write(m.M22); bw.Write(m.M23); bw.Write(m.M24);
            bw.Write(m.M31); bw.Write(m.M32); bw.Write(m.M33); bw.Write(m.M34);
            bw.Write(m.M41); bw.Write(m.M42); bw.Write(m.M43); bw.Write(m.M44);
        }
    }
}
