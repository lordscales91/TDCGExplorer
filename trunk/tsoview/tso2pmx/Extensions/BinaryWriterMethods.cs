using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace tso2pmx.Extensions
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
            bw.Write((int)s.Length);
            bw.Write(Encoding.UTF8.GetBytes(s));
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
        /// 指定ライタに行列を書き出します。
        /// </summary>
        /// <param name="bw">BinaryWriter</param>
        /// <param name="m">行列</param>
        public static void Write(this BinaryWriter bw, ref Matrix m)
        {
            bw.Write(m.M11); bw.Write(m.M12); bw.Write(m.M13); bw.Write(m.M14);
            bw.Write(m.M21); bw.Write(m.M22); bw.Write(m.M23); bw.Write(m.M24);
            bw.Write(m.M31); bw.Write(m.M32); bw.Write(m.M33); bw.Write(m.M34);
            bw.Write(m.M41); bw.Write(m.M42); bw.Write(m.M43); bw.Write(m.M44);
        }
    }
}
