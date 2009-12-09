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
                i.Write(bw);
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
                i.Write(bw);
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
                i.Write(bw);
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
