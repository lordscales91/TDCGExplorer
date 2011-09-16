using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace tso2mqo
{
    public enum PrimType
    {
	    PT_LIST,
	    PT_STRIP,
	    PT_FAN
    }

    public unsafe struct PrimitiveGroup
    {
	    public PrimType type;
	    public uint     numIndices;
	    public ushort*  indices;
    }

    public unsafe class NvTriStrip
    {
        public static ushort[] Optimize(ushort[] triangles)
        {
            fixed(ushort* p= &triangles[0])
            {
	            SetStitchStrips(true);

                PrimitiveGroup* pg  = null;
                ushort          num = 0;
                bool            rc  = GenerateStrips(p, (uint)triangles.Length, &pg, &num, false);

                if(!rc)                                 throw new Exception();

                try
                {
                    if(num != 1)                        throw new Exception();
                    if(pg[0].type != PrimType.PT_STRIP) throw new Exception();

                    ushort[]    nidx= new ushort[pg[0].numIndices];

                    for(int i= 0; i < nidx.Length; ++i)
                        nidx[i] = pg[0].indices[i];

                    return nidx;
                } finally
                {
                    DeletePrimitiveGroup(pg);
                }
            }
        }

      //[DllImport("NvTriStrip.dll")] public extern static int GetPicture(byte* file, int len, uint flag, out IntPtr pHBInfo, out IntPtr pHBm, void* lpPrgressCallback, uint lData); 
        [DllImport("NvTriStrip.dll")] public extern static void EnableRestart(uint _restartVal);
        [DllImport("NvTriStrip.dll")] public extern static void DisableRestart();
        [DllImport("NvTriStrip.dll")] public extern static void SetListsOnly(bool _bListsOnly);
        [DllImport("NvTriStrip.dll")] public extern static void SetCacheSize(uint _cacheSize);
        [DllImport("NvTriStrip.dll")] public extern static void SetStitchStrips(bool _bStitchStrips);
        [DllImport("NvTriStrip.dll")] public extern static void SetMinStripSize(uint _minStripSize);
      //[DllImport("NvTriStrip.dll")] public extern static void Cleanup(NvStripInfoVec& tempStrips, NvFaceInfoVec& tempFaces);
        [DllImport("NvTriStrip.dll")] public extern static bool SameTriangle(ushort firstTri0, ushort firstTri1, ushort firstTri2, ushort secondTri0, ushort secondTri1, ushort secondTri2);
      //[DllImport("NvTriStrip.dll")] public extern static bool TestTriangle(ushort v0, ushort v1, ushort v2, const std::vector<NvFaceInfo>* in_bins, const int NUMBINS);
        [DllImport("NvTriStrip.dll")] public extern static void DeletePrimitiveGroup(PrimitiveGroup* primGroups);
        [DllImport("NvTriStrip.dll")] public extern static bool GenerateStrips(ushort* in_indices, uint in_numIndices, PrimitiveGroup** primGroups, ushort* numGroups, bool validateEnabled);
        [DllImport("NvTriStrip.dll")] public extern static void RemapIndices(PrimitiveGroup* in_primGroups, ushort numGroups, ushort numVerts, PrimitiveGroup** remappedGroups);
    }
}
