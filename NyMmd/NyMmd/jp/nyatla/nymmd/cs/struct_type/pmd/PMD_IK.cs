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

namespace jp.nyatla.nymmd.cs.struct_type.pmd
{
    public class PMD_IK : StructType
    {
        internal int nTargetNo;	// IKターゲットボーン番号
        internal int nEffNo;		// IK先端ボーン番号
        public int cbNumLink;	// IKを構成するボーンの数
        public int unCount;
        public float fFact;
        internal int[] punLinkNo = new int[128];// IKを構成するボーンの配列(可変長配列)

        public void read(DataReader i_reader)
        {
            this.nTargetNo = i_reader.readShort();
            this.nEffNo = i_reader.readShort();
            this.cbNumLink = i_reader.read();
            this.unCount = i_reader.readUnsignedShort();
            this.fFact = i_reader.readFloat();
            //必要な数だけ配列を確保しなおす。
            if (this.cbNumLink > this.punLinkNo.Length)
            {
                this.punLinkNo = new int[this.cbNumLink];
                this.punLinkName = new string[this.cbNumLink];
            }
            for (int i = 0; i < this.cbNumLink; i++)
            {
                this.punLinkNo[i] = i_reader.readUnsignedShort();
            }
            return;
        }

        public void write(DataWriter i_writer)
        {
            i_writer.writeShort(this.nTargetNo);
            i_writer.writeShort(this.nEffNo);
            i_writer.write(this.cbNumLink);
            i_writer.writeUnsignedShort(this.unCount);
            i_writer.writeFloat(this.fFact);
            //必要な数だけ配列を確保しなおす。
            if (this.cbNumLink > this.punLinkNo.Length)
            {
                this.punLinkNo = new int[this.cbNumLink];
            }
            for (int i = 0; i < this.cbNumLink; i++)
            {
                i_writer.writeUnsignedShort(this.punLinkNo[i]);
            }
            return;
        }

        // 以下は入出力とは関係ない便宜上のデータ
        // 外部からは名前で処理を行い、入出力のときのみ番号で処理する
        public string nTargetName;	// IKターゲットボーン名
        public string nEffName;		// IK先端ボーン名
        public string[] punLinkName = new string[128];// IKを構成するボーンの配列(可変長配列)
    }
}
