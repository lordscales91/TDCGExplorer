using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace tso2mqo.Extensions
{
    /// <summary>
    /// BinaryReaderの拡張メソッドを定義します。
    /// </summary>
    public static class BinaryReaderMethods
    {
        /// <summary>
        /// null終端文字列を読みとります。
        /// </summary>
        /// <returns>文字列</returns>
        public static string ReadCString(this BinaryReader reader)
        {
            StringBuilder string_builder = new StringBuilder();
            while ( true ) {
                char c = reader.ReadChar();
                if (c == 0) break;
                string_builder.Append(c);
            }
            return string_builder.ToString();
        }

        /// <summary>
        /// Matrixを読みとります。
        /// </summary>
        /// <param name="reader">BinaryReader</param>
        /// <param name="m">Matrix</param>
        public static void ReadMatrix(this BinaryReader reader, ref Matrix m)
        {
            m.M11 = reader.ReadSingle();
            m.M12 = reader.ReadSingle();
            m.M13 = reader.ReadSingle();
            m.M14 = reader.ReadSingle();

            m.M21 = reader.ReadSingle();
            m.M22 = reader.ReadSingle();
            m.M23 = reader.ReadSingle();
            m.M24 = reader.ReadSingle();

            m.M31 = reader.ReadSingle();
            m.M32 = reader.ReadSingle();
            m.M33 = reader.ReadSingle();
            m.M34 = reader.ReadSingle();

            m.M41 = reader.ReadSingle();
            m.M42 = reader.ReadSingle();
            m.M43 = reader.ReadSingle();
            m.M44 = reader.ReadSingle();
        }

        /// <summary>
        /// Vector3を読みとります。
        /// </summary>
        /// <param name="reader">BinaryReader</param>
        /// <param name="v">Vector3</param>
        public static void ReadVector3(this BinaryReader reader, ref Vector3 v)
        {
            v.X = reader.ReadSingle();
            v.Y = reader.ReadSingle();
            v.Z = reader.ReadSingle();
        }
    }
}
