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
    /// スキンウェイト属性
    public struct SkinWeightAttr
    {
        /// ボーン参照インデックス
        public int bone_index;
        /// ウェイト値
        public float weight;
    }
    /// スキンウェイト操作
    public class SkinWeightCommand
    {
        /// スキンウェイト
        public SkinWeight skin_weight = null;
        /// 変更前の属性
        public SkinWeightAttr old_attr;
        /// 変更後の属性
        public SkinWeightAttr new_attr;
    }
    /// 頂点操作
    public class VertexCommand
    {
        /// 頂点id
        public int vertex_id;
        /// スキンウェイト操作リスト
        public List<SkinWeightCommand> skin_weight_commands = new List<SkinWeightCommand>();
    }
    /// メッシュ操作
    public class MeshCommand
    {
        /// メッシュ
        public TSOMesh mesh = null;
        /// 頂点操作リスト
        public List<VertexCommand> vertex_commands = new List<VertexCommand>();
    }

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

    /// 頂点を描画する。
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

    /// 半径
    public static float radius = 0.5f;

    /// 頂点を描画する。
    void DrawVertices(Figure fig, TSOMesh mesh)
    {
        Matrix[] clipped_boneMatrices = ClipBoneMatrices(fig, mesh);

        float scale = 0.1f;
        Vector4 color = new Vector4(1, 0, 0, 0.5f);

        if (selected_vertex_id != -1)
        {
            Vector3 v0 = selected_mesh.vertices[selected_vertex_id].position;

            for (int i = 0; i < mesh.vertices.Length; i++)
            {
                //頂点間距離が半径未満なら黄色にする。
                Vector3 v1 = mesh.vertices[i].position;
                float dx = v1.X - v0.X;
                float dy = v1.Y - v0.Y;
                float dz = v1.Z - v0.Z;
                if (dx * dx + dy * dy + dz * dz < radius * radius)
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

    /// 選択頂点を描画する。
    void DrawSelectedVertex(Figure fig)
    {
        TSOMesh mesh = selected_mesh;
        Matrix[] clipped_boneMatrices = ClipBoneMatrices(fig, mesh);

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

    /// 選択ボーンに対応するウェイトを加算する。
    public void GainSkinWeight(TSONode selected_node)
    {
        if (selected_mesh != null)
        {
            GainSkinWeight(selected_mesh, selected_node);
        }
    }

    /// 選択ボーンに対応するウェイトを加算する。
    public void GainSkinWeight(TSOMesh mesh, TSONode selected_node)
    {
        if (selected_vertex_id == -1)
            return;

        //操作を生成する。
        MeshCommand mesh_command = new MeshCommand();
        mesh_command.mesh = mesh;

        bool updated = false;
        Vector3 p0 = selected_mesh.vertices[selected_vertex_id].position;

        for (int i = 0; i < mesh.vertices.Length; i++)
        {
            Vertex v = mesh.vertices[i];

            //頂点間距離が半径未満ならウェイトを加算する。
            Vector3 p1 = v.position;
            float dx = p1.X - p0.X;
            float dy = p1.Y - p0.Y;
            float dz = p1.Z - p0.Z;
            if (dx * dx + dy * dy + dz * dz < radius * radius)
            {
                if (GainSkinWeight(mesh, selected_node, i, mesh_command))
                    updated = true;
            }
        }
        if (updated)
        {
            if (mesh_command_id == mesh_commands.Count)
                mesh_commands.Add(mesh_command);
            else
                mesh_commands[mesh_command_id] = mesh_command;
            mesh_command_id++;
        }
        if (updated)
            mesh.WriteBuffer(device);
    }

    /// 加算ウェイト値
    public static float weight = 0.2f;
    /// メッシュ操作リスト
    public static List<MeshCommand> mesh_commands = new List<MeshCommand>();
    static int mesh_command_id = 0;

    /// 選択ボーンに対応するウェイトを加算する。
    /// returns: ウェイトを変更したか
    public static bool GainSkinWeight(TSOMesh mesh, TSONode selected_node, int vertex_id, MeshCommand mesh_command)
    {
        Vertex v = mesh.vertices[vertex_id];

        VertexCommand vertex_command = new VertexCommand();
        vertex_command.vertex_id = vertex_id;
        foreach (SkinWeight skin_weight in v.skin_weights)
        {
            SkinWeightCommand skin_weight_command = new SkinWeightCommand();
            skin_weight_command.skin_weight = skin_weight;
            vertex_command.skin_weight_commands.Add(skin_weight_command);
        }
        mesh_command.vertex_commands.Add(vertex_command);
        //処理前の値を記憶する。
        {
            int nskin_weight = 0;
            foreach (SkinWeight skin_weight in v.skin_weights)
            {
                vertex_command.skin_weight_commands[nskin_weight].old_attr.bone_index = skin_weight.bone_index;
                vertex_command.skin_weight_commands[nskin_weight].old_attr.weight = skin_weight.weight;
                nskin_weight++;
            }
        }
        bool updated = false;

        //選択ボーンに対応するウェイトを検索する。
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
        //選択ボーンに対応するウェイトがなければ、最小値を持つウェイトを置き換える。
        if (selected_skin_weight == null)
        {
            //メッシュのボーン参照に指定ノードが含まれるか。
            bool found = false;
            int bone_index = 0;
            foreach (TSONode bone in mesh.bones)
            {
                if (bone == selected_node)
                {
                    found = true;
                    break;
                }
                bone_index++;
            }
            if (found)
            {
                selected_skin_weight = v.skin_weights[3];
                selected_skin_weight.bone_index = bone_index;
                selected_skin_weight.weight = 0.0f;
            }
        }
        //選択ボーンに対応するウェイトを加算する。
        if (selected_skin_weight != null)
        {
            updated = true;

            //変更前の対象ウェイト値
            float prev_selected_weight = selected_skin_weight.weight;
            //変更前の残りウェイト値
            float prev_rest_weight = 1.0f - prev_selected_weight;
            //ウェイトを加算する。
            selected_skin_weight.weight += weight;
            if (selected_skin_weight.weight > 1.0f)
                selected_skin_weight.weight = 1.0f;

            if (prev_rest_weight != 0.0f)
            {
                //実際の加算値
                float gain_weight = selected_skin_weight.weight - prev_selected_weight;
                //残りウェイトを減算する。
                foreach (SkinWeight skin_weight in v.skin_weights)
                {
                    if (skin_weight == selected_skin_weight)
                        continue;

                    skin_weight.weight -= gain_weight * skin_weight.weight / prev_rest_weight;
                    if (skin_weight.weight < 0.001f)
                        skin_weight.weight = 0.0f;
                }
            }
        }
        //処理後の値を記憶する。
        {
            int nskin_weight = 0;
            foreach (SkinWeight skin_weight in v.skin_weights)
            {
                vertex_command.skin_weight_commands[nskin_weight].new_attr.bone_index = skin_weight.bone_index;
                vertex_command.skin_weight_commands[nskin_weight].new_attr.weight = skin_weight.weight;
                nskin_weight++;
            }
        }
        return updated;
    }

    /// ひとつ前の操作による変更を元に戻せるか。
    public bool CanUndo()
    {
        return (mesh_command_id > 0);
    }
    /// ひとつ前の操作による変更を元に戻す。
    public void Undo()
    {
        if (!CanUndo())
            return;

        mesh_command_id--;
        Undo(mesh_commands[mesh_command_id]);
    }
    /// 指定操作による変更を元に戻す。
    public void Undo(MeshCommand mesh_command)
    {
        foreach (VertexCommand vertex_command in mesh_command.vertex_commands)
        {
            int nskin_weight = 0;
            foreach (SkinWeightCommand skin_weight_command in vertex_command.skin_weight_commands)
            {
                mesh_command.mesh.vertices[vertex_command.vertex_id].skin_weights[nskin_weight].bone_index = skin_weight_command.old_attr.bone_index;
                mesh_command.mesh.vertices[vertex_command.vertex_id].skin_weights[nskin_weight].weight = skin_weight_command.old_attr.weight;
                nskin_weight++;
            }
        }
        mesh_command.mesh.WriteBuffer(device);
    }

    /// ひとつ前の操作による変更をやり直せるか。
    public bool CanRedo()
    {
        return (mesh_command_id < mesh_commands.Count);
    }
    /// ひとつ前の操作による変更をやり直す。
    public void Redo()
    {
        if (!CanRedo())
            return;

        Redo(mesh_commands[mesh_command_id]);
        mesh_command_id++;
    }
    /// 指定操作による変更をやり直す。
    public void Redo(MeshCommand mesh_command)
    {
        foreach (VertexCommand vertex_command in mesh_command.vertex_commands)
        {
            int nskin_weight = 0;
            foreach (SkinWeightCommand skin_weight_command in vertex_command.skin_weight_commands)
            {
                mesh_command.mesh.vertices[vertex_command.vertex_id].skin_weights[nskin_weight].bone_index = skin_weight_command.new_attr.bone_index;
                mesh_command.mesh.vertices[vertex_command.vertex_id].skin_weights[nskin_weight].weight = skin_weight_command.new_attr.weight;
                nskin_weight++;
            }
        }
        mesh_command.mesh.WriteBuffer(device);
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

    /// 選択頂点id
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

    public static Matrix[] ClipBoneMatrices(Figure fig, TSOMesh mesh)
    {
        Matrix[] clipped_boneMatrices = new Matrix[mesh.maxPalettes];

        for (int numPalettes = 0; numPalettes < mesh.maxPalettes; numPalettes++)
        {
            TSONode tso_node = mesh.GetBone(numPalettes);
            TMONode tmo_node;
            if (fig.nodemap.TryGetValue(tso_node, out tmo_node))
                clipped_boneMatrices[numPalettes] = tso_node.OffsetMatrix * tmo_node.combined_matrix;
        }
        return clipped_boneMatrices;
    }

    /// スクリーン座標から頂点を見つけます。
    /// 衝突する頂点の中で最も近い位置にある頂点を返します。
    private int FindVertexOnScreenPoint(float x, float y, Figure fig, TSOMesh mesh)
    {
        int vertex_id = -1;

        Matrix[] clipped_boneMatrices = ClipBoneMatrices(fig, mesh);

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
