using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq;
using System.Text;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using Direct3D = Microsoft.DirectX.Direct3D;

namespace TDCG
{
    public class EditorViewer : Viewer
    {
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
    }
}
