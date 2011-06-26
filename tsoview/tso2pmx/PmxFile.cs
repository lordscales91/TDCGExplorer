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
        public PmxVertex[] vertices;
        public int[] vindices;
        public PmxMaterial[] materials;

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

            bw.Write((int)1);
            bw.WritePString("chip0.bmp");

            bw.Write(materials.Length);
            foreach (PmxMaterial m in materials)
            {
                m.Write(bw);
            }

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

    /// 材質
    public class PmxMaterial
    {
        public string name_ja;
        public string name_en;
        
        public Vector4 diffuse;
        public Vector4 specular;
        public Vector3 ambient;
        
        public byte flags;
        
        public Vector4 edge_color;
        public float edge_width;
        
        public sbyte tex_id = -1;
        public sbyte sphere_tex_id = -1;
        public byte sphere_mode = 0;
        public byte toon_flag = 0;
        public sbyte toon_tex_id = -1;
        
        public string memo;
        public int vertices_count;

        public PmxMaterial()
        {
            name_ja = "material ja";
            name_en = "material en";
            diffuse = new Vector4(0.800f, 0.712f, 0.624f, 1.0f);
            specular = new Vector4(0.150f, 0.150f, 0.150f, 6.0f);
            ambient = new Vector3(0.500f, 0.445f, 0.390f);
            flags = (byte)0x10;//描画フラグ 0x10:エッジ描画
            edge_color = new Vector4(0.0f, 0.0f, 0.0f, 1.0f);
            edge_width = 1.0f;

            memo = "memo";
        }

        public void Write(BinaryWriter bw)
        {
            bw.WritePString(name_ja);
            bw.WritePString(name_en);
            bw.Write(ref diffuse);
            bw.Write(ref specular);
            bw.Write(ref ambient);
            bw.Write(flags);
            bw.Write(ref edge_color);
            bw.Write(edge_width);
            bw.Write(tex_id);
            bw.Write(sphere_tex_id);
            bw.Write(sphere_mode);
            bw.Write(toon_flag);
            bw.Write(toon_tex_id);
            bw.WritePString(memo);
            bw.Write(vertices_count);
        }
    }
}
