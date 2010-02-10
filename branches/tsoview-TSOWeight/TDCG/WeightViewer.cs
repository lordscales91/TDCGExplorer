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

    public TSOFrame selected_frame = null;

    void DrawFigureVertices()
    {
        Figure fig;
        if (TryGetFigure(out fig))
        {
            if (selected_frame != null)
            {
                foreach (TSOMesh mesh in selected_frame.meshes)
                    DrawVertices(fig, mesh);
                if (selected_vertex_mesh != null)
                    DrawSelectedVertex(fig, selected_vertex_mesh);
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

    void DrawSelectedVertex(Figure fig, TSOMesh mesh)
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
        Vector4 color = new Vector4(1, 1, 0, 1);

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

    int selected_vertex_id = -1;
    TSOMesh selected_vertex_mesh = null;

    private void SelectVertex()
    {
        Figure fig;
        if (TryGetFigure(out fig))
        {
            if (selected_frame != null)
            {
                foreach (TSOMesh mesh in selected_frame.meshes)
                {
                    int vertex_id = FindVertexOnScreenPoint(lastScreenPoint.X, lastScreenPoint.Y, fig, mesh);
                    if (vertex_id != -1)
                    {
                        selected_vertex_id = vertex_id;
                        selected_vertex_mesh = mesh;
                        break;
                    }
                }
            }
        }
    }

    /// �X�N���[�����W���璸�_�������܂��B
    /// �Փ˂��钸�_�̒��ōł��߂��ʒu�ɂ��钸�_��Ԃ��܂��B
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
