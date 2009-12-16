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
    public static Vector3 GetMinSpineDummy()
    {
        return new Vector3(1.0f, 1.0f, 1.0f);
    }

    public static Vector3 GetMaxSpineDummy()
    {
        return new Vector3(1.089000f, 1.0f, 0.923500f);
    }

    public static Vector3 GetMinSpine1()
    {
        return new Vector3(1.0f, 1.0f, 1.0f);
    }

    public static Vector3 GetMaxSpine1()
    {
        return new Vector3(1.083563f, 1.0f, 1.082837f);
    }

    public static Vector3 GetMinSpine3()
    {
        return new Vector3(1.0f, 1.0f, 1.0f);
    }

    public static Vector3 GetMaxSpine3()
    {
        return new Vector3(0.847458f, 1.0f, 1.0f);
    }

    public static Vector3 GetMinHipsDummy()
    {
        return new Vector3(1.0f, 1.0f, 1.0f);
    }

    public static Vector3 GetMaxHipsDummy()
    {
        return new Vector3(1.200100f, 1.0f, 1.0f);
    }

    public static Vector3 GetMinUpLeg()
    {
        return new Vector3(0.809100f, 1.0f, 0.819000f);
    }

    public static Vector3 GetMaxUpLeg()
    {
        return new Vector3(1.0f, 1.0f, 1.0f);
    }

    public static Vector3 GetMinUpLegRoll()
    {
        return new Vector3(1.0f, 1.0f, 1.0f);
    }

    public static Vector3 GetMaxUpLegRoll()
    {
        return new Vector3(1.000917f, 1.0f, 1.0f);
    }

    public static Vector3 GetMinLegRoll()
    {
        return new Vector3(1.0f, 1.0f, 1.0f);
    }

    public static Vector3 GetMaxLegRoll()
    {
        return new Vector3(0.822344f, 1.0f, 1.0f);
    }

    public static Vector3 GetMinArmDummy()
    {
        return new Vector3(1.0f, 1.0f, 1.0f);
    }

    public static Vector3 GetMaxArmDummy()
    {
        return new Vector3(1.0f, 1.176000f, 1.0f);
    }

    public static Vector3 GetMinArm()
    {
        return new Vector3(1.0f, 0.735000f, 1.0f);
    }

    public static Vector3 GetMaxArm()
    {
        return new Vector3(1.0f, 1.0f, 1.0f);
    }

    public static Vector3 GetMinHand()
    {
        return new Vector3(1.0f, 1.360544f, 1.0f);
    }

    public static Vector3 GetMaxHand()
    {
        return new Vector3(1.0f, 0.850340f, 1.0f);
    }

    public static Matrix GetMinEyeR()
    {
Matrix m = Matrix.Identity;
m.M11 = 1.04776F; m.M12 = -0.165705F; m.M13 = -0.042457F; m.M14 = 0;
m.M21 = 0.169162F; m.M22 = 1.012264F; m.M23 = -0.011601F; m.M24 = 0;
m.M31 = 0.036979F; m.M32 = 0.024401F; m.M33 = 1.100661F; m.M34 = 0;
m.M41 = 0.004252F; m.M42 = 0.124786F; m.M43 = 0.025256F; m.M44 = 1;
return m;
    }

    public static Matrix GetMaxEyeR()
    {
Matrix m = Matrix.Identity;
m.M11 = 1.039604F; m.M12 = 0.255108F; m.M13 = -0.15971F; m.M14 = 0;
m.M21 = -0.27475F; m.M22 = 1.011007F; m.M23 = -0.040911F; m.M24 = 0;
m.M31 = 0.139395F; m.M32 = 0.085025F; m.M33 = 1.090691F; m.M34 = 0;
m.M41 = 0.180933F; m.M42 = -0.058389F; m.M43 = 0.094482F; m.M44 = 1;
return m;
    }

    public static Matrix GetMinEyeL()
    {
Matrix m = Matrix.Identity;
m.M11 = 1.04776F; m.M12 = 0.165707F; m.M13 = 0.042456F; m.M14 = 0;
m.M21 = -0.169164F; m.M22 = 1.012264F; m.M23 = -0.01111F; m.M24 = 0;
m.M31 = -0.036979F; m.M32 = 0.0244F; m.M33 = 1.100662F; m.M34 = 0;
m.M41 = -0.004275F; m.M42 = 0.124808F; m.M43 = 0.025122F; m.M44 = 1;
return m;
    }

    public static Matrix GetMaxEyeL()
    {
Matrix m = Matrix.Identity;
m.M11 = 1.039607F; m.M12 = -0.255101F; m.M13 = 0.159708F; m.M14 = 0;
m.M21 = 0.274743F; m.M22 = 1.01101F; m.M23 = -0.040846F; m.M24 = 0;
m.M31 = -0.139394F; m.M32 = 0.085025F; m.M33 = 1.090691F; m.M34 = 0;
m.M41 = -0.181016F; m.M42 = -0.058312F; m.M43 = 0.094464F; m.M44 = 1;
return m;
    }

    public static Matrix GetMatrixRatio(Vector3 min, Vector3 max, float ratio)
    {
        Vector3 v = Vector3.Empty;

        v.X = max.X * ratio + min.X * (1 - ratio);
        v.Y = max.Y * ratio + min.Y * (1 - ratio);
        v.Z = max.Z * ratio + min.Z * (1 - ratio);

        return Matrix.Scaling(v);
    }

    public static Matrix GetMatrixRatio(Matrix min, Matrix max, float ratio)
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
    }

    public Matrix SpineDummy;
    public Matrix Spine1;
    public Matrix Spine3;

    public Matrix HipsDummy;
    public Matrix UpLeg;
    public Matrix UpLegRoll;
    public Matrix LegRoll;

    public Matrix ArmDummy;
    public Matrix Arm;
    public Matrix Hand;

    public Matrix EyeR;
    public Matrix EyeL;

    public SlideMatrices()
    {
        LegRatio = 0.5f;
        ArmRatio = 0.5f;
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
            ArmDummy = GetMatrixRatio(GetMinArmDummy(), GetMaxArmDummy(), arm_ratio);
            Arm = GetMatrixRatio(GetMinArm(), GetMaxArm(), arm_ratio);
            Hand = GetMatrixRatio(GetMinHand(), GetMaxHand(), arm_ratio);
        }
    }

    float leg_ratio;
    public float LegRatio
    {
        get { return leg_ratio; }
        set
        {
            leg_ratio = value;
            HipsDummy = GetMatrixRatio(GetMinHipsDummy(), GetMaxHipsDummy(), leg_ratio);
            UpLeg = GetMatrixRatio(GetMinUpLeg(), GetMaxUpLeg(), leg_ratio);
            UpLegRoll = GetMatrixRatio(GetMinUpLegRoll(), GetMaxUpLegRoll(), leg_ratio);
            LegRoll = GetMatrixRatio(GetMinLegRoll(), GetMaxLegRoll(), leg_ratio);
        }
    }

    float waist_ratio;
    public float WaistRatio
    {
        get { return waist_ratio; }
        set
        {
            waist_ratio = value;
            SpineDummy = GetMatrixRatio(GetMinSpineDummy(), GetMaxSpineDummy(), waist_ratio);
            Spine1 = GetMatrixRatio(GetMinSpine1(), GetMaxSpine1(), waist_ratio);
            Spine3 = GetMatrixRatio(GetMinSpine3(), GetMaxSpine3(), waist_ratio);
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
            EyeR = GetMatrixRatio(GetMinEyeR(), GetMaxEyeR(), eye_ratio) * Matrix.Invert(FaceOya);
            EyeL = GetMatrixRatio(GetMinEyeL(), GetMaxEyeL(), eye_ratio) * Matrix.Invert(FaceOya);
        }
    }
}
}
