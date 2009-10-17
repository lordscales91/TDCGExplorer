using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace TDCG
{
    /// <summary>
    /// TMO�t�@�C���������܂��B
    /// </summary>
    public class TMOFile
    {
        /// <summary>
        /// �o�C�i���l�Ƃ��ēǂݎ��܂��B
        /// </summary>
        protected BinaryReader reader;

        /// <summary>
        /// �w�b�_
        /// </summary>
        public byte[] header;
        /// <summary>
        /// �I�v�V�����l0
        /// </summary>
        public int opt0;
        /// <summary>
        /// �I�v�V�����l1
        /// </summary>
        public int opt1;
        /// <summary>
        /// bone�z��
        /// </summary>
        public TMONode[] nodes;
        /// <summary>
        /// �t���[���z��
        /// </summary>
        public TMOFrame[] frames;
        /// <summary>
        /// �t�b�^
        /// </summary>
        public byte[] footer;

        internal Dictionary<string, TMONode> nodemap;

        /// <summary>
        /// �w��p�X�ɕۑ����܂��B
        /// </summary>
        /// <param name="dest_file">�p�X</param>
        public void Save(string dest_file)
        {
            using (Stream dest_stream = File.Create(dest_file))
                Save(dest_stream);
        }

        /// <summary>
        /// �w��X�g���[���ɕۑ����܂��B
        /// </summary>
        /// <param name="dest_stream">�X�g���[��</param>
        public void Save(Stream dest_stream)
        {
            BinaryWriter bw = new BinaryWriter(dest_stream);

            TMOWriter.WriteMagic(bw);
            TMOWriter.Write(bw, header);
            bw.Write(opt0);
            bw.Write(opt1);
            TMOWriter.Write(bw, nodes);
            TMOWriter.Write(bw, frames);
            TMOWriter.Write(bw, footer);
        }

        /// <summary>
        /// �w��p�X����ǂݍ��݂܂��B
        /// </summary>
        /// <param name="source_file">�p�X</param>
        public void Load(string source_file)
        {
            using (Stream source_stream = File.OpenRead(source_file))
                Load(source_stream);
        }

        /// <summary>
        /// �w��X�g���[������ǂݍ��݂܂��B
        /// </summary>
        /// <param name="source_stream">�X�g���[��</param>
        public void Load(Stream source_stream)
        {
            this.reader = new BinaryReader(source_stream, System.Text.Encoding.Default);

            byte[] magic = reader.ReadBytes(4);

            if(magic[0] != (byte)'T'
            || magic[1] != (byte)'M'
            || magic[2] != (byte)'O'
            || magic[3] != (byte)'1')
                throw new Exception("File is not TMO");

            this.header = reader.ReadBytes(8);
            this.opt0 = reader.ReadInt32();
            this.opt1 = reader.ReadInt32();

            int node_count = reader.ReadInt32();
            nodes = new TMONode[node_count];

            for (int i = 0; i < node_count; i++)
            {
                string name = ReadString();
                nodes[i] = new TMONode(i, name);
            }

            GenerateNodemapAndTree();

            int frame_count = reader.ReadInt32();
            frames = new TMOFrame[frame_count];

            for (int i = 0; i < frame_count; i++)
            {
                frames[i] = new TMOFrame();
                frames[i].id = i;

                int matrix_count = reader.ReadInt32();
                frames[i].matrices = new TMOMat[matrix_count];

                for (int j = 0; j < matrix_count; j++)
                {
                    TMOMat mat = frames[i].matrices[j] = new TMOMat();
                    ReadMatrix(ref mat.m);
                    nodes[j].frame_matrices.Add(mat);
                }
            }

            this.footer = reader.ReadBytes(4);
        }

        internal void GenerateNodemapAndTree()
        {
            nodemap = new Dictionary<string, TMONode>();

            for (int i = 0; i < nodes.Length; i++)
            {
                nodemap.Add(nodes[i].Name, nodes[i]);
            }

            for (int i = 0; i < nodes.Length; i++)
            {
                int index = nodes[i].Name.LastIndexOf('|');
                if (index <= 0)
                    continue;
                string pname = nodes[i].Name.Substring(0, index);
                nodes[i].parent = nodemap[pname];
                nodes[i].parent.child_nodes.Add(nodes[i]);
            }
        }

        /// <summary>
        /// �s��𓾂܂��B
        /// </summary>
        /// <param name="name">bone����</param>
        /// <param name="frame_index">�t���[���ԍ�</param>
        /// <returns></returns>
        public TMOMat GetTMOMat(string name, int frame_index)
        {
            return frames[frame_index].matrices[nodemap[name].ID];
        }

        /// <summary>
        /// node id�̃y�A���쐬���܂��B
        /// �y�A�̃L�[��node id
        /// �y�A�̒l�͂����ЂƂ�tmo�ɂ����Ė��́i�Z���`���j����v����node id�ɂȂ�܂��B
        /// </summary>
        /// <param name="motion">�����ЂƂ�tmo</param>
        /// <returns>tmo frame index�̃y�A</returns>
        public int[] CreateNodeIdPair(TMOFile motion)
        {
            Dictionary<string, TMONode> source_nodes = new Dictionary<string, TMONode>();

            Dictionary<string, TMONode> motion_nodes = new Dictionary<string, TMONode>();

            foreach (TMONode node in motion.nodes)
                try {
                    motion_nodes.Add(node.ShortName, node);
                } catch (ArgumentException) {
                    Console.WriteLine("node {0} already exists.", node.ShortName);
                }
            foreach (TMONode node in nodes)
            {
                if (! motion_nodes.ContainsKey(node.ShortName))
                {
                    throw new ArgumentException("error: node not found in motion: " + node.ShortName);
                }
                try {
                    source_nodes.Add(node.ShortName, node);
                } catch (ArgumentException) {
                    Console.WriteLine("node {0} already exists.", node.ShortName);
                }
            }

            int[] id_pair = new int[nodes.Length];

            foreach (TMONode node in nodes)
                id_pair[node.ID] = motion_nodes[node.ShortName].ID;

            return id_pair;
        }

        /// <summary>
        /// �w��tmo����t���[����ǉ����܂��B
        /// </summary>
        /// <param name="motion">tmo</param>
        public void AppendFrameFrom(TMOFile motion)
        {
            int[] id_pair = CreateNodeIdPair(motion);

            TMOFrame source_frame = frames[0];
            int append_length = motion.frames.Length;
            TMOFrame[] append_frames = new TMOFrame[append_length];
            for (int i = 0; i < motion.frames.Length; i++)
                append_frames[i] = TMOFrame.Select(source_frame, motion.frames[i], id_pair);
                
            int old_length = frames.Length;
            Array.Resize(ref frames, frames.Length + append_length);
            Array.Copy(append_frames, 0, frames, old_length, append_length);
            this.opt0 = frames.Length-1;
        }

        /// <summary>
        /// �w��tmo�փt���[�����Ԃ��܂��B
        /// </summary>
        /// <param name="motion">tmo</param>
        /// <param name="append_length">��Ԃ���t���[������</param>
        /// <param name="p1">��ԑ��x�W��</param>
        public void SlerpFrameEndTo(TMOFile motion, int append_length, float p1)
        {
            int[] id_pair = CreateNodeIdPair(motion);

            int i0 = (frames.Length > 1) ? frames.Length - 1 - 1 : 0;
            int i1 = frames.Length-1;
            int i2 = 0;
            int i3 = ( motion.frames.Length > 1 ) ? 1 : 0;

            TMOFrame frame0 = frames[i0];
            TMOFrame frame1 = frames[i1];
            TMOFrame frame2 = motion.frames[i2];
            TMOFrame frame3 = motion.frames[i3];

            TMOFrame[] interp_frames = TMOFrame.Slerp(frame0, frame1, frame2, frame3, append_length, p1, id_pair);
            int old_length = frames.Length;
            Array.Resize(ref frames, frames.Length + append_length);
            Array.Copy(interp_frames, 0, frames, old_length, append_length);
            this.opt0 = frames.Length-1;
        }

        /// <summary>
        /// �w��tmo�փt���[�����Ԃ��܂��B
        /// </summary>
        /// <param name="motion">tmo</param>
        /// <param name="append_length">��Ԃ���t���[������</param>
        public void SlerpFrameEndTo(TMOFile motion, int append_length)
        {
            SlerpFrameEndTo(motion, append_length, 0.5f);
        }

        /// <summary>
        /// �w��tmo�փt���[�����Ԃ��܂��B
        /// </summary>
        /// <param name="motion">tmo</param>
        public void SlerpFrameEndTo(TMOFile motion)
        {
            SlerpFrameEndTo(motion, 200, 0.5f);
        }

        /// <summary>
        /// �t���[�����w��index�݂̂ɐ؂�l�߂܂��B
        /// </summary>
        /// <param name="frame_index">index</param>
        public void TruncateFrame(int frame_index)
        {
            if (frame_index < 0)
                return;
            if (frame_index > frames.Length-1)
                return;
            if (frame_index > 0)
                Array.Copy(frames, frame_index, frames, 0, 1);
            Array.Resize(ref frames, 1);
            this.opt0 = 1;
        }

        /// <summary>
        /// �w�薼�́i�Z���`���j������node���������܂��B
        /// </summary>
        /// <param name="sname">node���́i�Z���`���j</param>
        /// <returns></returns>
        public TMONode FindNodeByShortName(string sname)
        {
            foreach(TMONode node in nodes)
                if (node.ShortName == sname)
                    return node;
            return null;
        }

        /// <summary>
        /// �w��tmo�̃��[�V�����i�J�n�t���[������̕ψʁj�𕡎ʂ��܂��B
        /// </summary>
        /// <param name="motion">tmo</param>
        public void CopyMotionFrom(TMOFile motion)
        {
            int[] id_pair = CreateNodeIdPair(motion);

            TMOFrame source_frame = frames[0];
            TMOFrame motion_frame = motion.frames[0];
            int append_length = motion.frames.Length;
            TMOFrame[] interp_frames = new TMOFrame[append_length];
            for (int i = 0; i < motion.frames.Length; i++)
                interp_frames[i] = TMOFrame.AddSub(source_frame, motion.frames[i], motion_frame, id_pair);
                
            int old_length = frames.Length;
            Array.Resize(ref frames, frames.Length + append_length);
            Array.Copy(interp_frames, 0, frames, old_length, append_length);
            this.opt0 = frames.Length-1;
        }

        /// <summary>
        /// �w��tmo�ɂ���w�薼�́i�Z���`���j��node�𓯂����̂�node�ɕ��ʂ��܂��B
        /// ���������ʂ̑Ώۂ͎qnode�ȍ~�ł��B�w��node�͕��ʂ��܂���B
        /// �܂��A���Onode�ȍ~��node�͕��ʂ��܂���B
        /// </summary>
        /// <param name="motion">tmo</param>
        /// <param name="sname">node���́i�Z���`���j</param>
        /// <param name="except_snames">���Onode���́i�Z���`���j���X�g</param>
        public void CopyChildrenNodeFrom(TMOFile motion, string sname, List<string> except_snames)
        {
            TMONode node = this.FindNodeByShortName(sname);
            if (node == null)
                return;
            TMONode motion_node = motion.FindNodeByShortName(sname);
            if (motion_node == null)
                return;
            node.CopyChildrenMatFrom(motion_node, except_snames);
        }

        /// <summary>
        /// �w��tmo�ɂ���w�薼�́i�Z���`���j��node�𓯂����̂�node�ɕ��ʂ��܂��B
        /// </summary>
        /// <param name="motion">tmo</param>
        /// <param name="sname">node���́i�Z���`���j</param>
        public void CopyNodeFrom(TMOFile motion, string sname)
        {
            TMONode node = this.FindNodeByShortName(sname);
            if (node == null)
                return;
            TMONode motion_node = motion.FindNodeByShortName(sname);
            if (motion_node == null)
                return;
            node.CopyMatFrom(motion_node);
        }

        /// <summary>
        /// �w��tmo�Ɠ���node tree�������B
        /// </summary>
        /// <param name="motion">tmo</param>
        /// <returns></returns>
        public bool IsSameNodeTree(TMOFile motion) {
            if (nodes.Length != motion.nodes.Length) {
                //Console.WriteLine("nodes length mismatch {0} {1}", nodes.Length, motion.nodes.Length);
                return false;
            }
            int i = 0;
            foreach(TMONode node in nodes) {
                TMONode motion_node = motion.nodes[i];
                //Console.WriteLine("node ShortName {0} {1}", node.ShortName, motion_node.ShortName);
                if (motion_node.ShortName != node.ShortName)
                    return false;
                i++;
            }
            return true;
        }

        /// <summary>
        /// null�I�[�������ǂ݂Ƃ�܂��B
        /// </summary>
        /// <returns>������</returns>
        public string ReadString()
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
        /// Matrix��ǂ݂Ƃ�܂��B
        /// </summary>
        /// <param name="m">Matrix</param>
        public void ReadMatrix(ref Matrix m)
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
    }

    /// <summary>
    /// Direct3D Matrix�̃��b�p
    /// </summary>
    public class TMOMat
    {
        internal Matrix m;

        /// <summary>
        /// TMOMat���쐬���܂��B
        /// </summary>
        public TMOMat()
        {
        }

        /// <summary>
        /// TMOMat���쐬���܂��B
        /// </summary>
        /// <param name="m">matrix</param>
        public TMOMat(Matrix m)
        {
            this.m = m;
        }

        /// <summary>
        /// �w��䗦�Ŋg�債�܂��B
        /// </summary>
        /// <param name="x">X���g��䗦</param>
        /// <param name="y">Y���g��䗦</param>
        /// <param name="z">Z���g��䗦</param>
        public void Scale(float x, float y, float z)
        {
            /*
            m.M11 *= x;
            m.M22 *= y;
            m.M33 *= z;
            */
            m.Multiply(Matrix.Scaling(x, y, z));
            m.M41 /= x;
            m.M42 /= y;
            m.M43 /= z;
        }

        /// <summary>
        /// �w��s��Ŋg�債�܂��B
        /// </summary>
        /// <param name="scaling">scaling matrix</param>
        public void Scale(Matrix scaling)
        {
            /*
            m.M11 *= x;
            m.M22 *= y;
            m.M33 *= z;
            */
            m.Multiply(scaling);
            m.M41 /= scaling.M11;
            m.M42 /= scaling.M22;
            m.M43 /= scaling.M33;
        }

        /// <summary>
        /// �w��s��ŏk�����܂��B�ʒu�͕ύX���܂���B
        /// </summary>
        /// <param name="scaling">scaling matrix</param>
        public void Scale0(Matrix scaling)
        {
            m.M11 /= scaling.M11;
            m.M21 /= scaling.M11;
            m.M31 /= scaling.M11;
            m.M12 /= scaling.M22;
            m.M22 /= scaling.M22;
            m.M32 /= scaling.M22;
            m.M13 /= scaling.M33;
            m.M23 /= scaling.M33;
            m.M33 /= scaling.M33;
        }

        /// <summary>
        /// �w��s��Ŋg�債�܂��B�ʒu�͕ύX���܂���B
        /// </summary>
        /// <param name="scaling">scaling matrix</param>
        public void Scale1(Matrix scaling)
        {
            m.M11 *= scaling.M11;
            m.M12 *= scaling.M11;
            m.M13 *= scaling.M11;
            m.M21 *= scaling.M22;
            m.M22 *= scaling.M22;
            m.M23 *= scaling.M22;
            m.M31 *= scaling.M33;
            m.M32 *= scaling.M33;
            m.M33 *= scaling.M33;
        }

        /// <summary>
        /// �w��p�x��X����]���܂��B
        /// </summary>
        /// <param name="angle">�p�x�i���W�A���j</param>
        public void RotateX(float angle)
        {
            Vector3 v = new Vector3(m.M11, m.M12, m.M13);
            Quaternion qt = Quaternion.RotationAxis(v, angle);
            m *= Matrix.RotationQuaternion(qt);
        }

        /// <summary>
        /// �w��p�x��Y����]���܂��B
        /// </summary>
        /// <param name="angle">�p�x�i���W�A���j</param>
        public void RotateY(float angle)
        {
            Vector3 v = new Vector3(m.M21, m.M22, m.M23);
            Quaternion qt = Quaternion.RotationAxis(v, angle);
            m *= Matrix.RotationQuaternion(qt);
        }

        /// <summary>
        /// �w��p�x��Z����]���܂��B
        /// </summary>
        /// <param name="angle">�p�x�i���W�A���j</param>
        public void RotateZ(float angle)
        {
            Vector3 v = new Vector3(m.M31, m.M32, m.M33);
            Quaternion qt = Quaternion.RotationAxis(v, angle);
            m *= Matrix.RotationQuaternion(qt);
        }

        /// <summary>
        /// ���[���h���W�n�ɂ����Ďw��p�x��Y����]���܂��B
        /// </summary>
        /// <param name="angle">�p�x�i���W�A���j</param>
        public void RotateWorldY(float angle)
        {
            Vector3 v = new Vector3(0.0f, 1.0f, 0.0f);
            Quaternion qt = Quaternion.RotationAxis(v, angle);
            m *= Matrix.RotationQuaternion(qt);
        }

        /// <summary>
        /// �w��ψʂ����ړ����܂��B
        /// </summary>
        /// <param name="translation">�ψ�</param>
        public void Move(Vector3 translation)
        {
            m.M41 += translation.X;
            m.M42 += translation.Y;
            m.M43 += translation.Z;
        }

        /// <summary>
        /// ��Ԃ��s���܂��B
        /// </summary>
        /// <param name="mat0">�s��0</param>
        /// <param name="mat1">�s��1</param>
        /// <param name="mat2">�s��2</param>
        /// <param name="mat3">�s��3</param>
        /// <param name="length">������</param>
        /// <returns>����������TMOMat�����z��</returns>
        public static TMOMat[] Slerp(TMOMat mat0, TMOMat mat1, TMOMat mat2, TMOMat mat3, int length)
        {
            return Slerp(mat0, mat1, mat2, mat3, length, 0.5f);
        }

        /// <summary>
        /// ��Ԃ��s���܂��B
        /// </summary>
        /// <param name="mat0">�s��0</param>
        /// <param name="mat1">�s��1</param>
        /// <param name="mat2">�s��2</param>
        /// <param name="mat3">�s��3</param>
        /// <param name="length">������</param>
        /// <param name="p1">��ԑ��x�W��</param>
        /// <returns>����������TMOMat�����z��</returns>
        public static TMOMat[] Slerp(TMOMat mat0, TMOMat mat1, TMOMat mat2, TMOMat mat3, int length, float p1)
        {
            TMOMat[] ret = new TMOMat[length];

            Quaternion q1 = Quaternion.RotationMatrix(mat1.m);
            Quaternion q2 = Quaternion.RotationMatrix(mat2.m);

            Vector3 v0 = new Vector3(mat0.m.M41, mat0.m.M42, mat0.m.M43);
            Vector3 v1 = new Vector3(mat1.m.M41, mat1.m.M42, mat1.m.M43);
            Vector3 v2 = new Vector3(mat2.m.M41, mat2.m.M42, mat2.m.M43);
            Vector3 v3 = new Vector3(mat3.m.M41, mat3.m.M42, mat3.m.M43);

            float p0 = 0.0f;
            float p2 = 1.0f;
            float dt = 1.0f/length;
            for (int i = 0; i < length; i++)
            {
                float t = dt*i;
                float p = t*t*(p2-2*p1+p0) + t*(2*p1-2*p0) + p0;
                ret[i] = new TMOMat(Matrix.RotationQuaternion(Quaternion.Slerp(q1, q2, p)) * Matrix.Translation(Vector3.CatmullRom(v0, v1, v2, v3, p)));
            }
            return ret;
        }

        /// <summary>
        /// ��]�s��ƈʒu�x�N�g���ɕ������܂��B
        /// </summary>
        /// <param name="m">���̍s��i�߂�l�͉�]�s��j</param>
        /// <returns>�ʒu�x�N�g��</returns>
        public static Vector3 DecomposeMatrix(ref Matrix m)
        {
            Vector3 t = new Vector3(m.M41, m.M42, m.M43);
            m.M41 = 0;
            m.M42 = 0;
            m.M43 = 0;
            return t;
        }

        /// <summary>
        /// �g��k���x�N�g���Ɖ�]�s��ƈʒu�x�N�g���ɕ������܂��B
        /// </summary>
        /// <param name="m">���̍s��i�߂�l�͉�]�s��j</param>
        /// <param name="scaling">�g��k���x�N�g��</param>
        /// <returns>�ʒu�x�N�g��</returns>
        public static Vector3 DecomposeMatrix(ref Matrix m, out Vector3 scaling)
        {
            Vector3 vx = new Vector3(m.M11, m.M12, m.M13);
            Vector3 vy = new Vector3(m.M21, m.M22, m.M23);
            Vector3 vz = new Vector3(m.M31, m.M32, m.M33);
            Vector3 vt = new Vector3(m.M41, m.M42, m.M43);
            float scax = vx.Length();
            float scay = vy.Length();
            float scaz = vz.Length();
            scaling = new Vector3(scax, scay, scaz);
            vx.Normalize();
            vy.Normalize();
            vz.Normalize();
            m.M11 = vx.X;
            m.M12 = vx.Y;
            m.M13 = vx.Z;
            m.M21 = vy.X;
            m.M22 = vy.Y;
            m.M23 = vy.Z;
            m.M31 = vz.X;
            m.M32 = vz.Y;
            m.M33 = vz.Z;
            m.M41 = 0;
            m.M42 = 0;
            m.M43 = 0;
            return vt;
        }

        /// <summary>
        /// �g��k���x�N�g���Ɖ�]quaternion�ƈʒu�x�N�g���ɕ������܂��B
        /// </summary>
        /// <param name="m">���̍s��i�߂�l�͉�]�s��j</param>
        /// <param name="scaling">�g��k���x�N�g��</param>
        /// <param name="rotation">��]quaternion</param>
        /// <returns>�ʒu�x�N�g��</returns>
        public static Vector3 DecomposeMatrix(ref Matrix m, out Vector3 scaling, out Quaternion rotation)
        {
            Vector3 translation = DecomposeMatrix(ref m, out scaling);
            rotation = Quaternion.RotationMatrix(m);
            return translation;
        }

        /// <summary>
        /// �����Z���s���܂��B
        /// </summary>
        /// <param name="mat0">�s��0</param>
        /// <param name="mat1">�s��1</param>
        /// <param name="mat2">�s��2</param>
        /// <returns>�s��1 - �s��2 + �s��0</returns>
        public static TMOMat AddSub(TMOMat mat0, TMOMat mat1, TMOMat mat2)
        {
            Matrix m0 = mat0.m;
            Matrix m1 = mat1.m;
            Matrix m2 = mat2.m;
            Vector3 t0 = DecomposeMatrix(ref m0);
            Vector3 t1 = DecomposeMatrix(ref m1);
            Vector3 t2 = DecomposeMatrix(ref m2);
            Matrix m = m1 * Matrix.Invert(m2) * m0;
            Vector3 t = t1 - t2 + t0;
            return new TMOMat(m * Matrix.Translation(t));
        }
    }

    /// <summary>
    /// �t���[���������܂��B
    /// </summary>
    public class TMOFrame
    {
        internal int id;
        internal TMOMat[] matrices;

        /// <summary>
        /// �t���[�����Ԃ��܂��B
        /// </summary>
        /// <param name="frame0">�t���[��0</param>
        /// <param name="frame1">�t���[��1</param>
        /// <param name="frame2">�t���[��2</param>
        /// <param name="frame3">�t���[��3</param>
        /// <param name="length">������</param>
        /// <param name="p1">��ԑ��x�W��</param>
        /// <param name="id_pair">node id�̃y�A</param>
        /// <returns></returns>
        public static TMOFrame[] Slerp(TMOFrame frame0, TMOFrame frame1, TMOFrame frame2, TMOFrame frame3, int length, float p1, int[] id_pair)
        {
            TMOFrame[] frames = new TMOFrame[length];

            for (int frame_index = 0; frame_index < length; frame_index++)
            {
                frames[frame_index] = new TMOFrame();
                frames[frame_index].matrices = new TMOMat[frame1.matrices.Length];
            }

            for (int i = 0; i < frame1.matrices.Length; i++)
            {
                TMOMat[] interpolated_matrices = TMOMat.Slerp(
                        frame0.matrices[i],
                        frame1.matrices[i],
                        frame2.matrices[id_pair[i]],
                        frame3.matrices[id_pair[i]],
                        length,
                        p1);

                for (int frame_index = 0; frame_index < length; frame_index++)
                    frames[frame_index].matrices[i] = interpolated_matrices[frame_index];
            }
            return frames;
        }

        /// <summary>
        /// frame1�̍s��ō\�����ꂽ�V����frame�𓾂܂��B
        /// �V����frame��frame0�Ɠ���node���тƂȂ�܂��B
        /// </summary>
        /// <param name="frame0"></param>
        /// <param name="frame1"></param>
        /// <param name="id_pair">node id�̃y�A</param>
        /// <returns>�V����frame</returns>
        public static TMOFrame Select(TMOFrame frame0, TMOFrame frame1, int[] id_pair)
        {
            TMOFrame ret = new TMOFrame();
            ret.matrices = new TMOMat[frame0.matrices.Length];
            for (int i = 0; i < frame0.matrices.Length; i++)
            {
                ret.matrices[i] = frame1.matrices[id_pair[i]];
            }
            return ret;
        }

        /// <summary>
        /// �����Z�̌��ʂƂ��ĐV����frame�𓾂܂��B
        /// �V����frame��frame0�Ɠ���node���тƂȂ�܂��B
        /// </summary>
        /// <param name="frame0">frame0</param>
        /// <param name="frame1">frame1</param>
        /// <param name="frame2">frame2</param>
        /// <param name="id_pair">node id�̃y�A</param>
        /// <returns>�V����frame</returns>
        public static TMOFrame AddSub(TMOFrame frame0, TMOFrame frame1, TMOFrame frame2, int[] id_pair)
        {
            TMOFrame ret = new TMOFrame();
            ret.matrices = new TMOMat[frame0.matrices.Length];
            for (int i = 0; i < frame0.matrices.Length; i++)
            {
                ret.matrices[i] = TMOMat.AddSub( frame0.matrices[i], frame1.matrices[id_pair[i]], frame2.matrices[id_pair[i]] );
            }
            return ret;
        }
    }

    /// <summary>
    /// bone�������܂��B
    /// </summary>
    public class TMONode
    {
        private int id;
        private string name;
        private string sname;

        private Quaternion rotation;
        private Vector3 translation;

        private Matrix transformation_matrix;
        private bool need_update_transformation;

        /// <summary>
        /// TMONode�𐶐����܂��B
        /// </summary>
        public TMONode(int id, string name)
        {
            this.id = id;
            this.name = name;
            this.sname = this.name.Substring(this.name.LastIndexOf('|') + 1);
        }

        /// <summary>
        /// ��]�ψ�
        /// </summary>
        public Quaternion Rotation
        {
            get {
                return rotation;
            }
            set {
                rotation = value;
                need_update_transformation = true;
            }
        }

        /// <summary>
        /// �ʒu�ψ�
        /// </summary>
        public Vector3 Translation
        {
            get {
                return translation;
            }
            set {
                translation = value;
                need_update_transformation = true;
            }
        }

        /// <summary>
        /// �qnode���X�g
        /// </summary>
        internal List<TMONode> child_nodes = new List<TMONode>();

        /// <summary>
        /// �enode
        /// </summary>
        internal TMONode parent;

        /// <summary>
        /// �s�񃊃X�g
        /// </summary>
        internal List<TMOMat> frame_matrices = new List<TMOMat>();

        /// <summary>
        /// ���[���h���W�n�ł̈ʒu�ƌ�����\���܂��B�����viewer����X�V����܂��B
        /// </summary>
        public Matrix combined_matrix;

        /// <summary>
        /// ID
        /// </summary>
        public int ID { get { return id; } }
        /// <summary>
        /// ����
        /// </summary>
        public string Name { get { return name; } }
        /// <summary>
        /// ���̂̒Z���`���B�����TMOFile���ŏd������\��������܂��B
        /// </summary>
        public string ShortName { get { return sname; } }

        /// <summary>
        /// �w�薼�́i�Z���`���j�����qnode���������܂��B
        /// </summary>
        /// <param name="sname">���́i�Z���`���j</param>
        /// <returns></returns>
        public TMONode FindChildByShortName(string sname)
        {
            foreach (TMONode child in child_nodes)
                if (child.sname == sname)
                    return child;
            return null;
        }

        /// <summary>
        /// �w��node����s��𕡎ʂ��܂��B
        /// </summary>
        /// <param name="motion">node</param>
        public void CopyThisMatFrom(TMONode motion)
        {
            //Console.WriteLine("copy mat {0} {1}", sname, motion.ShortName);
            int i = 0;
            foreach (TMOMat mat in frame_matrices)
            {
                mat.m = motion.frame_matrices[i % motion.frame_matrices.Count].m;
                i++;
            }
        }

        void CopyChildrenMatFrom_0(TMONode motion, List<string> except_snames)
        {
            List<TMONode> select_children = new List<TMONode>();
            foreach (TMONode child in child_nodes)
            {
                bool found = false;
                foreach (string except_sname in except_snames)
                {
                    if (child.sname == except_sname)
                    {
                        found = true;
                        break;
                    }
                }
                if (found)
                    except_snames.Remove(child.sname);
                else
                    select_children.Add(child);
            }
            foreach (TMONode child in select_children)
            {
                TMONode motion_child = motion.FindChildByShortName(child.sname);
                child.CopyThisMatFrom(motion_child);
                child.CopyChildrenMatFrom_0(motion_child, except_snames);
            }
        }

        /// <summary>
        /// �w��node����s��𕡎ʂ��܂��B
        /// ���������ʂ̑Ώۂ͎qnode�ȍ~�ł��B�w��node�͕��ʂ��܂���B
        /// �܂��A���Onode�ȍ~��node�͕��ʂ��܂���B
        /// </summary>
        /// <param name="motion">node</param>
        /// <param name="except_snames">���Onode���́i�Z���`���j���X�g</param>
        public void CopyChildrenMatFrom(TMONode motion, List<string> except_snames)
        {
            List<string> dup_except_snames = new List<string>();
            foreach (string except_sname in except_snames)
            {
                dup_except_snames.Add(except_sname);
            }
            CopyChildrenMatFrom_0(motion, dup_except_snames);
        }

        /// <summary>
        /// �w��node����s��𕡎ʂ��܂��B
        /// </summary>
        /// <param name="motion">node</param>
        public void CopyMatFrom(TMONode motion)
        {
            CopyThisMatFrom(motion);
            foreach (TMONode child in child_nodes)
            {
                child.CopyMatFrom(motion.FindChildByShortName(child.sname));
            }
        }

        /// <summary>
        /// �w��ψʂ����g�債�܂��B
        /// </summary>
        /// <param name="x">X���ψ�</param>
        /// <param name="y">Y���ψ�</param>
        /// <param name="z">Z���ψ�</param>
        public void Scale(float x, float y, float z)
        {
            Matrix scaling = Matrix.Scaling(x, y, z);

            foreach (TMOMat i in frame_matrices)
                i.Scale(scaling);
        }

        /// <summary>
        /// �w��ψʂ����k�����܂��B
        /// </summary>
        /// <param name="x">X���ψ�</param>
        /// <param name="y">Y���ψ�</param>
        /// <param name="z">Z���ψ�</param>
        public void Scale0(float x, float y, float z)
        {
            Matrix scaling = Matrix.Scaling(x, y, z);

            foreach (TMOMat i in frame_matrices)
                i.Scale0(scaling);
        }

        /// <summary>
        /// �w��ψʂ����g�債�܂��B����Ɋe�qnode���k�����܂��B
        /// </summary>
        /// <param name="x">X���ψ�</param>
        /// <param name="y">Y���ψ�</param>
        /// <param name="z">Z���ψ�</param>
        public void Scale1(float x, float y, float z)
        {
            Matrix scaling = Matrix.Scaling(x, y, z);

            foreach (TMOMat i in frame_matrices)
                i.Scale1(scaling);

            foreach (TMONode child in child_nodes)
                child.Scale0(x, y, z);
        }

        /// <summary>
        /// �w��p�x��X����]���܂��B
        /// </summary>
        /// <param name="angle">�p�x�i���W�A���j</param>
        public void RotateX(float angle)
        {
            foreach (TMOMat i in frame_matrices)
                i.RotateX(angle);
        }

        /// <summary>
        /// �w��p�x��Y����]���܂��B
        /// </summary>
        /// <param name="angle">�p�x�i���W�A���j</param>
        public void RotateY(float angle)
        {
            foreach (TMOMat i in frame_matrices)
                i.RotateY(angle);
        }

        /// <summary>
        /// �w��p�x��Z����]���܂��B
        /// </summary>
        /// <param name="angle">�p�x�i���W�A���j</param>
        public void RotateZ(float angle)
        {
            foreach (TMOMat i in frame_matrices)
                i.RotateZ(angle);
        }

        /// <summary>
        /// ���[���h���W�n�ɂ����Ďw��p�x��Y����]���܂��B
        /// </summary>
        /// <param name="angle">�p�x�i���W�A���j</param>
        public void RotateWorldY(float angle)
        {
            foreach (TMOMat i in frame_matrices)
                i.RotateWorldY(angle);
        }

        /// <summary>
        /// �w��ψʂ����ړ����܂��B
        /// </summary>
        /// <param name="x">X���ψ�</param>
        /// <param name="y">Y���ψ�</param>
        /// <param name="z">Z���ψ�</param>
        public void Move(float x, float y, float z)
        {
            Vector3 translation = new Vector3(x, y, z);

            foreach (TMOMat i in frame_matrices)
                i.Move(translation);
        }

        /// <summary>
        /// ���[���h���W�n�ł̈ʒu�𓾂܂��B
        /// </summary>
        /// <returns></returns>
        public Vector3 GetWorldPosition()
        {
            TMONode bone = this;
            Vector3 v = Vector3.Empty;
            while (bone != null)
            {
                v = Vector3.TransformCoordinate(v, bone.TransformationMatrix);
                bone = bone.parent;
            }
            return v;
        }

        /// <summary>
        /// ���[���h���W�n�ł̈ʒu�ƌ����𓾂܂��B
        /// </summary>
        /// <returns></returns>
        public Matrix GetWorldCoordinate()
        {
            TMONode bone = this;
            Matrix m = Matrix.Identity;
            while (bone != null)
            {
                m.Multiply(bone.TransformationMatrix);
                bone = bone.parent;
            }
            return m;
        }

        /// <summary>
        /// ��]�s��
        /// </summary>
        public Matrix RotationMatrix
        {
            get {
                return Matrix.RotationQuaternion(rotation);
            }
        }

        /// <summary>
        /// �ʒu�s��
        /// </summary>
        public Matrix TranslationMatrix
        {
            get {
                return Matrix.Translation(translation);
            }
        }

        /// <summary>
        /// �ό`�s��B����� ��]�s�� x �ʒu�s�� �ł��B
        /// </summary>
        public Matrix TransformationMatrix
        {
            get {
                if (need_update_transformation)
                {
                    transformation_matrix = RotationMatrix * TranslationMatrix;
                    need_update_transformation = false;
                }
                return transformation_matrix;
            }
            set {
                transformation_matrix = value;
                Matrix m = transformation_matrix;
                translation = TMOMat.DecomposeMatrix(ref m);
                rotation = Quaternion.RotationMatrix(m);
            }
        }
    }
}
