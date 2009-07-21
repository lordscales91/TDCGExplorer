using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
//using System.ComponentModel;
using System.Windows.Forms;
using System.IO;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using Direct3D=Microsoft.DirectX.Direct3D;
using NyARToolkitCSUtils.Capture;
using NyARToolkitCSUtils.NyAR;
//using NyARToolkitCSUtils.Direct3d;
using System.Runtime.InteropServices;

using jp.nyatla.nyartoolkit.cs;
using jp.nyatla.nyartoolkit.cs.core;
using jp.nyatla.nyartoolkit.cs.detector;
using TDCG;

namespace ARCG
{
public class Viewer : IDisposable, CaptureListener
{
    internal DsBGRX32Raster raster = null;
    internal Surface surface;
    internal NyARSingleDetectMarker marker;
    internal Matrix trans_matrix;

    const int camw = 640;
    const int camh = 480;

    public void OnBuffer(CaptureDevice i_sender, double i_sample_time, IntPtr i_buffer, int i_buffer_len)
    {
        raster.setBuffer(i_buffer);

        lock (this)
        {
            GraphicsStream gs = surface.LockRectangle(LockFlags.None);
            int s_idx = camw * camh * 4 - 4;
            int d_idx = 0;
            byte[] buf = (byte[])raster.getBufferReader().getBuffer();
            for (int y = 0; y < camh; y++)
            {
                for (int x = 0; x < camw; x++)
                {
                    Marshal.Copy(buf, s_idx, (IntPtr)((int)gs.InternalData + d_idx), 4);
                    s_idx -= 4;
                    d_idx += 4;
                }
            }
            surface.UnlockRectangle();
            if (marker.detectMarkerLite(raster, 110) && marker.getConfidence() > 0.4)
            {
                NyARTransMatResult trans_result = new NyARTransMatResult();
                marker.getTransmationMatrix(trans_result);
                NyARD3dUtil util = new NyARD3dUtil();
                util.toD3dMatrix(trans_result, ref trans_matrix);
            }
        }
    }

    internal TSOForm form;

    internal Device device;

    internal Direct3D.Font font;
    internal Effect effect;

    // キー入力を保持
    internal bool[] keys = new bool[256];
    internal bool[] keysEnabled = new bool[256];

    // モデル位置
    internal Vector3 modelPos = new Vector3(0.0f, -5.0f, 0.0f);
    // ライト方向
    internal Vector3 lightDir = new Vector3(0.0f, 0.0f, -1.0f);

    // アニメーション関連
    private MatrixStack matrixStack = null;

    internal List<TSOFile> TSOList = new List<TSOFile>();
    internal TMOFile tmo;
    internal Dictionary<string, TMONode> tmo_nodemap;

    private void form_OnKeyDown(object sender, KeyEventArgs e)
    {
        if ((int)e.KeyCode < keys.Length)
        {
            keys[(int)e.KeyCode] = true;
        }
    }

    private void form_OnKeyUp(object sender, KeyEventArgs e)
    {
        if ((int)e.KeyCode < keys.Length)
        {
            keys[(int)e.KeyCode] = false;
            keysEnabled[(int)e.KeyCode] = true;
        }
    }

    private Matrix Transform_View = Matrix.Identity;
    private Matrix Transform_Projection = Matrix.Identity;

    internal CaptureDevice capture;

    public bool select_capture_device()
    {
        //キャプチャデバイスリストを取得
        CaptureDeviceList capture_device_list = new CaptureDeviceList();
        if (capture_device_list.count < 1)
        {
            Console.WriteLine("Error: Capture device not found.");
            return false;
        }
        for (int i = 0; i < capture_device_list.count; i++)
        {
            Console.WriteLine(i + ": " + capture_device_list[i].name);
        }
        Console.Write("Select capture device: ");
        string line = Console.ReadLine();

        int capture_idx = 0;
        try
        {
            capture_idx = int.Parse(line);
        }
        catch (FormatException ex)
        {
            Console.WriteLine(ex);
            return false;
        }

        try
        {
            capture = capture_device_list[capture_idx];
        }
        catch (ArgumentOutOfRangeException ex)
        {
            Console.WriteLine(ex);
            return false;
        }
        return true;
    }

    public bool InitializeApplication(TSOForm form)
    {
        if (! select_capture_device())
            return false;
        capture.PrepareCapture(camw, camh, 30.0f);//width, height, fps

        this.form = form;

        for (int i = 0; i < keysEnabled.Length; i++)
        {
            keysEnabled[i] = true;
        }
        form.KeyDown += new KeyEventHandler(form_OnKeyDown);
        form.KeyUp += new KeyEventHandler(form_OnKeyUp);

        PresentParameters pp = new PresentParameters();
        try
        {
            pp.Windowed = true;
            pp.SwapEffect = SwapEffect.Discard;
            pp.BackBufferFormat = Format.X8R8G8B8;
            pp.BackBufferCount = 1;
            pp.EnableAutoDepthStencil = true;
            pp.AutoDepthStencilFormat = DepthFormat.D16;

            int adapter_ordinal = Manager.Adapters.Default.Adapter;

            int ret, quality;
            if (Manager.CheckDeviceMultiSampleType(adapter_ordinal, DeviceType.Hardware, pp.BackBufferFormat, pp.Windowed, MultiSampleType.FourSamples, out ret, out quality))
            {
                pp.MultiSample = MultiSampleType.FourSamples;
                pp.MultiSampleQuality = quality - 1;
            }

            CreateFlags flags = CreateFlags.SoftwareVertexProcessing;
            Caps caps = Manager.GetDeviceCaps(adapter_ordinal, DeviceType.Hardware);
            if (caps.DeviceCaps.SupportsHardwareTransformAndLight)
                flags = CreateFlags.HardwareVertexProcessing;
            if (caps.DeviceCaps.SupportsPureDevice)
                flags |= CreateFlags.PureDevice;
            device = new Device(adapter_ordinal, DeviceType.Hardware, form.Handle, flags, pp);

            FontDescription fd = new FontDescription();
            fd.Height = 24;
            fd.FaceName = "MS Gothic";
            font = new Direct3D.Font(device, fd);
        }
        catch (DirectXException ex)
        {
            Console.WriteLine("Error: " + ex);
            return false;
        }

        matrixStack = new MatrixStack();

        Directory.SetCurrentDirectory(Application.StartupPath);

        string effect_file = @"toonshader.cgfx";
        if (! File.Exists(effect_file))
        {
            Console.WriteLine("File not found: " + effect_file);
            return false;
        }
        string compile_error;
        effect = Effect.FromFile(device, effect_file, null, ShaderFlags.None, null, out compile_error);
        if (compile_error != null)
        {
            Console.WriteLine(compile_error);
            return false;
        }

        NyARParam param = new NyARParam();
        param.loadARParamFromFile(@"data\camera_para.dat");
        param.changeScreenSize(camw, camh);

        Transform_View = Matrix.LookAtLH(
                new Vector3( 0.0f, 0.0f, 10.0f ), 
                new Vector3( 0.0f, 0.0f, 0.0f ), 
                new Vector3( 0.0f, 1.0f, 0.0f ) );
        Transform_View.M31 = -Transform_View.M31;
        Transform_View.M32 = -Transform_View.M32;
        Transform_View.M33 = -Transform_View.M33;
        Transform_View.M34 = -Transform_View.M34;

        Transform_Projection = Matrix.PerspectiveFovLH(
                Geometry.DegreeToRadian(30.0f),
                (float)device.Viewport.Width / (float)device.Viewport.Height,
                1.0f,
                100.0f );

        NyARD3dUtil util = new NyARD3dUtil();
        Matrix m = new Matrix();
        util.toCameraFrustumRH(param, ref m);
        Transform_Projection = m;

        // xxx: for w-buffering
        device.Transform.View = Transform_View;
        device.Transform.Projection = Transform_Projection;

        if (effect != null)
        {
            effect.SetValue("view", Transform_View);
            effect.SetValue("proj", Transform_Projection);
        }

        device.RenderState.Lighting = false;
        device.RenderState.CullMode = Cull.CounterClockwise;

        device.RenderState.AlphaBlendEnable = true;
        device.SetTextureStageState(0, TextureStageStates.AlphaOperation, (int)TextureOperation.Modulate);
        device.SetTextureStageState(0, TextureStageStates.AlphaArgument1, (int)TextureArgument.TextureColor);
        device.SetTextureStageState(0, TextureStageStates.AlphaArgument2, (int)TextureArgument.Current);

        device.RenderState.SourceBlend = Blend.SourceAlpha; 
        device.RenderState.DestinationBlend = Blend.InvSourceAlpha;

        raster = new DsBGRX32Raster(camw, camh, camw*4);
        surface = device.CreateOffscreenPlainSurface(camw, camh, Format.X8R8G8B8, Pool.Default);

        NyARCode code = new NyARCode(16, 16);
        code.loadARPattFromFile(@"data\patt.hiro");
        marker = new NyARSingleDetectMarker(param, code, 80.0);
        marker.setContinueMode(false);

        trans_matrix = Matrix.Identity;
        capture.SetCaptureListener(this);

        OpenTSOFile(device, effect);

        return true;
    }

    internal int motionIndex = 0;
    internal bool motionEnabled = true;

    protected void UpdateMotion()
    {
        if (tmo.frames != null)
        {
            int tick = Environment.TickCount / 10;
            motionIndex = tick % tmo.frames.Length;
        }
    }

    protected void UpdateBoneMatrices(TSONode node)
    {
        matrixStack.Push();

        Matrix transform = node.transformation_matrix;

        if (tmo_nodemap != null)
        try
        {
            // TMO animation
            transform = tmo_nodemap[node.ShortName].frame_matrices[motionIndex].m;
        }
        catch (KeyNotFoundException)
        {
            /* not found */
        }

        matrixStack.MultiplyMatrixLocal(transform);
        
        {
            node.combined_matrix = matrixStack.Top;
        }

        foreach (TSONode child in node.child_nodes)
            UpdateBoneMatrices(child);

        matrixStack.Pop();
    }

    internal int keySave        = (int)Keys.Return;
    internal int keyMotion      = (int)Keys.Space;

    public void MainLoop()
    {
        if (keysEnabled[keySave] && keys[keySave])
        {
            keysEnabled[keySave] = false;
            SaveToBitmap();
        }
        if (keysEnabled[keyMotion] && keys[keyMotion])
        {
            keysEnabled[keyMotion] = false;
            motionEnabled = ! motionEnabled;
        }

        if (keys[(int)Keys.PageUp])
            modelPos.Y += 0.2f;
        if (keys[(int)Keys.PageDown])
            modelPos.Y -= 0.2f;
        if (keys[(int)Keys.Up])
            modelPos.Z += 0.2f;
        if (keys[(int)Keys.Down])
            modelPos.Z -= 0.2f;

        foreach (TSOFile tso in TSOList)
            tso.lightDir = lightDir;

        if (motionEnabled)
            UpdateMotion();

        float scale = 10.0f;
        Matrix world_matrix = Matrix.Scaling(scale, scale, scale) * Matrix.RotationX((float)Math.PI*0.5f) * trans_matrix;
        //device.Transform.World = world_matrix;
        matrixStack.LoadMatrix(Matrix.Identity);
        foreach (TSOFile tso in TSOList)
            UpdateBoneMatrices(tso.nodes[0]);

        Rectangle src_rect = new Rectangle(0, 0, camw, camh);

        //カメラ画像の転写矩形を作成
        Viewport vp = device.Viewport;
        Rectangle view_rect = new Rectangle(vp.X, vp.Y, vp.Width, vp.Height);

        device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, System.Drawing.Color.LightGray, 1.0f, 0);
        device.BeginScene();

    lock (this)
    {
        //背景描画
        Surface dest_surface = device.GetBackBuffer(0, 0, BackBufferType.Mono);
        device.StretchRectangle(surface, src_rect, dest_surface, view_rect, TextureFilter.None);
    }

        if (effect != null)
        {
            Matrix world_view_matrix = world_matrix * Transform_View;
            Matrix world_view_projection_matrix = world_view_matrix * Transform_Projection;
            effect.SetValue("wld", world_matrix);
            effect.SetValue("wv", world_view_matrix);
            effect.SetValue("wvp", world_view_projection_matrix);
        }

        foreach (TSOFile tso in TSOList)
        {
            tso.BeginRender();

            foreach (TSOMesh tm in tso.meshes)
            foreach (TSOSubMesh tm_sub in tm.sub_meshes)
            {
                device.RenderState.VertexBlend = (VertexBlend)(4 - 1);

                tso.SwitchShader(tm_sub);
                Matrix[] clipped_boneMatrices = new Matrix[tm_sub.maxPalettes];

                int i = 0;
                for (int numPalettes = 0; numPalettes < tm_sub.maxPalettes; numPalettes++)
                {
                    //device.Transform.SetWorldMatrixByIndex(numPalettes, combined_matrix);
                    TSONode bone = tm_sub.GetBone(numPalettes);
                    clipped_boneMatrices[numPalettes] = bone.GetOffsetMatrix() * bone.combined_matrix;
                }
                effect.SetValue("LocalBoneMats", clipped_boneMatrices);

                int npass = effect.Begin(0);
                for (int ipass = 0; ipass < npass; ipass++)
                {
                    effect.BeginPass(ipass);
                    tm_sub.dm.DrawSubset(i);
                    effect.EndPass();
                }
                effect.End();
            }
            tso.EndRender();
        }

        int height = 24;
        for (int i = 0; i < keys.Length; i++)
        {
            if (keys[i])
            {
                font.DrawText(null, ((Keys)i).ToString(), 0, height, Color.Black);
                height += 24;
            }
        }

        device.EndScene();
        device.Present();
        Thread.Sleep(30);
    }

    public void Dispose()
    {
        foreach (TSOFile tso in TSOList)
            tso.Dispose();
        if (effect != null)
            effect.Dispose();
        if (font != null)
            font.Dispose();
        if (surface != null)
            surface.Dispose();
        if (device != null)
            device.Dispose();
    }

    public void OpenTSOFile(Device device, Effect effect)
    {
        foreach (TSOFile tso in TSOList)
            tso.Open(device, effect);
    }

    public void LoadTSOFile(string source_file)
    {
        try
        {
            if (Path.GetExtension(source_file).ToUpper().Equals(".TSO"))
            {
                TSOFile tso = new TSOFile();
                tso.Load(source_file);
                TSOList.Add(tso);
            }
            else if (Directory.Exists(source_file))
            {
                string[] files = Directory.GetFiles(source_file, "*.TSO");
                foreach (string file in files)
                {
                    TSOFile tso = new TSOFile();
                    tso.Load(file);
                    TSOList.Add(tso);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex);
        }
    }

    public void LoadTMOFile(string source_file)
    {
        tmo = new TMOFile();
        if (File.Exists(source_file))
        try
        {
            tmo.Load(source_file);

            tmo_nodemap = new Dictionary<string, TMONode>();
            foreach (TMONode node in tmo.nodes)
            {
                tmo_nodemap.Add(node.ShortName, node);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex);
        }
    }

    public void SaveToBitmap()
    {
      using (Surface sf = device.GetBackBuffer(0, 0, BackBufferType.Mono))
      if (sf != null)
          SurfaceLoader.Save("viewer.bmp", ImageFileFormat.Bmp, sf);
    }

    public void start()
    {
        capture.StartCapture();
    }

    public void stop()
    {
        capture.StopCapture();
    }
}

public class TSOForm : Form
{
    public TSOForm()
    {
        this.ClientSize = new System.Drawing.Size(800, 600);
        this.Text = "ARCG";
    }

    protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
    {
        //this.Render(); // Render on painting
    }

    protected override void OnKeyPress(System.Windows.Forms.KeyPressEventArgs e)
    {
        if ((int)(byte)e.KeyChar == (int)System.Windows.Forms.Keys.Escape)
            this.Dispose(); // Esc was pressed
    }
}

static class ARCG
{
    static void Main(string[] args) 
    {
        if (args.Length < 1)
        {
            System.Console.WriteLine("Usage: ARCG <tso file> [tmo file]");
            return;
        }

        string tso_file = args[0];
        string tmo_file = null;
        if (args.Length > 1)
            tmo_file = args[1];

        using (Viewer viewer = new Viewer())
        using (TSOForm form = new TSOForm())
        {
            viewer.LoadTSOFile(tso_file);
            viewer.LoadTMOFile(tmo_file);

            if (viewer.InitializeApplication(form))
            {
                form.Show();
                viewer.start();
                // While the form is still valid, render and process messages
                while (form.Created)
                {
                    viewer.MainLoop();
                    Application.DoEvents();
                }
                viewer.stop();
            }

        }
    }
}
}
