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
#if NyMmd_FRAMEWORK_CFW
using Microsoft.WindowsMobile.DirectX.Direct3D;
using Microsoft.WindowsMobile.DirectX;
#else
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
#endif
using System.IO;

using jp.nyatla.nymmd.cs;
using jp.nyatla.nymmd.cs.types;

namespace NyMmdUtils
{

    class D3dTextureData
    {
        public Texture d3d_texture;
        public String file_name;
    }

    class D3dTextureList : IDisposable
    {
        private List<D3dTextureData> m_pTextureList = new List<D3dTextureData>();

        private Device _device;

        public D3dTextureList(Device i_device)
        {
            this._device = i_device;
        }
        public void Dispose()
        {
            reset();
        }

        public void reset()
        {
            for (int i = 0; i < m_pTextureList.Count; i++)
            {
                this.m_pTextureList[i].d3d_texture.Dispose();
            }
            this.m_pTextureList.Clear();
            return;
        }

        private D3dTextureData createTexture(String szFileName, Stream i_st)
        {
            D3dTextureData ret = new D3dTextureData();
            ret.file_name = szFileName;
            ret.d3d_texture = TextureLoader.FromStream(this._device, i_st);
            return ret;
        }

        public D3dTextureData getTexture(String i_filename, IMmdDataIo i_io)
        {
            D3dTextureData ret;

            int len = this.m_pTextureList.Count;
            for (int i = 0; i < len; i++)
            {
                ret = this.m_pTextureList[i];
                if (ret.file_name.Equals(i_filename, StringComparison.CurrentCultureIgnoreCase))
                {
                    // 読み込み済みのテクスチャを発見
                    return ret;
                }
            }

            // なければファイルを読み込んでテクスチャ作成
            ret = createTexture(i_filename, i_io.request(i_filename));
            if (ret != null)
            {
                this.m_pTextureList.Add(ret);
                return ret;
            }
            return null;// テクスチャ読み込みか作成失敗

        }
    }








    class D3dMaterial : IDisposable
    {
        public D3dTextureData texture;
        public int ulNumIndices;
        public IndexBuffer index_buf;
        public int unknown;
        public Material material = new Material();
        public void Dispose()
        {
            index_buf.Dispose();
        }
    }

    /*  MMDレンダラにちょっと細工をしてですね…。
     *  
     * 
     */
    public class MmdPmdRenderD3d : IMmdPmdRender
    {
        private D3dTextureList _texture_list;
        private D3dMaterial[] _materials;
        private Device _device;
        private VertexBuffer _vertex_buffer;
        public MmdPmdRenderD3d(Device i_device)
        {
            this._texture_list = new D3dTextureList(i_device);
            this._device = i_device;
            return;
        }
        public void Dispose()
        {
            this._vertex_buffer.Dispose();
            for (int i = 0; i < this._materials.Length; i++)
            {
                this._materials[i].Dispose();
            }
            this._texture_list.Dispose();
        }


        public void setPmd(MmdPmdModel i_pmd, IMmdDataIo i_io)
        {
            this._ref_pmd = i_pmd;
            int number_of_vertex = i_pmd.getNumberOfVertex();
            this._vertex_array = new CustomVertex.PositionNormalTextured[number_of_vertex];


            MmdTexUV[] uv_array = i_pmd.getUvArray();
            //先にセットできるものはセットしておく
            for (int i = 0; i < number_of_vertex; i++)
            {
                this._vertex_array[i].Tu = uv_array[i].u;
                this._vertex_array[i].Tv = uv_array[i].v;
            }

            //matreial
            PmdMaterial[] m = i_pmd.getMaterials();
            List<D3dMaterial> d3d_materials = new List<D3dMaterial>();
            for (int i = 0; i < m.Length; i++)
            {
                int number_of_indices = m[i].indices.Length;
                D3dMaterial new_material = new D3dMaterial();
                new_material.index_buf = new IndexBuffer(this._device, number_of_indices * 3 * 2, Usage.WriteOnly, Pool.Managed, true);
                new_material.index_buf.SetData(m[i].indices, 0, 0);

                new_material.material.DiffuseColor = new ColorValue(m[i].col4Diffuse.r, m[i].col4Diffuse.g, m[i].col4Diffuse.b, m[i].col4Diffuse.a);
                new_material.material.AmbientColor = new ColorValue(m[i].col4Ambient.r, m[i].col4Ambient.g, m[i].col4Ambient.b, m[i].col4Ambient.a);
                new_material.material.SpecularColor = new ColorValue(m[i].col4Specular.r, m[i].col4Specular.g, m[i].col4Specular.b, m[i].col4Specular.a);
                new_material.material.SpecularSharpness = m[i].fShininess;
                if (m[i].texture_name != null)
                {
                    new_material.texture = this._texture_list.getTexture(m[i].texture_name, i_io);
                }
                else
                {
                    new_material.texture = null;
                }
                new_material.unknown = m[i].edge_flag;
                new_material.ulNumIndices = number_of_indices;
                d3d_materials.Add(new_material);
            }
            this._materials = d3d_materials.ToArray();
            //
            this._vertex_buffer = new VertexBuffer(
                typeof(CustomVertex.PositionNormalTextured),
                number_of_vertex, this._device, 0, CustomVertex.PositionNormalTextured.Format, Pool.Managed);
            return;
        }
        private CustomVertex.PositionNormalTextured[] _vertex_array;
        private MmdPmdModel _ref_pmd;

        //この関数でthis._vertex_arrayを更新する。
        public void updateSkinning(MmdMatrix[] i_skinning_mat)
        {
            MmdPmdModel pmd = this._ref_pmd;
            int number_of_vertex = pmd.getNumberOfVertex();
            MmdVector3[] org_pos_array = pmd.getPositionArray();
            MmdVector3[] org_normal_array = pmd.getNormatArray();
            PmdSkinInfo[] org_skin_info = pmd.getSkinInfoArray();
            CustomVertex.PositionNormalTextured[] vertex_array = this._vertex_array;
            // 頂点スキニング
            MmdMatrix matTemp = new MmdMatrix();
            MmdVector3 position = new MmdVector3();
            MmdVector3 normal = new MmdVector3();
            for (int i = 0; i < number_of_vertex; i++)
            {
                PmdSkinInfo si = org_skin_info[i];
                if (si.fWeight == 0.0f)
                {
                    MmdMatrix mat = i_skinning_mat[si.unBoneNo[1]];
                    position.Vector3Transform(org_pos_array[i], mat);
                    normal.Vector3Rotate(org_normal_array[i], mat);
                }
                else if (si.fWeight >= 0.9999f)
                {
                    MmdMatrix mat = i_skinning_mat[si.unBoneNo[0]];
                    position.Vector3Transform(org_pos_array[i], mat);
                    normal.Vector3Rotate(org_normal_array[i], mat);
                }
                else
                {
                    MmdMatrix mat0 = i_skinning_mat[si.unBoneNo[0]];
                    MmdMatrix mat1 = i_skinning_mat[si.unBoneNo[1]];
                    matTemp.MatrixLerp(mat0, mat1, si.fWeight);
                    position.Vector3Transform(org_pos_array[i], matTemp);
                    normal.Vector3Rotate(org_normal_array[i], matTemp);
                }
                //ここの転写は少し考える。
                vertex_array[i].X = position.x;
                vertex_array[i].Y = position.y;
                vertex_array[i].Z = position.z;
                vertex_array[i].Nx = normal.x;
                vertex_array[i].Ny = normal.y;
                vertex_array[i].Nz = normal.z;
            }
            return;
        }
        public void render()
        {
            Device dev = this._device;
            //頂点データをセット
            GraphicsStream stm = this._vertex_buffer.Lock(0, 0, 0);
            stm.Write(this._vertex_array);
            stm.Dispose();
            this._vertex_buffer.Unlock();

            //
            D3dMaterial[] material = this._materials;
            for (int i = 0; i < material.Length; i++)
            {
                //カリング判定：何となくうまくいったから
                if ((0x100 & material[i].unknown) == 0x100)
                {
                    dev.RenderState.CullMode = Cull.None;
                }
                else
                {
                    dev.RenderState.CullMode = Cull.CounterClockwise;
                }
                /*                if ((0x0f & material[i].unknown) == 0x02)
                                {
                                    dev.RenderState.CullMode = Cull.None;
                                }
                                else
                                {
                                    dev.RenderState.CullMode = Cull.CounterClockwise;
                                }*/
                /*
                if (material[i].material.DiffuseColor.Alpha < 1.0f && material[i].texture == null)
                {
                    dev.RenderState.CullMode = Cull.None;
                }
                else
                {
                    dev.RenderState.CullMode = Cull.CounterClockwise;
                }
                */


                dev.Material = material[i].material;
                if (material[i].texture != null)
                {
                    dev.SetTexture(0, material[i].texture.d3d_texture);
                }
                else
                {
                    dev.SetTexture(0, null);
                }

                //インデクスをセット
                dev.Indices = material[i].index_buf;
                dev.SetStreamSource(0, this._vertex_buffer, 0);
                dev.VertexFormat = CustomVertex.PositionNormalTextured.Format;
                dev.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, material[i].ulNumIndices, 0, material[i].ulNumIndices/3);
            }
            return;
        }
    }


}
