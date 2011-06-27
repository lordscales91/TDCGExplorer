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
        PmdFile pmd = new PmdFile();

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
        TemplateList template_list;
        CorrespondTableList corTable_list;

        public Figure Figure { get { return fig; } set { fig = value; } }
        public PmdFile Pmd { get { return pmd; } }
        public int Bone_flag { set { bone_flag = value; } }
        public bool Spheremap_flag { set { spheremap_flag = value; } }
        public bool Edge_flag_flag { set { edge_flag_flag = value; } }
        public bool Merge_flag { set { merge_flag = value; } }
        public List<string> Category { set { category = value; } }
        public List<bool> Meshes_flag { set { meshes_flag = value; } }
        public TemplateList TemplateList { set { template_list = value; } }
        public CorrespondTableList CorTableList { set { corTable_list = value; } }
          
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
            pmd.pmd_header = new TDCGUtils.PMD_Header();
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
            MakePMDVertices(null, 2);

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
            pmd.pmd_bone = new TDCGUtils.PMD_Bone[pmd.number_of_bone];

            // センター
            pmd.pmd_bone[0] = new TDCGUtils.PMD_Bone();
            pmd.pmd_bone[0].szName = "センター";
            pmd.pmd_bone[0].cbKind = 1; // ボーンの種類 0:回転 1:回転と移動 2:IK 3:不明 4:IK影響下 5:回転影響下 6:IK接続先 7:非表示 8:捻り 9:回転運動
            pmd.pmd_bone[0].ParentName = null;
            pmd.pmd_bone[0].ChildName = "センター先";
            pmd.pmd_bone[0].IKTargetName = null;
            pmd.pmd_bone[0].vec3Position.X = 0.0f;	// モデル原点からの位置
            pmd.pmd_bone[0].vec3Position.Y = 5.0f;	// モデル原点からの位置
            pmd.pmd_bone[0].vec3Position.Z = 0.0f;	// モデル原点からの位置

            // センター先
            pmd.pmd_bone[1] = new TDCGUtils.PMD_Bone();
            pmd.pmd_bone[1].szName = "センター先";
            pmd.pmd_bone[1].cbKind = 7; // ボーンの種類 0:回転 1:回転と移動 2:IK 3:不明 4:IK影響下 5:回転影響下 6:IK接続先 7:非表示 8:捻り 9:回転運動
            pmd.pmd_bone[1].ParentName = "センター";
            pmd.pmd_bone[1].ChildName = null;
            pmd.pmd_bone[1].IKTargetName = null;
            pmd.pmd_bone[1].vec3Position.X = 0.0f;	// モデル原点からの位置
            pmd.pmd_bone[1].vec3Position.Y = 0.0f;	// モデル原点からの位置
            pmd.pmd_bone[1].vec3Position.Z = 0.0f;	// モデル原点からの位置

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
            pmd.bone_disp = new TDCGUtils.PMD_BoneDisp[pmd.bone_disp_count]; // 枠用ボーンデータ (3Bytes/bone)
            pmd.bone_disp[0] = new TDCGUtils.PMD_BoneDisp();
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
                corTable_list.SetManFlag = false;
                cor_table = corTable_list.GetCorrespondTable();
                mod_type = 0;
            }
            else if (fig.Tmo.nodes.Length == 75)
            {
                corTable_list.SetManFlag = true;
                cor_table = corTable_list.GetCorrespondTable();
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
            MakePMDVertices(cor_table, mod_type);

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
            List<TDCGUtils.PMD_Bone> bone_list = new List<TDCGUtils.PMD_Bone>();

            foreach (KeyValuePair<string, TDCGUtils.PMD_Bone> bone_kvp in cor_table.boneStructure)
            {
                TDCGUtils.PMD_Bone pmd_b = new TDCGUtils.PMD_Bone();
                TDCGUtils.PMD_Bone bone = bone_kvp.Value;

                pmd_b.szName = bone.szName;
                pmd_b.cbKind = bone.cbKind; // ボーンの種類 0:回転 1:回転と移動 2:IK 3:不明 4:IK影響下 5:回転影響下 6:IK接続先 7:非表示 8:捻り 9:回転運動
                pmd_b.ParentName = bone.ParentName;
                pmd_b.ChildName = bone.ChildName;
                pmd_b.IKTargetName = bone.IKTargetName;

                string bone_name = null;
                cor_table.bonePosition.TryGetValue(pmd_b.szName, out bone_name);
                if (bone_name != null)
                {
                    pmd_b.vec3Position
                        = Trans.CopyMat2Pos(fig.Tmo.FindNodeByName(bone_name).combined_matrix); // モデル原点からの位置
                }

                bone_list.Add(pmd_b);
            }

            // -----------------------------------------------------
            // 親と子の前後関係を並び替える
            for (int i = 0; i < bone_list.Count; i++)
            for (int j = 0; j < bone_list.Count; j++)
            {
                if (bone_list[i].szName == bone_list[j].ParentName)
                if (i > j)
                {
                    bone_list.Insert(j, bone_list[i]);
                    bone_list.RemoveAt(i+1);
                }
            }

            // -----------------------------------------------------
            // リストを配列に代入し直す
            pmd.number_of_bone = bone_list.Count;
            pmd.pmd_bone = (TDCGUtils.PMD_Bone[])bone_list.ToArray();

            // -----------------------------------------------------
            // センターボーンの位置調整
            pmd.getBoneByName("センター").vec3Position
                = new Vector3(
                    0.0f, 
                    pmd.getBoneByName("下半身").vec3Position.Y * 0.65f, 
                    0.0f);
            pmd.getBoneByName("センター先").vec3Position
                = new Vector3(
                    0.0f, 
                    0.0f, 
                    0.0f);

            // -----------------------------------------------------
            // 両目ボーンの位置調整
            if (mod_type == 0)
            {
                pmd.getBoneByName("両目").vec3Position
                    = new Vector3(
                        0.0f,
                        pmd.getBoneByName("左目").vec3Position.Y + pmd.getBoneByName("左目").vec3Position.X * 4.0f,
                        pmd.getBoneByName("左目").vec3Position.Z - pmd.getBoneByName("左目").vec3Position.X * 2.0f);
                pmd.getBoneByName("両目先").vec3Position
                    = new Vector3(
                        pmd.getBoneByName("両目").vec3Position.X,
                        pmd.getBoneByName("両目").vec3Position.Y,
                        pmd.getBoneByName("両目").vec3Position.Z - 1.0f);
            }

            // -----------------------------------------------------
            // IK先ボーンの位置調整
            pmd.getBoneByName("左足ＩＫ先").vec3Position
                = new Vector3(
                    pmd.getBoneByName("左足ＩＫ").vec3Position.X,
                    pmd.getBoneByName("左足ＩＫ").vec3Position.Y,
                    pmd.getBoneByName("左足ＩＫ").vec3Position.Z + 1.7f);
            pmd.getBoneByName("右足ＩＫ先").vec3Position
                = new Vector3(
                    pmd.getBoneByName("右足ＩＫ").vec3Position.X,
                    pmd.getBoneByName("右足ＩＫ").vec3Position.Y,
                    pmd.getBoneByName("右足ＩＫ").vec3Position.Z + 1.7f);

            pmd.getBoneByName("左つま先").vec3Position.Y = 0.0f;
            pmd.getBoneByName("左つま先ＩＫ").vec3Position.Y = 0.0f;
            pmd.getBoneByName("左つま先ＩＫ先").vec3Position
                = new Vector3(
                    pmd.getBoneByName("左つま先ＩＫ").vec3Position.X,
                    pmd.getBoneByName("左つま先ＩＫ").vec3Position.Y - 1.0f,
                    pmd.getBoneByName("左つま先ＩＫ").vec3Position.Z);

            pmd.getBoneByName("右つま先").vec3Position.Y = 0.0f;
            pmd.getBoneByName("右つま先ＩＫ").vec3Position.Y = 0.0f;
            pmd.getBoneByName("右つま先ＩＫ先").vec3Position
                = new Vector3(
                    pmd.getBoneByName("右つま先ＩＫ").vec3Position.X,
                    pmd.getBoneByName("右つま先ＩＫ").vec3Position.Y - 1.0f,
                    pmd.getBoneByName("右つま先ＩＫ").vec3Position.Z);
          
            // -----------------------------------------------------
            // IK配列
            // -----------------------------------------------------
            pmd.number_of_ik = cor_table.IKBone.Count;
            pmd.pmd_ik = (TDCGUtils.PMD_IK[])cor_table.IKBone.ToArray();

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
            pmd.bone_disp = new TDCGUtils.PMD_BoneDisp[pmd.bone_disp_count]; 

            int n = 0; // 通し番号
            int n_disp = 1;
            foreach (DispBoneGroup dbg in cor_table.dispBoneGroup)
            {
                foreach (string name in dbg.bone_name_list)
                {
                    pmd.bone_disp[n] = new TDCGUtils.PMD_BoneDisp();
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
            // 物理オブジェクトを生成
            physOb_list = new T2PPhysObjectList(bone_list);

            // -----------------------------------------------------
            // テンプレートを適用
            template_list.PhysObExecute(ref physOb_list);

            // -----------------------------------------------------
            // 剛体＆ジョイントを配列に代入し直す
            pmd.rigidbody_count = physOb_list.body_list.Count;
            pmd.rigidbody = (TDCGUtils.PMD_RigidBody[])physOb_list.body_list.ToArray();

            pmd.joint_count = physOb_list.joint_list.Count;
            pmd.joint = (TDCGUtils.PMD_Joint[])physOb_list.joint_list.ToArray();

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
            pmd.pmd_face = new TDCGUtils.PMD_FACE[pmd.number_of_face];

            // 表情に関連するboneに影響を受ける頂点を数え上げる
            int numFaceVertices = CalcNumFaceVertices(fig.Tmo);

            // baseの表情
            pmd.pmd_face[n_face++] = new TDCGUtils.PMD_FACE(numFaceVertices);

            // base以外の表情
            foreach (MorphGroup mg in morph.Groups)
            {
                foreach (Morph m in mg.Items)
                {
                    pmd.pmd_face[n_face] = new TDCGUtils.PMD_FACE(numFaceVertices);
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
        private void MakePMDVertices(CorrespondTable cor_table, int mod_type)
        {
            List<TDCGUtils.PMD_Vertex> vertex_list = new List<TDCGUtils.PMD_Vertex>();
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
                    TDCGUtils.PMD_Vertex pmd_v = new TDCGUtils.PMD_Vertex();

                    pmd_v.vec3Pos = Trans.CopyPos(pos);
                    pmd_v.vec3Normal = Trans.CopyPos(Vector3.Normalize(nor));
                    pmd_v.u = vertex.u;
                    pmd_v.v = vertex.v;

                    pmd_v.cbEdge = 0;

                    // -----------------------------------------------------
                    // スキニング
                    List<string> tmp_b = new List<string>();
                    List<float> tmp_w = new List<float>();

                    for (int i = 0; i < 4; i++)
                    {
                        TSONode tso_bone = sub_mesh.bones[(int)vertex.skin_weights[i].bone_index];
                        string bone_name;
                        if (mod_type == 0 || mod_type == 1)
                            bone_name = cor_table.skinning[tso_bone.Name];
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
                        if (vertex_list[i].vec3Pos.X == pmd_v.vec3Pos.X &&
                            vertex_list[i].vec3Pos.Y == pmd_v.vec3Pos.Y &&
                            vertex_list[i].vec3Pos.Z == pmd_v.vec3Pos.Z &&
                            vertex_list[i].vec3Normal.X == pmd_v.vec3Normal.X &&
                            vertex_list[i].vec3Normal.Y == pmd_v.vec3Normal.Y &&
                            vertex_list[i].vec3Normal.Z == pmd_v.vec3Normal.Z &&
                            vertex_list[i].u == pmd_v.u &&
                            vertex_list[i].v == pmd_v.v)
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
                        !((vertex_list[a].vec3Pos.X == vertex_list[b].vec3Pos.X &&
                           vertex_list[a].vec3Pos.Y == vertex_list[b].vec3Pos.Y &&
                           vertex_list[a].vec3Pos.Z == vertex_list[b].vec3Pos.Z) ||
                          (vertex_list[b].vec3Pos.X == vertex_list[c].vec3Pos.X &&
                           vertex_list[b].vec3Pos.Y == vertex_list[c].vec3Pos.Y &&
                           vertex_list[b].vec3Pos.Z == vertex_list[c].vec3Pos.Z) ||
                          (vertex_list[c].vec3Pos.X == vertex_list[a].vec3Pos.X &&
                           vertex_list[c].vec3Pos.Y == vertex_list[a].vec3Pos.Y &&
                           vertex_list[c].vec3Pos.Z == vertex_list[a].vec3Pos.Z)))
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
            pmd.pmd_vertex = (TDCGUtils.PMD_Vertex[])vertex_list.ToArray();
            // 頂点インデックス
            pmd.number_of_indices = indices.Count;
            pmd.indices_array = (short[])indices.ToArray();
            // マテリアル
            if (merge_flag == true) material_list.MergeMaterials();
            pmd.number_of_materials = material_list.material_list.Count;
            pmd.pmd_material = (TDCGUtils.PMD_Material[])material_list.material_list.ToArray();
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

                    TDCGUtils.PMD_Vertex pmd_v = pmd.pmd_vertex[idx];

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
                    TDCGUtils.PMD_Vertex pmd_v = pmd.pmd_vertex[idx];

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

                    TDCGUtils.PMD_Vertex pmd_v = pmd.pmd_vertex[idx];

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
                                Vector3 pmd_face_pos = Trans.CopyPos(verPos_face[i - 1][n_inMesh]);
                                pmd.pmd_face[i].pVertices[n_vertex].vec3Pos.X = pmd_face_pos.X - pmd_v.vec3Pos.X;
                                pmd.pmd_face[i].pVertices[n_vertex].vec3Pos.Y = pmd_face_pos.Y - pmd_v.vec3Pos.Y;
                                pmd.pmd_face[i].pVertices[n_vertex].vec3Pos.Z = pmd_face_pos.Z - pmd_v.vec3Pos.Z;
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
    }
}
