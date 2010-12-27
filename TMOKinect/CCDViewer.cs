using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using Direct3D = Microsoft.DirectX.Direct3D;

namespace TDCG
{
    /// <summary>
    /// TSOFileをDirect3D上でレンダリングします。
    /// </summary>
public class CCDViewer : Viewer
{
    public struct XnVector3D
    {
        public float X;
        public float Y;
        public float Z;
    }
    public struct XnSkeletonJointPosition
    {
        public XnVector3D position;
        public float confidence;
    }

    [DllImport("NiSimpleTracker.dll")]
    public static extern int OpenNIClean();
    [DllImport("NiSimpleTracker.dll")]
    public static extern IntPtr OpenNIGetDepthBuf();
    [DllImport("NiSimpleTracker.dll")]
    public static extern IntPtr OpenNIGetJointPos();
    [DllImport("NiSimpleTracker.dll")]
    public static extern bool OpenNIIsTracking();
    [DllImport("NiSimpleTracker.dll")]
    public static extern int OpenNIInit(StringBuilder path);
    [DllImport("NiSimpleTracker.dll")]
    public static extern void OpenNIDrawDepthMap();

    const int camw = 640;
    const int camh = 480;
    bool OpenNiEnabled = false;
    Surface surface = null;
    byte[] surface_buf = null;

    static Vector3 ToVector3(XnVector3D position)
    {
        return new Vector3(position.X, position.Y, -position.Z);
    }

    private void OnDeviceLost(object sender, EventArgs e)
    {
        Console.WriteLine("CCDViewer.OnDeviceLost");
        if (OpenNiEnabled)
        {
            ni_model_translation = Vector3.Empty;
            OpenNIClean();
            Console.WriteLine("ok OpenNIClean");
            OpenNiEnabled = false;
        }
        if (surface != null)
            surface.Dispose();
    }

    private void OnDeviceReset(object sender, EventArgs e)
    {
        Console.WriteLine("CCDViewer.OnDeviceReset");
        {
            StringBuilder path = new StringBuilder(Path.Combine(Application.StartupPath, "Data\\SamplesConfig.xml"));
            Console.WriteLine("OpenNIInit on {0}", path);
            OpenNiEnabled = (OpenNIInit(path) == 0);
            Console.WriteLine("ok OpenNIInit:{0}", OpenNiEnabled);
        }
        if (OpenNiEnabled)
        {
            surface = device.CreateOffscreenPlainSurface(camw, camh, Format.X8R8G8B8, Pool.Default);
            surface_buf = new byte[camw * camh * 4];
        }
    }

    internal Mesh sphere = null;

    CCDSolver solver = new CCDSolver();
    /// <summary>
    /// 逆運動学の解法
    /// </summary>
    public CCDSolver Solver { get { return solver; } }

    /// <summary>
    /// viewerを生成します。
    /// </summary>
    public CCDViewer()
    {
        this.Rendering += delegate()
        {
            RenderDerived();
        };
        SetJointNames();
    }

    /// マウスボタンを押したときに実行するハンドラ
    protected override void form_OnMouseDown(object sender, MouseEventArgs e)
    {
        switch (e.Button)
        {
        case MouseButtons.Left:
            if (Control.ModifierKeys == Keys.Control)
                lightDir = ScreenToOrientation(e.X, e.Y);
            else
                if (!MotionEnabled)
                {
                    SelectEffector();
                }
            break;
        }

        lastScreenPoint.X = e.X;
        lastScreenPoint.Y = e.Y;
    }

    /// マウスを移動したときに実行するハンドラ
    protected override void form_OnMouseMove(object sender, MouseEventArgs e)
    {
        int dx = e.X - lastScreenPoint.X;
        int dy = e.Y - lastScreenPoint.Y;

        switch (e.Button)
        {
        case MouseButtons.Left:
            if (Control.ModifierKeys == Keys.Control)
                lightDir = ScreenToOrientation(e.X, e.Y);
            else
                if (!MotionEnabled)
                {
                    if (Control.ModifierKeys == Keys.Shift)
                        SetTargetOnScreen(e.X, e.Y);
                    else
                        if (current_handle_dir != Vector3.Empty)
                            RotateOnScreen(dx, dy);
                        else
                            Camera.Move(dx, -dy, 0.0f);
                }
                else
                    Camera.Move(dx, -dy, 0.0f);
            break;
        case MouseButtons.Middle:
            Camera.MoveView(-dx * 0.125f, dy * 0.125f);
            break;
        case MouseButtons.Right:
            Camera.Move(0.0f, 0.0f, -dy * 0.125f);
            break;
        }

        lastScreenPoint.X = e.X;
        lastScreenPoint.Y = e.Y;
    }

    /// <summary>
    /// 指定シーンフレームに進みます。
    /// </summary>
    public void FrameMoveDerived()
    {
        if (OpenNiEnabled && OpenNIIsTracking())
        {
            OpenNiSolve();
        }
        if (MotionEnabled)
            return;

        if (solver.Solved)
            return;

        Figure fig;
        if (TryGetFigure(out fig))
        {
            if (current_effector_path == "|W_Hips")
                solver.SolveRootNode(fig.Tmo, current_effector_path);
            else
                solver.Solve(fig.Tmo, current_effector_path, solver.Target);
            fig.UpdateBoneMatricesWithoutTMOFrame();
        }
    }

    const float ni_world_scale = 0.0125f;
    Vector3 ToWorldPosition(XnVector3D p)
    {
        return new Vector3( p.X * ni_world_scale, p.Y * ni_world_scale, -p.Z * ni_world_scale);
    }

    XnSkeletonJointPosition[] ni_joint_ary = new XnSkeletonJointPosition[15];
    List<string> ni_joint_names = new List<string>();
    Dictionary<string, XnSkeletonJointPosition> ni_joint_map = new Dictionary<string, XnSkeletonJointPosition>();

    void SetJointNames()
    {
        ni_joint_names.Add("Torso");
        ni_joint_names.Add("Neck");
        ni_joint_names.Add("Head");
        ni_joint_names.Add("LeftShoulder");
        ni_joint_names.Add("LeftElbow");
        ni_joint_names.Add("LeftHand");
        ni_joint_names.Add("RightShoulder");
        ni_joint_names.Add("RightElbow");
        ni_joint_names.Add("RightHand");
        ni_joint_names.Add("LeftHip");
        ni_joint_names.Add("LeftKnee");
        ni_joint_names.Add("LeftFoot");
        ni_joint_names.Add("RightHip");
        ni_joint_names.Add("RightKnee");
        ni_joint_names.Add("RightFoot");
    }

    private void OpenNiSolve()
    {
        Figure fig;
        if (TryGetFigure(out fig))
        {
            IntPtr ptr = OpenNIGetJointPos();
            for (int i = 0; i < 15; i++)
            {
                ni_joint_ary[i] = (XnSkeletonJointPosition)Marshal.PtrToStructure(ptr, typeof(XnSkeletonJointPosition));
                ptr = (IntPtr)(ptr.ToInt32() + Marshal.SizeOf(typeof(XnSkeletonJointPosition)));
            }

            if (ni_joint_ary[0].confidence < 0.5f)
                return;

            for (int i = 0; i < 15; i++)
            {
                ni_joint_map[ni_joint_names[i]] = ni_joint_ary[i];
            }

            TMOFile tmo = fig.Tmo;
            {
                Vector3 p0 = tmo.FindNodeByName("W_Hips").Translation;
                Vector3 p1 = ToWorldPosition(Mean(ni_joint_map["LeftHip"].position, ni_joint_map["RightHip"].position));
                if (ni_model_translation == Vector3.Empty)
                {
                    ni_model_translation = p1 - p0;
                    Console.WriteLine("ni_model_translation:{0}", ni_model_translation);
                }
                tmo.FindNodeByName("W_Hips").Translation = p1 - ni_model_translation;
            }

            tmo.nodemap["|W_Hips"].Rotation = Quaternion.Identity;
            tmo.nodemap["|W_Hips|W_Spine_Dummy|W_Spine1"].Rotation = Quaternion.Identity;
            tmo.nodemap["|W_Hips|W_Spine_Dummy|W_Spine1|W_Spine2|W_Spine3"].Rotation = Quaternion.Identity;
            tmo.nodemap["|W_Hips|W_Spine_Dummy|W_Spine1|W_Spine2|W_Spine3|W_Neck"].Rotation = Quaternion.Identity;

            tmo.nodemap["|W_Hips|W_Spine_Dummy|W_Spine1|W_Spine2|W_Spine3|W_RightShoulder_Dummy|W_RightShoulder|W_RightArm_Dummy|W_RightArm"].Rotation = Quaternion.Identity;
            tmo.nodemap["|W_Hips|W_Spine_Dummy|W_Spine1|W_Spine2|W_Spine3|W_RightShoulder_Dummy|W_RightShoulder|W_RightArm_Dummy|W_RightArm|W_RightArmRoll|W_RightForeArm"].Rotation = Quaternion.Identity;
            tmo.nodemap["|W_Hips|W_RightHips_Dummy|W_RightUpLeg"].Rotation = Quaternion.Identity;
            tmo.nodemap["|W_Hips|W_RightHips_Dummy|W_RightUpLeg|W_RightUpLegRoll|W_RightLeg"].Rotation = Quaternion.Identity;

            tmo.nodemap["|W_Hips|W_Spine_Dummy|W_Spine1|W_Spine2|W_Spine3|W_LeftShoulder_Dummy|W_LeftShoulder|W_LeftArm_Dummy|W_LeftArm"].Rotation = Quaternion.Identity;
            tmo.nodemap["|W_Hips|W_Spine_Dummy|W_Spine1|W_Spine2|W_Spine3|W_LeftShoulder_Dummy|W_LeftShoulder|W_LeftArm_Dummy|W_LeftArm|W_LeftArmRoll|W_LeftForeArm"].Rotation = Quaternion.Identity;
            tmo.nodemap["|W_Hips|W_LeftHips_Dummy|W_LeftUpLeg"].Rotation = Quaternion.Identity;
            tmo.nodemap["|W_Hips|W_LeftHips_Dummy|W_LeftUpLeg|W_LeftUpLegRoll|W_LeftLeg"].Rotation = Quaternion.Identity;

            Quaternion q;
            if (TryNiRotationDirX(out q, ni_joint_map["LeftHip"], ni_joint_map["RightHip"]))
                tmo.FindNodeByName("W_Hips").Rotation = q;

            {
                Vector3 p1 = ToWorldPosition(ni_joint_map["Torso"].position);
                solver.Solve(tmo, "|W_Hips|W_Spine_Dummy|W_Spine1|W_Spine2|W_Spine3", p1 - ni_model_translation);
            }

            {
                Vector3 p1 = ToWorldPosition(ni_joint_map["Neck"].position);
                solver.Solve(tmo, "|W_Hips|W_Spine_Dummy|W_Spine1|W_Spine2|W_Spine3|W_Neck", p1 - ni_model_translation);
            }

            {
                Vector3 p1 = ToWorldPosition(ni_joint_map["Head"].position);
                solver.Solve(tmo, "|W_Hips|W_Spine_Dummy|W_Spine1|W_Spine2|W_Spine3|W_Neck|Head", p1 - ni_model_translation);
            }

            {
                //Vector3 p1 = ToWorldPosition(ni_joint_map["LeftShoulder"].position);
                //solver.Solve(tmo, "|W_Hips|W_Spine_Dummy|W_Spine1|W_Spine2|W_Spine3|W_RightShoulder_Dummy|W_RightShoulder|W_RightArm_Dummy|W_RightArm", p1 - ni_model_translation);
            }

            {
                Vector3 p1 = ToWorldPosition(ni_joint_map["LeftElbow"].position);
                solver.Solve(tmo, "|W_Hips|W_Spine_Dummy|W_Spine1|W_Spine2|W_Spine3|W_RightShoulder_Dummy|W_RightShoulder|W_RightArm_Dummy|W_RightArm|W_RightArmRoll|W_RightForeArm", p1 - ni_model_translation);
            }

            {
                Vector3 p1 = ToWorldPosition(ni_joint_map["LeftHand"].position);
                solver.Solve(tmo, "|W_Hips|W_Spine_Dummy|W_Spine1|W_Spine2|W_Spine3|W_RightShoulder_Dummy|W_RightShoulder|W_RightArm_Dummy|W_RightArm|W_RightArmRoll|W_RightForeArm|W_RightForeArmRoll|W_RightHand", p1 - ni_model_translation);
            }

            {
                Vector3 p1 = ToWorldPosition(ni_joint_map["LeftKnee"].position);
                solver.Solve(tmo, "|W_Hips|W_RightHips_Dummy|W_RightUpLeg|W_RightUpLegRoll|W_RightLeg", p1 - ni_model_translation);
            }

            {
                Vector3 p1 = ToWorldPosition(ni_joint_map["LeftFoot"].position);
                solver.Solve(tmo, "|W_Hips|W_RightHips_Dummy|W_RightUpLeg|W_RightUpLegRoll|W_RightLeg|W_RightLegRoll|W_RightFoot", p1 - ni_model_translation);
            }

            {
                //Vector3 p1 = ToWorldPosition(ni_joint_map["RightShoulder"].position);
                //solver.Solve(tmo, "|W_Hips|W_Spine_Dummy|W_Spine1|W_Spine2|W_Spine3|W_LeftShoulder_Dummy|W_LeftShoulder|W_LeftArm_Dummy|W_LeftArm", p1 - ni_model_translation);
            }

            {
                Vector3 p1 = ToWorldPosition(ni_joint_map["RightElbow"].position);
                solver.Solve(tmo, "|W_Hips|W_Spine_Dummy|W_Spine1|W_Spine2|W_Spine3|W_LeftShoulder_Dummy|W_LeftShoulder|W_LeftArm_Dummy|W_LeftArm|W_LeftArmRoll|W_LeftForeArm", p1 - ni_model_translation);
            }

            {
                Vector3 p1 = ToWorldPosition(ni_joint_map["RightHand"].position);
                solver.Solve(tmo, "|W_Hips|W_Spine_Dummy|W_Spine1|W_Spine2|W_Spine3|W_LeftShoulder_Dummy|W_LeftShoulder|W_LeftArm_Dummy|W_LeftArm|W_LeftArmRoll|W_LeftForeArm|W_LeftForeArmRoll|W_LeftHand", p1 - ni_model_translation);
            }

            {
                Vector3 p1 = ToWorldPosition(ni_joint_map["RightKnee"].position);
                solver.Solve(tmo, "|W_Hips|W_LeftHips_Dummy|W_LeftUpLeg|W_LeftUpLegRoll|W_LeftLeg", p1 - ni_model_translation);
            }

            {
                Vector3 p1 = ToWorldPosition(ni_joint_map["RightFoot"].position);
                solver.Solve(tmo, "|W_Hips|W_LeftHips_Dummy|W_LeftUpLeg|W_LeftUpLegRoll|W_LeftLeg|W_LeftLegRoll|W_LeftFoot", p1 - ni_model_translation);
            }

            /*
            if (TryNiRotation(out q, map["Neck"], map["LeftShoulder"], map["LeftElbow"]))
                tmo.FindNodeByName("W_RightArm").Rotation = q;
            if (TryNiRotation(out q, map["LeftShoulder"], map["LeftElbow"], map["LeftHand"]))
                tmo.FindNodeByName("W_RightForeArm").Rotation = q;

            if (TryNiRotation(out q, map["Neck"], map["RightShoulder"], map["RightElbow"]))
                tmo.FindNodeByName("W_LeftArm").Rotation = q;
            if (TryNiRotation(out q, map["RightShoulder"], map["RightElbow"], map["RightHand"]))
                tmo.FindNodeByName("W_LeftForeArm").Rotation = q;

            if (TryNiRotation(out q, map["LeftShoulder"], map["LeftHip"], map["LeftKnee"]))
                tmo.FindNodeByName("W_RightUpLeg").Rotation = q;
            if (TryNiRotation(out q, map["LeftHip"], map["LeftKnee"], map["LeftFoot"]))
                tmo.FindNodeByName("W_RightLeg").Rotation = q;

            if (TryNiRotation(out q, map["RightShoulder"], map["RightHip"], map["RightKnee"]))
                tmo.FindNodeByName("W_LeftUpLeg").Rotation = q;
            if (TryNiRotation(out q, map["RightHip"], map["RightKnee"], map["RightFoot"]))
                tmo.FindNodeByName("W_LeftLeg").Rotation = q;
            */
            fig.UpdateBoneMatricesWithoutTMOFrame();
        }
    }

    float Mean(float f1, float f2)
    {
        return (f1 + f2) / 2.0f;
    }
    XnVector3D Mean(XnVector3D v1, XnVector3D v2)
    {
        XnVector3D mean;
        mean.X = Mean(v1.X, v2.X);
        mean.Y = Mean(v1.Y, v2.Y);
        mean.Z = Mean(v1.Z, v2.Z);
        return mean;
    }
    XnSkeletonJointPosition Mean(XnSkeletonJointPosition p1, XnSkeletonJointPosition p2)
    {
        XnSkeletonJointPosition mean;
        mean.position = Mean(p1.position, p2.position);
        mean.confidence = Mean(p1.confidence, p2.confidence);
        return mean;
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

    Vector3 ni_model_translation = Vector3.Empty;

    bool TryNiRotationDirX(out Quaternion q, XnSkeletonJointPosition xnp1, XnSkeletonJointPosition xnp2)
    {
        if (xnp1.confidence < 0.5f || xnp2.confidence < 0.5f)
        {
            q = Quaternion.Identity;
            return false;
        }

        Vector3 p1 = ToVector3(xnp1.position);
        Vector3 p2 = ToVector3(xnp2.position);

        Vector3 vx = new Vector3(1, 0, 0);
        Vector3 v2 = p2 - p1;

        RotationVectorToVector(vx, v2, out q);
        return true;
    }

    bool TryNiRotationDirY(out Quaternion q, XnSkeletonJointPosition xnp1, XnSkeletonJointPosition xnp2)
    {
        if (xnp1.confidence < 0.5f || xnp2.confidence < 0.5f)
        {
            q = Quaternion.Identity;
            return false;
        }

        Vector3 p1 = ToVector3(xnp1.position);
        Vector3 p2 = ToVector3(xnp2.position);

        Vector3 vy = new Vector3(0, 1, 0);
        Vector3 v2 = p2 - p1;

        RotationVectorToVector(vy, v2, out q);
        return true;
    }

    bool TryNiRotation(out Quaternion q, XnSkeletonJointPosition xnp0, XnSkeletonJointPosition xnp1, XnSkeletonJointPosition xnp2)
    {
        if (xnp0.confidence < 0.5f || xnp1.confidence < 0.5f || xnp2.confidence < 0.5f)
        {
            q = Quaternion.Identity;
            return false;
        }

        Vector3 p0 = ToVector3(xnp0.position);
        Vector3 p1 = ToVector3(xnp1.position);
        Vector3 p2 = ToVector3(xnp2.position);

        Vector3 v1 = p1 - p0;
        Vector3 v2 = p2 - p1;

        RotationVectorToVector(v1, v2, out q);
        return true;
    }

    /// <summary>
    /// シーンをレンダリングします。
    /// </summary>
    public void RenderDerived()
    {
        if (OpenNiEnabled)
        {
            if (OpenNIIsTracking())
            {
                Vector4 color = new Vector4(1, 0, 0, 0.5f);
                foreach (XnSkeletonJointPosition xnp in ni_joint_ary)
                {
                    Vector3 p1 = ToWorldPosition(xnp.position);
                    Vector3 pos = p1 - ni_model_translation;
                    DrawMesh(sphere, Matrix.Translation(pos), color);
                }
            }

            {
                GraphicsStream gs = surface.LockRectangle(LockFlags.None);
                OpenNIDrawDepthMap();
                IntPtr ptr = OpenNIGetDepthBuf();
                int len = camw * camh * 4;
                Marshal.Copy(ptr, surface_buf, 0, len);
                Marshal.Copy(surface_buf, 0, gs.InternalData, len);
                surface.UnlockRectangle();
            }

            Rectangle src_rect = new Rectangle(0, 0, camw, camh);

            //カメラ画像の転写矩形を作成
            Viewport vp = device.Viewport;
            Rectangle view_rect = new Rectangle(vp.X + vp.Width - vp.Width / 4, vp.Y, vp.Width / 4, vp.Height / 4);

            {
                //背景描画
                Surface dest_surface = device.GetBackBuffer(0, 0, BackBufferType.Mono);
                device.StretchRectangle(surface, src_rect, dest_surface, view_rect, TextureFilter.None);
            }
        }
        if (MotionEnabled)
            return;

        DrawEffector();
        DrawTarget();
    }

    /// <summary>
    /// 逆運動学における目標をスクリーン座標で指定します。
    /// </summary>
    private void SetTargetOnScreen(float x, float y)
    {
        Figure fig;
        if (TryGetFigure(out fig))
        {
            Debug.Assert(fig.Tmo.nodemap != null, "fig.Tmo.nodemap should not be null");
            TMONode bone;
            if (fig.Tmo.nodemap.TryGetValue(current_effector_path, out bone))
            {
                Vector3 v = solver.Target;
                v = Vector3.TransformCoordinate(v, Transform_View);
                v = Vector3.TransformCoordinate(v, Transform_Projection);
                solver.Target = ScreenToWorld(x, y, v.Z);
                solver.Solved = false;
            }
        }
    }

    private void SelectEffector()
    {
        Vector3 dir;
        if (FindCurrentEffectorHandleOnScreenPoint(lastScreenPoint.X, lastScreenPoint.Y, out dir))
            current_handle_dir = dir;
        else
        {
            current_handle_dir = Vector3.Empty;

            TMONode effector;
            if (FindEffectorOnScreenPoint(lastScreenPoint.X, lastScreenPoint.Y, out effector))
            {
                current_effector_path = effector.Path;
                Matrix m = effector.combined_matrix;
                solver.Target = new Vector3(m.M41, m.M42, m.M43);
                solver.Solved = true;
            }
        }
    }

    void RotateOnScreen(int dx, int dy)
    {
        Figure fig;
        if (TryGetFigure(out fig))
        {
            Debug.Assert(fig.Tmo.nodemap != null, "fig.Tmo.nodemap should not be null");
            TMONode bone;
            if (fig.Tmo.nodemap.TryGetValue(current_effector_path, out bone))
            {
                float angle = dx * 0.01f;
                bone.Rotation = Quaternion.RotationAxis(current_handle_dir, angle) * bone.Rotation;
            }
            fig.UpdateBoneMatricesWithoutTMOFrame();
        }
    }

    /// <summary>
    /// deviceを作成します。
    /// </summary>
    /// <param name="control">レンダリング先となるcontrol</param>
    /// <returns>deviceの作成に成功したか</returns>
    public new bool InitializeApplication(Control control)
    {
        return InitializeApplication(control, false);
    }

    /// <summary>
    /// deviceを作成します。
    /// </summary>
    /// <param name="control">レンダリング先となるcontrol</param>
    /// <param name="shadowMapEnabled">シャドウマップを作成するか</param>
    /// <returns>deviceの作成に成功したか</returns>
    public new bool InitializeApplication(Control control, bool shadowMapEnabled)
    {
        if (! base.InitializeApplication(control, shadowMapEnabled))
            return false;

        sphere = Mesh.Sphere(device, 0.25f, 8, 6);

        current_effector_path = "|W_Hips";

        //should be update target when select figure
        this.FigureEvent += delegate(object sender, EventArgs e)
        {
            Figure fig;
            if (TryGetFigure(out fig))
            {
                Debug.Assert(fig.Tmo.nodemap != null, "fig.Tmo.nodemap should not be null");
                TMONode bone;
                if (fig.Tmo.nodemap.TryGetValue(current_effector_path, out bone))
                {
                    Matrix m = bone.combined_matrix;
                    solver.Target = new Vector3(m.M41, m.M42, m.M43);
                    solver.Solved = true;
                }
            }
        };

        device.DeviceLost += new EventHandler(OnDeviceLost);
        device.DeviceReset += new EventHandler(OnDeviceReset);
        OnDeviceReset(device, null);

        return true;
    }

    string current_effector_path = null;
    Vector3 current_handle_dir = Vector3.Empty;
    
    void DrawEffector()
    {
        Figure fig;
        if (TryGetFigure(out fig))
        {
            Debug.Assert(fig.Tmo.nodemap != null, "fig.Tmo.nodemap should not be null");
            foreach (string effector_path in solver.EachEffecterNames)
            {
                TMONode bone;
                if (fig.Tmo.nodemap.TryGetValue(effector_path, out bone))
                {
                    Vector4 color = (bone.Path == current_effector_path) ? new Vector4(1, 1, 1, 0.5f) : new Vector4(0.5f, 0.5f, 0.5f, 0.5f);
                    Matrix m = bone.combined_matrix;
                    Vector3 pos = new Vector3(m.M41, m.M42, m.M43);
                    DrawMesh(sphere, Matrix.Translation(pos), color);
                }
            }

            {
                TMONode bone;
                if (fig.Tmo.nodemap.TryGetValue(current_effector_path, out bone))
                {
                    Matrix m = bone.combined_matrix;
                    DrawMesh(sphere, Matrix.Translation(new Vector3(1, 0, 0)) * m, new Vector4(1, 0, 0, 0.5f));
                    DrawMesh(sphere, Matrix.Translation(new Vector3(0, 1, 0)) * m, new Vector4(0, 1, 0, 0.5f));
                    DrawMesh(sphere, Matrix.Translation(new Vector3(0, 0, 1)) * m, new Vector4(0, 0, 1, 0.5f));
                }
            }
        }
    }

    void DrawTarget()
    {
        DrawMesh(sphere, Matrix.Translation(solver.Target), new Vector4(1, 1, 0, 0.5f));
    }

    /// スクリーン座標からエフェクタを見つけます。
    /// 衝突するエフェクタの中で最も近い位置にあるエフェクタを返します。
    private bool FindEffectorOnScreenPoint(float x, float y, out TMONode effector)
    {
        effector = null;

        Figure fig;
        if (TryGetFigure(out fig))
        {
            Debug.Assert(fig.Tmo.nodemap != null, "fig.Tmo.nodemap should not be null");
            float min_time = 1e12f;
            foreach (string effector_path in solver.EachEffecterNames)
            {
                TMONode bone;
                if (fig.Tmo.nodemap.TryGetValue(effector_path, out bone))
                {
                    Vector3 collisionPoint;
                    float collisionTime;
                    if (FindBoneOnScreenPoint(lastScreenPoint.X, lastScreenPoint.Y, bone, out collisionPoint, out collisionTime))
                    {
                        if (collisionTime < min_time)
                        {
                            min_time = collisionTime;
                            effector = bone;
                        }
                    }
                }
            }
            if (effector != null)
                return true;
        }
        return false;
    }

    /// <summary>
    /// 指定スクリーン座標に指定ボーンがあるか。
    /// </summary>
    /// <param name="x">スクリーンX座標</param>
    /// <param name="y">スクリーンY座標</param>
    /// <param name="bone">ボーン</param>
    /// <returns>ボーンを見つけたか</returns>
    public bool FindBoneOnScreenPoint(float x, float y, TMONode bone)
    {
        float collisionTime;
        Vector3 collisionPoint;

        return FindBoneOnScreenPoint(x, y, bone, out collisionPoint, out collisionTime);
    }

    /// <summary>
    /// 指定スクリーン座標に指定ボーンがあるか。
    /// </summary>
    /// <param name="x">スクリーンX座標</param>
    /// <param name="y">スクリーンY座標</param>
    /// <param name="bone">ボーン</param>
    /// <param name="collisionPoint"></param>
    /// <param name="collisionTime"></param>
    /// <returns>ボーンを見つけたか</returns>
    public bool FindBoneOnScreenPoint(float x, float y, TMONode bone, out Vector3 collisionPoint, out float collisionTime)
    {
        collisionTime = 0.0f;
        collisionPoint = Vector3.Empty;

        Figure fig;
        if (TryGetFigure(out fig))
        {
            Matrix m = bone.combined_matrix;

            float sphereRadius = 0.25f;
            Vector3 sphereCenter = new Vector3(m.M41, m.M42, m.M43);
            Vector3 rayStart = ScreenToWorld(x, y, 0.0f);
            Vector3 rayEnd = ScreenToWorld(x, y, 1.0f);
            Vector3 rayOrientation = rayEnd - rayStart;

            return DetectSphereRayCollision(sphereRadius, ref sphereCenter, ref rayStart, ref rayOrientation, out collisionPoint, out collisionTime);
        }
        return false;
    }

    /// <summary>
    /// 指定スクリーン座標に現在のエフェクタハンドルがあるか。
    /// </summary>
    /// <param name="x">スクリーンX座標</param>
    /// <param name="y">スクリーンY座標</param>
    /// <param name="dir">ハンドルの方向</param>
    /// <returns>エフェクタハンドルを見つけたか</returns>
    public bool FindCurrentEffectorHandleOnScreenPoint(float x, float y, out Vector3 dir)
    {
        dir = Vector3.Empty;

        Figure fig;
        if (TryGetFigure(out fig))
        {
            Debug.Assert(fig.Tmo.nodemap != null, "fig.Tmo.nodemap should not be null");
            {
                TMONode bone;
                if (fig.Tmo.nodemap.TryGetValue(current_effector_path, out bone))
                {
                    dir = new Vector3(1,0,0);
                    if (FindBoneHandleOnScreenPoint(lastScreenPoint.X, lastScreenPoint.Y, bone, dir))
                        return true;
                    dir = new Vector3(0,1,0);
                    if (FindBoneHandleOnScreenPoint(lastScreenPoint.X, lastScreenPoint.Y, bone, dir))
                        return true;
                    dir = new Vector3(0,0,1);
                    if (FindBoneHandleOnScreenPoint(lastScreenPoint.X, lastScreenPoint.Y, bone, dir))
                        return true;
                }
            }
        }
        return false;
    }

    /// <summary>
    /// 指定スクリーン座標に指定ボーンハンドルがあるか。
    /// </summary>
    /// <param name="x">スクリーンX座標</param>
    /// <param name="y">スクリーンY座標</param>
    /// <param name="bone">ボーン</param>
    /// <param name="dir">ハンドルの方向</param>
    /// <returns>ボーンハンドルを見つけたか</returns>
    public bool FindBoneHandleOnScreenPoint(float x, float y, TMONode bone, Vector3 dir)
    {
        Figure fig;
        if (TryGetFigure(out fig))
        {
            Matrix m = Matrix.Translation(dir) * bone.combined_matrix;

            float sphereRadius = 0.25f;
            Vector3 sphereCenter = new Vector3(m.M41, m.M42, m.M43);
            Vector3 rayStart = ScreenToWorld(x, y, 0.0f);
            Vector3 rayEnd = ScreenToWorld(x, y, 1.0f);
            Vector3 rayOrientation = rayEnd - rayStart;

            Vector3 collisionPoint;
            float collisionTime;

            return DetectSphereRayCollision(sphereRadius, ref sphereCenter, ref rayStart, ref rayOrientation, out collisionPoint, out collisionTime);
        }
        return false;
    }

    /// <summary>
    /// 内部objectを破棄します。
    /// </summary>
    public new void Dispose()
    {
        if (surface != null)
            surface.Dispose();
        if (sphere != null)
            sphere.Dispose();
        base.Dispose();
    }
}
}
