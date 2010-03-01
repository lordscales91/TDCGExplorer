using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
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
        /// 頂点
        public Vertex vertex;
        /// スキンウェイト操作リスト
        public List<SkinWeightCommand> skin_weight_commands = new List<SkinWeightCommand>();
    }
    /// サブメッシュ操作
    public class SubMeshCommand
    {
        /// サブメッシュ
        public TSOSubMesh sub_mesh = null;
        /// 頂点操作リスト
        public List<VertexCommand> vertex_commands = new List<VertexCommand>();
    }
    /// メッシュ操作
    public class MeshCommand
    {
        /// メッシュ
        public TSOMesh mesh = null;
        /// サブメッシュ操作リスト
        public List<SubMeshCommand> sub_mesh_commands = new List<SubMeshCommand>();
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

    /// ワールド座標をスクリーン位置へ変換します。
    public Vector3 WorldToScreen(Vector3 v, Viewport viewport, Matrix view, Matrix proj)
    {
        return Vector3.TransformCoordinate(v, view * proj * CreateViewportMatrix(viewport));
    }

    /// ワールド座標をスクリーン位置へ変換します。
    public Vector3 WorldToScreen(Vector3 v)
    {
        return WorldToScreen(v, device.Viewport, Transform_View, Transform_Projection);
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
    /// 描画モード
    /// </summary>
    public enum ViewMode
    {
        /// <summary>
        /// トゥーン描画
        /// </summary>
        Toon,
        /// <summary>
        /// ウェイト描画
        /// </summary>
        Weight
    };
    /// <summary>
    /// 描画モード
    /// </summary>
    public ViewMode view_mode = ViewMode.Toon;

    /// <summary>
    /// フィギュアを描画します。
    /// </summary>
    protected override void DrawFigure()
    {
        device.RenderState.AlphaBlendEnable = true;

        device.SetRenderTarget(0, dev_surface);
        device.DepthStencilSurface = dev_zbuf;
        device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.LightGray, 1.0f, 0);

        if (ShadowMapEnabled)
        {
            effect.SetValue("texShadowMap", renderTextures[2]);
        }

        switch (view_mode)
        {
            case ViewMode.Toon:
                foreach (Figure fig in FigureList)
                    foreach (TSOFile tso in fig.TSOList)
                    {
                        tso.BeginRender();

                        foreach (TSOMesh mesh in tso.meshes)
                            foreach (TSOSubMesh sub_mesh in mesh.sub_meshes)
                            {
                                device.RenderState.VertexBlend = (VertexBlend)(4 - 1);
                                tso.SwitchShader(sub_mesh);

                                effect.SetValue(handle_LocalBoneMats, ClipBoneMatrices(fig, sub_mesh));

                                int npass = effect.Begin(0);
                                for (int ipass = 0; ipass < npass; ipass++)
                                {
                                    effect.BeginPass(ipass);
                                    sub_mesh.dm.DrawSubset(0);
                                    effect.EndPass();
                                }
                                effect.End();
                            }
                        tso.EndRender();
                    }
                break;
            case ViewMode.Weight:
                if (SelectedMesh != null)
                {
                    Figure fig;
                    if (TryGetFigure(out fig))
                    {
                        foreach (TSOSubMesh sub_mesh in SelectedMesh.sub_meshes)
                        {
                            device.RenderState.VertexBlend = (VertexBlend)(4 - 1);
                            effect.Technique = "BoneCol";
                            effect.SetValue("PenColor", new Vector4(1, 1, 1, 1));

                            effect.SetValue(handle_LocalBoneMats, ClipBoneMatrices(fig, sub_mesh));
                            effect.SetValue(handle_LocalBoneSels, ClipBoneSelections(fig, sub_mesh, selected_node));

                            int npass = effect.Begin(0);
                            for (int ipass = 0; ipass < npass; ipass++)
                            {
                                effect.BeginPass(ipass);
                                sub_mesh.dm.DrawSubset(0);
                                effect.EndPass();
                            }
                            effect.End();
                        }
                    }
                }
                break;
        }
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
            if (SelectedMesh != null)
            {
                foreach (TSOSubMesh sub_mesh in SelectedMesh.sub_meshes)
                {
                    DrawVertices(fig, sub_mesh);
                }
                DrawSelectedVertex(fig);
            }
        }
    }

    /// 半径
    public static float radius = 0.5f;

    /// 頂点を描画する。
    void DrawVertices(Figure fig, TSOSubMesh sub_mesh)
    {
        Matrix[] clipped_boneMatrices = ClipBoneMatrices(fig, sub_mesh);

        int width = 2;
        Color color = Color.Red;

        if (selected_vertex != null)
        {
            Vector3 p0 = CalcSkindeformPosition(selected_vertex, ClipBoneMatrices(fig, selected_sub_mesh));

            for (int i = 0; i < sub_mesh.vertices.Length; i++)
            {
                //頂点間距離が半径未満なら黄色にする。
                Vector3 p1 = CalcSkindeformPosition(sub_mesh.vertices[i], clipped_boneMatrices);
                float dx = p1.X - p0.X;
                float dy = p1.Y - p0.Y;
                float dz = p1.Z - p0.Z;
                if (dx * dx + dy * dy + dz * dz - radius * radius < float.Epsilon)
                    color = Color.Yellow;
                else
                    color = Color.Red;

                Vector3 p2 = WorldToScreen(p1);
                Vector2[] positions = new Vector2[5];
                positions[0] = new Vector2(p2.X - width, p2.Y - width);
                positions[1] = new Vector2(p2.X + width, p2.Y - width);
                positions[2] = new Vector2(p2.X + width, p2.Y + width);
                positions[3] = new Vector2(p2.X - width, p2.Y + width);
                positions[4] = new Vector2(p2.X - width, p2.Y - width);
                line.Draw(positions, color);
            }
        }
        else
        {
            for (int i = 0; i < sub_mesh.vertices.Length; i++)
            {
                Vector3 p1 = CalcSkindeformPosition(sub_mesh.vertices[i], clipped_boneMatrices);
                Vector3 p2 = WorldToScreen(p1);
                Vector2[] positions = new Vector2[5];
                positions[0] = new Vector2(p2.X - width, p2.Y - width);
                positions[1] = new Vector2(p2.X + width, p2.Y - width);
                positions[2] = new Vector2(p2.X + width, p2.Y + width);
                positions[3] = new Vector2(p2.X - width, p2.Y + width);
                positions[4] = new Vector2(p2.X - width, p2.Y - width);
                line.Draw(positions, color);
            }
        }
    }

    /// 選択頂点を描画する。
    void DrawSelectedVertex(Figure fig)
    {
        if (selected_vertex == null)
            return;

        Matrix[] clipped_boneMatrices = ClipBoneMatrices(fig, SelectedSubMesh);

        float scale = 0.1f;
        Vector4 color = new Vector4(0, 1, 0, 1);

        {
            Vector3 pos = CalcSkindeformPosition(selected_vertex, clipped_boneMatrices);
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
        Figure fig;
        if (TryGetFigure(out fig))
        {
            if (SelectedMesh != null)
            {
                MeshCommand mesh_command = new MeshCommand();
                mesh_command.mesh = SelectedMesh;
                bool updated = false;
                foreach (TSOSubMesh sub_mesh in SelectedMesh.sub_meshes)
                {
                    if (GainSkinWeight(fig, sub_mesh, selected_node, mesh_command))
                        updated = true;
                }
                if (updated)
                {
                    if (mesh_command_id == mesh_commands.Count)
                        mesh_commands.Add(mesh_command);
                    else
                        mesh_commands[mesh_command_id] = mesh_command;
                    mesh_command_id++;
                }
            }
        }
    }

    /// 選択ボーンに対応するウェイトを加算する。
    public bool GainSkinWeight(Figure fig, TSOSubMesh sub_mesh, TSONode selected_node, MeshCommand mesh_command)
    {
        bool updated = false;

        //操作を生成する。
        SubMeshCommand sub_mesh_command = new SubMeshCommand();
        sub_mesh_command.sub_mesh = sub_mesh;

        Matrix[] clipped_boneMatrices = ClipBoneMatrices(fig, sub_mesh);

        if (selected_vertex != null)
        {
            Vector3 p0 = CalcSkindeformPosition(selected_vertex, ClipBoneMatrices(fig, selected_sub_mesh));

            for (int i = 0; i < sub_mesh.vertices.Length; i++)
            {
                Vertex v = sub_mesh.vertices[i];

                //頂点間距離が半径未満ならウェイトを加算する。
                Vector3 p1 = CalcSkindeformPosition(v, clipped_boneMatrices);
                float dx = p1.X - p0.X;
                float dy = p1.Y - p0.Y;
                float dz = p1.Z - p0.Z;
                if (dx * dx + dy * dy + dz * dz - radius * radius < float.Epsilon)
                {
                    if (GainSkinWeight(sub_mesh, selected_node, v, sub_mesh_command))
                    {
                        updated = true;

                        v.FillSkinWeights();
                        v.GenerateBoneIndices();
                    }
                }
            }
        }

        if (updated)
            sub_mesh.WriteBuffer(device);
        if (updated)
            mesh_command.sub_mesh_commands.Add(sub_mesh_command);

        return updated;
    }

    /// 加算ウェイト値
    public static float weight = 0.2f;
    /// メッシュ操作リスト
    public static List<MeshCommand> mesh_commands = new List<MeshCommand>();
    static int mesh_command_id = 0;

    /// 選択ボーンに対応するウェイトを加算する。
    /// returns: ウェイトを変更したか
    public static bool GainSkinWeight(TSOSubMesh sub_mesh, TSONode selected_node, Vertex v, SubMeshCommand sub_mesh_command)
    {
        VertexCommand vertex_command = new VertexCommand();
        vertex_command.vertex = v;
        foreach (SkinWeight skin_weight in v.skin_weights)
        {
            SkinWeightCommand skin_weight_command = new SkinWeightCommand();
            skin_weight_command.skin_weight = skin_weight;
            vertex_command.skin_weight_commands.Add(skin_weight_command);
        }
        sub_mesh_command.vertex_commands.Add(vertex_command);
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
            TSONode bone = sub_mesh.GetBone(skin_weight.bone_index);
            if (bone == selected_node)
            {
                selected_skin_weight = skin_weight;
                break;
            }
        }
        //選択ボーンに対応するウェイトがなければ、最小値を持つウェイトを置き換える。
        if (selected_skin_weight == null)
        {
            //サブメッシュのボーン参照に指定ノードが含まれるか。
            bool found = false;
            int bone_index = 0;
            foreach (TSONode bone in sub_mesh.bones)
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

    /// 操作を消去します。
    public void ClearCommands()
    {
        mesh_commands.Clear();
        mesh_command_id = 0;
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
        foreach (SubMeshCommand sub_mesh_command in mesh_command.sub_mesh_commands)
        {
            foreach (VertexCommand vertex_command in sub_mesh_command.vertex_commands)
            {
                Vertex v = vertex_command.vertex;
                int nskin_weight = 0;
                foreach (SkinWeightCommand skin_weight_command in vertex_command.skin_weight_commands)
                {
                    v.skin_weights[nskin_weight].bone_index = skin_weight_command.old_attr.bone_index;
                    v.skin_weights[nskin_weight].weight = skin_weight_command.old_attr.weight;
                    nskin_weight++;
                }
                v.FillSkinWeights();
                v.GenerateBoneIndices();
            }
            sub_mesh_command.sub_mesh.WriteBuffer(device);
        }
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
        foreach (SubMeshCommand sub_mesh_command in mesh_command.sub_mesh_commands)
        {
            foreach (VertexCommand vertex_command in sub_mesh_command.vertex_commands)
            {
                Vertex v = vertex_command.vertex;
                int nskin_weight = 0;
                foreach (SkinWeightCommand skin_weight_command in vertex_command.skin_weight_commands)
                {
                    v.skin_weights[nskin_weight].bone_index = skin_weight_command.new_attr.bone_index;
                    v.skin_weights[nskin_weight].weight = skin_weight_command.new_attr.weight;
                    nskin_weight++;
                }
                v.FillSkinWeights();
                v.GenerateBoneIndices();
            }
            sub_mesh_command.sub_mesh.WriteBuffer(device);
        }
    }

    /// <summary>
    /// スキン変形後の指定頂点の位置を得ます。
    /// </summary>
    /// <param name="v">頂点</param>
    /// <param name="boneMatrices">スキン変形行列の配列</param>
    /// <returns></returns>
    public static Vector3 CalcSkindeformPosition(Vertex v, Matrix[] boneMatrices)
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

    TSOMesh selected_mesh = null;
    /// 選択メッシュ
    public TSOMesh SelectedMesh
    {
        get { return selected_mesh; }
        set
        {
            selected_mesh = value;
            selected_sub_mesh = null;
            selected_vertex = null;
        }
    }

    TSOSubMesh selected_sub_mesh = null;
    /// 選択サブメッシュ
    public TSOSubMesh SelectedSubMesh
    {
        get { return selected_sub_mesh; }
        set
        {
            selected_sub_mesh = value;
            selected_vertex = null;
        }
    }

    TSONode selected_node = null;
    /// 選択ボーン
    public TSONode SelectedNode { get { return selected_node; } set { selected_node = value; } }

    Vertex selected_vertex = null;
    /// 選択頂点id
    public Vertex SelectedVertex
    {
        get { return selected_vertex; }
        set
        {
            selected_vertex = value;
        }
    }

    /// <summary>
    /// サブメッシュ選択時に呼び出されるハンドラ
    /// </summary>
    public event EventHandler SubMeshEvent;

    /// <summary>
    /// 頂点選択時に呼び出されるハンドラ
    /// </summary>
    public event EventHandler VertexEvent;

    private void SelectVertex()
    {
        Figure fig;
        if (TryGetFigure(out fig))
        {
            if (SelectedMesh != null)
            {
                foreach (TSOSubMesh sub_mesh in SelectedMesh.sub_meshes)
                {
                    Vertex vertex = FindVertexOnScreenPoint(lastScreenPoint.X, lastScreenPoint.Y, fig, sub_mesh);
                    if (vertex != null)
                    {
                        selected_sub_mesh = sub_mesh;
                        if (SubMeshEvent != null)
                            SubMeshEvent(this, EventArgs.Empty);
                        selected_vertex = vertex;
                        if (VertexEvent != null)
                            VertexEvent(this, EventArgs.Empty);
                        break;
                    }
                }
            }
        }
    }

    /// スクリーン座標から頂点を見つけます。
    /// 衝突する頂点の中で最も近い位置にある頂点を返します。
    private Vertex FindVertexOnScreenPoint(float x, float y, Figure fig, TSOSubMesh sub_mesh)
    {
        Vertex vertex = null;

        Matrix[] clipped_boneMatrices = ClipBoneMatrices(fig, sub_mesh);

        {
            Vector3 collisionPoint;
            float collisionTime;
            float min_time = 1e12f;

            float sphereRadius = 0.25f * 0.1f;
            Vector3 rayStart = ScreenToWorld(x, y, 0.0f);
            Vector3 rayEnd = ScreenToWorld(x, y, 1.0f);
            Vector3 rayOrientation = rayEnd - rayStart;

            for (int i = 0; i < sub_mesh.vertices.Length; i++)
            {
                Vector3 sphereCenter = CalcSkindeformPosition(sub_mesh.vertices[i], clipped_boneMatrices);
                if (DetectSphereRayCollision(sphereRadius, ref sphereCenter, ref rayStart, ref rayOrientation, out collisionPoint, out collisionTime))
                {
                    if (collisionTime < min_time)
                    {
                        min_time = collisionTime;
                        vertex = sub_mesh.vertices[i];
                    }
                }
            }
        }

        return vertex;
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
