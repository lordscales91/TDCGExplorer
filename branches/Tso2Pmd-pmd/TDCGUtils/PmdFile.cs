using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

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

        public void WritePmdFile(StreamWriter i_writer)
        {
            //throw new NotImplementedException();
        }
        public PMD_Bone getBoneByName(string name)
        {
            foreach (PMD_Bone bone in this.pmd_bone)
                if (bone.szName == name) return bone;

            return null;
        }
    }

    public class PMD_Header
    {
        public const int SIZE_OF_STRUCT = 3 + 4 + 20 + 256;
        public String szMagic;
        public float fVersion;
        public String szName;
        public String szComment;
    }

    public class PMD_Vertex
    {
        public Vector3 vec3Pos = new Vector3();	// 座標
        public Vector3 vec3Normal = new Vector3();	// 法線ベクトル
        public float u, v;		// テクスチャ座標

        internal int[] unBoneNo = new int[2];	// ボーン番号
        public int cbWeight;		// ブレンドの重み (0～100％)
        public int cbEdge;			// エッジフラグ

        public string[] unBoneName = new string[2];	// ボーン番号
    }

    public class PMD_Material
    {
        public Vector4 col4Diffuse = new Vector4();
        public float fShininess;
        public Vector3 col3Specular = new Vector3();
        public Vector3 col3Ambient = new Vector3();
        public int toon_index; // toon??.bmp // 0.bmp:0xFF, 1(01).bmp:0x00 ・・・ 10.bmp:0x09
        public int edge_flag; // 輪郭、影
        public int ulNumIndices;		// この材質に対応する頂点数
        public String szTextureFileName;	// テクスチャファイル名
    }

    public class PMD_Bone
    {
        public String szName;			// ボーン名 (0x00 終端，余白は 0xFD)
        internal int nParentNo;			// 親ボーン番号 (なければ -1)
        internal int nChildNo;			// 子ボーン番号
        public int cbKind;		// ボーンの種類
        internal int unIKTarget;	// IK時のターゲットボーン
        public Vector3 vec3Position = new Vector3();	// モデル原点からの位置

        public string ParentName;	// 親ボーン名(なければ null)
        public string ChildName;	// 子ボーン名(なければ null)
        public string IKTargetName;	// IK時のターゲットボーン(なければ null)
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
    }

    public class PMD_FACE_VTX
    {
        public int ulIndex;
        public Vector3 vec3Pos = new Vector3();

        public static PMD_FACE_VTX[] createArray(int i_length)
        {
            PMD_FACE_VTX[] ret = new PMD_FACE_VTX[i_length];
            for (int i = 0; i < i_length; i++)
            {
                ret[i] = new PMD_FACE_VTX();
            }
            return ret;
        }
    }

    public class PMD_BoneDisp
    {
        internal int bone_index; // 枠用ボーン番号
        public int bone_disp_frame_index; // 表示枠番号

        public string bone_name;
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
        public Vector3 pos_pos = new Vector3(); // 位置：位置(x, y, z)
        public Vector3 pos_rot = new Vector3(); // 位置：回転(rad(x), rad(y), rad(z))
        public float rigidbody_weight; // 諸データ：質量 // 00 00 80 3F // 1.0
        public float rigidbody_pos_dim; // 諸データ：移動減 // 00 00 00 00
        public float rigidbody_rot_dim; // 諸データ：回転減 // 00 00 00 00
        public float rigidbody_recoil; // 諸データ：反発力 // 00 00 00 00
        public float rigidbody_friction; // 諸データ：摩擦力 // 00 00 00 00
        public int rigidbody_type; // 諸データ：タイプ(0:Bone追従、1:物理演算、2:物理演算(Bone位置合せ)) // 00 // Bone追従
    }

    public class PMD_Joint
    {
        public String joint_name; // 諸データ：名称 // 右髪1
        public int joint_rigidbody_a; // 諸データ：剛体A
        public int joint_rigidbody_b; // 諸データ：剛体B
        public Vector3 joint_pos = new Vector3(); // 諸データ：位置(x, y, z) // 諸データ：位置合せでも設定可
        public Vector3 joint_rot = new Vector3(); // 諸データ：回転(rad(x), rad(y), rad(z))
        public Vector3 constrain_pos_1 = new Vector3(); // 制限：移動1(x, y, z)
        public Vector3 constrain_pos_2 = new Vector3(); // 制限：移動2(x, y, z)
        public Vector3 constrain_rot_1 = new Vector3(); // 制限：回転1(rad(x), rad(y), rad(z))
        public Vector3 constrain_rot_2 = new Vector3(); // 制限：回転2(rad(x), rad(y), rad(z))
        public Vector3 spring_pos = new Vector3(); // ばね：移動(x, y, z)
        public Vector3 spring_rot = new Vector3(); // ばね：回転(rad(x), rad(y), rad(z))
    }
}
