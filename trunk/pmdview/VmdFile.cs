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

        public Quaternion rotation = Quaternion.Identity;
        public Vector3 translation = Vector3.Empty;

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
    }

    /// <summary>
    /// フレームを扱います。
    /// </summary>
    public class VmdFrame
    {
        public int id;

        /// <summary>
        /// 行列の配列
        /// </summary>
        public VmdMat[] matrices;
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

            //TODO: nodes
            //TODO: frames

            Dictionary<string, VmdNode> nmap = new Dictionary<string, VmdNode>();
            List<VmdNode> nary = new List<VmdNode>();
            int current_frame_index = 0;
            for (int i = 0; i < frame_count; i++)
            {
                string node_name = reader.ReadCString(15);
                Debug.WriteLine("node_name:" + node_name);

                int frame_index = reader.ReadInt32();
                Debug.WriteLine("frame_index:" + frame_index);

                if (frame_index != current_frame_index)
                    break;

                Vector3 translation = Vector3.Empty;
                reader.ReadVector3(ref translation);

                Quaternion rotation = Quaternion.Identity;
                reader.ReadQuaternion(ref rotation);

                byte[] bezier = reader.ReadBytes(64);

                if (! nmap.ContainsKey(node_name))
                {
                    VmdNode node = new VmdNode((ushort)nary.Count);
                    node.name = node_name;
                    node.translation = translation;
                    node.rotation = rotation;
                    nary.Add(node);
                    nmap[node_name] = node;
                }
            }
            nodes = nary.ToArray();

            GenerateNodemapAndTree();
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
