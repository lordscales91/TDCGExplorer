using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Drawing;

namespace TDCG
{
    class BMPSaveData
    {
        byte[] data;

        public void Read(Stream stream)
        {
            Bitmap bmp = new Bitmap(stream);

            // Lock the bitmap's bits.  
            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            System.Drawing.Imaging.BitmapData bmpData =
                bmp.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadOnly,
                bmp.PixelFormat);

            // Get the address of the first line.
            IntPtr ptr = bmpData.Scan0;

            // Declare an array to hold the bytes of the bitmap.
            int stride = bmpData.Stride;
            int height = bmp.Height;
            int nbyte  = stride * height;
            byte[] bytes = new byte[nbyte];

            // Copy the RGB values into the array.
            System.Runtime.InteropServices.Marshal.Copy(ptr, bytes, 0, nbyte);

            // Unlock the bits.
            bmp.UnlockBits(bmpData);

            data = new byte[nbyte / 8];
            int data_offset = 0;

            for (int y = height - 1; y >= 0; y--)
            {
                for (int x = 0; x < stride; x += 8)
                {
                    int i = y * stride + x;
                    byte c = 0;
                    c |= (byte)((bytes[i + 0] & 0x1) << 0);
                    c |= (byte)((bytes[i + 1] & 0x1) << 1);
                    c |= (byte)((bytes[i + 2] & 0x1) << 2);
                    c |= (byte)((bytes[i + 3] & 0x1) << 3);
                    c |= (byte)((bytes[i + 4] & 0x1) << 4);
                    c |= (byte)((bytes[i + 5] & 0x1) << 5);
                    c |= (byte)((bytes[i + 6] & 0x1) << 6);
                    c |= (byte)((bytes[i + 7] & 0x1) << 7);
                    data[data_offset++] = c;
                }
            }
        }

        static Encoding enc = Encoding.GetEncoding("Shift_JIS");

        public string GetFileName(int index)
        {
            return enc.GetString(data, index * 32, 32);
        }

        public float GetSliderValue(int index)
        {
            return BitConverter.ToSingle(data, 32 * 32 + 4 * index);
        }
    }
}
