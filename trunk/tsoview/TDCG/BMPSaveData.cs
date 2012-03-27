using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Drawing;

namespace TDCG
{
    /// <summary>
    /// ビットマップに埋め込まれたパラメータを扱います。
    /// </summary>
    public class BMPSaveData
    {
        Bitmap bitmap;
        byte[] savedata;

        /// <summary>
        /// 指定ストリームからビットマップを読み込みます。
        /// </summary>
        /// <param name="stream">ストリーム</param>
        public void Read(Stream stream)
        {
            bitmap = new Bitmap(stream);

            // Lock the bitmap's bits.  
            Rectangle rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
            System.Drawing.Imaging.BitmapData bitmapData =
                bitmap.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadOnly,
                bitmap.PixelFormat);

            // Get the address of the first line.
            IntPtr ptr = bitmapData.Scan0;

            // Declare an array to hold the bytes of the bitmap.
            int stride = bitmapData.Stride;
            int height = bitmap.Height;
            int nbyte  = stride * height;
            byte[] bytes = new byte[nbyte];

            // Copy the RGB values into the array.
            System.Runtime.InteropServices.Marshal.Copy(ptr, bytes, 0, nbyte);

            // Unlock the bits.
            bitmap.UnlockBits(bitmapData);

            savedata = new byte[nbyte / 8];

            int savedata_offset = 0;

            for (int y = height - 1; y >= 0; y--)
            {
                for (int x = 0; x < stride; x += 8)
                {
                    int i = y * stride + x;
                    byte c = 0;
                    for (int w = 0; w < 8; w++)
                        c |= (byte)((bytes[i + w] & 0x01) << w);
                    savedata[savedata_offset++] = c;
                }
            }
        }

        static Encoding enc = Encoding.GetEncoding("Shift_JIS");

        /// <summary>
        /// 指定添字のファイル名を得ます。
        /// </summary>
        /// <param name="index">添字</param>
        /// <returns>ファイル名</returns>
        public string GetFileName(int index)
        {
            int len = 32;
            for (int i = 0; i < 32; i++)
            {
                if (savedata[index * 32 + i] == 0)
                {
                    len = i;
                    break;
                }
            }
            return enc.GetString(savedata, index * 32, len);
        }

        /// <summary>
        /// 指定添字のスライダ値を得ます。
        /// </summary>
        /// <param name="index">添字</param>
        /// <returns>スライダ値</returns>
        public float GetSliderValue(int index)
        {
            return BitConverter.ToSingle(savedata, 32 * 32 + index * 4);
        }

        /// <summary>
        /// 指定添字のバイト配列を得ます。
        /// </summary>
        /// <param name="index">添字</param>
        /// <returns>バイト配列</returns>
        public byte[] GetBytes(int index)
        {
            byte[] bytes = new byte[4];
            Array.Copy(savedata, 32 * 32 + index * 4, bytes, 0, 4);
            return bytes;
        }

        /// <summary>
        /// 指定添字のファイル名を設定します。
        /// </summary>
        /// <param name="index">添字</param>
        /// <param name="file">ファイル名</param>
        public void SetFileName(int index, string file)
        {
            byte[] bytes = enc.GetBytes(file);
            Array.Resize(ref bytes, 32);
            Array.Copy(bytes, 0, savedata, index * 32, 32);
        }

        /// <summary>
        /// 指定添字のスライダ値を設定します。
        /// </summary>
        /// <param name="index">添字</param>
        /// <param name="ratio">スライダ値</param>
        public void SetSliderValue(int index, float ratio)
        {
            byte[] bytes = BitConverter.GetBytes(ratio);
            Array.Copy(bytes, 0, savedata, 32 * 32 + index * 4, 4);
        }

        /// <summary>
        /// 指定添字のバイト配列を設定します。
        /// </summary>
        /// <param name="index">添字</param>
        /// <param name="bytes">バイナリ値</param>
        public void SetBytes(int index, byte[] bytes)
        {
            Array.Copy(bytes, 0, savedata, 32 * 32 + index * 4, 4);
        }

        /// <summary>
        /// 指定パスにビットマップを書き出します。
        /// </summary>
        /// <param name="file">パス</param>
        public void Save(string file)
        {
            // Lock the bitmap's bits.  
            Rectangle rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
            System.Drawing.Imaging.BitmapData bitmapData =
                bitmap.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite,
                bitmap.PixelFormat);

            // Get the address of the first line.
            IntPtr ptr = bitmapData.Scan0;

            // Declare an array to hold the bytes of the bitmap.
            int stride = bitmapData.Stride;
            int height = bitmap.Height;
            int nbyte  = stride * height;
            byte[] bytes = new byte[nbyte];

            // Copy the RGB values into the array.
            System.Runtime.InteropServices.Marshal.Copy(ptr, bytes, 0, nbyte);

            int savedata_offset = 0;

            for (int y = height - 1; y >= 0; y--)
            {
                for (int x = 0; x < stride; x += 8)
                {
                    int i = y * stride + x;
                    byte c = savedata[savedata_offset];
                    for (int w = 0; w < 8; w++)
                        if ((c & (0x01 << w)) == (0x01 << w))
                            bytes[i + w] |= 0x01;
                        else
                            bytes[i + w] &= 0xFE;
                    savedata_offset++;
                }
            }

            // Copy the RGB values back to the bitmap
            System.Runtime.InteropServices.Marshal.Copy(bytes, 0, ptr, nbyte);

            // Unlock the bits.
            bitmap.UnlockBits(bitmapData);

            bitmap.Save(file);
        }
    }
}
