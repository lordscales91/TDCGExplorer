using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace TDCGUtils.Extensions
{
    /// <summary>
    /// BinaryWriterの拡張メソッドを定義します。
    /// </summary>
    public static class BinaryWriterMethods
    {
        /// <summary>
        /// 指定ライタに文字byte長と文字列を書き出します。
        /// </summary>
        /// <param name="bw">BinaryWriter</param>
        /// <param name="s">文字列</param>
        public static void WritePString(this BinaryWriter bw, string s)
        {
            if (s == null)
            {
                bw.Write((int)0);
                return;
            }
            byte[] buf = Encoding.UTF8.GetBytes(s);
            bw.Write(buf.Length);
            bw.Write(buf);
        }

        /// <summary>
        /// 指定ライタにベクトルを書き出します。
        /// </summary>
        /// <param name="bw">BinaryWriter</param>
        /// <param name="v">ベクトル</param>
        public static void Write(this BinaryWriter bw, ref Vector3 v)
        {
            bw.Write(v.X);
            bw.Write(v.Y);
            bw.Write(v.Z);
        }

        /// <summary>
        /// 指定ライタにベクトルを書き出します。
        /// </summary>
        /// <param name="bw">BinaryWriter</param>
        /// <param name="v">ベクトル</param>
        public static void Write(this BinaryWriter bw, ref Vector4 v)
        {
            bw.Write(v.X);
            bw.Write(v.Y);
            bw.Write(v.Z);
            bw.Write(v.W);
        }
    }
}
