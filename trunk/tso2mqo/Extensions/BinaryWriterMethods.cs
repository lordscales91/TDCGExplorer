using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace tso2mqo.Extensions
{
    /// <summary>
    /// BinaryWriter�̊g�����\�b�h���`���܂��B
    /// </summary>
    public static class BinaryWriterMethods
    {
        /// <summary>
        /// �w�胉�C�^��null�I�[������������o���܂��B
        /// </summary>
        /// <param name="bw">BinaryWriter</param>
        /// <param name="s">null�I�[������</param>
        public static void WriteCString(this BinaryWriter bw, string s)
        {
            foreach(byte i in Encoding.Default.GetBytes(s))
                bw.Write(i);

            bw.Write((byte)0);
        }

        /// <summary>
        /// �w�胉�C�^�Ƀx�N�g���������o���܂��B
        /// </summary>
        /// <param name="bw">BinaryWriter</param>
        /// <param name="v">�x�N�g��</param>
        public static void Write(this BinaryWriter bw, ref Vector3 v)
        {
            bw.Write(v.X);
            bw.Write(v.Y);
            bw.Write(v.Z);
        }

        /// <summary>
        /// �w�胉�C�^�ɍs��������o���܂��B
        /// </summary>
        /// <param name="bw">BinaryWriter</param>
        /// <param name="m">�s��</param>
        public static void Write(this BinaryWriter bw, ref Matrix m)
        {
            bw.Write(m.M11); bw.Write(m.M12); bw.Write(m.M13); bw.Write(m.M14);
            bw.Write(m.M21); bw.Write(m.M22); bw.Write(m.M23); bw.Write(m.M24);
            bw.Write(m.M31); bw.Write(m.M32); bw.Write(m.M33); bw.Write(m.M34);
            bw.Write(m.M41); bw.Write(m.M42); bw.Write(m.M43); bw.Write(m.M44);
        }
    }
}
