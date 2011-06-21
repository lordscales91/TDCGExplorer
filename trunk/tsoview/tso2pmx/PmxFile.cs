using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using tso2pmx.Extensions;

namespace tso2pmx
{
    class PmxFile
    {
        PmxVertex[] vertices;
        int[] vindices;

        /// <summary>
        /// 指定パスに保存します。
        /// </summary>
        /// <param name="dest_file">パス</param>
        public void Save(string dest_file)
        {
            using (Stream dest_stream = File.Create(dest_file))
                Save(dest_stream);
        }

        /// <summary>
        /// 指定ストリームに保存します。
        /// </summary>
        /// <param name="dest_stream">ストリーム</param>
        public void Save(Stream dest_stream)
        {
            BinaryWriter bw = new BinaryWriter(dest_stream);

            WriteMagic(bw);

            bw.Write((byte)8);
            bw.Write((byte)1); //文字エンコード方式
            bw.Write((byte)0); //追加UV数
            bw.Write((byte)4); //頂点Indexサイズ
            bw.Write((byte)1); //テクスチャIndexサイズ
            bw.Write((byte)1); //材質Indexサイズ
            bw.Write((byte)2); //ボーンIndexサイズ
            bw.Write((byte)1); //モーフIndexサイズ
            bw.Write((byte)1); //剛体Indexサイズ

            bw.WritePString("model name ja");
            bw.WritePString("model name en");
            bw.WritePString("comment ja");
            bw.WritePString("comment en");

            bw.Write(vertices.Length);
            foreach (PmxVertex v in vertices)
            {
                v.Write(bw);
            }
            bw.Write(vindices.Length);
            foreach (int i in vindices)
            {
                bw.Write(i);
            }

            bw.Write((int)0);//#textures
            bw.Write((int)0);//#materials
            bw.Write((int)0);//#nodes
            bw.Write((int)0);//#morphs
            bw.Write((int)0);//#labels
            bw.Write((int)0);//#rbodies
            bw.Write((int)0);//#joints
        }

        /// <summary>
        /// 'PMX ' とver (2.0)を書き出します。
        /// </summary>
        public static void WriteMagic(BinaryWriter bw)
        {
            bw.Write((byte)0x50);
            bw.Write((byte)0x4d);
            bw.Write((byte)0x58);
            bw.Write((byte)0x20);
            bw.Write(2.0f);
        }

        public PmxFile()
        {
            vertices = new PmxVertex[3];
            vertices[0] = new PmxVertex();
            vertices[0].position = new Vector3(0, 0, 0);
            vertices[1] = new PmxVertex();
            vertices[1].position = new Vector3(5, 0, 0);
            vertices[2] = new PmxVertex();
            vertices[2].position = new Vector3(0, 5, 0);
            vindices = new int[3];
            vindices[0] = 0;
            vindices[1] = 1;
            vindices[2] = 2;
        }
    }

    /// スキンウェイト
    public class PmxSkinWeight
    {
        public short bone_index;
        public float weight;

        /// <summary>
        /// スキンウェイトを生成します。
        /// </summary>
        /// <param name="bone_index">ボーンインデックス</param>
        /// <param name="weight">ウェイト</param>
        public PmxSkinWeight(short bone_index, float weight)
        {
            this.bone_index = bone_index;
            this.weight = weight;
        }
    }

    /// 頂点
    public class PmxVertex
    {
        public Vector3 position;
        public Vector3 normal;
        public float u;
        public float v;
        public PmxSkinWeight[] skin_weights;
        public float edge_scale;

        public PmxVertex()
        {
            skin_weights = new PmxSkinWeight[4];
            for (int i = 0; i < 4; i++)
            {
                skin_weights[i] = new PmxSkinWeight(0, 0.0f);
            }
            edge_scale = 1.0f;
        }

        /// <summary>
        /// 頂点を書き出します。
        /// </summary>
        public void Write(BinaryWriter bw)
        {
            bw.Write(ref this.position);
            bw.Write(ref this.normal);
            bw.Write(this.u);
            bw.Write(this.v);
            bw.Write((byte)2); //変形方式 2:BDEF4
            for (int i = 0; i < 4; i++)
            {
                bw.Write(skin_weights[i].bone_index);
            }
            for (int i = 0; i < 4; i++)
            {
                bw.Write(skin_weights[i].weight);
            }
            bw.Write(this.edge_scale);
        }
    }
}
