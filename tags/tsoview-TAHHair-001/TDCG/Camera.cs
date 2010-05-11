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
public class Camera
{
    private Vector3 center = Vector3.Empty;
    private Vector3 translation = Vector3.Empty;
    private Vector3 localP = new Vector3(0.0f, 0.0f, +10.0f);
    private Vector3 dirD = Vector3.Empty; //カメラ移動方向ベクトル
    private float zD = 0.0f;      //カメラ奥行オフセット値
    private bool needUpdate = true;
    private Matrix view = Matrix.Identity;  //ビュー行列
    private Matrix pose = Matrix.Identity;
    private float rotZD = 0.0f;   //カメラ Z軸回転差分
    private float angleU = 0.02f;        //移動時回転単位（ラジアン）

    /// <summary>
    /// 回転中心
    /// </summary>
    public Vector3 Center { get { return center; } set { center = value; } }

    /// <summary>
    /// view座標上のカメラの位置
    /// </summary>
    public Vector3 Translation { get { return translation; } set { translation = value; } }

    /// <summary>
    /// 注視点を原点とした座標上のカメラの位置
    /// </summary>
    public Vector3 LocalPosition { get { return localP; } set { localP = value; } }
    
    /// <summary>
    ///更新する必要があるか
    /// </summary>
    public bool NeedUpdate { get { return needUpdate; }}

    /// <summary>
    /// ビュー行列
    /// </summary>
    public Matrix ViewMatrix { get { return view; } }

    /// <summary>
    /// カメラの姿勢行列
    /// </summary>
    public Matrix PoseMatrix { get { return pose; } set { pose = value; } }

    /// <summary>
    /// カメラを生成します。
    /// </summary>
    public Camera()
    {
    }

    /// <summary>
    /// カメラの位置と姿勢を標準出力へ書き出します。
    /// </summary>
    public void Dump()
    {
        XmlSerializer serializer = new XmlSerializer(typeof(Camera));
        XmlWriterSettings settings = new XmlWriterSettings();
        settings.Encoding = Encoding.GetEncoding("Shift_JIS");
        settings.Indent = true;
        XmlWriter writer = XmlWriter.Create(Console.Out, settings);
        serializer.Serialize(writer, this);
        writer.Close();
    }

    /// <summary>
    /// カメラの位置と姿勢を指定パスへ書き出します。
    /// </summary>
    /// <param name="dest_file">パス</param>
    public void Save(string dest_file)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(Camera));
        XmlWriterSettings settings = new XmlWriterSettings();
        settings.Encoding = Encoding.GetEncoding("Shift_JIS");
        settings.Indent = true;
        XmlWriter writer = XmlWriter.Create(dest_file, settings);
        serializer.Serialize(writer, this);
        writer.Close();
    }

    /// <summary>
    /// カメラの位置と姿勢を指定パスから読み込みます。
    /// </summary>
    /// <param name="source_file">パス</param>
    /// <returns>カメラ</returns>
    public static Camera Load(string source_file)
    {
        XmlReader reader = XmlReader.Create(source_file);
        XmlSerializer serializer = new XmlSerializer(typeof(Camera));
        Camera camera = serializer.Deserialize(reader) as Camera;
        reader.Close();
        return camera;
    }

    /// <summary>
    /// カメラの位置と姿勢をリセットします。
    /// </summary>
    public void Reset()
    {
        center = Vector3.Empty;
        translation = Vector3.Empty;
        localP = new Vector3(0.0f, 0.0f, +10.0f);
        pose = Matrix.Identity;
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
    /// マウスの回転中心は原点にリセットします。
    /// 注意：この操作は Move() RotZ() Update() とは異なる系統です。
    /// </summary>
    /// <param name="eye">注視点</param>
    /// <param name="center">view座標上のカメラの位置</param>
    /// <param name="up">上方ベクトル</param>
    public void LookAt(Vector3 eye, Vector3 center, Vector3 up)
    {
        this.localP = center - eye;
        {
            // カメラ姿勢を更新
            Vector3 z = Vector3.Normalize(-localP);
            Vector3 y = up;
            Vector3 x = Vector3.Normalize(Vector3.Cross(y, z));
            y = Vector3.Normalize(Vector3.Cross(z, x));
            {
                Matrix m = Matrix.Identity;
                m.M11 = x.X;
                m.M12 = x.Y;
                m.M13 = x.Z;
                m.M21 = y.X;
                m.M22 = y.Y;
                m.M23 = y.Z;
                m.M31 = z.X;
                m.M32 = z.Y;
                m.M33 = z.Z;
                this.pose = m;
            }
        }
        this.center = Vector3.Empty;
        this.translation = eye;

        //view行列更新
        Vector3 posW = localP + this.center;
        {
            Matrix m = pose;
            m.M41 = posW.X;
            m.M42 = posW.Y;
            m.M43 = posW.Z;
            m.M44 = 1.0f;
            view = Matrix.Invert(m) * Matrix.Translation(-translation);
        }

        //差分をリセット
        ResetDefValue();
        needUpdate = false;
    }

    /// <summary>
    /// カメラの位置と姿勢を更新します。
    /// マウスの回転中心は原点にリセットします。
    /// 注意：この操作は Move() RotZ() Update() とは異なる系統です。
    /// </summary>
    /// <param name="eye">注視点</param>
    /// <param name="center">view座標上のカメラの位置</param>
    public void LookAt(Vector3 eye, Vector3 center)
    {
        LookAt(eye, center, new Vector3(0.0f, 1.0f, 0.0f));
    }

    /// <summary>
    /// カメラのZ軸方向を得ます。
    /// </summary>
    /// <returns>Z軸方向</returns>
    public Vector3 GetZAxis()
    {
        return new Vector3(pose.M31, pose.M32, pose.M33);
    }

    /// <summary>
    /// カメラのY軸方向を得ます。
    /// </summary>
    /// <returns>Y軸方向</returns>
    public Vector3 GetYAxis()
    {
        return new Vector3(pose.M21, pose.M22, pose.M23);
    }

    /// <summary>
    /// カメラの位置と姿勢を更新します。
    /// </summary>
    public void Update()
    {
        if (! needUpdate)
            return;

        //カメラ Z軸回転で姿勢を仮更新
        pose = Matrix.RotationZ(rotZD) * pose;

        //緯度経度の差分移動
        Vector3 localD = Vector3.TransformCoordinate(dirD, pose);
        if (localD.X != 0.0f || localD.Y != 0.0f || localD.Z != 0.0f)
        {
            //カメラ位置を更新
            Vector3 zAxis = GetZAxis();
            Vector3 rotAxis = Vector3.Cross(localD, zAxis);
            Quaternion q = Quaternion.RotationAxis(rotAxis, angleU * dirD.Length());
            Matrix rotation = Matrix.RotationQuaternion(q);
            localP = Vector3.TransformCoordinate(localP, rotation);

            //カメラ姿勢を更新
            Vector3 z = Vector3.Normalize(-localP);
            Vector3 y = GetYAxis();
            Vector3 x = Vector3.Normalize(Vector3.Cross(y, z));
            y = Vector3.Normalize(Vector3.Cross(z, x));
            {
                Matrix m = Matrix.Identity;
                m.M11 = x.X;
                m.M12 = x.Y;
                m.M13 = x.Z;
                m.M21 = y.X;
                m.M22 = y.Y;
                m.M23 = y.Z;
                m.M31 = z.X;
                m.M32 = z.Y;
                m.M33 = z.Z;
                pose = m;
            }
        }

        //奥行オフセットを更新
        if (zD != 0.0f && localP.Length() - zD > 0)
        {
            Vector3 z = Vector3.Normalize(-localP);
            localP += zD * z;
        }

        //view行列更新
        Vector3 worldP = localP + center;
        {
            Matrix m = pose;
            m.M41 = worldP.X;
            m.M42 = worldP.Y;
            m.M43 = worldP.Z;
            m.M44 = 1.0f;
            view = Matrix.Invert(m) * Matrix.Translation(-translation);
        }

        //差分をリセット
        ResetDefValue();
        needUpdate = false;
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
