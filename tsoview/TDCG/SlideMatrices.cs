using System;
using System.Collections.Generic;
using System.Diagnostics;
//using System.Drawing;
using System.Threading;
//using System.ComponentModel;
using System.Windows.Forms;
using System.IO;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace TDCG
{
public class SlideMatrices
{
    public static Matrix MinEyeR;
    public static Matrix GetMinEyeR()
    {
Matrix m = Matrix.Identity;
m.M11 = 1.04776F; m.M12 = -0.165705F; m.M13 = -0.042457F; m.M14 = 0;
m.M21 = 0.169162F; m.M22 = 1.012264F; m.M23 = -0.011601F; m.M24 = 0;
m.M31 = 0.036979F; m.M32 = 0.024401F; m.M33 = 1.100661F; m.M34 = 0;
m.M41 = 0.004252F; m.M42 = 0.124786F; m.M43 = 0.025256F; m.M44 = 1;
return m;
    }

    public static Matrix MaxEyeR;
    public static Matrix GetMaxEyeR()
    {
Matrix m = Matrix.Identity;
m.M11 = 1.039604F; m.M12 = 0.255108F; m.M13 = -0.15971F; m.M14 = 0;
m.M21 = -0.27475F; m.M22 = 1.011007F; m.M23 = -0.040911F; m.M24 = 0;
m.M31 = 0.139395F; m.M32 = 0.085025F; m.M33 = 1.090691F; m.M34 = 0;
m.M41 = 0.180933F; m.M42 = -0.058389F; m.M43 = 0.094482F; m.M44 = 1;
return m;
    }

    public static Matrix MinEyeL;
    public static Matrix GetMinEyeL()
    {
Matrix m = Matrix.Identity;
m.M11 = 1.04776F; m.M12 = 0.165707F; m.M13 = 0.042456F; m.M14 = 0;
m.M21 = -0.169164F; m.M22 = 1.012264F; m.M23 = -0.01111F; m.M24 = 0;
m.M31 = -0.036979F; m.M32 = 0.0244F; m.M33 = 1.100662F; m.M34 = 0;
m.M41 = -0.004275F; m.M42 = 0.124808F; m.M43 = 0.025122F; m.M44 = 1;
return m;
    }

    public static Matrix MaxEyeL;
    public static Matrix GetMaxEyeL()
    {
Matrix m = Matrix.Identity;
m.M11 = 1.039607F; m.M12 = -0.255101F; m.M13 = 0.159708F; m.M14 = 0;
m.M21 = 0.274743F; m.M22 = 1.01101F; m.M23 = -0.040846F; m.M24 = 0;
m.M31 = -0.139394F; m.M32 = 0.085025F; m.M33 = 1.090691F; m.M34 = 0;
m.M41 = -0.181016F; m.M42 = -0.058312F; m.M43 = 0.094464F; m.M44 = 1;
return m;
    }

    public static Matrix GetMatrixRatio(ref Matrix min, ref Matrix max, float ratio)
    {
        Matrix m = Matrix.Identity;

        m.M11 = max.M11 * ratio + min.M11 * (1 - ratio);
        m.M12 = max.M12 * ratio + min.M12 * (1 - ratio);
        m.M13 = max.M13 * ratio + min.M13 * (1 - ratio);
        m.M14 = max.M14 * ratio + min.M14 * (1 - ratio);

        m.M21 = max.M21 * ratio + min.M21 * (1 - ratio);
        m.M22 = max.M22 * ratio + min.M22 * (1 - ratio);
        m.M23 = max.M23 * ratio + min.M23 * (1 - ratio);
        m.M24 = max.M24 * ratio + min.M24 * (1 - ratio);

        m.M31 = max.M31 * ratio + min.M31 * (1 - ratio);
        m.M32 = max.M32 * ratio + min.M32 * (1 - ratio);
        m.M33 = max.M33 * ratio + min.M33 * (1 - ratio);
        m.M34 = max.M34 * ratio + min.M34 * (1 - ratio);

        m.M41 = max.M41 * ratio + min.M41 * (1 - ratio);
        m.M42 = max.M42 * ratio + min.M42 * (1 - ratio);
        m.M43 = max.M43 * ratio + min.M43 * (1 - ratio);
        m.M44 = max.M44 * ratio + min.M44 * (1 - ratio);

        return m;
    }

    public static Matrix FaceOya;

    static SlideMatrices()
    {
        FaceOya = Matrix.Scaling(1.1045F, 1.064401F, 1.1045F);
        MinEyeR = GetMinEyeR();
        MaxEyeR = GetMaxEyeR();
        MinEyeL = GetMinEyeL();
        MaxEyeL = GetMaxEyeL();
    }

    public Matrix EyeR;
    public Matrix EyeL;

    public SlideMatrices()
    {
        EyeRatio = 0.5f;
    }

    float age_ratio;
    public float AgeRatio
    {
        get { return age_ratio; }
        set {
            age_ratio = value;
        }
    }

    float arm_ratio;
    public float ArmRatio
    {
        get { return arm_ratio; }
        set {
            arm_ratio = value;
        }
    }

    float leg_ratio;
    public float LegRatio
    {
        get { return leg_ratio; }
        set
        {
            leg_ratio = value;
        }
    }

    float waist_ratio;
    public float WaistRatio
    {
        get { return waist_ratio; }
        set
        {
            waist_ratio = value;
        }
    }

    float bust_ratio;
    public float BustRatio
    {
        get { return bust_ratio; }
        set
        {
            bust_ratio = value;
        }
    }

    float eye_ratio;
    public float EyeRatio
    {
        get { return eye_ratio; }
        set {
            eye_ratio = value;
            EyeR = GetMatrixRatio(ref MinEyeR, ref MaxEyeR, eye_ratio) * Matrix.Invert(FaceOya);
            EyeL = GetMatrixRatio(ref MinEyeL, ref MaxEyeL, eye_ratio) * Matrix.Invert(FaceOya);
        }
    }
}
}
