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
            Write(bw, item.Path);
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

            byte[] buf = new byte[item.data.Length];
            Array.Copy(item.data, 0, buf, 0, buf.Length);

            for(int j = 0; j < buf.Length; j += 4)
            {
                byte tmp = buf[j+2];
                buf[j+2] = buf[j+0];
                buf[j+0] = tmp;
            }
            Write(bw, buf);
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
                Write(bw, i);
        }

        /// <summary>
        /// �w�胉�C�^�ɃX�N���v�g�������o���܂��B
        /// </summary>
        /// <param name="bw">���C�^</param>
        /// <param name="item">�X�N���v�g</param>
        public static void Write(BinaryWriter bw, TSOScript item)
        {
            Write(bw, item.name);
            bw.Write(item.script_data.Length);

            foreach (string i in item.script_data)
                Write(bw, i);
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
                Write(bw, i);
        }

        /// <summary>
        /// �w�胉�C�^�ɃT�u�X�N���v�g�������o���܂��B
        /// </summary>
        /// <param name="bw">���C�^</param>
        /// <param name="item">�T�u�X�N���v�g</param>
        public static void Write(BinaryWriter bw, TSOSubScript item)
        {
            Write(bw, item.name);
            Write(bw, item.file);
            bw.Write(item.script_data.Length);

            foreach (string i in item.script_data)
                Write(bw, i);
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
                Write(bw, i);
        }

        /// <summary>
        /// �w�胉�C�^�Ƀt���[���������o���܂��B
        /// </summary>
        /// <param name="bw">���C�^</param>
        /// <param name="item">�t���[��</param>
        public static void Write(BinaryWriter bw, TSOFrame item)
        {
            Write(bw, item.name);
            Matrix m = item.transform_matrix;
            bw.Write(ref m);
            bw.Write(item.unknown1);
            bw.Write(item.meshes.Length);

            foreach (TSOMesh i in item.meshes)
                i.Write(bw);
        }
    }
}
