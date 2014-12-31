using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.ComponentModel;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using Direct3D = Microsoft.DirectX.Direct3D;

namespace TDCG
{
    /// <summary>
    /// セーブファイルの内容を保持します。
    /// </summary>
    public class PNGSaveFile
    {
        /// <summary>
        /// タイプ
        /// </summary>
        public string type = null;
        /// <summary>
        /// 最後に読み込んだライト方向
        /// </summary>
        public Vector3 LightDirection;
        /// <summary>
        /// 最後に読み込んだtmo
        /// </summary>
        public TMOFile Tmo;
        /// <summary>
        /// フィギュアリスト
        /// </summary>
        public List<Figure> FigureList = new List<Figure>();
    }

    /// <summary>
    /// TSOFileをDirect3D上でレンダリングします。
    /// </summary>
public class Viewer : IDisposable
{
    /// <summary>
    /// control
    /// </summary>
    protected Control control;

    /// <summary>
    /// device
    /// </summary>
    protected Device device;

    /// <summary>
    /// effect
    /// </summary>
    protected Effect effect;

    /// <summary>
    /// effect handle for LocalBoneMats
    /// since v0.90
    /// </summary>
    protected EffectHandle handle_LocalBoneMats;

    /// <summary>
    /// effect handle for LightDirForced
    /// since v0.90
    /// </summary>
    protected EffectHandle handle_LightDirForced;

    /// <summary>
    /// effect handle for UVSCR
    /// since v0.91
    /// </summary>
    protected EffectHandle handle_UVSCR;

    /// <summary>
    /// effect handle for ShadowMap
    /// TSOView extension
    /// </summary>
    protected EffectHandle handle_ShadowMap;

    bool shadow_map_enabled;
    /// <summary>
    /// シャドウマップを作成するか
    /// </summary>
    public bool ShadowMapEnabled { get { return shadow_map_enabled; } }

    /// <summary>
    /// ztexture
    /// </summary>
    protected Texture ztex = null;
    /// <summary>
    /// surface of ztexture
    /// </summary>
    protected Surface ztex_surface = null;
    /// <summary>
    /// zbuffer of ztexture
    /// </summary>
    protected Surface ztex_zbuf = null;

    /// sprite
    public Sprite sprite = null;
    internal Line line = null;
    float w_scale = 1.0f;
    float h_scale = 1.0f;
    Rectangle ztex_rect;

    /// <summary>
    /// surface of device
    /// </summary>
    protected Surface dev_surface = null;
    /// <summary>
    /// zbuffer of device
    /// </summary>
    protected Surface dev_zbuf = null;

    /// <summary>
    /// viewerが保持しているフィギュアリスト
    /// </summary>
    public List<Figure> FigureList = new List<Figure>();

    /// <summary>
    /// マウスポイントしているスクリーン座標
    /// </summary>
    protected Point lastScreenPoint = Point.Empty;

    /// <summary>
    /// viewerを生成します。
    /// </summary>
    public Viewer()
    {
        ScreenColor = Color.LightGray;
    }

    /// <summary>
    /// 選択フィギュアの光源方向を設定します。
    /// </summary>
    /// <param name="dir">選択フィギュアの光源方向</param>
    public void SetLightDirection(Vector3 dir)
    {
        foreach (Figure fig in FigureList)
            fig.LightDirection = dir;
    }

    /// マウスボタンを押したときに実行するハンドラ
    protected virtual void form_OnMouseDown(object sender, MouseEventArgs e)
    {
        switch (e.Button)
        {
        case MouseButtons.Left:
            if (Control.ModifierKeys == Keys.Control)
                SetLightDirection(ScreenToOrientation(e.X, e.Y));
            break;
        }

        lastScreenPoint.X = e.X;
        lastScreenPoint.Y = e.Y;
    }

    /// マウスを移動したときに実行するハンドラ
    protected virtual void form_OnMouseMove(object sender, MouseEventArgs e)
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
            break;
        case MouseButtons.Middle:
            Camera.MoveView(-dx*0.125f, dy*0.125f);
            break;
        case MouseButtons.Right:
            Camera.Move(0.0f, 0.0f, -dy*0.125f);
            break;
        }

        lastScreenPoint.X = e.X;
        lastScreenPoint.Y = e.Y;
    }

    // 選択フィギュアindex
    int fig_index = 0;

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
            z = (float)-Math.Sqrt(1.0f - mag);

        return new Vector3(x, y, z);
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
        case ".tso":
            if (! append)
                ClearFigureList();
            LoadTSOFile(source_file);
            break;
        case ".tmo":
            LoadTMOFile(source_file);
            break;
        case ".png":
            AddFigureFromPNGFile(source_file, append);
            break;
        default:
            if (! append)
                ClearFigureList();
            if (Directory.Exists(source_file))
                AddFigureFromTSODirectory(source_file);
            break;
        }
    }

    /// <summary>
    /// フィギュア選択時に呼び出されるハンドラ
    /// </summary>
    public event EventHandler FigureEvent;

    /// <summary>
    /// フィギュアを選択します。
    /// </summary>
    /// <param name="fig_index">フィギュア番号</param>
    public void SetFigureIndex(int fig_index)
    {
        if (fig_index < 0)
            fig_index = 0;
        if (fig_index > FigureList.Count - 1)
            fig_index = 0;
        this.fig_index = fig_index;
        if (FigureEvent != null)
            FigureEvent(this, EventArgs.Empty);
    }

    /// <summary>
    /// 指定ディレクトリからフィギュアを作成して追加します。
    /// </summary>
    /// <param name="source_file">TSOFileを含むディレクトリ</param>
    public void AddFigureFromTSODirectory(string source_file)
    {
        List<TSOFile> tso_list = new List<TSOFile>();
        try
        {
            string[] files = Directory.GetFiles(source_file, "*.TSO");
            foreach (string file in files)
            {
                TSOFile tso = new TSOFile();
                Debug.WriteLine("loading " + file);
                tso.Load(file);
                tso_list.Add(tso);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex);
        }
        Figure fig = new Figure();
        foreach (TSOFile tso in tso_list)
        {
            tso.Open(device, effect);
            fig.TSOList.Add(tso);
        }
        fig.UpdateNodeMapAndBoneMatrices();
        int idx = FigureList.Count;
        FigureList.Add(fig);
        SetFigureIndex(idx);
        if (FigureEvent != null)
            FigureEvent(this, EventArgs.Empty);
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
            fig = FigureList[fig_index];
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
            fig = FigureList[fig_index];
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
        Debug.WriteLine("loading " + source_file);
        using (Stream source_stream = File.OpenRead(source_file))
            LoadTSOFile(source_stream, source_file);
    }

    /// <summary>
    /// 指定ストリームからTSOFileを読み込みます。
    /// </summary>
    /// <param name="source_stream">ストリーム</param>
    public void LoadTSOFile(Stream source_stream)
    {
        LoadTSOFile(source_stream, null);
    }

    /// <summary>
    /// 指定ストリームからTSOFileを読み込みます。
    /// </summary>
    /// <param name="source_stream">ストリーム</param>
    /// <param name="file">ファイル名</param>
    public void LoadTSOFile(Stream source_stream, string file)
    {
        List<TSOFile> tso_list = new List<TSOFile>();
        try
        {
            TSOFile tso = new TSOFile();
            tso.Load(source_stream);
            tso.FileName = file != null ? Path.GetFileNameWithoutExtension(file) : null;
            tso_list.Add(tso);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex);
        }
        Figure fig = GetSelectedOrCreateFigure();
        foreach (TSOFile tso in tso_list)
        {
            tso.Open(device, effect);
            fig.TSOList.Add(tso);
        }
        fig.UpdateNodeMapAndBoneMatrices();
        if (FigureEvent != null)
            FigureEvent(this, EventArgs.Empty);
    }

    /// <summary>
    /// 選択フィギュアを得ます。
    /// </summary>
    public bool TryGetFigure(out Figure fig)
    {
        fig = null;
        if (fig_index < FigureList.Count)
            fig = FigureList[fig_index];
        return fig != null;
    }

    /// 次のフィギュアを選択します。
    public void NextFigure()
    {
        SetFigureIndex(fig_index+1);
    }

    /// <summary>
    /// 指定パスからTMOFileを読み込みます。
    /// </summary>
    /// <param name="source_file">パス</param>
    public void LoadTMOFile(string source_file)
    {
        using (Stream source_stream = File.OpenRead(source_file))
            LoadTMOFile(source_stream);
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
            if (FigureEvent != null)
                FigureEvent(this, EventArgs.Empty);
        }
    }

    /// <summary>
    /// 指定パスからPNGFileを読み込みフィギュアを作成して追加します。
    /// </summary>
    /// <param name="source_file">PNGFile のパス</param>
    /// <param name="append">FigureListを消去せずに追加するか</param>
    public void AddFigureFromPNGFile(string source_file, bool append)
    {
        PNGSaveFile sav = LoadPNGFile(source_file);
        if (sav.FigureList.Count == 0) //POSE png
        {
            Debug.Assert(sav.Tmo != null, "save.Tmo should not be null");
            Figure fig;
            if (TryGetFigure(out fig))
            {
                if (sav.LightDirection != Vector3.Empty)
                    fig.LightDirection = sav.LightDirection;
                fig.Tmo = sav.Tmo;
                //fig.TransformTpo();
                fig.UpdateNodeMapAndBoneMatrices();
                if (FigureEvent != null)
                    FigureEvent(this, EventArgs.Empty);
            }
        }
        else
        {
            if (! append)
                ClearFigureList();

            int idx = FigureList.Count;
            foreach (Figure fig in sav.FigureList)
            {
                fig.OpenTSOFile(device, effect);
                fig.UpdateNodeMapAndBoneMatrices();
                FigureList.Add(fig);
            }
            SetFigureIndex(idx);
        }
    }

    /// <summary>
    /// 指定テクスチャを開き直します。
    /// </summary>
    /// <param name="tex">テクスチャ</param>
    public void OpenTexture(TSOTex tex)
    {
        tex.Open(device);
    }

    private SimpleCamera camera = new SimpleCamera();

    /// <summary>
    /// カメラ
    /// </summary>
    public SimpleCamera Camera
    {
        get {
            return camera;
        }
        set {
            camera = value;
        }
    }

    /// <summary>
    /// world行列
    /// </summary>
    protected Matrix world_matrix = Matrix.Identity;
    /// <summary>
    /// view変換行列
    /// </summary>
    protected Matrix Transform_View = Matrix.Identity;
    /// <summary>
    /// projection変換行列
    /// </summary>
    protected Matrix Transform_Projection = Matrix.Identity;
    /// <summary>
    /// 光源view変換行列
    /// </summary>
    protected Matrix Light_View = Matrix.Identity;
    /// <summary>
    /// 光源projection変換行列
    /// </summary>
    protected Matrix Light_Projection = Matrix.Identity;

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
    /// <param name="shadow_map_enabled">シャドウマップを作成するか</param>
    /// <returns>deviceの作成に成功したか</returns>
    public bool InitializeApplication(Control control, bool shadow_map_enabled)
    {
        this.shadow_map_enabled = shadow_map_enabled;
        SetControl(control);

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

            int adapter_ordinal = Manager.Adapters.Default.Adapter;
            DisplayMode display_mode = Manager.Adapters.Default.CurrentDisplayMode;

            int ret;
            if (Manager.CheckDepthStencilMatch(adapter_ordinal, DeviceType.Hardware, display_mode.Format, pp.BackBufferFormat, DepthFormat.D24S8, out ret))
                pp.AutoDepthStencilFormat = DepthFormat.D24S8;
            else
            if (Manager.CheckDepthStencilMatch(adapter_ordinal, DeviceType.Hardware, display_mode.Format, pp.BackBufferFormat, DepthFormat.D24X8, out ret))
                pp.AutoDepthStencilFormat = DepthFormat.D24X8;
            else
                pp.AutoDepthStencilFormat = DepthFormat.D16;

            int quality;
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
            device = new Device(adapter_ordinal, DeviceType.Hardware, control, flags, pp);

            device.DeviceLost += new EventHandler(OnDeviceLost);
            device.DeviceReset += new EventHandler(OnDeviceReset);
        }
        catch (DirectXException ex)
        {
            Console.WriteLine("Error: " + ex);
            return false;
        }

        Stopwatch sw = new Stopwatch();
        sw.Start();

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

        sw.Stop();
        Console.WriteLine("toonshader.cgfx read time: " + sw.Elapsed);

        handle_LocalBoneMats = effect.GetParameter(null, "LocalBoneMats");
        handle_LightDirForced = effect.GetParameter(null, "LightDirForced");
        handle_UVSCR = effect.GetParameter(null, "UVSCR");
        if (shadow_map_enabled)
        {
            handle_ShadowMap = effect.GetTechnique("ShadowMap");
        }
        sprite = new Sprite(device);
        line = new Line(device);
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

        int dev_zbufw = 0;
        int dev_zbufh = 0;
        dev_zbuf = device.DepthStencilSurface;
        {
            dev_zbufw = dev_surface.Description.Width;
            dev_zbufh = dev_surface.Description.Height;
        }
        Console.WriteLine("dev_zbuf {0}x{1}", dev_zbufw, dev_zbufh);

        if (shadow_map_enabled)
        {
            ztex = new Texture(device, 1024, 1024, 1, Usage.RenderTarget, Format.A8R8G8B8, Pool.Default);
            ztex_surface = ztex.GetSurfaceLevel(0);

            effect.SetValue("texShadowMap", ztex);
            int texw = ztex_surface.Description.Width;
            int texh = ztex_surface.Description.Height;
            Console.WriteLine("ztex {0}x{1}", texw, texh);

            ztex_zbuf = device.CreateDepthStencilSurface(texw, texh, DepthFormat.D16, MultiSampleType.None, 0, false);

            w_scale = (float)devw / texw;
            h_scale = (float)devh / texh;
            ztex_rect = new Rectangle(0, 0, texw, texh);
        }

        Transform_Projection = Matrix.PerspectiveFovRH(
                Geometry.DegreeToRadian(30.0f),
                (float)device.Viewport.Width / (float)device.Viewport.Height,
                1.0f,
                1000.0f );
        // xxx: for w-buffering
        device.Transform.Projection = Transform_Projection;
        effect.SetValue("proj", Transform_Projection);

        if (shadow_map_enabled)
        {
            Light_Projection = Matrix.PerspectiveFovLH(
                Geometry.DegreeToRadian(45.0f),
                1.0f,
                20.0f,
                250.0f );
            effect.SetValue("lightproj", Light_Projection);
        }

        device.SetRenderState(RenderStates.Lighting, false);
        device.SetRenderState(RenderStates.CullMode, (int)Cull.CounterClockwise);

        device.TextureState[0].AlphaOperation = TextureOperation.Modulate;
        device.TextureState[0].AlphaArgument1 = TextureArgument.TextureColor;
        device.TextureState[0].AlphaArgument2 = TextureArgument.Current;

        device.SetRenderState(RenderStates.AlphaBlendEnable, true);
        device.SetRenderState(RenderStates.SourceBlend, (int)Blend.SourceAlpha);
        device.SetRenderState(RenderStates.DestinationBlend, (int)Blend.InvSourceAlpha);
        device.SetRenderState(RenderStates.AlphaTestEnable, true);
        device.SetRenderState(RenderStates.ReferenceAlpha, 0x08);
        device.SetRenderState(RenderStates.AlphaFunction, (int)Compare.GreaterEqual);

        vd = new VertexDeclaration(device, TSOSubMesh.ve);

        //device.RenderState.IndexedVertexBlendEnable = true;
    }

    /// <summary>
    /// toonshader.cgfx に渡す頂点宣言
    /// </summary>
    protected VertexDeclaration vd;

    /// <summary>
    /// 全フィギュアを削除します。
    /// </summary>
    public void ClearFigureList()
    {
        foreach (Figure fig in FigureList)
            fig.Dispose();
        FigureList.Clear();
        SetFigureIndex(0);
        // free meshes and textures.
        Console.WriteLine("Total Memory: {0}", GC.GetTotalMemory(true));
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
            SetFigureIndex(fig_index-1);
        }
        fig = null;
        // free meshes and textures.
        Console.WriteLine("Total Memory: {0}", GC.GetTotalMemory(true));
    }

    bool motionEnabled = false;

    /// <summary>
    /// モーションの有無
    /// </summary>
    public bool MotionEnabled
    {
        get
        {
            return motionEnabled;
        }
        set
        {
            motionEnabled = value;

            if (motionEnabled)
            {
                start_ticks = DateTime.Now.Ticks;
                start_frame_index = frame_index;
            }
        }
    }

    /// <summary>
    /// シャドウマップの有無
    /// </summary>
    public bool ShadowShown = false;

    /// <summary>
    /// スプライトの有無
    /// </summary>
    public bool SpriteShown = false;

    /// <summary>
    /// モーションが有効であるか。
    /// </summary>
    [Obsolete("use MotionEnabled", true)]
    public bool IsMotionEnabled()
    {
        return motionEnabled;
    }

    /// <summary>
    /// モーションの有無を切り替えます。
    /// </summary>
    [Obsolete("use MotionEnabled", true)]
    public void SwitchMotionEnabled()
    {
        MotionEnabled = ! MotionEnabled;
    }
    long start_ticks = 0;
    int start_frame_index = 0;

    /// <summary>
    /// シャドウマップの有無を切り替えます。
    /// </summary>
    [Obsolete("use ShadowShown", true)]
    public void SwitchShadowShown()
    {
        ShadowShown = ! ShadowShown;
    }

    /// <summary>
    /// スプライトの有無を切り替えます。
    /// </summary>
    [Obsolete("use SpriteShown", true)]
    public void SwitchSpriteShown()
    {
        SpriteShown = ! SpriteShown;
    }

    /// <summary>
    /// フレームを進めるのに用いるデリゲート型
    /// </summary>
    public delegate void FrameMovingHandler();

    /// <summary>
    /// フレームを進めるハンドラ
    /// </summary>
    public FrameMovingHandler FrameMoving;

    /// <summary>
    /// 次のシーンフレームに進みます。
    /// </summary>
    public void FrameMove()
    {
        if (motionEnabled)
        {
            int frame_len = GetMaxFrameLength();
            if (frame_len > 0)
            {
                long ticks = DateTime.Now.Ticks - start_ticks;
                long current_frame_index = (long)(start_frame_index + ticks * 0.000006);
                frame_index = (int)(current_frame_index % frame_len);
                Debug.Assert(frame_index >= 0);
                Debug.Assert(frame_index < frame_len);
            }

            //フレーム番号を通知する。
            //camera.SetFrameIndex(frame_index);
        }
        FrameMove(frame_index);

        if (FrameMoving != null)
            FrameMoving();
    }

    /// <summary>
    /// 指定シーンフレームに進みます。
    /// </summary>
    /// <param name="frame_index">フレーム番号</param>
    public void FrameMove(int frame_index)
    {
        if (camera.NeedUpdate)
        {
            camera.Update();
            Transform_View = camera.ViewMatrix;
            // xxx: for w-buffering
            device.Transform.View = Transform_View;
            effect.SetValue("view", Transform_View);
        }

        if (shadow_map_enabled)
        {
            Figure fig;
            if (TryGetFigure(out fig))
            {
                float scale = 40.0f;
                Light_View = Matrix.LookAtLH(
                    fig.LightDirection * -scale,
                    new Vector3(0.0f, 5.0f, 0.0f),
                    new Vector3(0.0f, 1.0f, 0.0f));
                effect.SetValue("lightview", Light_View);
            }
        }

        if (motionEnabled)
        {
            //フレーム番号を通知する。
            foreach (Figure fig in FigureList)
                fig.SetFrameIndex(frame_index);

            //device.Transform.World = world_matrix;
            foreach (Figure fig in FigureList)
                fig.UpdateBoneMatrices();
        }
    }

    private int frame_index = 0;
    /// <summary>
    /// フレーム番号
    /// </summary>
    public int FrameIndex { get { return frame_index; } set { frame_index = value; } }

    /// <summary>
    /// tmo file中で最大のフレーム長さを得ます。
    /// </summary>
    /// <returns>フレーム長さ</returns>
    public int GetMaxFrameLength()
    {
        int max = 0;
        foreach (Figure fig in FigureList)
            if (fig.Tmo.frames != null && max < fig.Tmo.frames.Length)
                max = fig.Tmo.frames.Length;
        return max;
    }

    /// <summary>
    /// レンダリングするのに用いるデリゲート型
    /// </summary>
    public delegate void RenderingHandler();

    /// <summary>
    /// レンダリングするハンドラ
    /// </summary>
    public RenderingHandler Rendering;

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

        if (shadow_map_enabled)
        {
            if (ShadowShown)
            {
                DrawShadowMap();
            }
            else
            {
                ClearShadowMap();
            }
        }

        DrawFigure();

        if (shadow_map_enabled && SpriteShown)
        {
            DrawSprite();
        }

        if (Rendering != null)
            Rendering();

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

    void DrawShadowMap()
    {
        device.SetRenderState(RenderStates.AlphaBlendEnable, false);

        device.SetRenderTarget(0, ztex_surface);
        device.DepthStencilSurface = ztex_zbuf;
        device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.White, 1.0f, 0);

        device.VertexDeclaration = vd;
        effect.Technique = handle_ShadowMap;

        foreach (Figure fig in FigureList)
        foreach (TSOFile tso in fig.TSOList)
        {
            //tso.BeginRender();

            foreach (TSOMesh mesh in tso.meshes)
            foreach (TSOSubMesh sub_mesh in mesh.sub_meshes)
            {
                //device.RenderState.VertexBlend = (VertexBlend)(4 - 1);
                device.SetStreamSource(0, sub_mesh.vb, 0, 52);

                //tso.SwitchShader(sub_mesh);
                effect.SetValue(handle_LocalBoneMats, fig.ClipBoneMatrices(sub_mesh));

                int npass = effect.Begin(0);
                for (int ipass = 0; ipass < npass; ipass++)
                {
                    effect.BeginPass(ipass);
                    device.DrawPrimitives(PrimitiveType.TriangleStrip, 0, sub_mesh.vertices.Length - 2);
                    effect.EndPass();
                }
                effect.End();
            }
            //tso.EndRender();
        }
    }

    void ClearShadowMap()
    {
        device.SetRenderTarget(0, ztex_surface);
        device.DepthStencilSurface = ztex_zbuf;
        device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.White, 1.0f, 0);
    }

    /// スクリーン塗りつぶし色
    public Color ScreenColor { get; set; }

    /// <summary>
    /// UVSCR値を得ます。
    /// </summary>
    /// <returns></returns>
    public Vector4 UVSCR()
    {
        float x = Environment.TickCount * 0.000002f;
        return new Vector4(x, 0.0f, 0.0f, 0.0f);
    }

    /// <summary>
    /// フィギュアを描画します。
    /// </summary>
    protected virtual void DrawFigure()
    {
        device.SetRenderState(RenderStates.AlphaBlendEnable, true);

        device.SetRenderTarget(0, dev_surface);
        device.DepthStencilSurface = dev_zbuf;
        device.Clear(ClearFlags.Target | ClearFlags.ZBuffer | ClearFlags.Stencil, ScreenColor, 1.0f, 0);

        device.VertexDeclaration = vd;
        effect.SetValue(handle_UVSCR, UVSCR());
        foreach (Figure fig in FigureList)
        {
            effect.SetValue(handle_LightDirForced, fig.LightDirForced());
            foreach (TSOFile tso in fig.TSOList)
            {
                tso.BeginRender();

                foreach (TSOMesh mesh in tso.meshes)
                    foreach (TSOSubMesh sub_mesh in mesh.sub_meshes)
                    {
                        //device.RenderState.VertexBlend = (VertexBlend)(4 - 1);
                        device.SetStreamSource(0, sub_mesh.vb, 0, 52);

                        tso.SwitchShader(sub_mesh);
                        effect.SetValue(handle_LocalBoneMats, fig.ClipBoneMatrices(sub_mesh));

                        int npass = effect.Begin(0);
                        for (int ipass = 0; ipass < npass; ipass++)
                        {
                            effect.BeginPass(ipass);
                            device.DrawPrimitives(PrimitiveType.TriangleStrip, 0, sub_mesh.vertices.Length - 2);
                            effect.EndPass();
                        }
                        effect.End();
                    }
                tso.EndRender();
            }
        }
    }

    void DrawSprite()
    {
        device.SetRenderState(RenderStates.AlphaBlendEnable, false);

        sprite.Transform = Matrix.Scaling(w_scale, h_scale, 1.0f);

        sprite.Begin(0);
        sprite.Draw(ztex, ztex_rect, new Vector3(0, 0, 0), new Vector3(0, 0, 0), Color.White);
        sprite.End();
    }

    /// <summary>
    /// Direct3Dメッシュを描画します。
    /// </summary>
    /// <param name="mesh">メッシュ</param>
    /// <param name="wld">ワールド変換行列</param>
    /// <param name="color">描画色</param>
    public void DrawMesh(Mesh mesh, Matrix wld, Vector4 color)
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

    /// <summary>
    /// 内部objectを破棄します。
    /// </summary>
    public void Dispose()
    {
        foreach (Figure fig in FigureList)
            fig.Dispose();
        if (line != null)
            line.Dispose();
        if (sprite != null)
            sprite.Dispose();
        if (ztex_zbuf != null)
            ztex_zbuf.Dispose();
        if (dev_zbuf != null)
            dev_zbuf.Dispose();
        if (dev_surface != null)
            dev_surface.Dispose();
        if (effect != null)
            effect.Dispose();
        if (device != null)
            device.Dispose();
    }

    /// <summary>
    /// 指定パスからPNGFileを読み込みフィギュアを作成します。
    /// </summary>
    /// <param name="source_file">PNGFileのパス</param>
    public PNGSaveFile LoadPNGFile(string source_file)
    {
        PNGSaveFile sav = new PNGSaveFile();

        if (File.Exists(source_file))
        try
        {
            PNGFile png = new PNGFile();
            Figure fig = null;

            png.Hsav += delegate(string type)
            {
                sav.type = type;
                fig = new Figure();
                sav.FigureList.Add(fig);
            };
            png.Pose += delegate(string type)
            {
                sav.type = type;
            };
            png.Scne += delegate(string type)
            {
                sav.type = type;
            };
            png.Cami += delegate(Stream dest, int extract_length)
            {
                byte[] buf = new byte[extract_length];
                dest.Read(buf, 0, extract_length);

                List<float> factor = new List<float>();
                for (int offset = 0; offset < extract_length; offset += sizeof(float))
                {
                    float flo = BitConverter.ToSingle(buf, offset);
                    factor.Add(flo);
                }
                camera.Reset();
                camera.Translation = new Vector3(-factor[0], -factor[1], -factor[2]);
                camera.Angle = new Vector3(-factor[5], -factor[4], -factor[6]);
            };
            png.Lgta += delegate(Stream dest, int extract_length)
            {
                byte[] buf = new byte[extract_length];
                dest.Read(buf, 0, extract_length);

                List<float> factor = new List<float>();
                for (int offset = 0; offset < extract_length; offset += sizeof(float))
                {
                    float flo = BitConverter.ToSingle(buf, offset);
                    factor.Add(flo);
                }

                Matrix m;
                m.M11 = factor[0];
                m.M12 = factor[1];
                m.M13 = factor[2];
                m.M14 = factor[3];

                m.M21 = factor[4];
                m.M22 = factor[5];
                m.M23 = factor[6];
                m.M24 = factor[7];

                m.M31 = factor[8];
                m.M32 = factor[9];
                m.M33 = factor[10];
                m.M34 = factor[11];

                m.M41 = factor[12];
                m.M42 = factor[13];
                m.M43 = factor[14];
                m.M44 = factor[15];

                sav.LightDirection = Vector3.TransformCoordinate(new Vector3(0.0f, 0.0f, -1.0f), m);
            };
            png.Ftmo += delegate(Stream dest, int extract_length)
            {
                sav.Tmo = new TMOFile();
                sav.Tmo.Load(dest);
            };
            png.Figu += delegate(Stream dest, int extract_length)
            {
                fig = new Figure();
                fig.LightDirection = sav.LightDirection;
                fig.Tmo = sav.Tmo;
                sav.FigureList.Add(fig);

                byte[] buf = new byte[extract_length];
                dest.Read(buf, 0, extract_length);

                List<float> ratios = new List<float>();
                for (int offset = 0; offset < extract_length; offset += sizeof(float))
                {
                    float flo = BitConverter.ToSingle(buf, offset);
                    ratios.Add(flo);
                }
                /*
                ◆FIGU
                スライダの位置。値は float型で 0.0 .. 1.0
                    0: 姉妹
                    1: うで
                    2: あし
                    3: 胴まわり
                    4: おっぱい
                    5: つり目たれ目
                    6: やわらか
                 */
                if (fig.slider_matrix != null)
                {
                    fig.slider_matrix.TallRatio = ratios[0];
                    fig.slider_matrix.ArmRatio = ratios[1];
                    fig.slider_matrix.LegRatio = ratios[2];
                    fig.slider_matrix.WaistRatio = ratios[3];
                    fig.slider_matrix.BustRatio = ratios[4];
                    fig.slider_matrix.EyeRatio = ratios[5];
                }

                //fig.TransformTpo();
            };
            png.Ftso += delegate(Stream dest, int extract_length, byte[] opt1)
            {
                TSOFile tso = new TSOFile();
                tso.Load(dest);
                tso.Row = opt1[0];
                fig.TSOList.Add(tso);
            };
            Debug.WriteLine("loading " + source_file);
            png.Load(source_file);

            if (sav.type == "HSAV")
            {
                BMPSaveData data = new BMPSaveData();

                using (Stream stream = File.OpenRead(source_file))
                    data.Read(stream);

                if (fig.slider_matrix != null && data.bitmap.Size == new Size(128, 256))
                {
                    fig.slider_matrix.TallRatio = data.GetSliderValue(4);
                    fig.slider_matrix.ArmRatio = data.GetSliderValue(5);
                    fig.slider_matrix.LegRatio = data.GetSliderValue(6);
                    fig.slider_matrix.WaistRatio = data.GetSliderValue(7);
                    fig.slider_matrix.BustRatio = data.GetSliderValue(0);
                    fig.slider_matrix.EyeRatio = data.GetSliderValue(8);
                }

                for (int i = 0; i < fig.TSOList.Count; i++)
                {
                    TSOFile tso = fig.TSOList[i];
                    string file = data.GetFileName(tso.Row);
                    if (file != "")
                        tso.FileName = Path.GetFileName(file);
                    else
                        tso.FileName = string.Format("{0:X2}", tso.Row);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex);
        }
        return sav;
    }

    /// <summary>
    /// バックバッファをBMP形式でファイルに保存します。
    /// </summary>
    /// <param name="file">ファイル名</param>
    public void SaveToBitmap(string file)
    {
      using (Surface sf = device.GetBackBuffer(0, 0, BackBufferType.Mono))
      if (sf != null)
          SurfaceLoader.Save(file, ImageFileFormat.Bmp, sf);
    }

    /// <summary>
    /// バックバッファをPNG形式でファイルに保存します。
    /// </summary>
    /// <param name="file">ファイル名</param>
    public void SaveToPng(string file)
    {
      using (Surface sf = device.GetBackBuffer(0, 0, BackBufferType.Mono))
      if (sf != null)
          SurfaceLoader.Save(file, ImageFileFormat.Png, sf);
    }

    /// <summary>
    /// 球とレイの衝突を見つけます。
    /// </summary>
    /// <param name="sphereRadius">球の半径</param>
    /// <param name="sphereCenter">球の中心位置</param>
    /// <param name="rayStart">光線の発射位置</param>
    /// <param name="rayOrientation">光線の方向</param>
    /// <param name="collisionPoint">衝突位置</param>
    /// <param name="collisionTime">衝突時刻</param>
    /// <returns>衝突したか</returns>
    public static bool DetectSphereRayCollision(float sphereRadius, ref Vector3 sphereCenter, ref Vector3 rayStart, ref Vector3 rayOrientation, out Vector3 collisionPoint, out float collisionTime)
    {
        collisionTime = 0.0f;
        collisionPoint = Vector3.Empty;

        Vector3 u = rayStart - sphereCenter;
        float a = Vector3.Dot(rayOrientation, rayOrientation);
        float b = Vector3.Dot(rayOrientation, u);
        float c = Vector3.Dot(u, u) - sphereRadius * sphereRadius;
        if (a <= float.Epsilon)
            //誤差
            return false;
        float d = b * b - a * c;
        if (d < 0.0f)
            //衝突しない
            return false;
        collisionTime = (-b - (float)Math.Sqrt(d)) / a;
        collisionPoint = rayStart + rayOrientation * collisionTime;
        return true;
    }

    /// <summary>
    /// viewport行列を作成します。
    /// </summary>
    /// <param name="viewport">viewport</param>
    /// <returns>viewport行列</returns>
    public static Matrix CreateViewportMatrix(Viewport viewport)
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

    /// スクリーン位置をワールド座標へ変換します。
    public static Vector3 ScreenToWorld(float screenX, float screenY, float z, Viewport viewport, Matrix view, Matrix proj)
    {
        //スクリーン位置
        Vector3 v = new Vector3(screenX, screenY, z);

        Matrix inv_m = Matrix.Invert(CreateViewportMatrix(viewport));
        Matrix inv_proj = Matrix.Invert(proj);
        Matrix inv_view = Matrix.Invert(view);

        //スクリーン位置をワールド座標へ変換
        return Vector3.TransformCoordinate(v, inv_m * inv_proj * inv_view);
    }

    /// スクリーン位置をワールド座標へ変換します。
    public Vector3 ScreenToWorld(float screenX, float screenY, float z)
    {
        return ScreenToWorld(screenX, screenY, z, device.Viewport, Transform_View, Transform_Projection);
    }

    /// ワールド座標をスクリーン位置へ変換します。
    public static Vector3 WorldToScreen(Vector3 v, Viewport viewport, Matrix view, Matrix proj)
    {
        return Vector3.TransformCoordinate(v, view * proj * CreateViewportMatrix(viewport));
    }

    /// ワールド座標をスクリーン位置へ変換します。
    public Vector3 WorldToScreen(Vector3 v)
    {
        return WorldToScreen(v, device.Viewport, Transform_View, Transform_Projection);
    }
}
}
