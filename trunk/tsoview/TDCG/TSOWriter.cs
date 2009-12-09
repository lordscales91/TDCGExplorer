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
        /// �w�胉�C�^��node�z��������o���܂��B
        /// </summary>
        /// <param name="bw">���C�^</param>
        /// <param name="items">node�z��</param>
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
        /// �w�胉�C�^��node�������o���܂��B
        /// </summary>
        /// <param name="bw">���C�^</param>
        /// <param name="item">node</param>
        public static void Write(BinaryWriter bw, TSONode item)
        {
            bw.WriteCString(item.Path);
        }

        /// <summary>
        /// �w�胉�C�^�Ƀe�N�X�`���z��������o���܂��B
        /// </summary>
        /// <param name="bw">���C�^</param>
        /// <param name="items">�e�N�X�`���z��</param>
        public static void Write(BinaryWriter bw, TSOTex[] items)
        {
            bw.Write(items.Length);

            foreach (TSOTex i in items)
                i.Write(bw);
        }

        /// <summary>
        /// �w�胉�C�^�ɃX�N���v�g�z��������o���܂��B
        /// </summary>
        /// <param name="bw">���C�^</param>
        /// <param name="items">�X�N���v�g�z��</param>
        public static void Write(BinaryWriter bw, TSOScript[] items)
        {
            bw.Write(items.Length);

            foreach (TSOScript i in items)
                i.Write(bw);
        }

        /// <summary>
        /// �w�胉�C�^�ɃT�u�X�N���v�g�z��������o���܂��B
        /// </summary>
        /// <param name="bw">���C�^</param>
        /// <param name="items">�T�u�X�N���v�g�z��</param>
        public static void Write(BinaryWriter bw, TSOSubScript[] items)
        {
            bw.Write(items.Length);

            foreach (TSOSubScript i in items)
                i.Write(bw);
        }

        /// <summary>
        /// �w�胉�C�^�Ƀt���[���z��������o���܂��B
        /// </summary>
        /// <param name="bw">���C�^</param>
        /// <param name="items">�t���[���z��</param>
        public static void Write(BinaryWriter bw, TSOFrame[] items)
        {
            bw.Write(items.Length);

            foreach (TSOFrame i in items)
                i.Write(bw);
        }
    }
}
