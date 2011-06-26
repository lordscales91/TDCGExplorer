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
        public PmxNode[] nodes;

        public PmxRBody[] rbodies;
        public PmxJoint[] joints;

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

            bw.Write(nodes.Length);
            foreach (PmxNode n in nodes)
            {
                n.Write(bw);
            }

            bw.Write((int)0);//#morphs
            bw.Write((int)0);//#labels

            bw.Write(rbodies.Length);
            foreach (PmxRBody b in rbodies)
            {
                b.Write(bw);
            }

            bw.Write(joints.Length);
            foreach (PmxJoint j in joints)
            {
                j.Write(bw);
            }

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
        public int id;

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
        public short id;

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
        public int vindices_count;

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
            bw.Write(vindices_count);
        }
    }

    /// ボーン
    public class PmxNode
    {
        public short id;

        string name_ja;
        string name_en;
        Vector3 position;
        short parent_node_id;
        int calc_order;
        byte flags_hi;
        byte flags_lo;
        short tail_node_id;

        public PmxNode()
        {
            name_ja = "node ja";
            name_en = "node en";
            position = new Vector3(0, 5, 0);
            parent_node_id = -1;
            calc_order = 0;//変形階層
            flags_hi = (byte)0x00;//上位フラグ
            flags_lo = (byte)0x09;//下位フラグ 0x01: 接続先 1:ボーンで指定 0x08: 表示
            tail_node_id = -1;
        }

        public void Write(BinaryWriter bw)
        {
            bw.WritePString(name_ja);
            bw.WritePString(name_en);
            bw.Write(ref position);
            bw.Write(parent_node_id);
            bw.Write(calc_order);
            bw.Write(flags_lo);
            bw.Write(flags_hi);
            bw.Write(tail_node_id);
        }
    }

    /// 剛体
    public class PmxRBody
    {
        public short id;

        public string name_ja;
        public string name_en;

        public short rel_bone_id;
        
        public byte group_id;
        public ushort group_non_collision;
        
        public byte shape_id;
        public Vector3 size;

        public Vector3 position;
        public Vector3 rotation;
        
        public float weight;
        public float position_dim;
        public float rotation_dim;
        public float recoil;
        public float friction;
        
        public byte type;

        public void Write(BinaryWriter bw)
        {
            bw.WritePString(name_ja);
            bw.WritePString(name_en);

            bw.Write(rel_bone_id);

            bw.Write(group_id);
            bw.Write(group_non_collision);

            bw.Write(shape_id);
            bw.Write(ref size);

            bw.Write(ref position);
            bw.Write(ref rotation);

            bw.Write(weight);
            bw.Write(position_dim);
            bw.Write(rotation_dim);
            bw.Write(recoil);
            bw.Write(friction);

            bw.Write(type);
        }
    }

    /// ジョイント
    public class PmxJoint
    {
        public string name_ja;
        public string name_en;
        
        public byte type = 0;
        
        public short rbody_a_id;
        public short rbody_b_id;
        
        public Vector3 position;
        public Vector3 rotation;

        public Vector3 position_min;
        public Vector3 position_max;
        public Vector3 rotation_min;
        public Vector3 rotation_max;
        
        public Vector3 spring_position;
        public Vector3 spring_rotation;

        public void Write(BinaryWriter bw)
        {
            bw.WritePString(name_ja);
            bw.WritePString(name_en);

            bw.Write(type);

            bw.Write(rbody_a_id);
            bw.Write(rbody_b_id);

            bw.Write(ref position);
            bw.Write(ref rotation);

            bw.Write(ref position_min);
            bw.Write(ref position_max);
            bw.Write(ref rotation_min);
            bw.Write(ref rotation_max);

            bw.Write(ref spring_position);
            bw.Write(ref spring_rotation);
        }
    }
}
