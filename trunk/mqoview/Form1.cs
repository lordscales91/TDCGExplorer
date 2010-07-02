using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace mqoview
{
    public partial class Form1 : Form
    {
        Device device = null;
        Effect effect = null;
        Matrix world_matrix = Matrix.Identity;
        Matrix Transform_View = Matrix.Identity;
        Matrix Transform_Projection = Matrix.Identity;

        public Form1()
        {
            InitializeComponent();
            this.ClientSize = new Size(800, 600);
        }

        public bool InitializeGraphics()
        {
            PresentParameters pp = new PresentParameters();

            pp.Windowed = true;
            pp.SwapEffect = SwapEffect.Discard;

            int adapter_ordinal = Manager.Adapters.Default.Adapter;

            CreateFlags flags = CreateFlags.SoftwareVertexProcessing;
            Caps caps = Manager.GetDeviceCaps(adapter_ordinal, DeviceType.Hardware);
            if (caps.DeviceCaps.SupportsHardwareTransformAndLight)
                flags = CreateFlags.HardwareVertexProcessing;
            if (caps.DeviceCaps.SupportsPureDevice)
                flags |= CreateFlags.PureDevice;
            device = new Device(adapter_ordinal, DeviceType.Hardware, this, flags, pp);

            string effect_file = Path.Combine(Application.StartupPath, @"toonshader.cgfx");
            if (!File.Exists(effect_file))
            {
                Console.WriteLine("File not found: " + effect_file);
                return false;
            }
            using (FileStream effect_stream = File.OpenRead(effect_file))
            {
                string compile_error;
                effect = Effect.FromStream(device, effect_stream, null, ShaderFlags.None, null, out compile_error);
                if (compile_error != null)
                {
                    Console.WriteLine(compile_error);
                    return false;
                }
            }

            Transform_Projection = Matrix.PerspectiveFovLH((float)Math.PI / 4, Width / Height, 1.0f, 10000.0f);
            // xxx: for w-buffering
            device.Transform.Projection = Transform_Projection;
            effect.SetValue("proj", Transform_Projection);
            
            Transform_View = Matrix.LookAtLH(new Vector3(0, +10.0f, 44.0f), new Vector3(0, +10.0f, 0), new Vector3(0, 1, 0));
            // xxx: for w-buffering
            device.Transform.View = Transform_View;
            effect.SetValue("view", Transform_View);

            timer1.Enabled = true;
            return true;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            device.Clear(ClearFlags.Target, Color.CornflowerBlue, 1.0f, 0);
            device.Present();
        }

        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                foreach (string src in (string[])e.Data.GetData(DataFormats.FileDrop))
                    LoadAnyFile(src, (e.KeyState & 8) == 8);
            }
        }

        private void Form1_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                if ((e.KeyState & 8) == 8)
                    e.Effect = DragDropEffects.Copy;
                else
                    e.Effect = DragDropEffects.Move;
            }
        }

        /// <summary>
        /// 任意のファイルを読み込みます。
        /// </summary>
        /// <param name="source_file">任意のパス</param>
        public void LoadAnyFile(string source_file)
        {
            LoadAnyFile(source_file, false);
        }

        /// <summary>
        /// 任意のファイルを読み込みます。
        /// </summary>
        /// <param name="source_file">任意のパス</param>
        /// <param name="append">FigureListを消去せずに追加するか</param>
        public void LoadAnyFile(string source_file, bool append)
        {
            switch (Path.GetExtension(source_file).ToLower())
            {
                case ".mqo":
                    LoadMqoFile(source_file);
                    break;
            }
        }

        MqoFile mqo = null;

        public void LoadMqoFile(string source_file)
        {
            if (mqo != null)
            {
                mqo.Dispose();
                mqo = null;
            }
            mqo = new MqoFile();
            mqo.Load(source_file);
            foreach (MqoObject obj in mqo.Objects)
            {
                obj.WriteBuffer(device);
            }
        }

        /// <summary>
        /// シーンをレンダリングします。
        /// </summary>
        public void Render()
        {
            device.BeginScene();

            {
                Matrix world_view_matrix = world_matrix * Transform_View;
                Matrix world_view_projection_matrix = world_view_matrix * Transform_Projection;
                effect.SetValue("wld", world_matrix);
                effect.SetValue("wv", world_view_matrix);
                effect.SetValue("wvp", world_view_projection_matrix);
            }

            if (mqo != null)
            {
                foreach (MqoObject obj in mqo.Objects)
                {
                    if (obj.dm == null)
                        continue;

                    int npass = effect.Begin(0);
                    for (int ipass = 0; ipass < npass; ipass++)
                    {
                        Console.WriteLine("render {0} {1}", obj, ipass);
                        effect.BeginPass(ipass);
                        obj.dm.DrawSubset(0);
                        effect.EndPass();
                    }
                    effect.End();
                }
            }

            device.EndScene();
            {
                int ret;
                if (!device.CheckCooperativeLevel(out ret))
                {
                    switch ((ResultCode)ret)
                    {
                        case ResultCode.DeviceLost:
                            Thread.Sleep(30);
                            return;
                        case ResultCode.DeviceNotReset:
                            device.Reset(device.PresentationParameters);
                            break;
                        default:
                            Console.WriteLine((ResultCode)ret);
                            return;
                    }
                }
            }

            device.Present();
            Thread.Sleep(30);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Render();
        }

    }
}
