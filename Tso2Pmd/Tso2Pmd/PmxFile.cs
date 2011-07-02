using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using TDCGUtils.Extensions;

namespace Tso2Pmd
{
    public class PmxFile
    {
        public PMD_Header header;
        public PMD_Vertex[] vertices;
        public int[] vindices;
        public string[] texture_file_names;
        public PMD_Material[] materials;
        public PMD_Bone[] nodes;
        public PMD_IK[] iks;
        public PMD_Skin[] skins;
        public List<PMD_DispGroup> disp_groups = new List<PMD_DispGroup>();
        public PMD_RBody[] bodies;
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

            header.Write(bw);

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

            bw.Write(texture_file_names.Length);
            foreach (string file_name in texture_file_names)
            {
                bw.WritePString(file_name);
            }

            bw.Write(materials.Length);
            foreach (PMD_Material material in materials)
            {
                material.Write(bw);
            }

            foreach (PMD_IK ik in iks)
            {
                ik.SetBoneIDFromName(this);
            }

            foreach (PMD_Bone node in nodes)
            {
                node.SetBoneIDFromName(this);
            }

            // IKを親としている系列の変形階層を1にする
            foreach (PMD_Bone node in nodes)
            {
                if (node.calc_order == 1 && node.tail_node_id != -1)
                    nodes[node.tail_node_id].calc_order = 1;
            }

            bw.Write(nodes.Length);
            foreach (PMD_Bone node in nodes)
            {
                node.Write(bw);
            }

            bw.Write(skins.Length);
            foreach (PMD_Skin skin in skins)
            {
                skin.Write(bw);
            }

            bw.Write(disp_groups.Count);
            foreach (PMD_DispGroup disp_group in disp_groups)
            {
                foreach (PMD_Disp disp in disp_group.disps)
                {
                    disp.SetBoneIDFromName(this);
                }
                disp_group.Write(bw);
            }

            bw.Write(bodies.Length);
            foreach (PMD_RBody rbody in bodies)
            {
                rbody.Write(bw);
            }

            bw.Write(joints.Length);
            foreach (PMD_Joint joint in joints)
            {
                joint.Write(bw);
            }

        }

        public PMD_Bone GetBoneByName(string name)
        {
            foreach (PMD_Bone bone in nodes)
            {
                if (bone.name == name)
                    return bone;
            }
            return null;
        }

        public short GetBoneIDByName(string name)
        {
            if (name != null)
                for (short i = 0; i < nodes.Length; i++)
                {
                    if (nodes[i].name == name)
                        return i;
                }
            return -1;
        }
    }

    public class PMD_Header
    {
        float version = 2.0f;
        public String name;
        public String comment;

        internal void Write(BinaryWriter bw)
        {
            bw.Write((byte)0x50);
            bw.Write((byte)0x4d);
            bw.Write((byte)0x58);
            bw.Write((byte)0x20);
            bw.Write(version);

            bw.Write((byte)8);
            bw.Write((byte)0); //文字エンコード方式
            bw.Write((byte)0); //追加UV数
            bw.Write((byte)4); //頂点Indexサイズ
            bw.Write((byte)1); //テクスチャIndexサイズ
            bw.Write((byte)1); //材質Indexサイズ
            bw.Write((byte)2); //ボーンIndexサイズ
            bw.Write((byte)1); //モーフIndexサイズ
            bw.Write((byte)1); //剛体Indexサイズ

            bw.WritePString(name);
            bw.WritePString("name en");
            bw.WritePString(comment);
            bw.WritePString("comment en");
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
            bw.Write(ref position);
            bw.Write(ref normal);
            bw.Write(u);
            bw.Write(v);
            bw.Write((byte)2);//ウェイト変形方式 0:BDEF1 1:BDEF2 2:BDEF4 3:SDEF
            for (int i = 0; i < 4; i++)
            {
                bw.Write(skin_weights[i].bone_index);
            }
            for (int i = 0; i < 4; i++)
            {
                bw.Write(skin_weights[i].weight);
            }
            bw.Write(edge_scale);
        }
    }

    /// 材質
    public class PMD_Material
    {
        public short id;

        public string name;
        public string name_en;
        
        public Vector4 diffuse;
        public Vector4 specular;
        public Vector3 ambient;

        //描画フラグ
        //  0x01:両面描画
        //  0x02:地面影
        //  0x04:セルフシャドウマップへの描画
        //  0x08:セルフシャドウの描画
        //  0x10:エッジ描画
        [Flags]
        public enum Flags : byte
        {
            none = 0x00,
            both = 0x01,
            shadow = 0x02,
            selfshadowmap = 0x04,
            selfshadow = 0x08,
            edge = 0x10
        }
        public Flags flags;
        
        public Vector4 edge_color;
        public float edge_width;
        
        public sbyte tex_id = -1;
        public sbyte tex_sphere_id = -1;
        public byte sphere_mode = 0;
        public byte shared_toon = 0;
        public sbyte tex_toon_id = -1;
        
        public string memo;
        public int vindices_count;

        public PMD_Material()
        {
            name = "material ja";
            name_en = "material en";
            diffuse = new Vector4(0.800f, 0.712f, 0.624f, 1.0f);
            specular = new Vector4(0.150f, 0.150f, 0.150f, 6.0f);
            ambient = new Vector3(0.500f, 0.445f, 0.390f);
            flags = Flags.selfshadowmap | Flags.selfshadow;
            edge_color = new Vector4(0.0f, 0.0f, 0.0f, 1.0f);
            edge_width = 1.0f;

            memo = "memo";
        }

        public void Write(BinaryWriter bw)
        {
            bw.WritePString(name);
            bw.WritePString(name_en);
            bw.Write(ref diffuse);
            bw.Write(ref specular);
            bw.Write(ref ambient);
            bw.Write((byte)flags);
            bw.Write(ref edge_color);
            bw.Write(edge_width);
            bw.Write(tex_id);
            bw.Write(tex_sphere_id);
            bw.Write(sphere_mode);
            bw.Write(shared_toon);
            bw.Write(tex_toon_id);
            bw.WritePString(memo);
            bw.Write(vindices_count);
        }

        public bool UseEdge
        {
            set
            {
                if (value)
                    flags |=  PMD_Material.Flags.edge;
                else
                    flags &= ~PMD_Material.Flags.edge;
            }
        }

        public bool UseShadow
        {
            set
            {
                if (value)
                    flags |=  PMD_Material.Flags.shadow;
                else
                    flags &= ~PMD_Material.Flags.shadow;
            }
        }
    }

    /// ボーン
    public class PMD_Bone
    {
        public string name;
        public string name_en;
        public Vector3 position;
        public short parent_node_id;
        public int calc_order;
        byte flags_hi;
        byte flags_lo;
        public short tail_node_id;
        public short target_node_id;

        PMD_IK ik = null;

        public PMD_IK IK
        {
            get { return ik; }
            set
            {
                ik = value;

                if (ik != null)
                {
                    calc_order = 1;
                    flags_lo |= 0x20; // 0x20: IK
                }
            }
        }

        public PMD_Bone()
        {
            name = "node ja";
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
            bw.WritePString(name);
            bw.WritePString(name_en);
            bw.Write(ref position);
            bw.Write(parent_node_id);
            bw.Write(calc_order);
            bw.Write(flags_lo);
            bw.Write(flags_hi);
            bw.Write(tail_node_id);

            if (flags_hi == 0x01)
            {
                bw.Write(target_node_id);
                bw.Write(1.0f);
            }

            if (ik != null)
            {
                ik.Write(bw);
            }
        }

        // ボーンの種類 0:回転 1:回転と移動 2:IK 3:不明 4:IK影響下 5:回転影響下 6:IK接続先 7:非表示 8:捻り 9:回転運動
        public int kind
        {
            get
            {
                switch (flags_hi)
                {
                    case 0x01:
                        return 5;
                    default:
                        switch (flags_lo)
                        {
                            case 0x1B:
                                return 0;
                            case 0x1F:
                                return 1;
                            case 0x11:
                                return 7;
                            default:
                                return 0;
                        }
                }
            }
            set
            {
                switch (value)
                {
                    case 0:
                        flags_hi = (byte)0x00;
                        flags_lo = (byte)0x1B;
                        break;
                    case 1:
                        flags_hi = (byte)0x00;
                        flags_lo = (byte)0x1F;
                        break;
                    case 5:
                        flags_hi = (byte)0x01;
                        flags_lo = (byte)0x1B;
                        break;
                    case 7:
                        flags_hi = (byte)0x00;
                        flags_lo = (byte)0x11;
                        break;
                    default:
                        flags_hi = (byte)0x00;
                        flags_lo = (byte)0x1F;
                        break;
                }
            }
        }

        // 親ボーン名
        public string ParentName;

        // 子ボーン名
        public string TailName;

        // IKターゲットボーン名
        public string TargetName;

        // ボーン名をIDに置き換える
        public void SetBoneIDFromName(PmxFile pmd)
        {
            parent_node_id = pmd.GetBoneIDByName(ParentName);
            tail_node_id = pmd.GetBoneIDByName(TailName);
            target_node_id = pmd.GetBoneIDByName(TargetName);
        }
    }

    /// IK
    public class PMD_IK
    {
        public short target_node_id;
        public int niteration;
        public float weight;

        public List<PMD_IKNode> links = new List<PMD_IKNode>();

        public void Write(BinaryWriter bw)
        {
            bw.Write(target_node_id);
            bw.Write(niteration);
            bw.Write(weight * 4);

            bw.Write(links.Count);
            foreach (PMD_IKNode link in links)
            {
                link.Write(bw);
            }
        }

        // IK先端ボーン名
        public string effector_node_name;

        // IKターゲットボーン名
        public string target_node_name;

        // IKを構成するボーンの配列
        public List<string> chain_node_names = new List<string>();

        // ボーン名をIDに置き換える
        public void SetBoneIDFromName(PmxFile pmd)
        {
            pmd.GetBoneByName(effector_node_name).IK = this;

            target_node_id = pmd.GetBoneIDByName(target_node_name);

            links.Clear();
            foreach (string node_name in chain_node_names)
            {
                PMD_IKNode link = new PMD_IKNode();

                link.node_id = pmd.GetBoneIDByName(node_name);
                
                if (node_name == "左ひざ" || node_name == "右ひざ")
                {
                    link.constrain_angle = true;
                    link.angle_min.X = Geometry.DegreeToRadian(-180f);
                    link.angle_max.X = Geometry.DegreeToRadian(-0.5f);
                }
                links.Add(link);
            }
        }
    }

    /// IKリンク
    public class PMD_IKNode
    {
        public short node_id;
        public bool constrain_angle = false;
        public Vector3 angle_min = Vector3.Empty;
        public Vector3 angle_max = Vector3.Empty;

        public void Write(BinaryWriter bw)
        {
            bw.Write(node_id);
            if (constrain_angle)
            {
                bw.Write((byte)1);
                bw.Write(ref angle_min);
                bw.Write(ref angle_max);
            }
            else
            {
                bw.Write((byte)0);
            }
        }
    }

    /// 表情
    public class PMD_Skin
    {
        public string name;
        public string name_en;

        public byte panel_id;
        public byte type = 1;//モーフ種類 1:頂点

        public PMD_SkinVertex[] vertices;

        public void Write(BinaryWriter bw)
        {
            bw.WritePString(name);
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

    public class PMD_DispGroup
    {
        public string name;
        public string name_en;
        public byte spec;
        public List<PMD_Disp> disps = new List<PMD_Disp>();

        public void Write(BinaryWriter bw)
        {
            bw.WritePString(name);
            bw.WritePString(name_en);

            bw.Write(spec);
            bw.Write(disps.Count);
            foreach (PMD_Disp disp in disps)
            {
                disp.Write(bw);
            }
        }
    }
    
    public abstract class PMD_Disp
    {
        public abstract void Write(BinaryWriter bw);
        public abstract void SetBoneIDFromName(PmxFile pmd);
    }
    
    public class PMD_BoneDisp : PMD_Disp
    {
        public short bone_id;

        public override void Write(BinaryWriter bw)
        {
            bw.Write((byte)0);
            bw.Write(bone_id);
        }

        public string bone_name;

        // ボーン名をIDに置き換える
        public override void SetBoneIDFromName(PmxFile pmd)
        {
            bone_id = pmd.GetBoneIDByName(bone_name);
        }
    }

    public class PMD_SkinDisp : PMD_Disp
    {
        public sbyte skin_id;

        public override void Write(BinaryWriter bw)
        {
            bw.Write((byte)1);
            bw.Write(skin_id);
        }

        // ボーン名をIDに置き換える
        public override void SetBoneIDFromName(PmxFile pmd)
        {
            //
        }
    }

    /// 剛体
    public class PMD_RBody
    {
        public string name;
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
            bw.WritePString(name);
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
        public string name;
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
            bw.WritePString(name);
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
