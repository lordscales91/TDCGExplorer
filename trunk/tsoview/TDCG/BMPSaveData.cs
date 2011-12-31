using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Drawing;

namespace TDCG
{
class BMPSaveData
{
    public string[] names = new string[32];
    public float[] sliders = new float[12];

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
        int nbyte  = bmpData.Stride * bmp.Height;
        byte[] bytes = new byte[nbyte];

        // Copy the RGB values into the array.
        System.Runtime.InteropServices.Marshal.Copy(ptr, bytes, 0, nbyte);

        // Unlock the bits.
        bmp.UnlockBits(bmpData);

        int stride = bmpData.Stride;
        int height = bmpData.Height;

        byte[] data = new byte[stride * height / 8];
        int offset = 0;

        for (int y = height-1; y >=0; y--)
        {
            for (int x = 0; x < stride; x += 8)
            {
                int i = y * stride + x;
                byte c = (byte)(bytes[i + 0] & 0x1);
                c |= (byte)((bytes[i + 1] & 0x1) << 1);
                c |= (byte)((bytes[i + 2] & 0x1) << 2);
                c |= (byte)((bytes[i + 3] & 0x1) << 3);
                c |= (byte)((bytes[i + 4] & 0x1) << 4);
                c |= (byte)((bytes[i + 5] & 0x1) << 5);
                c |= (byte)((bytes[i + 6] & 0x1) << 6);
                c |= (byte)((bytes[i + 7] & 0x1) << 7);
                data[offset++] = c;
            }
        }

        Encoding enc = Encoding.GetEncoding("Shift_JIS");
        for (int i = 0; i < 32; i++)
            names[i] = enc.GetString(data, i * 32, 32);

        for (int i = 0; i < 12; i++)
            sliders[i] = BitConverter.ToSingle(data, 32 * 32 + 4 * i);
    }

    public float GetSliderValue(int index)
    {
        return sliders[index];
    }
}
}
