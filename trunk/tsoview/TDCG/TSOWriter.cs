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

            foreach (TSONode i in items)
                Write(bw, i);

            bw.Write(items.Length);

            Matrix m = Matrix.Identity;
            foreach (TSONode i in items)
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

        public static void Write(BinaryWriter bw, TSOScript[] items)
        {
            bw.Write(items.Length);

            foreach (TSOScript i in items)
                Write(bw, i);
        }

        public static void Write(BinaryWriter bw, TSOScript item)
        {
            Write(bw, item.name);
            bw.Write(item.script_data.Length);

            foreach (string i in item.script_data)
                Write(bw, i);
        }

        public static void Write(BinaryWriter bw, TSOSubScript[] items)
        {
            bw.Write(items.Length);

            foreach (TSOSubScript i in items)
                Write(bw, i);
        }

        public static void Write(BinaryWriter bw, TSOSubScript item)
        {
            Write(bw, item.name);
            Write(bw, item.file);
            bw.Write(item.script_data.Length);

            foreach (string i in item.script_data)
                Write(bw, i);
        }

        public static void Write(BinaryWriter bw, TSOMesh[] items)
        {
            bw.Write(items.Length);

            foreach (TSOMesh i in items)
                Write(bw, i);
        }

        public static void Write(BinaryWriter bw, TSOMesh item)
        {
            Write(bw, item.name);
            Matrix m = item.transform_matrix;
            Write(bw, ref m);
            bw.Write(item.unknown1);
            bw.Write(item.sub_meshes.Length);

            foreach (TSOSubMesh i in item.sub_meshes)
                Write(bw, i);
        }

        public static void Write(BinaryWriter bw, TSOSubMesh[] items)
        {
            bw.Write(items.Length);

            foreach (TSOSubMesh i in items)
                Write(bw, i);
        }

        public static void Write(BinaryWriter bw, TSOSubMesh item)
        {
            bw.Write(item.spec);
            bw.Write(item.bone_index_LUT.Count);

            foreach (uint i in item.bone_index_LUT)
                bw.Write(i);
            bw.Write(item.vertices.Length);

            for (int i = 0; i < item.vertices.Length; i++)
            {
                Write(bw, ref item.vertices[i]);
            }
        }

        public static void Write(BinaryWriter bw, ref vertex_field v)
        {
            Write(bw, ref v.position);
            Write(bw, ref v.normal);
            bw.Write(v.u);
            bw.Write(v.v);

            int bone_weight_entry_count = 0;
            SkinWeight[] skin_weights = new SkinWeight[4];
            foreach (SkinWeight i in v.skin_weights)
            {
                if (i.weight == 0.0f)
                    continue;

                skin_weights[bone_weight_entry_count++] = i;
            }
            bw.Write(bone_weight_entry_count);

            for (int i = 0; i < bone_weight_entry_count; i++)
            {
                bw.Write(skin_weights[i].index);
                bw.Write(skin_weights[i].weight);
            }
        }

        public static void Write(BinaryWriter bw, ref Vector3 v)
        {
            bw.Write(v.X);
            bw.Write(v.Y);
            bw.Write(v.Z);
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
