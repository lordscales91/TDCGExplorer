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
        string name = "身体";   // 枠に表示される名前
        int group = 3;          // 表示される枠の種類（0:髪 1:乳 2:スカート 3:その他（初期チェックあり）4:その他（初期チェックなし））

        public string Name() { return name; }
        public int Group() { return group; }

        public void Execute(ref T2PPhysObjectList phys_list)
        {
            phys_list.MakeBodyFromBone("下半身");

            phys_list.MakeBodyFromBone("上半身");
            {
                PMD_RBody body = phys_list.GetBodyByName("上半身");
                body.shape_id = 1;
                body.size.X = 1.0f;
                body.size.Y *= 0.5f;
                body.size.Z = 0.5f;
            }

            phys_list.MakeBodyFromBone("上半身２");
            {
                PMD_RBody body = phys_list.GetBodyByName("上半身２");
                body.shape_id = 1;
                body.size.X = 1.0f;
                body.size.Y *= 0.5f;
                body.size.Z = 0.5f;
            }

            phys_list.MakeBodyFromBone("上半身３");
            {
                PMD_RBody body = phys_list.GetBodyByName("上半身３");
                body.shape_id = 1;
                body.size.X = 1.0f;
                body.size.Y *= 0.5f;
                body.size.Z = 0.5f;
            }

            phys_list.MakeBodyFromBone("頭");

            phys_list.MakeBodyFromBone("左肩");

            phys_list.MakeBodyFromBone("左腕");

            phys_list.MakeBodyFromBone("左ひじ");

            phys_list.MakeBodyFromBone("右肩");

            phys_list.MakeBodyFromBone("右腕");

            phys_list.MakeBodyFromBone("右ひじ");

            phys_list.MakeBodyFromBone("左足");
            phys_list.GetBodyByName("左足").size.X = 0.75f;
            
            phys_list.MakeBodyFromBone("左ひざ");

            phys_list.MakeBodyFromBone("右足");
            phys_list.GetBodyByName("右足").size.X = 0.75f;
            
            phys_list.MakeBodyFromBone("右ひざ");
        }
    }
}
