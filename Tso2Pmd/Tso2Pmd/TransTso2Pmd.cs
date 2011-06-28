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
            pmd.pmd_header = new PMD_Header();
            pmd.pmd_header.magic = "Pmd";
            pmd.pmd_header.version = 1.0f;

            if (name.Length > 9) return "モデル名が9文字を超えています。";
            pmd.pmd_header.name = name;

            if (comment.Length > 127) return "コメントが127文字を超えています。";
            pmd.pmd_header.comment = comment;

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
            if (pmd.vertices.Length > 65535)
                return "頂点数(" + pmd.vertices.Length.ToString() + ")が上限(65535)を超えています。";

            // -----------------------------------------------------
            // 表情枠
            // -----------------------------------------------------

            // -----------------------------------------------------
            // ボーン情報
            // -----------------------------------------------------
            pmd.nodes = new PMD_Bone[2];

            // センター
            pmd.nodes[0] = new PMD_Bone();
            pmd.nodes[0].name = "センター";
            // 1:回転と移動
            pmd.nodes[0].kind = 1;
            pmd.nodes[0].ParentName = null;
            pmd.nodes[0].TailName = "センター先";
            pmd.nodes[0].TargetName = null;
            pmd.nodes[0].position = new Vector3(0.0f, 5.0f, 0.0f);	// モデル原点からの位置

            // センター先
            pmd.nodes[1] = new PMD_Bone();
            pmd.nodes[1].name = "センター先";
            // 7:非表示
            pmd.nodes[1].kind = 7;
            pmd.nodes[1].ParentName = "センター";
            pmd.nodes[1].TailName = null;
            pmd.nodes[1].TargetName = null;
            pmd.nodes[1].position = new Vector3(0.0f, 0.0f, 0.0f);	// モデル原点からの位置

            // -----------------------------------------------------
            // IK配列
            // -----------------------------------------------------

            // -----------------------------------------------------
            // ボーン枠用枠名リスト
            // -----------------------------------------------------
            pmd.disp_names = new string[1]; // 枠名(50Bytes/枠)
            pmd.disp_names[0] = "センター" + Convert.ToChar(Convert.ToInt16("0A", 16));
            //PMDEditorを使う場合は、枠名を0x0A00で終わらせる必要があります(0x00のみだと表示されません)。

            // -----------------------------------------------------
            // ボーン枠用表示リスト
            // -----------------------------------------------------
            pmd.bone_disps = new PMD_BoneDisp[1]; // 枠用ボーンデータ (3Bytes/bone)
            pmd.bone_disps[0] = new PMD_BoneDisp();
            pmd.bone_disps[0].bone_name = "センター"; // 枠用ボーン名
            pmd.bone_disps[0].disp_group_id = 0; // 表示枠番号

            // -----------------------------------------------------
            // 英名対応(0:英名対応なし, 1:英名対応あり)
            // -----------------------------------------------------
            pmd.english_name_compatibility = 0;

            // -----------------------------------------------------
            // 剛体＆ジョイント
            // -----------------------------------------------------

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
            if (pmd.vertices.Length > 65535)
                return "頂点数(" + pmd.vertices.Length.ToString() + ")が上限(65535)を超えています。";

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
                /*
                pmd.number_of_skin = 0;
                */
            }

            // -----------------------------------------------------
            // 表情枠
            // -----------------------------------------------------
            if (mod_type == 0)
            {
                pmd.skin_disp_indices = new int[pmd.skins.Length - 1];
                for (int i = 0; i < pmd.skin_disp_indices.Length; i++)
                {
                    pmd.skin_disp_indices[i] = i + 1;
                }
            }
            else if (mod_type == 1)
            {
                pmd.skin_disp_indices = new int[0];
            }

            // -----------------------------------------------------
            // ボーン情報
            // -----------------------------------------------------
            List<PMD_Bone> bone_list = new List<PMD_Bone>();

            foreach (KeyValuePair<string, PMD_Bone> bone_kvp in cor_table.boneStructure)
            {
                PMD_Bone bone = bone_kvp.Value;
                PMD_Bone pmd_b = new PMD_Bone();

                pmd_b.name = bone.name;
                pmd_b.kind = bone.kind;
                pmd_b.ParentName = bone.ParentName;
                pmd_b.TailName = bone.TailName;
                pmd_b.TargetName = bone.TargetName;

                string bone_name = null;
                cor_table.bonePosition.TryGetValue(pmd_b.name, out bone_name);
                if (bone_name != null)
                {
                    pmd_b.position
                        = Trans.CopyMat2Pos(fig.Tmo.FindNodeByName(bone_name).combined_matrix); // モデル原点からの位置
                }

                bone_list.Add(pmd_b);
            }

            // -----------------------------------------------------
            // 親と子の前後関係を並び替える
            for (int i = 0; i < bone_list.Count; i++)
            for (int j = 0; j < bone_list.Count; j++)
            {
                if (bone_list[i].name == bone_list[j].ParentName)
                if (i > j)
                {
                    bone_list.Insert(j, bone_list[i]);
                    bone_list.RemoveAt(i+1);
                }
            }

            // -----------------------------------------------------
            // リストを配列に代入し直す
            pmd.nodes = (PMD_Bone[])bone_list.ToArray();

            // -----------------------------------------------------
            // センターボーンの位置調整
            pmd.GetBoneByName("センター").position
                = new Vector3(
                    0.0f, 
                    pmd.GetBoneByName("下半身").position.Y * 0.65f, 
                    0.0f);
            pmd.GetBoneByName("センター先").position
                = new Vector3(
                    0.0f, 
                    0.0f, 
                    0.0f);

            // -----------------------------------------------------
            // 両目ボーンの位置調整
            if (mod_type == 0)
            {
                pmd.GetBoneByName("両目").position
                    = new Vector3(
                        0.0f,
                        pmd.GetBoneByName("左目").position.Y + pmd.GetBoneByName("左目").position.X * 4.0f,
                        pmd.GetBoneByName("左目").position.Z - pmd.GetBoneByName("左目").position.X * 2.0f);
                pmd.GetBoneByName("両目先").position
                    = new Vector3(
                        pmd.GetBoneByName("両目").position.X,
                        pmd.GetBoneByName("両目").position.Y,
                        pmd.GetBoneByName("両目").position.Z - 1.0f);
            }

            // -----------------------------------------------------
            // IK先ボーンの位置調整
            pmd.GetBoneByName("左足ＩＫ先").position
                = new Vector3(
                    pmd.GetBoneByName("左足ＩＫ").position.X,
                    pmd.GetBoneByName("左足ＩＫ").position.Y,
                    pmd.GetBoneByName("左足ＩＫ").position.Z + 1.7f);
            pmd.GetBoneByName("右足ＩＫ先").position
                = new Vector3(
                    pmd.GetBoneByName("右足ＩＫ").position.X,
                    pmd.GetBoneByName("右足ＩＫ").position.Y,
                    pmd.GetBoneByName("右足ＩＫ").position.Z + 1.7f);

            pmd.GetBoneByName("左つま先").position.Y = 0.0f;
            pmd.GetBoneByName("左つま先ＩＫ").position.Y = 0.0f;
            pmd.GetBoneByName("左つま先ＩＫ先").position
                = new Vector3(
                    pmd.GetBoneByName("左つま先ＩＫ").position.X,
                    pmd.GetBoneByName("左つま先ＩＫ").position.Y - 1.0f,
                    pmd.GetBoneByName("左つま先ＩＫ").position.Z);

            pmd.GetBoneByName("右つま先").position.Y = 0.0f;
            pmd.GetBoneByName("右つま先ＩＫ").position.Y = 0.0f;
            pmd.GetBoneByName("右つま先ＩＫ先").position
                = new Vector3(
                    pmd.GetBoneByName("右つま先ＩＫ").position.X,
                    pmd.GetBoneByName("右つま先ＩＫ").position.Y - 1.0f,
                    pmd.GetBoneByName("右つま先ＩＫ").position.Z);
          
            pmd.iks = (PMD_IK[])cor_table.iks.ToArray();

            pmd.disp_names = new string[cor_table.boneDispGroups.Count];

            for (int i = 0; i < pmd.disp_names.Length; i++)
                pmd.disp_names[i] = cor_table.boneDispGroups[i].name + Convert.ToChar(Convert.ToInt16("0A", 16));
            //PMDEditorを使う場合は、枠名を0x0A00で終わらせる必要があります(0x00のみだと表示されません)。

            // -----------------------------------------------------
            // ボーン枠用表示リスト
            // -----------------------------------------------------
            // 枠に表示するボーン数
            int bone_disp_count = 0;
            for (int i = 0; i < cor_table.boneDispGroups.Count; i++)
                bone_disp_count += cor_table.boneDispGroups[i].bone_names.Count;
            
            // 枠用ボーンデータ (3Bytes/bone)
            pmd.bone_disps = new PMD_BoneDisp[bone_disp_count]; 

            int idx = 0; // 通し番号
            int group_idx = 1;
            foreach (BoneDispGroup group in cor_table.boneDispGroups)
            {
                foreach (string name in group.bone_names)
                {
                    pmd.bone_disps[idx] = new PMD_BoneDisp();
                    pmd.bone_disps[idx].bone_name = name; // 枠用ボーン名
                    pmd.bone_disps[idx].disp_group_id = (sbyte)group_idx; // 表示枠番号
                    idx++;
                }
                group_idx++;
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
            pmd.bodies = (PMD_RBody[])physOb_list.rbody_list.ToArray();

            pmd.joints = (PMD_Joint[])physOb_list.joint_list.ToArray();

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
            int number_of_skin = 1;

            // 表情数
            foreach (MorphGroup mg in morph.Groups)
                number_of_skin += mg.Items.Count;
            pmd.skins = new PMD_Skin[number_of_skin];

            // 表情に関連するboneに影響を受ける頂点を数え上げる
            int numFaceVertices = CalcNumFaceVertices(fig.Tmo);

            // baseの表情
            pmd.skins[n_face++] = new PMD_Skin(numFaceVertices);

            // base以外の表情
            foreach (MorphGroup mg in morph.Groups)
            {
                foreach (Morph m in mg.Items)
                {
                    pmd.skins[n_face] = new PMD_Skin(numFaceVertices);
                    pmd.skins[n_face].name = m.Name; // 表情名 (0x00 終端，余白は 0xFD)

                    // 分類 (0：base、1：まゆ、2：目、3：リップ、4：その他)
                    switch (mg.Name)
                    {
                        case "まゆ": pmd.skins[n_face].panel_id = 1; break;
                        case "目": pmd.skins[n_face].panel_id = 2; break;
                        case "リップ": pmd.skins[n_face].panel_id = 3; break;
                        case "その他": pmd.skins[n_face].panel_id = 4; break;
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

                    pmd_v.position = Trans.CopyPos(pos);
                    pmd_v.normal = Trans.CopyPos(Vector3.Normalize(nor));
                    pmd_v.u = vertex.u;
                    pmd_v.v = vertex.v;

                    pmd_v.edge = 0;

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

                    pmd_v.weight = (sbyte)(w0 * 100 / (w0 + w1));

                    // -----------------------------------------------------
                    // 頂点リストに頂点を追加
                    
                    // 重複している頂点がないかをチェックし、
                    // 存在すれば、そのインデックスを参照
                    // 存在しなければ、頂点リストに頂点を追加
                    int idx = -1;
                    for (int i = prevNumVertices; i < vertex_list.Count; i++)
                    {
                        if (vertex_list[i].position.X == pmd_v.position.X &&
                            vertex_list[i].position.Y == pmd_v.position.Y &&
                            vertex_list[i].position.Z == pmd_v.position.Z &&
                            vertex_list[i].normal.X == pmd_v.normal.X &&
                            vertex_list[i].normal.Y == pmd_v.normal.Y &&
                            vertex_list[i].normal.Z == pmd_v.normal.Z &&
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
                        !((vertex_list[a].position.X == vertex_list[b].position.X &&
                           vertex_list[a].position.Y == vertex_list[b].position.Y &&
                           vertex_list[a].position.Z == vertex_list[b].position.Z) ||
                          (vertex_list[b].position.X == vertex_list[c].position.X &&
                           vertex_list[b].position.Y == vertex_list[c].position.Y &&
                           vertex_list[b].position.Z == vertex_list[c].position.Z) ||
                          (vertex_list[c].position.X == vertex_list[a].position.X &&
                           vertex_list[c].position.Y == vertex_list[a].position.Y &&
                           vertex_list[c].position.Z == vertex_list[a].position.Z)))
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
                material_list.material_list[n_mesh++].vindices_count = indices.Count - prevNumIndices;
                prevNumIndices = indices.Count;
                prevNumVertices = vertex_list.Count;
            }
            // -----------------------------------------------------
            // リストを配列に代入し直す
            // 頂点情報
            pmd.vertices = (PMD_Vertex[])vertex_list.ToArray();
            // 頂点インデックス
            pmd.vindices = (short[])indices.ToArray();
            // マテリアル
            if (merge_flag == true) material_list.MergeMaterials();
            pmd.materials = (PMD_Material[])material_list.material_list.ToArray();
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

                    PMD_Vertex pmd_v = pmd.vertices[idx];

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
                    PMD_Vertex pmd_v = pmd.vertices[idx];

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
                            pmd.skins[0].vertices[n_vertex].vertex_id = idx; // 表情用の頂点の番号(頂点リストにある番号)
                            pmd.skins[0].vertices[n_vertex].position = pmd_v.position;

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

                    PMD_Vertex pmd_v = pmd.vertices[idx];

                    // 表情に関連するboneに影響を受ける頂点であれば、情報を記憶する
                    foreach (SkinWeight skin_w in vertex.skin_weights)
                    {
                        // 表情に関連するboneに影響を受ける頂点であれば、表情用の頂点とする
                        if (FACE_BONE_MIN <= sub_mesh.bone_indices[skin_w.bone_index]
                            && sub_mesh.bone_indices[skin_w.bone_index] <= FACE_BONE_MAX)
                        {
                            // 表情の頂点情報（base以外）
                            for (int i = 1; i < pmd.skins.Length; i++)
                            {
                                // 表情用の頂点の番号(baseの番号。skin_vert_index)
                                pmd.skins[i].vertices[n_vertex].vertex_id = n_vertex;

                                // bace以外は相対位置で指定
                                Vector3 pmd_face_pos = Trans.CopyPos(verPos_face[i - 1][n_inMesh]);
                                pmd.skins[i].vertices[n_vertex].position.X = pmd_face_pos.X - pmd_v.position.X;
                                pmd.skins[i].vertices[n_vertex].position.Y = pmd_face_pos.Y - pmd_v.position.Y;
                                pmd.skins[i].vertices[n_vertex].position.Z = pmd_face_pos.Z - pmd_v.position.Z;
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
