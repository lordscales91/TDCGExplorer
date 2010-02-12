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
        /// ���_id
        public int vertex_id;
        /// �X�L���E�F�C�g���샊�X�g
        public List<SkinWeightCommand> skin_weight_commands = new List<SkinWeightCommand>();
    }
    /// ���b�V������
    public class MeshCommand
    {
        /// ���b�V��
        public TSOMesh mesh = null;
        /// ���_���샊�X�g
        public List<VertexCommand> vertex_commands = new List<VertexCommand>();
    }

    /// <summary>
    /// TSOFile��Direct3D��Ń����_�����O���܂��B
    /// </summary>
public class WeightViewer : Viewer
{
    internal Mesh sphere = null;

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

        return true;
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
            if (selected_mesh != null)
            {
                DrawVertices(fig, selected_mesh);
                if (selected_vertex_id != -1)
                    DrawSelectedVertex(fig);
            }
        }
    }

    /// ���a
    public static float radius = 0.5f;

    /// ���_��`�悷��B
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
                //���_�ԋ��������a�����Ȃ物�F�ɂ���B
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

    /// �I�𒸓_��`�悷��B
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

    /// �I���{�[���ɑΉ�����E�F�C�g�����Z����B
    public void GainSkinWeight(TSONode selected_node)
    {
        if (selected_mesh != null)
        {
            GainSkinWeight(selected_mesh, selected_node);
        }
    }

    /// �I���{�[���ɑΉ�����E�F�C�g�����Z����B
    public void GainSkinWeight(TSOMesh mesh, TSONode selected_node)
    {
        if (selected_vertex_id == -1)
            return;

        //����𐶐�����B
        MeshCommand mesh_command = new MeshCommand();
        mesh_command.mesh = mesh;

        bool updated = false;
        Vector3 p0 = selected_mesh.vertices[selected_vertex_id].position;

        for (int i = 0; i < mesh.vertices.Length; i++)
        {
            Vertex v = mesh.vertices[i];

            //���_�ԋ��������a�����Ȃ�E�F�C�g�����Z����B
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

    /// ���Z�E�F�C�g�l
    public static float weight = 0.2f;
    /// ���b�V�����샊�X�g
    public static List<MeshCommand> mesh_commands = new List<MeshCommand>();
    static int mesh_command_id = 0;

    /// �I���{�[���ɑΉ�����E�F�C�g�����Z����B
    /// returns: �E�F�C�g��ύX������
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
            TSONode bone = mesh.GetBone(skin_weight.bone_index);
            if (bone == selected_node)
            {
                selected_skin_weight = skin_weight;
                break;
            }
        }
        //�I���{�[���ɑΉ�����E�F�C�g���Ȃ���΁A�ŏ��l�����E�F�C�g��u��������B
        if (selected_skin_weight == null)
        {
            //���b�V���̃{�[���Q�ƂɎw��m�[�h���܂܂�邩�B
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

    /// �}�E�X�{�^�����������Ƃ��Ɏ��s����n���h��
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

    /// �I�𒸓_id
    public int selected_vertex_id = -1;

    /// <summary>
    /// ���_�I�����ɌĂяo�����n���h��
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

    /// �X�N���[�����W���璸�_�������܂��B
    /// �Փ˂��钸�_�̒��ōł��߂��ʒu�ɂ��钸�_��Ԃ��܂��B
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
    /// ����object��j�����܂��B
    /// </summary>
    public new void Dispose()
    {
        if (sphere != null)
            sphere.Dispose();
        base.Dispose();
    }
}
}
