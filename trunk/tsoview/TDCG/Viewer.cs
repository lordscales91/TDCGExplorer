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
    private EffectHandle handle_LocalBoneSels;

    private bool shadowMapEnabled = false;
    internal Texture[] renderTextures = null;
    int ztexw = 0;
    int ztexh = 0;
    internal Surface[] renderSurfaces = null;
    internal Surface renderZ = null;

    internal Sprite sprite = null;
    float w_scale = 1.0f;
    float h_scale = 1.0f;

    internal Surface dev_surface = null;
    internal Surface dev_zbuf = null;

    /// <summary>
    /// viewerが保持しているフィギュアリスト
    /// </summary>
    public List<Figure> FigureList = new List<Figure>();

    // ライト方向
    internal Vector3 lightDir = new Vector3(0.0f, 0.0f, -1.0f);

    // マウスポイントしているスクリーン座標
    internal Point lastScreenPoint = Point.Empty;

    ProportionList pro_list = new ProportionList();

    /// <summary>
    /// viewerを生成します。
    /// </summary>
    public Viewer()
    {
        pro_list.Load();
        Figure.ProportionList = pro_list;
    }

    private void control_OnSizeChanged(object sender, EventArgs e)
    {
        if (device == null)
            return;

        Transform_Projection = Matrix.PerspectiveFovRH(
                Geometry.DegreeToRadian(30.0f),
                (float)control.Width / (float)control.Height,
                1.0f,
                1000.0f );
        // xxx: for w-buffering
        device.Transform.Projection = Transform_Projection;
        effect.SetValue("proj", Transform_Projection);
    }

    /// マウスボタンを押したときに実行するハンドラ
    protected virtual void form_OnMouseDown(object sender, MouseEventArgs e)
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

    /// マウスを移動したときに実行するハンドラ
    protected virtual void form_OnMouseMove(object sender, MouseEventArgs e)
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
        switch (Path.GetExtension(source_file).ToUpper())
        {
        case ".TSO":
            if (! append)
                ClearFigureList();
            LoadTSOFile(source_file);
            break;
        case ".TMO":
            LoadTMOFile(source_file);
            break;
        case ".PNG":
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
                Debug.WriteLine("tso sum vertices count: " + tso.SumVerticesCount().ToString());
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
            fig.AddTSO(tso);
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
            LoadTSOFile(source_stream);
    }

    /// <summary>
    /// 指定ストリームからTSOFileを読み込みます。
    /// </summary>
    /// <param name="source_stream">ストリーム</param>
    public void LoadTSOFile(Stream source_stream)
    {
        List<TSOFile> tso_list = new List<TSOFile>();
        try
        {
            TSOFile tso = new TSOFile();
            tso.Load(source_stream);
            Debug.WriteLine("tso sum vertices count: " + tso.SumVerticesCount().ToString());
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
            fig.AddTSO(tso);
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
        List<Figure> fig_list = LoadPNGFile(source_file);
        if (fig_list.Count != 0) //taOb png
        if (fig_list[0].TSOList.Count == 0) //POSE png
        {
            TMOFile tmo = fig_list[0].Tmo;
            Debug.Assert(tmo != null);
            Figure fig;
            if (TryGetFigure(out fig))
            {
                fig.Tmo = tmo;
                fig.TransformTpo();
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
            foreach (Figure fig in fig_list)
            {
                fig.OpenTSOFile(device, effect);
                fig.UpdateNodeMapAndBoneMatrices();
                FigureList.Add(fig);
            }
            SetFigureIndex(idx);
        }
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

    internal Matrix world_matrix = Matrix.Identity;
    internal Matrix Transform_View = Matrix.Identity;
    internal Matrix Transform_Projection = Matrix.Identity;
    internal Matrix Light_View = Matrix.Identity;
    internal Matrix Light_Projection = Matrix.Identity;

    private VertexBuffer vbGauss;

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
        handle_LocalBoneSels = effect.GetParameter(null, "LocalBoneSels");
        camera.Update();
        OnDeviceReset(device, null);

        return true;
    }

    private void OnDeviceLost(object sender, EventArgs e)
    {
        Console.WriteLine("OnDeviceLost");
        if (renderZ != null)
            renderZ.Dispose();
        if (renderSurfaces != null)
            foreach (Surface surface in renderSurfaces)
                surface.Dispose();
        if (renderTextures != null)
            foreach (Texture texture in renderTextures)
                texture.Dispose();
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

        if (shadowMapEnabled)
        {
            renderTextures = new Texture[3];
            renderSurfaces = new Surface[3];
            for (int i = 0; i < 3; i++)
            {
                renderTextures[i] = new Texture(device, 512, 512, 1, Usage.RenderTarget, Format.G32R32F, Pool.Default);
                renderSurfaces[i] = renderTextures[i].GetSurfaceLevel(0);
            }

            //effect.SetValue("texShadowMap", renderTextures[0]);
            {
                ztexw = renderSurfaces[0].Description.Width;
                ztexh = renderSurfaces[0].Description.Height;
            }
            Console.WriteLine("ztex {0}x{1}", ztexw, ztexh);

            renderZ = device.CreateDepthStencilSurface(ztexw, ztexh, DepthFormat.D24S8, MultiSampleType.None, 0, false);

            w_scale = (float)devw / ztexw;
            h_scale = (float)devh / ztexh;
        }

        Transform_Projection = Matrix.PerspectiveFovRH(
                Geometry.DegreeToRadian(30.0f),
                (float)device.Viewport.Width / (float)device.Viewport.Height,
                1.0f,
                1000.0f );
        // xxx: for w-buffering
        device.Transform.Projection = Transform_Projection;
        effect.SetValue("proj", Transform_Projection);

        if (shadowMapEnabled)
        {
            Light_Projection = Matrix.PerspectiveFovLH(
                Geometry.DegreeToRadian(45.0f),
                1.0f,
                20.0f,
                250.0f );
            effect.SetValue("lightproj", Light_Projection);
        }

        device.RenderState.Lighting = false;
        device.RenderState.CullMode = Cull.CounterClockwise;

        device.SetTextureStageState(0, TextureStageStates.AlphaOperation, (int)TextureOperation.Modulate);
        device.SetTextureStageState(0, TextureStageStates.AlphaArgument1, (int)TextureArgument.TextureColor);
        device.SetTextureStageState(0, TextureStageStates.AlphaArgument2, (int)TextureArgument.Current);

        device.RenderState.SourceBlend = Blend.SourceAlpha; 
        device.RenderState.DestinationBlend = Blend.InvSourceAlpha;
        device.RenderState.AlphaTestEnable = true;
        device.RenderState.ReferenceAlpha = 0x08;
        device.RenderState.AlphaFunction = Compare.GreaterEqual;

        device.RenderState.IndexedVertexBlendEnable = true;

        if (shadowMapEnabled)
        {
            CustomVertex.PositionTextured[] verts = new CustomVertex.PositionTextured[4];
            verts[0] = new CustomVertex.PositionTextured(-1, +1, 0, 0, 0);
            verts[1] = new CustomVertex.PositionTextured(+1, +1, 0, 1, 0);
            verts[2] = new CustomVertex.PositionTextured(-1, -1, 0, 0, 1);
            verts[3] = new CustomVertex.PositionTextured(+1, -1, 0, 1, 1);

            vbGauss = new VertexBuffer(typeof(CustomVertex.PositionTextured), 4, device, Usage.Dynamic | Usage.WriteOnly, CustomVertex.PositionTextured.Format, Pool.Default);
            vbGauss.SetData(verts, 0, LockFlags.None);
        }

        if (shadowMapEnabled)
            SetGaussianWeight(12.5f);
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

    internal bool motionEnabled = false;
    internal bool shadowShown = false;
    internal bool SpriteShown = false;

    /// <summary>
    /// モーションが有効であるか。
    /// </summary>
    public bool IsMotionEnabled()
    {
        return motionEnabled;
    }

    /// <summary>
    /// モーションの有無を切り替えます。
    /// </summary>
    public void SwitchMotionEnabled()
    {
        motionEnabled = ! motionEnabled;

        if (motionEnabled)
        {
            start_ticks = DateTime.Now.Ticks;
            start_frame_index = frame_index;
        }
    }
    long start_ticks = 0;
    int start_frame_index = 0;

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
        if (motionEnabled)
        {
            int frame_len = GetMaxFrameLength();
            if (frame_len > 0)
            {
                long dt = DateTime.Now.Ticks - start_ticks;
                int new_frame_index = (int)((start_frame_index + dt / wait) % frame_len);
                Debug.Assert(new_frame_index >= 0);
                Debug.Assert(new_frame_index < frame_len);
                frame_index = new_frame_index;
            }

            //フレーム番号を通知する。
            //camera.SetFrameIndex(frame_index);
        }
        FrameMove(frame_index);
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

        if (shadowMapEnabled)
        {
            float scale = 40.0f;

            Light_View = Matrix.LookAtLH(
                    lightDir * -scale,
                    new Vector3( 0.0f, 5.0f, 0.0f ), 
                    new Vector3( 0.0f, 1.0f, 0.0f ) );
            effect.SetValue("lightview", Light_View);
        }

        foreach (Figure fig in FigureList)
        foreach (TSOFile tso in fig.TSOList)
            tso.lightDir = lightDir;

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
    long wait = (long)(10000000.0f / 60.0f);

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
    /// 描画モード
    /// </summary>
    public enum ViewMode {
        /// <summary>
        /// トゥーン描画
        /// </summary>
        Toon,
        /// <summary>
        /// ウェイト描画
        /// </summary>
        Weight };
    /// <summary>
    /// 描画モード
    /// </summary>
    public ViewMode view_mode = ViewMode.Toon;

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

        if (shadowMapEnabled)
        {
            if (shadowShown)
            {
                DrawShadowMap();
                DrawGaussianBlur();
            }
            else
            {
                ClearShadowMap();
            }
        }

        DrawFigure();

        if (shadowMapEnabled && SpriteShown)
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
        device.RenderState.AlphaBlendEnable = false;

        device.SetRenderTarget(0, renderSurfaces[0]);
        device.DepthStencilSurface = renderZ;
        device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.White, 1.0f, 0);

        effect.Technique = handle_ShadowMap;

        foreach (Figure fig in FigureList)
        foreach (TSOFile tso in fig.TSOList)
        {
            //tso.BeginRender();

            foreach (TSOMesh frame in tso.meshes)
            foreach (TSOSubMesh sub_mesh in frame.sub_meshes)
            {
                device.RenderState.VertexBlend = (VertexBlend)(4 - 1);

                //tso.SwitchShader(sub_mesh);
                Matrix[] clipped_boneMatrices = new Matrix[sub_mesh.maxPalettes];

                for (int numPalettes = 0; numPalettes < sub_mesh.maxPalettes; numPalettes++)
                {
                    //device.Transform.SetWorldMatrixByIndex(numPalettes, combined_matrix);
                    TSONode tso_node = sub_mesh.GetBone(numPalettes);
                    TMONode tmo_node;
                    if (fig.nodemap.TryGetValue(tso_node, out tmo_node))
                        clipped_boneMatrices[numPalettes] = tso_node.OffsetMatrix * tmo_node.combined_matrix;
                }
                effect.SetValue(handle_LocalBoneMats, clipped_boneMatrices);

                int npass = effect.Begin(0);
                for (int ipass = 0; ipass < npass; ipass++)
                {
                    effect.BeginPass(ipass);
                    sub_mesh.dm.DrawSubset(0);
                    effect.EndPass();
                }
                effect.End();
            }
            //tso.EndRender();
        }
    }

    void SetGaussianWeight(float disp)
    {
        float[] weights = new float[8];
        float t = 0.0f;
        for (int i = 0; i < 8; i++)
        {
            float p = 1.0f + 2.0f * (float)i;
            weights[i] = (float)Math.Exp(-0.5f * p * p / disp);
            t += 2.0f * weights[i];
        }
        for (int i = 0; i < 8; i++)
            weights[i] /= t;
        effect.SetValue("gaussw", weights);
    }

    void DrawGaussianBlur()
    {
        effect.Technique = "Tec4_GaussDraw";

        int npass = effect.Begin(0);

        device.SetRenderTarget(0, renderSurfaces[1]);
        device.DepthStencilSurface = renderZ;
        device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.White, 1.0f, 0);

        effect.SetValue("texShadowMap", renderTextures[0]);
        device.VertexFormat = CustomVertex.PositionTextured.Format;
        {
            effect.BeginPass(0);
            device.SetStreamSource(0, vbGauss, 0);
            device.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 2);
            effect.EndPass();
        }

        device.SetRenderTarget(0, renderSurfaces[2]);
        device.DepthStencilSurface = renderZ;
        device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.White, 1.0f, 0);

        effect.SetValue("texShadowMap", renderTextures[1]);
        device.VertexFormat = CustomVertex.PositionTextured.Format;
        {
            effect.BeginPass(1);
            device.SetStreamSource(0, vbGauss, 0);
            device.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 2);
            effect.EndPass();
        }
        effect.End();
    }

    void ClearShadowMap()
    {
        device.SetRenderTarget(0, renderSurfaces[2]);
        device.DepthStencilSurface = renderZ;
        device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.LightGray, 1.0f, 0);
    }

    /// 選択サブメッシュ
    public TSOSubMesh selected_sub_mesh = null;

    /// 選択ボーン
    public TSONode selected_node = null;

    /// <summary>
    /// スキン変形行列の配列を得ます。
    /// </summary>
    /// <param name="fig">フィギュア</param>
    /// <param name="sub_mesh">サブメッシュ</param>
    /// <returns>スキン変形行列の配列</returns>
    public static Matrix[] ClipBoneMatrices(Figure fig, TSOSubMesh sub_mesh)
    {
        Matrix[] clipped_boneMatrices = new Matrix[sub_mesh.maxPalettes];

        for (int numPalettes = 0; numPalettes < sub_mesh.maxPalettes; numPalettes++)
        {
            TSONode tso_node = sub_mesh.GetBone(numPalettes);
            TMONode tmo_node;
            if (fig.nodemap.TryGetValue(tso_node, out tmo_node))
                clipped_boneMatrices[numPalettes] = tso_node.OffsetMatrix * tmo_node.combined_matrix;
        }
        return clipped_boneMatrices;
    }

    /// <summary>
    /// ボーン選択の配列を得ます。
    /// </summary>
    /// <param name="fig">フィギュア</param>
    /// <param name="sub_mesh">サブメッシュ</param>
    /// <param name="selected_node">選択ボーン</param>
    /// <returns>ボーン選択の配列</returns>
    public static int[] ClipBoneSelections(Figure fig, TSOSubMesh sub_mesh, TSONode selected_node)
    {
        int[] clipped_boneSelections = new int[sub_mesh.maxPalettes];

        for (int numPalettes = 0; numPalettes < sub_mesh.maxPalettes; numPalettes++)
        {
            TSONode tso_node = sub_mesh.GetBone(numPalettes);
            clipped_boneSelections[numPalettes] = (selected_node == tso_node) ? 1 : 0;
        }
        return clipped_boneSelections;
    }

    void DrawFigure()
    {
        device.RenderState.AlphaBlendEnable = true;

        device.SetRenderTarget(0, dev_surface);
        device.DepthStencilSurface = dev_zbuf;
        device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.LightGray, 1.0f, 0);

        if (shadowMapEnabled)
        {
            effect.SetValue("texShadowMap", renderTextures[2]);
        }

        switch (view_mode)
        {
        case ViewMode.Toon:
            foreach (Figure fig in FigureList)
            foreach (TSOFile tso in fig.TSOList)
            {
                tso.BeginRender();

                foreach (TSOMesh frame in tso.meshes)
                foreach (TSOSubMesh sub_mesh in frame.sub_meshes)
                {
                    device.RenderState.VertexBlend = (VertexBlend)(4 - 1);
                    tso.SwitchShader(sub_mesh);

                    effect.SetValue(handle_LocalBoneMats, ClipBoneMatrices(fig, sub_mesh));

                    int npass = effect.Begin(0);
                    for (int ipass = 0; ipass < npass; ipass++)
                    {
                        effect.BeginPass(ipass);
                        sub_mesh.dm.DrawSubset(0);
                        effect.EndPass();
                    }
                    effect.End();
                }
                tso.EndRender();
            }
            break;
        case ViewMode.Weight:
            if (selected_sub_mesh != null)
            {
                Figure fig;
                if (TryGetFigure(out fig))
                {
                    TSOSubMesh sub_mesh = selected_sub_mesh;

                    device.RenderState.VertexBlend = (VertexBlend)(4 - 1);
                    effect.Technique = "BoneCol";
                    effect.SetValue("PenColor", new Vector4(1, 1, 1, 1));

                    effect.SetValue(handle_LocalBoneMats, ClipBoneMatrices(fig, sub_mesh));
                    effect.SetValue(handle_LocalBoneSels, ClipBoneSelections(fig, sub_mesh, selected_node));

                    int npass = effect.Begin(0);
                    for (int ipass = 0; ipass < npass; ipass++)
                    {
                        effect.BeginPass(ipass);
                        sub_mesh.dm.DrawSubset(0);
                        effect.EndPass();
                    }
                    effect.End();
                }
            }
            break;
        }
    }

    void DrawSprite()
    {
        device.RenderState.AlphaBlendEnable = false;

        sprite.Transform = Matrix.Scaling(w_scale, h_scale, 1.0f);
        Rectangle rect = new Rectangle(0, 0, ztexw, ztexh);

        sprite.Begin(0);
        sprite.Draw(renderTextures[2], rect, new Vector3(0, 0, 0), new Vector3(0, 0, 0), Color.White);
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
        if (sprite != null)
            sprite.Dispose();
        if (renderZ != null)
            renderZ.Dispose();
        if (renderSurfaces != null)
            foreach (Surface surface in renderSurfaces)
                surface.Dispose();
        if (renderTextures != null)
            foreach (Texture texture in renderTextures)
                texture.Dispose();
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
            string png_type = null;

            png.Hsav += delegate(string type)
            {
                fig = new Figure();
                fig_list.Add(fig);
                png_type = type;
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
                fig.slide_matrices.TallRatio = ratios[0];
                fig.slide_matrices.ArmRatio = ratios[1];
                fig.slide_matrices.LegRatio = ratios[2];
                fig.slide_matrices.WaistRatio = ratios[3];
                fig.slide_matrices.BustRatio = ratios[4];
                fig.slide_matrices.EyeRatio = ratios[5];

                fig.TransformTpo();
            };
            png.Ftso += delegate(Stream dest, int extract_length, byte[] opt1)
            {
                TSOFile tso = new TSOFile();
                tso.Load(dest);
                Debug.WriteLine("tso sum vertices count: " + tso.SumVerticesCount().ToString());
                fig.TSOList.Add(tso);
            };
            Debug.WriteLine("loading " + source_file);
            png.Load(source_file);

            if (png_type == "HSAV")
            {
                MemoryStream ms = new MemoryStream();
                png.Save(ms);
                ms.Seek(0, SeekOrigin.Begin);
                BMPSaveData data = new BMPSaveData();
                data.Read(ms);

                fig.slide_matrices.TallRatio = data.proportions[1];
                fig.slide_matrices.ArmRatio = data.proportions[2];
                fig.slide_matrices.LegRatio = data.proportions[3];
                fig.slide_matrices.WaistRatio = data.proportions[4];
                fig.slide_matrices.BustRatio = data.proportions[0];
                fig.slide_matrices.EyeRatio = data.proportions[5];
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex);
        }
        return fig_list;
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
}
}
