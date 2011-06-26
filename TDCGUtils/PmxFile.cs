using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using Tso2Pmd.Extensions;

namespace TDCGUtils
{
    public class PmxFile
    {
        public PMD_Vertex[] vertices;
        public int[] vindices;
        public PMD_Material[] materials;
        public PMD_Bone[] nodes;
        public PMD_IK[] iks;
        public PMD_Skin[] skins;

        public PMD_RBody[] rbodies;
        public PMD_Joint[] joints;

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
            foreach (PMD_Vertex v in vertices)
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
            foreach (PMD_Material m in materials)
            {
                m.Write(bw);
            }

            bw.Write(nodes.Length);
            foreach (PMD_Bone n in nodes)
            {
                n.Write(bw);
            }

            bw.Write(skins.Length);
            foreach (PMD_Skin s in skins)
            {
                s.Write(bw);
            }

            bw.Write((int)0);//#labels

            bw.Write(rbodies.Length);
            foreach (PMD_RBody b in rbodies)
            {
                b.Write(bw);
            }

            bw.Write(joints.Length);
            foreach (PMD_Joint j in joints)
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
    public class PMD_SkinWeight
    {
        public short bone_index;
        public float weight;

        /// <summary>
        /// スキンウェイトを生成します。
        /// </summary>
        /// <param name="bone_index">ボーンインデックス</param>
        /// <param name="weight">ウェイト</param>
        public PMD_SkinWeight(short bone_index, float weight)
        {
            this.bone_index = bone_index;
            this.weight = weight;
        }
    }

    /// 頂点
    public class PMD_Vertex
    {
        public int id;

        public Vector3 position;
        public Vector3 normal;
        public float u;
        public float v;
        public PMD_SkinWeight[] skin_weights;
        public float edge_scale;

        public PMD_Vertex()
        {
            skin_weights = new PMD_SkinWeight[4];
            for (int i = 0; i < 4; i++)
            {
                skin_weights[i] = new PMD_SkinWeight(0, 0.0f);
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
    public class PMD_Material
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

        public PMD_Material()
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
    public class PMD_Bone
    {
        public short id;

        public string name_ja;
        public string name_en;
        public Vector3 position;
        public short parent_node_id;
        public int calc_order;
        public byte flags_hi;
        public byte flags_lo;
        public short tail_node_id;

        public PMD_Bone(short id)
        {
            this.id = id;

            name_ja = "node ja";
            name_en = "node en";
            position = new Vector3(0, 5, 0);
            parent_node_id = -1;
            calc_order = 0;//変形階層
            flags_hi = (byte)0x00;//上位フラグ
            flags_lo = (byte)0x1F;//下位フラグ 0x01: 接続先 1:ボーンで指定
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

        public string ParentName
        {
            get { return null; }
        }

        public string TailName
        {
            get { return null; }
        }
    }

    /// IK
    public class PMD_IK
    {
        public short id;

        public string name_ja;
        public string name_en;
        public Vector3 position;
        public short parent_node_id;
        public int calc_order;
        public byte flags_hi;
        public byte flags_lo;
        public short tail_node_id;

        public int target_node_id;
        public int loop_count;
        public float cons_angle;

        public PMD_IL[] links;

        public PMD_IK(short id)
        {
            this.id = id;

            name_ja = "node ja";
            name_en = "node en";
            position = new Vector3(0, 5, 0);
            parent_node_id = -1;
            calc_order = 1;//変形階層
            flags_hi = (byte)0x00;//上位フラグ
            flags_lo = (byte)0x3F;//下位フラグ 0x01: 接続先 1:ボーンで指定
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

            bw.Write(target_node_id);
            bw.Write(loop_count);
            bw.Write(cons_angle);

            bw.Write(links.Length);
            foreach (PMD_IL l in links)
            {
                bw.Write(l.node_id);
                bw.Write(l.cons);
            }
        }
    }

    public class PMD_IL
    {
        public short node_id;
        public byte cons = 0;
    }

    /// 表情
    public class PMD_Skin
    {
        public sbyte id;

        public string name_ja;
        public string name_en;

        public byte panel_id;
        public byte type = 1;//モーフ種類 1:頂点

        public PMD_SkinVertex[] vertices;

        public PMD_Skin(sbyte id)
        {
            this.id = id;
        }

        public void Write(BinaryWriter bw)
        {
            bw.WritePString(name_ja);
            bw.WritePString(name_en);

            bw.Write(panel_id);
            bw.Write(type);

            bw.Write(vertices.Length);
            foreach (PMD_SkinVertex v in vertices)
            {
                bw.Write(v.vertex_id);
                bw.Write(ref v.position);
            }
        }
    }

    /// 表情頂点
    public class PMD_SkinVertex
    {
        public int vertex_id;
        public Vector3 position;
    }

    /// 剛体
    public class PMD_RBody
    {
        public sbyte id;

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
    public class PMD_Joint
    {
        public string name_ja;
        public string name_en;
        
        public byte type = 0;

        public sbyte rbody_a_id;
        public sbyte rbody_b_id;
        
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
