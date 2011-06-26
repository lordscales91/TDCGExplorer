using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System.Windows.Forms;
using CSScriptLibrary;

using TDCG;

namespace TDCGUtils
{
    public class PNGFileUtils
    {
        public List<string> GetCategoryList(string file_name)
        {
            string[] category = {
                "身体",
                "前髪",
                "後髪",
                "頭皮",
                "瞳",
                "ブラ",
                "全身下着・水着",
                "パンツ",
                "靴下",
                "上衣",
                "全身衣装",
                "上着オプション",
                "下衣",
                "尻尾",
                "靴",
                "頭部装備",
                "眼鏡",
                "首輪",
                "手首",
                "背中",
                "アホ毛類",
                "眼帯",
                "タイツ・ガーター",
                "腕装備",
                "リボン",
                "手持ちの小物or背景",
                "眉毛",
                "ほくろ",
                "八重歯",
                "イヤリング類"};

            List<string> opt = new List<string>();
            PNGFile png = new PNGFile();
            png.Ftso += delegate(Stream dest, int extract_length, byte[] opt1)
            {
                opt.Add(category[(int)opt1[0]]);
            };
            png.Load(file_name);

            return opt;
        }
    }

    // -----------------------------------------------------
    // TSOFileクラスに追加メソッド
    // -----------------------------------------------------
    public static class TSOFileUtils
    {
        public static void ReconNodePath(this TDCG.TSOFile tso)
        {
            foreach (TSONode node in tso.nodes)
            {
                node.Path = GetReconPath(node);
            }
        }

        private static string GetReconPath(TSONode node)
        {
            if (node.parent != null)
            {
                return GetReconPath(node.parent) + "|" + node.Name;
            }
            else
            {
                return "|" + node.Name;
            }
        }

        /// <summary>
        /// 指定名称（短い形式）を持つnodeを検索します。
        /// </summary>
        /// <param name="name">node名称（短い形式）</param>
        /// <returns></returns>
        public static TSONode FindNodeByName(this TDCG.TSOFile tso, string name)
        {
            foreach (TSONode node in tso.nodes)
                if (node.Name == name)
                    return node;
            return null;
        }
    }

    // -----------------------------------------------------
    // TMOFileクラスに追加メソッド
    // -----------------------------------------------------
    public static class TMOFileUtils
    {
        /// <summary>
        /// 現在の行列を新規フレームに保存します。
        /// (実際に新規に増やしているのは、nodeのmatrixで、frameは増えてない)
        /// </summary>
        public static void SaveTransformationMatrixToFrameAdd(this TDCG.TMOFile tmo)
        {
            foreach (TMONode node in tmo.nodes)
            {
                TMOMat tm = new TMOMat();
                tm.m = node.TransformationMatrix;
                node.matrices.Add(tm);
            }
        }

        /// <summary>
        /// nodeのmatricesを、frameのmatricesにそっくりコピーします。
        /// </summary>
        public static void NodeMatrices2FrameMatrices(this TDCG.TMOFile tmo)
        {
            NodeMatrices2FrameMatrices(tmo, 0);
        }

        /// <summary>
        /// nodeのmatricesを、frameのmatricesにそっくりコピーします。
        /// </summary>
        public static void NodeMatrices2FrameMatrices(this TDCG.TMOFile tmo, int start_point)
        {
            // Frameの数を、Nodeの行列の数に合わせる
            int n_frame = tmo.nodes[0].matrices.Count - start_point;
            Array.Resize(ref tmo.frames, n_frame);
            for (int i = 0; i < n_frame; i++)
                tmo.frames[i] = new TMOFrame(i);
            tmo.opt0 = tmo.frames.Length - 1;

            // そっくりコピー
            foreach (TMOFrame frame in tmo.frames)
            {
                frame.Matrices = new TMOMat[tmo.nodes.Length];

                foreach (TMONode node in tmo.nodes)
                {
                    frame.Matrices[node.ID] = new TMOMat(ref node.matrices[frame.ID + start_point].m);
                }
            }
        }
    }

    // -----------------------------------------------------
    // TMONodeクラスに追加メソッド
    // -----------------------------------------------------
    public static class TMONodeUtils
    {
        /// <summary>
        /// ワールド座標系での回転を得る
        /// </summary>
        public static Quaternion GetWorldRotation(this TDCG.TMONode node)
        {
            TMONode n = node;
            Quaternion q = Quaternion.Identity;
            while (n != null)
            {
                q.Multiply(n.Rotation);
                n = n.parent;
            }
            return q;
        }
    }

    // -----------------------------------------------------
    // ProportionListクラスに追加メソッド
    // -----------------------------------------------------
    public static class ProportionListUtils
    {
        /// <summary>
        /// 体型スクリプトを読み込みます。
        /// </summary>
        public static void Load(this TDCG.ProportionList pl, string folder)
        {
            string proportion_path = Path.Combine(Application.StartupPath, folder);
            if (!Directory.Exists(proportion_path))
                return;

            string[] script_files = Directory.GetFiles(proportion_path, "*.cs");
            foreach (string script_file in script_files)
            {
                string class_name = "TDCG.Proportion." + Path.GetFileNameWithoutExtension(script_file);
                var script = CSScript.Load(script_file).CreateInstance(class_name).AlignToInterface<IProportion>();
                pl.items.Add(script);
            }
        }
    }

}
