using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace TDCG
{
    public struct XnVector3D
    {
        public float X;
        public float Y;
        public float Z;
    }

    public struct XnSkeletonJointPosition
    {
        public XnVector3D position;
        public float confidence;
    }

    public class NiSimpleTracker
    {
        [DllImport("NiSimpleTracker.dll", EntryPoint = "OpenNIClean")]
        public static extern int Clean();
        [DllImport("NiSimpleTracker.dll", EntryPoint = "OpenNIGetDepthBuf")]
        public static extern IntPtr GetDepthBuf();
        [DllImport("NiSimpleTracker.dll", EntryPoint = "OpenNIGetJointPos")]
        public static extern IntPtr GetJointPos();
        [DllImport("NiSimpleTracker.dll", EntryPoint = "OpenNIIsTracking")]
        public static extern bool IsTracking();
        [DllImport("NiSimpleTracker.dll", EntryPoint = "OpenNIInit")]
        public static extern int Init(StringBuilder path);
        [DllImport("NiSimpleTracker.dll", EntryPoint = "OpenNIDrawDepthMap")]
        public static extern void DrawDepthMap();
    }
}
