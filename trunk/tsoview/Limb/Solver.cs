using System;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace Limb
{

public class Solver
{
    float upperBone;
    float lowerBone;

    public Solver()
    {
        this.upperBone = 0.0f;
        this.lowerBone = 0.0f;
    }

    public Solver(float upperBone, float lowerBone)
    {
        this.upperBone = upperBone;
        this.lowerBone = lowerBone;
    }

    public void InitStructure(float upperBone, float lowerBone)
    {
        this.upperBone = upperBone;
        this.lowerBone = lowerBone;
    }

    /// ���̑��݂𔻒�
    public bool IsSolution(ref Vector3 vgoal)
    {
        float distance = Vector3.Length(vgoal);
        return ! (distance >= upperBone + lowerBone || distance <= float.Epsilon);
    }
    public bool IsSolution(ref Matrix mgoal)
    {
        Vector3 vgoal = new Vector3(mgoal.M41, mgoal.M42, mgoal.M43);
        return IsSolution(ref vgoal);
    }

    public bool Solve(out Matrix mR1, out Matrix mRy, ref Vector3 vgoal, float swivel)
    {
        mR1 = Matrix.Identity;
        mRy = Matrix.Identity;

        // distance���Z���Ȃ�Ƃ�Ry�֐߂��ړ�����������i����W�n�j
        Vector3 va = new Vector3(-1.0f, 0.0f, 0.0f);

        // �ڕW�܂ł̋���
        float distance = Vector3.Length(vgoal);

        // �􉽊w�I�ɓ��B�s�\�ȏꍇ�͉�͎��s
        if (distance >= upperBone + lowerBone)
            return false;

        // �ڕW�����܂�ɂ��߂��ꍇ�͌v�Z�����U����̂ŁA�Ƃ肠������͎��s�Ƃ��Ă���
        if (distance <= float.Epsilon)
            return false;

        // �����N�n�n�_����ڕW�֌������P�ʃx�N�g��
        Vector3 vn = Vector3.Normalize(vgoal);

        // �]���藝���ARy�֐߂̉�]�ʂ��Z�o
        float fRy = (float)Math.Acos((distance * distance - upperBone * upperBone - lowerBone * lowerBone) / (2.0f * upperBone * lowerBone));
        if (fRy < 0)
            fRy += (float)Math.PI;
        mRy = Matrix.RotationY(fRy);

        // �]���藝���AR1->Ry�֐߃x�N�g����R1->R2�֐߃x�N�g���̂Ȃ��p���v�Z
        float cosa = (distance * distance + upperBone * upperBone - lowerBone * lowerBone) / (2.0f * distance * upperBone);
        Vector3 vc = cosa * upperBone * vn;
        float radius = (float)Math.Sqrt(1.0f - cosa * cosa) * upperBone;

        // swivel�p�̓��͂�degree��z��
        swivel = Geometry.DegreeToRadian(swivel);

        // swivel=0�ɂ�����O�Չ~���S����Ry�֐߂ւ̕����x�N�g��
        Vector3 vu = Vector3.Normalize(va - Vector3.Dot(va, vn) * vn);
        Vector3 vv = Vector3.Cross(vn, vu);

        // Ry�֐߂̈ʒu
        Vector3 ve = vc + radius * ((float)Math.Cos(swivel) * vu + (float)Math.Sin(swivel) * vv);

        // R1�֐߂̉�]�ʂ̌v�Z
        Vector3 vR1z = Vector3.Normalize(ve);
        Vector3 vR1x = Vector3.Normalize(vgoal - Vector3.Dot(vgoal, vR1z) * vR1z);
        Vector3 vR1y = Vector3.Cross(vR1z, vR1x);

        mR1.M11 = vR1x.X;
        mR1.M12 = vR1x.Y;
        mR1.M13 = vR1x.Z;
        mR1.M21 = vR1y.X;
        mR1.M22 = vR1y.Y;
        mR1.M23 = vR1y.Z;
        mR1.M31 = vR1z.X;
        mR1.M32 = vR1z.Y;
        mR1.M33 = vR1z.Z;

        return true;
    }

    public bool Solve(out Matrix mR1, out Matrix mRy, out Matrix mR2, ref Matrix mgoal, float swivel)
    {
        mR2 = Matrix.Identity;

        Matrix mgoalRot = mgoal;
        mgoalRot.M41 = 0;
        mgoalRot.M42 = 0;
        mgoalRot.M43 = 0;

        Vector3 vgoal = new Vector3(mgoal.M41, mgoal.M42, mgoal.M43);
        if (! Solve(out mR1, out mRy, ref vgoal, swivel))
            return false;

        mR2 = mgoalRot * Matrix.TransposeMatrix(mR1) * Matrix.TransposeMatrix(mRy);
        return true;
    }

    public bool GetSwivel(ref float swivel, ref Vector3 vgoal, ref Vector3 vRyPos)
    {
        // distance���Z���Ȃ�Ƃ�Ry�֐߂��ړ�����������i����W�n�j
        Vector3 va = new Vector3(-1.0f, 0.0f, 0.0f);

        // �ڕW�܂ł̋���
        float distance = Vector3.Length(vgoal);

        // �􉽊w�I�ɓ��B�s�\�ȏꍇ�͉�͎��s
        if (distance >= upperBone + lowerBone)
            return false;

        // �����N�n�n�_����ڕW�֌������P�ʃx�N�g��
        Vector3 vn = Vector3.Normalize(vgoal);

        // swivel=0�ɂ�����O�Չ~���S����Ry�֐߂ւ̕����x�N�g��
        Vector3 vu = Vector3.Normalize(va - Vector3.Dot(va, vn) * vn);
        Vector3 vv = Vector3.Cross(vn, vu);

        // �]���藝���AR1->Ry�֐߃x�N�g����R1->R2�֐߃x�N�g���̂Ȃ��p���v�Z
        float cosa = (distance * distance + upperBone * upperBone - lowerBone * lowerBone) / (2.0f * distance * upperBone);
        Vector3 vc = cosa * upperBone * vn;
        float radius = (float)Math.Sqrt(1.0f - cosa * cosa) * upperBone;

        Vector3 vp = vRyPos - vc;
        Vector3 vs = vp - Vector3.Dot(vp, vn) * vn;
        Vector3 vt = Vector3.Cross(vs, vu);

        swivel = (float)Math.Atan2(Vector3.Length(vt), Vector3.Dot(vs, vu));

        // Ry�֐߂̈ʒu
        Vector3 ve = vc + radius * ((float)Math.Cos(swivel) * vu + (float)Math.Sin(swivel) * vv);

        if (Vector3.Length(ve - vRyPos) > 1.0f)
            swivel = -swivel;
        swivel = Geometry.RadianToDegree(swivel);
        return true;
    }

    public bool GetSwivel(ref float swivel, ref Matrix mgoal, ref Vector3 vRyPos)
    {
        Vector3 vgoal = new Vector3(mgoal.M41, mgoal.M42, mgoal.M43);
        return GetSwivel(ref swivel, ref vgoal, ref vRyPos);
    }
}
}
