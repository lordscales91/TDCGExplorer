using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

using TDCG;
using TDCGUtils;

using NyMmdUtils;
using jp.nyatla.nymmd.cs;
using jp.nyatla.nymmd.cs.core;
using jp.nyatla.nymmd.cs.types;

namespace Vmd2Tmo
{
    public class Vmd2Tmo
    {
        private MmdPmdModel pmd;
        private MmdVmdMotion vmd;
        private MmdMotionPlayer player;

        private Vector3 offset_position;
        public Vector3 Offset_position { set { offset_position = value; } }

        public MmdVmdMotion Vmd { get { return vmd; } }

        // -----------------------------------------------------
        // 変換表
        // -----------------------------------------------------
        CorrespondTable girl2miku = new CorrespondTable("girl2miku");
        //CorrespondTable man2miku = new CorrespondTable("man2miku");

        // -----------------------------------------------------
        // クォータニオンの変換処理
        // -----------------------------------------------------
        TransBoneRotation tbr = new TransBoneRotation();

        // -----------------------------------------------------
        // 表情設定リスト
        // -----------------------------------------------------
        Morphing morph = new Morphing();

        // 初期化
        public void initialize(string pmd_FileName, string vmd_FileName)
        {
            // PMDファイルを開く
            StreamReader pmds = new StreamReader(pmd_FileName);
            pmd = new MmdPmdModel(pmds);

            // VMDファイルを開く
            StreamReader vmds = new StreamReader(vmd_FileName);
            vmd = new MmdVmdMotion(vmds);

            //Mmdプレイヤーのオブジェクトを作る
            player = new MmdMotionPlayer(this.pmd, this.vmd);
    
            player.setLoop(false); // ループさせるフラグ


            // -----------------------------------------------------
            // 表情ファイルを読みとる
            morph.Load(Application.StartupPath + @"/表情");

        }

        // 指定時間だけ進める
        public void FrameMove(float fDiffTime)
        {
            player.updateMotion(fDiffTime);
        }

        public void SetInitPose(Figure fig)
        {
            tbr.SetInitPose(fig);
        }

        // MmdVmdMotionより、現時刻でのTmo情報を得る
        public void GetRotation(ref TMOFile tmo)
        {
            // -----------------------------------------------------
            // 表情の移植
            // -----------------------------------------------------

            foreach (MorphGroup mg in morph.Groups)
                foreach (Morph mi in mg.Items)
                    mi.Ratio = player.GetFaceRateByName(mi.Name);

            morph.Morph(tmo); // モーフ変形を実行


            // -----------------------------------------------------
            // ボーンのRotation情報を移植
            // -----------------------------------------------------

            PmdBone bone;

            // ミクとカス子ではセンターボーンが異なるので、調整が必要
            bone = pmd.getBoneByName("センター");
            Quaternion rot_center = tbr.MmdRotationToCustomRotation(
                new Quaternion(
                    (float)bone.m_vec4Rotate.x * (-1),
                    (float)bone.m_vec4Rotate.y * (-1),
                    (float)bone.m_vec4Rotate.z,
                    (float)bone.m_vec4Rotate.w
                ),
                "W_Hips");
            bone = pmd.getBoneByName("下半身");
            Quaternion rot_down = tbr.MmdRotationToCustomRotation(
                new Quaternion(
                    (float)bone.m_vec4Rotate.x * (-1),
                    (float)bone.m_vec4Rotate.y * (-1),
                    (float)bone.m_vec4Rotate.z,
                    (float)bone.m_vec4Rotate.w
                ),
                "W_Spine_Dummy");

            // センターの回転
            tmo.FindNodeByName("W_Hips").Rotation = rot_down * rot_center;
            
            // 下半身の回転
            tmo.FindNodeByName("W_Spine_Dummy").Rotation = Quaternion.Invert(rot_down);

            // センターの位置
            bone = pmd.getBoneByName("センター");
            Vector3 pos_center = new Vector3(
                    bone.m_vec3Position.x,
                    bone.m_vec3Position.y,
                    -bone.m_vec3Position.z
                );
            Vector3 d = tmo.FindNodeByName("W_Spine1").GetWorldPosition()
                - tmo.FindNodeByName("W_Hips").GetWorldPosition();
            tmo.FindNodeByName("W_Hips").Translation = pos_center + offset_position - d;

            // その他のボーンの回転
            foreach (KeyValuePair<string, string> kvp in girl2miku.boneCorrespond_v2t)
            {
                if (kvp.Key != "センター" && kvp.Key != "下半身")
                {
                    bone = pmd.getBoneByName(kvp.Key);

                    Quaternion q
                        = new Quaternion(
                            (float)bone.m_vec4Rotate.x * (-1),
                            (float)bone.m_vec4Rotate.y * (-1),
                            (float)bone.m_vec4Rotate.z,
                            (float)bone.m_vec4Rotate.w
                        );

                    tmo.FindNodeByName(kvp.Value).Rotation =
                        tbr.MmdRotationToCustomRotation(q, kvp.Value);
                }
            }
        }
    }
}
