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
        }

        /// マウスボタンを押したときに実行するハンドラ
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
                        if (!MotionEnabled)
                        {
                            //if (!SelectVertex())
                            //    SelectNode();
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
                        lightDir = ScreenToOrientation(e.X, e.Y);
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
