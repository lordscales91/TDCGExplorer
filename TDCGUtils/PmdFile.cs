﻿using System;
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
        public PMD_Header pmd_header; // ヘッダー情報

        public int number_of_vertex; // 頂点数
        public PMD_Vertex[] pmd_vertex; // 頂点配列

        public int number_of_indices; // 頂点インデックス数
        public short[] indices_array; // 頂点インデックス配列

        public int number_of_materials; // マテリアル数　
        public PMD_Material[] pmd_material; // マテリアル配列

        public int number_of_bone; // Bone数
        public PMD_Bone[] pmd_bone; // Bone配列

        public int number_of_ik; // IK数
        public PMD_IK[] pmd_ik; // IK配列

        public int number_of_face; // Face数
        public PMD_FACE[] pmd_face; // Face配列

        public int skin_disp_count; // 表情枠に表示する表情数
        public int[] skin_index; // 表情番号

        public int bone_disp_name_count; // ボーン枠用の枠名数
        public string[] disp_name; // 枠名(50Bytes/枠)

        public int bone_disp_count; // ボーン枠に表示するボーン数 (枠0(センター)を除く、すべてのボーン枠の合計)
        public PMD_BoneDisp[] bone_disp; // 枠用ボーンデータ (3Bytes/bone)

        public int english_name_compatibility; // 英名対応(01:英名対応あり)

        public string[] toon_file_name;//[100][10]; // トゥーンテクスチャファイル名

        public int rigidbody_count; // 剛体数 // 2D 00 00 00 == 45
        public PMD_RigidBody[] rigidbody;//[rigidbody_count]; // 剛体データ(83Bytes/rigidbody)

        public int joint_count; // ジョイント数 // 1B 00 00 00 == 27
        public PMD_Joint[] joint;//[joint_count]; // ジョイントデータ(124Bytes/joint)

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

            // -----------------------------------------------------
            // ヘッダー情報
            pmd_header.Write(bw);

            // -----------------------------------------------------
            // 頂点数
            bw.Write((int)number_of_vertex);

            // ボーン名をIDに置き換え
            foreach (PMD_Vertex vertex in pmd_vertex)
            {
                if (vertex.unBoneName[0] == null) vertex.unBoneNo[0] = -1;
                else vertex.unBoneNo[0] = getBoneIDByName(vertex.unBoneName[0]);
                if (vertex.unBoneName[1] == null) vertex.unBoneNo[1] = -1;
                else vertex.unBoneNo[1] = getBoneIDByName(vertex.unBoneName[1]);
            }

            // 頂点配列
            for (int i = 0; i < number_of_vertex; i++)
            {
                pmd_vertex[i].Write(bw);
            }

            // -----------------------------------------------------
            // 頂点インデックス数
            bw.Write((int)number_of_indices);

            // 頂点インデックス配列
            for (int i = 0; i < number_of_indices; i++)
            {
                bw.Write((short)indices_array[i]);
            }

            // -----------------------------------------------------
            // マテリアル数
            bw.Write((int)number_of_materials);

            // マテリアル配列
            for (int i = 0; i < number_of_materials; i++)
            {
                pmd_material[i].Write(bw);
            }

            // -----------------------------------------------------
            // Bone数
            bw.Write((short)number_of_bone);

            // ボーン名をIDに置き換え
            foreach (PMD_Bone bone in pmd_bone)
            {
                if (bone.ParentName == null) bone.nParentNo = -1;
                else bone.nParentNo = getBoneIDByName(bone.ParentName);
                if (bone.ChildName == null) bone.nChildNo = 0;
                else bone.nChildNo = getBoneIDByName(bone.ChildName);
                if (bone.IKTargetName == null) bone.unIKTarget = 0;
                else bone.unIKTarget = getBoneIDByName(bone.IKTargetName);
            }

            // Bone配列
            for (int i = 0; i < number_of_bone; i++)
            {
                pmd_bone[i].Write(bw);
            }

            // -----------------------------------------------------
            // IK数
            bw.Write((short)number_of_ik);

            if (number_of_ik > 0)
            {
                // ボーン名をIDに置き換え
                foreach (PMD_IK ik in pmd_ik)
                {
                    if (ik.nTargetName == null) ik.nTargetNo = -1;
                    else ik.nTargetNo = getBoneIDByName(ik.nTargetName);
                    if (ik.nEffName == null) ik.nEffNo = -1;
                    else ik.nEffNo = getBoneIDByName(ik.nEffName);

                    if (ik.nTargetNo <= -1) ik.nTargetName = null;
                    else ik.nTargetName = pmd_bone[ik.nTargetNo].szName;
                    if (ik.nEffNo <= -1) ik.nEffName = null;
                    else ik.nEffName = pmd_bone[ik.nEffNo].szName;

                    for (int i = 0; i < ik.cbNumLink; i++)
                    {
                        if (ik.punLinkName[i] == null) ik.punLinkNo[i] = -1;
                        else ik.punLinkNo[i] = getBoneIDByName(ik.punLinkName[i]);
                    }
                }

                // IK配列
                for (int i = 0; i < number_of_ik; i++)
                    pmd_ik[i].Write(bw);
            }

            // -----------------------------------------------------
            // Face数
            bw.Write((short)number_of_face);

            // Face配列
            for (int i = 0; i < number_of_face; i++)
            {
                pmd_face[i].Write(bw);
            }

            // -----------------------------------------------------
            // 表情枠
            // -----------------------------------------------------
            // 表情枠に表示する表情数
            bw.Write((byte)skin_disp_count);

            // 表情番号
            if (skin_disp_count > 0)
            {
                for (int i = 0; i < skin_disp_count; i++)
                {
                    bw.Write((short)skin_index[i]);
                }
            }

            // -----------------------------------------------------
            // ボーン枠
            // -----------------------------------------------------
            // ボーン枠用の枠名数
            bw.Write((byte)bone_disp_name_count);

            // 枠名(50Bytes/枠)
            if (bone_disp_name_count > 0)
            {
                for (int i = 0; i < bone_disp_name_count; i++)
                {
                    bw.WriteCString(disp_name[i], 50);
                }
            }

            // -----------------------------------------------------
            // ボーン枠に表示するボーン
            // -----------------------------------------------------
            // ボーン枠に表示するボーン数
            bw.Write((int)bone_disp_count);

            // ボーン名をIDに置き換え
            for (int i = 0; i < bone_disp.Length; i++)
            {
                bone_disp[i].bone_index = getBoneIDByName(bone_disp[i].bone_name);
            }

            // 枠用ボーンデータ (3Bytes/bone)
            if (bone_disp_count > 0)
            {
                for (int i = 0; i < bone_disp_count; i++)
                {
                    bone_disp[i].Write(bw);
                }
            }

            // -----------------------------------------------------
            // 英名対応(0:英名対応なし, 1:英名対応あり)
            // -----------------------------------------------------
            bw.Write((byte)0);//english_name_compatibility

            // -----------------------------------------------------
            // トゥーンテクスチャファイル名
            // -----------------------------------------------------
            for (int i = 0; i < 10; i++)
            {
                bw.WriteCString(toon_file_name[i], 100);
            }

            // -----------------------------------------------------
            // 剛体
            // -----------------------------------------------------
            // 剛体数
            bw.Write((int)rigidbody_count);

            // 剛体データ(83Bytes/rigidbody)
            if (rigidbody_count > 0)
            {
                for (int i = 0; i < rigidbody_count; i++)
                {
                    rigidbody[i].Write(bw);
                }
            }

            // -----------------------------------------------------
            // ジョイント
            // -----------------------------------------------------
            // ジョイント数
            bw.Write((int)joint_count);

            // ジョイントデータ(124Bytes/joint)
            if (joint_count > 0)
            {
                for (int i = 0; i < joint_count; i++)
                {
                    joint[i].Write(bw);
                }
            }
        }
        public PMD_Bone getBoneByName(string name)
        {
            foreach (PMD_Bone bone in this.pmd_bone)
                if (bone.szName == name) return bone;

            return null;
        }

        int getBoneIDByName(string name)
        {
            for (int i = 0; i < this.pmd_bone.Length; i++)
                if (pmd_bone[i].szName == name) return i;

            return -1;
        }
    }

    public class PMD_Header
    {
        public const int SIZE_OF_STRUCT = 3 + 4 + 20 + 256;
        public String szMagic;
        public float fVersion;
        public String szName;
        public String szComment;

        internal void Write(BinaryWriter writer)
        {
            writer.WriteCString(this.szMagic, 3);
            writer.Write(this.fVersion);
            writer.WriteCString(this.szName, 20);
            writer.WriteCString(this.szComment, 256);
        }
    }

    public class PMD_Vertex
    {
        public Vector3 vec3Pos = Vector3.Empty;	// 座標
        public Vector3 vec3Normal = Vector3.Empty;	// 法線ベクトル
        public float u, v;		// テクスチャ座標

        internal int[] unBoneNo = new int[2];	// ボーン番号
        public int cbWeight;		// ブレンドの重み (0～100％)
        public int cbEdge;			// エッジフラグ

        public string[] unBoneName = new string[2];	// ボーン番号

        internal void Write(BinaryWriter writer)
        {
            writer.Write(ref this.vec3Pos);
            writer.Write(ref this.vec3Normal);
            writer.Write(u);
            writer.Write(v);
            writer.Write((ushort)this.unBoneNo[0]);
            writer.Write((ushort)this.unBoneNo[1]);
            writer.Write((sbyte)this.cbWeight);
            writer.Write((sbyte)this.cbEdge);
        }
    }

    public class PMD_Material
    {
        public Vector4 col4Diffuse = Vector4.Empty;
        public float fShininess;
        public Vector3 col3Specular = Vector3.Empty;
        public Vector3 col3Ambient = Vector3.Empty;
        public int toon_index; // toon??.bmp // 0.bmp:0xFF, 1(01).bmp:0x00 ・・・ 10.bmp:0x09
        public int edge_flag; // 輪郭、影
        public int ulNumIndices;		// この材質に対応する頂点数
        public String szTextureFileName;	// テクスチャファイル名

        internal void Write(BinaryWriter writer)
        {
            writer.Write(ref this.col4Diffuse);
            writer.Write(this.fShininess);
            writer.Write(ref this.col3Specular);
            writer.Write(ref this.col3Ambient);
            writer.Write((byte)this.toon_index); // toon??.bmp // 0.bmp:0xFF, 1(01).bmp:0x00 ・・・ 10.bmp:0x09
            writer.Write((byte)this.edge_flag); // 輪郭、影
            writer.Write(this.ulNumIndices);
            writer.WriteCString(this.szTextureFileName, 20);
        }
    }

    public class PMD_Bone
    {
        public String szName;			// ボーン名 (0x00 終端，余白は 0xFD)
        internal int nParentNo;			// 親ボーン番号 (なければ -1)
        internal int nChildNo;			// 子ボーン番号
        public int cbKind;		// ボーンの種類
        internal int unIKTarget;	// IK時のターゲットボーン
        public Vector3 vec3Position = Vector3.Empty;	// モデル原点からの位置

        public string ParentName;	// 親ボーン名(なければ null)
        public string ChildName;	// 子ボーン名(なければ null)
        public string IKTargetName;	// IK時のターゲットボーン(なければ null)

        internal void Write(BinaryWriter writer)
        {
            writer.WriteCString(this.szName, 20);
            writer.Write((short)this.nParentNo);
            writer.Write((short)this.nChildNo);
            writer.Write((byte)this.cbKind);
            writer.Write((short)this.unIKTarget);
            writer.Write(ref this.vec3Position);
        }
    }

    public class PMD_IK
    {
        internal int nTargetNo;	// IKターゲットボーン番号
        internal int nEffNo;		// IK先端ボーン番号
        public int cbNumLink;	// IKを構成するボーンの数
        public int unCount;
        public float fFact;
        internal int[] punLinkNo = new int[128];// IKを構成するボーンの配列(可変長配列)

        public string nTargetName;	// IKターゲットボーン名
        public string nEffName;		// IK先端ボーン名
        public string[] punLinkName = new string[128];// IKを構成するボーンの配列(可変長配列)

        internal void Write(BinaryWriter writer)
        {
            writer.Write((short)this.nTargetNo);
            writer.Write((short)this.nEffNo);
            writer.Write((sbyte)this.cbNumLink);
            writer.Write((ushort)this.unCount);
            writer.Write(this.fFact);

            if (this.cbNumLink > this.punLinkNo.Length)
            {
                this.punLinkNo = new int[this.cbNumLink];
            }
            for (int i = 0; i < this.cbNumLink; i++)
            {
                writer.Write((ushort)this.punLinkNo[i]);
            }
        }
    }

    public class PMD_FACE
    {
        public String szName;		// 表情名 (0x00 終端，余白は 0xFD)
        private int numVertices;	// 表情頂点数
        public int cbType;			// 分類 (0：base、1：まゆ、2：目、3：リップ、4：その他)
        public PMD_FACE_VTX[] pVertices = PMD_FACE_VTX.createArray(64);// 表情頂点データ

        public PMD_FACE(int n)
        {
            this.numVertices = n;

            if (this.numVertices > this.pVertices.Length)
            {
                this.pVertices = PMD_FACE_VTX.createArray(this.numVertices);
            }
        }

        internal void Write(BinaryWriter writer)
        {
            writer.WriteCString(this.szName, 20);
            writer.Write(this.numVertices);
            writer.Write((sbyte)this.cbType);

            for (int i = 0; i < this.numVertices; i++)
            {
                this.pVertices[i].Write(writer);
            }
        }
    }

    public class PMD_FACE_VTX
    {
        public int ulIndex;
        public Vector3 vec3Pos = Vector3.Empty;

        public static PMD_FACE_VTX[] createArray(int i_length)
        {
            PMD_FACE_VTX[] ret = new PMD_FACE_VTX[i_length];
            for (int i = 0; i < i_length; i++)
            {
                ret[i] = new PMD_FACE_VTX();
            }
            return ret;
        }

        internal void Write(BinaryWriter writer)
        {
            writer.Write(this.ulIndex);
            writer.Write(ref this.vec3Pos);
        }
    }

    public class PMD_BoneDisp
    {
        internal int bone_index; // 枠用ボーン番号
        public int bone_disp_frame_index; // 表示枠番号

        public string bone_name;

        internal void Write(BinaryWriter writer)
        {
            writer.Write((ushort)this.bone_index); // 枠用ボーン番号
            writer.Write((byte)this.bone_disp_frame_index); // 表示枠番号
        }
    }

    public class PMD_RigidBody
    {
        public String rigidbody_name; // 諸データ：名称 // 頭
        public int rigidbody_rel_bone_index; // 諸データ：関連ボーン番号 // 03 00 == 3 // 頭
        public int rigidbody_group_index; // 諸データ：グループ // 00
        public int rigidbody_group_target; // 諸データ：グループ：対象 // 0xFFFFとの差 // 38 FE
        public int shape_type; // 形状：タイプ(0:球、1:箱、2:カプセル) // 00 // 球
        public float shape_w; // 形状：半径(幅) // CD CC CC 3F // 1.6
        public float shape_h; // 形状：高さ // CD CC CC 3D // 0.1
        public float shape_d; // 形状：奥行 // CD CC CC 3D // 0.1
        public Vector3 pos_pos = Vector3.Empty; // 位置：位置(x, y, z)
        public Vector3 pos_rot = Vector3.Empty; // 位置：回転(rad(x), rad(y), rad(z))
        public float rigidbody_weight; // 諸データ：質量 // 00 00 80 3F // 1.0
        public float rigidbody_pos_dim; // 諸データ：移動減 // 00 00 00 00
        public float rigidbody_rot_dim; // 諸データ：回転減 // 00 00 00 00
        public float rigidbody_recoil; // 諸データ：反発力 // 00 00 00 00
        public float rigidbody_friction; // 諸データ：摩擦力 // 00 00 00 00
        public int rigidbody_type; // 諸データ：タイプ(0:Bone追従、1:物理演算、2:物理演算(Bone位置合せ)) // 00 // Bone追従

        internal void Write(BinaryWriter writer)
        {
            writer.WriteCString(this.rigidbody_name, 20); // 諸データ：名称 // 頭
            writer.Write((short)this.rigidbody_rel_bone_index); // 諸データ：関連ボーン番号 // 03 00 == 3 // 頭
            writer.Write((byte)this.rigidbody_group_index); // 諸データ：グループ // 00
            writer.Write((short)this.rigidbody_group_target); // 諸データ：グループ：対象 // 0xFFFFとの差 // 38 FE
            writer.Write((byte)this.shape_type); // 形状：タイプ(0:球、1:箱、2:カプセル) // 00 // 球
            writer.Write(this.shape_w); // 形状：半径(幅) // CD CC CC 3F // 1.6
            writer.Write(this.shape_h); // 形状：高さ // CD CC CC 3D // 0.1
            writer.Write(this.shape_d); // 形状：奥行 // CD CC CC 3D // 0.1
            writer.Write(ref this.pos_pos); // 位置：位置(x, y, z)
            writer.Write(ref this.pos_rot); // 位置：回転(rad(x), rad(y), rad(z))
            writer.Write(this.rigidbody_weight); // 諸データ：質量 // 00 00 80 3F // 1.0
            writer.Write(this.rigidbody_pos_dim); // 諸データ：移動減 // 00 00 00 00
            writer.Write(this.rigidbody_rot_dim); // 諸データ：回転減 // 00 00 00 00
            writer.Write(this.rigidbody_recoil); // 諸データ：反発力 // 00 00 00 00
            writer.Write(this.rigidbody_friction); // 諸データ：摩擦力 // 00 00 00 00
            writer.Write((byte)this.rigidbody_type); // 諸データ：タイプ(0:Bone追従、1:物理演算、2:物理演算(Bone位置合せ)) // 00 // Bone追従
        }
    }

    public class PMD_Joint
    {
        public String joint_name; // 諸データ：名称 // 右髪1
        public int joint_rigidbody_a; // 諸データ：剛体A
        public int joint_rigidbody_b; // 諸データ：剛体B
        public Vector3 joint_pos = Vector3.Empty; // 諸データ：位置(x, y, z) // 諸データ：位置合せでも設定可
        public Vector3 joint_rot = Vector3.Empty; // 諸データ：回転(rad(x), rad(y), rad(z))
        public Vector3 constrain_pos_1 = Vector3.Empty; // 制限：移動1(x, y, z)
        public Vector3 constrain_pos_2 = Vector3.Empty; // 制限：移動2(x, y, z)
        public Vector3 constrain_rot_1 = Vector3.Empty; // 制限：回転1(rad(x), rad(y), rad(z))
        public Vector3 constrain_rot_2 = Vector3.Empty; // 制限：回転2(rad(x), rad(y), rad(z))
        public Vector3 spring_pos = Vector3.Empty; // ばね：移動(x, y, z)
        public Vector3 spring_rot = Vector3.Empty; // ばね：回転(rad(x), rad(y), rad(z))

        internal void Write(BinaryWriter writer)
        {
            writer.WriteCString(this.joint_name, 20); // 諸データ：名称 // 右髪1
            writer.Write(this.joint_rigidbody_a); // 諸データ：剛体A
            writer.Write(this.joint_rigidbody_b); // 諸データ：剛体B
            writer.Write(ref this.joint_pos); // 諸データ：位置(x, y, z) // 諸データ：位置合せでも設定可
            writer.Write(ref this.joint_rot); // 諸データ：回転(rad(x), rad(y), rad(z))
            writer.Write(ref this.constrain_pos_1); // 制限：移動1(x, y, z)
            writer.Write(ref this.constrain_pos_2); // 制限：移動2(x, y, z)
            writer.Write(ref this.constrain_rot_1); // 制限：回転1(rad(x), rad(y), rad(z))
            writer.Write(ref this.constrain_rot_2); // 制限：回転2(rad(x), rad(y), rad(z))
            writer.Write(ref this.spring_pos); // ばね：移動(x, y, z)
            writer.Write(ref this.spring_rot); // ばね：回転(rad(x), rad(y), rad(z))
        }
    }
}