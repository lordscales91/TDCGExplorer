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
    public class Skirt_Default : IPhysObTemplate
    {
        string name = "�f�t�H���g"; // �g�ɕ\������閼�O
        int group = 2;              // �\�������g�̎�ށi0:�� 1:�� 2:�X�J�[�g 3:���̑��j

        public string Name() { return name; }
        public int Group() { return group; }

        public void Execute(ref T2PPhysObjectList phys_list)
        {
            phys_list.MakeChain("���X��P");
            phys_list.MakeChain("�E�X��P");
            phys_list.MakeChain("�E�X���P");
            phys_list.MakeChain("�E�X�O�P");
            phys_list.MakeChain("���X�O�P");
            phys_list.MakeChain("���X���P");

            SetParameter(phys_list.GetBodyListByName(".�X.."));
            SetParameterSide(phys_list.GetBodyListByName(".�X��."));
            SetParameterEnd(phys_list.GetBodyListByName(".�X.��"));
            SetParameterSideEnd(phys_list.GetBodyListByName(".�X����"));
            SetParameter(phys_list.GetJointListByName(".�X.."));

            phys_list.GetJointByName("�����g-���X��P").constrain_rot_1.X = (float)((-0.0 / 180.0) * Math.PI); // �����F��]1(rad(x), rad(y), rad(z))
            phys_list.GetJointByName("�����g-���X��P").constrain_rot_2.X = (float)((60.0 / 180.0) * Math.PI); // �����F��]2(rad(x), rad(y), rad(z))
            phys_list.GetJointByName("�����g-���X��P").constrain_rot_1.Y = (float)((-0.0 / 180.0) * Math.PI); // �����F��]1(rad(x), rad(y), rad(z))
            phys_list.GetJointByName("�����g-���X��P").constrain_rot_2.Y = (float)((0.0 / 180.0) * Math.PI); // �����F��]2(rad(x), rad(y), rad(z))
            phys_list.GetJointByName("�����g-���X��P").constrain_rot_1.Z = (float)((-5.0 / 180.0) * Math.PI); // �����F��]1(rad(x), rad(y), rad(z))
            phys_list.GetJointByName("�����g-���X��P").constrain_rot_2.Z = (float)((5.0 / 180.0) * Math.PI); // �����F��]2(rad(x), rad(y), rad(z))
            phys_list.GetJointByName("�����g-���X��P").spring_rot.X = 50.0f; // �΂ˁF��](rad(x), rad(y), rad(z))
            phys_list.GetJointByName("�����g-���X��P").spring_rot.Y = 0.0f; // �΂ˁF��](rad(x), rad(y), rad(z))
            phys_list.GetJointByName("�����g-���X��P").spring_rot.Z = 0.0f; // �΂ˁF��](rad(x), rad(y), rad(z))

            phys_list.GetJointByName("�����g-�E�X��P").constrain_rot_1.X = (float)((-0.0 / 180.0) * Math.PI); // �����F��]1(rad(x), rad(y), rad(z))
            phys_list.GetJointByName("�����g-�E�X��P").constrain_rot_2.X = (float)((60.0 / 180.0) * Math.PI); // �����F��]2(rad(x), rad(y), rad(z))
            phys_list.GetJointByName("�����g-�E�X��P").constrain_rot_1.Y = (float)((-0.0 / 180.0) * Math.PI); // �����F��]1(rad(x), rad(y), rad(z))
            phys_list.GetJointByName("�����g-�E�X��P").constrain_rot_2.Y = (float)((0.0 / 180.0) * Math.PI); // �����F��]2(rad(x), rad(y), rad(z))
            phys_list.GetJointByName("�����g-�E�X��P").constrain_rot_1.Z = (float)((-5.0 / 180.0) * Math.PI); // �����F��]1(rad(x), rad(y), rad(z))
            phys_list.GetJointByName("�����g-�E�X��P").constrain_rot_2.Z = (float)((5.0 / 180.0) * Math.PI); // �����F��]2(rad(x), rad(y), rad(z))
            phys_list.GetJointByName("�����g-�E�X��P").spring_rot.X = 50.0f; // �΂ˁF��](rad(x), rad(y), rad(z))
            phys_list.GetJointByName("�����g-�E�X��P").spring_rot.Y = 0.0f; // �΂ˁF��](rad(x), rad(y), rad(z))
            phys_list.GetJointByName("�����g-�E�X��P").spring_rot.Z = 0.0f; // �΂ˁF��](rad(x), rad(y), rad(z))

            phys_list.GetJointByName("�����g-�E�X���P").constrain_rot_1.X = (float)((-5.0 / 180.0) * Math.PI); // �����F��]1(rad(x), rad(y), rad(z))
            phys_list.GetJointByName("�����g-�E�X���P").constrain_rot_2.X = (float)((5.0 / 180.0) * Math.PI); // �����F��]2(rad(x), rad(y), rad(z))
            phys_list.GetJointByName("�����g-�E�X���P").constrain_rot_1.Y = (float)((-0.0 / 180.0) * Math.PI); // �����F��]1(rad(x), rad(y), rad(z))
            phys_list.GetJointByName("�����g-�E�X���P").constrain_rot_2.Y = (float)((0.0 / 180.0) * Math.PI); // �����F��]2(rad(x), rad(y), rad(z))
            phys_list.GetJointByName("�����g-�E�X���P").constrain_rot_1.Z = (float)((-15.0 / 180.0) * Math.PI); // �����F��]1(rad(x), rad(y), rad(z))
            phys_list.GetJointByName("�����g-�E�X���P").constrain_rot_2.Z = (float)((60.0 / 180.0) * Math.PI); // �����F��]2(rad(x), rad(y), rad(z))
            phys_list.GetJointByName("�����g-�E�X���P").spring_rot.X = 0.0f; // �΂ˁF��](rad(x), rad(y), rad(z))
            phys_list.GetJointByName("�����g-�E�X���P").spring_rot.Y = 0.0f; // �΂ˁF��](rad(x), rad(y), rad(z))
            phys_list.GetJointByName("�����g-�E�X���P").spring_rot.Z = 50.0f; // �΂ˁF��](rad(x), rad(y), rad(z))

            phys_list.GetJointByName("�����g-�E�X�O�P").constrain_rot_1.X = (float)((-120.0 / 180.0) * Math.PI); // �����F��]1(rad(x), rad(y), rad(z))
            phys_list.GetJointByName("�����g-�E�X�O�P").constrain_rot_2.X = (float)((20.0 / 180.0) * Math.PI); // �����F��]2(rad(x), rad(y), rad(z))
            phys_list.GetJointByName("�����g-�E�X�O�P").constrain_rot_1.Y = (float)((-0.0 / 180.0) * Math.PI); // �����F��]1(rad(x), rad(y), rad(z))
            phys_list.GetJointByName("�����g-�E�X�O�P").constrain_rot_2.Y = (float)((0.0 / 180.0) * Math.PI); // �����F��]2(rad(x), rad(y), rad(z))
            phys_list.GetJointByName("�����g-�E�X�O�P").constrain_rot_1.Z = (float)((-5.0 / 180.0) * Math.PI); // �����F��]1(rad(x), rad(y), rad(z))
            phys_list.GetJointByName("�����g-�E�X�O�P").constrain_rot_2.Z = (float)((5.0 / 180.0) * Math.PI); // �����F��]2(rad(x), rad(y), rad(z))
            phys_list.GetJointByName("�����g-�E�X�O�P").spring_rot.X = 50.0f; // �΂ˁF��](rad(x), rad(y), rad(z))
            phys_list.GetJointByName("�����g-�E�X�O�P").spring_rot.Y = 0.0f; // �΂ˁF��](rad(x), rad(y), rad(z))
            phys_list.GetJointByName("�����g-�E�X�O�P").spring_rot.Z = 0.0f; // �΂ˁF��](rad(x), rad(y), rad(z))

            phys_list.GetJointByName("�����g-���X�O�P").constrain_rot_1.X = (float)((-120.0 / 180.0) * Math.PI); // �����F��]1(rad(x), rad(y), rad(z))
            phys_list.GetJointByName("�����g-���X�O�P").constrain_rot_2.X = (float)((20.0 / 180.0) * Math.PI); // �����F��]2(rad(x), rad(y), rad(z))
            phys_list.GetJointByName("�����g-���X�O�P").constrain_rot_1.Y = (float)((-0.0 / 180.0) * Math.PI); // �����F��]1(rad(x), rad(y), rad(z))
            phys_list.GetJointByName("�����g-���X�O�P").constrain_rot_2.Y = (float)((0.0 / 180.0) * Math.PI); // �����F��]2(rad(x), rad(y), rad(z))
            phys_list.GetJointByName("�����g-���X�O�P").constrain_rot_1.Z = (float)((-5.0 / 180.0) * Math.PI); // �����F��]1(rad(x), rad(y), rad(z))
            phys_list.GetJointByName("�����g-���X�O�P").constrain_rot_2.Z = (float)((5.0 / 180.0) * Math.PI); // �����F��]2(rad(x), rad(y), rad(z))
            phys_list.GetJointByName("�����g-���X�O�P").spring_rot.X = 50.0f; // �΂ˁF��](rad(x), rad(y), rad(z))
            phys_list.GetJointByName("�����g-���X�O�P").spring_rot.Y = 0.0f; // �΂ˁF��](rad(x), rad(y), rad(z))
            phys_list.GetJointByName("�����g-���X�O�P").spring_rot.Z = 0.0f; // �΂ˁF��](rad(x), rad(y), rad(z))

            phys_list.GetJointByName("�����g-���X���P").constrain_rot_1.X = (float)((-5.0 / 180.0) * Math.PI); // �����F��]1(rad(x), rad(y), rad(z))
            phys_list.GetJointByName("�����g-���X���P").constrain_rot_2.X = (float)((5.0 / 180.0) * Math.PI); // �����F��]2(rad(x), rad(y), rad(z))
            phys_list.GetJointByName("�����g-���X���P").constrain_rot_1.Y = (float)((-0.0 / 180.0) * Math.PI); // �����F��]1(rad(x), rad(y), rad(z))
            phys_list.GetJointByName("�����g-���X���P").constrain_rot_2.Y = (float)((0.0 / 180.0) * Math.PI); // �����F��]2(rad(x), rad(y), rad(z))
            phys_list.GetJointByName("�����g-���X���P").constrain_rot_1.Z = (float)((-60.0 / 180.0) * Math.PI); // �����F��]1(rad(x), rad(y), rad(z))
            phys_list.GetJointByName("�����g-���X���P").constrain_rot_2.Z = (float)((15.0 / 180.0) * Math.PI); // �����F��]2(rad(x), rad(y), rad(z))
            phys_list.GetJointByName("�����g-���X���P").spring_rot.X = 0.0f; // �΂ˁF��](rad(x), rad(y), rad(z))
            phys_list.GetJointByName("�����g-���X���P").spring_rot.Y = 0.0f; // �΂ˁF��](rad(x), rad(y), rad(z))
            phys_list.GetJointByName("�����g-���X���P").spring_rot.Z = 50.0f; // �΂ˁF��](rad(x), rad(y), rad(z))

            phys_list.MakeJointFromTwoBones("���X�O�Q", "�E�X�O�Q");
            phys_list.MakeJointFromTwoBones("�E�X�O�Q", "�E�X���Q");
            phys_list.MakeJointFromTwoBones("�E�X���Q", "�E�X��Q");
            phys_list.MakeJointFromTwoBones("�E�X��Q", "���X��Q");
            phys_list.MakeJointFromTwoBones("���X��Q", "���X���Q");
            phys_list.MakeJointFromTwoBones("���X���Q", "���X�O�Q");
            phys_list.MakeJointFromTwoBones("���X�O�R", "�E�X�O�R");
            phys_list.MakeJointFromTwoBones("�E�X�O�R", "�E�X���R");
            phys_list.MakeJointFromTwoBones("�E�X���R", "�E�X��R");
            phys_list.MakeJointFromTwoBones("�E�X��R", "���X��R");
            phys_list.MakeJointFromTwoBones("���X��R", "���X���R");
            phys_list.MakeJointFromTwoBones("���X���R", "���X�O�R");
        }

        // �X�J�[�g�O��
        private void SetParameter(List<PMD_RigidBody> body_list)
        {
            foreach (PMD_RigidBody body in body_list)
            {
                body.rigidbody_group_index = 4; // ���f�[�^�F�O���[�v // 00
                body.rigidbody_group_target = 1; // ���f�[�^�F�O���[�v�F�Ώ� // 0xFFFF�Ƃ̍� // 38 FE
                body.shape_type = 1; // �`��F�^�C�v(0:���A1:���A2:�J�v�Z��) // 00 // ��
                body.shape_w = 0.8f; // �`��F���a(��) // CD CC CC 3F // 1.6
                body.shape_h *= 0.6f; // �`��F���� // CD CC CC 3D // 0.1
                body.shape_d = 0.2f; // �`��F���s // CD CC CC 3D // 0.1

                body.rigidbody_weight = 0.5f; // ���f�[�^�F���� // 00 00 80 3F // 1.0
                body.rigidbody_pos_dim = 0.5f; // ���f�[�^�F�ړ��� // 00 00 00 00
                body.rigidbody_rot_dim = 0.9f; // ���f�[�^�F��]�� // 00 00 00 00
                body.rigidbody_recoil = 0.0f; // ���f�[�^�F������ // 00 00 00 00
                body.rigidbody_friction = 0.0f; // ���f�[�^�F���C�� // 00 00 00 00
                body.rigidbody_type = 1; // ���f�[�^�F�^�C�v(0:Bone�Ǐ]�A1:�������Z�A2:�������Z(Bone�ʒu����)) // 00 // Bone�Ǐ]
            }
        }

        // �X�J�[�g��
        private void SetParameterSide(List<PMD_RigidBody> body_list)
        {
            foreach (PMD_RigidBody body in body_list)
            {
                body.rigidbody_group_index = 4; // ���f�[�^�F�O���[�v // 00
                body.rigidbody_group_target = 1; // ���f�[�^�F�O���[�v�F�Ώ� // 0xFFFF�Ƃ̍� // 38 FE
                body.shape_type = 1; // �`��F�^�C�v(0:���A1:���A2:�J�v�Z��) // 00 // ��
                body.shape_w = 0.2f; // �`��F���a(��) // CD CC CC 3F // 1.6
                body.shape_h *= 0.6f; // �`��F���� // CD CC CC 3D // 0.1
                body.shape_d = 0.8f; // �`��F���s // CD CC CC 3D // 0.1

                body.rigidbody_weight = 0.5f; // ���f�[�^�F���� // 00 00 80 3F // 1.0
                body.rigidbody_pos_dim = 0.5f; // ���f�[�^�F�ړ��� // 00 00 00 00
                body.rigidbody_rot_dim = 0.9f; // ���f�[�^�F��]�� // 00 00 00 00
                body.rigidbody_recoil = 0.0f; // ���f�[�^�F������ // 00 00 00 00
                body.rigidbody_friction = 0.0f; // ���f�[�^�F���C�� // 00 00 00 00
                body.rigidbody_type = 1; // ���f�[�^�F�^�C�v(0:Bone�Ǐ]�A1:�������Z�A2:�������Z(Bone�ʒu����)) // 00 // Bone�Ǐ]
            }
        }

        // �X�J�[�g�O���
        private void SetParameterEnd(List<PMD_RigidBody> body_list)
        {
            foreach (PMD_RigidBody body in body_list)
            {
                body.rigidbody_group_index = 4; // ���f�[�^�F�O���[�v // 00
                body.rigidbody_group_target = 1; // ���f�[�^�F�O���[�v�F�Ώ� // 0xFFFF�Ƃ̍� // 38 FE
                body.shape_type = 0; // �`��F�^�C�v(0:���A1:���A2:�J�v�Z��) // 00 // ��
                body.shape_w = 0.2f; // �`��F���a(��) // CD CC CC 3F // 1.6

                body.rigidbody_weight = 0.5f; // ���f�[�^�F���� // 00 00 80 3F // 1.0
                body.rigidbody_pos_dim = 0.5f; // ���f�[�^�F�ړ��� // 00 00 00 00
                body.rigidbody_rot_dim = 0.9f; // ���f�[�^�F��]�� // 00 00 00 00
                body.rigidbody_recoil = 0.0f; // ���f�[�^�F������ // 00 00 00 00
                body.rigidbody_friction = 0.0f; // ���f�[�^�F���C�� // 00 00 00 00
                body.rigidbody_type = 1; // ���f�[�^�F�^�C�v(0:Bone�Ǐ]�A1:�������Z�A2:�������Z(Bone�ʒu����)) // 00 // Bone�Ǐ]
            }
        }

        // �X�J�[�g����
        private void SetParameterSideEnd(List<PMD_RigidBody> body_list)
        {
            foreach (PMD_RigidBody body in body_list)
            {
                body.rigidbody_group_index = 4; // ���f�[�^�F�O���[�v // 00
                body.rigidbody_group_target = 1; // ���f�[�^�F�O���[�v�F�Ώ� // 0xFFFF�Ƃ̍� // 38 FE
                body.shape_type = 0; // �`��F�^�C�v(0:���A1:���A2:�J�v�Z��) // 00 // ��
                body.shape_w = 0.2f; // �`��F���a(��) // CD CC CC 3F // 1.6

                body.rigidbody_weight = 0.5f; // ���f�[�^�F���� // 00 00 80 3F // 1.0
                body.rigidbody_pos_dim = 0.5f; // ���f�[�^�F�ړ��� // 00 00 00 00
                body.rigidbody_rot_dim = 0.9f; // ���f�[�^�F��]�� // 00 00 00 00
                body.rigidbody_recoil = 0.0f; // ���f�[�^�F������ // 00 00 00 00
                body.rigidbody_friction = 0.0f; // ���f�[�^�F���C�� // 00 00 00 00
                body.rigidbody_type = 1; // ���f�[�^�F�^�C�v(0:Bone�Ǐ]�A1:�������Z�A2:�������Z(Bone�ʒu����)) // 00 // Bone�Ǐ]
            }
        }

        private void SetParameter(List<PMD_Joint> joint_list)
        {
            foreach (PMD_Joint joint in joint_list)
            {
                joint.constrain_rot_1.X = (float)((-15.0 / 180.0) * Math.PI); // �����F��]1(rad(x), rad(y), rad(z))
                joint.constrain_rot_2.X = (float)((15.0 / 180.0) * Math.PI); // �����F��]2(rad(x), rad(y), rad(z))
                joint.constrain_rot_1.Z = (float)((-15.0 / 180.0) * Math.PI); // �����F��]1(rad(x), rad(y), rad(z))
                joint.constrain_rot_2.Z = (float)((15.0 / 180.0) * Math.PI); // �����F��]2(rad(x), rad(y), rad(z))
                joint.spring_rot.X = 20.0f; // �΂ˁF��](rad(x), rad(y), rad(z))
                joint.spring_rot.Y = 20.0f; // �΂ˁF��](rad(x), rad(y), rad(z))
                joint.spring_rot.Z = 20.0f; // �΂ˁF��](rad(x), rad(y), rad(z))
            }
        }
    }
}
