using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.ComponentModel;
using System.Windows.Forms;
using System.IO;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using Direct3D=Microsoft.DirectX.Direct3D;

namespace TDCG
{
    /// <summary>
    /// TSOFileをDirect3D上でレンダリングします。
    /// </summary>
public class Viewer : IDisposable
{
    internal Control control;

    internal Device device;

    internal Direct3D.Font font;
    internal Effect effect;

    private EffectHandle handle_LocalBoneMats;
    private EffectHandle handle_ShadowMap;

    private bool shadowMapEnabled = false;
    internal Texture ztex = null;
    int ztexw = 0;
    int ztexh = 0;
    internal Surface ztex_surface = null;
    internal Surface ztex_zbuf = null;

    internal Sprite sprite = null;
    float w_scale = 1.0f;
    float h_scale = 1.0f;

    internal Surface dev_surface = null;
    internal Surface dev_zbuf = null;

    internal Mesh sphere = null;

    /// <summary>
    /// viewerが保持しているフィギュアリスト
    /// </summary>
    public List<Figure> FigureList = new List<Figure>();

    // ライト方向
    internal Vector3 lightDir = new Vector3(0.0f, 0.0f, 1.0f);

    Vector3 target = new Vector3(5.0f, 10.0f, 0.0f);
    float swivel = 0.0f;

    public void MoveTarget(float dx, float dy, float dz)
    {
        if (dx == 0 && dy == 0 && dz == 0)
            return;
        target.X += dx;
        target.Y += dy;
        target.Z += dz;
        solved = false;
    }

    public void MoveSwivel(float delta)
    {
        if (delta == 0)
            return;
        swivel += delta;
        solved = false;
    }

    // マウスポイントしているスクリーン座標
    private Point lastScreenPoint = Point.Empty;

    private void control_OnSizeChanged(object sender, EventArgs e)
    {
        Transform_Projection = Matrix.PerspectiveFovLH(
                Geometry.DegreeToRadian(30.0f),
                (float)control.Width / (float)control.Height,
                1.0f,
                1000.0f );
    }

    private void form_OnMouseDown(object sender, MouseEventArgs e)
    {
        switch (e.Button)
        {
        case MouseButtons.Left:
            if (Control.ModifierKeys == Keys.Control)
                lightDir = ScreenToOrientation(e.X, e.Y);
            break;
        }

        lastScreenPoint.X = e.X;
        lastScreenPoint.Y = e.Y;
    }

    private void form_OnMouseMove(object sender, MouseEventArgs e)
    {
        int dx = e.X - lastScreenPoint.X;
        int dy = e.Y - lastScreenPoint.Y;

        switch (e.Button)
        {
        case MouseButtons.Left:
            if (Control.ModifierKeys == Keys.Control)
                lightDir = ScreenToOrientation(e.X, e.Y);
            else
                camera.Move(-dx, dy, 0.0f);
            break;
        case MouseButtons.Middle:
            camera.MoveView(-dx*0.125f, dy*0.125f);
            break;
        case MouseButtons.Right:
            camera.Move(0.0f, 0.0f, dy*0.125f);
            break;
        }

        lastScreenPoint.X = e.X;
        lastScreenPoint.Y = e.Y;
    }

    // 選択フィギュアindex
    int figureIndex = 0;

    // スクリーンの中心座標
    private float screenCenterX = 800 / 2.0f;
    private float screenCenterY = 600 / 2.0f;

    /// <summary>
    /// controlを保持します。スクリーンの中心座標を更新します。
    /// </summary>
    /// <param name="control">control</param>
    protected void SetControl(Control control)
    {
        this.control = control;
        screenCenterX = control.ClientSize.Width / 2.0f;
        screenCenterY = control.ClientSize.Height / 2.0f;
    }

    /// <summary>
    /// 指定スクリーン座標からスクリーン中心へ向かうベクトルを得ます。
    /// </summary>
    /// <param name="screenPointX">スクリーンX座標</param>
    /// <param name="screenPointY">スクリーンY座標</param>
    /// <returns>方向ベクトル</returns>
    public Vector3 ScreenToOrientation(float screenPointX, float screenPointY)
    {
        float radius = 1.0f;
        float x = -(screenPointX - screenCenterX) / (radius * screenCenterX);
        float y = +(screenPointY - screenCenterY) / (radius * screenCenterY);
        float z = 0.0f;
        float mag = (x*x) + (y*y);

        if (mag > 1.0f)
        {
            float scale = 1.0f / (float)Math.Sqrt(mag);
            x *= scale;
            y *= scale;
        }
        else
            z = (float)Math.Sqrt(1.0f - mag);

        return new Vector3(x, y, z);
    }

    /// 球とレイの衝突を見つけます。
    public bool DetectSphereRayCollision(float sphereRadius, ref Vector3 sphereCenter, ref Vector3 rayStart, ref Vector3 rayOrientation, out Vector3 collisionPoint, out float collisionTime)
    {
        collisionTime = 0.0f;
        collisionPoint = Vector3.Empty;

        Vector3 u = rayStart - sphereCenter;
        float a = Vector3.Dot(rayOrientation, rayOrientation);
        float b = Vector3.Dot(rayOrientation, u);
        float c = Vector3.Dot(u, u) - sphereRadius*sphereRadius;
        if (a <= float.Epsilon)
            //誤差
            return false;
        float d = b*b - a*c;
        if (d < 0.0f)
            //衝突しない
            return false;
        collisionTime = (-b - (float)Math.Sqrt(d))/a;
        collisionPoint = rayStart + rayOrientation*collisionTime;
        return true;
    }

    /// スクリーン位置をワールド座標へ変換します。
    public Vector3 ScreenToWorld(float screenX, float screenY, float z, ref Matrix view, ref Matrix proj)
    {
        //viewport行列を作成
        Matrix m = Matrix.Identity;
        Viewport vp = device.Viewport;
        m.M11 = (float)vp.Width/2;
        m.M22 = -1.0f*(float)vp.Height/2;
        m.M33 = (float)vp.MaxZ - (float)vp.MinZ;
        m.M41 = (float)(vp.X + vp.Width/2);
        m.M42 = (float)(vp.Y + vp.Height/2);
        m.M43 = vp.MinZ;

        //スクリーン位置
        Vector3 v = new Vector3(screenX, screenY,  z);

        Matrix inv_m = Matrix.Invert(m);
        Matrix inv_proj = Matrix.Invert(proj);
        Matrix inv_view = Matrix.Invert(view);

        //スクリーン位置をワールド座標へ変換
        return Vector3.TransformCoordinate(v, inv_m * inv_proj * inv_view);
    }

    /// <summary>
    /// 任意のファイルを読み込みます。
    /// </summary>
    /// <param name="source_file">任意のパス</param>
    public void LoadAnyFile(string source_file)
    {
        switch (Path.GetExtension(source_file).ToUpper())
        {
        case ".TSO":
            LoadTSOFile(source_file);
            break;
        case ".TMO":
            LoadTMOFile(source_file);
            break;
        case ".PNG":
            AddFigureFromPNGFile(source_file);
            break;
        default:
            if (Directory.Exists(source_file))
                AddFigureFromTSODirectory(source_file);
            break;
        }
    }

    public event EventHandler FigureEvent;

    /// <summary>
    /// フィギュアを選択します。
    /// </summary>
    /// <param name="figureIndex">フィギュア番号</param>
    public void SetFigureIndex(int figureIndex)
    {
        if (figureIndex < 0)
            figureIndex = 0;
        if (figureIndex > FigureList.Count-1)
            figureIndex = 0;
        this.figureIndex = figureIndex;
        if (FigureEvent != null)
            FigureEvent(this, EventArgs.Empty);
    }

    /// <summary>
    /// 指定ディレクトリからフィギュアを作成して追加します。
    /// </summary>
    /// <param name="source_file">TSOFileを含むディレクトリ</param>
    public void AddFigureFromTSODirectory(string source_file)
    {
        Figure fig = new Figure();
        List<TSOFile> tso_list = new List<TSOFile>();
        try
        {
            string[] files = Directory.GetFiles(source_file, "*.TSO");
            foreach (string file in files)
            {
                TSOFile tso = new TSOFile();
                tso.Load(file);
                tso_list.Add(tso);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex);
        }
        foreach (TSOFile tso in tso_list)
        {
            tso.Open(device, effect);
            fig.AddTSO(tso);
        }
        int idx = FigureList.Count;
        FigureList.Add(fig);
        SetFigureIndex(idx);
    }

    /// <summary>
    /// 選択フィギュアを得ます。
    /// </summary>
    public Figure GetSelectedFigure()
    {
        Figure fig;
        if (FigureList.Count == 0)
            fig = null;
        else
            fig = FigureList[figureIndex];
        return fig;
    }

    /// <summary>
    /// 選択フィギュアを得ます。なければ作成します。
    /// </summary>
    public Figure GetSelectedOrCreateFigure()
    {
        Figure fig;
        if (FigureList.Count == 0)
            fig = new Figure();
        else
            fig = FigureList[figureIndex];
        if (FigureList.Count == 0)
        {
            int idx = FigureList.Count;
            FigureList.Add(fig);
            SetFigureIndex(idx);
        }
        return fig;
    }

    /// <summary>
    /// 指定パスからTSOFileを読み込みます。
    /// </summary>
    /// <param name="source_file">パス</param>
    public void LoadTSOFile(string source_file)
    {
        Figure fig = GetSelectedOrCreateFigure();
        try
        {
            TSOFile tso = new TSOFile();
            tso.Load(source_file);
            tso.Open(device, effect);
            fig.AddTSO(tso);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex);
        }
        if (FigureEvent != null)
            FigureEvent(this, EventArgs.Empty);
    }

    /// <summary>
    /// 指定ストリームからTSOFileを読み込みます。
    /// </summary>
    /// <param name="source_stream">ストリーム</param>
    public void LoadTSOFile(Stream source_stream)
    {
        Figure fig = GetSelectedOrCreateFigure();
        try
        {
            TSOFile tso = new TSOFile();
            tso.Load(source_stream);
            tso.Open(device, effect);
            fig.AddTSO(tso);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex);
        }
        if (FigureEvent != null)
            FigureEvent(this, EventArgs.Empty);
    }

    /// <summary>
    /// 選択フィギュアを得ます。
    /// </summary>
    public bool TryGetFigure(out Figure fig)
    {
        fig = null;
        if (figureIndex < FigureList.Count)
            fig = FigureList[figureIndex];
        return fig != null;
    }

    /// 次のフィギュアを選択します。
    public void NextFigure()
    {
        SetFigureIndex(figureIndex+1);
        Figure fig;
        if (TryGetFigure(out fig))
            camera.SetCenter(fig.Center);
    }

    /// <summary>
    /// 指定パスからTMOFileを読み込みます。
    /// </summary>
    /// <param name="source_file">パス</param>
    public void LoadTMOFile(string source_file)
    {
        Figure fig;
        if (TryGetFigure(out fig))
        {
            if (File.Exists(source_file))
            try
            {
                TMOFile tmo = new TMOFile();
                tmo.Load(source_file);
                fig.Tmo = tmo;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex);
            }
            fig.UpdateNodeMapAndBoneMatrices();
            camera.SetCenter(fig.Center);
        }
    }

    /// <summary>
    /// 指定ストリームからTMOFileを読み込みます。
    /// </summary>
    /// <param name="source_stream">ストリーム</param>
    public void LoadTMOFile(Stream source_stream)
    {
        Figure fig;
        if (TryGetFigure(out fig))
        {
            try
            {
                TMOFile tmo = new TMOFile();
                tmo.Load(source_stream);
                fig.Tmo = tmo;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex);
            }
            fig.UpdateNodeMapAndBoneMatrices();
            camera.SetCenter(fig.Center);
        }
    }

    /// <summary>
    /// 指定パスからPNGFileを読み込みフィギュアを作成して追加します。
    /// </summary>
    /// <param name="source_file">PNGFile のパス</param>
    public void AddFigureFromPNGFile(string source_file)
    {
        List<Figure> fig_list = LoadPNGFile(source_file);
        if (fig_list.Count != 0) //taOb png
        if (fig_list[0].TSOList.Count == 0) //POSE png
        {
            TMOFile tmo = fig_list[0].Tmo;
            Figure fig;
            if (tmo != null && TryGetFigure(out fig))
            {
                fig.Tmo = tmo;
                fig.UpdateNodeMapAndBoneMatrices();
            }
        }
        else
        {
            int idx = FigureList.Count;
            foreach (Figure fig in fig_list)
            {
                fig.OpenTSOFile(device, effect);
                fig.UpdateNodeMapAndBoneMatrices();
                FigureList.Add(fig);
            }
            SetFigureIndex(idx);
        }
        {
            Figure fig;
            if (TryGetFigure(out fig))
                camera.SetCenter(fig.Center);
        }
    }

    private Camera camera = new Camera();

    /// <summary>
    /// カメラ
    /// </summary>
    public Camera Camera
    {
        get {
            return camera;
        }
        set {
            camera = value;
        }
    }

    private Matrix world_matrix = Matrix.Identity;
    private Matrix Transform_View = Matrix.Identity;
    private Matrix Transform_Projection = Matrix.Identity;
    private Matrix Light_View = Matrix.Identity;
    private Matrix Light_Projection = Matrix.Identity;

    /// <summary>
    /// deviceを作成します。
    /// </summary>
    /// <param name="control">レンダリング先となるcontrol</param>
    /// <returns>deviceの作成に成功したか</returns>
    public bool InitializeApplication(Control control)
    {
        return InitializeApplication(control, false);
    }

    /// <summary>
    /// deviceを作成します。
    /// </summary>
    /// <param name="control">レンダリング先となるcontrol</param>
    /// <param name="shadowMapEnabled">シャドウマップを作成するか</param>
    /// <returns>deviceの作成に成功したか</returns>
    public bool InitializeApplication(Control control, bool shadowMapEnabled)
    {
        this.shadowMapEnabled = shadowMapEnabled;
        SetControl(control);

        control.SizeChanged += new EventHandler(control_OnSizeChanged);
        control.MouseDown += new MouseEventHandler(form_OnMouseDown);
        control.MouseMove += new MouseEventHandler(form_OnMouseMove);

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
            device = new Device(adapter_ordinal, DeviceType.Hardware, control.Handle, flags, pp);

            device.DeviceLost += new EventHandler(OnDeviceLost);
            device.DeviceReset += new EventHandler(OnDeviceReset);
            device.DeviceResizing += new CancelEventHandler(CancelResize);

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

        string effect_file = Path.Combine(Application.StartupPath, @"toonshader.cgfx");
        if (! File.Exists(effect_file))
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
        handle_LocalBoneMats = effect.GetParameter(null, "LocalBoneMats");
        if (shadowMapEnabled)
        {
            handle_ShadowMap = effect.GetTechnique("ShadowMap");
            effect.ValidateTechnique(effect.Technique);

            sprite = new Sprite(device);
        }
        sphere = Mesh.Sphere(device, 1.0f, 8, 6);
        camera.Update();
        OnDeviceReset(device, null);

        return true;
    }

    private void OnDeviceLost(object sender, EventArgs e)
    {
        Console.WriteLine("OnDeviceLost");
        if (ztex_zbuf != null)
            ztex_zbuf.Dispose();
        if (ztex_surface != null)
            ztex_surface.Dispose();
        if (ztex != null)
            ztex.Dispose();
        if (dev_zbuf != null)
            dev_zbuf.Dispose();
        if (dev_surface != null)
            dev_surface.Dispose();
    }

    private void OnDeviceReset(object sender, EventArgs e)
    {
        Console.WriteLine("OnDeviceReset");
        int devw = 0;
        int devh = 0;
        dev_surface = device.GetRenderTarget(0);
        {
            devw = dev_surface.Description.Width;
            devh = dev_surface.Description.Height;
        }
        Console.WriteLine("dev {0}x{1}", devw, devh);

        dev_zbuf = device.DepthStencilSurface;
        if (shadowMapEnabled)
        {
            ztex = new Texture(device, 1024, 1024, 1, Usage.RenderTarget, Format.A8R8G8B8, Pool.Default);
            effect.SetValue("texShadowMap", ztex);
            ztex_surface = ztex.GetSurfaceLevel(0);
            {
                ztexw = ztex_surface.Description.Width;
                ztexh = ztex_surface.Description.Height;
            }
            Console.WriteLine("ztex {0}x{1}", ztexw, ztexh);

            ztex_zbuf = device.CreateDepthStencilSurface(ztexw, ztexh, DepthFormat.D16, MultiSampleType.None, 0, false);

            w_scale = (float)devw / ztexw;
            h_scale = (float)devh / ztexh;
        }
        Transform_Projection = Matrix.PerspectiveFovLH(
                Geometry.DegreeToRadian(30.0f),
                (float)device.Viewport.Width / (float)device.Viewport.Height,
                1.0f,
                1000.0f );
        if (shadowMapEnabled)
            Light_Projection = Matrix.PerspectiveFovLH(
                Geometry.DegreeToRadian(45.0f),
                1.0f,
                20.0f,
                250.0f );

        // xxx: for w-buffering
        device.Transform.Projection = Transform_Projection;

        {
            effect.SetValue("proj", Transform_Projection);
            if (shadowMapEnabled)
                effect.SetValue("lightproj", Light_Projection);
        }

        device.RenderState.Lighting = false;
        device.RenderState.CullMode = Cull.CounterClockwise;

        device.RenderState.AlphaBlendEnable = true;
        device.SetTextureStageState(0, TextureStageStates.AlphaOperation, (int)TextureOperation.Modulate);
        device.SetTextureStageState(0, TextureStageStates.AlphaArgument1, (int)TextureArgument.TextureColor);
        device.SetTextureStageState(0, TextureStageStates.AlphaArgument2, (int)TextureArgument.Current);

        device.RenderState.SourceBlend = Blend.SourceAlpha; 
        device.RenderState.DestinationBlend = Blend.InvSourceAlpha;
        device.RenderState.AlphaTestEnable = true;
        device.RenderState.ReferenceAlpha = 0x08;
        device.RenderState.AlphaFunction = Compare.GreaterEqual;

        device.RenderState.IndexedVertexBlendEnable = true;
    }

    private void CancelResize(object sender, CancelEventArgs e)
    {
        Console.WriteLine("CancelResize");
        //e.Cancel = true;
    }

    /// <summary>
    /// 全フィギュアを削除します。
    /// </summary>
    public void ClearFigureList()
    {
        foreach (Figure fig in FigureList)
            fig.Dispose();
        FigureList.Clear();
        SetFigureIndex(0);
        GC.Collect(); // free meshes and textures.
    }

    /// <summary>
    /// 選択フィギュアを削除します。
    /// </summary>
    public void RemoveSelectedFigure()
    {
        Figure fig;
        if (TryGetFigure(out fig))
        {
            fig.Dispose();
            FigureList.Remove(fig);
            SetFigureIndex(figureIndex-1);
            GC.Collect(); // free meshes and textures.
        }
    }

    internal bool motionEnabled = false;
    internal bool shadowShown = false;
    internal bool SpriteShown = false;

    /// <summary>
    /// モーションの有無を切り替えます。
    /// </summary>
    public void SwitchMotionEnabled()
    {
        motionEnabled = ! motionEnabled;
    }

    /// <summary>
    /// シャドウマップの有無を切り替えます。
    /// </summary>
    public void SwitchShadowShown()
    {
        shadowShown = ! shadowShown;
    }

    /// <summary>
    /// スプライトの有無を切り替えます。
    /// </summary>
    public void SwitchSpriteShown()
    {
        SpriteShown = ! SpriteShown;
    }

    /// <summary>
    /// 次のシーンフレームに進みます。
    /// </summary>
    public void FrameMove()
    {
        foreach (Figure fig in FigureList)
        {
            fig.NextFrame();
        }
        camera.NextFrame();

        camera.Update();

        Transform_View = camera.GetViewMatrix();
        Transform_View.M31 = -Transform_View.M31;
        Transform_View.M32 = -Transform_View.M32;
        Transform_View.M33 = -Transform_View.M33;
        Transform_View.M34 = -Transform_View.M34;

        // xxx: for w-buffering
        device.Transform.View = Transform_View;
        effect.SetValue("view", Transform_View);

        if (shadowMapEnabled)
        {
            float scale = 40.0f;

            Light_View = Matrix.LookAtLH(
                    lightDir * -scale,
                    new Vector3( 0.0f, 5.0f, 0.0f ), 
                    new Vector3( 0.0f, 1.0f, 0.0f ) );
            Light_View.M31 = -Light_View.M31;
            Light_View.M32 = -Light_View.M32;
            Light_View.M33 = -Light_View.M33;
            Light_View.M34 = -Light_View.M34;
        }

        foreach (Figure fig in FigureList)
        foreach (TSOFile tso in fig.TSOList)
            tso.lightDir = lightDir;

        if (! motionEnabled && ! solved)
        {
            foreach (Figure fig in FigureList)
            {
                foreach (TSOFile tso in fig.TSOList)
                    Solve(tso);
                fig.UpdateBoneMatricesWithoutTMO();
            }
        }
        else
        {
            //device.Transform.World = world_matrix;
            foreach (Figure fig in FigureList)
                fig.UpdateBoneMatrices();
        }

        if (motionEnabled)
        {
            foreach (Figure fig in FigureList)
                fig.NextTMOFrame();
        }
    }
    bool solved = false;

    /// <summary>
    /// シーンをレンダリングします。
    /// </summary>
    public void Render()
    {
        device.BeginScene();

        if (shadowMapEnabled)
        {
            device.SetRenderTarget(0, ztex_surface);
            device.DepthStencilSurface = ztex_zbuf;
            device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.White, 1.0f, 0);
        }

        {
            Matrix world_view_matrix = world_matrix * Transform_View;
            Matrix world_view_projection_matrix = world_view_matrix * Transform_Projection;
            effect.SetValue("wld", world_matrix);
            effect.SetValue("wv", world_view_matrix);
            effect.SetValue("wvp", world_view_projection_matrix);
            if (shadowMapEnabled)
                effect.SetValue("lightview", Light_View);
        }

    if (shadowMapEnabled && shadowShown)
    {
        device.RenderState.AlphaBlendEnable = false;

        effect.Technique = handle_ShadowMap;

        foreach (Figure fig in FigureList)
        foreach (TSOFile tso in fig.TSOList)
        {
            //tso.BeginRender();

            foreach (TSOMesh tm in tso.meshes)
            foreach (TSOSubMesh tm_sub in tm.sub_meshes)
            {
                device.RenderState.VertexBlend = (VertexBlend)(4 - 1);

                //tso.SwitchShader(tm_sub);
                Matrix[] clipped_boneMatrices = new Matrix[tm_sub.maxPalettes];

                for (int numPalettes = 0; numPalettes < tm_sub.maxPalettes; numPalettes++)
                {
                    //device.Transform.SetWorldMatrixByIndex(numPalettes, combined_matrix);
                    TSONode bone = tm_sub.GetBone(numPalettes);
                    clipped_boneMatrices[numPalettes] = bone.GetOffsetMatrix() * bone.combined_matrix;
                }
                effect.SetValue(handle_LocalBoneMats, clipped_boneMatrices);

                int npass = effect.Begin(0);
                for (int ipass = 0; ipass < npass; ipass++)
                {
                    effect.BeginPass(ipass);
                    tm_sub.dm.DrawSubset(0);
                    effect.EndPass();
                }
                effect.End();
            }
            //tso.EndRender();
        }
    }

        device.SetRenderTarget(0, dev_surface);
        device.DepthStencilSurface = dev_zbuf;
        device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.LightGray, 1.0f, 0);

        device.RenderState.AlphaBlendEnable = true;

        foreach (Figure fig in FigureList)
        foreach (TSOFile tso in fig.TSOList)
        {
            tso.BeginRender();

            foreach (TSOMesh tm in tso.meshes)
            foreach (TSOSubMesh tm_sub in tm.sub_meshes)
            {
                device.RenderState.VertexBlend = (VertexBlend)(4 - 1);

                tso.SwitchShader(tm_sub);
                Matrix[] clipped_boneMatrices = new Matrix[tm_sub.maxPalettes];

                for (int numPalettes = 0; numPalettes < tm_sub.maxPalettes; numPalettes++)
                {
                    //device.Transform.SetWorldMatrixByIndex(numPalettes, combined_matrix);
                    TSONode bone = tm_sub.GetBone(numPalettes);
                    clipped_boneMatrices[numPalettes] = bone.GetOffsetMatrix() * bone.combined_matrix;
                }
                effect.SetValue(handle_LocalBoneMats, clipped_boneMatrices);

                int npass = effect.Begin(0);
                for (int ipass = 0; ipass < npass; ipass++)
                {
                    effect.BeginPass(ipass);
                    tm_sub.dm.DrawSubset(0);
                    effect.EndPass();
                }
                effect.End();
            }
            tso.EndRender();
        }

        device.RenderState.AlphaBlendEnable = false;

    if (shadowMapEnabled && SpriteShown)
    {
        sprite.Transform = Matrix.Scaling(w_scale, h_scale, 1.0f);
        Rectangle rect = new Rectangle(0, 0, ztexw, ztexh);

        sprite.Begin(0);
        sprite.Draw(ztex, rect, new Vector3(0, 0, 0), new Vector3(0, 0, 0), Color.White);
        sprite.End();
    }

    {
        Figure fig;

        if (TryGetFigure(out fig))
        {
            TSONode bone = fig.TSOList[0].nodemap[sphere_bone_name];
            DrawMeshSub(sphere, bone.combined_matrix * world_matrix, GetBoneColor(bone));
        }
    }
    {
        MatrixStack stack = new MatrixStack();
        stack.LoadIdentity();
        stack.Translate(target);
        DrawMeshSub(sphere, stack.Top, new Vector4(1,1,0,1));
    }

        device.EndScene();
        {
            int ret;
            if (! device.CheckCooperativeLevel(out ret))
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

    public void DrawMeshSub(Mesh mesh, Matrix wld, Vector4 color)
    {
        effect.Technique = "BONE";

        Matrix wv = wld * Transform_View;
        Matrix wvp = wv * Transform_Projection;

        effect.SetValue("wld", wld);
        effect.SetValue("wv", wv);
        effect.SetValue("wvp", wvp);

        effect.SetValue("ManColor", color);

        int npass = effect.Begin(0);
        for (int ipass = 0; ipass < npass; ipass++)
        {
            effect.BeginPass(ipass);
            mesh.DrawSubset(0);
            effect.EndPass();
        }
        effect.End();
    }

    public string sphere_bone_name = "|W_Hips|W_Spine_Dummy|W_Spine1|W_Spine2|W_Spine3|W_Neck";

    public Vector4 GetBoneColor(TSONode node)
    {
        if (FindBoneOnScreenPoint(lastScreenPoint.X, lastScreenPoint.Y))
            return new Vector4(1,1,1,1);
        else
            return new Vector4(1,0,0,1);
    }

    private bool FindBoneOnScreenPoint(float x, float y)
    {
        Figure fig;

        if (TryGetFigure(out fig))
        {
            TSONode bone = fig.TSOList[0].nodemap[sphere_bone_name];
            Matrix m = bone.combined_matrix * world_matrix;

            float sphereRadius = 1.0f;
            Vector3 sphereCenter = new Vector3(m.M41, m.M42, m.M43);
            Vector3 rayStart = ScreenToWorld(x, y, 0.0f, ref Transform_View, ref Transform_Projection);
            Vector3 rayEnd = ScreenToWorld(x, y, 1.0f, ref Transform_View, ref Transform_Projection);
            Vector3 rayOrientation = rayEnd - rayStart;

            Vector3 collisionPoint;
            float collisionTime;

            return DetectSphereRayCollision(sphereRadius, ref sphereCenter, ref rayStart, ref rayOrientation, out collisionPoint, out collisionTime);
        }
        return false;
    }

    private void Solve(TSOFile tso)
    {
        string effector_name = "|W_Hips|W_Spine_Dummy|W_Spine1|W_Spine2|W_Spine3|W_LeftShoulder_Dummy|W_LeftShoulder|W_LeftArm_Dummy|W_LeftArm|W_LeftArmRoll|W_LeftForeArm|W_LeftForeArmRoll|W_LeftHand";
        TSONode effector = tso.nodemap[effector_name];

        string node_name;
        TSONode node;

        node_name = "|W_Hips|W_Spine_Dummy|W_Spine1|W_Spine2|W_Spine3|W_LeftShoulder_Dummy|W_LeftShoulder|W_LeftArm_Dummy|W_LeftArm|W_LeftArmRoll|W_LeftForeArm";
        node = tso.nodemap[node_name];
        Solve(effector, node);

        node_name = "|W_Hips|W_Spine_Dummy|W_Spine1|W_Spine2|W_Spine3|W_LeftShoulder_Dummy|W_LeftShoulder|W_LeftArm_Dummy|W_LeftArm";
        node = tso.nodemap[node_name];
        Solve(effector, node);
    }

    // Cyclic-Coordinate-Descent (CCD) 法
    private void Solve(TSONode effector, TSONode node)
    {
        Vector3 worldTargetP = target;

        Vector3 worldEffectorP = effector.GetWorldPosition();
        Vector3 worldNodeP = node.GetWorldPosition();

        Matrix invCoord = Matrix.Invert(node.GetWorldCoordinate());
        Vector3 localEffectorP = Vector3.TransformCoordinate(worldEffectorP, invCoord);
        Vector3 localTargetP = Vector3.TransformCoordinate(worldTargetP, invCoord);

        Vector3 toEffector = Vector3.Normalize(localEffectorP);
        Vector3 toTarget = Vector3.Normalize(localTargetP);

        float rotDotProduct = Vector3.Dot(toEffector, toTarget);
        float rotAngle = (float)Math.Acos(rotDotProduct);
        if (rotAngle > float.Epsilon)
        {
            Vector3 rotAxis = Vector3.Cross(toEffector, toTarget);
            Quaternion q = Quaternion.RotationAxis(rotAxis, rotAngle);
            Matrix m = node.TransformationMatrix;
            Vector3 v = TMOMat.DecomposeMatrix(ref m);
            m *= Matrix.RotationQuaternion(q);
            m.M41 = v.X;
            m.M42 = v.Y;
            m.M43 = v.Z;
            node.TransformationMatrix = m;
        }
        if ((localEffectorP - localTargetP).LengthSq() < 0.1f)
            solved = true;
    }

    public void Dispose()
    {
        foreach (Figure fig in FigureList)
            fig.Dispose();
        if (sphere != null)
            sphere.Dispose();
        if (sprite != null)
            sprite.Dispose();
        if (ztex_zbuf != null)
            ztex_zbuf.Dispose();
        if (ztex_surface != null)
            ztex_surface.Dispose();
        if (ztex != null)
            ztex.Dispose();
        if (dev_zbuf != null)
            dev_zbuf.Dispose();
        if (dev_surface != null)
            dev_surface.Dispose();
        if (effect != null)
            effect.Dispose();
        if (font != null)
            font.Dispose();
        if (device != null)
            device.Dispose();
    }

    /// <summary>
    /// 指定パスからPNGFileを読み込みフィギュアを作成します。
    /// </summary>
    /// <param name="source_file">PNGFileのパス</param>
    public List<Figure> LoadPNGFile(string source_file)
    {
        List<Figure> fig_list = new List<Figure>();

        if (File.Exists(source_file))
        try
        {
            PNGFile png = new PNGFile();
            Figure fig = null;
            TMOFile tmo = null;

            png.Hsav += delegate(string type)
            {
                fig = new Figure();
                fig_list.Add(fig);
            };
            png.Lgta += delegate(Stream dest, int extract_length)
            {
                fig = new Figure();
                fig_list.Add(fig);
            };
            png.Ftmo += delegate(Stream dest, int extract_length)
            {
                tmo = new TMOFile();
                tmo.Load(dest);
                fig.Tmo = tmo;
            };
            png.Figu += delegate(Stream dest, int extract_length)
            {
            };
            png.Ftso += delegate(Stream dest, int extract_length, byte[] opt1)
            {
                TSOFile tso = new TSOFile();
                tso.Load(dest);
                fig.TSOList.Add(tso);
            };
            png.Load(source_file);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex);
        }
        return fig_list;
    }

    /// <summary>
    /// バックバッファをBitmapファイルに保存します。
    /// </summary>
    public void SaveToBitmap()
    {
      using (Surface sf = device.GetBackBuffer(0, 0, BackBufferType.Mono))
      if (sf != null)
          SurfaceLoader.Save("sample.bmp", ImageFileFormat.Bmp, sf);
    }
}
}
