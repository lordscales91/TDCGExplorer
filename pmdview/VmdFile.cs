using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace pmdview
{
    /// <summary>
    /// boneを扱います。
    /// </summary>
    public class VmdNode
    {
        public ushort id;
        public string name;
        public ushort parent_node_id;

        /// <summary>
        /// 行列の配列
        /// </summary>
        public VmdMat[] matrices;

        public List<VmdNode> children = new List<VmdNode>();
        public VmdNode parent;

        /// <summary>
        /// VmdNodeを生成します。
        /// </summary>
        public VmdNode(ushort id)
        {
            this.id = id;
        }
    }

    /// <summary>
    /// 変形行列を扱います。
    /// </summary>
    public class VmdMat
    {
        public Quaternion rotation = Quaternion.Identity;
        public Vector3 translation = Vector3.Empty;
    }

    /// <summary>
    /// フレームを扱います。
    /// </summary>
    public class VmdFrame : IComparable
    {
        public int index;
        public Quaternion rotation = Quaternion.Identity;
        public Vector3 translation = Vector3.Empty;

        public VmdFrame(int index, Quaternion q, Vector3 v)
        {
            this.index = index;
            rotation = q;
            translation = v;
        }

        public int CompareTo(object obj)
        {
            if (obj is VmdFrame)
            {
                VmdFrame other = (VmdFrame)obj;
                return index.CompareTo(other.index);
            }
            else
            {
               throw new ArgumentException("Object is not a VmdFrame");
            }    
        }
    }

    /// <summary>
    /// vmdファイルを扱います。
    /// </summary>
    public class VmdFile
    {
        /// <summary>
        /// bone配列
        /// </summary>
        public VmdNode[] nodes;
        /// <summary>
        /// フレーム配列
        /// </summary>
        public VmdFrame[] frames;

        public void Load(string source_file)
        {
            using (Stream source_stream = File.OpenRead(source_file))
                Load(source_stream);
        }

        public void Load(Stream source_stream)
        {
            BinaryReader reader = new BinaryReader(source_stream, System.Text.Encoding.Default);

            string caption = reader.ReadCString(30);
            Debug.WriteLine("caption:" + caption);

            string model_name = reader.ReadCString(20);
            Debug.WriteLine("model_name:" + model_name);

            int frame_count = reader.ReadInt32();
            Debug.WriteLine("frame_count:" + frame_count);

            Dictionary<string, VmdNode> nmap = new Dictionary<string, VmdNode>();
            List<VmdNode> nary = new List<VmdNode>();
            Dictionary<VmdNode, List<VmdFrame>> fmap = new Dictionary<VmdNode, List<VmdFrame>>();
            int max_frame_index = 0;
            for (int i = 0; i < frame_count; i++)
            {
                string node_name = reader.ReadCString(15);
                Debug.WriteLine("node_name:" + node_name);

                int frame_index = reader.ReadInt32();
                Debug.WriteLine("frame_index:" + frame_index);
                if (max_frame_index < frame_index)
                    max_frame_index = frame_index;

                Vector3 translation = Vector3.Empty;
                reader.ReadVector3(ref translation);

                Quaternion rotation = Quaternion.Identity;
                reader.ReadQuaternion(ref rotation);

                byte[] bezier = reader.ReadBytes(64);

                VmdNode node;
                if (! nmap.ContainsKey(node_name))
                {
                    node = new VmdNode((ushort)nary.Count);
                    node.name = node_name;
                    nary.Add(node);
                    nmap[node_name] = node;
                }
                else
                    node = nmap[node_name];

                if (! fmap.ContainsKey(node))
                    fmap[node] = new List<VmdFrame>();

                fmap[node].Add(new VmdFrame(frame_index, rotation, translation));
            }
            frame_length = max_frame_index + 1;
            nodes = nary.ToArray();
            foreach (VmdNode node in nodes)
            {
                node.matrices = CreateMatrices(fmap[node]);
            }

            GenerateNodemapAndTree();
        }

        VmdMat[] CreateMatrices(List<VmdFrame> frames)
        {
            frames.Sort();

            VmdMat[] matrices = new VmdMat[frame_length];
            for (int i = 1; i < frames.Count; i++)
            {
                VmdFrame fa = frames[i - 1];
                VmdFrame fb = frames[i];
                for (int index = fa.index; index < fb.index; index++)
                {
                    float ratio = ((float)(index - fa.index)) / ((float)(fb.index - fa.index));
                    VmdMat mat = new VmdMat();
                    mat.rotation = Quaternion.Slerp(fa.rotation, fb.rotation, ratio);
                    mat.translation.X = Lerp(fa.translation.X, fb.translation.X, ratio);
                    mat.translation.Y = Lerp(fa.translation.Y, fb.translation.Y, ratio);
                    mat.translation.Z = Lerp(fa.translation.Z, fb.translation.Z, ratio);
                    matrices[index] = mat;
                }
                {
                    VmdMat mat = new VmdMat();
                    mat.rotation = fb.rotation;
                    mat.translation = fb.translation;
                    matrices[fb.index] = mat;
                }
            }
            {
                VmdFrame last_frame = frames[frames.Count - 1];
                for (int index = last_frame.index; index < frame_length; index++)
                {
                    VmdMat mat = new VmdMat();
                    mat.rotation = last_frame.rotation;
                    mat.translation = last_frame.translation;
                    matrices[index] = mat;
                }
            }
            return matrices;
        }

        public static float Lerp(float value1, float value2, float amount)
        {
            return value1 + (value2 - value1) * amount;
        }

        int frame_length = 0;

        /// フレーム長さ
        public int FrameLength
        {
            get { return frame_length; }
        }

        public Dictionary<string, VmdNode> nodemap = new Dictionary<string, VmdNode>();

        public List<VmdNode> root_nodes = new List<VmdNode>();

        public void GenerateNodemapAndTree()
        {
            nodemap.Clear();
            foreach (VmdNode node in nodes)
                nodemap[node.name] = node;

            foreach (VmdNode node in nodes)
            {
                node.parent = null;
                node.children.Clear();
            }
            root_nodes.Clear();
            foreach (VmdNode node in nodes)
            {
                if (node.parent_node_id == ushort.MaxValue)
                    root_nodes.Add(node);
                if (node.parent_node_id == ushort.MaxValue)
                    continue;
                node.parent = nodes[node.parent_node_id];
                node.parent.children.Add(node);
            }
        }
    }
}
