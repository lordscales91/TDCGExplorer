using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.Direct3D9;

namespace TDCG
{
    /// 計算を補助する関数を定義します。
    public class Helper
    {
        /// <summary>
        /// 回転行列と位置ベクトルに分割します。
        /// </summary>
        /// <param name="m">元の行列（戻り値は回転行列）</param>
        /// <returns>位置ベクトル</returns>
        public static Vector3 DecomposeMatrix(ref Matrix m)
        {
            Vector3 t = new Vector3(m.M41, m.M42, m.M43);
            m.M41 = 0;
            m.M42 = 0;
            m.M43 = 0;
            return t;
        }

        /// <summary>
        /// 拡大縮小ベクトルと回転行列と位置ベクトルに分割します。
        /// </summary>
        /// <param name="m">元の行列（戻り値は回転行列）</param>
        /// <param name="scaling">拡大縮小ベクトル</param>
        /// <returns>位置ベクトル</returns>
        public static Vector3 DecomposeMatrix(ref Matrix m, out Vector3 scaling)
        {
            Vector3 vx = new Vector3(m.M11, m.M12, m.M13);
            Vector3 vy = new Vector3(m.M21, m.M22, m.M23);
            Vector3 vz = new Vector3(m.M31, m.M32, m.M33);
            Vector3 vt = new Vector3(m.M41, m.M42, m.M43);
            float scax = vx.Length();
            float scay = vy.Length();
            float scaz = vz.Length();
            scaling = new Vector3(scax, scay, scaz);
            vx.Normalize();
            vy.Normalize();
            vz.Normalize();
            m.M11 = vx.X;
            m.M12 = vx.Y;
            m.M13 = vx.Z;
            m.M21 = vy.X;
            m.M22 = vy.Y;
            m.M23 = vy.Z;
            m.M31 = vz.X;
            m.M32 = vz.Y;
            m.M33 = vz.Z;
            m.M41 = 0;
            m.M42 = 0;
            m.M43 = 0;
            return vt;
        }

        /// <summary>
        /// 拡大縮小ベクトルと回転quaternionと位置ベクトルに分割します。
        /// </summary>
        /// <param name="m">元の行列（戻り値は回転行列）</param>
        /// <param name="scaling">拡大縮小ベクトル</param>
        /// <param name="rotation">回転quaternion</param>
        /// <returns>位置ベクトル</returns>
        public static Vector3 DecomposeMatrix(ref Matrix m, out Vector3 scaling, out Quaternion rotation)
        {
            Vector3 translation = DecomposeMatrix(ref m, out scaling);
            rotation = Quaternion.RotationMatrix(m);
            return translation;
        }

        /// euler角 (zxy回転) をquaternionに変換
        public static Quaternion ToQuaternionZXY(Vector3 angle)
        {
            Quaternion qx, qy, qz;
            qx = Quaternion.RotationAxis(new Vector3(1.0f, 0.0f, 0.0f), MathUtil.DegreesToRadians(angle.X));
            qy = Quaternion.RotationAxis(new Vector3(0.0f, 1.0f, 0.0f), MathUtil.DegreesToRadians(angle.Y));
            qz = Quaternion.RotationAxis(new Vector3(0.0f, 0.0f, 1.0f), MathUtil.DegreesToRadians(angle.Z));
            return qy * qx * qz;
        }

        /// 回転行列をeuler角 (zxy回転) に変換
        public static Vector3 ToAngleZXY(Matrix m)
        {
            Vector3 angle;
            if (m.M23 < +1.0f - float.Epsilon)
            {
                if (m.M23 > -1.0f + float.Epsilon)
                {
                    angle.Z = MathUtil.RadiansToDegrees((float)Math.Atan2(-m.M21, m.M22));
                    angle.X = MathUtil.RadiansToDegrees((float)Math.Asin(m.M23));
                    angle.Y = MathUtil.RadiansToDegrees((float)Math.Atan2(-m.M13, m.M33));
                }
                else
                {
                    angle.Z = MathUtil.RadiansToDegrees((float)Math.Atan2(m.M12, m.M11));
                    angle.X = -90.0f;
                    angle.Y = 0.0f;
                }
            }
            else
            {
                angle.Z = MathUtil.RadiansToDegrees((float)Math.Atan2(m.M12, m.M11));
                angle.X = +90.0f;
                angle.Y = 0.0f;
            }
            return angle;
        }

        /// quaternionをeuler角 (zxy回転) に変換
        public static Vector3 ToAngleZXY(Quaternion q)
        {
            return ToAngleZXY(Matrix.RotationQuaternion(q));
        }

        /// euler角 (xyz回転) をquaternionに変換
        public static Quaternion ToQuaternionXYZ(Vector3 angle)
        {
            Quaternion qx, qy, qz;
            qx = Quaternion.RotationAxis(new Vector3(1.0f, 0.0f, 0.0f), MathUtil.DegreesToRadians(angle.X));
            qy = Quaternion.RotationAxis(new Vector3(0.0f, 1.0f, 0.0f), MathUtil.DegreesToRadians(angle.Y));
            qz = Quaternion.RotationAxis(new Vector3(0.0f, 0.0f, 1.0f), MathUtil.DegreesToRadians(angle.Z));
            return qz * qy * qx;
        }

        /// 回転行列をeuler角 (xyz回転) に変換
        public static Vector3 ToAngleXYZ(Matrix m)
        {
            Vector3 angle;
            if (m.M31 < +1.0f - float.Epsilon)
            {
                if (m.M31 > -1.0f + float.Epsilon)
                {
                    angle.X = MathUtil.RadiansToDegrees((float)Math.Atan2(-m.M32, m.M33));
                    angle.Y = MathUtil.RadiansToDegrees((float)Math.Asin(m.M31));
                    angle.Z = MathUtil.RadiansToDegrees((float)Math.Atan2(-m.M21, m.M11));
                }
                else
                {
                    angle.X = MathUtil.RadiansToDegrees((float)Math.Atan2(m.M21, m.M22));
                    angle.Y = -90.0f;
                    angle.Z = 0.0f;
                }
            }
            else
            {
                angle.X = MathUtil.RadiansToDegrees((float)Math.Atan2(m.M21, m.M22));
                angle.Y = +90.0f;
                angle.Z = 0.0f;
            }
            return angle;
        }

        /// quaternionをeuler角 (xyz回転) に変換
        public static Vector3 ToAngleXYZ(Quaternion q)
        {
            return ToAngleXYZ(Matrix.RotationQuaternion(q));
        }

        /// 左右反転します。
        public static void FlipMatrix(ref Matrix m)
        {
            //y回転
            m.M31 = -m.M31;
            m.M13 = -m.M13;

            //z回転
            m.M21 = -m.M21;
            m.M12 = -m.M12;

            //x移動
            m.M41 = -m.M41;
        }

        /// 線形補間を行います。
        public static float Lerp(float value1, float value2, float amount)
        {
            return value1 + (value2 - value1) * amount;
        }
    }
}
