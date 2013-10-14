//css_reference Microsoft.DirectX.Direct3DX;

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

            phys_list.GetJointByName("�����g-���X��P").rotation_min.X = (float)((-0.0 / 180.0) * Math.PI); // �����F��]1(rad(x), rad(y), rad(z))
            phys_list.GetJointByName("�����g-���X��P").rotation_max.X = (float)((60.0 / 180.0) * Math.PI); // �����F��]2(rad(x), rad(y), rad(z))
            phys_list.GetJointByName("�����g-���X��P").rotation_min.Y = (float)((-0.0 / 180.0) * Math.PI); // �����F��]1(rad(x), rad(y), rad(z))
            phys_list.GetJointByName("�����g-���X��P").rotation_max.Y = (float)((0.0 / 180.0) * Math.PI); // �����F��]2(rad(x), rad(y), rad(z))
            phys_list.GetJointByName("�����g-���X��P").rotation_min.Z = (float)((-5.0 / 180.0) * Math.PI); // �����F��]1(rad(x), rad(y), rad(z))
            phys_list.GetJointByName("�����g-���X��P").rotation_max.Z = (float)((5.0 / 180.0) * Math.PI); // �����F��]2(rad(x), rad(y), rad(z))
            phys_list.GetJointByName("�����g-���X��P").spring_rotation = new Vector3(50.0f, 0.0f, 0.0f);

            phys_list.GetJointByName("�����g-�E�X��P").rotation_min.X = (float)((-0.0 / 180.0) * Math.PI); // �����F��]1(rad(x), rad(y), rad(z))
            phys_list.GetJointByName("�����g-�E�X��P").rotation_max.X = (float)((60.0 / 180.0) * Math.PI); // �����F��]2(rad(x), rad(y), rad(z))
            phys_list.GetJointByName("�����g-�E�X��P").rotation_min.Y = (float)((-0.0 / 180.0) * Math.PI); // �����F��]1(rad(x), rad(y), rad(z))
            phys_list.GetJointByName("�����g-�E�X��P").rotation_max.Y = (float)((0.0 / 180.0) * Math.PI); // �����F��]2(rad(x), rad(y), rad(z))
            phys_list.GetJointByName("�����g-�E�X��P").rotation_min.Z = (float)((-5.0 / 180.0) * Math.PI); // �����F��]1(rad(x), rad(y), rad(z))
            phys_list.GetJointByName("�����g-�E�X��P").rotation_max.Z = (float)((5.0 / 180.0) * Math.PI); // �����F��]2(rad(x), rad(y), rad(z))
            phys_list.GetJointByName("�����g-�E�X��P").spring_rotation = new Vector3(50.0f, 0.0f, 0.0f);

            phys_list.GetJointByName("�����g-�E�X���P").rotation_min.X = (float)((-5.0 / 180.0) * Math.PI); // �����F��]1(rad(x), rad(y), rad(z))
            phys_list.GetJointByName("�����g-�E�X���P").rotation_max.X = (float)((5.0 / 180.0) * Math.PI); // �����F��]2(rad(x), rad(y), rad(z))
            phys_list.GetJointByName("�����g-�E�X���P").rotation_min.Y = (float)((-0.0 / 180.0) * Math.PI); // �����F��]1(rad(x), rad(y), rad(z))
            phys_list.GetJointByName("�����g-�E�X���P").rotation_max.Y = (float)((0.0 / 180.0) * Math.PI); // �����F��]2(rad(x), rad(y), rad(z))
            phys_list.GetJointByName("�����g-�E�X���P").rotation_min.Z = (float)((-15.0 / 180.0) * Math.PI); // �����F��]1(rad(x), rad(y), rad(z))
            phys_list.GetJointByName("�����g-�E�X���P").rotation_max.Z = (float)((60.0 / 180.0) * Math.PI); // �����F��]2(rad(x), rad(y), rad(z))
            phys_list.GetJointByName("�����g-�E�X���P").spring_rotation = new Vector3(0.0f, 0.0f, 50.0f);

            phys_list.GetJointByName("�����g-�E�X�O�P").rotation_min.X = (float)((-120.0 / 180.0) * Math.PI); // �����F��]1(rad(x), rad(y), rad(z))
            phys_list.GetJointByName("�����g-�E�X�O�P").rotation_max.X = (float)((20.0 / 180.0) * Math.PI); // �����F��]2(rad(x), rad(y), rad(z))
            phys_list.GetJointByName("�����g-�E�X�O�P").rotation_min.Y = (float)((-0.0 / 180.0) * Math.PI); // �����F��]1(rad(x), rad(y), rad(z))
            phys_list.GetJointByName("�����g-�E�X�O�P").rotation_max.Y = (float)((0.0 / 180.0) * Math.PI); // �����F��]2(rad(x), rad(y), rad(z))
            phys_list.GetJointByName("�����g-�E�X�O�P").rotation_min.Z = (float)((-5.0 / 180.0) * Math.PI); // �����F��]1(rad(x), rad(y), rad(z))
            phys_list.GetJointByName("�����g-�E�X�O�P").rotation_max.Z = (float)((5.0 / 180.0) * Math.PI); // �����F��]2(rad(x), rad(y), rad(z))
            phys_list.GetJointByName("�����g-�E�X�O�P").spring_rotation = new Vector3(50.0f, 0.0f, 0.0f);
            
            phys_list.GetJointByName("�����g-���X�O�P").rotation_min.X = (float)((-120.0 / 180.0) * Math.PI); // �����F��]1(rad(x), rad(y), rad(z))
            phys_list.GetJointByName("�����g-���X�O�P").rotation_max.X = (float)((20.0 / 180.0) * Math.PI); // �����F��]2(rad(x), rad(y), rad(z))
            phys_list.GetJointByName("�����g-���X�O�P").rotation_min.Y = (float)((-0.0 / 180.0) * Math.PI); // �����F��]1(rad(x), rad(y), rad(z))
            phys_list.GetJointByName("�����g-���X�O�P").rotation_max.Y = (float)((0.0 / 180.0) * Math.PI); // �����F��]2(rad(x), rad(y), rad(z))
            phys_list.GetJointByName("�����g-���X�O�P").rotation_min.Z = (float)((-5.0 / 180.0) * Math.PI); // �����F��]1(rad(x), rad(y), rad(z))
            phys_list.GetJointByName("�����g-���X�O�P").rotation_max.Z = (float)((5.0 / 180.0) * Math.PI); // �����F��]2(rad(x), rad(y), rad(z))
            phys_list.GetJointByName("�����g-���X�O�P").spring_rotation = new Vector3(50.0f, 0.0f, 0.0f);

            phys_list.GetJointByName("�����g-���X���P").rotation_min.X = (float)((-5.0 / 180.0) * Math.PI); // �����F��]1(rad(x), rad(y), rad(z))
            phys_list.GetJointByName("�����g-���X���P").rotation_max.X = (float)((5.0 / 180.0) * Math.PI); // �����F��]2(rad(x), rad(y), rad(z))
            phys_list.GetJointByName("�����g-���X���P").rotation_min.Y = (float)((-0.0 / 180.0) * Math.PI); // �����F��]1(rad(x), rad(y), rad(z))
            phys_list.GetJointByName("�����g-���X���P").rotation_max.Y = (float)((0.0 / 180.0) * Math.PI); // �����F��]2(rad(x), rad(y), rad(z))
            phys_list.GetJointByName("�����g-���X���P").rotation_min.Z = (float)((-60.0 / 180.0) * Math.PI); // �����F��]1(rad(x), rad(y), rad(z))
            phys_list.GetJointByName("�����g-���X���P").rotation_max.Z = (float)((15.0 / 180.0) * Math.PI); // �����F��]2(rad(x), rad(y), rad(z))
            phys_list.GetJointByName("�����g-���X���P").spring_rotation = new Vector3(0.0f, 0.0f, 50.0f);

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
        private void SetParameter(List<PMD_RBody> body_list)
        {
            foreach (PMD_RBody body in body_list)
            {
                body.group_id = 4; // ���f�[�^�F�O���[�v // 00
                body.group_non_collision = 1; // ���f�[�^�F�O���[�v�F�Ώ� // 0xFFFF�Ƃ̍� // 38 FE
                body.shape_id = 1; // �`��F�^�C�v(0:���A1:���A2:�J�v�Z��) // 00 // ��
                body.size.X = 0.8f; // �`��F���a(��) // CD CC CC 3F // 1.6
                body.size.Y *= 0.6f; // �`��F���� // CD CC CC 3D // 0.1
                body.size.Z = 0.2f; // �`��F���s // CD CC CC 3D // 0.1

                body.weight = 0.5f; // ���f�[�^�F���� // 00 00 80 3F // 1.0
                body.position_dim = 0.5f; // ���f�[�^�F�ړ��� // 00 00 00 00
                body.rotation_dim = 0.9f; // ���f�[�^�F��]�� // 00 00 00 00
                body.recoil = 0.0f; // ���f�[�^�F������ // 00 00 00 00
                body.friction = 0.0f; // ���f�[�^�F���C�� // 00 00 00 00
                body.type = 1; // ���f�[�^�F�^�C�v(0:Bone�Ǐ]�A1:�������Z�A2:�������Z(Bone�ʒu����)) // 00 // Bone�Ǐ]
            }
        }

        // �X�J�[�g��
        private void SetParameterSide(List<PMD_RBody> body_list)
        {
            foreach (PMD_RBody body in body_list)
            {
                body.group_id = 4; // ���f�[�^�F�O���[�v // 00
                body.group_non_collision = 1; // ���f�[�^�F�O���[�v�F�Ώ� // 0xFFFF�Ƃ̍� // 38 FE
                body.shape_id = 1; // �`��F�^�C�v(0:���A1:���A2:�J�v�Z��) // 00 // ��
                body.size.X = 0.2f; // �`��F���a(��) // CD CC CC 3F // 1.6
                body.size.Y *= 0.6f; // �`��F���� // CD CC CC 3D // 0.1
                body.size.Z = 0.8f; // �`��F���s // CD CC CC 3D // 0.1

                body.weight = 0.5f; // ���f�[�^�F���� // 00 00 80 3F // 1.0
                body.position_dim = 0.5f; // ���f�[�^�F�ړ��� // 00 00 00 00
                body.rotation_dim = 0.9f; // ���f�[�^�F��]�� // 00 00 00 00
                body.recoil = 0.0f; // ���f�[�^�F������ // 00 00 00 00
                body.friction = 0.0f; // ���f�[�^�F���C�� // 00 00 00 00
                body.type = 1; // ���f�[�^�F�^�C�v(0:Bone�Ǐ]�A1:�������Z�A2:�������Z(Bone�ʒu����)) // 00 // Bone�Ǐ]
            }
        }

        // �X�J�[�g�O���
        private void SetParameterEnd(List<PMD_RBody> body_list)
        {
            foreach (PMD_RBody body in body_list)
            {
                body.group_id = 4; // ���f�[�^�F�O���[�v // 00
                body.group_non_collision = 1; // ���f�[�^�F�O���[�v�F�Ώ� // 0xFFFF�Ƃ̍� // 38 FE
                body.shape_id = 0; // �`��F�^�C�v(0:���A1:���A2:�J�v�Z��) // 00 // ��
                body.size.X = 0.2f; // �`��F���a(��) // CD CC CC 3F // 1.6

                body.weight = 0.5f; // ���f�[�^�F���� // 00 00 80 3F // 1.0
                body.position_dim = 0.5f; // ���f�[�^�F�ړ��� // 00 00 00 00
                body.rotation_dim = 0.9f; // ���f�[�^�F��]�� // 00 00 00 00
                body.recoil = 0.0f; // ���f�[�^�F������ // 00 00 00 00
                body.friction = 0.0f; // ���f�[�^�F���C�� // 00 00 00 00
                body.type = 1; // ���f�[�^�F�^�C�v(0:Bone�Ǐ]�A1:�������Z�A2:�������Z(Bone�ʒu����)) // 00 // Bone�Ǐ]
            }
        }

        // �X�J�[�g����
        private void SetParameterSideEnd(List<PMD_RBody> body_list)
        {
            foreach (PMD_RBody body in body_list)
            {
                body.group_id = 4; // ���f�[�^�F�O���[�v // 00
                body.group_non_collision = 1; // ���f�[�^�F�O���[�v�F�Ώ� // 0xFFFF�Ƃ̍� // 38 FE
                body.shape_id = 0; // �`��F�^�C�v(0:���A1:���A2:�J�v�Z��) // 00 // ��
                body.size.X = 0.2f; // �`��F���a(��) // CD CC CC 3F // 1.6

                body.weight = 0.5f; // ���f�[�^�F���� // 00 00 80 3F // 1.0
                body.position_dim = 0.5f; // ���f�[�^�F�ړ��� // 00 00 00 00
                body.rotation_dim = 0.9f; // ���f�[�^�F��]�� // 00 00 00 00
                body.recoil = 0.0f; // ���f�[�^�F������ // 00 00 00 00
                body.friction = 0.0f; // ���f�[�^�F���C�� // 00 00 00 00
                body.type = 1; // ���f�[�^�F�^�C�v(0:Bone�Ǐ]�A1:�������Z�A2:�������Z(Bone�ʒu����)) // 00 // Bone�Ǐ]
            }
        }

        private void SetParameter(List<PMD_Joint> joint_list)
        {
            foreach (PMD_Joint joint in joint_list)
            {
                joint.rotation_min.X = Geometry.DegreeToRadian(-15.0f);
                joint.rotation_max.X = Geometry.DegreeToRadian(+15.0f);
                joint.rotation_min.Z = Geometry.DegreeToRadian(-15.0f);
                joint.rotation_max.Z = Geometry.DegreeToRadian(+15.0f);
                joint.spring_rotation = new Vector3(20.0f, 20.0f, 20.0f);
            }
        }
    }
}
