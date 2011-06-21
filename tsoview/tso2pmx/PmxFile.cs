using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace tso2pmx
{
    class PmxFile
    {
        /// <summary>
        /// 指定パスに保存します。
        /// </summary>
        /// <param name="dest_file">パス</param>
        public void Save(string dest_file)
        {
            using (Stream dest_stream = File.Create(dest_file))
                Save(dest_stream);
        }

        public void WritePString(BinaryWriter bw, string str)
        {
            bw.Write((int)str.Length);
            bw.Write(Encoding.UTF8.GetBytes(str));
        }

        /// <summary>
        /// 指定ストリームに保存します。
        /// </summary>
        /// <param name="dest_stream">ストリーム</param>
        public void Save(Stream dest_stream)
        {
            BinaryWriter bw = new BinaryWriter(dest_stream);

            WriteMagic(bw);

            bw.Write((byte)8);
            bw.Write((byte)1); //文字エンコード方式
            bw.Write((byte)0); //追加UV数
            bw.Write((byte)4); //頂点Indexサイズ
            bw.Write((byte)1); //テクスチャIndexサイズ
            bw.Write((byte)1); //材質Indexサイズ
            bw.Write((byte)1); //ボーンIndexサイズ
            bw.Write((byte)1); //モーフIndexサイズ
            bw.Write((byte)1); //剛体Indexサイズ

            WritePString(bw, "model name ja");
            WritePString(bw, "model name en");
            WritePString(bw, "comment ja");
            WritePString(bw, "comment en");

            bw.Write((int)0);//#vertices
            bw.Write((int)0);//#faces
            bw.Write((int)0);//#textures
            bw.Write((int)0);//#materials
            bw.Write((int)0);//#nodes
            bw.Write((int)0);//#morphs
            bw.Write((int)0);//#labels
            bw.Write((int)0);//#rbodies
            bw.Write((int)0);//#joints
        }

        /// <summary>
        /// 'PMX ' とver (2.0)を書き出します。
        /// </summary>
        public static void WriteMagic(BinaryWriter bw)
        {
            bw.Write((byte)0x50);
            bw.Write((byte)0x4d);
            bw.Write((byte)0x58);
            bw.Write((byte)0x20);
            bw.Write(2.0f);
        }
    }
}
