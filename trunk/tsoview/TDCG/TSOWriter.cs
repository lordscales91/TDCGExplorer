using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace TDCG
{
    /// <summary>
    /// TSOFile�������o�����\�b�h�Q
    /// </summary>
    public class TSOWriter
    {
        /// <summary>
        /// �w�胉�C�^ �� 'TSO1' �������o���܂��B
        /// </summary>
        /// <param name="bw">���C�^</param>
        public static void WriteMagic(BinaryWriter bw)
        {
            bw.Write(0x314F5354);
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
        public static void Write(BinaryWriter bw, TSONode[] items)
        {
            bw.Write(items.Length);

            foreach(var i in items)
                Write(bw, i);

            bw.Write(items.Length);

            Matrix m = Matrix.Identity;
            foreach (var i in items)
            {
                m = i.TransformationMatrix;
                Write(bw, ref m);
            }
        }

        /// <summary>
        /// �w�胉�C�^��node�������o���܂��B
        /// </summary>
        /// <param name="bw">���C�^</param>
        /// <param name="item">node</param>
        public static void Write(BinaryWriter bw, TSONode item)
        {
            Write(bw, item.Name);
        }

        /// <summary>
        /// �w�胉�C�^�Ƀe�N�X�`���z��������o���܂��B
        /// </summary>
        /// <param name="bw">���C�^</param>
        /// <param name="items">�e�N�X�`���z��</param>
        public static void Write(BinaryWriter bw, TSOTex[] items)
        {
            bw.Write(items.Length);

            foreach(var i in items)
                Write(bw, i);
        }

        /// <summary>
        /// �w�胉�C�^�Ƀe�N�X�`���������o���܂��B
        /// </summary>
        /// <param name="bw">���C�^</param>
        /// <param name="item">�e�N�X�`��</param>
        public static void Write(BinaryWriter bw, TSOTex item)
        {
            Write(bw, item.name);
            Write(bw, item.file);
            bw.Write(item.width);
            bw.Write(item.height);
            bw.Write(item.depth);
            Write(bw, item.data);
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
