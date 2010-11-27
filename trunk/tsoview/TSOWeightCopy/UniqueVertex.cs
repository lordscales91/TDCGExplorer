using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using TDCG;

namespace TSOWeightCopy
{
    /// 同じ位置にある頂点（同一視頂点）を扱います。
    public class UniqueVertex
    {
        /// <summary>
        /// 指定位置の文字表現を得ます。
        /// </summary>
        /// <param name="v">位置</param>
        /// <returns></returns>
        public static string ToString(Vector3 v)
        {
            return string.Format("[ {0} {1} {2} ]", v.X, v.Y, v.Z);
        }

        /// 頂点リスト
        public Dictionary<Vertex, TSOSubMesh> vertices;

        /// 位置
        public Vector3 position;

        /// スキンウェイト配列
        public SkinWeight[] skin_weights;

        /// 対称位置にある同一視頂点
        public UniqueVertex opposite_vertex;

        /// <summary>
        /// 対称関係にあるボーン（の添字）辞書
        /// </summary>
        public static Dictionary<int, int> oppnode_idmap = null;

        /// <summary>
        /// 指定ボーンと対称関係にあるボーン（の添字）を得ます。
        /// </summary>
        /// <param name="bone_index">ボーン添字</param>
        /// <returns></returns>
        public int GetOppositeBoneIndex(int bone_index)
        {
            return oppnode_idmap[bone_index];
        }

        /// <summary>
        /// 同一視頂点を生成します。
        /// </summary>
        /// <param name="a">頂点</param>
        /// <param name="sub">頂点を含むサブメッシュ</param>
        public UniqueVertex(Vertex a, TSOSubMesh sub)
        {
            this.vertices = new Dictionary<Vertex, TSOSubMesh>();
            vertices[a] = sub;
            this.position = a.position;
            this.skin_weights = new SkinWeight[4];
            for (int i = 0; i < 4; i++)
            {
                SkinWeight sw = a.skin_weights[i];
                skin_weights[i] = new SkinWeight(sub.bone_indices[sw.bone_index], sw.weight);
            }
            opposite_vertex = null;
        }

        /// 同じ位置にある頂点を追加します。
        public void Push(Vertex a, TSOSubMesh sub)
        {
            vertices[a] = sub;

            //頂点のボーン参照が最初の頂点と異なる場合は警告する。
            for (int i = 0; i < 4; i++)
            {
                SkinWeight sw = skin_weights[i];
                SkinWeight a_sw = a.skin_weights[i];
                if (sw.weight != 0.0f)
                {
                    if (sw.bone_index != sub.bone_indices[a_sw.bone_index])
                    {
                        Console.WriteLine("### warn: bone_index not match");
                    }
                }
            }
        }

        /// 対称位置を得ます。
        public Vector3 GetOppositePosition()
        {
            return new Vector3(-position.X, position.Y, position.Z);
        }

        /// <summary>
        /// 文字列表現を得ます。
        /// </summary>
        /// <returns>文字列表現</returns>
        public override string ToString()
        {
            return string.Format("UniqueVertex(p:{0} #v:{1})", ToString(position), vertices.Count);
        }

        /// <summary>
        /// 文字列表現を出力します。
        /// </summary>
        public void Dump()
        {
            Console.WriteLine(this);
            Console.WriteLine("opp {0}", opposite_vertex);
        }

        /// 警告 'ウェイト値がずれている' を出力します。
        public void WarnOppositeWeights()
        {
            Console.WriteLine("### warn: weights gap found");
            Dump();
            for (int i = 0; i < 4; i++)
            {
                SkinWeight sw = skin_weights[i];
                SkinWeight opp_sw = opposite_vertex.skin_weights[i];
                Console.WriteLine("{0} sw({1} {2}) opp sw({3} {4})", i, sw.bone_index, sw.weight, opp_sw.bone_index, opp_sw.weight);
            }
            Console.WriteLine();
        }

        /// 警告 'ボーン参照が見つからなかった' を出力します。
        public void WarnBoneIndexNotFound(Vertex a, TSOSubMesh sub)
        {
            Console.WriteLine("### warn: a_sw.bone_index not found in sub.bone_indices");
            Dump();
            for (int i = 0; i < 4; i++)
            {
                SkinWeight sw = skin_weights[i];
                SkinWeight a_sw = a.skin_weights[i];
                Console.WriteLine("{0} sw({1} {2}) a sw({3} {4})", i, sw.bone_index, sw.weight, sub.bone_indices[a_sw.bone_index], a_sw.weight);
            }
            Console.WriteLine();
        }

        /// 対称位置にある同一視頂点のスキンウェイトを複写します。
        public void CopyOppositeWeights()
        {
            Debug.Assert(opposite_vertex != null, "opposite_vertex should not be null");
            Debug.Assert(opposite_vertex != this, "opposite_vertex should not be self");

            //ウェイト値がずれている場合は警告する。
            bool weights_gap_found = false;
            for (int i = 0; i < 4; i++)
            {
                SkinWeight sw = skin_weights[i];
                SkinWeight opp_sw = opposite_vertex.skin_weights[i];
                if (Math.Abs(sw.weight - opp_sw.weight) >= 1.0e-2f)
                {
                    weights_gap_found = true;
                    break;
                }
            }
            if (weights_gap_found)
                WarnOppositeWeights();

            for (int i = 0; i < 4; i++)
            {
                SkinWeight sw = skin_weights[i];
                SkinWeight opp_sw = opposite_vertex.skin_weights[i];
                sw.bone_index = GetOppositeBoneIndex(opp_sw.bone_index);
                sw.weight = opp_sw.weight;
            }

            foreach (KeyValuePair<Vertex, TSOSubMesh> pair in vertices)
            {
                Vertex a = pair.Key;
                TSOSubMesh sub = pair.Value;
                bool bone_index_not_found = false;
                for (int i = 0; i < 4; i++)
                {
                    SkinWeight sw = skin_weights[i];
                    SkinWeight a_sw = a.skin_weights[i];
                    int a_bone_idx = Array.IndexOf(sub.bone_indices, sw.bone_index);
                    if (a_bone_idx == -1)
                    {
                        if (sw.weight == 0.0f)
                            a_bone_idx = 0;
                        else if (a_sw.weight == 0.0f)
                            a_bone_idx = 0;
                        else
                        {
                            bone_index_not_found = true;
                            continue;
                        }
                    }
                    a_sw.bone_index = a_bone_idx;
                    a_sw.weight = sw.weight;
                }
                if (bone_index_not_found)
                    WarnBoneIndexNotFound(a, sub);
            }
        }
    }
}
