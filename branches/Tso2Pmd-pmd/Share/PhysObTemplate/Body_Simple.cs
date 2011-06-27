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
            phys_list.GetBodyByName("上半身").shape_type = 1; // 形状：タイプ(0:球、1:箱、2:カプセル) // 00 // 球
            phys_list.GetBodyByName("上半身").shape_w = 1.0f; // 形状：半径(幅) // CD CC CC 3F // 1.6
            phys_list.GetBodyByName("上半身").shape_h *= 0.5f;
            phys_list.GetBodyByName("上半身").shape_d = 0.5f; // 形状：奥行 // CD CC CC 3D // 0.1

            phys_list.MakeBodyFromBone("上半身２");
            phys_list.GetBodyByName("上半身２").shape_type = 1; // 形状：タイプ(0:球、1:箱、2:カプセル) // 00 // 球
            phys_list.GetBodyByName("上半身２").shape_w = 1.0f; // 形状：半径(幅) // CD CC CC 3F // 1.6
            phys_list.GetBodyByName("上半身２").shape_h *= 0.5f;
            phys_list.GetBodyByName("上半身２").shape_d = 0.5f; // 形状：奥行 // CD CC CC 3D // 0.1

            phys_list.MakeBodyFromBone("上半身３");
            phys_list.GetBodyByName("上半身３").shape_type = 1; // 形状：タイプ(0:球、1:箱、2:カプセル) // 00 // 球
            phys_list.GetBodyByName("上半身３").shape_w = 1.0f; // 形状：半径(幅) // CD CC CC 3F // 1.6
            phys_list.GetBodyByName("上半身３").shape_h *= 0.5f;
            phys_list.GetBodyByName("上半身３").shape_d = 0.5f; // 形状：奥行 // CD CC CC 3D // 0.1

            phys_list.MakeBodyFromBone("頭");

            phys_list.MakeBodyFromBone("左肩");

            phys_list.MakeBodyFromBone("左腕");

            phys_list.MakeBodyFromBone("左ひじ");

            phys_list.MakeBodyFromBone("右肩");

            phys_list.MakeBodyFromBone("右腕");

            phys_list.MakeBodyFromBone("右ひじ");

            phys_list.MakeBodyFromBone("左足");
            phys_list.GetBodyByName("左足").shape_w = 0.75f; // 形状：半径(幅) // CD CC CC 3F // 1.6

            phys_list.MakeBodyFromBone("左ひざ");

            phys_list.MakeBodyFromBone("右足");
            phys_list.GetBodyByName("右足").shape_w = 0.75f; // 形状：半径(幅) // CD CC CC 3F // 1.6

            phys_list.MakeBodyFromBone("右ひざ");
        }
    }
}
