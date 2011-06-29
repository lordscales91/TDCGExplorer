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
        public List<PMD_Material> materials = new List<PMD_Material>();

        List<TSOFile> tsos;
        List<string> categories;
        T2PTextureList tex_list;
        List<string> toon_names = new List<string>();

        public T2PMaterialList(List<TSOFile> tsos, List<string> categories)
        {
            this.tsos = tsos;
            this.categories = categories;

            // テクスチャを準備
            tex_list = new T2PTextureList();
            int tso_num = 0;
            foreach (TSOFile tso in tsos)
            {
                foreach (TSOTex tex in tso.textures)
                {
                    tex_list.Add(tex, tso_num);
                }
                tso_num++;
            }
        }

        public void Save(string dest_path, string file_name, bool use_spheremap)
        {
            tex_list.Save(dest_path, use_spheremap);
            SaveNamesToFile(dest_path + "/" + file_name + ".txt");
        }

        /// <summary>
        /// マテリアル名のリストを保存します。
        /// </summary>
        /// <param name="dest_file">保存ファイル名</param>
        void SaveNamesToFile(string dest_file)
        {
            using (StreamWriter sw = new StreamWriter(dest_file, false,
                System.Text.Encoding.GetEncoding("shift_jis")))
            {
                foreach (PMD_Material material in materials)
                    sw.WriteLine(material.name);
            }
        }
        
        // TSOSubScriptより、PMD_Materialを生成する
        // (ただし、頂点インデックス数は0となっているため、後に設定する必要がある)
        public void Add(int tso_num, int script_num, bool use_edge, bool use_spheremap)
        {
            PMD_Material pmd_m = new PMD_Material();

            pmd_m.name = categories[tso_num] + " " + tsos[tso_num].sub_scripts[script_num].Name;

            // スクリプトよりシェーダパラメータを取得
            Shader shader = new Shader();
            shader.Load(tsos[tso_num].sub_scripts[script_num].lines);

            pmd_m.diffuse = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);
            pmd_m.specular = new Vector4(0.15f, 0.15f, 0.15f, 6.0f);
            pmd_m.ambient = new Vector3(0.5f, 0.5f, 0.5f);

            if (use_edge)
                pmd_m.edge_width = 1;
            else
                pmd_m.edge_width = 0;

            // 頂点インデックス数（0となっているため、後に設定する必要がある）
            pmd_m.vindices_count = 0;

            // colorテクスチャ
            pmd_m.tex_file = tex_list.GetFileName(tso_num, shader.ColorTexName);

            // toonテクスチャ
            string toon_file = tex_list.GetFileName(tso_num, shader.ShadeTexName);
            if (toon_file != null) // 存在しないtoonテクスチャを参照しているパーツがあるのでこれを確認
            {
                if (toon_names.IndexOf(toon_file) != -1)
                {
                    // toonテクスチャファイル中でのインデックス
                    pmd_m.tex_toon_id = (sbyte)toon_names.IndexOf(toon_file);
                }
                else
                {
                    if (toon_names.Count <= 9)
                    {
                        // toonテクスチャとするテクスチャ名を記憶
                        // 後にtoonテクスチャファイル名を出力するときに使用。
                        toon_names.Add(toon_file);

                        // toonテクスチャファイル中でのインデックス
                        pmd_m.tex_toon_id = (sbyte)(toon_names.Count - 1);
                    }
                    else
                    {
                        // toonテクスチャとするテクスチャ名を記憶
                        // 後にtoonテクスチャファイル名を出力するときに使用。
                        toon_names.Add("toon10.bmp"); // 10以上は無理なので、それ以上は全てtoon10.bmp

                        // toonテクスチャファイル中でのインデックス
                        pmd_m.tex_toon_id = 9; // 10以上は無理なので、それ以上は全て9
                    }
                }

                // スフィアマップを使う
                if (use_spheremap)
                {
                    // toonテクスチャが256×16のサイズなら、スフィアマップを指定する
                    Bitmap toon_bmp = tex_list.GetBitmap(tso_num, shader.ShadeTexName);
                    if (toon_bmp.Width == 256 && toon_bmp.Height == 16)
                    {
                        string sphere_file = Path.ChangeExtension(toon_file, ".sph");
                        pmd_m.tex_file = pmd_m.tex_file + "*" + sphere_file;
                    }
                }
            }
            else
            {
                pmd_m.tex_toon_id = 9;
            }

            // 要素を追加
            materials.Add(pmd_m);
        }

        // 隣り合う同一のマテリアルを統合する
        public void UniqueMaterials()
        {
            for (int i = 0; i < materials.Count - 1; i++)
            {
                if (EqualMaterials(materials[i], materials[i + 1]))
                {
                    materials[i].vindices_count += materials[i + 1].vindices_count;
                    materials.RemoveAt(i + 1);
                    i = 0;
                }
            }
        }

        // ２つのマテリアルが等しいか判定する
        public bool EqualMaterials(PMD_Material m1, PMD_Material m2)
        {
            if (m1.edge_width != m2.edge_width)
                return false;

            if (m1.tex_file != m2.tex_file)
                return false;

            if (m1.tex_toon_id != m2.tex_toon_id)
                return false;

            return true;
        }

        // Toonテクスチャファイル名を得る
        public string[] GetToonFileNameList()
        {
            string[] names = new string[10];

            if (toon_names.Count <= 10)
            {
                for (int i = 0; i < toon_names.Count; i++)
                    names[i] = toon_names[i];

                for (int i = toon_names.Count; i < 10; i++)
                    names[i] = "toon" + i.ToString("00") + ".bmp";
            }
            else
            {
                for (int i = 0; i < 10; i++)
                    names[i] = toon_names[i];
            }

            // toonテクスチャがうまく呼び出せない場合に呼び出す、空のtoonテクスチャ
            names[9] = "toon10.bmp";

            return names;
        }
    }
}
