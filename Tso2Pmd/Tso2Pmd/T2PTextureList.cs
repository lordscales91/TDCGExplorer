using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Drawing;
using System.Runtime.InteropServices;

using TDCG;

namespace Tso2Pmd
{
    public class T2PTextureList
    {
        // 保持リスト
        List<PMD_Texture> items = new List<PMD_Texture>();
        // 辞書
        Dictionary<string, PMD_Texture> map = new Dictionary<string, PMD_Texture>();

        // スフィアマップを使うか
        public readonly bool use_spheremap;

        public T2PTextureList(bool use_spheremap)
        {
            this.use_spheremap = use_spheremap;
        }

        public string[] GetFileNameList()
        {
            string[] names = new string[items.Count];
            int i = 0;
            foreach (PMD_Texture tex in items)
            {
                names[i++] = tex.FileName;
            }
            return names;
        }

        public void Add(TSOTex tso_tex, int tso_num)
        {
            if (tso_tex.width == 0 || tso_tex.height == 0)
                return;

            Bitmap bmp = new Bitmap(tso_tex.width, tso_tex.height);
            SetBitmapBytes(bmp, tso_tex.data);

            PMD_Texture tex = new PMD_Texture(GenName(tso_num, tso_tex.Name));
            tex.Bitmap = bmp;

            foreach (PMD_Texture other in items)
            {
                if (other.Bitmap != null && EqualBitmaps(bmp, other.Bitmap))
                {
                    map[tex.Code] = other;

                    if (use_spheremap && other.IsToon)
                    {
                        PMD_Texture tex_sphere = new PMD_Texture(GenName(tso_num, tso_tex.Name), tex);
                        
                        map[tex_sphere.Code] = other.Sphere;
                    }
                    return;
                }
            }

            tex.SetID((sbyte)items.Count);
            items.Add(tex);
            map[tex.Code] = tex;

            if (use_spheremap && tex.IsToon)
            {
                PMD_Texture tex_sphere = new PMD_Texture(GenName(tso_num, tso_tex.Name), tex);
                
                tex_sphere.SetID((sbyte)items.Count);
                items.Add(tex_sphere);
                map[tex_sphere.Code] = tex_sphere;
            }
        }

        /// <summary>
        /// 全てのビットマップを書き出します。
        /// </summary>
        /// <param name="dest_path">出力先パス</param>
        public void Save(string dest_path)
        {
            foreach (PMD_Texture texture in items)
            {
                texture.Save(dest_path);
            }
        }
        
        // byte[]をBitmapに変換する
        private void SetBitmapBytes(Bitmap bmp, byte[] bytes)
        {
            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            System.Drawing.Imaging.BitmapData bmpData =
                bmp.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite,
                bmp.PixelFormat);

            // Bitmapの先頭アドレスを取得
            IntPtr ptr = bmpData.Scan0;

            // Bitmapへコピー
            Marshal.Copy(bytes, 0, ptr, bytes.Length);

            bmp.UnlockBits(bmpData);
        }

        // ２つのBitmapが等しいか判定する
        private bool EqualBitmaps(Bitmap bmp1, Bitmap bmp2)
        {
            if (bmp1.Size != bmp2.Size)
                return false;

            for (int x = 0; x < bmp1.Width; x++)
                for (int y = 0; y < bmp1.Height; y++)
                    if (bmp1.GetPixel(x, y) != bmp2.GetPixel(x, y)) 
                        return false;

            return true;
        }

        string GenName(int tso_num, string name)
        {
            return tso_num.ToString() + "-" + name;
        }

        string GenBitmapCode(int tso_num, string name)
        {
            return GenName(tso_num, name) + ".bmp";
        }

        string GenSphereCode(int tso_num, string name)
        {
            return GenName(tso_num, name) + ".sph";
        }

        public sbyte GetBitmapID(int tso_num, string name)
        {
            string code = GenBitmapCode(tso_num, name);
            PMD_Texture tex;
            if (map.TryGetValue(code, out tex))
                return tex.ID;
            else
                return -1;
        }

        public sbyte GetSphereID(int tso_num, string name)
        {
            string code = GenSphereCode(tso_num, name);
            PMD_Texture tex;
            if (map.TryGetValue(code, out tex))
                return tex.ID;
            else
                return -1;
        }
    }

    public class PMD_Texture
    {
        sbyte id;
        string name;
        Bitmap bmp;
        PMD_Texture toon = null;
        PMD_Texture sphere = null;

        /// <summary>
        /// ID
        /// </summary>
        public sbyte ID { get { return id; } }

        /// <summary>
        /// IDを設定します。
        /// </summary>
        /// <param name="id">ID</param>
        public void SetID(sbyte id)
        {
            this.id = id;
        }

        /// <summary>
        /// 元のテクスチャを保持するビットマップ
        /// </summary>
        public Bitmap Bitmap { get { return bmp; } set { bmp = value; } }

        /// <summary>
        /// Toonから見たスフィアマップ
        /// </summary>
        public PMD_Texture Sphere { get { return sphere; } }
        
        /// <summary>
        /// スフィアマップを設定します。
        /// </summary>
        /// <param name="sphere">スフィアマップ</param>
        public void SetSphere(PMD_Texture sphere)
        {
            this.sphere = sphere;
        }

        /// <summary>
        /// テクスチャを登録します。
        /// </summary>
        /// <param name="name">テクスチャ名</param>
        public PMD_Texture(string name)
        {
            this.name = name;
        }

        /// <summary>
        /// テクスチャを登録します。
        /// スフィアマップを登録する場合は元になるToonを指定します。
        /// </summary>
        /// <param name="name">テクスチャ名</param>
        /// <param name="toon">Toon</param>
        public PMD_Texture(string name, PMD_Texture toon)
        {
            this.name = name;
            this.toon = toon;
            toon.SetSphere(this);
        }

        /// <summary>
        /// Toonであるか
        /// </summary>
        public bool IsToon
        {
            get { return bmp != null && bmp.Width == 256 && bmp.Height == 16; }
        }

        /// <summary>
        /// スフィアマップであるか
        /// </summary>
        public bool IsSphere { get { return toon != null; } }

        public string FileExtension
        {
            get { return IsSphere ? ".sph" : ".bmp"; }
        }

        public string Code
        {
            get { return name + FileExtension; }
        }

        public string FileName
        {
            get { return string.Format("t{0:D3}", ID) + FileExtension; }
        }

        public void Save(string dest_path)
        {
            Bitmap saved_bmp = bmp;

            if (IsToon)
                saved_bmp = TurnBitmap(bmp);

            if (IsSphere)
                saved_bmp = MakeSphereBitmap(toon.Bitmap);

            saved_bmp.Save(dest_path + "/" + FileName, System.Drawing.Imaging.ImageFormat.Bmp);
        }

        // Toonを最適化する
        Bitmap TurnBitmap(Bitmap bmp1)
        {
            Bitmap bmp2 = new Bitmap(16, 250);

            for (int i = 0; i < 250; i++)
            {
                Color c = bmp1.GetPixel(i, 0);

                for (int j = 0; j < 16; j++)
                    bmp2.SetPixel(j, 250 - (i + 1), c);
            }

            return bmp2;
        }

        // Toonから色とび補完用のスフィアマップを作成する
        Bitmap MakeSphereBitmap(Bitmap bmp1)
        {
            Bitmap bmp2 = new Bitmap(1, 1);

            bmp2.SetPixel(0, 0, bmp1.GetPixel(249, 0));

            return bmp2;
        }
    }
}
