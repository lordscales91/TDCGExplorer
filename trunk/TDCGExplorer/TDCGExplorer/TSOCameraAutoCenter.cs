using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using TDCG;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using Direct3D = Microsoft.DirectX.Direct3D;

namespace TDCGExplorer
{
    public class TSOCameraAutoCenter
    {
        Viewer viewer;

        public TSOCameraAutoCenter(Viewer TsoView)
        {
            viewer = TsoView;
        }

        internal char current_row = 'A';

        private void SetCurrentTSOFileName(string filename)
        {
            string basename = Path.GetFileNameWithoutExtension(filename);
            if (basename.Length == 12)
                current_row = basename.ToUpper()[9];
            else
                current_row = 'A';
        }

        private int GetCenterBoneType()
        {
            switch (current_row)
            {
                case 'S'://手首
                case 'X'://腕装備(手甲など)
                case 'Z'://手持ちの小物
                    return 1;
                case 'W'://タイツ・ガーター
                case 'I'://靴下
                    return 2;
                case 'O'://靴
                    return 3;
                default:
                    return 0;
            }
        }

        private string GetCenterBoneName()
        {
            switch (current_row)
            {
                case 'A'://身体
                    return "W_Neck";
                case 'E'://瞳
                case 'D'://頭皮(生え際)
                case 'B'://前髪
                case 'C'://後髪
                case 'U'://アホ毛類
                    return "Kami_Oya";
                case 'Q'://眼鏡
                case 'V'://眼帯
                case 'Y'://リボン
                case 'P'://頭部装備(帽子等)
                    return "face_oya";
                case '3'://イヤリング類
                case '0'://眉毛
                case '2'://ほくろ
                case '1'://八重歯
                    return "Head";
                case 'R'://首輪
                    return "W_Neck";
                case 'F'://ブラ
                case 'J'://上衣(シャツ等)
                case 'T'://背中(羽など)
                case 'L'://上着オプション(エプロン等)
                    return "W_Spine3";
                case 'G'://全身下着・水着
                case 'K'://全身衣装(ナース服等)
                    return "W_Spine1";
                case 'S'://手首
                case 'X'://腕装備(手甲など)
                case 'Z'://手持ちの小物
                    return "W_Hips";//not reached
                case 'H'://パンツ
                case 'M'://下衣(スカート等)
                case 'N'://尻尾
                    return "W_Hips";
                case 'W'://タイツ・ガーター
                case 'I'://靴下
                    return "W_Hips";//not reached
                case 'O'://靴
                    return "W_Hips";//not reached
                default:
                    return "W_Hips";
            }
        }

        public void UpdateCenterPosition(string tsoname)
        {
            Vector3 position;

            TSOFile tso = viewer.FigureList[0].TSOList[0];

            Dictionary<string, TSONode> nodemap = new Dictionary<string, TSONode>();
            foreach (TSONode node in tso.nodes)
            {
                if (nodemap.ContainsKey(node.ShortName) == false)
                    nodemap.Add(node.ShortName, node);
            }


            SetCurrentTSOFileName(tsoname);

            switch (GetCenterBoneType())
            {
                case 1://Hand
                    {
                        TSONode tso_nodeR;
                        TSONode tso_nodeL;
                        string boneR = "W_RightHand";
                        string boneL = "W_LeftHand";
                        if (nodemap.TryGetValue(boneR, out tso_nodeR) && nodemap.TryGetValue(boneL, out tso_nodeL))
                        {
                            Matrix mR = tso_nodeR.combined_matrix;
                            Matrix mL = tso_nodeL.combined_matrix;
                            position = new Vector3((mR.M41 + mL.M41) / 2.0f, (mR.M42 + mL.M42) / 2.0f, -(mR.M43 + mL.M43) / 2.0f);
                            viewer.Camera.Reset();
                            viewer.Camera.SetCenter(position);
                        }
                    }
                    break;
                case 2://Leg
                    {
                        TSONode tso_nodeR;
                        TSONode tso_nodeL;
                        string boneR = "W_RightLeg";
                        string boneL = "W_LeftLeg";
                        if (nodemap.TryGetValue(boneR, out tso_nodeR) && nodemap.TryGetValue(boneL, out tso_nodeL))
                        {
                            Matrix mR = tso_nodeR.combined_matrix;
                            Matrix mL = tso_nodeL.combined_matrix;
                            position = new Vector3((mR.M41 + mL.M41) / 2.0f, (mR.M42 + mL.M42) / 2.0f, -(mR.M43 + mL.M43) / 2.0f);
                            viewer.Camera.Reset();
                            viewer.Camera.SetCenter(position);
                        }
                    }
                    break;
                case 3://Foot
                    {
                        TSONode tso_nodeR;
                        TSONode tso_nodeL;
                        string boneR = "W_RightFoot";
                        string boneL = "W_LeftFoot";
                        if (nodemap.TryGetValue(boneR, out tso_nodeR) && nodemap.TryGetValue(boneL, out tso_nodeL))
                        {
                            Matrix mR = tso_nodeR.combined_matrix;
                            Matrix mL = tso_nodeL.combined_matrix;
                            position = new Vector3((mR.M41 + mL.M41) / 2.0f, (mR.M42 + mL.M42) / 2.0f, -(mR.M43 + mL.M43) / 2.0f);
                            viewer.Camera.Reset();
                            viewer.Camera.SetCenter(position);
                        }
                    }
                    break;
                default:
                    {
                        TSONode tso_node;
                        string bone = GetCenterBoneName();
                        if (nodemap.TryGetValue(bone, out tso_node))
                        {
                            Matrix m = tso_node.combined_matrix;
                            position = new Vector3(m.M41, m.M42, -m.M43);
                            viewer.Camera.Reset();
                            viewer.Camera.SetCenter(position);
                        }
                    }
                    break;
            }
        }

        public void SelectBone(string bone)
        {
            Vector3 position;

            TSOFile tso = viewer.FigureList[0].TSOList[0];

            Dictionary<string, TSONode> nodemap = new Dictionary<string, TSONode>();
            foreach (TSONode node in tso.nodes)
            {
                if (nodemap.ContainsKey(node.ShortName) == false)
                    nodemap.Add(node.ShortName, node);
            }

            TSONode tso_node;
            if (nodemap.TryGetValue(bone, out tso_node))
            {
                Matrix m = tso_node.combined_matrix;
                position = new Vector3(m.M41, m.M42, -m.M43);
                viewer.Camera.Reset();
                viewer.Camera.SetCenter(position);
            }
        }
    }
}
