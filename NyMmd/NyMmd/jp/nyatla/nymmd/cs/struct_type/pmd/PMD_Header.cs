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
    public class PMD_Header : StructType
    {
        public const int SIZE_OF_STRUCT = 3 + 4 + 20 + 256;
        public String szMagic;
        public float fVersion;
        public String szName;
        public String szComment;

        public void read(DataReader i_reader)
        {
            //szMagic
            this.szMagic = i_reader.readAscii(3);
            //fVersion
            this.fVersion = i_reader.readFloat();
            //szName
            this.szName = i_reader.readAscii(20);
            //szComment
            this.szComment = i_reader.readAscii(256);
            return;
        }

        public void write(DataWriter i_writer)
        {
            //szMagic
            i_writer.writeAscii(this.szMagic, 3);
            //fVersion
            i_writer.writeFloat(this.fVersion);
            //szName
            i_writer.writeAscii(this.szName, 20);
            //szComment
            i_writer.writeAscii(this.szComment, 256);
            return;
        }
    }
}
