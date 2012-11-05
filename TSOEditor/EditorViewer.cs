using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Text;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using Direct3D = Microsoft.DirectX.Direct3D;

namespace TDCG
{
    public class EditorViewer : Viewer
    {
        internal Texture dot_texture = null;

        /// <summary>
        /// viewerを生成します。
        /// </summary>
        public EditorViewer()
        {
            this.Rendering += delegate()
            {
                RenderDerived();
            };
            LineColor = Color.FromArgb(100, 100, 230); //from MikuMikuDance
            SelectedLineColor = Color.FromArgb(255, 0, 0); //red
        }

        /// マウスボタンを押したときに実行するハンドラ
        protected override void form_OnMouseDown(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                    if (Control.ModifierKeys == Keys.Control)
                    {
                        SetLightDirection(ScreenToOrientation(e.X, e.Y));
                        control.Invalidate(false);
                    }
                    else
                        if (!MotionEnabled)
                        {
                            SelectNode();
                            control.Invalidate(false);
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
                        Camera.Move(dx, -dy, 0.0f);
                    control.Invalidate(false);
                    break;
                case MouseButtons.Middle:
                    Camera.MoveView(-dx * 0.125f, dy * 0.125f);
                    control.Invalidate(false);
                    break;
                case MouseButtons.Right:
                    Camera.Move(0.0f, 0.0f, -dy * 0.125f);
                    control.Invalidate(false);
                    break;
            }

            lastScreenPoint.X = e.X;
            lastScreenPoint.Y = e.Y;
        }

        /// get path to dot.bmp
        public static string GetDotBitmapPath()
        {
            return Path.Combine(Application.StartupPath, @"dot.bmp");
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
            if (!base.InitializeApplication(control, shadowMapEnabled))
                return false;

            dot_texture = TextureLoader.FromFile(device, GetDotBitmapPath());

            return true;
        }

        TSOFile selected_tso_file = null;
        TSONode selected_node = null;

        /// 選択TSOファイル
        public TSOFile SelectedTSOFile
        {
            get { return selected_tso_file; }
            set
            {
                selected_tso_file = value;
            }
        }

        /// <summary>
        /// node選択時に呼び出されるハンドラ
        /// </summary>
        public event EventHandler SelectedNodeChanged;

        /// nodeを選択します。
        /// returns: nodeを見つけたかどうか
        public bool SelectNode()
        {
            bool found = false;

            Figure fig;
            if (TryGetFigure(out fig))
            {
                if (SelectedTSOFile != null)
                {
                    //スクリーン座標からnodeを見つけます。
                    //衝突する頂点の中で最も近い位置にあるnodeを返します。

                    float x = lastScreenPoint.X;
                    float y = lastScreenPoint.Y;

                    int width = 5;//頂点ハンドルの幅
                    float min_z = 1e12f;

                    TSONode found_node = null;

                    foreach (TSONode node in SelectedTSOFile.nodes)
                    {
                        TMONode bone;
                        if (fig.nodemap.TryGetValue(node, out bone))
                        {
                            Vector3 p2 = GetNodePositionOnScreen(bone);
                            if (p2.X - width <= x && x <= p2.X + width && p2.Y - width <= y && y <= p2.Y + width)
                            {
                                if (p2.Z < min_z)
                                {
                                    min_z = p2.Z;
                                    found = true;
                                    found_node = node;
                                }
                            }
                        }
                    }

                    if (found)
                    {
                        selected_node = found_node;
                        if (SelectedNodeChanged != null)
                            SelectedNodeChanged(this, EventArgs.Empty);
                    }
                }
            }
            return found;
        }

        /// <summary>
        /// シーンをレンダリングします。
        /// </summary>
        public void RenderDerived()
        {
            if (MotionEnabled)
                return;

            Figure fig;
            if (TryGetFigure(out fig))
            {
                //nodeを描画する。
                DrawNodeTree(fig);
                DrawSelectedNode(fig);
            }
        }

        /// node line描画色
        public Color LineColor { get; set; }

        /// <summary>
        /// フィギュアに含まれるnode treeを描画する。
        /// </summary>
        /// <param name="fig"></param>
        void DrawNodeTree(Figure fig)
        {
            TMOFile tmo = fig.Tmo;

            Line line = new Line(device);
            foreach (TMONode node in tmo.nodes)
            {
                Vector3 p0 = GetNodePositionOnScreen(node);

                TMONode parent_node = node.parent;
                if (parent_node != null)
                {
                    Vector3 p1 = GetNodePositionOnScreen(parent_node);

                    Vector3 pd = p0 - p1;
                    float len = Vector3.Length(pd);
                    float scale = 4.0f / len;
                    Vector2 p3 = new Vector2(p1.X + pd.Y * scale, p1.Y - pd.X * scale);
                    Vector2 p4 = new Vector2(p1.X - pd.Y * scale, p1.Y + pd.X * scale);

                    Vector2[] vertices = new Vector2[3];
                    vertices[0] = new Vector2(p3.X, p3.Y);
                    vertices[1] = new Vector2(p0.X, p0.Y);
                    vertices[2] = new Vector2(p4.X, p4.Y);
                    line.Draw(vertices, LineColor);
                }
            }
            line.Dispose();
            line = null;

            Rectangle rect = new Rectangle(0, 16, 15, 15); //node circle
            Vector3 rect_center = new Vector3(7, 7, 0);
            sprite.Begin(SpriteFlags.None);
            foreach (TMONode node in tmo.nodes)
            {
                Vector3 p0 = GetNodePositionOnScreen(node);
                sprite.Draw(dot_texture, rect, rect_center, p0, Color.White);
            }
            sprite.End();
        }

        /// 指定行列の移動変位を得ます。
        public static Vector3 GetMatrixTranslation(ref Matrix m)
        {
            return new Vector3(m.M41, m.M42, m.M43);
        }

        Vector3 GetNodePositionOnScreen(TMONode node)
        {
            Vector3 p1 = GetMatrixTranslation(ref node.combined_matrix);
            Vector3 p2 = WorldToScreen(p1);
            p2.Z = 0.0f; //表面に固定
            return p2;
        }

        /// 選択node line描画色
        public Color SelectedLineColor { get; set; }

        /// 選択nodeを描画する。
        void DrawSelectedNode(Figure fig)
        {
            if (selected_node == null)
                return;

            TMONode bone;
            if (fig.nodemap.TryGetValue(selected_node, out bone))
            {
                Vector3 p1 = GetNodePositionOnScreen(bone);

                if (selected_node.children.Count != 0)
                {
                    TMONode child_bone;
                    if (fig.nodemap.TryGetValue(selected_node.children[0], out child_bone))
                    {
                        Line line = new Line(device);

                        Vector3 p0 = GetNodePositionOnScreen(child_bone);

                        Vector3 pd = p0 - p1;
                        float len = Vector3.Length(pd);
                        float scale = 4.0f / len;
                        Vector2 p3 = new Vector2(p1.X + pd.Y * scale, p1.Y - pd.X * scale);
                        Vector2 p4 = new Vector2(p1.X - pd.Y * scale, p1.Y + pd.X * scale);

                        Vector2[] vertices = new Vector2[3];
                        vertices[0] = new Vector2(p3.X, p3.Y);
                        vertices[1] = new Vector2(p0.X, p0.Y);
                        vertices[2] = new Vector2(p4.X, p4.Y);
                        line.Draw(vertices, SelectedLineColor);

                        line.Dispose();
                        line = null;
                    }
                }

                {
                    Vector3 px = GetNodeDirXPositionOnScreen(bone);
                    Vector3 py = GetNodeDirYPositionOnScreen(bone);
                    Vector3 pz = GetNodeDirZPositionOnScreen(bone);

                    Color line_color_x = Color.FromArgb(255, 0, 0); //R
                    Color line_color_y = Color.FromArgb(0, 255, 0); //G
                    Color line_color_z = Color.FromArgb(0, 0, 255); //B
                    Line line = new Line(device);
                    line.Width = 3;

                    Vector2[] vertices = new Vector2[2];
                    vertices[0] = new Vector2(p1.X, p1.Y);
                    vertices[1] = new Vector2(px.X, px.Y);
                    line.Draw(vertices, line_color_x);
                    vertices[1] = new Vector2(py.X, py.Y);
                    line.Draw(vertices, line_color_y);
                    vertices[1] = new Vector2(pz.X, pz.Y);
                    line.Draw(vertices, line_color_z);

                    line.Dispose();
                    line = null;
                }

                Rectangle rect = new Rectangle(16, 16, 15, 15); //node circle
                Vector3 rect_center = new Vector3(7, 7, 0);
                sprite.Begin(SpriteFlags.None);
                sprite.Draw(dot_texture, rect, rect_center, p1, Color.White);
                sprite.End();
            }
        }

        /// 指定行列のX軸先位置を得ます。
        public static Vector3 GetMatrixDirXTranslation(ref Matrix m, float len)
        {
            return new Vector3(m.M11 * len + m.M41, m.M12 * len + m.M42, m.M13 * len + m.M43);
        }

        /// 指定行列のY軸先位置を得ます。
        public static Vector3 GetMatrixDirYTranslation(ref Matrix m, float len)
        {
            return new Vector3(m.M21 * len + m.M41, m.M22 * len + m.M42, m.M23 * len + m.M43);
        }

        /// 指定行列のZ軸先位置を得ます。
        public static Vector3 GetMatrixDirZTranslation(ref Matrix m, float len)
        {
            return new Vector3(m.M31 * len + m.M41, m.M32 * len + m.M42, m.M33 * len + m.M43);
        }

        Vector3 GetNodeDirXPositionOnScreen(TMONode node)
        {
            Vector3 p1 = GetMatrixDirXTranslation(ref node.combined_matrix, 1);
            Vector3 p2 = WorldToScreen(p1);
            p2.Z = 0.0f; //表面に固定
            return p2;
        }

        Vector3 GetNodeDirYPositionOnScreen(TMONode node)
        {
            Vector3 p1 = GetMatrixDirYTranslation(ref node.combined_matrix, 1);
            Vector3 p2 = WorldToScreen(p1);
            p2.Z = 0.0f; //表面に固定
            return p2;
        }

        Vector3 GetNodeDirZPositionOnScreen(TMONode node)
        {
            Vector3 p1 = GetMatrixDirZTranslation(ref node.combined_matrix, 1);
            Vector3 p2 = WorldToScreen(p1);
            p2.Z = 0.0f; //表面に固定
            return p2;
        }

        internal void BeginNodeCommand()
        {
            throw new NotImplementedException();
        }

        internal bool HasNodeCommand()
        {
            throw new NotImplementedException();
        }

        internal void TranslateXOnScreen(int dx, int dy)
        {
            throw new NotImplementedException();
        }

        internal void TranslateYOnScreen(int dx, int dy)
        {
            throw new NotImplementedException();
        }

        internal void TranslateZOnScreen(int dx, int dy)
        {
            throw new NotImplementedException();
        }

        internal void RotateXOnScreen(int dx, int dy)
        {
            throw new NotImplementedException();
        }

        internal void RotateYOnScreen(int dx, int dy)
        {
            throw new NotImplementedException();
        }

        internal void RotateZOnScreen(int dx, int dy)
        {
            throw new NotImplementedException();
        }

        internal void EndNodeCommand()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 内部objectを破棄します。
        /// </summary>
        public new void Dispose()
        {
            if (dot_texture != null)
                dot_texture.Dispose();
            base.Dispose();
        }
    }
}
