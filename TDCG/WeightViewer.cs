using System;
using System.Collections.Generic;
using System.Diagnostics;
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
public class WeightViewer : Viewer
{
    internal Mesh sphere = null;

    /// <summary>
    /// viewerを生成します。
    /// </summary>
    public WeightViewer()
    {
        this.Rendering += delegate()
        {
            RenderDerived();
        };
    }

    /// 球とレイの衝突を見つけます。
    public bool DetectSphereRayCollision(float sphereRadius, ref Vector3 sphereCenter, ref Vector3 rayStart, ref Vector3 rayOrientation, out Vector3 collisionPoint, out float collisionTime)
    {
        collisionTime = 0.0f;
        collisionPoint = Vector3.Empty;

        Vector3 u = rayStart - sphereCenter;
        float a = Vector3.Dot(rayOrientation, rayOrientation);
        float b = Vector3.Dot(rayOrientation, u);
        float c = Vector3.Dot(u, u) - sphereRadius*sphereRadius;
        if (a <= float.Epsilon)
            //誤差
            return false;
        float d = b*b - a*c;
        if (d < 0.0f)
            //衝突しない
            return false;
        collisionTime = (-b - (float)Math.Sqrt(d))/a;
        collisionPoint = rayStart + rayOrientation*collisionTime;
        return true;
    }

    /// <summary>
    /// viewport行列を作成します。
    /// </summary>
    /// <param name="viewport">viewport</param>
    /// <returns>viewport行列</returns>
    public Matrix CreateViewportMatrix(Viewport viewport)
    {
        Matrix m = Matrix.Identity;
        m.M11 = (float)viewport.Width / 2;
        m.M22 = -1.0f * (float)viewport.Height / 2;
        m.M33 = (float)viewport.MaxZ - (float)viewport.MinZ;
        m.M41 = (float)(viewport.X + viewport.Width / 2);
        m.M42 = (float)(viewport.Y + viewport.Height / 2);
        m.M43 = viewport.MinZ;
        return m;
    }

    /// スクリーン位置をワールド座標へ変換します。
    public Vector3 ScreenToWorld(float screenX, float screenY, float z, Viewport viewport, Matrix view, Matrix proj)
    {
        //スクリーン位置
        Vector3 v = new Vector3(screenX, screenY,  z);

        Matrix inv_m = Matrix.Invert(CreateViewportMatrix(viewport));
        Matrix inv_proj = Matrix.Invert(proj);
        Matrix inv_view = Matrix.Invert(view);

        //スクリーン位置をワールド座標へ変換
        return Vector3.TransformCoordinate(v, inv_m * inv_proj * inv_view);
    }

    /// スクリーン位置をワールド座標へ変換します。
    public Vector3 ScreenToWorld(float screenX, float screenY, float z)
    {
        return ScreenToWorld(screenX, screenY, z, device.Viewport, Transform_View, Transform_Projection);
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

        sphere = Mesh.Sphere(device, 0.25f, 4, 2);

        return true;
    }

    /// <summary>
    /// シーンをレンダリングします。
    /// </summary>
    public void RenderDerived()
    {
        if (motionEnabled)
            return;

        DrawFigureVertices();
    }

    public TSOMesh selected_mesh = null;

    void DrawFigureVertices()
    {
        Figure fig;
        if (TryGetFigure(out fig))
        {
            if (selected_mesh != null)
            {
                DrawVertices(fig, selected_mesh);
                if (selected_vertex_id != -1)
                    DrawSelectedVertex(fig);
            }
        }
    }

    void DrawVertices(Figure fig, TSOMesh mesh)
    {
        Matrix[] clipped_boneMatrices = new Matrix[mesh.maxPalettes];

        for (int numPalettes = 0; numPalettes < mesh.maxPalettes; numPalettes++)
        {
            TSONode tso_node = mesh.GetBone(numPalettes);
            TMONode tmo_node;
            if (fig.nodemap.TryGetValue(tso_node, out tmo_node))
                clipped_boneMatrices[numPalettes] = tso_node.OffsetMatrix * tmo_node.combined_matrix;
        }


        float scale = 0.1f;
        Vector4 color = new Vector4(1, 0, 0, 0.5f);

        if (selected_vertex_id != -1)
        {
            Vector3 v0 = selected_mesh.vertices[selected_vertex_id].position;

            for (int i = 0; i < mesh.vertices.Length; i++)
            {
                Vector3 v1 = mesh.vertices[i].position;
                float dx = v1.X - v0.X;
                float dy = v1.Y - v0.Y;
                float dz = v1.Z - v0.Z;
                if (dx * dx + dy * dy + dz * dz < 0.5f * 0.5f)
                    color.Y = 1;
                else
                    color.Y = 0;

                Vector3 pos = CalcSkindeformPosition(ref mesh.vertices[i], clipped_boneMatrices);
                Matrix m = Matrix.Scaling(scale, scale, scale);
                m.M41 = pos.X;
                m.M42 = pos.Y;
                m.M43 = pos.Z;
                DrawMesh(sphere, m, color);
            }
        }
        else
        {
            for (int i = 0; i < mesh.vertices.Length; i++)
            {
                Vector3 pos = CalcSkindeformPosition(ref mesh.vertices[i], clipped_boneMatrices);
                Matrix m = Matrix.Scaling(scale, scale, scale);
                m.M41 = pos.X;
                m.M42 = pos.Y;
                m.M43 = pos.Z;
                DrawMesh(sphere, m, color);
            }
        }
    }

    void DrawSelectedVertex(Figure fig)
    {
        TSOMesh mesh = selected_mesh;
        Matrix[] clipped_boneMatrices = new Matrix[mesh.maxPalettes];

        for (int numPalettes = 0; numPalettes < mesh.maxPalettes; numPalettes++)
        {
            TSONode tso_node = mesh.GetBone(numPalettes);
            TMONode tmo_node;
            if (fig.nodemap.TryGetValue(tso_node, out tmo_node))
                clipped_boneMatrices[numPalettes] = tso_node.OffsetMatrix * tmo_node.combined_matrix;
        }

        float scale = 0.1f;
        Vector4 color = new Vector4(0, 1, 0, 1);

        {
            int i = selected_vertex_id;
            Vector3 pos = CalcSkindeformPosition(ref mesh.vertices[i], clipped_boneMatrices);
            Matrix m = Matrix.Scaling(scale, scale, scale);
            m.M41 = pos.X;
            m.M42 = pos.Y;
            m.M43 = pos.Z;
            DrawMesh(sphere, m, color);
        }
    }

    public void GainSkinWeight(TSONode selected_node)
    {
        Figure fig;
        if (TryGetFigure(out fig))
        {
            if (selected_mesh != null)
            {
                GainSkinWeight(selected_mesh, selected_node);
            }
        }
    }

    public void GainSkinWeight(TSOMesh mesh, TSONode selected_node)
    {
        bool updated = false;
        Vector3 p0 = selected_mesh.vertices[selected_vertex_id].position;

        for (int i = 0; i < mesh.vertices.Length; i++)
        {
            Vertex v = mesh.vertices[i];
            Vector3 p1 = v.position;
            float dx = p1.X - p0.X;
            float dy = p1.Y - p0.Y;
            float dz = p1.Z - p0.Z;
            if (dx * dx + dy * dy + dz * dz < 0.5f * 0.5f)
            {
                if (GainSkinWeight(mesh, selected_node, ref v))
                    updated = true;
            }
        }
        if (updated)
            mesh.WriteBuffer(device);
    }

    private static bool GainSkinWeight(TSOMesh mesh, TSONode selected_node, ref Vertex v)
    {
        bool updated = false;
        SkinWeight selected_skin_weight = null;
        foreach (SkinWeight skin_weight in v.skin_weights)
        {
            TSONode bone = mesh.GetBone(skin_weight.bone_index);
            if (bone == selected_node)
            {
                selected_skin_weight = skin_weight;
                break;
            }
        }
        if (selected_skin_weight != null)
        {
            updated = true;

            float prev_selected_weight = selected_skin_weight.weight;
            float prev_rest_weight = 1.0f - prev_selected_weight;
            selected_skin_weight.weight += 0.2f;
            if (selected_skin_weight.weight > 1.0f)
                selected_skin_weight.weight = 1.0f;
            float gain_weight = selected_skin_weight.weight - prev_selected_weight;

            //reduce weight
            foreach (SkinWeight skin_weight in v.skin_weights)
            {
                if (skin_weight == selected_skin_weight)
                    continue;

                skin_weight.weight -= gain_weight * skin_weight.weight / prev_rest_weight;
                if (skin_weight.weight < 0.0001f)
                    skin_weight.weight = 0.0f;
            }
        }
        return updated;
    }

    public static Vector3 CalcSkindeformPosition(ref Vertex v, Matrix[] boneMatrices)
    {
        Vector3 pos = Vector3.Empty;
        for (int i = 0; i < 4; i++)
        {
            Matrix m = boneMatrices[v.skin_weights[i].bone_index];
            float w = v.skin_weights[i].weight;
            pos += Vector3.TransformCoordinate(v.position, m) * w;
        }
        return pos;
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
                if (!motionEnabled)
                {
                    SelectVertex();
                }
            break;
        }

        lastScreenPoint.X = e.X;
        lastScreenPoint.Y = e.Y;
    }

    public int selected_vertex_id = -1;

    /// <summary>
    /// 頂点選択時に呼び出されるハンドラ
    /// </summary>
    public event EventHandler VertexEvent;

    private void SelectVertex()
    {
        Figure fig;
        if (TryGetFigure(out fig))
        {
            if (selected_mesh != null)
            {
                int vertex_id = FindVertexOnScreenPoint(lastScreenPoint.X, lastScreenPoint.Y, fig, selected_mesh);
                if (vertex_id != -1)
                {
                    selected_vertex_id = vertex_id;
                    if (VertexEvent != null)
                        VertexEvent(this, EventArgs.Empty);
                }
            }
        }
    }

    /// スクリーン座標から頂点を見つけます。
    /// 衝突する頂点の中で最も近い位置にある頂点を返します。
    private int FindVertexOnScreenPoint(float x, float y, Figure fig, TSOMesh mesh)
    {
        int vertex_id = -1;

        Matrix[] clipped_boneMatrices = new Matrix[mesh.maxPalettes];

        for (int numPalettes = 0; numPalettes < mesh.maxPalettes; numPalettes++)
        {
            TSONode tso_node = mesh.GetBone(numPalettes);
            TMONode tmo_node;
            if (fig.nodemap.TryGetValue(tso_node, out tmo_node))
                clipped_boneMatrices[numPalettes] = tso_node.OffsetMatrix * tmo_node.combined_matrix;
        }

        {
            Vector3 collisionPoint;
            float collisionTime;
            float min_time = 1e12f;

            float sphereRadius = 0.25f * 0.1f;
            Vector3 rayStart = ScreenToWorld(x, y, 0.0f);
            Vector3 rayEnd = ScreenToWorld(x, y, 1.0f);
            Vector3 rayOrientation = rayEnd - rayStart;

            for (int i = 0; i < mesh.vertices.Length; i++)
            {
                Vector3 sphereCenter = CalcSkindeformPosition(ref mesh.vertices[i], clipped_boneMatrices);
                if (DetectSphereRayCollision(sphereRadius, ref sphereCenter, ref rayStart, ref rayOrientation, out collisionPoint, out collisionTime))
                {
                    if (collisionTime < min_time)
                    {
                        min_time = collisionTime;
                        vertex_id = i;
                    }
                }
            }
        }

        return vertex_id;
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
