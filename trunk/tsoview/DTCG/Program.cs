using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;
//using System.ComponentModel;
using System.Windows.Forms;
using System.IO;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using Direct3D=Microsoft.DirectX.Direct3D;
using TAHdecrypt;

namespace DTCG
{
public class TSOSample : IDisposable
{
    internal TSOForm form;

    internal Device device;
    internal Surface surface;

    internal Direct3D.Font font;
    internal Effect effect;

    // キー入力を保持
    internal bool[] keys = new bool[256];
    internal bool[] keysEnabled = new bool[256];

    // モデル位置
    internal Vector3 modelPos = new Vector3(0.0f, -10.0f, 0.0f);
    internal float modelAngle = 0.0f;

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

    public bool InitializeApplication(TSOForm form, TMOForm tmo_form)
    {
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
            device = new Device(adapter_ordinal, DeviceType.Hardware, tmo_form.Handle, flags, pp);

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

        Transform_View = Matrix.LookAtLH(
                new Vector3( 0.0f, -10.0f, -44.0f ), 
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

        /*
        device.RenderState.AlphaBlendEnable = true;
        device.SetTextureStageState(0, TextureStageStates.AlphaOperation, (int)TextureOperation.Modulate);
        device.SetTextureStageState(0, TextureStageStates.AlphaArgument1, (int)TextureArgument.TextureColor);
        device.SetTextureStageState(0, TextureStageStates.AlphaArgument2, (int)TextureArgument.Current);

        device.RenderState.SourceBlend = Blend.SourceAlpha; 
        device.RenderState.DestinationBlend = Blend.InvSourceAlpha;
        */

        device.RenderState.IndexedVertexBlendEnable = true;

        surface = device.CreateOffscreenPlainSurface(form.Width, form.Height, Format.A8R8G8B8, Pool.Scratch);

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
            SaveToBitmap("sample.bmp");
        }
        if (keysEnabled[keyMotion] && keys[keyMotion])
        {
            keysEnabled[keyMotion] = false;
            motionEnabled = ! motionEnabled;
        }

        if (keys[(int)Keys.Left])
            modelAngle -= 2.0f;
        if (keys[(int)Keys.Right])
            modelAngle += 2.0f;
        if (keys[(int)Keys.PageUp])
            modelPos.Y += 0.2f;
        if (keys[(int)Keys.PageDown])
            modelPos.Y -= 0.2f;
        if (keys[(int)Keys.Up])
            modelPos.Z += 0.2f;
        if (keys[(int)Keys.Down])
            modelPos.Z -= 0.2f;

        if (motionEnabled)
            UpdateMotion();

        Matrix world_matrix = Matrix.RotationY(Geometry.DegreeToRadian(modelAngle)) * Matrix.Translation(modelPos);
        //device.Transform.World = world_matrix;
        matrixStack.LoadMatrix(Matrix.Identity);
        foreach (TSOFile tso in TSOList)
            UpdateBoneMatrices(tso.nodes[0]);

        device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, System.Drawing.Color.FromArgb(0x00, 0xFF, 0x00), 1.0f, 0);
        device.BeginScene();

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
        //device.Present();
        UpdateBitmap();
        form.Invalidate();
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

    public void SaveToBitmap(string file)
    {
        using (Surface sf = device.GetBackBuffer(0, 0, BackBufferType.Mono))
        {
            SurfaceLoader.Save(file, ImageFileFormat.Bmp, sf);
        }
    }

    public unsafe void UpdateBitmap()
    {
        using (Surface sf = device.GetBackBuffer(0, 0, BackBufferType.Mono))
        {
            SurfaceLoader.FromSurface(surface, sf, Filter.None, 0);
        }

        Bitmap bmp = form.bmp;
        BitmapData bmp_data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
        uint* q = (uint*)bmp_data.Scan0.ToPointer();

        int xMax = bmp.Width;
        int yMax = bmp.Height;

    GraphicsStream gs = surface.LockRectangle(LockFlags.ReadOnly);
    {
        uint* p = (uint*)gs.InternalData;

        for (int y = 0; y < yMax; y++)
        {
            for (int x = 0; x < xMax; x++)
            {
                *q++ = *p++;
            }
        }
    }
        surface.UnlockRectangle();
        bmp.UnlockBits(bmp_data);

        //form.UpdateBackground();
    }
}

public class TMOForm : Form
{
    public TMOForm()
    {
        this.ClientSize = new System.Drawing.Size(320, 480);
        //this.FormBorderStyle = FormBorderStyle.None;
        this.Text = "DTCG";

        //this.TransparencyKey = Color.FromArgb(0x00, 0xFF, 0x00);
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

public class TSOForm : Form
{
    public void UpdateBackground()
    {
        using (Bitmap src = new Bitmap("sample.bmp"))
        using (Graphics g = Graphics.FromImage(bmp))
        {
            g.DrawImage(src, 0, 0);
        }
        //this.BackgroundImage = bmp;

        Invalidate();
    }

    internal Bitmap bmp = null;
    //internal ImageAttributes imageAttributes = null;

    public TSOForm()
    {
        this.ClientSize = new System.Drawing.Size(320, 480);
        //this.FormBorderStyle = FormBorderStyle.None;
        this.Text = "DTCG";
        this.TopMost = true;

        //this.BackgroundImage = bmp;
        this.TransparencyKey = Color.FromArgb(0x00, 0xFF, 0x00);

        bmp = new Bitmap(Width, Height);
        //ImageAttributes imageAttributes = new ImageAttributes();
        //imageAttributes.SetColorKey(bmp.GetPixel(0, 0), bmp.GetPixel(0, 0));
    }

    protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
    {
        e.Graphics.DrawImage(bmp, new Rectangle(0, 0, bmp.Width, bmp.Height), 0, 0, bmp.Width, bmp.Height, GraphicsUnit.Pixel/*, imageAttributes */);
    }

    protected override void OnPaintBackground(PaintEventArgs e)
    {
        // nothing
    }

    protected override void OnKeyPress(System.Windows.Forms.KeyPressEventArgs e)
    {
        if ((int)(byte)e.KeyChar == (int)System.Windows.Forms.Keys.Escape)
            this.Dispose(); // Esc was pressed
    }
}

static class DTCG
{
    static void Main(string[] args) 
    {
        Debug.Listeners.Add(new TextWriterTraceListener(Console.Out));

        if (args.Length < 1)
        {
            System.Console.WriteLine("Usage: DTCG <tso file> [tmo file]");
            return;
        }

        string tso_file = args[0];
        string tmo_file = null;
        if (args.Length > 1)
            tmo_file = args[1];

        using (TSOSample sample = new TSOSample())
        using (TSOForm form = new TSOForm())
        using (TMOForm tmo_form = new TMOForm())
        {
            sample.LoadTSOFile(tso_file);
            sample.LoadTMOFile(tmo_file);

            if (sample.InitializeApplication(form, tmo_form))
            {
                form.Show();
                //tmo_form.Show();
                // While the form is still valid, render and process messages
                while (form.Created)
                {
                    sample.MainLoop();
                    Application.DoEvents();
                }
            }

        }
    }
}
}
