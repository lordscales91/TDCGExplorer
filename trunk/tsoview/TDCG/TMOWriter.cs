using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace TDCG
{
    /// <summary>
    /// TMOFile�������o�����\�b�h�Q
    /// </summary>
    public class TMOWriter
    {
        /// <summary>
        /// �w�胉�C�^ �� 'TMO1' �������o���܂��B
        /// </summary>
        /// <param name="bw">���C�^</param>
        public static void WriteMagic(BinaryWriter bw)
        {
            bw.Write(0x314F4D54);
        }

        /// <summary>
        /// �w�胉�C�^��byte�z��������o���܂��B
        /// </summary>
        /// <param name="bw">���C�^</param>
        /// <param name="bytes">byte�z��</param>
        public static void Write(BinaryWriter bw, byte[] bytes)
        {
            bw.Write(bytes);
        }

        /// <summary>
        /// �w�胉�C�^��null�I�[������������o���܂��B
        /// </summary>
        /// <param name="bw">���C�^</param>
        /// <param name="s">������</param>
        public static void Write(BinaryWriter bw, string s)
        {
            foreach(byte i in Encoding.Default.GetBytes(s))
                bw.Write(i);

            bw.Write((byte)0);
        }

        /// <summary>
        /// �w�胉�C�^��node�z��������o���܂��B
        /// </summary>
        /// <param name="bw">���C�^</param>
        /// <param name="items">node�z��</param>
        public static void Write(BinaryWriter bw, TMONode[] items)
        {
            bw.Write(items.Length);

            foreach (TMONode i in items)
                Write(bw, i);
        }

        /// <summary>
        /// �w�胉�C�^��node�������o���܂��B
        /// </summary>
        /// <param name="bw">���C�^</param>
        /// <param name="item">node</param>
        public static void Write(BinaryWriter bw, TMONode item)
        {
            Write(bw, item.Path);
        }

        /// <summary>
        /// �w�胉�C�^��frame�z��������o���܂��B
        /// </summary>
        /// <param name="bw">���C�^</param>
        /// <param name="items">frame�z��</param>
        public static void Write(BinaryWriter bw, TMOFrame[] items)
        {
            bw.Write(items.Length);

            foreach (TMOFrame i in items)
            {
                Write(bw, i);
            }
        }

        /// <summary>
        /// �w�胉�C�^��frame�������o���܂��B
        /// </summary>
        /// <param name="bw">���C�^</param>
        /// <param name="item">frame</param>
        public static void Write(BinaryWriter bw, TMOFrame item)
        {
            Write(bw, item.matrices);
        }

        /// <summary>
        /// �w�胉�C�^�ɍs��̔z��������o���܂��B
        /// </summary>
        /// <param name="bw">���C�^</param>
        /// <param name="items">�s��̔z��</param>
        public static void Write(BinaryWriter bw, TMOMat[] items)
        {
            bw.Write(items.Length);

            foreach (TMOMat i in items)
                Write(bw, i);
        }

        /// <summary>
        /// �w�胉�C�^�ɍs��������o���܂��B
        /// </summary>
        /// <param name="bw">���C�^</param>
        /// <param name="item">�s��</param>
        public static void Write(BinaryWriter bw, TMOMat item)
        {
            Matrix m = item.m;
            Write(bw, ref m);
        }

        /// <summary>
        /// �w�胉�C�^�ɍs��������o���܂��B
        /// </summary>
        /// <param name="bw">���C�^</param>
        /// <param name="m">�s��</param>
        public static void Write(BinaryWriter bw, ref Matrix m)
        {
            bw.Write(m.M11); bw.Write(m.M12); bw.Write(m.M13); bw.Write(m.M14);
            bw.Write(m.M21); bw.Write(m.M22); bw.Write(m.M23); bw.Write(m.M24);
            bw.Write(m.M31); bw.Write(m.M32); bw.Write(m.M33); bw.Write(m.M34);
            bw.Write(m.M41); bw.Write(m.M42); bw.Write(m.M43); bw.Write(m.M44);
        }
    }
}
