using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace TDCGUtils.Extensions
{
    public static class BinaryWriterMethods
    {
        public static void WriteCString(this BinaryWriter bw, String str, int i_length)
        {
            if (str != null)
            {
                bw.Write(Encoding.GetEncoding(932).GetBytes(str)); // Shift JISとしてbyte配列に変換
                if (i_length > Encoding.GetEncoding(932).GetBytes(str).Length)
                    bw.Write(Convert.ToByte("00", 16)); // 文字列終端の記号
                for (int i = 0; i < (i_length - 1 - Encoding.GetEncoding(932).GetBytes(str).Length); i++)
                    bw.Write(Convert.ToByte("FD", 16)); // ストリームの位置を合わせるため、意味のないByteを書き込む
            }
            else
            {
                bw.Write(Convert.ToByte("00", 16)); // 文字列終端の記号
                for (int i = 0; i < (i_length - 1); i++)
                    bw.Write(Convert.ToByte("FD", 16)); // ストリームの位置を合わせるため、意味のないByteを書き込む
            }
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
