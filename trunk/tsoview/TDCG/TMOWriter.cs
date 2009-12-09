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
        /// �w�胉�C�^��node�z��������o���܂��B
        /// </summary>
        /// <param name="bw">���C�^</param>
        /// <param name="items">node�z��</param>
        public static void Write(BinaryWriter bw, TMONode[] items)
        {
            bw.Write(items.Length);

            foreach (TMONode i in items)
                i.Write(bw);
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
            bw.Write(ref m);
        }
    }
}
