using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

// デフォルトポーズのtmoを取得する.
namespace TDCGExplorer
{
    public static class DefaultTMOPng
    {
        private static PNGPoseData posedata;
        private static MemoryStream tmoms;

        public static void InitialDefaultTMOPng()
        {
            tmoms = new MemoryStream();

            using (FileStream fs = File.OpenRead("SnapShotPose.tdcgpose.png"))
            {
                using (PNGPOSEStream posestream = new PNGPOSEStream())
                {
                    posedata = posestream.LoadStream(fs);
                    // figureが何個だろうが必ず１個目.
                    using (MemoryStream filetmo = new MemoryStream(posedata.figures[0].tmo.data))
                    {
                        ZipFileUtil.CopyStream(filetmo, tmoms);
                    }
                }
            }
        }

        public static MemoryStream tmo
        {
            get
            {
                tmoms.Seek(0, SeekOrigin.Begin);
                return tmoms;
            }
        }
    }
}
