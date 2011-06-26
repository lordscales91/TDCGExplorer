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
using System.Text;
using System.IO;
using jp.nyatla.nymmd.cs.types;

namespace jp.nyatla.nymmd.cs.struct_type.pmd
{
    public class PMD_Joint : StructType
    {
	    public String joint_name; // 諸データ：名称 // 右髪1
	    public int joint_rigidbody_a; // 諸データ：剛体A
	    public int joint_rigidbody_b; // 諸データ：剛体B
	    public MmdVector3 joint_pos = new MmdVector3(); // 諸データ：位置(x, y, z) // 諸データ：位置合せでも設定可
	    public MmdVector3 joint_rot = new MmdVector3(); // 諸データ：回転(rad(x), rad(y), rad(z))
	    public MmdVector3 constrain_pos_1 = new MmdVector3(); // 制限：移動1(x, y, z)
	    public MmdVector3 constrain_pos_2 = new MmdVector3(); // 制限：移動2(x, y, z)
	    public MmdVector3 constrain_rot_1 = new MmdVector3(); // 制限：回転1(rad(x), rad(y), rad(z))
	    public MmdVector3 constrain_rot_2 = new MmdVector3(); // 制限：回転2(rad(x), rad(y), rad(z))
	    public MmdVector3 spring_pos = new MmdVector3(); // ばね：移動(x, y, z)
        public MmdVector3 spring_rot = new MmdVector3(); // ばね：回転(rad(x), rad(y), rad(z))

        /*
        補足１：
        constrain_pos_1[3]; // 制限：移動1(x, y, z)、constrain_pos_2[3]; // 制限：移動2(x, y, z)
        記録される順番に注意。
        設定ボックスの並びは、移動1x - 移動2x 移動1y - 移動2y 移動1z - 移動2z
        記録される値の並びは、移動1x 移動1y 移動1z 移動2x 移動2y 移動2z
        制限：回転も同様。
        */

        public void read(DataReader i_reader)
        {
            this.joint_name = i_reader.readAscii(20); // 諸データ：名称 // 右髪1
            this.joint_rigidbody_a = i_reader.readInt(); // 諸データ：剛体A
            this.joint_rigidbody_b = i_reader.readInt(); // 諸データ：剛体B
            StructReader.read(this.joint_pos, i_reader); // 諸データ：位置(x, y, z) // 諸データ：位置合せでも設定可
            StructReader.read(this.joint_rot, i_reader); // 諸データ：回転(rad(x), rad(y), rad(z))
            StructReader.read(this.constrain_pos_1, i_reader); // 制限：移動1(x, y, z)
            StructReader.read(this.constrain_pos_2, i_reader); // 制限：移動2(x, y, z)
            StructReader.read(this.constrain_rot_1, i_reader); // 制限：回転1(rad(x), rad(y), rad(z))
            StructReader.read(this.constrain_rot_2, i_reader); // 制限：回転2(rad(x), rad(y), rad(z))
            StructReader.read(this.spring_pos, i_reader); // ばね：移動(x, y, z)
            StructReader.read(this.spring_rot, i_reader); // ばね：回転(rad(x), rad(y), rad(z))

            return;
        }

        public void write(DataWriter i_writer)
        {
            i_writer.writeAscii(this.joint_name, 20); // 諸データ：名称 // 右髪1
            i_writer.writeInt(this.joint_rigidbody_a); // 諸データ：剛体A
            i_writer.writeInt(this.joint_rigidbody_b); // 諸データ：剛体B
            StructWriter.write(this.joint_pos, i_writer); // 諸データ：位置(x, y, z) // 諸データ：位置合せでも設定可
            StructWriter.write(this.joint_rot, i_writer); // 諸データ：回転(rad(x), rad(y), rad(z))
            StructWriter.write(this.constrain_pos_1, i_writer); // 制限：移動1(x, y, z)
            StructWriter.write(this.constrain_pos_2, i_writer); // 制限：移動2(x, y, z)
            StructWriter.write(this.constrain_rot_1, i_writer); // 制限：回転1(rad(x), rad(y), rad(z))
            StructWriter.write(this.constrain_rot_2, i_writer); // 制限：回転2(rad(x), rad(y), rad(z))
            StructWriter.write(this.spring_pos, i_writer); // ばね：移動(x, y, z)
            StructWriter.write(this.spring_rot, i_writer); // ばね：回転(rad(x), rad(y), rad(z))

            return;
        }
    }

}
