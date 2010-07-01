using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace TDCG
{
    /// <summary>
    /// カメラ
    /// </summary>
    public class SimpleCamera
    {
        private Vector3 center = Vector3.Empty;
        private Vector3 translation = new Vector3(0.0f, 0.0f, +10.0f);
        private Vector3 dirD = Vector3.Empty; //カメラ移動方向ベクトル
        private float zD = 0.0f;      //カメラ奥行オフセット値
        private bool needUpdate = true;
        private Matrix view = Matrix.Identity;
        private Vector3 angle = Vector3.Empty;
        private float rotZD = 0.0f;   //カメラ Z軸回転差分
        private float angleU = 0.01f;        //移動時回転単位（ラジアン）

        /// <summary>
        /// 回転中心
        /// </summary>
        public Vector3 Center { get { return center; } set { center = value; } }

        /// <summary>
        /// view座標上のカメラの位置
        /// </summary>
        public Vector3 Translation { get { return translation; } set { translation = value; } }
    
        /// <summary>
        ///更新する必要があるか
        /// </summary>
        public bool NeedUpdate { get { return needUpdate; }}

        /// <summary>
        /// ビュー行列
        /// </summary>
        public Matrix ViewMatrix { get { return view; } }

        /// <summary>
        /// カメラの位置と姿勢をリセットします。
        /// </summary>
        public void Reset()
        {
            center = Vector3.Empty;
            translation = new Vector3(0.0f, 0.0f, +10.0f);
            angle = Vector3.Empty;
            needUpdate = true;
        }

        /// <summary>
        /// view座標上のカメラの位置をリセットします。
        /// </summary>
        public void ResetTranslation()
        {
            translation = new Vector3(0.0f, 0.0f, +10.0f);
            needUpdate = true;
        }

        /// <summary>
        /// カメラの位置を更新します。
        /// </summary>
        /// <param name="dirX">移動方向（経度）</param>
        /// <param name="dirY">移動方向（緯度）</param>
        /// <param name="dirZ">移動方向（奥行）</param>
        public void Move(float dirX, float dirY, float dirZ)
        {
            if (dirX == 0.0f && dirY == 0.0f && dirZ == 0.0f)
                return;

            dirD.X += dirX;
            dirD.Y += dirY;
            this.zD += dirZ;
            needUpdate = true;
        }

        /// <summary>
        /// カメラをZ軸回転します。
        /// </summary>
        /// <param name="angle">回転角度（ラジアン）</param>
        public void RotZ(float angle)
        {
            if (angle == 0.0f)
                return;

            rotZD = angle;
            needUpdate = true;
        }

        /// <summary>
        /// カメラの位置と姿勢を更新します。
        /// </summary>
        public void Update()
        {
            if (!needUpdate)
                return;

            angle.Y += angleU * -dirD.X;
            angle.X += angleU * +dirD.Y;
            angle.Z += +rotZD;
            this.translation.Z += zD;

            Matrix m = Matrix.RotationYawPitchRoll(angle.Y, angle.X, angle.Z);
            m.M41 = center.X;
            m.M42 = center.Y;
            m.M43 = center.Z;
            m.M44 = 1;

            view = Matrix.Invert(m) * Matrix.Translation(-translation);

            //差分をリセット
            ResetDefValue();
            needUpdate = false;
        }

        /// <summary>
        /// view行列を取得します。
        /// </summary>
        public Matrix GetViewMatrix()
        {
            return view;
        }

        /// <summary>
        /// 回転中心を設定します。
        /// </summary>
        /// <param name="center">回転中心</param>
        public void SetCenter(Vector3 center)
        {
            this.center = center;
            needUpdate = true;
        }
        /// <summary>
        /// 回転中心を設定します。
        /// </summary>
        /// <param name="x">回転中心x座標</param>
        /// <param name="y">回転中心y座標</param>
        /// <param name="z">回転中心z座標</param>
        public void SetCenter(float x, float y, float z)
        {
            SetCenter(new Vector3(x, y, z));
        }

        /// <summary>
        /// view座標上の位置を設定します。
        /// </summary>
        /// <param name="translation">view座標上の位置</param>
        public void SetTranslation(Vector3 translation)
        {
            this.translation = translation;
            needUpdate = true;
        }
        /// <summary>
        /// view座標上の位置を設定します。
        /// </summary>
        /// <param name="x">view座標上の位置x座標</param>
        /// <param name="y">view座標上の位置y座標</param>
        /// <param name="z">view座標上の位置z座標</param>
        public void SetTranslation(float x, float y, float z)
        {
            SetTranslation(new Vector3(x, y, z));
        }

        /// <summary>
        /// view座標上で移動します。
        /// </summary>
        /// <param name="dx">X軸移動距離</param>
        /// <param name="dy">Y軸移動距離</param>
        public void MoveView(float dx, float dy)
        {
            this.translation.X += dx;
            this.translation.Y += dy;
            needUpdate = true;
        }

        /// <summary>
        /// 差分をリセットします。
        /// </summary>
        protected void ResetDefValue()
        {
            dirD = Vector3.Empty;
            zD = 0.0f;
            rotZD = 0.0f;
        }
    }
}
