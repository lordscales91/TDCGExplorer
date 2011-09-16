using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace tso2mqo
{
    using BYTE  = Byte;
    using WORD  = UInt16;
    using DWORD = UInt32;
    using LONG  = Int32;

    [StructLayout(LayoutKind.Sequential, Pack=1)]
    public struct TARGA_HEADER
    {
	    public BYTE     id;
	    public BYTE		colormap;
	    public BYTE		imagetype;
	    public BYTE		unknown0;
	    public BYTE		unknown1;
	    public BYTE		unknown2;
	    public BYTE		unknown3;
	    public BYTE		unknown4;
	    public WORD		x;
	    public WORD		y;
	    public WORD		width;
	    public WORD		height;
	    public BYTE		depth;
	    public BYTE		type;
    };

    [StructLayout(LayoutKind.Sequential, Pack=1)]
    public struct BITMAPFILEHEADER
    {
        public WORD    bfType;
        public DWORD   bfSize;
        public WORD    bfReserved1;
        public WORD    bfReserved2;
        public DWORD   bfOffBits;
    }

    [StructLayout(LayoutKind.Sequential, Pack=1)]
    public struct BITMAPINFOHEADER
    {
        public DWORD      biSize;
        public LONG       biWidth;
        public LONG       biHeight;
        public WORD       biPlanes;
        public WORD       biBitCount;
        public DWORD      biCompression;
        public DWORD      biSizeImage;
        public LONG       biXPelsPerMeter;
        public LONG       biYPelsPerMeter;
        public DWORD      biClrUsed;
        public DWORD      biClrImportant;
    }
}
