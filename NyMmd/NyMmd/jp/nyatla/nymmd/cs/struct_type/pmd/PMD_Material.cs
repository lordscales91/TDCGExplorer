using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using jp.nyatla.nymmd.cs.types;


namespace jp.nyatla.nymmd.cs.struct_type.pmd
{
    public class PMD_Material : StructType
    {        
        public MmdColor4 col4Diffuse = new MmdColor4();
        public float fShininess;
        public MmdColor3 col3Specular = new MmdColor3();
        public MmdColor3 col3Ambient = new MmdColor3();
        public int toon_index; // toon??.bmp // 0.bmp:0xFF, 1(01).bmp:0x00 ・・・ 10.bmp:0x09
        public int edge_flag; // 輪郭、影
        public int ulNumIndices;		// この材質に対応する頂点数
        public String szTextureFileName;	// テクスチャファイル名
 
        public void read(DataReader i_reader)
        {
            StructReader.read(this.col4Diffuse, i_reader);
            this.fShininess = i_reader.readFloat();
            StructReader.read(this.col3Specular, i_reader);
            StructReader.read(this.col3Ambient, i_reader);
            this.toon_index = i_reader.readByte(); // toon??.bmp // 0.bmp:0xFF, 1(01).bmp:0x00 ・・・ 10.bmp:0x09
            this.edge_flag = i_reader.readByte(); // 輪郭、影
            this.ulNumIndices = i_reader.readInt();
            this.szTextureFileName = i_reader.readAscii(20);
            return;
        }

        public void write(DataWriter i_writer)
        {
            StructWriter.write(this.col4Diffuse, i_writer);
            i_writer.writeFloat(this.fShininess);
            StructWriter.write(this.col3Specular, i_writer);
            StructWriter.write(this.col3Ambient, i_writer);
            i_writer.writeByte(this.toon_index); // toon??.bmp // 0.bmp:0xFF, 1(01).bmp:0x00 ・・・ 10.bmp:0x09
            i_writer.writeByte(this.edge_flag); // 輪郭、影
            i_writer.writeInt(this.ulNumIndices);
            i_writer.writeAscii(this.szTextureFileName, 20);
            return;
        }
    }
}
