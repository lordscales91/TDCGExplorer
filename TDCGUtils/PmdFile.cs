using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

using TDCGUtils.Extensions;

namespace TDCGUtils
{
    public class PmdFile
    {
        // ヘッダー
        public PMD_Header pmd_header;

        // 頂点配列
        public PMD_Vertex[] vertices;

        // 頂点インデックス配列
        public short[] vindices;

        // 材質配列
        public PMD_Material[] materials;

        // ボーン配列
        public PMD_Bone[] nodes;

        // IK配列
        public PMD_IK[] iks;

        // 表情配列
        public PMD_Skin[] skins;

        // 表情枠：表情インデックス配列
        public int[] skin_disp_indices;

        // ボーン枠の枠名配列
        public string[] disp_names;

        // ボーン枠
        public PMD_BoneDisp[] bone_disps;

        // 英名対応(01:英名対応あり)
        public int english_name_compatibility;

        // トゥーンテクスチャファイル名
        public string[] toon_file_name;

        // 剛体
        public PMD_RBody[] bodies;

        // ジョイント
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

            pmd_header.Write(bw);

            bw.Write(vertices.Length);
            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i].SetBoneIDFromName(this);
                vertices[i].Write(bw);
            }

            bw.Write(vindices.Length);
            for (int i = 0; i < vindices.Length; i++)
            {
                bw.Write((short)vindices[i]);
            }

            bw.Write(materials.Length);
            for (int i = 0; i < materials.Length; i++)
            {
                materials[i].Write(bw);
            }

            bw.Write((short)nodes.Length);
            for (int i = 0; i < nodes.Length; i++)
            {
                nodes[i].SetBoneIDFromName(this);
                nodes[i].Write(bw);
            }

            bw.Write((short)iks.Length);
            for (int i = 0; i < iks.Length; i++)
            {
                iks[i].SetBoneIDFromName(this);
                iks[i].Write(bw);
            }

            bw.Write((short)skins.Length);
            for (int i = 0; i < skins.Length; i++)
            {
                skins[i].Write(bw);
            }

            bw.Write((byte)skin_disp_indices.Length);
            for (int i = 0; i < skin_disp_indices.Length; i++)
            {
                bw.Write((short)skin_disp_indices[i]);
            }

            bw.Write((byte)disp_names.Length);
            for (int i = 0; i < disp_names.Length; i++)
            {
                bw.WriteCString(disp_names[i], 50);
            }

            bw.Write(bone_disps.Length);
            for (int i = 0; i < bone_disps.Length; i++)
            {
                bone_disps[i].SetBoneIDFromName(this);
                bone_disps[i].Write(bw);
            }

            bw.Write((byte)0);//english_name_compatibility

            for (int i = 0; i < 10; i++)
            {
                bw.WriteCString(toon_file_name[i], 100);
            }

            bw.Write(bodies.Length);
            for (int i = 0; i < bodies.Length; i++)
            {
                bodies[i].Write(bw);
            }

            bw.Write(joints.Length);
            for (int i = 0; i < joints.Length; i++)
            {
                joints[i].Write(bw);
            }
        }

        public PMD_Bone GetBoneByName(string name)
        {
            if (name != null)
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
        public const int SIZE_OF_STRUCT = 3 + 4 + 20 + 256;
        public String magic;
        public float version;
        public String name;
        public String comment;

        internal void Write(BinaryWriter writer)
        {
            writer.WriteCString(this.magic, 3);
            writer.Write(this.version);
            writer.WriteCString(this.name, 20);
            writer.WriteCString(this.comment, 256);
        }
    }

    public class PMD_Vertex
    {
        // 座標
        public Vector3 position = Vector3.Empty;

        // 法線ベクトル
        public Vector3 normal = Vector3.Empty;

        // テクスチャ座標
        public float u, v;

        // ボーン番号
        internal int[] bone_indices = new int[2];

        // ブレンドの重み (0～100％)
        public sbyte weight;	

        // エッジフラグ
        public sbyte edge;	

        public string[] unBoneName = new string[2];	// ボーン番号

        internal void Write(BinaryWriter writer)
        {
            writer.Write(ref this.position);
            writer.Write(ref this.normal);
            writer.Write(u);
            writer.Write(v);
            writer.Write((ushort)this.bone_indices[0]);
            writer.Write((ushort)this.bone_indices[1]);
            writer.Write(this.weight);
            writer.Write(this.edge);
        }

        // ボーン名をIDに置き換える
        public void SetBoneIDFromName(PmdFile pmd)
        {
            bone_indices[0] = pmd.GetBoneIDByName(unBoneName[0]);
            bone_indices[1] = pmd.GetBoneIDByName(unBoneName[1]);
        }
    }

    public class PMD_Material
    {
        public Vector4 diffuse = Vector4.Empty;
        public float shininess;
        public Vector3 specular = Vector3.Empty;
        public Vector3 ambient = Vector3.Empty;
        public sbyte toon_tex_id; // toon??.bmp // 0.bmp:0xFF, 1(01).bmp:0x00 ・・・ 10.bmp:0x09
        public sbyte edge; // 輪郭、影
        public int vindices_count;		// この材質に対応する頂点数
        public String tex_path;	// テクスチャファイル名

        internal void Write(BinaryWriter writer)
        {
            writer.Write(ref this.diffuse);
            writer.Write(this.shininess);
            writer.Write(ref this.specular);
            writer.Write(ref this.ambient);
            writer.Write(this.toon_tex_id);
            writer.Write(this.edge);
            writer.Write(this.vindices_count);
            writer.WriteCString(this.tex_path, 20);
        }
    }

    public class PMD_Bone
    {
        // ボーン名 (0x00 終端，余白は 0xFD)
        public String name;

        // 親ボーン番号
        public short parent_node_id;

        // 子ボーン番号
        public short tail_node_id;

        // ボーンの種類
        public int kind;

        // IK時のターゲットボーン
        public short target_node_id;

        // モデル原点からの位置
        public Vector3 position = Vector3.Empty;

        // 親ボーン名
        public string ParentName;

        // 子ボーン名
        public string TailName;

        // IK時のターゲットボーン
        public string TargetName;

        internal void Write(BinaryWriter writer)
        {
            writer.WriteCString(this.name, 20);
            writer.Write(parent_node_id);
            writer.Write(tail_node_id);
            writer.Write((byte)this.kind);
            writer.Write(target_node_id);
            writer.Write(ref this.position);
        }

        // ボーン名をIDに置き換える
        public void SetBoneIDFromName(PmdFile pmd)
        {
            parent_node_id = pmd.GetBoneIDByName(ParentName);
            tail_node_id = pmd.GetBoneIDByName(TailName);
            target_node_id = pmd.GetBoneIDByName(TargetName);
        }
    }

    public class PMD_IK
    {
        // IKターゲットボーン番号
        internal int target_node_id;
        
        // IK先端ボーン番号
        internal int effector_node_id;
        
        // IKを構成するボーンの数
        public int chain_length
        {
            get { return chain_node_ids.Length; }
        }
        
        public int niteration;
        
        public float weight;
        
        // IKを構成するボーンの配列
        internal int[] chain_node_ids;
        
        // IKターゲットボーン名
        public string target_node_name;
        
        // IK先端ボーン名
        public string effector_node_name;
        
        // IKを構成するボーンの配列
        public List<string> chain_node_names = new List<string>();
        
        internal void Write(BinaryWriter writer)
        {
            writer.Write((short)this.target_node_id);
            writer.Write((short)this.effector_node_id);
            writer.Write((sbyte)this.chain_length);
            writer.Write((ushort)this.niteration);
            writer.Write(this.weight);

            for (int i = 0; i < this.chain_length; i++)
            {
                writer.Write((ushort)this.chain_node_ids[i]);
            }
        }

        // ボーン名をIDに置き換える
        public void SetBoneIDFromName(PmdFile pmd)
        {
            target_node_id = pmd.GetBoneIDByName(target_node_name);
            effector_node_id = pmd.GetBoneIDByName(effector_node_name);

            chain_node_ids = new int[chain_node_names.Count];
            for (int i = 0; i < chain_length; i++)
            {
                chain_node_ids[i] = pmd.GetBoneIDByName(chain_node_names[i]);
            }
        }

    }

    public class PMD_Skin
    {
        // 表情名 (0x00 終端，余白は 0xFD)
        public String name;

        // 表情頂点数
        int vertices_count;
        
        // 分類 (0：base、1：まゆ、2：目、3：リップ、4：その他)
        public int panel_id;

        // 表情頂点データ
        public PMD_SkinVertex[] vertices = PMD_SkinVertex.createArray(64);

        public PMD_Skin(int n)
        {
            this.vertices_count = n;

            if (this.vertices_count > this.vertices.Length)
            {
                this.vertices = PMD_SkinVertex.createArray(this.vertices_count);
            }
        }

        internal void Write(BinaryWriter writer)
        {
            writer.WriteCString(this.name, 20);
            writer.Write(this.vertices_count);
            writer.Write((sbyte)this.panel_id);

            for (int i = 0; i < this.vertices_count; i++)
            {
                this.vertices[i].Write(writer);
            }
        }
    }

    public class PMD_SkinVertex
    {
        public int vertex_id;
        public Vector3 position = Vector3.Empty;

        public static PMD_SkinVertex[] createArray(int i_length)
        {
            PMD_SkinVertex[] ret = new PMD_SkinVertex[i_length];
            for (int i = 0; i < i_length; i++)
            {
                ret[i] = new PMD_SkinVertex();
            }
            return ret;
        }

        internal void Write(BinaryWriter writer)
        {
            writer.Write(this.vertex_id);
            writer.Write(ref this.position);
        }
    }

    public class PMD_BoneDisp
    {
        // 枠用ボーン番号
        internal short bone_index;
        
        // 表示枠番号
        public sbyte disp_group_id; 

        public string bone_name;

        internal void Write(BinaryWriter writer)
        {
            writer.Write(bone_index);
            writer.Write(disp_group_id);
        }

        public void SetBoneIDFromName(PmdFile pmd)
        {
            bone_index = pmd.GetBoneIDByName(bone_name);
        }
    }

    public class PMD_RBody
    {
        public String name; // 諸データ：名称 // 頭
        public int rel_bone_id; // 諸データ：関連ボーン番号 // 03 00 == 3 // 頭
        public int group_id; // 諸データ：グループ // 00
        public int group_non_collision; // 諸データ：グループ：対象 // 0xFFFFとの差 // 38 FE
        public int shape_id; // 形状：タイプ(0:球、1:箱、2:カプセル) // 00 // 球
        
        // 形状：半径(幅) // CD CC CC 3F // 1.6
        // 形状：高さ // CD CC CC 3D // 0.1
        // 形状：奥行 // CD CC CC 3D // 0.1
        public Vector3 size = Vector3.Empty;
        
        public Vector3 position = Vector3.Empty; // 位置：位置(x, y, z)
        public Vector3 rotation = Vector3.Empty; // 位置：回転(rad(x), rad(y), rad(z))
        public float weight; // 諸データ：質量 // 00 00 80 3F // 1.0
        public float position_dim; // 諸データ：移動減 // 00 00 00 00
        public float rotation_dim; // 諸データ：回転減 // 00 00 00 00
        public float recoil; // 諸データ：反発力 // 00 00 00 00
        public float friction; // 諸データ：摩擦力 // 00 00 00 00
        public int type; // 諸データ：タイプ(0:Bone追従、1:物理演算、2:物理演算(Bone位置合せ)) // 00 // Bone追従

        internal void Write(BinaryWriter writer)
        {
            writer.WriteCString(this.name, 20);
            writer.Write((short)this.rel_bone_id);
            writer.Write((byte)this.group_id);
            writer.Write((short)this.group_non_collision);
            writer.Write((byte)this.shape_id);
            writer.Write(ref this.size);
            writer.Write(ref this.position);
            writer.Write(ref this.rotation);
            writer.Write(this.weight);
            writer.Write(this.position_dim);
            writer.Write(this.rotation_dim);
            writer.Write(this.recoil);
            writer.Write(this.friction);
            writer.Write((byte)this.type);
        }
    }

    public class PMD_Joint
    {
        public String name; // 諸データ：名称 // 右髪1
        public int rbody_a_id; // 諸データ：剛体A
        public int rbody_b_id; // 諸データ：剛体B
        public Vector3 position = Vector3.Empty; // 諸データ：位置(x, y, z) // 諸データ：位置合せでも設定可
        public Vector3 rotation = Vector3.Empty; // 諸データ：回転(rad(x), rad(y), rad(z))
        public Vector3 position_min = Vector3.Empty; // 制限：移動1(x, y, z)
        public Vector3 position_max = Vector3.Empty; // 制限：移動2(x, y, z)
        public Vector3 rotation_min = Vector3.Empty; // 制限：回転1(rad(x), rad(y), rad(z))
        public Vector3 rotation_max = Vector3.Empty; // 制限：回転2(rad(x), rad(y), rad(z))
        public Vector3 spring_position = Vector3.Empty; // ばね：移動(x, y, z)
        public Vector3 spring_rotation = Vector3.Empty; // ばね：回転(rad(x), rad(y), rad(z))

        internal void Write(BinaryWriter writer)
        {
            writer.WriteCString(this.name, 20);
            writer.Write(this.rbody_a_id);
            writer.Write(this.rbody_b_id);
            writer.Write(ref this.position);
            writer.Write(ref this.rotation);
            writer.Write(ref this.position_min);
            writer.Write(ref this.position_max);
            writer.Write(ref this.rotation_min);
            writer.Write(ref this.rotation_max);
            writer.Write(ref this.spring_position);
            writer.Write(ref this.spring_rotation);
        }
    }
}
