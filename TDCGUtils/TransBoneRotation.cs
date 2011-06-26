using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

using TDCG;

namespace TDCGUtils
{
    public class TransBoneRotation
    {
        CorrespondBoneForTmo2Vpd tmo2vpd = new CorrespondBoneForTmo2Vpd("girl2miku");

        TMOFile initPose;
        TMOFile pmd_initPose_diff;
        TMOFile pmd_initPose_diff_w;

        public void SetInitPose(Figure fig)
        {
            fig.UpdateBoneMatrices(true);

            // カス子の初期ポーズを記憶
            initPose = fig.Tmo.Dup();
            initPose.LoadTransformationMatrixFromFrame(0);

            // MMDの初期ポーズを記憶
            fig.TPOList["TDCG.Proportion.AAA_PMDInitPose"].Ratio = 1.0f;
            fig.TransformTpo();
            fig.UpdateBoneMatrices(true);

            TMOFile pmd_initPose = fig.Tmo.Dup();
            pmd_initPose.LoadTransformationMatrixFromFrame(0);
            pmd_initPose_diff = DiffTmo(pmd_initPose, initPose);
            pmd_initPose_diff_w = DiffWorldTmo(pmd_initPose, initPose);
          
            // カス子の初期ポーズに戻しておく
            fig.TPOList["TDCG.Proportion.AAA_PMDInitPose"].Ratio = 0.0f;
            fig.TransformTpo();
            fig.UpdateBoneMatrices(true);
        }

        private TMOFile DiffWorldTmo(TMOFile tmo1, TMOFile tmo2)
        {
            TMOFile output = tmo1.Dup();

            foreach (TMONode node in tmo1.nodes)
                output.FindNodeByName(node.Name).Rotation
                    = Quaternion.Invert(tmo2.FindNodeByName(node.Name).GetWorldRotation()) *
                        node.GetWorldRotation();

            return output;
        }
        private TMOFile DiffTmo(TMOFile tmo1, TMOFile tmo2)
        {
            return MultiTmo(InvertTmo(tmo2),tmo1);
        }
        private TMOFile UnitaryTmo(TMOFile tmo1, TMOFile tmo2)
        {
            return MultiTmo(MultiTmo(InvertTmo(tmo2), tmo1), tmo2);
        }
        private TMOFile MultiTmo(TMOFile tmo1, TMOFile tmo2)
        {
            TMOFile output = tmo1.Dup();

            foreach (TMONode node in tmo1.nodes)
                output.FindNodeByName(node.Name).Rotation
                    = node.Rotation *
                        tmo2.FindNodeByName(node.Name).Rotation;

            return output;
        }
        private TMOFile InvertTmo(TMOFile tmo)
        {
            TMOFile output = tmo.Dup();

            foreach (TMONode node in tmo.nodes)
                output.FindNodeByName(node.Name).Rotation
                    = Quaternion.Invert(node.Rotation);

            return output;
        }

        public Quaternion MmdRotationToCustomRotation(Quaternion q, string cus_bone_name)
        {
            return
                initPose.FindNodeByName(cus_bone_name).Rotation *
                pmd_initPose_diff_w.FindNodeByName(cus_bone_name).Rotation *
                q *
                pmd_initPose_diff.FindNodeByName(cus_bone_name).Rotation *
                Quaternion.Invert(pmd_initPose_diff_w.FindNodeByName(cus_bone_name).Rotation);
        }
 
        public void SaveVpd(TMOFile pose, string file_name)
        {
            TMOFile pose_diff = DiffTmo(pose, initPose);
            TMOFile output =
                MultiTmo(
                    UnitaryTmo(pose_diff, pmd_initPose_diff_w),
                    InvertTmo(pmd_initPose_diff)
                );
            TMOFile output2 = UnitaryTmo(pose_diff, pmd_initPose_diff_w);

            // ファイルを上書きし、Shift JISで書き込む
            System.IO.StreamWriter sw = new System.IO.StreamWriter(
                file_name,
                false,
                System.Text.Encoding.GetEncoding("shift_jis"));

            // ヘッダ
            sw.WriteLine(@"Vocaloid Pose Data file");
            sw.WriteLine(@"");
            sw.WriteLine(@"miku.osm;");
            sw.WriteLine((tmo2vpd.CorrespondBone.Count).ToString() + @";");

            // ボーンの回転
            int i = 0;
            foreach (KeyValuePair<string, List<string>> kvp in tmo2vpd.CorrespondBone)
            {
                sw.WriteLine(@"");
                sw.WriteLine(@"Bone" + i.ToString() + "{" + kvp.Key); i++;
                sw.WriteLine(@"0.000000,0.000000,0.000000;");

                Quaternion q;

                if (kvp.Key == "センター")
                {
                    q = output.FindNodeByName("W_Hips").Rotation *
                        output.FindNodeByName("W_Spine_Dummy").Rotation;
                }
                else if (kvp.Key == "下半身")
                {
                    q = Quaternion.Invert(output.FindNodeByName("W_Spine_Dummy").Rotation);
                }
                else if (kvp.Key == "頭")
                {
                    Quaternion face_q = output.FindNodeByName("face_oya").Rotation;

                    q = output.FindNodeByName("Head").Rotation *
                        new Quaternion(-face_q.X, face_q.Y, -face_q.Z, face_q.W);
                }
                else if (kvp.Key.IndexOf("捩") >= 0)
                {
                    q = Quaternion.Identity;

                    foreach (string bone_name in kvp.Value)
                        q = q * output2.FindNodeByName(bone_name).Rotation;
                }
                else
                {
                    q = Quaternion.Identity;

                    foreach (string bone_name in kvp.Value)
                        q = q * output.FindNodeByName(bone_name).Rotation;
                }

                sw.WriteLine(
                    (q.X * (-1)).ToString() + "," +
                    (q.Y * (-1)).ToString() + "," +
                    q.Z.ToString() + "," +
                    q.W.ToString() + ";");

                sw.WriteLine(@"}");
            }

            // ファイルを閉じる
            sw.Close();
        }
    }
}
