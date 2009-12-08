using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace TDCG
{
    /// <summary>
    /// TMOFileを書き出すメソッド群
    /// </summary>
    public class TMOWriter
    {
        /// <summary>
        /// 指定ライタ に 'TMO1' を書き出します。
        /// </summary>
        /// <param name="bw">ライタ</param>
        public static void WriteMagic(BinaryWriter bw)
        {
            bw.Write(0x314F4D54);
        }

        /// <summary>
        /// 指定ライタにbyte配列を書き出します。
        /// </summary>
        /// <param name="bw">ライタ</param>
        /// <param name="bytes">byte配列</param>
        public static void Write(BinaryWriter bw, byte[] bytes)
        {
            bw.Write(bytes);
        }

        /// <summary>
        /// 指定ライタにnull終端文字列を書き出します。
        /// </summary>
        /// <param name="bw">ライタ</param>
        /// <param name="s">文字列</param>
        public static void Write(BinaryWriter bw, string s)
        {
            foreach(byte i in Encoding.Default.GetBytes(s))
                bw.Write(i);

            bw.Write((byte)0);
        }

        /// <summary>
        /// 指定ライタにnode配列を書き出します。
        /// </summary>
        /// <param name="bw">ライタ</param>
        /// <param name="items">node配列</param>
        public static void Write(BinaryWriter bw, TMONode[] items)
        {
            bw.Write(items.Length);

            foreach (TMONode i in items)
                Write(bw, i);
        }

        /// <summary>
        /// 指定ライタにnodeを書き出します。
        /// </summary>
        /// <param name="bw">ライタ</param>
        /// <param name="item">node</param>
        public static void Write(BinaryWriter bw, TMONode item)
        {
            Write(bw, item.Path);
        }

        /// <summary>
        /// 指定ライタにframe配列を書き出します。
        /// </summary>
        /// <param name="bw">ライタ</param>
        /// <param name="items">frame配列</param>
        public static void Write(BinaryWriter bw, TMOFrame[] items)
        {
            bw.Write(items.Length);

            foreach (TMOFrame i in items)
            {
                Write(bw, i);
            }
        }

        /// <summary>
        /// 指定ライタにframeを書き出します。
        /// </summary>
        /// <param name="bw">ライタ</param>
        /// <param name="item">frame</param>
        public static void Write(BinaryWriter bw, TMOFrame item)
        {
            Write(bw, item.matrices);
        }

        /// <summary>
        /// 指定ライタに行列の配列を書き出します。
        /// </summary>
        /// <param name="bw">ライタ</param>
        /// <param name="items">行列の配列</param>
        public static void Write(BinaryWriter bw, TMOMat[] items)
        {
            bw.Write(items.Length);

            foreach (TMOMat i in items)
                Write(bw, i);
        }

        /// <summary>
        /// 指定ライタに行列を書き出します。
        /// </summary>
        /// <param name="bw">ライタ</param>
        /// <param name="item">行列</param>
        public static void Write(BinaryWriter bw, TMOMat item)
        {
            Matrix m = item.m;
            Write(bw, ref m);
        }

        /// <summary>
        /// 指定ライタに行列を書き出します。
        /// </summary>
        /// <param name="bw">ライタ</param>
        /// <param name="m">行列</param>
        public static void Write(BinaryWriter bw, ref Matrix m)
        {
            bw.Write(m.M11); bw.Write(m.M12); bw.Write(m.M13); bw.Write(m.M14);
            bw.Write(m.M21); bw.Write(m.M22); bw.Write(m.M23); bw.Write(m.M24);
            bw.Write(m.M31); bw.Write(m.M32); bw.Write(m.M33); bw.Write(m.M34);
            bw.Write(m.M41); bw.Write(m.M42); bw.Write(m.M43); bw.Write(m.M44);
        }
    }
}
