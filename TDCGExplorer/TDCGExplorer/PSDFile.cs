using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;

namespace TDCGExplorer
{
    using DWORD = UInt32;
    using LONG  = Int32;
    using WORD  = UInt16;

    public struct BITMAPINFOHEADER
    {
        public DWORD  biSize; 
        public LONG   biWidth; 
        public LONG   biHeight; 
        public WORD   biPlanes; 
        public WORD   biBitCount; 
        public DWORD  biCompression; 
        public DWORD  biSizeImage; 
        public LONG   biXPelsPerMeter; 
        public LONG   biYPelsPerMeter; 
        public DWORD  biClrUsed; 
        public DWORD  biClrImportant; 
    };

    public unsafe class PSDFile
    {
        public Bitmap Bitmap { get { return bmp; } }
        Bitmap bmp = null;

        public void Load(string source_file)
        {
            using (Stream source_stream = File.OpenRead(source_file))
                Load(source_stream);
        }

        public void Load(Stream fs)
        {
            byte[]      buf = new byte[fs.Length];
            fs.Read(buf, 0, buf.Length);
            
            IntPtr  pHBInfo = IntPtr.Zero;
            IntPtr  pHBm    = IntPtr.Zero;

            fixed(byte* p= &buf[0])
            {
                int rc  = GetPicture(p, buf.Length, 1, out pHBInfo, out pHBm, null, 0);

                if(rc != 0)
                    return;

                BITMAPINFOHEADER*   pInfo   = (BITMAPINFOHEADER*)LocalLock(pHBInfo);
                byte*               pBM     = (byte*)LocalLock(pHBm);
                int                 s       = 0;
                Rectangle           r       = new Rectangle(0, 0, pInfo->biWidth, pInfo->biHeight);
                bmp                         = new Bitmap(pInfo->biWidth, pInfo->biHeight, PixelFormat.Format24bppRgb);
                BitmapData          data    = bmp.LockBits(r, ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
                byte*               dst     = (byte*)data.Scan0;
                dst                         +=data.Stride * data.Height;

                switch(pInfo->biBitCount)
                {
                case  8:    break;
                case 16:    break;
                case 24:
                    for(uint y= 0; y < pInfo->biHeight; ++y)
                    {
                        dst             -=data.Stride;

                        for(uint x= 0; x < pInfo->biWidth; ++x)
                        {
                            dst[x*3+0]  = pBM[s++];
                            dst[x*3+1]  = pBM[s++];
                            dst[x*3+2]  = pBM[s++];
                        }
                    }
                    break;

                case 32:
                    for(uint y= 0; y < pInfo->biHeight; ++y)
                    {
                        dst             -=data.Stride;

                        for(uint x= 0; x < pInfo->biWidth; ++x)
                        {
                            dst[x*3+0]  = pBM[s++];
                            dst[x*3+1]  = pBM[s++];
                            dst[x*3+2]  = pBM[s++];
                            ++s;
                        }
                    }
                    break;
                }

                bmp.UnlockBits(data);

                LocalUnlock(pHBInfo);
                LocalUnlock(pHBm);
            }
        }

        [DllImport("kernel32.dll")]
        public extern static void* LocalLock(IntPtr handle);

        [DllImport("kernel32.dll")]
        public extern static int LocalUnlock(IntPtr handle);

        [DllImport("ifpsd.spi")]
        public extern static int GetPicture(byte* file, int len, uint flag, out IntPtr pHBInfo, out IntPtr pHBm, void* lpPrgressCallback, uint lData); 

      //[DllImport("axpsd.spi")]
      //public extern static int GetPicture(string file, int len, uint flag, out IntPtr pHBInfo, out IntPtr pHBm, void* lpPrgressCallback, uint lData); 
    }
}
