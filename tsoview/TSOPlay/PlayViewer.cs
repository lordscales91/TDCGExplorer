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

        private void OnDeviceReset(object sender, EventArgs e)
        {
            Console.WriteLine("PlayViewer OnDeviceReset");
            int devw = dev_surface.Description.Width;
            int devh = dev_surface.Description.Height;
            Console.WriteLine("dev {0}x{1}", devw, devh);

            tra_tex = new Texture(device, 1024, 1024, 1, Usage.RenderTarget, Format.A8R8G8B8, Pool.Default);
            tra_surface = tra_tex.GetSurfaceLevel(0);

            int texw = ztex_surface.Description.Width;
            int texh = ztex_surface.Description.Height;
            Console.WriteLine("tra_tex {0}x{1}", texw, texh);

            float w_scale = (float)devw / texw;
            float h_scale = (float)devh / texh;

            tra_sprite = new Sprite(device);
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
