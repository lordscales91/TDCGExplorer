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
        // テクスチャBitmapのリスト（同一イメージの重複を許さない）
        List<Bitmap> bmps = new List<Bitmap>();
        // tso番号+テクスチャ名より、bmp_listへのインデックス
        // （Bitmapはアドレスの情報しか持たないので，実体は重複している可能性あり）
        Dictionary<string, Bitmap> bmap = new Dictionary<string, Bitmap>();
        // bmp_listより、出力ファイル名へのインデックス
        Dictionary<Bitmap, string> file_names = new Dictionary<Bitmap, string>();

        // スフィアマップを使うか
        public readonly bool use_spheremap;

        public T2PTextureList(bool use_spheremap)
        {
            this.use_spheremap = use_spheremap;
        }

        public string[] GetFileNameList()
        {
            string[] names = new string[bmps.Count];
            int i = 0;
            foreach (Bitmap bmp in bmps)
            {
                names[i++] = file_names[bmp];
            }
            return names;
        }

        // テクスチャを指定し、Bitmapとして記憶し、参照可能なようにインデックス
        // をつける。ただし、以前記憶したものの中に同じBitmapがあるなら、新規の
        // Bitmapは作成せず、インデックスのみつける。
        public void Add(TSOTex tex, int tso_id)
        {
            if (tex.width == 0 || tex.height == 0)
                return;

            bool toon = (tex.width == 256 && tex.height == 16);

            Bitmap bmp = new Bitmap(tex.width, tex.height);
            SetBitmapBytes(bmp, tex.data);

            Bitmap tmp_bmp = bmp;

            // Toonなら加工してから書き出す
            if (toon)
            {
                bmp = TurnBitmap(bmp);
            }

            // bmps中に同じものがあれば、アドレスのみ参照する
            foreach (Bitmap other_bmp in bmps)
            {
                if (EqualBitmaps(bmp, other_bmp))
                {
                    bmap.Add(GetBitmapCode(tso_id, tex.Name), other_bmp);

                    // 色飛び補完用のスフィアマップ。Toonが同じならスフィアマップも同じ
                    if (toon && use_spheremap)
                    {
                        Bitmap other_sphere_bmp = GetSphere(tso_id, tex.Name);
                        bmap.Add(GetSphereCode(tso_id, tex.Name), other_sphere_bmp);
                    }
                    return;
                }
            }

            // 同じものがなければ、bmpsにBitmapを追加する
            file_names.Add(bmp, string.Format("t{0:D3}.bmp", bmps.Count));
            bmps.Add(bmp);
            bmap.Add(GetBitmapCode(tso_id, tex.Name), bmp);

            // 色飛び補完用のスフィアマップを書き出す
            if (toon && use_spheremap)
            {
                Bitmap sphere_bmp = MakeSphereBitmap(tmp_bmp);
                file_names.Add(sphere_bmp, string.Format("t{0:D3}.sph", bmps.Count));
                bmps.Add(sphere_bmp);
                bmap.Add(GetSphereCode(tso_id, tex.Name), sphere_bmp);
            }
        }

        /// <summary>
        /// 全てのビットマップを書き出します。
        /// </summary>
        /// <param name="dest_path">出力先パス</param>
        public void Save(string dest_path)
        {
            foreach (Bitmap bmp in bmps)
            {
                bmp.Save(dest_path + "/" + file_names[bmp], System.Drawing.Imaging.ImageFormat.Bmp);
            }
        }
        
        string GetBitmapCode(int tso_id, string tex_name)
        {
            return tso_id.ToString() + "-" + tex_name + ".bmp";
        }

        // ビットマップを得る
        public Bitmap GetBitmap(int tso_id, string tex_name)
        {
            Bitmap bmp;
            bmap.TryGetValue(GetBitmapCode(tso_id, tex_name), out bmp);
            return bmp;
        }

        // ビットマップを出力したファイル名を得る
        public string GetFileName(int tso_id, string tex_name)
        {
            Bitmap bmp = GetBitmap(tso_id, tex_name);
    
            if (bmp == null)
                return null;

            string str;
            file_names.TryGetValue(bmp, out str);
            return str;
        }

        public sbyte GetBitmapID(int tso_id, string tex_name)
        {
            Bitmap bmp = GetBitmap(tso_id, tex_name);

            if (bmp == null)
                return -1;

            return (sbyte)bmps.IndexOf(bmp);
        }

        string GetSphereCode(int tso_id, string tex_name)
        {
            return tso_id.ToString() + "-" + tex_name + ".sph";
        }

        // スフィアマップを得る
        public Bitmap GetSphere(int tso_id, string tex_name)
        {
            Bitmap bmp;
            bmap.TryGetValue(GetSphereCode(tso_id, tex_name), out bmp);
            return bmp;
        }

        // スフィアマップを出力したファイル名を得る
        public string GetSphereFileName(int tso_id, string tex_name)
        {
            Bitmap bmp = GetSphere(tso_id, tex_name);

            if (bmp == null)
                return null;

            string str;
            file_names.TryGetValue(bmp, out str);
            return str;
        }

        public sbyte GetSphereID(int tso_id, string tex_name)
        {
            Bitmap bmp = GetSphere(tso_id, tex_name);

            if (bmp == null)
                return -1;

            return (sbyte)bmps.IndexOf(bmp);
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

        // toonテクスチャを最適化する
        private Bitmap TurnBitmap(Bitmap bmp1)
        {
            if (bmp1.Width == 256 && bmp1.Height == 16)
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
            else
            {
                return bmp1;
            }
        }

        // toonテクスチャより、色とび補完用のスフィアマップを作成する
        private Bitmap MakeSphereBitmap(Bitmap bmp1)
        {
            Bitmap bmp2 = new Bitmap(1, 1);

            bmp2.SetPixel(0, 0, bmp1.GetPixel(249, 0));

            return bmp2;
        }
    }
}
