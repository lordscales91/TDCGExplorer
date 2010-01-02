using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using Direct3D = Microsoft.DirectX.Direct3D;

namespace TDCG
{
    /// <summary>
    /// 逆運動学の解法
    /// </summary>
    public class CCDSolver
    {
        Vector3 target;
        /// <summary>
        /// 逆運動学における目標
        /// </summary>
        public Vector3 Target { get { return target; } set { target = value; } }

        /// <summary>
        /// 逆運動学における目標を移動します。
        /// </summary>
        public void MoveTarget(float dx, float dy, float dz)
        {
            if (dx == 0 && dy == 0 && dz == 0)
                return;
            target.X -= dx;
            target.Y -= dy;
            target.Z -= dz;
            solved = false;
        }

        /// <summary>
        /// 接地が有効であるか。
        /// </summary>
        public bool FloorEnabled { get; set; }

        bool solved = true;
        /// <summary>
        /// 逆運動学による解を得られたか。
        /// </summary>
        public bool Solved { get { return solved; } set { solved = value; } }

        Dictionary<string, string[]> effector_dictionary = new Dictionary<string, string[]>();
        List<string> effector_list = new List<string>();
        Dictionary<string, Vector3> target_dictionary = new Dictionary<string, Vector3>();

        /// <summary>
        /// 各エフェクタの名称を返します。
        /// </summary>
        public Dictionary<string, string[]>.KeyCollection EachEffecterNames
        {
            get { return effector_dictionary.Keys; }
        }
        
        /// <summary>
        /// 逆運動学の解法を生成します。
        /// </summary>
        public CCDSolver()
        {
            Target = new Vector3(5.0f, 10.0f, 0.0f);

            //左腕
            effector_dictionary["|W_Hips|W_Spine_Dummy|W_Spine1|W_Spine2|W_Spine3|W_LeftShoulder_Dummy|W_LeftShoulder|W_LeftArm_Dummy|W_LeftArm|W_LeftArmRoll|W_LeftForeArm|W_LeftForeArmRoll|W_LeftHand"] =
                new string[] {
                "|W_Hips|W_Spine_Dummy|W_Spine1|W_Spine2|W_Spine3|W_LeftShoulder_Dummy|W_LeftShoulder|W_LeftArm_Dummy|W_LeftArm|W_LeftArmRoll|W_LeftForeArm",
                "|W_Hips|W_Spine_Dummy|W_Spine1|W_Spine2|W_Spine3|W_LeftShoulder_Dummy|W_LeftShoulder|W_LeftArm_Dummy|W_LeftArm",
                "|W_Hips|W_Spine_Dummy|W_Spine1|W_Spine2|W_Spine3|W_LeftShoulder_Dummy|W_LeftShoulder" };

            effector_dictionary["|W_Hips|W_Spine_Dummy|W_Spine1|W_Spine2|W_Spine3|W_LeftShoulder_Dummy|W_LeftShoulder|W_LeftArm_Dummy|W_LeftArm|W_LeftArmRoll|W_LeftForeArm"] =
                new string[] {
                "|W_Hips|W_Spine_Dummy|W_Spine1|W_Spine2|W_Spine3|W_LeftShoulder_Dummy|W_LeftShoulder|W_LeftArm_Dummy|W_LeftArm",
                "|W_Hips|W_Spine_Dummy|W_Spine1|W_Spine2|W_Spine3|W_LeftShoulder_Dummy|W_LeftShoulder" };

            effector_dictionary["|W_Hips|W_Spine_Dummy|W_Spine1|W_Spine2|W_Spine3|W_LeftShoulder_Dummy|W_LeftShoulder|W_LeftArm_Dummy|W_LeftArm"] =
                new string[] {
                "|W_Hips|W_Spine_Dummy|W_Spine1|W_Spine2|W_Spine3|W_LeftShoulder_Dummy|W_LeftShoulder" };

            effector_dictionary["|W_Hips|W_Spine_Dummy|W_Spine1|W_Spine2|W_Spine3|W_LeftShoulder_Dummy|W_LeftShoulder"] =
                new string[] { };

            //右腕
            effector_dictionary["|W_Hips|W_Spine_Dummy|W_Spine1|W_Spine2|W_Spine3|W_RightShoulder_Dummy|W_RightShoulder|W_RightArm_Dummy|W_RightArm|W_RightArmRoll|W_RightForeArm|W_RightForeArmRoll|W_RightHand"] =
                new string[] {
                "|W_Hips|W_Spine_Dummy|W_Spine1|W_Spine2|W_Spine3|W_RightShoulder_Dummy|W_RightShoulder|W_RightArm_Dummy|W_RightArm|W_RightArmRoll|W_RightForeArm",
                "|W_Hips|W_Spine_Dummy|W_Spine1|W_Spine2|W_Spine3|W_RightShoulder_Dummy|W_RightShoulder|W_RightArm_Dummy|W_RightArm",
                "|W_Hips|W_Spine_Dummy|W_Spine1|W_Spine2|W_Spine3|W_RightShoulder_Dummy|W_RightShoulder" };

            effector_dictionary["|W_Hips|W_Spine_Dummy|W_Spine1|W_Spine2|W_Spine3|W_RightShoulder_Dummy|W_RightShoulder|W_RightArm_Dummy|W_RightArm|W_RightArmRoll|W_RightForeArm"] =
                new string[] {
                "|W_Hips|W_Spine_Dummy|W_Spine1|W_Spine2|W_Spine3|W_RightShoulder_Dummy|W_RightShoulder|W_RightArm_Dummy|W_RightArm",
                "|W_Hips|W_Spine_Dummy|W_Spine1|W_Spine2|W_Spine3|W_RightShoulder_Dummy|W_RightShoulder" };

            effector_dictionary["|W_Hips|W_Spine_Dummy|W_Spine1|W_Spine2|W_Spine3|W_RightShoulder_Dummy|W_RightShoulder|W_RightArm_Dummy|W_RightArm"] =
                new string[] {
                "|W_Hips|W_Spine_Dummy|W_Spine1|W_Spine2|W_Spine3|W_RightShoulder_Dummy|W_RightShoulder" };

            effector_dictionary["|W_Hips|W_Spine_Dummy|W_Spine1|W_Spine2|W_Spine3|W_RightShoulder_Dummy|W_RightShoulder"] =
                new string[] { };

            //右足
            effector_dictionary["|W_Hips|W_RightHips_Dummy|W_RightUpLeg|W_RightUpLegRoll|W_RightLeg|W_RightLegRoll|W_RightFoot|W_RightToeBase"] =
                new string[] {
                "|W_Hips|W_RightHips_Dummy|W_RightUpLeg|W_RightUpLegRoll|W_RightLeg|W_RightLegRoll|W_RightFoot" };

            effector_dictionary["|W_Hips|W_RightHips_Dummy|W_RightUpLeg|W_RightUpLegRoll|W_RightLeg|W_RightLegRoll|W_RightFoot"] =
                new string[] {
                "|W_Hips|W_RightHips_Dummy|W_RightUpLeg|W_RightUpLegRoll|W_RightLeg",
                "|W_Hips|W_RightHips_Dummy|W_RightUpLeg" };

            effector_dictionary["|W_Hips|W_RightHips_Dummy|W_RightUpLeg|W_RightUpLegRoll|W_RightLeg"] =
                new string[] {
                "|W_Hips|W_RightHips_Dummy|W_RightUpLeg" };

            effector_dictionary["|W_Hips|W_RightHips_Dummy|W_RightUpLeg"] =
                new string[] { };

            //左足
            effector_dictionary["|W_Hips|W_LeftHips_Dummy|W_LeftUpLeg|W_LeftUpLegRoll|W_LeftLeg|W_LeftLegRoll|W_LeftFoot|W_LeftToeBase"] =
                new string[] {
                "|W_Hips|W_LeftHips_Dummy|W_LeftUpLeg|W_LeftUpLegRoll|W_LeftLeg|W_LeftLegRoll|W_LeftFoot" };

            effector_dictionary["|W_Hips|W_LeftHips_Dummy|W_LeftUpLeg|W_LeftUpLegRoll|W_LeftLeg|W_LeftLegRoll|W_LeftFoot"] =
                new string[] {
                "|W_Hips|W_LeftHips_Dummy|W_LeftUpLeg|W_LeftUpLegRoll|W_LeftLeg",
                "|W_Hips|W_LeftHips_Dummy|W_LeftUpLeg" };

            effector_dictionary["|W_Hips|W_LeftHips_Dummy|W_LeftUpLeg|W_LeftUpLegRoll|W_LeftLeg"] =
                new string[] {
                "|W_Hips|W_LeftHips_Dummy|W_LeftUpLeg" };

            effector_dictionary["|W_Hips|W_LeftHips_Dummy|W_LeftUpLeg"] =
                new string[] { };

            //頭
            effector_dictionary["|W_Hips|W_Spine_Dummy|W_Spine1|W_Spine2|W_Spine3|W_Neck|Head"] =
                new string[] { };

            //腰
            effector_dictionary["|W_Hips|W_Spine_Dummy|W_Spine1|W_Spine2|W_Spine3"] =
                new string[] { };

            effector_dictionary["|W_Hips|W_Spine_Dummy|W_Spine1"] =
                new string[] { };

            effector_dictionary["|W_Hips"] =
                new string[] { };

            effector_list.Add("|W_Hips|W_RightHips_Dummy|W_RightUpLeg|W_RightUpLegRoll|W_RightLeg|W_RightLegRoll|W_RightFoot");
            effector_list.Add("|W_Hips|W_LeftHips_Dummy|W_LeftUpLeg|W_LeftUpLegRoll|W_LeftLeg|W_LeftLegRoll|W_LeftFoot");
            effector_list.Add("|W_Hips|W_RightHips_Dummy|W_RightUpLeg|W_RightUpLegRoll|W_RightLeg|W_RightLegRoll|W_RightFoot|W_RightToeBase");
            effector_list.Add("|W_Hips|W_LeftHips_Dummy|W_LeftUpLeg|W_LeftUpLegRoll|W_LeftLeg|W_LeftLegRoll|W_LeftFoot|W_LeftToeBase");
        }

        /// <summary>
        /// 接地の目標を設定します。
        /// </summary>
        /// <param name="tmo">tmo</param>
        public void SaveFloorTargets(TMOFile tmo)
        {
            Debug.Assert(tmo.nodemap != null, "fig.Tmo.nodemap should not be null");
            target_dictionary.Clear();
            foreach (string effector_name in effector_list)
            {
                TMONode node;
                if (tmo.nodemap.TryGetValue(effector_name, out node))
                {
                    Vector3 p = node.GetWorldPosition();
                    p.Y = 0;
                    target_dictionary[effector_name] = p;
                }
            }
        }

        /// <summary>
        /// root nodeに対する解を得ます。
        /// </summary>
        /// <param name="tmo">tmo</param>
        /// <param name="effector_name">エフェクタnode名称</param>
        public void SolveRootNode(TMOFile tmo, string effector_name)
        {
            Debug.Assert(tmo.nodemap != null, "tso.nodemap should not be null");
            TMONode effector;
            if (tmo.nodemap.TryGetValue(effector_name, out effector))
            {
                effector.Translation = target;
            }
            if (FloorEnabled)
                foreach (string ename in effector_list)
                {
                    Solve(tmo, ename, target_dictionary[ename]);
                }
        }

        /// <summary>
        /// nodeを扱うデリゲート型
        /// </summary>
        /// <param name="node"></param>
        public delegate void TMONodeHandler(TMONode node);

        /// <summary>
        /// nodeを回転したときに呼び出すハンドラ
        /// </summary>
        public TMONodeHandler TMONodeRotation;

        /// <summary>
        /// 逆運動学による解を得ます。
        /// </summary>
        /// <param name="tmo">tmo</param>
        /// <param name="effector_name">エフェクタnode名称</param>
        /// <param name="target">目標</param>
        public void Solve(TMOFile tmo, string effector_name, Vector3 target)
        {
            Debug.Assert(tmo.nodemap != null, "tso.nodemap should not be null");
            TMONode effector;
            if (tmo.nodemap.TryGetValue(effector_name, out effector))
            {
                foreach (string node_name in effector_dictionary[effector_name])
                {
                    TMONode node;
                    if (tmo.nodemap.TryGetValue(node_name, out node))
                    {
                        Solve(effector, node, target);
                        if (TMONodeRotation != null)
                            TMONodeRotation(node);
                    }
                }
            }
        }

        /// <summary>
        /// Cyclic-Coordinate-Descent (CCD) 法による逆運動学の実装です。
        /// </summary>
        /// <param name="effector">エフェクタnode</param>
        /// <param name="node">対象node</param>
        /// <param name="target">目標</param>
        public void Solve(TMONode effector, TMONode node, Vector3 target)
        {
            Vector3 worldTargetP = target;

            Vector3 worldEffectorP = effector.GetWorldPosition();
            Vector3 worldNodeP = node.GetWorldPosition();

            Matrix invCoord = Matrix.Invert(node.GetWorldCoordinate());
            Vector3 localEffectorP = Vector3.TransformCoordinate(worldEffectorP, invCoord);
            Vector3 localTargetP = Vector3.TransformCoordinate(worldTargetP, invCoord);

            Quaternion q;
            if (RotationVectorToVector(localEffectorP, localTargetP, out q))
                node.Rotation = q * node.Rotation;
            if ((localEffectorP - localTargetP).LengthSq() < 0.1f)
                solved = true;
        }

        /// <summary>
        /// v1をv2に合わせる回転を得ます。
        /// </summary>
        /// <param name="v1">v1</param>
        /// <param name="v2">v2</param>
        /// <param name="q">q</param>
        /// <returns>回転が必要であるか</returns>
        public bool RotationVectorToVector(Vector3 v1, Vector3 v2, out Quaternion q)
        {
            Vector3 n1 = Vector3.Normalize(v1);
            Vector3 n2 = Vector3.Normalize(v2);
            float dotProduct = Vector3.Dot(n1, n2);
            float angle = (float)Math.Acos(dotProduct);
            bool needRotate = (angle > float.Epsilon);
            if (needRotate)
            {
                Vector3 axis = Vector3.Cross(n1, n2);
                q = Quaternion.RotationAxis(axis, angle);
            }
            else
                q = Quaternion.Identity;
            return needRotate;
        }
    }
}
