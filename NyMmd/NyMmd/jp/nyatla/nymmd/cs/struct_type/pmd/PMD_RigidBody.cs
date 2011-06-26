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
    public class PMD_RigidBody : StructType
    {
	    public String rigidbody_name; // 諸データ：名称 // 頭
	    public int rigidbody_rel_bone_index; // 諸データ：関連ボーン番号 // 03 00 == 3 // 頭
	    public int rigidbody_group_index; // 諸データ：グループ // 00
	    public int rigidbody_group_target; // 諸データ：グループ：対象 // 0xFFFFとの差 // 38 FE
	    public int shape_type; // 形状：タイプ(0:球、1:箱、2:カプセル) // 00 // 球
	    public float shape_w; // 形状：半径(幅) // CD CC CC 3F // 1.6
	    public float shape_h; // 形状：高さ // CD CC CC 3D // 0.1
	    public float shape_d; // 形状：奥行 // CD CC CC 3D // 0.1
	    public MmdVector3 pos_pos = new MmdVector3(); // 位置：位置(x, y, z)
	    public MmdVector3 pos_rot = new MmdVector3(); // 位置：回転(rad(x), rad(y), rad(z))
	    public float rigidbody_weight; // 諸データ：質量 // 00 00 80 3F // 1.0
	    public float rigidbody_pos_dim; // 諸データ：移動減 // 00 00 00 00
	    public float rigidbody_rot_dim; // 諸データ：回転減 // 00 00 00 00
	    public float rigidbody_recoil; // 諸データ：反発力 // 00 00 00 00
	    public float rigidbody_friction; // 諸データ：摩擦力 // 00 00 00 00
	    public int rigidbody_type; // 諸データ：タイプ(0:Bone追従、1:物理演算、2:物理演算(Bone位置合せ)) // 00 // Bone追従

        public void read(DataReader i_reader)
        {
            this.rigidbody_name = i_reader.readAscii(20); // 諸データ：名称 // 頭
            this.rigidbody_rel_bone_index = i_reader.readShort(); // 諸データ：関連ボーン番号 // 03 00 == 3 // 頭
            this.rigidbody_group_index = i_reader.readByte(); // 諸データ：グループ // 00
            this.rigidbody_group_target = i_reader.readShort(); // 諸データ：グループ：対象 // 0xFFFFとの差 // 38 FE
            this.shape_type = i_reader.readByte(); // 形状：タイプ(0:球、1:箱、2:カプセル) // 00 // 球
            this.shape_w = i_reader.readFloat(); // 形状：半径(幅) // CD CC CC 3F // 1.6
            this.shape_h = i_reader.readFloat(); // 形状：高さ // CD CC CC 3D // 0.1
            this.shape_d = i_reader.readFloat(); // 形状：奥行 // CD CC CC 3D // 0.1
            StructReader.read(this.pos_pos, i_reader); // 位置：位置(x, y, z)
            StructReader.read(this.pos_rot, i_reader); // 位置：回転(rad(x), rad(y), rad(z))
            this.rigidbody_weight = i_reader.readFloat(); // 諸データ：質量 // 00 00 80 3F // 1.0
            this.rigidbody_pos_dim = i_reader.readFloat(); // 諸データ：移動減 // 00 00 00 00
            this.rigidbody_rot_dim = i_reader.readFloat(); // 諸データ：回転減 // 00 00 00 00
            this.rigidbody_recoil = i_reader.readFloat(); // 諸データ：反発力 // 00 00 00 00
            this.rigidbody_friction = i_reader.readFloat(); // 諸データ：摩擦力 // 00 00 00 00
            this.rigidbody_type = i_reader.readByte(); // 諸データ：タイプ(0:Bone追従、1:物理演算、2:物理演算(Bone位置合せ)) // 00 // Bone追従

            return;
        }

        public void write(DataWriter i_writer)
        {
            i_writer.writeAscii(this.rigidbody_name, 20); // 諸データ：名称 // 頭
            i_writer.writeShort(this.rigidbody_rel_bone_index); // 諸データ：関連ボーン番号 // 03 00 == 3 // 頭
            i_writer.writeByte(this.rigidbody_group_index); // 諸データ：グループ // 00
            i_writer.writeShort(this.rigidbody_group_target); // 諸データ：グループ：対象 // 0xFFFFとの差 // 38 FE
            i_writer.writeByte(this.shape_type); // 形状：タイプ(0:球、1:箱、2:カプセル) // 00 // 球
            i_writer.writeFloat(this.shape_w); // 形状：半径(幅) // CD CC CC 3F // 1.6
            i_writer.writeFloat(this.shape_h); // 形状：高さ // CD CC CC 3D // 0.1
            i_writer.writeFloat(this.shape_d); // 形状：奥行 // CD CC CC 3D // 0.1
            StructWriter.write(this.pos_pos, i_writer); // 位置：位置(x, y, z)
            StructWriter.write(this.pos_rot, i_writer); // 位置：回転(rad(x), rad(y), rad(z))
            i_writer.writeFloat(this.rigidbody_weight); // 諸データ：質量 // 00 00 80 3F // 1.0
            i_writer.writeFloat(this.rigidbody_pos_dim); // 諸データ：移動減 // 00 00 00 00
            i_writer.writeFloat(this.rigidbody_rot_dim); // 諸データ：回転減 // 00 00 00 00
            i_writer.writeFloat(this.rigidbody_recoil); // 諸データ：反発力 // 00 00 00 00
            i_writer.writeFloat(this.rigidbody_friction); // 諸データ：摩擦力 // 00 00 00 00
            i_writer.writeByte(this.rigidbody_type); // 諸データ：タイプ(0:Bone追従、1:物理演算、2:物理演算(Bone位置合せ)) // 00 // Bone追従

            return;
        }
    }

}
