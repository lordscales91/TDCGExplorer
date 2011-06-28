using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Drawing;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

using TDCG;
using TDCGUtils;

namespace Tso2Pmd
{
    class T2PMaterialList
    {
        public List<string> name_list = new List<string>();
        public List<PMD_Material> material_list = new List<PMD_Material>();

        List<TSOFile> TSOList = new List<TSOFile>();
        List<string> cateList = new List<string>();
        T2PTextureList tex_list = new T2PTextureList();
        List<string> toon_name_list = new List<string>();

        public T2PMaterialList(List<TSOFile> TSOList, List<string> cateList)
        {
            this.TSOList = TSOList;
            this.cateList = cateList;

            // テクスチャの準備
            for (int tso_num = 0; tso_num < TSOList.Count; tso_num++)
            foreach (TSOTex tex in TSOList[tso_num].textures)
            {
                tex_list.Add(tex, tso_num);
            }
        }

        public void Save(string path, string file_name, bool spheremap_flag)
        {
            // -----------------------------------------------------
            // テクスチャをBitmapファイルに出力
            tex_list.Save(path, spheremap_flag);

            // -----------------------------------------------------
            // マテリアル名のリストが書かれたファイルを出力
            // ファイルを開く
            System.IO.StreamWriter sw = new System.IO.StreamWriter(
                path + "/" + file_name + ".txt",
                false,
                System.Text.Encoding.GetEncoding("shift_jis"));

            // 書き出す
            foreach (string name in name_list)
                sw.WriteLine(name);

            //閉じる
            sw.Close();
        }
        
        // TSOSubScriptより、PMD_Materialを生成する
        // (ただし、頂点インデックス数は0となっているため、後に設定する必要がある)
        public void Add(int tso_num, int script_num, bool edge, bool spheremap_flag)
        {
            PMD_Material pmd_m = new PMD_Material();

            // スクリプトよりシェーダパラメータを取得
            Shader shader = new Shader();
            shader.Load(TSOList[tso_num].sub_scripts[script_num].lines);

            pmd_m.diffuse = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);
            pmd_m.specular = new Vector4(0.15f, 0.15f, 0.15f, 6.0f);
            pmd_m.ambient = new Vector3(0.5f, 0.5f, 0.5f);

            if (edge == true)
                pmd_m.edge_width = 1;
            else
                pmd_m.edge_width = 0;

            // 頂点インデックス数（0となっているため、後に設定する必要がある）
            pmd_m.vindices_count = 0;

            // colorテクスチャ
            pmd_m.tex_path = tex_list.GetFileName(tso_num, shader.ColorTexName);

            // toonテクスチャ
            string toon_file = tex_list.GetFileName(tso_num, shader.ShadeTexName);
            if (toon_file != null) // 存在しないtoonテクスチャを参照しているパーツがあるのでこれを確認
            {
                if (toon_name_list.IndexOf(toon_file) != -1)
                {
                    // toonテクスチャファイル中でのインデックス
                    pmd_m.toon_tex_id = (sbyte)toon_name_list.IndexOf(toon_file);
                }
                else
                {
                    if (toon_name_list.Count <= 9)
                    {
                        // toonテクスチャとするテクスチャ名を記憶
                        // 後にtoonテクスチャファイル名を出力するときに使用。
                        toon_name_list.Add(toon_file);

                        // toonテクスチャファイル中でのインデックス
                        pmd_m.toon_tex_id = (sbyte)(toon_name_list.Count - 1);
                    }
                    else
                    {
                        // toonテクスチャとするテクスチャ名を記憶
                        // 後にtoonテクスチャファイル名を出力するときに使用。
                        toon_name_list.Add("toon10.bmp"); // 10以上は無理なので、それ以上は全てtoon10.bmp

                        // toonテクスチャファイル中でのインデックス
                        pmd_m.toon_tex_id = 9; // 10以上は無理なので、それ以上は全て9
                    }
                }

                // スフィアマップ
                if (spheremap_flag == true)
                {
                    // toonテクスチャが256×16のサイズなら、スフィアマップを指定する
                    Bitmap toon_bmp = tex_list.GetBitmap(tso_num, shader.ShadeTexName);
                    if (toon_bmp.Width == 256 && toon_bmp.Height == 16)
                    {
                        string sphere_file = Path.ChangeExtension(toon_file, ".sph");
                        pmd_m.tex_path = pmd_m.tex_path + "*" + sphere_file;
                    }
                }
            }
            else
            {
                pmd_m.toon_tex_id = 9;
            }

            // 要素を追加
            name_list.Add(cateList[tso_num] + " " 
                    + TSOList[tso_num].sub_scripts[script_num].Name);
            material_list.Add(pmd_m);
        }

        // 隣り合う同一のマテリアルを統合する
        public void MergeMaterials()
        {
            for (int i = 0; i < material_list.Count - 1; i++)
            {
                if (EqualMaterials(material_list[i], material_list[i + 1]))
                {
                    material_list[i].vindices_count += material_list[i + 1].vindices_count;
                    material_list.RemoveAt(i + 1);
                    name_list.RemoveAt(i + 1);
                    i = 0;
                }
            }
        }

        // ２つのマテリアルが等しいか判定する
        public bool EqualMaterials(PMD_Material m1, PMD_Material m2)
        {
            if (m1.edge_width != m2.edge_width)
                return false;

            if (m1.tex_path != m2.tex_path)
                return false;

            if (m1.toon_tex_id != m2.toon_tex_id)
                return false;

            return true;
        }

        // トゥーンテクスチャファイル名を得る
        public string[] GetToonFileNameList()
        {
            string[] name_list = new string[10];

            if (toon_name_list.Count <= 10)
            {
                for (int i = 0; i < toon_name_list.Count; i++)
                {
                    name_list[i] = toon_name_list[i];
                }
                for (int i = toon_name_list.Count; i < 10; i++)
                {
                    name_list[i] = "toon" + i.ToString("00") + ".bmp";
                }
            }
            else
            {
                for (int i = 0; i < 10; i++)
                {
                    name_list[i] = toon_name_list[i];
                }
            }

            // toonテクスチャがうまく呼び出せない場合に呼び出す、空のtoonテクスチャ
            name_list[9] = "toon10.bmp";

            return name_list;
        }
    }
}
