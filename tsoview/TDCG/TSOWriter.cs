using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using TDCG.Extensions;

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
        /// 指定ライタにnode配列を書き出します。
        /// </summary>
        /// <param name="bw">ライタ</param>
        /// <param name="items">node配列</param>
        public static void Write(BinaryWriter bw, TSONode[] items)
        {
            bw.Write(items.Length);

            foreach (TSONode i in items)
                Write(bw, i);

            bw.Write(items.Length);

            Matrix m = Matrix.Identity;
            foreach (TSONode i in items)
            {
                m = i.TransformationMatrix;
                bw.Write(ref m);
            }
        }

        /// <summary>
        /// 指定ライタにnodeを書き出します。
        /// </summary>
        /// <param name="bw">ライタ</param>
        /// <param name="item">node</param>
        public static void Write(BinaryWriter bw, TSONode item)
        {
            bw.WriteCString(item.Path);
        }

        /// <summary>
        /// 指定ライタにテクスチャ配列を書き出します。
        /// </summary>
        /// <param name="bw">ライタ</param>
        /// <param name="items">テクスチャ配列</param>
        public static void Write(BinaryWriter bw, TSOTex[] items)
        {
            bw.Write(items.Length);

            foreach (TSOTex i in items)
                Write(bw, i);
        }

        /// <summary>
        /// 指定ライタにテクスチャを書き出します。
        /// </summary>
        /// <param name="bw">ライタ</param>
        /// <param name="item">テクスチャ</param>
        public static void Write(BinaryWriter bw, TSOTex item)
        {
            bw.WriteCString(item.name);
            bw.WriteCString(item.file);
            bw.Write(item.width);
            bw.Write(item.height);
            bw.Write(item.depth);

            byte[] buf = new byte[item.data.Length];
            Array.Copy(item.data, 0, buf, 0, buf.Length);

            for(int j = 0; j < buf.Length; j += 4)
            {
                byte tmp = buf[j+2];
                buf[j+2] = buf[j+0];
                buf[j+0] = tmp;
            }
            Write(bw, buf);
        }

        /// <summary>
        /// 指定ライタにスクリプト配列を書き出します。
        /// </summary>
        /// <param name="bw">ライタ</param>
        /// <param name="items">スクリプト配列</param>
        public static void Write(BinaryWriter bw, TSOScript[] items)
        {
            bw.Write(items.Length);

            foreach (TSOScript i in items)
                Write(bw, i);
        }

        /// <summary>
        /// 指定ライタにスクリプトを書き出します。
        /// </summary>
        /// <param name="bw">ライタ</param>
        /// <param name="item">スクリプト</param>
        public static void Write(BinaryWriter bw, TSOScript item)
        {
            bw.WriteCString(item.name);
            bw.Write(item.script_data.Length);

            foreach (string i in item.script_data)
                bw.WriteCString(i);
        }

        /// <summary>
        /// 指定ライタにサブスクリプト配列を書き出します。
        /// </summary>
        /// <param name="bw">ライタ</param>
        /// <param name="items">サブスクリプト配列</param>
        public static void Write(BinaryWriter bw, TSOSubScript[] items)
        {
            bw.Write(items.Length);

            foreach (TSOSubScript i in items)
                Write(bw, i);
        }

        /// <summary>
        /// 指定ライタにサブスクリプトを書き出します。
        /// </summary>
        /// <param name="bw">ライタ</param>
        /// <param name="item">サブスクリプト</param>
        public static void Write(BinaryWriter bw, TSOSubScript item)
        {
            bw.WriteCString(item.name);
            bw.WriteCString(item.file);
            bw.Write(item.script_data.Length);

            foreach (string i in item.script_data)
                bw.WriteCString(i);
        }

        /// <summary>
        /// 指定ライタにフレーム配列を書き出します。
        /// </summary>
        /// <param name="bw">ライタ</param>
        /// <param name="items">フレーム配列</param>
        public static void Write(BinaryWriter bw, TSOFrame[] items)
        {
            bw.Write(items.Length);

            foreach (TSOFrame i in items)
                i.Write(bw);
        }
    }
}
