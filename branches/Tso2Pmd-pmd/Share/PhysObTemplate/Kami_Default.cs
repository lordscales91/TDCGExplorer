using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

using TDCG;
using TDCGUtils;
using Tso2Pmd;

namespace TDCG.PhysObTemplate
{
    public class Kami_Default : IPhysObTemplate
    {
        string name = "�f�t�H���g"; // �g�ɕ\������閼�O
        int group = 0;              // �\�������g�̎�ށi0:�� 1:�� 2:�X�J�[�g 3:���̑��j

        public string Name() { return name; }
        public int Group() { return group; }

        public void Execute(ref T2PPhysObjectList phys_list)
        {
            phys_list.MakeChain("������P");
            phys_list.MakeChain("�������P");
            phys_list.MakeChain("�E�����P");
            phys_list.MakeChain("������P");
            phys_list.MakeChain("�E����P");
            phys_list.MakeChain("�����O�P");
            phys_list.MakeChain("�E���O�P");

            SetParameter(phys_list.GetBodyListByName(".��.*"));
            SetParameterEnd(phys_list.GetBodyListByName(".��.*��"));
            SetParameter(phys_list.GetJointListByName(".��.*"));

            phys_list.GetBodyByName("������P").rigidbody_type = 0; // ���f�[�^�F�^�C�v(0:Bone�Ǐ]�A1:�������Z�A2:�������Z(Bone�ʒu����)) // 00 // Bone�Ǐ]
            phys_list.GetBodyByName("������Q").rigidbody_type = 0; // ���f�[�^�F�^�C�v(0:Bone�Ǐ]�A1:�������Z�A2:�������Z(Bone�ʒu����)) // 00 // Bone�Ǐ]
            phys_list.GetJointByName("������Q-������R").constrain_rot_1.X = (float)((-5.0 / 180.0) * Math.PI); // �����F��]1(rad(x), rad(y), rad(z))
            phys_list.GetJointByName("������Q-������R").constrain_rot_2.X = (float)((30.0 / 180.0) * Math.PI); // �����F��]2(rad(x), rad(y), rad(z))
            phys_list.GetJointByName("������R-������S").constrain_rot_1.X = (float)((-5.0 / 180.0) * Math.PI); // �����F��]1(rad(x), rad(y), rad(z))
            phys_list.GetJointByName("������R-������S").constrain_rot_2.X = (float)((30.0 / 180.0) * Math.PI); // �����F��]2(rad(x), rad(y), rad(z))

            phys_list.GetBodyByName("�������P").rigidbody_type = 0; // ���f�[�^�F�^�C�v(0:Bone�Ǐ]�A1:�������Z�A2:�������Z(Bone�ʒu����)) // 00 // Bone�Ǐ]
            phys_list.GetBodyByName("�������Q").rigidbody_type = 0; // ���f�[�^�F�^�C�v(0:Bone�Ǐ]�A1:�������Z�A2:�������Z(Bone�ʒu����)) // 00 // Bone�Ǐ]
            phys_list.GetBodyByName("�������R").rigidbody_type = 0; // ���f�[�^�F�^�C�v(0:Bone�Ǐ]�A1:�������Z�A2:�������Z(Bone�ʒu����)) // 00 // Bone�Ǐ]

            phys_list.GetBodyByName("�E�����P").rigidbody_type = 0; // ���f�[�^�F�^�C�v(0:Bone�Ǐ]�A1:�������Z�A2:�������Z(Bone�ʒu����)) // 00 // Bone�Ǐ]
            phys_list.GetBodyByName("�E�����Q").rigidbody_type = 0; // ���f�[�^�F�^�C�v(0:Bone�Ǐ]�A1:�������Z�A2:�������Z(Bone�ʒu����)) // 00 // Bone�Ǐ]
            phys_list.GetBodyByName("�E�����R").rigidbody_type = 0; // ���f�[�^�F�^�C�v(0:Bone�Ǐ]�A1:�������Z�A2:�������Z(Bone�ʒu����)) // 00 // Bone�Ǐ]

            phys_list.GetBodyByName("������P").rigidbody_type = 0; // ���f�[�^�F�^�C�v(0:Bone�Ǐ]�A1:�������Z�A2:�������Z(Bone�ʒu����)) // 00 // Bone�Ǐ]
            phys_list.GetBodyByName("������Q").rigidbody_type = 0; // ���f�[�^�F�^�C�v(0:Bone�Ǐ]�A1:�������Z�A2:�������Z(Bone�ʒu����)) // 00 // Bone�Ǐ]
            phys_list.GetJointByName("������Q-������R").constrain_rot_1.X = (float)((-5.0 / 180.0) * Math.PI); // �����F��]1(rad(x), rad(y), rad(z))
            phys_list.GetJointByName("������Q-������R").constrain_rot_2.X = (float)((30.0 / 180.0) * Math.PI); // �����F��]2(rad(x), rad(y), rad(z))
            phys_list.GetJointByName("������R-������S").constrain_rot_1.X = (float)((-5.0 / 180.0) * Math.PI); // �����F��]1(rad(x), rad(y), rad(z))
            phys_list.GetJointByName("������R-������S").constrain_rot_2.X = (float)((30.0 / 180.0) * Math.PI); // �����F��]2(rad(x), rad(y), rad(z))

            phys_list.GetBodyByName("�E����P").rigidbody_type = 0; // ���f�[�^�F�^�C�v(0:Bone�Ǐ]�A1:�������Z�A2:�������Z(Bone�ʒu����)) // 00 // Bone�Ǐ]
            phys_list.GetBodyByName("�E����Q").rigidbody_type = 0; // ���f�[�^�F�^�C�v(0:Bone�Ǐ]�A1:�������Z�A2:�������Z(Bone�ʒu����)) // 00 // Bone�Ǐ]
            phys_list.GetJointByName("�E����Q-�E����R").constrain_rot_1.X = (float)((-5.0 / 180.0) * Math.PI); // �����F��]1(rad(x), rad(y), rad(z))
            phys_list.GetJointByName("�E����Q-�E����R").constrain_rot_2.X = (float)((30.0 / 180.0) * Math.PI); // �����F��]2(rad(x), rad(y), rad(z))
            phys_list.GetJointByName("�E����R-�E����S").constrain_rot_1.X = (float)((-5.0 / 180.0) * Math.PI); // �����F��]1(rad(x), rad(y), rad(z))
            phys_list.GetJointByName("�E����R-�E����S").constrain_rot_2.X = (float)((30.0 / 180.0) * Math.PI); // �����F��]2(rad(x), rad(y), rad(z))

            phys_list.GetBodyByName("�����O�P").rigidbody_type = 0; // ���f�[�^�F�^�C�v(0:Bone�Ǐ]�A1:�������Z�A2:�������Z(Bone�ʒu����)) // 00 // Bone�Ǐ]
            phys_list.GetBodyByName("�����O�Q").rigidbody_type = 0; // ���f�[�^�F�^�C�v(0:Bone�Ǐ]�A1:�������Z�A2:�������Z(Bone�ʒu����)) // 00 // Bone�Ǐ]
            phys_list.GetBodyByName("�����O�R").rigidbody_type = 0; // ���f�[�^�F�^�C�v(0:Bone�Ǐ]�A1:�������Z�A2:�������Z(Bone�ʒu����)) // 00 // Bone�Ǐ]
            phys_list.GetBodyByName("�����O��").rigidbody_type = 0; // ���f�[�^�F�^�C�v(0:Bone�Ǐ]�A1:�������Z�A2:�������Z(Bone�ʒu����)) // 00 // Bone�Ǐ]

            phys_list.GetBodyByName("�E���O�P").rigidbody_type = 0; // ���f�[�^�F�^�C�v(0:Bone�Ǐ]�A1:�������Z�A2:�������Z(Bone�ʒu����)) // 00 // Bone�Ǐ]
            phys_list.GetBodyByName("�E���O�Q").rigidbody_type = 0; // ���f�[�^�F�^�C�v(0:Bone�Ǐ]�A1:�������Z�A2:�������Z(Bone�ʒu����)) // 00 // Bone�Ǐ]
            phys_list.GetBodyByName("�E���O�R").rigidbody_type = 0; // ���f�[�^�F�^�C�v(0:Bone�Ǐ]�A1:�������Z�A2:�������Z(Bone�ʒu����)) // 00 // Bone�Ǐ]
            phys_list.GetBodyByName("�E���O��").rigidbody_type = 0; // ���f�[�^�F�^�C�v(0:Bone�Ǐ]�A1:�������Z�A2:�������Z(Bone�ʒu����)) // 00 // Bone�Ǐ]
        }

        private void SetParameter(List<PMD_RigidBody> body_list)
        {
            foreach (PMD_RigidBody body in body_list)
            {
                body.rigidbody_group_index = 2; // ���f�[�^�F�O���[�v // 00
                body.rigidbody_group_target = 1; // ���f�[�^�F�O���[�v�F�Ώ� // 0xFFFF�Ƃ̍� // 38 FE
                body.shape_type = 2; // �`��F�^�C�v(0:���A1:���A2:�J�v�Z��) // 00 // ��
                body.shape_w = 0.4f; // �`��F���a(��) // CD CC CC 3F // 1.6
                body.shape_h *= 0.6f; // �`��F���� // CD CC CC 3D // 0.1

                body.rigidbody_weight = 0.5f; // ���f�[�^�F���� // 00 00 80 3F // 1.0
                body.rigidbody_pos_dim = 0.8f; // ���f�[�^�F�ړ��� // 00 00 00 00
                body.rigidbody_rot_dim = 0.8f; // ���f�[�^�F��]�� // 00 00 00 00
                body.rigidbody_recoil = 0.0f; // ���f�[�^�F������ // 00 00 00 00
                body.rigidbody_friction = 0.8f; // ���f�[�^�F���C�� // 00 00 00 00
                body.rigidbody_type = 1; // ���f�[�^�F�^�C�v(0:Bone�Ǐ]�A1:�������Z�A2:�������Z(Bone�ʒu����)) // 00 // Bone�Ǐ]
            }
        }

        private void SetParameterEnd(List<PMD_RigidBody> body_list)
        {
            foreach (PMD_RigidBody body in body_list)
            {
                body.rigidbody_group_index = 2; // ���f�[�^�F�O���[�v // 00
                body.rigidbody_group_target = 1; // ���f�[�^�F�O���[�v�F�Ώ� // 0xFFFF�Ƃ̍� // 38 FE
                body.shape_type = 0; // �`��F�^�C�v(0:���A1:���A2:�J�v�Z��) // 00 // ��
                body.shape_w = 0.3f; // �`��F���a(��) // CD CC CC 3F // 1.6

                body.rigidbody_weight = 1.0f; // ���f�[�^�F���� // 00 00 80 3F // 1.0
                body.rigidbody_pos_dim = 0.8f; // ���f�[�^�F�ړ��� // 00 00 00 00
                body.rigidbody_rot_dim = 0.8f; // ���f�[�^�F��]�� // 00 00 00 00
                body.rigidbody_recoil = 0.0f; // ���f�[�^�F������ // 00 00 00 00
                body.rigidbody_friction = 0.8f; // ���f�[�^�F���C�� // 00 00 00 00
                body.rigidbody_type = 1; // ���f�[�^�F�^�C�v(0:Bone�Ǐ]�A1:�������Z�A2:�������Z(Bone�ʒu����)) // 00 // Bone�Ǐ]
            }
        }

        private void SetParameter(List<PMD_Joint> joint_list)
        {
            foreach (PMD_Joint joint in joint_list)
            {
                joint.constrain_rot_1.X = (float)((-20.0 / 180.0) * Math.PI); // �����F��]1(rad(x), rad(y), rad(z))
                joint.constrain_rot_2.X = (float)((20.0 / 180.0) * Math.PI); // �����F��]2(rad(x), rad(y), rad(z))
                joint.constrain_rot_1.Z = (float)((-20.0 / 180.0) * Math.PI); // �����F��]1(rad(x), rad(y), rad(z))
                joint.constrain_rot_2.Z = (float)((20.0 / 180.0) * Math.PI); // �����F��]2(rad(x), rad(y), rad(z))
                joint.spring_rot.X = 10.0f; // �΂ˁF��](rad(x), rad(y), rad(z))
                joint.spring_rot.Y = 10.0f; // �΂ˁF��](rad(x), rad(y), rad(z))
                joint.spring_rot.Z = 10.0f; // �΂ˁF��](rad(x), rad(y), rad(z))
            }
        }
    }
}
