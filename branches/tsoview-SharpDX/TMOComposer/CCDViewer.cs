using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using SharpDX;
using SharpDX.Direct3D9;

namespace TDCG
{
    /// <summary>
    /// TSOFileをDirect3D上でレンダリングします。
    /// </summary>
public class CCDViewer : Viewer
{
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
        LimitRotationEnabled = true;
        Grounded = true;
        solver.TMONodeRotation += delegate(TMONode node)
        {
            LimitRotation(node);
        };
        this.Rendering += delegate()
        {
            RenderDerived();
        };
    }

    /// マウスボタンを押したときに実行するハンドラ
    protected override void form_OnMouseDown(object sender, MouseEventArgs e)
    {
        switch (e.Button)
        {
        case MouseButtons.Left:
            if (Control.ModifierKeys == Keys.Control)
                SetLightDirection(ScreenToOrientation(e.X, e.Y));
            else
                if (!MotionEnabled)
                {
                    SelectEffector();
                    if (current_effector_path == "|W_Hips")
                        SaveFloorTargets();
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
                SetLightDirection(ScreenToOrientation(e.X, e.Y));
            else
                if (!MotionEnabled)
                {
                    if (Control.ModifierKeys == Keys.Shift)
                        SetTargetOnScreen(e.X, e.Y);
                    else
                        if (current_handle_dir != Vector3.Zero)
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

    /// <summary>
    /// シーンをレンダリングします。
    /// </summary>
    public void RenderDerived()
    {
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
            current_handle_dir = Vector3.Zero;

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

    private void SaveFloorTargets()
    {
        Figure fig;
        if (TryGetFigure(out fig))
        {
            solver.SaveFloorTargets(fig.Tmo);
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
                LimitRotation(bone);
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

        constraint_xyz = TMOConstraint.Load(@"angle-GRABIA-xyz.xml");
        constraint_zxy = TMOConstraint.Load(@"angle-GRABIA-zxy.xml");

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

        return true;
    }

    string current_effector_path = null;
    Vector3 current_handle_dir = Vector3.Zero;
    TMOConstraint constraint_xyz = null;
    TMOConstraint constraint_zxy = null;
    
    /// <summary>
    /// 回転角制限が有効であるか。
    /// </summary>
    public bool LimitRotationEnabled { get; set; }
    
    /// <summary>
    /// 接地が有効であるか。
    /// </summary>
    public bool Grounded { get { return solver.Grounded; } set { solver.Grounded = value; } }

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
        collisionPoint = Vector3.Zero;

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
        dir = Vector3.Zero;

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

    static Regex re_legnode = new Regex(@"Leg|Foot|Toe");

    private void LimitRotation(TMONode node)
    {
        if (LimitRotationEnabled)
            if (re_legnode.IsMatch(node.Name))
                LimitRotationXYZ(node);
            else
                LimitRotationZXY(node);
    }

    private void LimitRotationXYZ(TMONode node)
    {
        TMOConstraintItem item = constraint_xyz.GetItem(node.Name);
        Vector3 angle1 = Helper.ToAngleXYZ(node.Rotation);
        Vector3 angle0 = item.Limit(angle1);
        node.Rotation = Helper.ToQuaternionXYZ(angle0);
        //Console.WriteLine("node {0} x {1:F2} y {2:F2} z {3:F2}", node.Name, angle0.X, angle0.Y, angle0.Z);
    }

    private void LimitRotationZXY(TMONode node)
    {
        TMOConstraintItem item = constraint_zxy.GetItem(node.Name);
        Vector3 angle1 = Helper.ToAngleZXY(node.Rotation);
        Vector3 angle0 = item.Limit(angle1);
        node.Rotation = Helper.ToQuaternionZXY(angle0);
        //Console.WriteLine("node {0} x {1:F2} y {2:F2} z {3:F2}", node.Name, angle0.X, angle0.Y, angle0.Z);
    }

    /// <summary>
    /// 内部objectを破棄します。
    /// </summary>
    public new void Dispose()
    {
        if (sphere != null)
            sphere.Dispose();
        base.Dispose();
    }
}
}
