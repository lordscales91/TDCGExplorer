using System;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

public class TSOCamera
{
    internal Vector3 center = Vector3.Empty;    //��]���S�ʒu
    internal Vector3 translation = Vector3.Empty;
    internal Vector3 camPosL = new Vector3(0.0f, 0.0f, -10.0f); //�J�����ʒu
    internal Vector3 camDirDef = Vector3.Empty; //�J�����ړ������x�N�g��
    internal float offsetZ = 0.0f;      //�J�������s�I�t�Z�b�g�l
    internal bool needUpdate = true;    //�X�V������
    internal Matrix viewMat = Matrix.Identity;  //�r���[�s��
    internal Matrix camPoseMat = Matrix.Identity;       //�J�����p���s��
    internal float camZRotDef = 0.0f;   //�J���� Z����]����
    internal float camAngleUnit = 0.02f;        //�ړ�����]�P�ʁi���W�A���j

    /// <summary>�J�����ʒu�X�V</summary>
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

    /// <summary>Z����]</summary>
    public void RotZ(float radian)
    {
        if (radian == 0.0f)
            return;

        camZRotDef = radian;
        needUpdate = true;
    }

    public Vector3 GetCamZAxis()
    {
        return new Vector3(camPoseMat.M31, camPoseMat.M32, camPoseMat.M33);
    }

    public Vector3 GetCamYAxis()
    {
        return new Vector3(camPoseMat.M21, camPoseMat.M22, camPoseMat.M23);
    }

    /// <summary>�J�����X�V</summary>
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
            //�J�����ʒu���X�V
            Vector3 camZAxis = GetCamZAxis();
            Vector3 rotAxis = Vector3.Cross(dL, camZAxis);
            Quaternion q = Quaternion.RotationAxis(rotAxis, camAngleUnit * camDirDef.Length());
            Matrix rotMat = Matrix.RotationQuaternion(q);
            camPosL = Vector3.TransformCoordinate(camPosL, rotMat);

            //�J�����p�����X�V
            Vector3 z = Vector3.Normalize(-camPosL);
            Vector3 y = GetCamYAxis();
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

        //���s�I�t�Z�b�g���X�V
        if (offsetZ != 0.0f && camPosL.Length() - offsetZ > 0)
        {
            Vector3 z = Vector3.Normalize(-camPosL);
            camPosL += offsetZ * z;
        }

        //�r���[�s��X�V
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

    /// <summary>�r���[�s����擾</summary>
    public Matrix GetViewMatrix()
    {
        return viewMat;
    }

    /// <summary>��]���S�ʒu��ݒ�</summary>
    public void SetCenter(Vector3 center)
    {
        this.center = center;
        needUpdate = true;
    }

    /// <summary>�ړ��ʒu��ݒ�</summary>
    public void SetTranslation(Vector3 translation)
    {
        this.translation = translation;
        needUpdate = true;
    }

    // ���������Z�b�g
    protected void ResetDefValue()
    {
        camDirDef = Vector3.Empty;
        offsetZ = 0.0f;
        camZRotDef = 0.0f;
    }
}
