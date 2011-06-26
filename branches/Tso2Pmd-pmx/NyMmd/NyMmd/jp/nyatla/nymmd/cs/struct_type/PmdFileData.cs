using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using jp.nyatla.nymmd.cs.types;
using jp.nyatla.nymmd.cs.core;
using jp.nyatla.nymmd.cs.struct_type;
using jp.nyatla.nymmd.cs.struct_type.pmd;

namespace jp.nyatla.nymmd.cs.struct_type
{
    public class PmdFileData
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
	    public string model_name_eg;//[20]; // モデル名(英語)
	    public string comment_eg;//[256]; // コメント(英語)

	    public string[] bone_name_eg;//[20][bone_count]; // ボーン名(英語)

	    public string[] skin_name_eg;//[20][skin_count - 1]; // 表情名(英語)

	    public string[] disp_name_eg;//[50][bone_disp_name_count]; // 枠名(英語) // MMDでは「区分名」

	    public string[] toon_file_name;//[100][10]; // トゥーンテクスチャファイル名

	    public int rigidbody_count; // 剛体数 // 2D 00 00 00 == 45
        public PMD_RigidBody[] rigidbody;//[rigidbody_count]; // 剛体データ(83Bytes/rigidbody)

	    public int joint_count; // ジョイント数 // 1B 00 00 00 == 27
        public PMD_Joint[] joint;//[joint_count]; // ジョイントデータ(124Bytes/joint)


        // -----------------------------------------------------
        // コンストラクタ
        // -----------------------------------------------------
        public PmdFileData()
        {
            return;
        }
        public PmdFileData(Stream i_stream)
        {
            this.ReadPmdFile(new BinaryReader(i_stream));
            return;
        }
        public PmdFileData(StreamReader i_reader)
        {
            this.ReadPmdFile(new BinaryReader(i_reader.BaseStream));
            return;
        }
        public PmdFileData(BinaryReader i_reader)
        {
            this.ReadPmdFile(i_reader);
            return;
        }

        // -----------------------------------------------------
        // 各種メソッド
        // -----------------------------------------------------
        // 名前からボーンを得る
        public PMD_Bone getBoneByName(string name)
        {
            foreach (PMD_Bone bone in this.pmd_bone)
                if (bone.szName == name) return bone;

            return null;
        }
        // 名前からボーンIDを得る
        private int getBoneIDByName(string name)
        {
            for (int i = 0; i < this.pmd_bone.Length; i++)
                if (pmd_bone[i].szName == name) return i;

            return -1;
        }

        // -----------------------------------------------------
        // .pmdファイルを読み出す
        // -----------------------------------------------------
        public void ReadPmdFile(Stream i_stream)
        {
            ReadPmdFile(new BinaryReader(i_stream));
            return;
        }
        public void ReadPmdFile(StreamReader i_reader)
        {
            ReadPmdFile(new BinaryReader(i_reader.BaseStream));
            return;
        }
        public void ReadPmdFile(BinaryReader i_reader)
        {
            DataReader reader = new DataReader(i_reader);

            // -----------------------------------------------------
            // ヘッダー情報取得
            pmd_header = new PMD_Header();
            pmd_header.read(reader);
            if (!pmd_header.szMagic.Equals("PMD", StringComparison.CurrentCultureIgnoreCase))
            {
                throw new MmdException();
            }

            // -----------------------------------------------------
            // 頂点数取得
            number_of_vertex = reader.readInt();//
            if (number_of_vertex < 0)
            {
                throw new MmdException();
            }

            // 頂点配列取得
            pmd_vertex = new PMD_Vertex[number_of_vertex];

            for (int i = 0; i < number_of_vertex; i++)
            {
                pmd_vertex[i] = new PMD_Vertex();
                pmd_vertex[i].read(reader);
            }

            // -----------------------------------------------------
            // 頂点インデックス数取得
            number_of_indices = reader.readInt();

            // 頂点インデックス配列取得
            indices_array = new short[number_of_indices];
            for (int i = 0; i < number_of_indices; i++)
            {
                indices_array[i] = reader.readShort();
            }

            // -----------------------------------------------------
            // マテリアル数取得
            number_of_materials = reader.readInt();

            // マテリアル配列取得
            pmd_material = new PMD_Material[number_of_materials];
            for (int i = 0; i < number_of_materials; i++)
            {
                pmd_material[i] = new PMD_Material();
                pmd_material[i].read(reader);
            }

            // -----------------------------------------------------
            // Bone数取得
            number_of_bone = reader.readShort();

            // Bone配列取得
            pmd_bone = new PMD_Bone[number_of_bone];
            for (int i = 0; i < number_of_bone; i++)
            {
                pmd_bone[i] = new PMD_Bone();
                pmd_bone[i].read(reader);
            }

            // ボーンIDを名前に置き換え
            // Bone
            foreach (PMD_Bone bone in pmd_bone)
            {
                if (bone.nParentNo <= -1) bone.ParentName = null;
                else bone.ParentName = pmd_bone[bone.nParentNo].szName;
                if (bone.nChildNo <= 0) bone.ChildName = null;
                else bone.ChildName = pmd_bone[bone.nChildNo].szName;
                if (bone.unIKTarget <= 0) bone.IKTargetName = null;
                else bone.IKTargetName = pmd_bone[bone.unIKTarget].szName;
            }
            // Vertex
            foreach (PMD_Vertex vertex in pmd_vertex)
            {
                if (vertex.unBoneNo[0] <= -1) vertex.unBoneName[0] = null;
                else vertex.unBoneName[0] = pmd_bone[vertex.unBoneNo[0]].szName;
                if (vertex.unBoneNo[1] <= -1) vertex.unBoneName[1] = null;
                else vertex.unBoneName[1] = pmd_bone[vertex.unBoneNo[1]].szName;
            }

            // -----------------------------------------------------
            // IK配列数取得
            number_of_ik = reader.readShort();

            // IK配列取得
            pmd_ik = new PMD_IK[number_of_ik];
            if (number_of_ik > 0)
            {
                for (int i = 0; i < number_of_ik; i++)
                {
                    pmd_ik[i] = new PMD_IK();
                    pmd_ik[i].read(reader);
                }
            }
            // ボーンIDを名前に置き換え
            foreach (PMD_IK ik in pmd_ik)
            {
                if (ik.nTargetNo <= -1) ik.nTargetName = null;
                else ik.nTargetName = pmd_bone[ik.nTargetNo].szName;
                if (ik.nEffNo <= -1) ik.nEffName = null;
                else ik.nEffName = pmd_bone[ik.nEffNo].szName;

                for (int i = 0; i < ik.cbNumLink; i++)
                {
                    if (ik.punLinkNo[i] <= -1) ik.punLinkName[i] = null;
                    else ik.punLinkName[i] = pmd_bone[ik.punLinkNo[i]].szName;
                }
            }

            // -----------------------------------------------------
            // Face数取得
            number_of_face = reader.readShort();

            // Face配列取得
            pmd_face = new PMD_FACE[number_of_face];
            if (number_of_face > 0)
            {
                for (int i = 0; i < number_of_face; i++)
                {
                    pmd_face[i] = new PMD_FACE();
                    pmd_face[i].read(reader);
                }
            }

            // -----------------------------------------------------
            // 表情枠
            // -----------------------------------------------------
            // 表情枠に表示する表情数
            skin_disp_count = reader.readByte();

            // 表情番号
            skin_index = new int[skin_disp_count];
            if (skin_disp_count > 0)
            {
                for (int i = 0; i < skin_disp_count; i++)
                {
                    skin_index[i] = reader.readShort();
                }
            }

            // -----------------------------------------------------
            // ボーン枠
            // -----------------------------------------------------
            // ボーン枠用の枠名数
            bone_disp_name_count = reader.readByte();

            // 枠名(50Bytes/枠)
            disp_name = new string[bone_disp_name_count];
            if (bone_disp_name_count > 0)
            {
                for (int i = 0; i < bone_disp_name_count; i++)
                {
                    disp_name[i] = reader.readAscii(50);
                }
            }

            // -----------------------------------------------------
            // ボーン枠に表示するボーン
            // -----------------------------------------------------
            // ボーン枠に表示するボーン数
            bone_disp_count = reader.readInt();

            // 枠用ボーンデータ (3Bytes/bone)
            bone_disp = new PMD_BoneDisp[bone_disp_count];
            if (bone_disp_count > 0)
            {
                for (int i = 0; i < bone_disp_count; i++)
                {
                    bone_disp[i] = new PMD_BoneDisp();
                    bone_disp[i].read(reader);
                }
            }

            // ボーンIDを名前に置き換え
            for (int i = 0; i < bone_disp.Length; i++)
            {
                bone_disp[i].bone_name = pmd_bone[bone_disp[i].bone_index].szName;
            }

            // -----------------------------------------------------
            // 英名対応(0:英名対応なし, 1:英名対応あり)
            // -----------------------------------------------------
            english_name_compatibility = reader.readByte();

            if ( english_name_compatibility==1 )
            {
                model_name_eg = reader.readAscii(20); // モデル名(英語)
                comment_eg = reader.readAscii(256); // コメント(英語)

                // -----------------------------------------------------
                // ボーン名(英語)
                // -----------------------------------------------------
                bone_name_eg = new string[number_of_bone];
                if (number_of_bone > 0)
                {
                    for (int i = 0; i < number_of_bone; i++)
                    {
                        bone_name_eg[i] = reader.readAscii(20);
                    }
                }

                // -----------------------------------------------------
                // 表情名(英語)
                // -----------------------------------------------------
                skin_name_eg = new string[number_of_face - 1];
                if (number_of_face - 1 > 0)
                {
                    for (int i = 0; i < number_of_face - 1; i++)
                    {
                        skin_name_eg[i] = reader.readAscii(20);
                    }
                }

                // -----------------------------------------------------
                // 枠名(英語)
                // -----------------------------------------------------
                disp_name_eg = new string[bone_disp_name_count];
                if (bone_disp_name_count > 0)
                {
                    for (int i = 0; i < bone_disp_name_count; i++)
                    {
                        disp_name_eg[i] = reader.readAscii(50);
                    }
                }
            }

            // -----------------------------------------------------
            // トゥーンテクスチャファイル名
            // -----------------------------------------------------
            toon_file_name = new string[10];
            for (int i = 0; i < 10; i++)
            {
                toon_file_name[i] = reader.readAscii(100);
            }

            // -----------------------------------------------------
            // 剛体
            // -----------------------------------------------------
            // 剛体数
            rigidbody_count = reader.readInt();

            // 剛体データ(83Bytes/rigidbody)
            rigidbody = new PMD_RigidBody[rigidbody_count];
            if (rigidbody_count > 0)
            {
                for (int i = 0; i < rigidbody_count; i++)
                {
                    rigidbody[i] = new PMD_RigidBody();
                    rigidbody[i].read(reader);
                }
            }

            // -----------------------------------------------------
            // ジョイント
            // -----------------------------------------------------
            // ジョイント数
            joint_count = reader.readInt();

            // ジョイントデータ(124Bytes/joint)
            joint = new PMD_Joint[joint_count];
            if (joint_count > 0)
            {
                for (int i = 0; i < joint_count; i++)
                {
                    joint[i] = new PMD_Joint();
                    joint[i].read(reader);
                }
            }

            return;
        }


        // -----------------------------------------------------
        // .pmdファイルを書き出す
        // -----------------------------------------------------
        public void WritePmdFile(Stream i_stream)
        {
            WritePmdFile(new BinaryWriter(i_stream));
        }
        public void WritePmdFile(StreamWriter i_writer)
        {
            WritePmdFile(new BinaryWriter(i_writer.BaseStream));
        }
        public void WritePmdFile(BinaryWriter i_writer)
        {
            DataWriter writer = new DataWriter(i_writer);

            // -----------------------------------------------------
            // ヘッダー情報
            pmd_header.write(writer);

            // -----------------------------------------------------
            // 頂点数
            writer.writeInt(number_of_vertex);

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
                pmd_vertex[i].write(writer);
            }

            // -----------------------------------------------------
            // 頂点インデックス数
            writer.writeInt(number_of_indices);

            // 頂点インデックス配列
            for (int i = 0; i < number_of_indices; i++)
            {
                writer.writeShort(indices_array[i]);
            }

            // -----------------------------------------------------
            // マテリアル数
            writer.writeInt(number_of_materials);

            // マテリアル配列
            for (int i = 0; i < number_of_materials; i++)
            {
                pmd_material[i].write(writer);
            }

            // -----------------------------------------------------
            // Bone数
            writer.writeShort(number_of_bone);

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
                pmd_bone[i].write(writer);
            }

            // -----------------------------------------------------
            // IK数
            writer.writeShort(number_of_ik);

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
                    pmd_ik[i].write(writer);
            }

            // -----------------------------------------------------
            // Face数
            writer.writeShort(number_of_face);

            // Face配列
            for (int i = 0; i < number_of_face; i++)
            {
                pmd_face[i].write(writer);
            }

            // -----------------------------------------------------
            // 表情枠
            // -----------------------------------------------------
            // 表情枠に表示する表情数
            writer.writeByte(skin_disp_count);

            // 表情番号
            if (skin_disp_count > 0)
            {
                for (int i = 0; i < skin_disp_count; i++)
                {
                    writer.writeShort(skin_index[i]);
                }
            }

            // -----------------------------------------------------
            // ボーン枠
            // -----------------------------------------------------
            // ボーン枠用の枠名数
            writer.writeByte(bone_disp_name_count);

            // 枠名(50Bytes/枠)
            if (bone_disp_name_count > 0)
            {
                for (int i = 0; i < bone_disp_name_count; i++)
                {
                    writer.writeAscii(disp_name[i], 50);
                }
            }

            // -----------------------------------------------------
            // ボーン枠に表示するボーン
            // -----------------------------------------------------
            // ボーン枠に表示するボーン数
            writer.writeInt(bone_disp_count);

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
                    bone_disp[i].write(writer);
                }
            }

            // -----------------------------------------------------
            // 英名対応(0:英名対応なし, 1:英名対応あり)
            // -----------------------------------------------------
            writer.writeByte(english_name_compatibility);

            if ( english_name_compatibility==1 )
            {
                writer.writeAscii(model_name_eg, 20); // モデル名(英語)
                writer.writeAscii(comment_eg, 256); // コメント(英語)

                // -----------------------------------------------------
                // ボーン名(英語)
                // -----------------------------------------------------
                if (number_of_bone > 0)
                {
                    for (int i = 0; i < number_of_bone; i++)
                    {
                        writer.writeAscii(bone_name_eg[i], 20);
                    }
                }

                // -----------------------------------------------------
                // 表情名(英語)
                // -----------------------------------------------------
                if (number_of_face - 1 > 0)
                {
                    for (int i = 0; i < number_of_face - 1; i++)
                    {
                        writer.writeAscii(skin_name_eg[i], 20);
                    }
                }

                // -----------------------------------------------------
                // 枠名(英語)
                // -----------------------------------------------------
                if (bone_disp_name_count > 0)
                {
                    for (int i = 0; i < bone_disp_name_count; i++)
                    {
                        writer.writeAscii(disp_name_eg[i], 50);
                    }
                }
            }

            // -----------------------------------------------------
            // トゥーンテクスチャファイル名
            // -----------------------------------------------------
            for (int i = 0; i < 10; i++)
            {
                writer.writeAscii(toon_file_name[i], 100);
            }

            // -----------------------------------------------------
            // 剛体
            // -----------------------------------------------------
            // 剛体数
            writer.writeInt(rigidbody_count);

            // 剛体データ(83Bytes/rigidbody)
            if (rigidbody_count > 0)
            {
                for (int i = 0; i < rigidbody_count; i++)
                {
                    rigidbody[i].write(writer);
                }
            }

            // -----------------------------------------------------
            // ジョイント
            // -----------------------------------------------------
            // ジョイント数
            writer.writeInt(joint_count);

            // ジョイントデータ(124Bytes/joint)
            if (joint_count > 0)
            {
                for (int i = 0; i < joint_count; i++)
                {
                    joint[i].write(writer);
                }
            }

            return;
        }
    }
}
