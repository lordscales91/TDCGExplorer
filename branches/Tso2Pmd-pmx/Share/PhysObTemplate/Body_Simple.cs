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
            {
                PMD_RBody body = phys_list.GetBodyByName("�㔼�g");
                body.shape_id = 1;
                body.size.X = 1.0f;
                body.size.Y *= 0.5f;
                body.size.Z = 0.5f;
            }

            phys_list.MakeBodyFromBone("�㔼�g�Q");
            {
                PMD_RBody body = phys_list.GetBodyByName("�㔼�g�Q");
                body.shape_id = 1;
                body.size.X = 1.0f;
                body.size.Y *= 0.5f;
                body.size.Z = 0.5f;
            }

            phys_list.MakeBodyFromBone("�㔼�g�R");
            {
                PMD_RBody body = phys_list.GetBodyByName("�㔼�g�R");
                body.shape_id = 1;
                body.size.X = 1.0f;
                body.size.Y *= 0.5f;
                body.size.Z = 0.5f;
            }

            phys_list.MakeBodyFromBone("��");

            phys_list.MakeBodyFromBone("����");

            phys_list.MakeBodyFromBone("���r");

            phys_list.MakeBodyFromBone("���Ђ�");

            phys_list.MakeBodyFromBone("�E��");

            phys_list.MakeBodyFromBone("�E�r");

            phys_list.MakeBodyFromBone("�E�Ђ�");

            phys_list.MakeBodyFromBone("����");
            phys_list.GetBodyByName("����").size.X = 0.75f;
            
            phys_list.MakeBodyFromBone("���Ђ�");

            phys_list.MakeBodyFromBone("�E��");
            phys_list.GetBodyByName("�E��").size.X = 0.75f;
            
            phys_list.MakeBodyFromBone("�E�Ђ�");
        }
    }
}
