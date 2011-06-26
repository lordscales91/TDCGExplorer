using System;
using System.Collections.Generic;
using System.Text;
using jp.nyatla.nymmd.cs.types;

namespace jp.nyatla.nymmd.cs.struct_type.pmd
{
    public class PMD_Vertex : StructType
    {
        public MmdVector3 vec3Pos = new MmdVector3();	// 座標
        public MmdVector3 vec3Normal = new MmdVector3();	// 法線ベクトル
        public MmdTexUV uvTex = new MmdTexUV();		// テクスチャ座標

        internal int[] unBoneNo = new int[2];	// ボーン番号
        public int cbWeight;		// ブレンドの重み (0～100％)
        public int cbEdge;			// エッジフラグ

        public void read(DataReader i_reader)
        {
            StructReader.read(this.vec3Pos, i_reader);
            StructReader.read(this.vec3Normal, i_reader);
            StructReader.read(this.uvTex, i_reader);
            this.unBoneNo[0] = i_reader.readUnsignedShort();
            this.unBoneNo[1] = i_reader.readUnsignedShort();
            this.cbWeight = (byte)i_reader.read();
            this.cbEdge = (byte)i_reader.read();
            return;
        }

        public void write(DataWriter i_writer)
        {
            StructWriter.write(this.vec3Pos, i_writer);
            StructWriter.write(this.vec3Normal, i_writer);
            StructWriter.write(this.uvTex, i_writer);
            i_writer.writeUnsignedShort(this.unBoneNo[0]);
            i_writer.writeUnsignedShort(this.unBoneNo[1]);
            i_writer.write(this.cbWeight);
            i_writer.write(this.cbEdge);
            return;
        }

        // 以下は入出力とは関係ない便宜上のデータ
        // 外部からは名前で処理を行い、入出力のときのみ番号で処理する
        public string[] unBoneName = new string[2];	// ボーン番号
    }

}
