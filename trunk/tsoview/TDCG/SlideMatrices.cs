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
    /// スライダ変形行列
public class SlideMatrices
{
    /// 姉妹スライダ0.0でのlocal scaling factor
    public static Vector3 GetMinLocal()
    {
        float scale = 0.9520f;
        return new Vector3(scale, scale, scale);
    }

    /// 姉妹スライダ1.0でのlocal scaling factor
    public static Vector3 GetMaxLocal()
    {
        float scale = 1.05f;
        return new Vector3(scale, scale, scale);
    }

    /// 姉妹スライダ0.0でのface_oya scaling factor
    public static Vector3 GetMinFaceOya()
    {
        return new Vector3(1.224272f, 1.066630f, 1.224272f);
    }

    /// 姉妹スライダ1.0でのface_oya scaling factor
    public static Vector3 GetMaxFaceOya()
    {
        return new Vector3(0.967304f, 1.025342f, 0.967304f);
    }

    /// 胴まわりスライダ0.0でのW_Spine_Dummy scaling factor
    public static Vector3 GetMinSpineDummy()
    {
        return new Vector3(1.0f, 1.0f, 1.0f);
    }

    /// 胴まわりスライダ1.0でのW_Spine_Dummy scaling factor
    public static Vector3 GetMaxSpineDummy()
    {
        return new Vector3(1.0890f, 1.0f, 0.9235f);
    }

    /// 胴まわりスライダ0.0でのW_Spine1 scaling factor
    public static Vector3 GetMinSpine1()
    {
        return new Vector3(1.0f, 1.0f, 1.0f);
    }

    /// 胴まわりスライダ1.0でのW_Spine1 scaling factor
    public static Vector3 GetMaxSpine1()
    {
        return new Vector3(1.1800f, 1.0f, 1.0f);
    }

    /// あしスライダ0.0でのW_(LR)Hips_Dummy scaling factor
    public static Vector3 GetMinHipsDummy()
    {
        return new Vector3(1.0f, 1.0f, 1.0f);
    }

    /// あしスライダ1.0でのW_(LR)Hips_Dummy scaling factor
    public static Vector3 GetMaxHipsDummy()
    {
        return new Vector3(1.2001f, 1.0f, 1.0f);
    }

    /// あしスライダ0.0でのW_(LR)UpLeg scaling factor
    public static Vector3 GetMinUpLeg()
    {
        return new Vector3(0.8091f, 1.0f, 0.8190f);
    }

    /// あしスライダ1.0でのW_(LR)UpLeg scaling factor
    public static Vector3 GetMaxUpLeg()
    {
        return new Vector3(1.2001f, 1.0f, 1.0f);
    }

    /// あしスライダ0.0でのW_(LR)UpLegRoll scaling factor
    public static Vector3 GetMinUpLegRoll()
    {
        return new Vector3(0.8091f, 1.0f, 0.8190f);
    }

    /// あしスライダ1.0でのW_(LR)UpLegRoll scaling factor
    public static Vector3 GetMaxUpLegRoll()
    {
        return new Vector3(1.2012f, 1.0f, 1.0f);
    }

    /// あしスライダ0.0でのW_(LR)LegRoll scaling factor
    public static Vector3 GetMinLegRoll()
    {
        return new Vector3(0.8091f, 1.0f, 0.8190f);
    }

    /// あしスライダ1.0でのW_(LR)LegRoll scaling factor
    public static Vector3 GetMaxLegRoll()
    {
        return new Vector3(0.9878f, 1.0f, 1.0f);
    }

    /// うでスライダ0.0でのW_(LR)Arm_Dummy scaling factor
    public static Vector3 GetMinArmDummy()
    {
        return new Vector3(1.0f, 1.0f, 1.0f);
    }

    /// うでスライダ1.0でのW_(LR)Arm_Dummy scaling factor
    public static Vector3 GetMaxArmDummy()
    {
        return new Vector3(1.0f, 1.1760f, 1.0f);
    }

    /// うでスライダ0.0でのW_(LR)Arm scaling factor
    public static Vector3 GetMinArm()
    {
        return new Vector3(1.0f, 0.7350f, 1.0f);
    }

    /// うでスライダ1.0でのW_(LR)Arm scaling factor
    public static Vector3 GetMaxArm()
    {
        return new Vector3(1.0f, 1.1760f, 1.0f);
    }

    /// 変形行列 ChichiR1 0.0
    public static Matrix GetMinChichiR1()
    {
        Matrix m = Matrix.Identity;
m.M11 = 0.838979F;
m.M12 = 0.000000F;
m.M13 = 0.092851F;
m.M14 = 0.000000F;
m.M21 = 0.014229F;
m.M22 = 0.991598F;
m.M23 = -0.128573F;
m.M24 = 0.000000F;
m.M31 = -0.050885F;
m.M32 = 0.060347F;
m.M33 = 0.459783F;
m.M34 = 0.000000F;
m.M41 = -0.112165F;
m.M42 = 0.003479F;
m.M43 = 0.256438F;
m.M44 = 1.000000F;
        return m;
    }

    /// 変形行列 ChichiR2 0.0
    public static Matrix GetMinChichiR2()
    {
        Matrix m = Matrix.Identity;
m.M11 = 0.931420F;
m.M12 = -0.000001F;
m.M13 = -0.164172F;
m.M14 = 0.000000F;
m.M21 = 0.018493F;
m.M22 = 0.921283F;
m.M23 = 0.343484F;
m.M24 = 0.000000F;
m.M31 = 0.060012F;
m.M32 = -0.091304F;
m.M33 = 1.114690F;
m.M34 = 0.000000F;
m.M41 = 0.048976F;
m.M42 = 0.163741F;
m.M43 = 0.235518F;
m.M44 = 1.000000F;
        return m;
    }

    /// 変形行列 ChichiR3 0.0
    public static Matrix GetMinChichiR3()
    {
        Matrix m = Matrix.Identity;
m.M11 = 0.871897F;
m.M12 = 0.000001F;
m.M13 = -0.000002F;
m.M14 = 0.000000F;
m.M21 = 0.000000F;
m.M22 = 0.736430F;
m.M23 = -0.000001F;
m.M24 = 0.000000F;
m.M31 = 0.000000F;
m.M32 = 0.000001F;
m.M33 = 1.298548F;
m.M34 = 0.000000F;
m.M41 = -0.028062F;
m.M42 = -0.134603F;
m.M43 = -0.145561F;
m.M44 = 1.000000F;
        return m;
    }

    /// 変形行列 ChichiR4 0.0
    public static Matrix GetMinChichiR4()
    {
        Matrix m = Matrix.Identity;
m.M11 = 1.451919F;
m.M12 = 0.000000F;
m.M13 = 0.000000F;
m.M14 = 0.000000F;
m.M21 = 0.000000F;
m.M22 = 1.451918F;
m.M23 = 0.000000F;
m.M24 = 0.000000F;
m.M31 = -0.000001F;
m.M32 = 0.000000F;
m.M33 = 1.451920F;
m.M34 = 0.000000F;
m.M41 = 0.043505F;
m.M42 = 0.007941F;
m.M43 = -0.174366F;
m.M44 = 1.000000F;
        return m;
    }

    /// 変形行列 ChichiR5 0.0
    public static Matrix GetMinChichiR5()
    {
        Matrix m = Matrix.Identity;
m.M11 = 1.000000F;
m.M12 = 0.000000F;
m.M13 = 0.000001F;
m.M14 = 0.000000F;
m.M21 = 0.000000F;
m.M22 = 1.000001F;
m.M23 = 0.000001F;
m.M24 = 0.000000F;
m.M31 = 0.000000F;
m.M32 = 0.000000F;
m.M33 = 1.000001F;
m.M34 = 0.000000F;
m.M41 = -0.000099F;
m.M42 = -0.000705F;
m.M43 = -0.000509F;
m.M44 = 1.000000F;
        return m;
    }

    /// 変形行列 ChichiR5_end 0.0
    public static Matrix GetMinChichiR5E()
    {
        Matrix m = Matrix.Identity;
m.M11 = 1.000000F;
m.M12 = 0.000000F;
m.M13 = -0.000001F;
m.M14 = 0.000000F;
m.M21 = 0.000001F;
m.M22 = 1.000000F;
m.M23 = -0.000001F;
m.M24 = 0.000000F;
m.M31 = 0.000001F;
m.M32 = 0.000000F;
m.M33 = 0.999999F;
m.M34 = 0.000000F;
m.M41 = -0.000274F;
m.M42 = 0.000190F;
m.M43 = 0.000653F;
m.M44 = 1.000000F;
        return m;
    }

    /// 変形行列 ChichiL1 0.0
    public static Matrix GetMinChichiL1()
    {
        Matrix m = Matrix.Identity;

m.M11 = 0.838981F;
m.M12 = 0.000000F;
m.M13 = -0.092846F;
m.M14 = 0.000000F;
m.M21 = -0.014228F;
m.M22 = 0.991598F;
m.M23 = -0.128574F;
m.M24 = 0.000000F;
m.M31 = 0.050883F;
m.M32 = 0.060347F;
m.M33 = 0.459783F;
m.M34 = 0.000000F;
m.M41 = 0.112161F;
m.M42 = 0.003479F;
m.M43 = 0.256440F;
m.M44 = 1.000000F;
        return m;
    }

    /// 変形行列 ChichiL2 0.0
    public static Matrix GetMinChichiL2()
    {
        Matrix m = Matrix.Identity;
m.M11 = 0.931217F;
m.M12 = 0.000001F;
m.M13 = 0.164140F;
m.M14 = 0.000000F;
m.M21 = -0.018493F;
m.M22 = 0.921284F;
m.M23 = 0.343484F;
m.M24 = 0.000000F;
m.M31 = -0.060014F;
m.M32 = -0.091303F;
m.M33 = 1.114690F;
m.M34 = 0.000000F;
m.M41 = -0.048692F;
m.M42 = 0.163041F;
m.M43 = 0.235785F;
m.M44 = 1.000000F;
        return m;
    }

    /// 変形行列 ChichiL3 0.0
    public static Matrix GetMinChichiL3()
    {
        Matrix m = Matrix.Identity;
m.M11 = 0.872085F;
m.M12 = 0.000000F;
m.M13 = -0.000002F;
m.M14 = 0.000000F;
m.M21 = 0.000000F;
m.M22 = 0.736429F;
m.M23 = -0.000002F;
m.M24 = 0.000000F;
m.M31 = 0.000000F;
m.M32 = 0.000000F;
m.M33 = 1.298548F;
m.M34 = 0.000000F;
m.M41 = 0.027806F;
m.M42 = -0.134507F;
m.M43 = -0.145748F;
m.M44 = 1.000000F;
        return m;
    }

    /// 変形行列 ChichiL4 0.0
    public static Matrix GetMinChichiL4()
    {
        Matrix m = Matrix.Identity;
m.M11 = 1.451920F;
m.M12 = -0.000002F;
m.M13 = 0.000002F;
m.M14 = 0.000000F;
m.M21 = -0.000002F;
m.M22 = 1.451919F;
m.M23 = 0.000001F;
m.M24 = 0.000000F;
m.M31 = -0.000002F;
m.M32 = 0.000000F;
m.M33 = 1.451920F;
m.M34 = 0.000000F;
m.M41 = -0.042320F;
m.M42 = 0.007925F;
m.M43 = -0.174345F;
m.M44 = 1.000000F;
        return m;
    }

    /// 変形行列 ChichiL5 0.0
    public static Matrix GetMinChichiL5()
    {
        Matrix m = Matrix.Identity;
m.M11 = 1.000001F;
m.M12 = 0.000000F;
m.M13 = 0.000000F;
m.M14 = 0.000000F;
m.M21 = 0.000001F;
m.M22 = 1.000000F;
m.M23 = 0.000001F;
m.M24 = 0.000000F;
m.M31 = 0.000001F;
m.M32 = 0.000000F;
m.M33 = 1.000001F;
m.M34 = 0.000000F;
m.M41 = -0.000287F;
m.M42 = 0.000293F;
m.M43 = -0.000554F;
m.M44 = 1.000000F;
        return m;
    }

    /// 変形行列 ChichiL5_End 0.0
    public static Matrix GetMinChichiL5E()
    {
        Matrix m = Matrix.Identity;
m.M11 = 0.999999F;
m.M12 = 0.000000F;
m.M13 = 0.000001F;
m.M14 = 0.000000F;
m.M21 = -0.000001F;
m.M22 = 0.999999F;
m.M23 = 0.000000F;
m.M24 = 0.000000F;
m.M31 = -0.000001F;
m.M32 = 0.000000F;
m.M33 = 1.000000F;
m.M34 = 0.000000F;
m.M41 = 0.000289F;
m.M42 = 0.000146F;
m.M43 = -0.000345F;
m.M44 = 1.000000F;
        return m;
    }

    /// おっぱいスライダ0.225でのscaling factor
    public static Vector3 GetMinChichi()
    {
        return new Vector3(0.8350f, 0.8240f, 0.7800f);
    }

    /// おっぱいスライダ1.0でのscaling factor
    public static Vector3 GetMaxChichi()
    {
        return new Vector3(1.2500f, 1.3000f, 1.1800f);
    }

    /// たれ目つり目スライダ0.0での変形
    public static Matrix GetMinEyeR()
    {
        Matrix m = Matrix.Identity;
        m.M11 = 1.04776F; m.M12 = -0.165705F; m.M13 = -0.042457F; m.M14 = 0;
        m.M21 = 0.169162F; m.M22 = 1.012264F; m.M23 = -0.011601F; m.M24 = 0;
        m.M31 = 0.036979F; m.M32 = 0.024401F; m.M33 = 1.100661F; m.M34 = 0;
        m.M41 = 0.004252F; m.M42 = 0.124786F; m.M43 = 0.025256F; m.M44 = 1;
        return m;
    }

    /// たれ目つり目スライダ1.0での変形
    public static Matrix GetMaxEyeR()
    {
        Matrix m = Matrix.Identity;
        m.M11 = 1.039604F; m.M12 = 0.255108F; m.M13 = -0.15971F; m.M14 = 0;
        m.M21 = -0.27475F; m.M22 = 1.011007F; m.M23 = -0.040911F; m.M24 = 0;
        m.M31 = 0.139395F; m.M32 = 0.085025F; m.M33 = 1.090691F; m.M34 = 0;
        m.M41 = 0.180933F; m.M42 = -0.058389F; m.M43 = 0.094482F; m.M44 = 1;
        return m;
    }

    /// たれ目つり目スライダ0.0での変形
    public static Matrix GetMinEyeL()
    {
        Matrix m = Matrix.Identity;
        m.M11 = 1.04776F; m.M12 = 0.165707F; m.M13 = 0.042456F; m.M14 = 0;
        m.M21 = -0.169164F; m.M22 = 1.012264F; m.M23 = -0.01111F; m.M24 = 0;
        m.M31 = -0.036979F; m.M32 = 0.0244F; m.M33 = 1.100662F; m.M34 = 0;
        m.M41 = -0.004275F; m.M42 = 0.124808F; m.M43 = 0.025122F; m.M44 = 1;
        return m;
    }

    /// たれ目つり目スライダ1.0での変形
    public static Matrix GetMaxEyeL()
    {
        Matrix m = Matrix.Identity;
        m.M11 = 1.039607F; m.M12 = -0.255101F; m.M13 = 0.159708F; m.M14 = 0;
        m.M21 = 0.274743F; m.M22 = 1.01101F; m.M23 = -0.040846F; m.M24 = 0;
        m.M31 = -0.139394F; m.M32 = 0.085025F; m.M33 = 1.090691F; m.M34 = 0;
        m.M41 = -0.181016F; m.M42 = -0.058312F; m.M43 = 0.094464F; m.M44 = 1;
        return m;
    }

    /// 指定比率に比例するscaling factorを得ます。
    public static Vector3 GetVector3Ratio(Vector3 min, Vector3 max, float ratio)
    {
        Vector3 v = Vector3.Empty;

        v.X = max.X * ratio + min.X * (1 - ratio);
        v.Y = max.Y * ratio + min.Y * (1 - ratio);
        v.Z = max.Z * ratio + min.Z * (1 - ratio);

        return v;
    }

    /// 指定比率に比例する変形行列を得ます。
    public static Matrix GetMatrixRatio(Vector3 min, Vector3 max, float ratio)
    {
        return Matrix.Scaling(GetVector3Ratio(min, max, ratio));
    }

    /// 指定比率に比例する変形行列を得ます。
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

    /// face_oyaの変形行列
    public static Matrix FaceOyaDefault;

    static SlideMatrices()
    {
        FaceOyaDefault = Matrix.Scaling(1.1045F, 1.064401F, 1.1045F);
    }

    /// 拡大変位
    public Vector3 Local;
    /// 拡大変位
    public Vector3 FaceOya;

    /// 拡大変位
    public Vector3 SpineDummy;
    /// 拡大変位
    public Vector3 Spine1;

    /// 拡大変位
    public Vector3 HipsDummy;
    /// 拡大変位
    public Vector3 UpLeg;
    /// 拡大変位
    public Vector3 UpLegRoll;
    /// 拡大変位
    public Vector3 LegRoll;

    /// 拡大変位
    public Vector3 ArmDummy;
    /// 拡大変位
    public Vector3 Arm;

    /// 変形行列
    public Matrix ChichiR1;
    /// 変形行列
    public Matrix ChichiR2;
    /// 変形行列
    public Matrix ChichiR3;
    /// 変形行列
    public Matrix ChichiR4;
    /// 変形行列
    public Matrix ChichiR5;
    /// 変形行列
    public Matrix ChichiR5E;

    /// 変形行列
    public Matrix ChichiL1;
    /// 変形行列
    public Matrix ChichiL2;
    /// 変形行列
    public Matrix ChichiL3;
    /// 変形行列
    public Matrix ChichiL4;
    /// 変形行列
    public Matrix ChichiL5;
    /// 変形行列
    public Matrix ChichiL5E;

    /// 拡大変位
    public Vector3 Chichi;

    /// 変形行列
    public Matrix EyeR;
    /// 変形行列
    public Matrix EyeL;

    /// スライダ変形行列を生成します。
    public SlideMatrices()
    {
        ArmRatio = 0.5f;
        LegRatio = 0.5f;
        WaistRatio = 0.0f; //scaling factorから見て胴まわりの基準は0.0である
        BustRatio = 0.5f;
        TallRatio = 0.5f;
        EyeRatio = 0.5f;
    }

    float arm_ratio;
    /// うでスライダ比率
    public float ArmRatio
    {
        get { return arm_ratio; }
        set {
            arm_ratio = value;
            ArmDummy = GetVector3Ratio(GetMinArmDummy(), GetMaxArmDummy(), arm_ratio);
            Arm = GetVector3Ratio(GetMinArm(), GetMaxArm(), arm_ratio);
        }
    }

    float leg_ratio;
    /// あしスライダ比率
    public float LegRatio
    {
        get { return leg_ratio; }
        set
        {
            leg_ratio = value;
            HipsDummy = GetVector3Ratio(GetMinHipsDummy(), GetMaxHipsDummy(), leg_ratio);
            UpLeg = GetVector3Ratio(GetMinUpLeg(), GetMaxUpLeg(), leg_ratio);
            UpLegRoll = GetVector3Ratio(GetMinUpLegRoll(), GetMaxUpLegRoll(), leg_ratio);
            LegRoll = GetVector3Ratio(GetMinLegRoll(), GetMaxLegRoll(), leg_ratio);
        }
    }

    float waist_ratio;
    /// 胴まわりスライダ比率
    public float WaistRatio
    {
        get { return waist_ratio; }
        set
        {
            waist_ratio = value;
            SpineDummy = GetVector3Ratio(GetMinSpineDummy(), GetMaxSpineDummy(), waist_ratio);
            Spine1 = GetVector3Ratio(GetMinSpine1(), GetMaxSpine1(), waist_ratio);
        }
    }

    float bust_ratio;
    /// おっぱいスライダ比率
    public float BustRatio
    {
        get { return bust_ratio; }
        set
        {
            bust_ratio = value;
            
            if (Flat())
            {
                float ratio = bust_ratio / FlatRatio;

                ChichiR1 = GetMatrixRatio(GetMinChichiR1(), Matrix.Identity, ratio);
                ChichiR2 = GetMatrixRatio(GetMinChichiR2(), Matrix.Identity, ratio);
                ChichiR3 = GetMatrixRatio(GetMinChichiR3(), Matrix.Identity, ratio);
                ChichiR4 = GetMatrixRatio(GetMinChichiR4(), Matrix.Identity, ratio);
                ChichiR5 = GetMatrixRatio(GetMinChichiR5(), Matrix.Identity, ratio);
                ChichiR5E = GetMatrixRatio(GetMinChichiR5E(), Matrix.Identity, ratio);

                ChichiL1 = GetMatrixRatio(GetMinChichiL1(), Matrix.Identity, ratio);
                ChichiL2 = GetMatrixRatio(GetMinChichiL2(), Matrix.Identity, ratio);
                ChichiL3 = GetMatrixRatio(GetMinChichiL3(), Matrix.Identity, ratio);
                ChichiL4 = GetMatrixRatio(GetMinChichiL4(), Matrix.Identity, ratio);
                ChichiL5 = GetMatrixRatio(GetMinChichiL5(), Matrix.Identity, ratio);
                ChichiL5E = GetMatrixRatio(GetMinChichiL5E(), Matrix.Identity, ratio);

                Chichi = GetVector3Ratio(new Vector3(1,1,1), GetMinChichi(), ratio);
            }
            else
                Chichi = GetVector3Ratio(GetMinChichi(), GetMaxChichi(), (bust_ratio - FlatRatio) / (1.0f - FlatRatio));
        }
    }

    /// 貧乳であるか
    public bool Flat()
    {
        return bust_ratio < FlatRatio;
    }

    /// 貧乳境界比率
    public static float FlatRatio = 0.2250F;

    float tall_ratio;
    /// 姉妹スライダ比率
    public float TallRatio
    {
        get { return tall_ratio; }
        set
        {
            tall_ratio = value;
            Local = GetVector3Ratio(GetMinLocal(), GetMaxLocal(), tall_ratio);
            FaceOya = GetVector3Ratio(GetMinFaceOya(), GetMaxFaceOya(), tall_ratio);
        }
    }

    float eye_ratio;
    /// たれ目つり目スライダ比率
    public float EyeRatio
    {
        get { return eye_ratio; }
        set {
            eye_ratio = value;
            EyeR = GetMatrixRatio(GetMinEyeR(), GetMaxEyeR(), eye_ratio) * Matrix.Invert(FaceOyaDefault);
            EyeL = GetMatrixRatio(GetMinEyeL(), GetMaxEyeL(), eye_ratio) * Matrix.Invert(FaceOyaDefault);
        }
    }
}
}
