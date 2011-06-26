/* 
 * PROJECT: MMD for Java
 * --------------------------------------------------------------------------------
 * This work is based on the ARTK_MMD v0.1 
 *   PY
 * http://ppyy.hp.infoseek.co.jp/
 * py1024<at>gmail.com
 * http://www.nicovideo.jp/watch/sm7398691
 *
 * The MMD for Java is Java version MMD class library.
 * Copyright (C)2009 nyatla
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this framework; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 * 
 * For further information please contact.
 *	http://nyatla.jp/
 *	<airmail(at)ebony.plala.or.jp>
 * 
 */
using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.IO;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

using jp.nyatla.nymmd.cs.types;
using jp.nyatla.nymmd.cs.core;
using jp.nyatla.nymmd.cs.struct_type;
using jp.nyatla.nymmd.cs.struct_type.pmd;

namespace jp.nyatla.nymmd.cs
{

    class DataComparator : IComparer<PmdIK>
    {
        public int Compare(PmdIK o1, PmdIK o2)
        {
            return (int)(o1.getSortVal() - o2.getSortVal());
        }
    }

    public class MmdPmdModel
    {
        private String _name;	// モデル名
        private int _number_of_vertex;	// 頂点数

        private PmdFace[] m_pFaceArray; // 表情配列
        private PmdBone[] m_pBoneArray; // ボーン配列
        private PmdIK[] m_pIKArray;    // IK配列

        private MmdVector3[] _position_array;	// 座標配列	
        private MmdVector3[] _normal_array;		// 法線配列
        private MmdTexUV[] _texture_uv;		// テクスチャ座標配列
        private PmdSkinInfo[] _skin_info_array;
        private PmdMaterial[] _materials;		// マテリアル配列

        // -----------------------------------------------------
        // コンストラクタ
        // -----------------------------------------------------
        public MmdPmdModel()
        {
            return;
        }
        public MmdPmdModel(Stream i_stream)
        {
            PmdFileData pmd = new PmdFileData(i_stream);
            initialize(pmd);
            return;
        }
        public MmdPmdModel(StreamReader i_reader)
        {
            PmdFileData pmd = new PmdFileData(i_reader);
            initialize(pmd);
            return;
        }
        public MmdPmdModel(BinaryReader i_reader)
        {
            PmdFileData pmd = new PmdFileData(i_reader);
            initialize(pmd);
            return;
        }
        public MmdPmdModel(PmdFileData pmd)
        {
            initialize(pmd);
            return;
        }

        public int getNumberOfVertex()
        {
            return this._number_of_vertex;
        }
        public PmdMaterial[] getMaterials()
        {
            return this._materials;
        }
        public MmdTexUV[] getUvArray()
        {
            return this._texture_uv;
        }
        public MmdVector3[] getPositionArray()
        {
            return this._position_array;
        }
        public MmdVector3[] getNormatArray()
        {
            return this._normal_array;
        }
        public PmdSkinInfo[] getSkinInfoArray()
        {
            return this._skin_info_array;
        }
        public PmdFace[] getFaceArray()
        {
            return this.m_pFaceArray;
        }
        public PmdBone[] getBoneArray()
        {
            return this.m_pBoneArray;
        }
        public PmdIK[] getIKArray()
        {
            return this.m_pIKArray;
        }


        public PmdBone getBoneByName(String i_name)
        {
            PmdBone[] bone_array = this.m_pBoneArray;
            for (int i = 0; i < bone_array.Length; i++)
            {
                PmdBone bone = bone_array[i];
                if (bone.getName().Equals(i_name))
                    return bone;
            }
            return null;
        }
        public PmdFace getFaceByName(String i_name)
        {
            PmdFace[] face_array = this.m_pFaceArray;
            for (int i = 0; i < face_array.Length; i++)
            {
                PmdFace face = face_array[i];
                if (face.getName().Equals(i_name))
                    return face;
            }
            return null;
        }

        public String getModelName()
        {
            return this._name;
        }

        private void initialize(PmdFileData pmd)
        {
            // -----------------------------------------------------
            // モデル名をコピー
            this._name = pmd.pmd_header.szName;

            // -----------------------------------------------------
            // 頂点数をコピー
            this._number_of_vertex = pmd.number_of_vertex;

            // 頂点配列をコピー
            this._position_array = MmdVector3.createArray(this._number_of_vertex);
            this._normal_array = MmdVector3.createArray(this._number_of_vertex);
            this._texture_uv = MmdTexUV.createArray(this._number_of_vertex);
            this._skin_info_array = new PmdSkinInfo[this._number_of_vertex];

            for (int i = 0; i < this._number_of_vertex; i++)
            {
                _position_array[i].setValue(pmd.pmd_vertex[i].vec3Pos);
                _normal_array[i].setValue(pmd.pmd_vertex[i].vec3Normal);
                _texture_uv[i].setValue(pmd.pmd_vertex[i].uvTex);

                this._skin_info_array[i] = new PmdSkinInfo();
                this._skin_info_array[i].fWeight = pmd.pmd_vertex[i].cbWeight / 100.0f;
                this._skin_info_array[i].unBoneNo[0] = pmd.pmd_vertex[i].unBoneNo[0];
                this._skin_info_array[i].unBoneNo[1] = pmd.pmd_vertex[i].unBoneNo[1];
            }

            // -----------------------------------------------------
            // マテリアル配列をコピー
            this._materials = new PmdMaterial[pmd.number_of_materials];

            int indices_ptr = 0;
            for (int i = 0; i < pmd.number_of_materials; i++)
            {
                this._materials[i] = new PmdMaterial();
                this._materials[i].toon_index = pmd.pmd_material[i].toon_index;
                this._materials[i].edge_flag = pmd.pmd_material[i].edge_flag;

                this._materials[i].indices = new short[pmd.pmd_material[i].ulNumIndices];
                System.Array.Copy(pmd.indices_array, indices_ptr, this._materials[i].indices, 0, pmd.pmd_material[i].ulNumIndices);
                indices_ptr += pmd.pmd_material[i].ulNumIndices;

                this._materials[i].col4Diffuse.setValue(pmd.pmd_material[i].col4Diffuse);

                this._materials[i].col4Specular.r = pmd.pmd_material[i].col3Specular.r;
                this._materials[i].col4Specular.g = pmd.pmd_material[i].col3Specular.g;
                this._materials[i].col4Specular.b = pmd.pmd_material[i].col3Specular.b;
                this._materials[i].col4Specular.a = 1.0f;

                this._materials[i].col4Ambient.r = pmd.pmd_material[i].col3Ambient.r;
                this._materials[i].col4Ambient.g = pmd.pmd_material[i].col3Ambient.g;
                this._materials[i].col4Ambient.b = pmd.pmd_material[i].col3Ambient.b;
                this._materials[i].col4Ambient.a = 1.0f;

                this._materials[i].fShininess = pmd.pmd_material[i].fShininess;

                this._materials[i].texture_name = pmd.pmd_material[i].szTextureFileName;
                if (this._materials[i].texture_name.Length < 1)
                {
                    this._materials[i].texture_name = null;
                }
            }

            // -----------------------------------------------------
            // Bone配列のコピー
            this.m_pBoneArray = new PmdBone[pmd.number_of_bone];
            for (int i = 0; i < pmd.number_of_bone; i++)
            {
                //ボーンの親子関係を一緒に読みだすので。
                this.m_pBoneArray[i] = new PmdBone(pmd.pmd_bone[i], this.m_pBoneArray);
            }
            for (int i = 0; i < pmd.number_of_bone; i++)
            {
                this.m_pBoneArray[i].recalcOffset();
            }
 
            // -----------------------------------------------------
            // IK配列のコピー
            this.m_pIKArray = new PmdIK[pmd.number_of_ik];
            // IK配列を作成
            if (pmd.number_of_ik > 0)
            {
                for (int i = 0; i < pmd.number_of_ik; i++)
                {
                    this.m_pIKArray[i] = new PmdIK(pmd.pmd_ik[i], this.m_pBoneArray);
                }
                System.Array.Sort<PmdIK>(this.m_pIKArray, new DataComparator());
            }
            
            // -----------------------------------------------------
            // Face配列のコピー
            if (pmd.number_of_face > 0)
            {
                this.m_pFaceArray = new PmdFace[pmd.number_of_face];
                for (int i = 0; i < pmd.number_of_face; i++)
                {
                    this.m_pFaceArray[i] = new PmdFace(pmd.pmd_face[i], this.m_pFaceArray[0]);
                }
            }
            
            return;
        }
    }

}
