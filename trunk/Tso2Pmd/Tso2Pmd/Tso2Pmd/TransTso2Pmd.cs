using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

using TDCG;
using TDCGUtils;

using jp.nyatla.nymmd.cs.types;
using jp.nyatla.nymmd.cs.core;
using jp.nyatla.nymmd.cs.struct_type;
using jp.nyatla.nymmd.cs.struct_type.pmd;

namespace Tso2Pmd
{
    public class TransTso2Pmd
    {
        PmdFileData pmd = new PmdFileData();

        int kami_flag, chichi_flag, skirt_flag;

        int bone_flag;

        bool spheremap_flag;
        bool edge_flag_flag;
        bool merge_flag;

        List<string> category;

        List<bool> meshes_flag = new List<bool>();

        Figure fig;
        List<TSOSubMesh> mesh_list;
        T2PMaterialList material_list;
        T2PPhysObjectList physOb_list;

        public Figure Figure { get { return fig; } set { fig = value; } }
        public PmdFileData Pmd { get { return pmd; } }
        public int Kami_flag { set { kami_flag = value; } }
        public int Chichi_flag { set { chichi_flag = value; } }
        public int Skirt_flag { set { skirt_flag = value; } }
        public int Bone_flag { set { bone_flag = value; } }
        public bool Spheremap_flag { set { spheremap_flag = value; } }
        public bool Edge_flag_flag { set { edge_flag_flag = value; } }
        public bool Merge_flag { set { merge_flag = value; } }
        public List<string> Category { set { category = value; } }
        public List<bool> Meshes_flag { set { meshes_flag = value; } }

        // -----------------------------------------------------
        // 変換表
        // -----------------------------------------------------
        CorrespondTable girl2miku = new CorrespondTable("girl2miku");
        CorrespondTable man2miku = new CorrespondTable("man2miku");
                
        // -----------------------------------------------------
        // 表情設定リスト
        // -----------------------------------------------------
        Morphing morph = new Morphing();
      
        // 表情に関連するBoneの最小、及び最大
        const int FACE_BONE_MIN = 86;
        const int FACE_BONE_MAX = 135;

        // -----------------------------------------------------
        // コンストラクタ
        // -----------------------------------------------------
        public TransTso2Pmd()
        {
            // -----------------------------------------------------
            // 表情ファイルを読みとる
            morph.Load(Application.StartupPath + @"/表情");

            return;
        }

        // -----------------------------------------------------
        // ヘッダー情報を入力
        // -----------------------------------------------------
        public string InputHeader(string name, string comment)
        { 
            pmd.pmd_header = new PMD_Header();
            pmd.pmd_header.szMagic = "Pmd";
            pmd.pmd_header.fVersion = 1.0f;

            if (name.Length > 9) return "モデル名が9文字を超えています。";
            pmd.pmd_header.szName = name;

            if (comment.Length > 127) return "コメントが127文字を超えています。";
            pmd.pmd_header.szComment = comment;

            return "";
        }

        // -----------------------------------------------------
        // マテリアル関係のファイルを出力
        // -----------------------------------------------------
        public void OutputMaterialFile(string path, string name)
        {
            material_list.Save(path, name, spheremap_flag);
        }

        // -----------------------------------------------------
        // FigureデータよりPmdFileデータを作成
        // -----------------------------------------------------
        public string Figure2PmdFileData()
        {
            if (bone_flag == 0)
            {
                return Figure2PmdFileDataWithHumanBone();
            }
            else
            {
                return Figure2PmdFileDataWithOneBone();
            }
        }

        // -----------------------------------------------------
        // FigureデータよりPmdFileデータを作成
        // -----------------------------------------------------
        public string Figure2PmdFileDataWithOneBone()
        {
            // -----------------------------------------------------
            // 予め、情報をコピーするmeshを選定し、並び替えておく
            // -----------------------------------------------------
            SelectMeshes();

            // -----------------------------------------------------
            // 頂点＆マテリアル
            // -----------------------------------------------------
            MakePMDVertices(2);

            // 頂点数が上限を超えてないかチェックし、超えていたらエラーを出して終了
            if (pmd.number_of_vertex > 65535)
                return "頂点数(" + pmd.number_of_vertex.ToString() + ")が上限(65535)を超えています。";

            // -----------------------------------------------------
            // 表情枠
            // -----------------------------------------------------
            pmd.skin_disp_count = 0; // 表情枠に表示する表情数

            // -----------------------------------------------------
            // ボーン情報
            // -----------------------------------------------------
            pmd.number_of_bone = 2;
            pmd.pmd_bone = new PMD_Bone[pmd.number_of_bone];

            // センター
            pmd.pmd_bone[0] = new PMD_Bone();
            pmd.pmd_bone[0].szName = "センター";
            pmd.pmd_bone[0].cbKind = 1; // ボーンの種類 0:回転 1:回転と移動 2:IK 3:不明 4:IK影響下 5:回転影響下 6:IK接続先 7:非表示 8:捻り 9:回転運動
            pmd.pmd_bone[0].ParentName = null;
            pmd.pmd_bone[0].ChildName = "センター先";
            pmd.pmd_bone[0].IKTargetName = null;
            pmd.pmd_bone[0].vec3Position.x = 0.0f;	// モデル原点からの位置
            pmd.pmd_bone[0].vec3Position.y = 5.0f;	// モデル原点からの位置
            pmd.pmd_bone[0].vec3Position.z = 0.0f;	// モデル原点からの位置

            // センター先
            pmd.pmd_bone[1] = new PMD_Bone();
            pmd.pmd_bone[1].szName = "センター先";
            pmd.pmd_bone[1].cbKind = 7; // ボーンの種類 0:回転 1:回転と移動 2:IK 3:不明 4:IK影響下 5:回転影響下 6:IK接続先 7:非表示 8:捻り 9:回転運動
            pmd.pmd_bone[1].ParentName = "センター";
            pmd.pmd_bone[1].ChildName = null;
            pmd.pmd_bone[1].IKTargetName = null;
            pmd.pmd_bone[1].vec3Position.x = 0.0f;	// モデル原点からの位置
            pmd.pmd_bone[1].vec3Position.y = 0.0f;	// モデル原点からの位置
            pmd.pmd_bone[1].vec3Position.z = 0.0f;	// モデル原点からの位置

            // -----------------------------------------------------
            // IK配列
            // -----------------------------------------------------
            pmd.number_of_ik = 0;

            // -----------------------------------------------------
            // ボーン枠用枠名リスト
            // -----------------------------------------------------
            pmd.bone_disp_name_count = 1; // ボーン枠用の枠名数
            pmd.disp_name = new string[pmd.bone_disp_name_count]; // 枠名(50Bytes/枠)
            pmd.disp_name[0] = "センター" + Convert.ToChar(Convert.ToInt16("0A", 16));
            //PMDEditorを使う場合は、枠名を0x0A00で終わらせる必要があります(0x00のみだと表示されません)。

            // -----------------------------------------------------
            // ボーン枠用表示リスト
            // -----------------------------------------------------
            pmd.bone_disp_count = 1;
            pmd.bone_disp = new PMD_BoneDisp[pmd.bone_disp_count]; // 枠用ボーンデータ (3Bytes/bone)
            pmd.bone_disp[0] = new PMD_BoneDisp();
            pmd.bone_disp[0].bone_name = "センター"; // 枠用ボーン名
            pmd.bone_disp[0].bone_disp_frame_index = 0; // 表示枠番号

            // -----------------------------------------------------
            // 英名対応(0:英名対応なし, 1:英名対応あり)
            // -----------------------------------------------------
            pmd.english_name_compatibility = 0;

            // -----------------------------------------------------
            // 剛体＆ジョイント
            // -----------------------------------------------------
            pmd.rigidbody_count = 0;
            pmd.joint_count = 0;

            // -----------------------------------------------------
            // 終了
            return "";
        }

        // -----------------------------------------------------
        // FigureデータよりPmdFileデータを作成
        // -----------------------------------------------------
        public string Figure2PmdFileDataWithHumanBone()
        {
            CorrespondTable cor_table = null;
            int mod_type = 0;

            if (fig.Tmo.nodes.Length == 227)
            {
                cor_table = girl2miku;
                mod_type = 0;
            }
            else if (fig.Tmo.nodes.Length == 75)
            {
                cor_table = man2miku;
                mod_type = 1;
            }
            else
            {
                return "未対応のボーン構造です。\n人型以外を変換する場合は、\n出力ボーンに”1ボーン”を指定してください。";
            }
 
            // -----------------------------------------------------
            // 予め、情報をコピーするmeshを選定し、並び替えておく
            // -----------------------------------------------------
            SelectMeshes();

            // -----------------------------------------------------
            // 頂点
            // -----------------------------------------------------
            MakePMDVertices(mod_type);

            // 頂点数が上限を超えてないかチェックし、超えていたらエラーを出して終了
            if (pmd.number_of_vertex > 65535)
                return "頂点数(" + pmd.number_of_vertex.ToString() + ")が上限(65535)を超えています。";

            // -----------------------------------------------------
            // 表情
            // -----------------------------------------------------
            if (mod_type == 0)
            {
                InitializePMDFaces();
                MakePMDBaseFace();
                MakePMDFaces();
            }
            else if (mod_type == 1)
            {
                InitializePMDFaces();
                MakePMDBaseFace();
                pmd.number_of_face = 0;
            }

            // -----------------------------------------------------
            // 表情枠
            // -----------------------------------------------------
            if (mod_type == 0)
            {
                pmd.skin_disp_count = pmd.number_of_face - 1; // 表情枠に表示する表情数
                pmd.skin_index = new int[pmd.skin_disp_count];
                for (int i = 0; i < pmd.skin_disp_count; i++)
                    pmd.skin_index[i] = i + 1; // 表情番号
            }
            else if (mod_type == 1)
            {
                pmd.skin_disp_count = 0; // 表情枠に表示する表情数
            }

            // -----------------------------------------------------
            // ボーン情報
            // -----------------------------------------------------
            List<PMD_Bone> bone_list = new List<PMD_Bone>();

            foreach (PMD_Bone bone in cor_table.boneStructure)
            {
                PMD_Bone pmd_b = new PMD_Bone();

                pmd_b.szName = bone.szName;
                pmd_b.cbKind = bone.cbKind; // ボーンの種類 0:回転 1:回転と移動 2:IK 3:不明 4:IK影響下 5:回転影響下 6:IK接続先 7:非表示 8:捻り 9:回転運動
                pmd_b.ParentName = bone.ParentName;
                pmd_b.ChildName = bone.ChildName;
                pmd_b.IKTargetName = bone.IKTargetName;

                string bone_name = null;
                if (mod_type == 0) girl2miku.bonePosition.TryGetValue(pmd_b.szName, out bone_name);
                else if (mod_type == 1) man2miku.bonePosition.TryGetValue(pmd_b.szName, out bone_name);
                if (bone_name != null)
                {
                    pmd_b.vec3Position
                        = Trans.CopyMat2Pos(fig.Tmo.FindNodeByName(bone_name).combined_matrix); // モデル原点からの位置
                }

                bone_list.Add(pmd_b);
            }

            // -----------------------------------------------------
            // 物理オブジェクトを生成
            physOb_list = new T2PPhysObjectList(bone_list);

            // -----------------------------------------------------
            // リストを配列に代入し直す
            pmd.number_of_bone = bone_list.Count;
            pmd.pmd_bone = (PMD_Bone[])bone_list.ToArray();

            // -----------------------------------------------------
            // センターボーンの位置調整
            pmd.getBoneByName("センター").vec3Position
                = new MmdVector3(
                    0.0f, 
                    pmd.getBoneByName("下半身").vec3Position.y * 0.65f, 
                    0.0f);
            pmd.getBoneByName("センター先").vec3Position
                = new MmdVector3(
                    0.0f, 
                    0.0f, 
                    0.0f);

            // -----------------------------------------------------
            // 両目ボーンの位置調整
            if (mod_type == 0)
            {
                pmd.getBoneByName("両目").vec3Position
                    = new MmdVector3(
                        0.0f,
                        pmd.getBoneByName("左目").vec3Position.y + pmd.getBoneByName("左目").vec3Position.x * 4.0f,
                        pmd.getBoneByName("左目").vec3Position.z - pmd.getBoneByName("左目").vec3Position.x * 2.0f);
                pmd.getBoneByName("両目先").vec3Position
                    = new MmdVector3(
                        pmd.getBoneByName("両目").vec3Position.x,
                        pmd.getBoneByName("両目").vec3Position.y,
                        pmd.getBoneByName("両目").vec3Position.z - 1.0f);
            }

            // -----------------------------------------------------
            // IK先ボーンの位置調整
            pmd.getBoneByName("左足ＩＫ先").vec3Position
                = new MmdVector3(
                    pmd.getBoneByName("左足ＩＫ").vec3Position.x,
                    pmd.getBoneByName("左足ＩＫ").vec3Position.y,
                    pmd.getBoneByName("左足ＩＫ").vec3Position.z + 2.0f);
            pmd.getBoneByName("右足ＩＫ先").vec3Position
                = new MmdVector3(
                    pmd.getBoneByName("右足ＩＫ").vec3Position.x,
                    pmd.getBoneByName("右足ＩＫ").vec3Position.y,
                    pmd.getBoneByName("右足ＩＫ").vec3Position.z + 2.0f);
            pmd.getBoneByName("左つま先ＩＫ").vec3Position.z -= 0.25f;
            pmd.getBoneByName("左つま先ＩＫ先").vec3Position
                = new MmdVector3(
                    pmd.getBoneByName("左つま先ＩＫ").vec3Position.x,
                    pmd.getBoneByName("左つま先ＩＫ").vec3Position.y - 2.0f,
                    pmd.getBoneByName("左つま先ＩＫ").vec3Position.z);
            pmd.getBoneByName("右つま先ＩＫ").vec3Position.z -= 0.25f;
            pmd.getBoneByName("右つま先ＩＫ先").vec3Position
                = new MmdVector3(
                    pmd.getBoneByName("右つま先ＩＫ").vec3Position.x,
                    pmd.getBoneByName("右つま先ＩＫ").vec3Position.y - 2.0f,
                    pmd.getBoneByName("右つま先ＩＫ").vec3Position.z);
          
            // -----------------------------------------------------
            // IK配列
            // -----------------------------------------------------
            pmd.number_of_ik = 4;

            pmd.pmd_ik = new PMD_IK[pmd.number_of_ik];

            int n_ik = 0; // 通し番号
            pmd.pmd_ik[n_ik] = new PMD_IK();
            pmd.pmd_ik[n_ik].nTargetName = "右足ＩＫ";	// IKボーン番号
            pmd.pmd_ik[n_ik].nEffName = "右足首";		// IKターゲットボーン番号 // IKボーンが最初に接続するボーン
            pmd.pmd_ik[n_ik].cbNumLink = 2;	// IKチェーンの長さ(子の数)
            pmd.pmd_ik[n_ik].unCount = 40;      // 再帰演算回数 // IK値1
            pmd.pmd_ik[n_ik].fFact = 0.5f;       // IKの影響度 // IK値2
            string[] tem_array1 = { "右ひざ", "右足" }; // IK影響下のボーン番号
            pmd.pmd_ik[n_ik++].punLinkName = tem_array1;

            pmd.pmd_ik[n_ik] = new PMD_IK();
            pmd.pmd_ik[n_ik].nTargetName = "左足ＩＫ";	// IKボーン番号
            pmd.pmd_ik[n_ik].nEffName = "左足首";		// IKターゲットボーン番号 // IKボーンが最初に接続するボーン
            pmd.pmd_ik[n_ik].cbNumLink = 2;	// IKチェーンの長さ(子の数)
            pmd.pmd_ik[n_ik].unCount = 40;      // 再帰演算回数 // IK値1
            pmd.pmd_ik[n_ik].fFact = 0.5f;       // IKの影響度 // IK値2
            string[] tem_array2 = { "左ひざ", "左足" }; // IK影響下のボーン番号
            pmd.pmd_ik[n_ik++].punLinkName = tem_array2;

            pmd.pmd_ik[n_ik] = new PMD_IK();
            pmd.pmd_ik[n_ik].nTargetName = "右つま先ＩＫ";	// IKボーン番号
            pmd.pmd_ik[n_ik].nEffName = "右つま先";		// IKターゲットボーン番号 // IKボーンが最初に接続するボーン
            pmd.pmd_ik[n_ik].cbNumLink = 1;	// IKチェーンの長さ(子の数)
            pmd.pmd_ik[n_ik].unCount = 3;      // 再帰演算回数 // IK値1
            pmd.pmd_ik[n_ik].fFact = 1;       // IKの影響度 // IK値2
            string[] tem_array3 = { "右足首" }; // IK影響下のボーン番号
            pmd.pmd_ik[n_ik++].punLinkName = tem_array3;

            pmd.pmd_ik[n_ik] = new PMD_IK();
            pmd.pmd_ik[n_ik].nTargetName = "左つま先ＩＫ";	// IKボーン番号
            pmd.pmd_ik[n_ik].nEffName = "左つま先";		// IKターゲットボーン番号 // IKボーンが最初に接続するボーン
            pmd.pmd_ik[n_ik].cbNumLink = 1;	// IKチェーンの長さ(子の数)
            pmd.pmd_ik[n_ik].unCount = 3;      // 再帰演算回数 // IK値1
            pmd.pmd_ik[n_ik].fFact = 1;       // IKの影響度 // IK値2
            string[] tem_array4 = { "左足首" }; // IK影響下のボーン番号
            pmd.pmd_ik[n_ik++].punLinkName = tem_array4;

            // -----------------------------------------------------
            // ボーン枠用枠名リスト
            // -----------------------------------------------------
            pmd.bone_disp_name_count = cor_table.dispBoneGroup.Count; // ボーン枠用の枠名数
            pmd.disp_name = new string[pmd.bone_disp_name_count]; // 枠名(50Bytes/枠)

            for (int i = 0; i < cor_table.dispBoneGroup.Count; i++)
                pmd.disp_name[i] = cor_table.dispBoneGroup[i].group_name + Convert.ToChar(Convert.ToInt16("0A", 16));
            //PMDEditorを使う場合は、枠名を0x0A00で終わらせる必要があります(0x00のみだと表示されません)。

            // -----------------------------------------------------
            // ボーン枠用表示リスト
            // -----------------------------------------------------
            // 枠に表示するボーン数
            pmd.bone_disp_count = 0;
            for (int i = 0; i < cor_table.dispBoneGroup.Count; i++)
                pmd.bone_disp_count += cor_table.dispBoneGroup[i].bone_name_list.Count;
            
            // 枠用ボーンデータ (3Bytes/bone)
            pmd.bone_disp = new PMD_BoneDisp[pmd.bone_disp_count]; 

            int n = 0; // 通し番号
            int n_disp = 1;
            foreach (DispBoneGroup dbg in cor_table.dispBoneGroup)
            {
                foreach (string name in dbg.bone_name_list)
                {
                    pmd.bone_disp[n] = new PMD_BoneDisp();
                    pmd.bone_disp[n].bone_name = name; // 枠用ボーン名
                    pmd.bone_disp[n++].bone_disp_frame_index = n_disp; // 表示枠番号
                }
                n_disp++;
            }

            // -----------------------------------------------------
            // 英名対応(0:英名対応なし, 1:英名対応あり)
            // -----------------------------------------------------
            pmd.english_name_compatibility = 0;

            // -----------------------------------------------------
            // 剛体＆ジョイント
            // -----------------------------------------------------

            // -----------------------------------------------------
            // 身体
            physOb_list.MakeBodyFromBone("下半身", 4);

            physOb_list.MakeBodyFromBone("上半身", 4);
            physOb_list.GetBodyByName("上半身").shape_type = 1; // 形状：タイプ(0:球、1:箱、2:カプセル) // 00 // 球
            physOb_list.GetBodyByName("上半身").shape_w = 1.0f; // 形状：半径(幅) // CD CC CC 3F // 1.6
            physOb_list.GetBodyByName("上半身").shape_h *= 0.5f;
            physOb_list.GetBodyByName("上半身").shape_d = 0.5f; // 形状：奥行 // CD CC CC 3D // 0.1

            physOb_list.MakeBodyFromBone("上半身２", 4);
            physOb_list.GetBodyByName("上半身２").shape_type = 1; // 形状：タイプ(0:球、1:箱、2:カプセル) // 00 // 球
            physOb_list.GetBodyByName("上半身２").shape_w = 1.0f; // 形状：半径(幅) // CD CC CC 3F // 1.6
            physOb_list.GetBodyByName("上半身２").shape_h *= 0.5f;
            physOb_list.GetBodyByName("上半身２").shape_d = 0.5f; // 形状：奥行 // CD CC CC 3D // 0.1
 
            physOb_list.MakeBodyFromBone("上半身３", 4);
            physOb_list.GetBodyByName("上半身３").shape_type = 1; // 形状：タイプ(0:球、1:箱、2:カプセル) // 00 // 球
            physOb_list.GetBodyByName("上半身３").shape_w = 1.0f; // 形状：半径(幅) // CD CC CC 3F // 1.6
            physOb_list.GetBodyByName("上半身３").shape_h *= 0.5f;
            physOb_list.GetBodyByName("上半身３").shape_d = 0.5f; // 形状：奥行 // CD CC CC 3D // 0.1

            physOb_list.MakeBodyFromBone("頭", 4);

            physOb_list.MakeBodyFromBone("左肩", 4);

            physOb_list.MakeBodyFromBone("左腕", 4);

            physOb_list.MakeBodyFromBone("左ひじ", 4);

            physOb_list.MakeBodyFromBone("右肩", 4);

            physOb_list.MakeBodyFromBone("右腕", 4);

            physOb_list.MakeBodyFromBone("右ひじ", 4);

            physOb_list.MakeBodyFromBone("左足", 4);
            physOb_list.GetBodyByName("左足").shape_w = 0.75f; // 形状：半径(幅) // CD CC CC 3F // 1.6

            physOb_list.MakeBodyFromBone("左ひざ", 4);

            physOb_list.MakeBodyFromBone("右足", 4);
            physOb_list.GetBodyByName("右足").shape_w = 0.75f; // 形状：半径(幅) // CD CC CC 3F // 1.6

            physOb_list.MakeBodyFromBone("右ひざ", 4);

            // -----------------------------------------------------
            // 身体以外
            if (mod_type == 0)
            {
                if (kami_flag > 0) MakeKamiPhysObject();
                if (chichi_flag > 0) MakeChichiPhysObject();
                if (skirt_flag > 0) MakeSkirtPhysObject();
            }

            // -----------------------------------------------------
            // 剛体＆ジョイントを配列に代入し直す
            pmd.rigidbody_count = physOb_list.body_list.Count;
            pmd.rigidbody = (PMD_RigidBody[])physOb_list.body_list.ToArray();

            pmd.joint_count = physOb_list.joint_list.Count;
            pmd.joint = (PMD_Joint[])physOb_list.joint_list.ToArray();

            // -----------------------------------------------------
            // 終了
            return "";
        }

        // -----------------------------------------------------
        // 表情情報（表情情報では、頂点の情報が必要になるので、
        // 頂点についていろいろやる前に初期化をやっておかねばならない）
        // -----------------------------------------------------
        private void InitializePMDFaces()
        {
            int n_face = 0; // 通し番号
            pmd.number_of_face = 1;

            // 表情数
            foreach (MorphGroup mg in morph.Groups)
                pmd.number_of_face += mg.Items.Count;
            pmd.pmd_face = new PMD_FACE[pmd.number_of_face];

            // 表情に関連するboneに影響を受ける頂点を数え上げる
            int numFaceVertices = CalcNumFaceVertices(fig.Tmo);

            // baseの表情
            pmd.pmd_face[n_face++] = new PMD_FACE(numFaceVertices);

            // base以外の表情
            foreach (MorphGroup mg in morph.Groups)
            {
                foreach (Morph m in mg.Items)
                {
                    pmd.pmd_face[n_face] = new PMD_FACE(numFaceVertices);
                    pmd.pmd_face[n_face].szName = m.Name; // 表情名 (0x00 終端，余白は 0xFD)

                    // 分類 (0：base、1：まゆ、2：目、3：リップ、4：その他)
                    switch (mg.Name)
                    {
                        case "まゆ": pmd.pmd_face[n_face].cbType = 1; break;
                        case "目": pmd.pmd_face[n_face].cbType = 2; break;
                        case "リップ": pmd.pmd_face[n_face].cbType = 3; break;
                        case "その他": pmd.pmd_face[n_face].cbType = 4; break;
                    }

                    n_face++;
                }
            }
        }

        // -----------------------------------------------------
        // 予め、情報をコピーするmeshを選定し、並び替えておく
        // -----------------------------------------------------
        private void SelectMeshes()
        {
            mesh_list = new List<TSOSubMesh>();
            material_list = new T2PMaterialList(fig.TSOList, category);

            int tso_num = 0;
            int sub_mesh_num = 0;
            foreach (TSOFile tso in fig.TSOList)
            {
                for (int script_num = 0; script_num < tso.sub_scripts.Length; script_num++)
                foreach (TSOMesh mesh in tso.meshes)
                foreach (TSOSubMesh sub_mesh in mesh.sub_meshes)
                {
                    if (sub_mesh.spec == script_num)
                    if (meshes_flag[sub_mesh_num++] == true)
                    {
                        mesh_list.Add(sub_mesh);
                        material_list.Add(tso_num, script_num, edge_flag_flag, spheremap_flag);
                    }
                }
                tso_num++;
            }
        }

        private Matrix[] ClipBoneMatrices(TSOSubMesh sub_mesh, TMOFile tmo)
        {
            Matrix[] clipped_boneMatrices = new Matrix[sub_mesh.maxPalettes];
            for (int numPalettes = 0; numPalettes < sub_mesh.maxPalettes; numPalettes++)
            {
                TSONode tso_node = sub_mesh.GetBone(numPalettes);
                TMONode tmo_node = tmo.FindNodeByName(tso_node.Name);
                clipped_boneMatrices[numPalettes] = tso_node.OffsetMatrix * tmo_node.combined_matrix;
            }
            return clipped_boneMatrices;
        }

        List<int> inList_indices = new List<int>();

        // -----------------------------------------------------
        // 頂点を作成
        // -----------------------------------------------------
        private void MakePMDVertices(int mod_type)
        {
            List<PMD_Vertex> vertex_list = new List<PMD_Vertex>();
            List<short> indices = new List<short>(); // インデックスリスト
            inList_indices.Clear();

            // -----------------------------------------------------
            // Tmoの変形を実行
            fig.TPOList.Transform();
            morph.Morph(fig.Tmo);
            fig.UpdateBoneMatricesWithoutTMOFrame();

            // -----------------------------------------------------
            // 情報をコピー
            int n_inList = -1; // list中のvertexの番号（処理の前に++するために、初期値は0でなく-1としている)
            int n_mesh = 0;
            int prevNumIndices = 0;
            int prevNumVertices = 0;

            foreach (TSOSubMesh sub_mesh in mesh_list)
            {
                int n_inMesh = -1; // mesh中のvertexの番号（処理の前に++するために、初期値は0でなく-1としている)
                int a=-1, b=-1, c=-1; // 隣合うインデックス

                Matrix[] clipped_boneMatrices = ClipBoneMatrices(sub_mesh, fig.Tmo);

                foreach (Vertex vertex in sub_mesh.vertices)
                {
                    n_inList++; // list中のvertexの番号を一つ増やす
                    n_inMesh++; // mesh中のvertexの番号を一つ増やす

                    // Tmo中のBoneに従って、Mesh中の頂点位置及び法線ベクトルを置き換えて、書き出す

                    Vector3 pos = Vector3.Empty;
                    Vector3 nor = Vector3.Empty;

                    foreach (SkinWeight sw in vertex.skin_weights)
                    {
                        Matrix m = clipped_boneMatrices[sw.bone_index];

                        // 頂点位置
                        pos += Vector3.TransformCoordinate(vertex.position, m) * sw.weight;

                        // 法線ベクトル
                        m.M41 = 0;
                        m.M42 = 0;
                        m.M43 = 0;
                        nor += Vector3.TransformCoordinate(vertex.normal, m) * sw.weight;
                    }

                    // -----------------------------------------------------
                    // 頂点情報をコピー
                    PMD_Vertex pmd_v = new PMD_Vertex();

                    pmd_v.vec3Pos = Trans.CopyPos(pos);
                    pmd_v.vec3Normal = Trans.CopyPos(Vector3.Normalize(nor));
                    pmd_v.uvTex.u = vertex.u;
                    pmd_v.uvTex.v = vertex.v;

                    pmd_v.cbEdge = 0;

                    // -----------------------------------------------------
                    // スキニング
                    List<string> tmp_b = new List<string>();
                    List<float> tmp_w = new List<float>();

                    for (int i = 0; i < 4; i++)
                    {
                        TSONode tso_bone = sub_mesh.bones[(int)vertex.skin_weights[i].bone_index];
                        string bone_name;
                        if (mod_type == 0)
                            bone_name = girl2miku.skinning[tso_bone.Name];
                        else if (mod_type == 1)
                            bone_name = man2miku.skinning[tso_bone.Name];
                        else
                            bone_name = "センター";

                        if (tmp_b.IndexOf(bone_name) < 0)
                        {
                            tmp_b.Add(bone_name);
                            tmp_w.Add(vertex.skin_weights[i].weight);
                        }
                        else
                        {
                            tmp_w[tmp_b.IndexOf(bone_name)] += vertex.skin_weights[i].weight;
                        }
                    }

                    float w0 = tmp_w.Max();
                    pmd_v.unBoneName[0] = tmp_b[tmp_w.IndexOf(w0)];
                    tmp_b.RemoveAt(tmp_w.IndexOf(w0));
                    tmp_w.RemoveAt(tmp_w.IndexOf(w0));

                    float w1;
                    if (tmp_b.Count == 0)
                    {
                        w1 = 0.0f;
                        pmd_v.unBoneName[1] = pmd_v.unBoneName[0];
                    }
                    else
                    {
                        w1 = tmp_w.Max();
                        pmd_v.unBoneName[1] = tmp_b[tmp_w.IndexOf(w1)];
                    }

                    pmd_v.cbWeight = (int)(w0 * 100 / (w0 + w1));

                    // -----------------------------------------------------
                    // 頂点リストに頂点を追加
                    
                    // 重複している頂点がないかをチェックし、
                    // 存在すれば、そのインデックスを参照
                    // 存在しなければ、頂点リストに頂点を追加
                    int idx = -1;
                    for (int i = prevNumVertices; i < vertex_list.Count; i++)
                    {
                        if (vertex_list[i].vec3Pos.x == pmd_v.vec3Pos.x &&
                            vertex_list[i].vec3Pos.y == pmd_v.vec3Pos.y &&
                            vertex_list[i].vec3Pos.z == pmd_v.vec3Pos.z &&
                            vertex_list[i].vec3Normal.x == pmd_v.vec3Normal.x &&
                            vertex_list[i].vec3Normal.y == pmd_v.vec3Normal.y &&
                            vertex_list[i].vec3Normal.z == pmd_v.vec3Normal.z &&
                            vertex_list[i].uvTex.u == pmd_v.uvTex.u &&
                            vertex_list[i].uvTex.v == pmd_v.uvTex.v)
                        {
                            idx = i;
                            break;
                        }
                    }
                    if (idx == -1)
                    {
                        vertex_list.Add(pmd_v);
                        idx = vertex_list.Count - 1;
                        inList_indices.Add(idx);
                    }
                    else
                        inList_indices.Add(-1);

                    // -----------------------------------------------------
                    // 頂点インデックス

                    // 過去３つまでのインデックスを記憶しておく
                    a = b; b = c; c = idx;

                    // 隣合うインデックスが参照する頂点位置の重複を判定し、
                    // 重複している場合はインデックスの追加を省略する
                    if ((n_inMesh >= 2) &&
                        !((vertex_list[a].vec3Pos.x == vertex_list[b].vec3Pos.x &&
                           vertex_list[a].vec3Pos.y == vertex_list[b].vec3Pos.y &&
                           vertex_list[a].vec3Pos.z == vertex_list[b].vec3Pos.z) ||
                          (vertex_list[b].vec3Pos.x == vertex_list[c].vec3Pos.x &&
                           vertex_list[b].vec3Pos.y == vertex_list[c].vec3Pos.y &&
                           vertex_list[b].vec3Pos.z == vertex_list[c].vec3Pos.z) ||
                          (vertex_list[c].vec3Pos.x == vertex_list[a].vec3Pos.x &&
                           vertex_list[c].vec3Pos.y == vertex_list[a].vec3Pos.y &&
                           vertex_list[c].vec3Pos.z == vertex_list[a].vec3Pos.z)))
                    {
                        if (n_inMesh % 2 == 0)
                        {
                            indices.Add((short)(c));
                            indices.Add((short)(b));
                            indices.Add((short)(a));
                        }
                        else
                        {
                            indices.Add((short)(a));
                            indices.Add((short)(b));
                            indices.Add((short)(c));
                        }
                    }

                }

                // meshごとのインデックス数を記録
                material_list.material_list[n_mesh++].ulNumIndices = indices.Count - prevNumIndices;
                prevNumIndices = indices.Count;
                prevNumVertices = vertex_list.Count;
            }
            // -----------------------------------------------------
            // リストを配列に代入し直す
            // 頂点情報
            pmd.number_of_vertex = vertex_list.Count;
            pmd.pmd_vertex = (PMD_Vertex[])vertex_list.ToArray();
            // 頂点インデックス
            pmd.number_of_indices = indices.Count;
            pmd.indices_array = (short[])indices.ToArray();
            // マテリアル
            if (merge_flag == true) material_list.MergeMaterials();
            pmd.number_of_materials = material_list.material_list.Count;
            pmd.pmd_material = (PMD_Material[])material_list.material_list.ToArray();
            // Toonテクスチャファイル名
            pmd.toon_file_name = material_list.GetToonFileNameList();
        }

        // 表情に関連するboneに影響を受ける頂点を数え上げる
        private int CalcNumFaceVertices(TMOFile tmo)
        {
            int n_vertex = 0; // 表情の頂点の番号（通し番号）
            int n_inList = -1; // list中のvertexの番号（処理の前に++するために、初期値は0でなく-1としている)
            foreach (TSOSubMesh sub_mesh in mesh_list)
            {
                int n_inMesh = -1; // mesh中のvertexの番号（処理の前に++するために、初期値は0でなく-1としている)
                foreach (Vertex vertex in sub_mesh.vertices)
                {
                    n_inList++; // list中のvertexの番号を一つ増やす
                    n_inMesh++; // mesh中のvertexの番号を一つ増やす
                    int idx = inList_indices[n_inList];

                    if (idx == -1)
                        continue;

                    PMD_Vertex pmd_v = pmd.pmd_vertex[idx];

                    // -----------------------------------------------------
                    // 表情情報

                    // 表情に関連するboneに影響を受ける頂点であれば、情報を記憶する
                    foreach (SkinWeight skin_w in vertex.skin_weights)
                    {
                        // 表情に関連するboneに影響を受ける頂点であれば、表情用の頂点とする
                        if (FACE_BONE_MIN <= sub_mesh.bone_indices[skin_w.bone_index]
                            && sub_mesh.bone_indices[skin_w.bone_index] <= FACE_BONE_MAX)
                        {
                            n_vertex++;
                            break;
                        }
                    }
                }
            }
            return n_vertex;
        }

        // デフォルト表情を設定
        private void MakePMDBaseFace()
        {
            int n_vertex = 0; // 表情の頂点の番号（通し番号）
            int n_inList = -1; // list中のvertexの番号（処理の前に++するために、初期値は0でなく-1としている)
            foreach (TSOSubMesh sub_mesh in mesh_list)
            {
                int n_inMesh = -1; // mesh中のvertexの番号（処理の前に++するために、初期値は0でなく-1としている)
                foreach (Vertex vertex in sub_mesh.vertices)
                {
                    n_inList++; // list中のvertexの番号を一つ増やす
                    n_inMesh++; // mesh中のvertexの番号を一つ増やす
                    int idx = inList_indices[n_inList];
                    if (idx == -1)
                        continue;
                    PMD_Vertex pmd_v = pmd.pmd_vertex[idx];

                    // -----------------------------------------------------
                    // 表情情報

                    // 表情に関連するboneに影響を受ける頂点であれば、情報を記憶する
                    foreach (SkinWeight skin_w in vertex.skin_weights)
                    {
                        // 表情に関連するboneに影響を受ける頂点であれば、表情用の頂点とする
                        if (FACE_BONE_MIN <= sub_mesh.bone_indices[skin_w.bone_index]
                            && sub_mesh.bone_indices[skin_w.bone_index] <= FACE_BONE_MAX)
                        {
                            // 表情の頂点情報（base）
                            pmd.pmd_face[0].pVertices[n_vertex].ulIndex = idx; // 表情用の頂点の番号(頂点リストにある番号)
                            pmd.pmd_face[0].pVertices[n_vertex].vec3Pos = pmd_v.vec3Pos;

                            n_vertex++;
                            break;
                        }
                    }
                }
            }
        }

        // 表情モーフを設定
        private void MakePMDFaces()
        {
            int n_vertex = 0; // 表情の頂点の番号（通し番号）
            int n_inList = -1; // list中のvertexの番号（処理の前に++するために、初期値は0でなく-1としている)
            // -----------------------------------------------------
            // 表情情報（base以外）
            // -----------------------------------------------------
            List<Vector3[]> verPos_face = new List<Vector3[]>();

            foreach (TSOSubMesh sub_mesh in mesh_list)
            {
                int n_inMesh = -1; // mesh中のvertexの番号（処理の前に++するために、初期値は0でなく-1としている)
                verPos_face.Clear(); // 前回の分を消去

                foreach (MorphGroup mg in morph.Groups)
                {
                    foreach (Morph mi in mg.Items)
                    {
                        // 現在のモーフを有効にする
                        mi.Ratio = 1.0f;

                        // モーフ変形を実行
                        fig.TPOList.Transform();
                        morph.Morph(fig.Tmo);
                        fig.UpdateBoneMatricesWithoutTMOFrame();
                        
                        // 現在のモーフを無効にする
                        mi.Ratio = 0.0f;

                        Matrix[] clipped_boneMatrices_for_morphing = ClipBoneMatrices(sub_mesh, fig.Tmo);

                        // Tmo（各表情に対応する）中のBoneに従って、Mesh中の頂点位置を置き換える
                        Vector3[] output_v = new Vector3[sub_mesh.vertices.Length];
                        int n = 0;
                        foreach (Vertex vertex in sub_mesh.vertices)
                        {
                            Vector3 pos = Vector3.Empty;
                            Vector3 nor = Vector3.Empty;

                            foreach (SkinWeight sw in vertex.skin_weights)
                            {
                                // 頂点位置
                                Matrix m = clipped_boneMatrices_for_morphing[sw.bone_index];
                                pos += Vector3.TransformCoordinate(vertex.position, m) * sw.weight;
                            }

                            output_v[n++] = pos;
                        }
                        verPos_face.Add(output_v);
                    }

                    // モーフ変形を初期化する
                    fig.TPOList.Transform();
                    morph.Morph(fig.Tmo);
                    fig.UpdateBoneMatricesWithoutTMOFrame();
                }

                foreach (Vertex vertex in sub_mesh.vertices)
                {
                    n_inList++; // list中のvertexの番号を一つ増やす
                    n_inMesh++; // mesh中のvertexの番号を一つ増やす
                    int idx = inList_indices[n_inList];

                    if (idx == -1)
                        continue;

                    PMD_Vertex pmd_v = pmd.pmd_vertex[idx];

                    // 表情に関連するboneに影響を受ける頂点であれば、情報を記憶する
                    foreach (SkinWeight skin_w in vertex.skin_weights)
                    {
                        // 表情に関連するboneに影響を受ける頂点であれば、表情用の頂点とする
                        if (FACE_BONE_MIN <= sub_mesh.bone_indices[skin_w.bone_index]
                            && sub_mesh.bone_indices[skin_w.bone_index] <= FACE_BONE_MAX)
                        {
                            // 表情の頂点情報（base以外）
                            for (int i = 1; i < pmd.number_of_face; i++)
                            {
                                // 表情用の頂点の番号(baseの番号。skin_vert_index)
                                pmd.pmd_face[i].pVertices[n_vertex].ulIndex = n_vertex;

                                // bace以外は相対位置で指定
                                MmdVector3 pmd_face_pos = Trans.CopyPos(verPos_face[i - 1][n_inMesh]);
                                pmd.pmd_face[i].pVertices[n_vertex].vec3Pos.x = pmd_face_pos.x - pmd_v.vec3Pos.x;
                                pmd.pmd_face[i].pVertices[n_vertex].vec3Pos.y = pmd_face_pos.y - pmd_v.vec3Pos.y;
                                pmd.pmd_face[i].pVertices[n_vertex].vec3Pos.z = pmd_face_pos.z - pmd_v.vec3Pos.z;
                            }

                            n_vertex++;
                            break;
                        }
                    }

                }
            }
        }

        // にっこりさせる
        public void NikkoriFace()
        {
            foreach (MorphGroup mg in morph.Groups)
            {
                if (mg.Name == "まゆ")
                {
                    foreach (Morph mi in mg.Items)
                    {
                        if (mi.Name == "にこり")
                        {
                            // 現在のモーフを有効にする
                            mi.Ratio = 1.0f;
                            break;
                        }
                    }
                }
                if (mg.Name == "目")
                {
                    foreach (Morph mi in mg.Items)
                    {
                        if (mi.Name == "笑い")
                        {
                            // 現在のモーフを有効にする
                            mi.Ratio = 1.0f;
                            break;
                        }
                    }
                }
                else if (mg.Name == "リップ")
                {
                    foreach (Morph mi in mg.Items)
                    {
                        if (mi.Name == "わーい")
                        {
                            // 現在のモーフを有効にする
                            mi.Ratio = 1.0f;
                            break;
                        }
                    }
                }
            }

            // モーフ変形を実行
            fig.TPOList.Transform();
            morph.Morph(fig.Tmo);
            fig.UpdateBoneMatricesWithoutTMOFrame();
        }

        // 初期の表情にする
        public void DefaultFace()
        {
            foreach (MorphGroup mg in morph.Groups)
            foreach (Morph mi in mg.Items)
                mi.Ratio = 0.0f;

            // モーフ変形を実行
            fig.TPOList.Transform();
            morph.Morph(fig.Tmo);
            fig.UpdateBoneMatricesWithoutTMOFrame();
        }

        private void MakeKamiPhysObject()
        {
            // (151, kami_Back_Mid1) - (155, kami_Back_Mid4_End)
            physOb_list.MakeChain("中髪後１", 0);
            physOb_list.GetBodyByName("中髪後１").rigidbody_type = 0; // 諸データ：タイプ(0:Bone追従、1:物理演算、2:物理演算(Bone位置合せ)) // 00 // Bone追従
            physOb_list.GetBodyByName("中髪後２").rigidbody_type = 0; // 諸データ：タイプ(0:Bone追従、1:物理演算、2:物理演算(Bone位置合せ)) // 00 // Bone追従
            physOb_list.GetJointByName("中髪後２-中髪後３").constrain_rot_1.x = (float)((-5.0 / 180.0) * Math.PI); // 制限：回転1(rad(x), rad(y), rad(z))
            physOb_list.GetJointByName("中髪後２-中髪後３").constrain_rot_2.x = (float)((30.0 / 180.0) * Math.PI); // 制限：回転2(rad(x), rad(y), rad(z))
            physOb_list.GetJointByName("中髪後３-中髪後４").constrain_rot_1.x = (float)((-5.0 / 180.0) * Math.PI); // 制限：回転1(rad(x), rad(y), rad(z))
            physOb_list.GetJointByName("中髪後３-中髪後４").constrain_rot_2.x = (float)((30.0 / 180.0) * Math.PI); // 制限：回転2(rad(x), rad(y), rad(z))

            // (156, kami_Front_L1) - (160, kami_Front_L4_End)
            physOb_list.MakeChain("左髪横１", 0);
            physOb_list.GetBodyByName("左髪横１").rigidbody_type = 0; // 諸データ：タイプ(0:Bone追従、1:物理演算、2:物理演算(Bone位置合せ)) // 00 // Bone追従
            physOb_list.GetBodyByName("左髪横２").rigidbody_type = 0; // 諸データ：タイプ(0:Bone追従、1:物理演算、2:物理演算(Bone位置合せ)) // 00 // Bone追従
            physOb_list.GetBodyByName("左髪横３").rigidbody_type = 0; // 諸データ：タイプ(0:Bone追従、1:物理演算、2:物理演算(Bone位置合せ)) // 00 // Bone追従

            // (161, kami_Front_R1) - (165, kami_Front_R4_End)
            physOb_list.MakeChain("右髪横１", 0);
            physOb_list.GetBodyByName("右髪横１").rigidbody_type = 0; // 諸データ：タイプ(0:Bone追従、1:物理演算、2:物理演算(Bone位置合せ)) // 00 // Bone追従
            physOb_list.GetBodyByName("右髪横２").rigidbody_type = 0; // 諸データ：タイプ(0:Bone追従、1:物理演算、2:物理演算(Bone位置合せ)) // 00 // Bone追従
            physOb_list.GetBodyByName("右髪横３").rigidbody_type = 0; // 諸データ：タイプ(0:Bone追従、1:物理演算、2:物理演算(Bone位置合せ)) // 00 // Bone追従

            // (166, kami_Back_L1) - (170, kami_Back_L4_End)
            physOb_list.MakeChain("左髪後１", 0);
            physOb_list.GetBodyByName("左髪後１").rigidbody_type = 0; // 諸データ：タイプ(0:Bone追従、1:物理演算、2:物理演算(Bone位置合せ)) // 00 // Bone追従
            physOb_list.GetBodyByName("左髪後２").rigidbody_type = 0; // 諸データ：タイプ(0:Bone追従、1:物理演算、2:物理演算(Bone位置合せ)) // 00 // Bone追従
            physOb_list.GetJointByName("左髪後２-左髪後３").constrain_rot_1.x = (float)((-5.0 / 180.0) * Math.PI); // 制限：回転1(rad(x), rad(y), rad(z))
            physOb_list.GetJointByName("左髪後２-左髪後３").constrain_rot_2.x = (float)((30.0 / 180.0) * Math.PI); // 制限：回転2(rad(x), rad(y), rad(z))
            physOb_list.GetJointByName("左髪後３-左髪後４").constrain_rot_1.x = (float)((-5.0 / 180.0) * Math.PI); // 制限：回転1(rad(x), rad(y), rad(z))
            physOb_list.GetJointByName("左髪後３-左髪後４").constrain_rot_2.x = (float)((30.0 / 180.0) * Math.PI); // 制限：回転2(rad(x), rad(y), rad(z))

            // (171, kami_Back_R1) - (175, kami_Back_R4_End)
            physOb_list.MakeChain("右髪後１", 0);
            physOb_list.GetBodyByName("右髪後１").rigidbody_type = 0; // 諸データ：タイプ(0:Bone追従、1:物理演算、2:物理演算(Bone位置合せ)) // 00 // Bone追従
            physOb_list.GetBodyByName("右髪後２").rigidbody_type = 0; // 諸データ：タイプ(0:Bone追従、1:物理演算、2:物理演算(Bone位置合せ)) // 00 // Bone追従
            physOb_list.GetJointByName("右髪後２-右髪後３").constrain_rot_1.x = (float)((-5.0 / 180.0) * Math.PI); // 制限：回転1(rad(x), rad(y), rad(z))
            physOb_list.GetJointByName("右髪後２-右髪後３").constrain_rot_2.x = (float)((30.0 / 180.0) * Math.PI); // 制限：回転2(rad(x), rad(y), rad(z))
            physOb_list.GetJointByName("右髪後３-右髪後４").constrain_rot_1.x = (float)((-5.0 / 180.0) * Math.PI); // 制限：回転1(rad(x), rad(y), rad(z))
            physOb_list.GetJointByName("右髪後３-右髪後４").constrain_rot_2.x = (float)((30.0 / 180.0) * Math.PI); // 制限：回転2(rad(x), rad(y), rad(z))

            // (176, kami_Front_Mid1_L) - (179, kami_Front_Mid3_End_L)
            physOb_list.MakeChain("左髪前１", 0);
            physOb_list.GetBodyByName("左髪前１").rigidbody_type = 0; // 諸データ：タイプ(0:Bone追従、1:物理演算、2:物理演算(Bone位置合せ)) // 00 // Bone追従
            physOb_list.GetBodyByName("左髪前２").rigidbody_type = 0; // 諸データ：タイプ(0:Bone追従、1:物理演算、2:物理演算(Bone位置合せ)) // 00 // Bone追従
            physOb_list.GetBodyByName("左髪前３").rigidbody_type = 0; // 諸データ：タイプ(0:Bone追従、1:物理演算、2:物理演算(Bone位置合せ)) // 00 // Bone追従
            physOb_list.GetBodyByName("左髪前先").rigidbody_type = 0; // 諸データ：タイプ(0:Bone追従、1:物理演算、2:物理演算(Bone位置合せ)) // 00 // Bone追従

            // (180, kami_Front_Mid1_R) - (183, kami_Front_Mid3_End_R)
            physOb_list.MakeChain("右髪前１", 0);
            physOb_list.GetBodyByName("右髪前１").rigidbody_type = 0; // 諸データ：タイプ(0:Bone追従、1:物理演算、2:物理演算(Bone位置合せ)) // 00 // Bone追従
            physOb_list.GetBodyByName("右髪前２").rigidbody_type = 0; // 諸データ：タイプ(0:Bone追従、1:物理演算、2:物理演算(Bone位置合せ)) // 00 // Bone追従
            physOb_list.GetBodyByName("右髪前３").rigidbody_type = 0; // 諸データ：タイプ(0:Bone追従、1:物理演算、2:物理演算(Bone位置合せ)) // 00 // Bone追従
            physOb_list.GetBodyByName("右髪前先").rigidbody_type = 0; // 諸データ：タイプ(0:Bone追従、1:物理演算、2:物理演算(Bone位置合せ)) // 00 // Bone追従

            if (kami_flag == 2)
            {
                foreach (PMD_RigidBody rb in physOb_list.body_list)
                {
                    if (rb.rigidbody_name.Length >= 2)
                    if (rb.rigidbody_name[1] == '髪')
                    {
                        rb.rigidbody_weight = 1.0f;
                        rb.rigidbody_pos_dim = 0.9f;
                        rb.rigidbody_rot_dim = 0.9f;
                    }
                }
            }
        }

        private void MakeChichiPhysObject()
        {
            // (184, Chichi_Right1) - (189, Chichi_Right5_end)
            physOb_list.MakeChain("右乳１", 1);
            physOb_list.GetBodyByName("右乳１").rigidbody_type = 0; // 諸データ：タイプ(0:Bone追従、1:物理演算、2:物理演算(Bone位置合せ)) // 00 // Bone追従

            // (190, Chichi_Left1) - (195, Chichi_Left5_End)
            physOb_list.MakeChain("左乳１", 1);
            physOb_list.GetBodyByName("左乳１").rigidbody_type = 0; // 諸データ：タイプ(0:Bone追従、1:物理演算、2:物理演算(Bone位置合せ)) // 00 // Bone追従

            if (chichi_flag == 1)
            {
                physOb_list.GetBodyByName("右乳２").rigidbody_type = 0; // 諸データ：タイプ(0:Bone追従、1:物理演算、2:物理演算(Bone位置合せ)) // 00 // Bone追従
                physOb_list.GetBodyByName("左乳２").rigidbody_type = 0; // 諸データ：タイプ(0:Bone追従、1:物理演算、2:物理演算(Bone位置合せ)) // 00 // Bone追従
            }
            if (chichi_flag == 3)
            {
                physOb_list.GetJointByName("右乳１-右乳２").constrain_rot_1.x = (float)((-10.0 / 180.0) * Math.PI); // 制限：回転1(rad(x), rad(y), rad(z))
                physOb_list.GetJointByName("右乳１-右乳２").constrain_rot_2.x = (float)((10.0 / 180.0) * Math.PI); // 制限：回転2(rad(x), rad(y), rad(z))
                physOb_list.GetJointByName("右乳１-右乳２").constrain_rot_1.y = (float)((-10.0 / 180.0) * Math.PI); // 制限：回転1(rad(x), rad(y), rad(z))
                physOb_list.GetJointByName("右乳１-右乳２").constrain_rot_2.y = (float)((10.0 / 180.0) * Math.PI); // 制限：回転2(rad(x), rad(y), rad(z))
                physOb_list.GetJointByName("右乳１-右乳２").constrain_rot_1.z = (float)((-10.0 / 180.0) * Math.PI); // 制限：回転1(rad(x), rad(y), rad(z))
                physOb_list.GetJointByName("右乳１-右乳２").constrain_rot_2.z = (float)((10.0 / 180.0) * Math.PI); // 制限：回転2(rad(x), rad(y), rad(z))

                physOb_list.GetJointByName("左乳１-左乳２").constrain_rot_1.x = (float)((-10.0 / 180.0) * Math.PI); // 制限：回転1(rad(x), rad(y), rad(z))
                physOb_list.GetJointByName("左乳１-左乳２").constrain_rot_2.x = (float)((10.0 / 180.0) * Math.PI); // 制限：回転2(rad(x), rad(y), rad(z))
                physOb_list.GetJointByName("左乳１-左乳２").constrain_rot_1.y = (float)((-10.0 / 180.0) * Math.PI); // 制限：回転1(rad(x), rad(y), rad(z))
                physOb_list.GetJointByName("左乳１-左乳２").constrain_rot_2.y = (float)((10.0 / 180.0) * Math.PI); // 制限：回転2(rad(x), rad(y), rad(z))
                physOb_list.GetJointByName("左乳１-左乳２").constrain_rot_1.z = (float)((-10.0 / 180.0) * Math.PI); // 制限：回転1(rad(x), rad(y), rad(z))
                physOb_list.GetJointByName("左乳１-左乳２").constrain_rot_2.z = (float)((10.0 / 180.0) * Math.PI); // 制限：回転2(rad(x), rad(y), rad(z))

                physOb_list.GetJointByName("右乳２-右乳３").constrain_rot_1.x = (float)((-10.0 / 180.0) * Math.PI); // 制限：回転1(rad(x), rad(y), rad(z))
                physOb_list.GetJointByName("右乳２-右乳３").constrain_rot_2.x = (float)((10.0 / 180.0) * Math.PI); // 制限：回転2(rad(x), rad(y), rad(z))
                physOb_list.GetJointByName("右乳２-右乳３").constrain_rot_1.y = (float)((-10.0 / 180.0) * Math.PI); // 制限：回転1(rad(x), rad(y), rad(z))
                physOb_list.GetJointByName("右乳２-右乳３").constrain_rot_2.y = (float)((10.0 / 180.0) * Math.PI); // 制限：回転2(rad(x), rad(y), rad(z))
                physOb_list.GetJointByName("右乳２-右乳３").constrain_rot_1.z = (float)((-10.0 / 180.0) * Math.PI); // 制限：回転1(rad(x), rad(y), rad(z))
                physOb_list.GetJointByName("右乳２-右乳３").constrain_rot_2.z = (float)((10.0 / 180.0) * Math.PI); // 制限：回転2(rad(x), rad(y), rad(z))

                physOb_list.GetJointByName("左乳２-左乳３").constrain_rot_1.x = (float)((-10.0 / 180.0) * Math.PI); // 制限：回転1(rad(x), rad(y), rad(z))
                physOb_list.GetJointByName("左乳２-左乳３").constrain_rot_2.x = (float)((10.0 / 180.0) * Math.PI); // 制限：回転2(rad(x), rad(y), rad(z))
                physOb_list.GetJointByName("左乳２-左乳３").constrain_rot_1.y = (float)((-10.0 / 180.0) * Math.PI); // 制限：回転1(rad(x), rad(y), rad(z))
                physOb_list.GetJointByName("左乳２-左乳３").constrain_rot_2.y = (float)((10.0 / 180.0) * Math.PI); // 制限：回転2(rad(x), rad(y), rad(z))
                physOb_list.GetJointByName("左乳２-左乳３").constrain_rot_1.z = (float)((-10.0 / 180.0) * Math.PI); // 制限：回転1(rad(x), rad(y), rad(z))
                physOb_list.GetJointByName("左乳２-左乳３").constrain_rot_2.z = (float)((10.0 / 180.0) * Math.PI); // 制限：回転2(rad(x), rad(y), rad(z))
            }
        }

        private void MakeSkirtPhysObject()
        {
            // (196, skirt_LeftB01) - (199, skirt_LeftB03_end)
            physOb_list.MakeChain("左ス後１", 2);
            physOb_list.GetJointByName("下半身-左ス後１").constrain_rot_1.x = (float)((-0.0 / 180.0) * Math.PI); // 制限：回転1(rad(x), rad(y), rad(z))
            physOb_list.GetJointByName("下半身-左ス後１").constrain_rot_2.x = (float)((60.0 / 180.0) * Math.PI); // 制限：回転2(rad(x), rad(y), rad(z))
            physOb_list.GetJointByName("下半身-左ス後１").constrain_rot_1.y = (float)((-0.0 / 180.0) * Math.PI); // 制限：回転1(rad(x), rad(y), rad(z))
            physOb_list.GetJointByName("下半身-左ス後１").constrain_rot_2.y = (float)((0.0 / 180.0) * Math.PI); // 制限：回転2(rad(x), rad(y), rad(z))
            physOb_list.GetJointByName("下半身-左ス後１").constrain_rot_1.z = (float)((-5.0 / 180.0) * Math.PI); // 制限：回転1(rad(x), rad(y), rad(z))
            physOb_list.GetJointByName("下半身-左ス後１").constrain_rot_2.z = (float)((5.0 / 180.0) * Math.PI); // 制限：回転2(rad(x), rad(y), rad(z))
            physOb_list.GetJointByName("下半身-左ス後１").spring_rot.x = 50.0f; // ばね：回転(rad(x), rad(y), rad(z))
            physOb_list.GetJointByName("下半身-左ス後１").spring_rot.y = 0.0f; // ばね：回転(rad(x), rad(y), rad(z))
            physOb_list.GetJointByName("下半身-左ス後１").spring_rot.z = 0.0f; // ばね：回転(rad(x), rad(y), rad(z))

            // (200, skirt_RightB01) - (203, skirt_RightB03_end)
            physOb_list.MakeChain("右ス後１", 2);
            physOb_list.GetJointByName("下半身-右ス後１").constrain_rot_1.x = (float)((-0.0 / 180.0) * Math.PI); // 制限：回転1(rad(x), rad(y), rad(z))
            physOb_list.GetJointByName("下半身-右ス後１").constrain_rot_2.x = (float)((60.0 / 180.0) * Math.PI); // 制限：回転2(rad(x), rad(y), rad(z))
            physOb_list.GetJointByName("下半身-右ス後１").constrain_rot_1.y = (float)((-0.0 / 180.0) * Math.PI); // 制限：回転1(rad(x), rad(y), rad(z))
            physOb_list.GetJointByName("下半身-右ス後１").constrain_rot_2.y = (float)((0.0 / 180.0) * Math.PI); // 制限：回転2(rad(x), rad(y), rad(z))
            physOb_list.GetJointByName("下半身-右ス後１").constrain_rot_1.z = (float)((-5.0 / 180.0) * Math.PI); // 制限：回転1(rad(x), rad(y), rad(z))
            physOb_list.GetJointByName("下半身-右ス後１").constrain_rot_2.z = (float)((5.0 / 180.0) * Math.PI); // 制限：回転2(rad(x), rad(y), rad(z))
            physOb_list.GetJointByName("下半身-右ス後１").spring_rot.x = 50.0f; // ばね：回転(rad(x), rad(y), rad(z))
            physOb_list.GetJointByName("下半身-右ス後１").spring_rot.y = 0.0f; // ばね：回転(rad(x), rad(y), rad(z))
            physOb_list.GetJointByName("下半身-右ス後１").spring_rot.z = 0.0f; // ばね：回転(rad(x), rad(y), rad(z))

            // (204, skirt_RightS01) - (207, skirt_RightS03_end)
            physOb_list.MakeChain("右ス横１", 3);
            physOb_list.GetJointByName("下半身-右ス横１").constrain_rot_1.x = (float)((-5.0 / 180.0) * Math.PI); // 制限：回転1(rad(x), rad(y), rad(z))
            physOb_list.GetJointByName("下半身-右ス横１").constrain_rot_2.x = (float)((5.0 / 180.0) * Math.PI); // 制限：回転2(rad(x), rad(y), rad(z))
            physOb_list.GetJointByName("下半身-右ス横１").constrain_rot_1.y = (float)((-0.0 / 180.0) * Math.PI); // 制限：回転1(rad(x), rad(y), rad(z))
            physOb_list.GetJointByName("下半身-右ス横１").constrain_rot_2.y = (float)((0.0 / 180.0) * Math.PI); // 制限：回転2(rad(x), rad(y), rad(z))
            physOb_list.GetJointByName("下半身-右ス横１").constrain_rot_1.z = (float)((-15.0 / 180.0) * Math.PI); // 制限：回転1(rad(x), rad(y), rad(z))
            physOb_list.GetJointByName("下半身-右ス横１").constrain_rot_2.z = (float)((60.0 / 180.0) * Math.PI); // 制限：回転2(rad(x), rad(y), rad(z))
            physOb_list.GetJointByName("下半身-右ス横１").spring_rot.x = 0.0f; // ばね：回転(rad(x), rad(y), rad(z))
            physOb_list.GetJointByName("下半身-右ス横１").spring_rot.y = 0.0f; // ばね：回転(rad(x), rad(y), rad(z))
            physOb_list.GetJointByName("下半身-右ス横１").spring_rot.z = 50.0f; // ばね：回転(rad(x), rad(y), rad(z))

            // (208, skirt_RightF01) - (211, skirt_RightF03_end)
            physOb_list.MakeChain("右ス前１", 2);
            physOb_list.GetJointByName("下半身-右ス前１").constrain_rot_1.x = (float)((-120.0 / 180.0) * Math.PI); // 制限：回転1(rad(x), rad(y), rad(z))
            physOb_list.GetJointByName("下半身-右ス前１").constrain_rot_2.x = (float)((20.0 / 180.0) * Math.PI); // 制限：回転2(rad(x), rad(y), rad(z))
            physOb_list.GetJointByName("下半身-右ス前１").constrain_rot_1.y = (float)((-0.0 / 180.0) * Math.PI); // 制限：回転1(rad(x), rad(y), rad(z))
            physOb_list.GetJointByName("下半身-右ス前１").constrain_rot_2.y = (float)((0.0 / 180.0) * Math.PI); // 制限：回転2(rad(x), rad(y), rad(z))
            physOb_list.GetJointByName("下半身-右ス前１").constrain_rot_1.z = (float)((-5.0 / 180.0) * Math.PI); // 制限：回転1(rad(x), rad(y), rad(z))
            physOb_list.GetJointByName("下半身-右ス前１").constrain_rot_2.z = (float)((5.0 / 180.0) * Math.PI); // 制限：回転2(rad(x), rad(y), rad(z))
            physOb_list.GetJointByName("下半身-右ス前１").spring_rot.x = 50.0f; // ばね：回転(rad(x), rad(y), rad(z))
            physOb_list.GetJointByName("下半身-右ス前１").spring_rot.y = 0.0f; // ばね：回転(rad(x), rad(y), rad(z))
            physOb_list.GetJointByName("下半身-右ス前１").spring_rot.z = 0.0f; // ばね：回転(rad(x), rad(y), rad(z))

            // (212, skirt_LeftF01) - (215, skirt_LeftF03_end)
            physOb_list.MakeChain("左ス前１", 2);
            physOb_list.GetJointByName("下半身-左ス前１").constrain_rot_1.x = (float)((-120.0 / 180.0) * Math.PI); // 制限：回転1(rad(x), rad(y), rad(z))
            physOb_list.GetJointByName("下半身-左ス前１").constrain_rot_2.x = (float)((20.0 / 180.0) * Math.PI); // 制限：回転2(rad(x), rad(y), rad(z))
            physOb_list.GetJointByName("下半身-左ス前１").constrain_rot_1.y = (float)((-0.0 / 180.0) * Math.PI); // 制限：回転1(rad(x), rad(y), rad(z))
            physOb_list.GetJointByName("下半身-左ス前１").constrain_rot_2.y = (float)((0.0 / 180.0) * Math.PI); // 制限：回転2(rad(x), rad(y), rad(z))
            physOb_list.GetJointByName("下半身-左ス前１").constrain_rot_1.z = (float)((-5.0 / 180.0) * Math.PI); // 制限：回転1(rad(x), rad(y), rad(z))
            physOb_list.GetJointByName("下半身-左ス前１").constrain_rot_2.z = (float)((5.0 / 180.0) * Math.PI); // 制限：回転2(rad(x), rad(y), rad(z))
            physOb_list.GetJointByName("下半身-左ス前１").spring_rot.x = 50.0f; // ばね：回転(rad(x), rad(y), rad(z))
            physOb_list.GetJointByName("下半身-左ス前１").spring_rot.y = 0.0f; // ばね：回転(rad(x), rad(y), rad(z))
            physOb_list.GetJointByName("下半身-左ス前１").spring_rot.z = 0.0f; // ばね：回転(rad(x), rad(y), rad(z))

            // (216, skirt_LeftS01) - (219, skirt_LeftS03_end)
            physOb_list.MakeChain("左ス横１", 3);
            physOb_list.GetJointByName("下半身-左ス横１").constrain_rot_1.x = (float)((-5.0 / 180.0) * Math.PI); // 制限：回転1(rad(x), rad(y), rad(z))
            physOb_list.GetJointByName("下半身-左ス横１").constrain_rot_2.x = (float)((5.0 / 180.0) * Math.PI); // 制限：回転2(rad(x), rad(y), rad(z))
            physOb_list.GetJointByName("下半身-左ス横１").constrain_rot_1.y = (float)((-0.0 / 180.0) * Math.PI); // 制限：回転1(rad(x), rad(y), rad(z))
            physOb_list.GetJointByName("下半身-左ス横１").constrain_rot_2.y = (float)((0.0 / 180.0) * Math.PI); // 制限：回転2(rad(x), rad(y), rad(z))
            physOb_list.GetJointByName("下半身-左ス横１").constrain_rot_1.z = (float)((-60.0 / 180.0) * Math.PI); // 制限：回転1(rad(x), rad(y), rad(z))
            physOb_list.GetJointByName("下半身-左ス横１").constrain_rot_2.z = (float)((15.0 / 180.0) * Math.PI); // 制限：回転2(rad(x), rad(y), rad(z))
            physOb_list.GetJointByName("下半身-左ス横１").spring_rot.x = 0.0f; // ばね：回転(rad(x), rad(y), rad(z))
            physOb_list.GetJointByName("下半身-左ス横１").spring_rot.y = 0.0f; // ばね：回転(rad(x), rad(y), rad(z))
            physOb_list.GetJointByName("下半身-左ス横１").spring_rot.z = 50.0f; // ばね：回転(rad(x), rad(y), rad(z))

            // スカートの先を隣同士でジョイントする
            physOb_list.MakeJointFromTwoBones("左ス前２", "右ス前２");
            physOb_list.MakeJointFromTwoBones("右ス前２", "右ス横２");
            physOb_list.MakeJointFromTwoBones("右ス横２", "右ス後２");
            physOb_list.MakeJointFromTwoBones("右ス後２", "左ス後２");
            physOb_list.MakeJointFromTwoBones("左ス後２", "左ス横２");
            physOb_list.MakeJointFromTwoBones("左ス横２", "左ス前２");
            physOb_list.MakeJointFromTwoBones("左ス前３", "右ス前３");
            physOb_list.MakeJointFromTwoBones("右ス前３", "右ス横３");
            physOb_list.MakeJointFromTwoBones("右ス横３", "右ス後３");
            physOb_list.MakeJointFromTwoBones("右ス後３", "左ス後３");
            physOb_list.MakeJointFromTwoBones("左ス後３", "左ス横３");
            physOb_list.MakeJointFromTwoBones("左ス横３", "左ス前３");

            if (skirt_flag == 2)
            {
                foreach (PMD_RigidBody rb in physOb_list.body_list)
                {
                    if (rb.rigidbody_name.Length >= 2)
                    if (rb.rigidbody_name[1] == 'ス')
                    {
                        rb.rigidbody_weight = 1.0f;
                        rb.rigidbody_pos_dim = 0.9f;
                    }
                }
            }
        }
    }
}
