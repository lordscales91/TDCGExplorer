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
    public class Chichi_Default : IPhysObTemplate
    {
        string name = "�f�t�H���g";   // �g�ɕ\������閼�O
        int group = 1;          // �\�������g�̎�ށi0:�� 1:�� 2:�X�J�[�g 3:���̑��j

        public string Name() { return name; }
        public int Group() { return group; }

        public void Execute(ref T2PPhysObjectList phys_list)
        {
            phys_list.MakeChain("�E���P");
            phys_list.MakeChain("�����P");
        
            SetParameter(phys_list.GetBodyListByName(".��."));
            SetParameterEnd(phys_list.GetBodyListByName(".����"));
            SetParameter(phys_list.GetJointListByName(".��."));
            phys_list.GetBodyByName("�E���P").type = 0; // ���f�[�^�F�^�C�v(0:Bone�Ǐ]�A1:�������Z�A2:�������Z(Bone�ʒu����)) // 00 // Bone�Ǐ]
            phys_list.GetBodyByName("�����P").type = 0; // ���f�[�^�F�^�C�v(0:Bone�Ǐ]�A1:�������Z�A2:�������Z(Bone�ʒu����)) // 00 // Bone�Ǐ]
        }

        private void SetParameter(List<PMD_RBody> body_list)
        {
            foreach (PMD_RBody body in body_list)
            {
                body.group_id = 3; // ���f�[�^�F�O���[�v // 00
                body.group_non_collision = 1; // ���f�[�^�F�O���[�v�F�Ώ� // 0xFFFF�Ƃ̍� // 38 FE
                body.shape_id = 2; // �`��F�^�C�v(0:���A1:���A2:�J�v�Z��) // 00 // ��
                body.size.X = 0.2f; // �`��F���a(��) // CD CC CC 3F // 1.6

                body.weight = 0.1f; // ���f�[�^�F���� // 00 00 80 3F // 1.0
                body.position_dim = 0.5f; // ���f�[�^�F�ړ��� // 00 00 00 00
                body.rotation_dim = 0.5f; // ���f�[�^�F��]�� // 00 00 00 00
                body.recoil = 0.0f; // ���f�[�^�F������ // 00 00 00 00
                body.friction = 0.0f; // ���f�[�^�F���C�� // 00 00 00 00
                body.type = 1; // ���f�[�^�F�^�C�v(0:Bone�Ǐ]�A1:�������Z�A2:�������Z(Bone�ʒu����)) // 00 // Bone�Ǐ]
            }
        }

        private void SetParameterEnd(List<PMD_RBody> body_list)
        {
            foreach (PMD_RBody body in body_list)
            {
                body.group_id = 3; // ���f�[�^�F�O���[�v // 00
                body.group_non_collision = 1; // ���f�[�^�F�O���[�v�F�Ώ� // 0xFFFF�Ƃ̍� // 38 FE
                body.shape_id = 0; // �`��F�^�C�v(0:���A1:���A2:�J�v�Z��) // 00 // ��
                body.size.X = 0.2f; // �`��F���a(��) // CD CC CC 3F // 1.6

                body.weight = 0.01f; // ���f�[�^�F���� // 00 00 80 3F // 1.0
                body.position_dim = 0.5f; // ���f�[�^�F�ړ��� // 00 00 00 00
                body.rotation_dim = 0.5f; // ���f�[�^�F��]�� // 00 00 00 00
                body.recoil = 0.0f; // ���f�[�^�F������ // 00 00 00 00
                body.friction = 0.0f; // ���f�[�^�F���C�� // 00 00 00 00
                body.type = 1; // ���f�[�^�F�^�C�v(0:Bone�Ǐ]�A1:�������Z�A2:�������Z(Bone�ʒu����)) // 00 // Bone�Ǐ]
            }
        }

        private void SetParameter(List<PMD_Joint> joint_list)
        {
            foreach (PMD_Joint joint in joint_list)
            {
                joint.spring_rotation = new Vector3(200.0f, 200.0f, 200.0f);
            }
        }
    }
}