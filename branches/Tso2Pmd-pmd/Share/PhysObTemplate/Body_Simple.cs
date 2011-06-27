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
    public class Body_Simple : IPhysObTemplate
    {
        string name = "�g��";   // �g�ɕ\������閼�O
        int group = 3;          // �\�������g�̎�ށi0:�� 1:�� 2:�X�J�[�g 3:���̑��i�����`�F�b�N����j4:���̑��i�����`�F�b�N�Ȃ��j�j

        public string Name() { return name; }
        public int Group() { return group; }

        public void Execute(ref T2PPhysObjectList phys_list)
        {
            phys_list.MakeBodyFromBone("�����g");

            phys_list.MakeBodyFromBone("�㔼�g");
            phys_list.GetBodyByName("�㔼�g").shape_type = 1; // �`��F�^�C�v(0:���A1:���A2:�J�v�Z��) // 00 // ��
            phys_list.GetBodyByName("�㔼�g").shape_w = 1.0f; // �`��F���a(��) // CD CC CC 3F // 1.6
            phys_list.GetBodyByName("�㔼�g").shape_h *= 0.5f;
            phys_list.GetBodyByName("�㔼�g").shape_d = 0.5f; // �`��F���s // CD CC CC 3D // 0.1

            phys_list.MakeBodyFromBone("�㔼�g�Q");
            phys_list.GetBodyByName("�㔼�g�Q").shape_type = 1; // �`��F�^�C�v(0:���A1:���A2:�J�v�Z��) // 00 // ��
            phys_list.GetBodyByName("�㔼�g�Q").shape_w = 1.0f; // �`��F���a(��) // CD CC CC 3F // 1.6
            phys_list.GetBodyByName("�㔼�g�Q").shape_h *= 0.5f;
            phys_list.GetBodyByName("�㔼�g�Q").shape_d = 0.5f; // �`��F���s // CD CC CC 3D // 0.1

            phys_list.MakeBodyFromBone("�㔼�g�R");
            phys_list.GetBodyByName("�㔼�g�R").shape_type = 1; // �`��F�^�C�v(0:���A1:���A2:�J�v�Z��) // 00 // ��
            phys_list.GetBodyByName("�㔼�g�R").shape_w = 1.0f; // �`��F���a(��) // CD CC CC 3F // 1.6
            phys_list.GetBodyByName("�㔼�g�R").shape_h *= 0.5f;
            phys_list.GetBodyByName("�㔼�g�R").shape_d = 0.5f; // �`��F���s // CD CC CC 3D // 0.1

            phys_list.MakeBodyFromBone("��");

            phys_list.MakeBodyFromBone("����");

            phys_list.MakeBodyFromBone("���r");

            phys_list.MakeBodyFromBone("���Ђ�");

            phys_list.MakeBodyFromBone("�E��");

            phys_list.MakeBodyFromBone("�E�r");

            phys_list.MakeBodyFromBone("�E�Ђ�");

            phys_list.MakeBodyFromBone("����");
            phys_list.GetBodyByName("����").shape_w = 0.75f; // �`��F���a(��) // CD CC CC 3F // 1.6

            phys_list.MakeBodyFromBone("���Ђ�");

            phys_list.MakeBodyFromBone("�E��");
            phys_list.GetBodyByName("�E��").shape_w = 0.75f; // �`��F���a(��) // CD CC CC 3F // 1.6

            phys_list.MakeBodyFromBone("�E�Ђ�");
        }
    }
}
