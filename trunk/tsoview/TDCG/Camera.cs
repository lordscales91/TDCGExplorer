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
    /// �J����
    /// </summary>
public class Camera
{
    internal Vector3 center = Vector3.Empty;
    /// <summary>
    /// ��]���S
    /// </summary>
    public Vector3 Center { get { return center; } set { center = value; } }

    internal Vector3 translation = Vector3.Empty;
    /// <summary>
    /// view���W��̃J�����̈ʒu
    /// </summary>
    public Vector3 Translation { get { return translation; } set { translation = value; } }

    internal Vector3 camPosL = new Vector3(0.0f, 0.0f, -10.0f);
    /// <summary>
    /// �����_�����_�Ƃ������W��̃J�����̈ʒu
    /// </summary>
    public Vector3 CamPosL { get { return camPosL; } set { camPosL = value; } }
    
    internal Vector3 camDirDef = Vector3.Empty; //�J�����ړ������x�N�g��
    internal float offsetZ = 0.0f;      //�J�������s�I�t�Z�b�g�l
    internal bool needUpdate = true;    //�X�V������
    internal Matrix viewMat = Matrix.Identity;  //�r���[�s��

    internal Matrix camPoseMat = Matrix.Identity;
    /// <summary>
    /// �J�����̎p���s��
    /// </summary>
    public Matrix CamPoseMat { get { return camPoseMat; } set { camPoseMat = value; } }

    internal float camZRotDef = 0.0f;   //�J���� Z����]����
    internal float camAngleUnit = 0.02f;        //�ړ�����]�P�ʁi���W�A���j

    /// <summary>
    /// �J�����𐶐����܂��B
    /// </summary>
    public Camera()
    {
        motion = new CameraMotion(this);
    }

    /// <summary>
    /// �J�����̈ʒu�Ǝp����W���o�͂֏����o���܂��B
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
    /// �J�����̈ʒu�Ǝp�����w��p�X�֏����o���܂��B
    /// </summary>
    /// <param name="dest_file">�p�X</param>
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
    /// �J�����̈ʒu�Ǝp�����w��p�X����ǂݍ��݂܂��B
    /// </summary>
    /// <param name="source_file">�p�X</param>
    /// <returns>�J����</returns>
    public static Camera Load(string source_file)
    {
        XmlReader reader = XmlReader.Create(source_file);
        XmlSerializer serializer = new XmlSerializer(typeof(Camera));
        Camera camera = serializer.Deserialize(reader) as Camera;
        reader.Close();
        return camera;
    }

    /// <summary>
    /// �J�����̈ʒu�Ǝp����⊮���܂��B
    /// </summary>
    /// <param name="cam1">��ԊJ�n���̈ʒu���p����ێ�����J����</param>
    /// <param name="cam2">��ԏI�����̈ʒu���p����ێ�����J����</param>
    /// <param name="ratio">��Ԕ䗦</param>
    /// <returns>�J����</returns>
    public static Camera Interpolation(Camera cam1, Camera cam2, float ratio)
    {
        Camera camera = new Camera();
        camera.Center = Vector3.Lerp(cam1.Center, cam2.Center, ratio);
        camera.Translation = Vector3.Lerp(cam1.Translation, cam2.Translation, ratio);
        camera.CamPosL = Vector3.Lerp(cam1.CamPosL, cam2.CamPosL, ratio);
        Quaternion q1 = Quaternion.RotationMatrix(cam1.CamPoseMat);
        Quaternion q2 = Quaternion.RotationMatrix(cam2.CamPoseMat);
        camera.CamPoseMat = Matrix.RotationQuaternion(Quaternion.Slerp(q1, q2, ratio));
        return camera;
    }

    /// <summary>
    /// �J�����̈ʒu�Ǝp����⊮���܂��B
    /// </summary>
    /// <param name="cam1">��ԊJ�n���̈ʒu���p����ێ�����J����</param>
    /// <param name="cam2">��ԏI�����̈ʒu���p����ێ�����J����</param>
    /// <param name="ratio">��Ԕ䗦</param>
    public void Interp(Camera cam1, Camera cam2, float ratio)
    {
        center = Vector3.Lerp(cam1.Center, cam2.Center, ratio);
        translation = Vector3.Lerp(cam1.Translation, cam2.Translation, ratio);
        camPosL = Vector3.Lerp(cam1.CamPosL, cam2.CamPosL, ratio);
        Quaternion q1 = Quaternion.RotationMatrix(cam1.CamPoseMat);
        Quaternion q2 = Quaternion.RotationMatrix(cam2.CamPoseMat);
        camPoseMat = Matrix.RotationQuaternion(Quaternion.Slerp(q1, q2, ratio));

        //view�s��X�V
        Vector3 posW = camPosL + center;
        {
            Matrix m = camPoseMat;
            m.M41 = posW.X;
            m.M42 = posW.Y;
            m.M43 = posW.Z;
            m.M44 = 1.0f;
            viewMat = Matrix.Invert(m) * Matrix.Translation(-translation);
        }

        //���������Z�b�g
        ResetDefValue();
        needUpdate = false;
    }

    /// <summary>
    /// �J�����̈ʒu�Ǝp�������Z�b�g���܂��B
    /// </summary>
    public void Reset()
    {
        center = Vector3.Empty;
        translation = Vector3.Empty;
        camPosL = new Vector3(0.0f, 0.0f, -10.0f);
        camPoseMat = Matrix.Identity;
        needUpdate = true;
    }

    /// <summary>
    /// �J�����̈ʒu���X�V���܂��B
    /// </summary>
    /// <param name="camDirX">�ړ������i�o�x�j</param>
    /// <param name="camDirY">�ړ������i�ܓx�j</param>
    /// <param name="offsetZ">���s�I�t�Z�b�g�l</param>
    public void Move(float camDirX, float camDirY, float offsetZ)
    {
        if (camDirX == 0.0f && camDirY == 0.0f && offsetZ == 0.0f)
            return;

        camDirDef.X += camDirX;
        camDirDef.Y += camDirY;
        this.offsetZ += offsetZ;
        needUpdate = true;
    }

    /// <summary>
    /// �J������Z����]���܂��B
    /// </summary>
    /// <param name="radian">��]�p�x�i���W�A���j</param>
    public void RotZ(float radian)
    {
        if (radian == 0.0f)
            return;

        camZRotDef = radian;
        needUpdate = true;
    }

    /// <summary>
    /// �J�����̈ʒu�Ǝp�����X�V���܂��B
    /// �}�E�X�̉�]���S�͌��_�Ƀ��Z�b�g���܂��B
    /// ���ӁF���̑���� Move() RotZ() Update() �Ƃ͈قȂ�n���ł��B
    /// </summary>
    /// <param name="eye">�����_</param>
    /// <param name="center">view���W��̃J�����̈ʒu</param>
    /// <param name="up">����x�N�g��</param>
    public void LookAt(Vector3 eye, Vector3 center, Vector3 up)
    {
        this.camPosL = center - eye;
        {
            // �J�����p�����X�V
            Vector3 z = Vector3.Normalize(-camPosL);
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
                this.camPoseMat = m;
            }
        }
        this.center = Vector3.Empty;
        this.translation = eye;

        //view�s��X�V
        Vector3 posW = camPosL + this.center;
        {
            Matrix m = camPoseMat;
            m.M41 = posW.X;
            m.M42 = posW.Y;
            m.M43 = posW.Z;
            m.M44 = 1.0f;
            viewMat = Matrix.Invert(m) * Matrix.Translation(-translation);
        }

        //���������Z�b�g
        ResetDefValue();
        needUpdate = false;
    }

    /// <summary>
    /// �J�����̈ʒu�Ǝp�����X�V���܂��B
    /// �}�E�X�̉�]���S�͌��_�Ƀ��Z�b�g���܂��B
    /// ���ӁF���̑���� Move() RotZ() Update() �Ƃ͈قȂ�n���ł��B
    /// </summary>
    /// <param name="eye">�����_</param>
    /// <param name="center">view���W��̃J�����̈ʒu</param>
    public void LookAt(Vector3 eye, Vector3 center)
    {
        LookAt(eye, center, new Vector3(0.0f, 1.0f, 0.0f));
    }

    /// <summary>
    /// �J�����̈ʒu�Ǝp�����X�V���܂��B
    /// </summary>
    public void Update()
    {
        if (! needUpdate)
            return;

        //�J���� Z����]�Ŏp�������X�V
        camPoseMat = Matrix.RotationZ(camZRotDef) * camPoseMat;

        //�ܓx�o�x�̍����ړ�
        Vector3 dL = Vector3.TransformCoordinate(camDirDef, camPoseMat);
        if (dL.X != 0.0f || dL.Y != 0.0f || dL.Z != 0.0f)
        {
            Vector3 camZAxis = new Vector3(camPoseMat.M31, camPoseMat.M32, camPoseMat.M33);
            Vector3 rotAxis = Vector3.Cross(dL, camZAxis);
            Quaternion q = Quaternion.RotationAxis(rotAxis, camAngleUnit * camDirDef.Length());
            Matrix rotMat = Matrix.RotationQuaternion(q);
            camPosL = Vector3.TransformCoordinate(camPosL, rotMat);

            // �ړ���J�����p�����X�V
            Vector3 z = Vector3.Normalize(-camPosL);
            Vector3 y = new Vector3(camPoseMat.M21, camPoseMat.M22, camPoseMat.M23);
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
                camPoseMat = m;
            }
        }

        // ���s�I�t�Z�b�g���X�V
        if (camPosL.Length() - offsetZ > 0)
        {
            Vector3 z = Vector3.Normalize(-camPosL);
            camPosL += offsetZ * z;
        }

        //view�s��X�V
        Vector3 posW = camPosL + center;
        {
            Matrix m = camPoseMat;
            m.M41 = posW.X;
            m.M42 = posW.Y;
            m.M43 = posW.Z;
            m.M44 = 1.0f;
            viewMat = Matrix.Invert(m) * Matrix.Translation(-translation);
        }

        //���������Z�b�g
        ResetDefValue();
        needUpdate = false;
    }

    /// <summary>
    /// view�s����擾���܂��B
    /// </summary>
    public Matrix GetViewMatrix()
    {
        return viewMat;
    }

    /// <summary>
    /// ��]���S��ݒ肵�܂��B
    /// </summary>
    /// <param name="center">��]���S</param>
    public void SetCenter(Vector3 center)
    {
        this.center = center;
        needUpdate = true;
    }

    /// <summary>
    /// view���W��̈ʒu��ݒ肵�܂��B
    /// </summary>
    /// <param name="translation">view���W��̈ʒu</param>
    public void SetTranslation(Vector3 translation)
    {
        this.translation = translation;
        needUpdate = true;
    }

    /// <summary>
    /// view���W��ňړ����܂��B
    /// </summary>
    /// <param name="dx">X���ړ�����</param>
    /// <param name="dy">Y���ړ�����</param>
    public void MoveView(float dx, float dy)
    {
        this.translation.X += dx;
        this.translation.Y += dy;
        needUpdate = true;
    }

    /// <summary>
    /// ���������Z�b�g���܂��B
    /// </summary>
    protected void ResetDefValue()
    {
        camDirDef = Vector3.Empty;
        offsetZ = 0.0f;
        camZRotDef = 0.0f;
    }

    private CameraMotion motion = null;

    /// <summary>
    /// �J�������[�V����
    /// </summary>
    public CameraMotion Motion
    {
        get { return motion; }
    }

    /// <summary>
    /// �J�������[�V������ݒ肵�܂��B
    /// </summary>
    /// <param name="frame_index">�t���[���ԍ�</param>
    /// <param name="eye">�����_</param>
    /// <param name="center">�J�����̈ʒu</param>
    public void SetMotion(int frame_index, Vector3 eye, Vector3 center)
    {
        motion.Add(frame_index, eye, center);
    }

    /// <summary>
    /// �J�������[�V������ݒ肵�܂��B
    /// </summary>
    /// <param name="frame_index">�t���[���ԍ�</param>
    /// <param name="eye">�����_</param>
    /// <param name="center">�J�����̈ʒu</param>
    /// <param name="interp_length">��Ԃ���t���[������</param>
    public void SetMotion(int frame_index, Vector3 eye, Vector3 center, int interp_length)
    {
        motion.Add(frame_index, eye, center, interp_length);
    }

    /// <summary>
    /// �J�������[�V������ݒ肵�܂��B
    /// </summary>
    /// <param name="frame_index">�t���[���ԍ�</param>
    /// <param name="eyex">�����_��X���W</param>
    /// <param name="eyey">�����_��Y���W</param>
    /// <param name="eyez">�����_��Z���W</param>
    /// <param name="centerx">�J�����̈ʒu��X���W</param>
    /// <param name="centery">�J�����̈ʒu��Y���W</param>
    /// <param name="centerz">�J�����̈ʒu��Z���W</param>
    public void SetMotion(int frame_index, float eyex, float eyey, float eyez, float centerx, float centery, float centerz)
    {
        motion.Add(frame_index, new Vector3(eyex, eyey, eyez), new Vector3(centerx, centery, centerz));
    }

    /// <summary>
    /// �J�������[�V������ݒ肵�܂��B
    /// </summary>
    /// <param name="frame_index">�t���[���ԍ�</param>
    /// <param name="eyex">�����_��X���W</param>
    /// <param name="eyey">�����_��Y���W</param>
    /// <param name="eyez">�����_��Z���W</param>
    /// <param name="centerx">�J�����̈ʒu��X���W</param>
    /// <param name="centery">�J�����̈ʒu��Y���W</param>
    /// <param name="centerz">�J�����̈ʒu��Z���W</param>
    /// <param name="interp_length">��Ԃ���t���[������</param>
    public void SetMotion(int frame_index, float eyex, float eyey, float eyez, float centerx, float centery, float centerz, int interp_length)
    {
        motion.Add(frame_index, new Vector3(eyex, eyey, eyez), new Vector3(centerx, centery, centerz), interp_length);
    }

    /// <summary>
    /// ���̃��[�V�����t���[���ɐi�݂܂��B
    /// </summary>
    public void NextFrame()
    {
        if (motion.Count != 0)
        {
            motion.NextFrame();
        }
    }
}
}
