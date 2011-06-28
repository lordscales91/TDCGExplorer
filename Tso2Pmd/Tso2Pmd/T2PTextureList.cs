using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

using TDCG;

namespace Tso2Pmd
{
    public class T2PTextureList
    {
        // テクスチャBitmapのリスト（同一イメージの重複を許さない）
        List<Bitmap> bmp_list = new List<Bitmap>();
        // tso番号+テクスチャ名より、bmp_listへのインデックス
        // （Bitmapはアドレスの情報しか持たないので，実体は重複している可能性あり）
        Dictionary<string, Bitmap> bmp_index = new Dictionary<string, Bitmap>();
        // bmp_listより、出力ファイル名へのインデックス
        Dictionary<Bitmap, string> file_name = new Dictionary<Bitmap, string>();

        // テクスチャを指定し、Bitmapとして記憶し、参照可能なようにインデックス
        // をつける。ただし、以前記憶したものの中に同じBitmapがあるなら、新規の
        // Bitmapは作成せず、インデックスのみつける。
        public void Add(TSOTex tex, int tso_id)
        {
            // 幅が0な、不正なテクスチャの場合処理しない
            if (tex.width == 0 || tex.height == 0) return;

            // byte列をBitmapに変換
            Bitmap bmp = new Bitmap(tex.width, tex.height);
            ByteArray2Bitmap(tex.data, bmp);

            // bmp_listと比較して、同じものがあればそれのアドレスのみ参照しておく
            foreach (Bitmap tmp_bmp in bmp_list)
            {
                if (EqualBitmaps(bmp, tmp_bmp) == 0)
                {
                    bmp_index.Add(tso_id.ToString() + "-" + tex.Name, tmp_bmp);
                    return;
                }
            }

            // 同じものがなければ、新規にbmp_listにBitmapを追加
            bmp_list.Add(bmp);
            bmp_index.Add(tso_id.ToString() + "-" + tex.Name, bmp);

            // 同時にこれを出力するときのファイル名を作成
            file_name.Add(bmp, "t" + (bmp_list.Count - 1).ToString("000") + ".bmp");
        }

        // bmp_listを全てファイルに書き出す
        public void Save(string outputFilePath, bool spheremap_flag)
        {
            foreach (Bitmap bmp in bmp_list)
            {
                if (bmp.Width == 256 && bmp.Height == 16)
                {
                    // テクスチャがtoonテクスチャなら加工（最適化）してから、書き出す
                    Bitmap toon_bmp = TurnBitmap(bmp);
                    toon_bmp.Save(outputFilePath + "/" + file_name[bmp],
                        System.Drawing.Imaging.ImageFormat.Bmp);

                    // 色飛び補完用のスフィアマップを書き出す
                    if (spheremap_flag == true)
                    {
                        Bitmap sphere_bmp = MakeSphereBitmap(bmp);
                        string sphere_file_name
                            = System.Text.RegularExpressions.Regex.Replace(
                                file_name[bmp], ".bmp", ".sph");
                        sphere_bmp.Save(outputFilePath + "/" + sphere_file_name,
                            System.Drawing.Imaging.ImageFormat.Bmp);
                    }
                }
                else
                {
                    bmp.Save(outputFilePath + "/" + file_name[bmp],
                        System.Drawing.Imaging.ImageFormat.Bmp);
                }
            }
        }

        // ビットマップを得る
        public Bitmap GetBitmap(int tso_id, string tex_name)
        {
            Bitmap bmp;
            bmp_index.TryGetValue(tso_id.ToString() + "-" + tex_name, out bmp);
            return bmp;
        }

        // テクスチャを出力したファイル名を得る
        public string GetFileName(int tso_id, string tex_name)
        {
            Bitmap bmp; 
            bmp_index.TryGetValue(tso_id.ToString() + "-" + tex_name, out bmp);
            if (bmp == null) return null;

            string str;
            file_name.TryGetValue(bmp, out str);
            return str;
        }

        // byte[]をBitmapに変換する
        private void ByteArray2Bitmap(byte[] rgbValues, Bitmap bmp)
        {
            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            System.Drawing.Imaging.BitmapData bmpData =
                bmp.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite,
                PixelFormat.Format32bppArgb);

            // Bitmapの先頭アドレスを取得
            IntPtr ptr = bmpData.Scan0;

            // Bitmapへコピー
            Marshal.Copy(rgbValues, 0, ptr, rgbValues.Length);

            bmp.UnlockBits(bmpData);
        }

        // ２つのBitmapが等しいか判定する
        private int EqualBitmaps(Bitmap bmp1, Bitmap bmp2)
        {
            if (bmp1.Height != bmp2.Height) return -1;

            if (bmp1.Width != bmp2.Width) return -1;

            for (int i = 0; i < bmp1.Width; i++)
            {
                for (int j = 0; j < bmp1.Height; j++)
                {
                    if (bmp1.GetPixel(i, j) != bmp2.GetPixel(i, j)) return -1;
                }
            }

            return 0;
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
