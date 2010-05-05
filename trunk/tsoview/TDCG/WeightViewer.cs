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
    /// �X�L���E�F�C�g����
    public struct SkinWeightAttr
    {
        /// �{�[���Q�ƃC���f�b�N�X
        public int bone_index;
        /// �E�F�C�g�l
        public float weight;
    }
    /// �X�L���E�F�C�g����
    public class SkinWeightCommand
    {
        /// �X�L���E�F�C�g
        public SkinWeight skin_weight = null;
        /// �ύX�O�̑���
        public SkinWeightAttr old_attr;
        /// �ύX��̑���
        public SkinWeightAttr new_attr;
    }
    /// ���_����
    public class VertexCommand
    {
        /// ���_
        public Vertex vertex;
        /// �X�L���E�F�C�g���샊�X�g
        public List<SkinWeightCommand> skin_weight_commands = new List<SkinWeightCommand>();
    }
    /// �T�u���b�V������
    public class SubMeshCommand
    {
        /// �T�u���b�V��
        public TSOSubMesh sub_mesh = null;
        /// ���_���샊�X�g
        public List<VertexCommand> vertex_commands = new List<VertexCommand>();
    }
    /// ���b�V������
    public class MeshCommand
    {
        /// ���b�V��
        public TSOMesh mesh = null;
        /// �T�u���b�V�����샊�X�g
        public List<SubMeshCommand> sub_mesh_commands = new List<SubMeshCommand>();
    }

    /// <summary>
    /// TSOFile��Direct3D��Ń����_�����O���܂��B
    /// </summary>
public class WeightViewer : Viewer
{
    internal Mesh sphere = null;
    internal Texture dot_texture = null;

    /// <summary>
    /// viewer�𐶐����܂��B
    /// </summary>
    public WeightViewer()
    {
        this.Rendering += delegate()
        {
            RenderDerived();
        };
    }

    /// ���ƃ��C�̏Փ˂������܂��B
    public bool DetectSphereRayCollision(float sphereRadius, ref Vector3 sphereCenter, ref Vector3 rayStart, ref Vector3 rayOrientation, out Vector3 collisionPoint, out float collisionTime)
    {
        collisionTime = 0.0f;
        collisionPoint = Vector3.Empty;

        Vector3 u = rayStart - sphereCenter;
        float a = Vector3.Dot(rayOrientation, rayOrientation);
        float b = Vector3.Dot(rayOrientation, u);
        float c = Vector3.Dot(u, u) - sphereRadius*sphereRadius;
        if (a <= float.Epsilon)
            //�덷
            return false;
        float d = b*b - a*c;
        if (d < 0.0f)
            //�Փ˂��Ȃ�
            return false;
        collisionTime = (-b - (float)Math.Sqrt(d))/a;
        collisionPoint = rayStart + rayOrientation*collisionTime;
        return true;
    }

    /// <summary>
    /// viewport�s����쐬���܂��B
    /// </summary>
    /// <param name="viewport">viewport</param>
    /// <returns>viewport�s��</returns>
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

    /// �X�N���[���ʒu�����[���h���W�֕ϊ����܂��B
    public Vector3 ScreenToWorld(float screenX, float screenY, float z, Viewport viewport, Matrix view, Matrix proj)
    {
        //�X�N���[���ʒu
        Vector3 v = new Vector3(screenX, screenY,  z);

        Matrix inv_m = Matrix.Invert(CreateViewportMatrix(viewport));
        Matrix inv_proj = Matrix.Invert(proj);
        Matrix inv_view = Matrix.Invert(view);

        //�X�N���[���ʒu�����[���h���W�֕ϊ�
        return Vector3.TransformCoordinate(v, inv_m * inv_proj * inv_view);
    }

    /// �X�N���[���ʒu�����[���h���W�֕ϊ����܂��B
    public Vector3 ScreenToWorld(float screenX, float screenY, float z)
    {
        return ScreenToWorld(screenX, screenY, z, device.Viewport, Transform_View, Transform_Projection);
    }

    /// ���[���h���W���X�N���[���ʒu�֕ϊ����܂��B
    public Vector3 WorldToScreen(Vector3 v, Viewport viewport, Matrix view, Matrix proj)
    {
        return Vector3.TransformCoordinate(v, view * proj * CreateViewportMatrix(viewport));
    }

    /// ���[���h���W���X�N���[���ʒu�֕ϊ����܂��B
    public Vector3 WorldToScreen(Vector3 v)
    {
        return WorldToScreen(v, device.Viewport, Transform_View, Transform_Projection);
    }

    /// get path to dot.bmp
    public static string GetDotBitmapPath()
    {
        return Path.Combine(Application.StartupPath, @"dot.bmp");
    }

    /// <summary>
    /// device���쐬���܂��B
    /// </summary>
    /// <param name="control">�����_�����O��ƂȂ�control</param>
    /// <returns>device�̍쐬�ɐ���������</returns>
    public new bool InitializeApplication(Control control)
    {
        return InitializeApplication(control, false);
    }

    /// <summary>
    /// device���쐬���܂��B
    /// </summary>
    /// <param name="control">�����_�����O��ƂȂ�control</param>
    /// <param name="shadowMapEnabled">�V���h�E�}�b�v���쐬���邩</param>
    /// <returns>device�̍쐬�ɐ���������</returns>
    public new bool InitializeApplication(Control control, bool shadowMapEnabled)
    {
        if (! base.InitializeApplication(control, shadowMapEnabled))
            return false;

        sphere = Mesh.Sphere(device, 0.25f, 4, 2);
        dot_texture = TextureLoader.FromFile(device, GetDotBitmapPath());

        return true;
    }

    /// <summary>
    /// �`�惂�[�h
    /// </summary>
    public enum ViewMode
    {
        /// <summary>
        /// �g�D�[���`��
        /// </summary>
        Toon,
        /// <summary>
        /// �E�F�C�g�`��
        /// </summary>
        Weight
    };
    /// <summary>
    /// �`�惂�[�h
    /// </summary>
    public ViewMode view_mode = ViewMode.Toon;

    public enum MeshSelectionMode
    {
        AllMeshes, SelectedMesh
    }
    public MeshSelectionMode mesh_selection_mode = MeshSelectionMode.AllMeshes;

    /// <summary>
    /// �t�B�M���A��`�悵�܂��B
    /// </summary>
    protected override void DrawFigure()
    {
        device.RenderState.AlphaBlendEnable = true;

        device.SetRenderTarget(0, dev_surface);
        device.DepthStencilSurface = dev_zbuf;
        device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.LightGray, 1.0f, 0);

        Figure fig;
        if (TryGetFigure(out fig))
        {
            switch (view_mode)
            {
                case ViewMode.Toon:
                    switch (mesh_selection_mode)
                    {
                        case MeshSelectionMode.AllMeshes:
                            {
                                foreach (TSOFile tso in fig.TSOList)
                                {
                                    tso.BeginRender();
                                    foreach (TSOMesh mesh in tso.meshes)
                                        foreach (TSOSubMesh sub_mesh in mesh.sub_meshes)
                                            DrawSubMeshForToonRendering(fig, tso, sub_mesh);
                                    tso.EndRender();
                                }
                            }
                            break;
                        case MeshSelectionMode.SelectedMesh:
                            if (SelectedTSOFile != null && SelectedMesh != null)
                            {
                                {
                                    TSOFile tso = SelectedTSOFile;
                                    tso.BeginRender();
                                    foreach (TSOSubMesh sub_mesh in SelectedMesh.sub_meshes)
                                        DrawSubMeshForToonRendering(fig, tso, sub_mesh);
                                    tso.EndRender();
                                }
                            }
                            break;
                    }
                    break;
                case ViewMode.Weight:
                    switch (mesh_selection_mode)
                    {
                        case MeshSelectionMode.AllMeshes:
                            {
                                foreach (TSOFile tso in fig.TSOList)
                                {
                                    tso.BeginRender();
                                    foreach (TSOMesh mesh in tso.meshes)
                                        foreach (TSOSubMesh sub_mesh in mesh.sub_meshes)
                                            DrawSubMeshForWeightPainting(fig, sub_mesh);
                                    tso.EndRender();
                                }
                            }
                            break;
                        case MeshSelectionMode.SelectedMesh:
                            if (SelectedTSOFile != null && SelectedMesh != null)
                            {
                                TSOFile tso = SelectedTSOFile;
                                tso.BeginRender();
                                foreach (TSOSubMesh sub_mesh in SelectedMesh.sub_meshes)
                                    DrawSubMeshForWeightPainting(fig, sub_mesh);
                                tso.EndRender();
                            }
                            break;
                    }
                    break;
            }
        }
    }

    private void DrawSubMeshForToonRendering(Figure fig, TSOFile tso, TSOSubMesh sub_mesh)
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

    private void DrawSubMeshForWeightPainting(Figure fig, TSOSubMesh sub_mesh)
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

    /// <summary>
    /// �V�[���������_�����O���܂��B
    /// </summary>
    public void RenderDerived()
    {
        if (motionEnabled)
            return;

        DrawFigureVertices();
    }

    /// ���_��`�悷��B
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

    /// ���a
    public static float radius = 0.5f;

    /// ���_��`�悷��B
    void DrawVertices(Figure fig, TSOSubMesh sub_mesh)
    {
        Matrix[] clipped_boneMatrices = ClipBoneMatrices(fig, sub_mesh);

        Rectangle rect = new Rectangle(0, 0, 7, 7);//red
        Vector3 rect_center = new Vector3(3, 3, 0);

        Vector3[] view_positions = new Vector3[sub_mesh.vertices.Length];
        Vector3[] screen_positions = new Vector3[sub_mesh.vertices.Length];
        for (int i = 0; i < sub_mesh.vertices.Length; i++)
        {
            Vector3 p1 = CalcSkindeformPosition(sub_mesh.vertices[i], clipped_boneMatrices);
            view_positions[i] = Vector3.TransformCoordinate(p1, Transform_View);
            screen_positions[i] = WorldToScreen(p1);
        }
        bool[] ccws = new bool[sub_mesh.vertices.Length];
        for (int i = 2; i < sub_mesh.vertices.Length; i++)
        {
            ccws[i] = false;
        }
        for (int i = 2; i < sub_mesh.vertices.Length; i++)
        {
            int a, b, c;
            if (i % 2 != 0)
            {
                a = i-0;
                b = i-1;
                c = i-2;
            }
            else
            {
                a = i-2;
                b = i-1;
                c = i-0;
            }
            ccws[i] = ccws[i] || IsCounterClockWise(view_positions[a], view_positions[b], view_positions[c]);
        }
        ccws[0] = ccws[2];
        ccws[1] = ccws[2];

        if (selected_vertex != null)
        {
            Vector3 p0 = CalcSkindeformPosition(selected_vertex, ClipBoneMatrices(fig, selected_sub_mesh));

            sprite.Begin(SpriteFlags.None);

            for (int i = 0; i < sub_mesh.vertices.Length; i++)
            {
                if (!ccws[i])
                    continue;

                //���_�ԋ��������a�����Ȃ物�F�ɂ���B
                Vector3 p1 = CalcSkindeformPosition(sub_mesh.vertices[i], clipped_boneMatrices);
                float dx = p1.X - p0.X;
                float dy = p1.Y - p0.Y;
                float dz = p1.Z - p0.Z;
                if (dx * dx + dy * dy + dz * dz - radius * radius < float.Epsilon)
                    rect = new Rectangle(8, 8, 7, 7);//yellow
                else
                    rect = new Rectangle(0, 0, 7, 7);//red

                Vector3 p2 = WorldToScreen(p1);
                p2.Z = 0.0f;
                sprite.Draw(dot_texture, rect, rect_center, p2, Color.White);
            }
            sprite.End();
        }
        else
        {
            sprite.Begin(SpriteFlags.None);

            for (int i = 0; i < sub_mesh.vertices.Length; i++)
            {
                if (!ccws[i])
                    continue;

                Vector3 p2 = screen_positions[i];
                p2.Z = 0.0f;
                sprite.Draw(dot_texture, rect, rect_center, p2, Color.White);
            }
            sprite.End();
        }
    }

    bool IsCounterClockWise(float x0, float y0, float x1, float y1, float x2, float y2)
    {
        return (x2-x0) * (y1-y0) - (y2-y0) * (x1-x0) < 0.0f;
    }

    bool IsCounterClockWise(Vector3 p0, Vector3 p1, Vector3 p2)
    {
        return IsCounterClockWise(p0.X, p0.Y, p1.X, p1.Y, p2.X, p2.Y);
    }

    /// �I�𒸓_��`�悷��B
    void DrawSelectedVertex(Figure fig)
    {
        if (selected_vertex == null)
            return;

        Matrix[] clipped_boneMatrices = ClipBoneMatrices(fig, SelectedSubMesh);

        Rectangle rect = new Rectangle(8, 0, 7, 7);//green
        Vector3 rect_center = new Vector3(3, 3, 0);

        sprite.Begin(SpriteFlags.None);
        {
            Vector3 p1 = CalcSkindeformPosition(selected_vertex, clipped_boneMatrices);
            Vector3 p2 = WorldToScreen(p1);
            p2.Z = 0.0f;
            sprite.Draw(dot_texture, rect, rect_center, p2, Color.White);
        }
        sprite.End();
    }

    /// �I���{�[���ɑΉ�����E�F�C�g�����Z����B
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

    /// �I���{�[���ɑΉ�����E�F�C�g�����Z����B
    public bool GainSkinWeight(Figure fig, TSOSubMesh sub_mesh, TSONode selected_node, MeshCommand mesh_command)
    {
        bool updated = false;

        //����𐶐�����B
        SubMeshCommand sub_mesh_command = new SubMeshCommand();
        sub_mesh_command.sub_mesh = sub_mesh;

        Matrix[] clipped_boneMatrices = ClipBoneMatrices(fig, sub_mesh);

        if (selected_vertex != null)
        {
            Vector3 p0 = CalcSkindeformPosition(selected_vertex, ClipBoneMatrices(fig, selected_sub_mesh));

            for (int i = 0; i < sub_mesh.vertices.Length; i++)
            {
                Vertex v = sub_mesh.vertices[i];

                //���_�ԋ��������a�����Ȃ�E�F�C�g�����Z����B
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

    /// ���Z�E�F�C�g�l
    public static float weight = 0.2f;
    /// ���b�V�����샊�X�g
    public static List<MeshCommand> mesh_commands = new List<MeshCommand>();
    static int mesh_command_id = 0;

    /// �I���{�[���ɑΉ�����E�F�C�g�����Z����B
    /// returns: �E�F�C�g��ύX������
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
        //�����O�̒l���L������B
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

        //�I���{�[���ɑΉ�����E�F�C�g����������B
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
        //�I���{�[���ɑΉ�����E�F�C�g���Ȃ���΁A�ŏ��l�����E�F�C�g��u��������B
        if (selected_skin_weight == null)
        {
            //�T�u���b�V���̃{�[���Q�ƂɎw��m�[�h���܂܂�邩�B
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
        //�I���{�[���ɑΉ�����E�F�C�g�����Z����B
        if (selected_skin_weight != null)
        {
            updated = true;

            //�ύX�O�̑ΏۃE�F�C�g�l
            float prev_selected_weight = selected_skin_weight.weight;
            //�ύX�O�̎c��E�F�C�g�l
            float prev_rest_weight = 1.0f - prev_selected_weight;
            //�E�F�C�g�����Z����B
            selected_skin_weight.weight += weight;
            if (selected_skin_weight.weight > 1.0f)
                selected_skin_weight.weight = 1.0f;

            if (prev_rest_weight != 0.0f)
            {
                //���ۂ̉��Z�l
                float gain_weight = selected_skin_weight.weight - prev_selected_weight;
                //�c��E�F�C�g�����Z����B
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
        //������̒l���L������B
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

    /// ������������܂��B
    public void ClearCommands()
    {
        mesh_commands.Clear();
        mesh_command_id = 0;
    }

    /// �ЂƂO�̑���ɂ��ύX�����ɖ߂��邩�B
    public bool CanUndo()
    {
        return (mesh_command_id > 0);
    }
    /// �ЂƂO�̑���ɂ��ύX�����ɖ߂��B
    public void Undo()
    {
        if (!CanUndo())
            return;

        mesh_command_id--;
        Undo(mesh_commands[mesh_command_id]);
    }
    /// �w�葀��ɂ��ύX�����ɖ߂��B
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

    /// �ЂƂO�̑���ɂ��ύX����蒼���邩�B
    public bool CanRedo()
    {
        return (mesh_command_id < mesh_commands.Count);
    }
    /// �ЂƂO�̑���ɂ��ύX����蒼���B
    public void Redo()
    {
        if (!CanRedo())
            return;

        Redo(mesh_commands[mesh_command_id]);
        mesh_command_id++;
    }
    /// �w�葀��ɂ��ύX����蒼���B
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
    /// �X�L���ό`��̎w�蒸�_�̈ʒu�𓾂܂��B
    /// </summary>
    /// <param name="v">���_</param>
    /// <param name="boneMatrices">�X�L���ό`�s��̔z��</param>
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

    /// �}�E�X�{�^�����������Ƃ��Ɏ��s����n���h��
    protected override void form_OnMouseDown(object sender, MouseEventArgs e)
    {
        switch (e.Button)
        {
        case MouseButtons.Left:
            if (Control.ModifierKeys == Keys.Control)
            {
                lightDir = ScreenToOrientation(e.X, e.Y);
                control.Invalidate(false);
            }
            else
                if (!motionEnabled)
                {
                    SelectVertex();
                    control.Invalidate(false);
                }
            break;
        }

        lastScreenPoint.X = e.X;
        lastScreenPoint.Y = e.Y;
    }

    /// �}�E�X���ړ������Ƃ��Ɏ��s����n���h��
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
                Camera.Move(dx, -dy, 0.0f);
            control.Invalidate(false);
            break;
        case MouseButtons.Middle:
            Camera.MoveView(-dx*0.125f, dy*0.125f);
            control.Invalidate(false);
            break;
        case MouseButtons.Right:
            Camera.Move(0.0f, 0.0f, -dy*0.125f);
            control.Invalidate(false);
            break;
        }

        lastScreenPoint.X = e.X;
        lastScreenPoint.Y = e.Y;
    }

    TSOFile selected_tso_file = null;

    public TSOFile SelectedTSOFile
    {
        get { return selected_tso_file; }
        set
        {
            selected_tso_file = value;
            selected_mesh = null;
            selected_sub_mesh = null;
            selected_vertex = null;
        }
    }

    TSOMesh selected_mesh = null;
    /// �I�����b�V��
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
    /// �I���T�u���b�V��
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
    /// �I���{�[��
    public TSONode SelectedNode { get { return selected_node; } set { selected_node = value; } }

    Vertex selected_vertex = null;
    /// �I�𒸓_id
    public Vertex SelectedVertex
    {
        get { return selected_vertex; }
        set
        {
            selected_vertex = value;
        }
    }

    /// <summary>
    /// �T�u���b�V���I�����ɌĂяo�����n���h��
    /// </summary>
    public event EventHandler SubMeshEvent;

    /// <summary>
    /// ���_�I�����ɌĂяo�����n���h��
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

    /// �X�N���[�����W���璸�_�������܂��B
    /// �Փ˂��钸�_�̒��ōł��߂��ʒu�ɂ��钸�_��Ԃ��܂��B
    private Vertex FindVertexOnScreenPoint(float x, float y, Figure fig, TSOSubMesh sub_mesh)
    {
        Vertex vertex = null;

        Matrix[] clipped_boneMatrices = ClipBoneMatrices(fig, sub_mesh);

        {
            int width = 3;
            float min_z = 1e12f;

            for (int i = 0; i < sub_mesh.vertices.Length; i++)
            {
                Vector3 p1 = CalcSkindeformPosition(sub_mesh.vertices[i], clipped_boneMatrices);
                Vector3 p2 = WorldToScreen(p1);
                if (p2.X - width <= x && x <= p2.X + width && p2.Y - width <= y && y <= p2.Y + width)
                {
                    if (p2.Z < min_z)
                    {
                        min_z = p2.Z;
                        vertex = sub_mesh.vertices[i];
                    }
                }
            }
        }

        return vertex;
    }

    /// <summary>
    /// ����object��j�����܂��B
    /// </summary>
    public new void Dispose()
    {
        if (dot_texture != null)
            dot_texture.Dispose();
        if (sphere != null)
            sphere.Dispose();
        base.Dispose();
    }
}
}