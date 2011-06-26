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
using System.Text;
using jp.nyatla.nymmd.cs.types;
using jp.nyatla.nymmd.cs.struct_type.pmd;

namespace jp.nyatla.nymmd.cs.core
{
    public class PmdFace
    {
        private String _name;

        private PMD_FACE_VTX[] _face_vertex; // 表情頂点データ

        public PmdFace(PMD_FACE pPMDFaceData, PmdFace pPMDFaceBase)
        {
            // 表情名のコピー
            this._name = pPMDFaceData.szName;

            // 表情頂点データのコピー
            int number_of_vertex = pPMDFaceData.ulNumVertices;

            this._face_vertex = PMD_FACE_VTX.createArray(number_of_vertex);
            for (int i = 0; i < this._face_vertex.Length; i++)
            {
                this._face_vertex[i].setValue(pPMDFaceData.pVertices[i]);
            }
            // baseとの相対インデックスを絶対インデックスに変換
            if (pPMDFaceBase != null)
            {
                PMD_FACE_VTX[] vertex_array = this._face_vertex;
                for (int i = 0; i < this._face_vertex.Length; i++)
                {
                    PMD_FACE_VTX vertex = vertex_array[i];
                    vertex.vec3Pos.Vector3Add(pPMDFaceBase._face_vertex[vertex.ulIndex].vec3Pos, vertex.vec3Pos);
                    vertex.ulIndex = pPMDFaceBase._face_vertex[vertex.ulIndex].ulIndex;
                }
            }
            return;
        }

        public void setFace(MmdVector3[] pvec3Vertices)
        {
            if (this._face_vertex == null)
            {
                throw new MmdException();
            }

            PMD_FACE_VTX[] vertex_array = this._face_vertex;
            for (int i = 0; i < vertex_array.Length; i++)
            {
                PMD_FACE_VTX vertex = vertex_array[i];
                pvec3Vertices[vertex.ulIndex].setValue(vertex.vec3Pos);
            }
            return;
        }

        public void blendFace(MmdVector3[] pvec3Vertices, float fRate)
        {
            if (this._face_vertex == null)
            {
                throw new MmdException();
            }

            PMD_FACE_VTX[] vertex_array = this._face_vertex;
            for (int i = 0; i < vertex_array.Length; i++)
            {
                PMD_FACE_VTX vertex = vertex_array[i];
                int ulIndex = vertex.ulIndex;
                pvec3Vertices[ulIndex].Vector3Lerp(pvec3Vertices[ulIndex], vertex.vec3Pos, fRate);
            }
            return;
        }

        public String getName()
        {
            return this._name;
        }
    }
}
