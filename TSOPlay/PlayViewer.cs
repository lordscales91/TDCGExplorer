using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Text;
using TDCG;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using Direct3D = Microsoft.DirectX.Direct3D;

namespace TSOPlay
{
    class PlayViewer : Viewer
    {
        Texture tra_tex = null;
        Surface tra_surface = null;
        Sprite tra_sprite = null;

        public PlayViewer()
        {
            this.Rendering += delegate()
            {
                RenderDerived();
            };
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

            device.DeviceLost += new EventHandler(OnDeviceLost);
            device.DeviceReset += new EventHandler(OnDeviceReset);
            OnDeviceReset(this, null);

            return true;
        }
        
        private void OnDeviceLost(object sender, EventArgs e)
        {
            Console.WriteLine("PlayViewer OnDeviceLost");
            if (tra_surface != null)
                tra_surface.Dispose();
            if (tra_tex != null)
                tra_tex.Dispose();
        }

        float w_scale;
        float h_scale;
        Rectangle tra_rect;

        private void OnDeviceReset(object sender, EventArgs e)
        {
            Console.WriteLine("PlayViewer OnDeviceReset");
            int devw = dev_surface.Description.Width;
            int devh = dev_surface.Description.Height;
            Console.WriteLine("dev {0}x{1}", devw, devh);

            tra_tex = new Texture(device, devw, devh, 1, Usage.RenderTarget, Format.A8R8G8B8, Pool.Default);
            tra_surface = tra_tex.GetSurfaceLevel(0);

            int texw = tra_surface.Description.Width;
            int texh = tra_surface.Description.Height;
            Console.WriteLine("tra_tex {0}x{1}", texw, texh);

            w_scale = (float)devw / texw;
            h_scale = (float)devh / texh;
            tra_rect = new Rectangle(0, 0, texw, texh);

            tra_sprite = new Sprite(device);
        }

        int alpha = 0;

        /// <summary>
        /// シーンをレンダリングします。
        /// </summary>
        public void RenderDerived()
        {
            if (alpha > 0)
            {
                DrawSprite(alpha);
                alpha -= 4;
            }
        }

        public void ReadyTransition()
        {
            CopyDeviceSurface();
            alpha = 255;
        }

        void CopyDeviceSurface()
        {
            int devw = dev_surface.Description.Width;
            int devh = dev_surface.Description.Height;
            Console.WriteLine("dev {0}x{1}", devw, devh);

            Rectangle dev_rect = new Rectangle(0, 0, devw, devh);
            device.StretchRectangle(dev_surface, dev_rect, tra_surface, tra_rect, TextureFilter.None);
        }

        void DrawSprite(int alpha)
        {
            device.RenderState.AlphaBlendEnable = false;

            tra_sprite.Transform = Matrix.Scaling(w_scale, h_scale, 1.0f);

            tra_sprite.Begin(SpriteFlags.AlphaBlend);
            tra_sprite.Draw(tra_tex, tra_rect, new Vector3(0, 0, 0), new Vector3(0, 0, 0), Color.FromArgb(alpha, Color.White));
            tra_sprite.End();
        }

        /// <summary>
        /// 内部objectを破棄します。
        /// </summary>
        public new void Dispose()
        {
            if (tra_sprite != null)
                tra_sprite.Dispose();
            base.Dispose();
        }
    }
}
