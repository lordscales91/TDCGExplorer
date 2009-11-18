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
    public class SimpleCamera
    {
        private Vector3 center = Vector3.Empty;
        private Vector3 translation = new Vector3(0.0f, 0.0f, +10.0f);
        private Vector3 dirD = Vector3.Empty; //�J�����ړ������x�N�g��
        private float zD = 0.0f;      //�J�������s�I�t�Z�b�g�l
        private bool needUpdate = true;    //�X�V����K�v�����邩
        private Matrix view = Matrix.Identity;  //�r���[�s��
        private Vector3 angle = Vector3.Empty;
        private float rotZD = 0.0f;   //�J���� Z����]����
        private float angleU = 0.01f;        //�ړ�����]�P�ʁi���W�A���j

        /// <summary>
        /// ��]���S
        /// </summary>
        public Vector3 Center { get { return center; } set { center = value; } }

        /// <summary>
        /// view���W��̃J�����̈ʒu
        /// </summary>
        public Vector3 Translation { get { return translation; } set { translation = value; } }

        /// <summary>
        /// �J�����̈ʒu�Ǝp�������Z�b�g���܂��B
        /// </summary>
        public void Reset()
        {
            center = Vector3.Empty;
            translation = new Vector3(0.0f, 0.0f, +10.0f);
            angle = Vector3.Empty;
            needUpdate = true;
        }

        /// <summary>
        /// �J�����̈ʒu���X�V���܂��B
        /// </summary>
        /// <param name="dirX">�ړ������i�o�x�j</param>
        /// <param name="dirY">�ړ������i�ܓx�j</param>
        /// <param name="dirZ">�ړ������i���s�j</param>
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
        /// �J������Z����]���܂��B
        /// </summary>
        /// <param name="angle">��]�p�x�i���W�A���j</param>
        public void RotZ(float angle)
        {
            if (angle == 0.0f)
                return;

            rotZD = angle;
            needUpdate = true;
        }

        /// <summary>
        /// �J�����̈ʒu�Ǝp�����X�V���܂��B
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

            //���������Z�b�g
            ResetDefValue();
            needUpdate = false;
        }

        /// <summary>
        /// view�s����擾���܂��B
        /// </summary>
        public Matrix GetViewMatrix()
        {
            return view;
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
        /// ��]���S��ݒ肵�܂��B
        /// </summary>
        /// <param name="x">��]���Sx���W</param>
        /// <param name="y">��]���Sy���W</param>
        /// <param name="z">��]���Sz���W</param>
        public void SetCenter(float x, float y, float z)
        {
            SetCenter(new Vector3(x, y, z));
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
        /// view���W��̈ʒu��ݒ肵�܂��B
        /// </summary>
        /// <param name="x">view���W��̈ʒux���W</param>
        /// <param name="y">view���W��̈ʒuy���W</param>
        /// <param name="z">view���W��̈ʒuz���W</param>
        public void SetTranslation(float x, float y, float z)
        {
            SetTranslation(new Vector3(x, y, z));
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
            dirD = Vector3.Empty;
            zD = 0.0f;
        }

        // konoa modified.
        public Vector3 Angle
        {
            get { return angle; }
            set { angle = value; needUpdate = true; }
        }

    }
}
