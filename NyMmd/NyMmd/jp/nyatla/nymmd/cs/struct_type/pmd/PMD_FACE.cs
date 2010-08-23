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
using System.Linq;
using System.Text;

namespace jp.nyatla.nymmd.cs.struct_type.pmd
{
    public class PMD_FACE : StructType
    {
        public String szName;		// 表情名 (0x00 終端，余白は 0xFD)
        private int numVertices;	// 表情頂点数
        public int cbType;			// 分類 (0：base、1：まゆ、2：目、3：リップ、4：その他)
        public PMD_FACE_VTX[] pVertices = PMD_FACE_VTX.createArray(64);// 表情頂点データ

        public PMD_FACE()
        {
            this.szName = "デフォ";
            this.numVertices = 20;
            this.cbType = 0;
            return;
        }
        public PMD_FACE(int n)
        {
            this.szName = "デフォ";
            this.numVertices = n;
            this.cbType = 0;
            //必要な数だけ配列を確保しなおす。
            if (this.numVertices > this.pVertices.Length)
            {
                this.pVertices = PMD_FACE_VTX.createArray(this.numVertices);
            }
            return;
        }

        public int ulNumVertices
        { 
            get { return numVertices; }
        }
              
        public void read(DataReader i_reader)
        {
            this.szName = i_reader.readAscii(20);
            this.numVertices = i_reader.readInt();
            this.cbType = i_reader.read();

            //必要な数だけ配列を確保しなおす。
            if (this.numVertices > this.pVertices.Length)
            {
                this.pVertices = PMD_FACE_VTX.createArray(this.numVertices);
            }

            for (int i = 0; i < this.numVertices; i++)
            {
                this.pVertices[i].read(i_reader);
            }
            return;
        }

        public void write(DataWriter i_writer)
        {
            i_writer.writeAscii(this.szName, 20);
            i_writer.writeInt(this.numVertices);
            i_writer.write(this.cbType);

            for (int i = 0; i < this.numVertices; i++)
            {
                this.pVertices[i].write(i_writer);
            }
            return;
        }
    }

}
