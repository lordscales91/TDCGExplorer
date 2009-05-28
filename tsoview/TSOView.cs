using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
//using System.ComponentModel;
using System.Windows.Forms;
using System.IO;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using Direct3D=Microsoft.DirectX.Direct3D;

namespace TAHdecrypt
{
public class TSOFigure : IDisposable
{
    internal List<TSOFile> TSOList = new List<TSOFile>();
    internal TMOFile tmo = null;
    internal Vector3 position = Vector3.Empty; //中心点

    public TMOFile Tmo
    {
        get { return tmo; }
        set
        {
            tmo = value;
            UpdateTMO();
        }
    }

    internal Dictionary<TSONode, TMONode> nodemap;

    //nodemapとbone行列を更新します。
    public void UpdateNodeMapAndBoneMatrices()
    {
        nodemap.Clear();
        if (tmo.frames != null)
        foreach (TSOFile tso in TSOList)
            AddNodeMap(tso);

        TMOFrame tmo_frame = GetTMOFrame();
        foreach (TSOFile tso in TSOList)
            UpdateBoneMatrices(tso, tmo_frame);
    }

    //TSOFileに対するnodemapを追加します。
    protected void AddNodeMap(TSOFile tso)
    {
        foreach (TSONode tso_node in tso.nodes)
        {
            TMONode tmo_node;
            if (tmo.nodemap.TryGetValue(tso_node.Name, out tmo_node))
                nodemap.Add(tso_node, tmo_node);
        }
    }

    private MatrixStack matrixStack = null;
    private int frame_index = 0;
    private int current_frame_index = -1;

    public TSOFigure()
    {
        tmo = new TMOFile();
        nodemap = new Dictionary<TSONode, TMONode>();
        matrixStack = new MatrixStack();
    }

    //TMOFileを変更したときに呼ぶ必要があります。
    //frame indexと中心点を設定します。
    protected void UpdateTMO()
    {
        frame_index = 0;
        current_frame_index = 0;

        TMONode tmo_node;
        if (tmo.nodemap.TryGetValue("|W_Hips", out tmo_node))
        {
            Matrix m = tmo_node.frame_matrices[0].m;
            position = new Vector3(m.M41, m.M42, -m.M43);
        }
    }

    public void NextTMOFrame()
    {
        if (tmo.frames != null)
        {
            frame_index++;
            if (frame_index >= tmo.frames.Length)
                frame_index = 0;
        }
    }

    //現在のmotion frameを得ます。
    protected TMOFrame GetTMOFrame()
    {
        if (tmo.frames != null)
            return tmo.frames[current_frame_index];
        return null;
    }

    //TSOFileをTSOListに追加します。
    public void AddTSO(TSOFile tso)
    {
        if (tmo.frames != null)
            AddNodeMap(tso);

        current_frame_index = frame_index;

        TMOFrame tmo_frame = GetTMOFrame();
        UpdateBoneMatrices(tso, tmo_frame);

        TSOList.Add(tso);
    }

    //bone行列を更新します。
    //ただしframe indexに変更なければ更新しません。
    public void UpdateBoneMatrices()
    {
        if (frame_index == current_frame_index)
            return;
        current_frame_index = frame_index;

        TMOFrame tmo_frame = GetTMOFrame();
        foreach (TSOFile tso in TSOList)
            UpdateBoneMatrices(tso, tmo_frame);
    }

    protected void UpdateBoneMatrices(TSOFile tso, TMOFrame tmo_frame)
    {
        matrixStack.LoadMatrix(Matrix.Identity);
        UpdateBoneMatrices(tso.nodes[0], tmo_frame);
    }

    protected void UpdateBoneMatrices(TSONode tso_node, TMOFrame tmo_frame)
    {
        matrixStack.Push();

        Matrix transform;

        if (tmo_frame != null)
        {
            // TMO animation
            TMONode tmo_node;
            if (nodemap.TryGetValue(tso_node, out tmo_node))
                transform = tmo_frame.matrices[tmo_node.ID].m;
            else
                transform = tso_node.transformation_matrix;
        }
        else
            transform = tso_node.transformation_matrix;

        matrixStack.MultiplyMatrixLocal(transform);
        tso_node.combined_matrix = matrixStack.Top;

        foreach (TSONode child_node in tso_node.child_nodes)
            UpdateBoneMatrices(child_node, tmo_frame);

        matrixStack.Pop();
    }

    public List<TSOFile> LoadTSOFile(string source_file)
    {
        List<TSOFile> tso_list = new List<TSOFile>();
        try
        {
            if (Path.GetExtension(source_file).ToUpper() == ".TSO")
            {
                TSOFile tso = new TSOFile();
                tso.Load(source_file);
                tso_list.Add(tso);
            }
            else if (Directory.Exists(source_file))
            {
                string[] files = Directory.GetFiles(source_file, "*.TSO");
                foreach (string file in files)
                {
                    TSOFile tso = new TSOFile();
                    tso.Load(file);
                    tso_list.Add(tso);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex);
        }
        return tso_list;
    }

    public void LoadTMOFile(string source_file)
    {
        if (File.Exists(source_file))
        try
        {
            tmo.Load(source_file);
            UpdateTMO();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex);
        }
    }

    public void OpenTSOFile(Device device, Effect effect)
    {
        foreach (TSOFile tso in TSOList)
            tso.Open(device, effect);
    }

    public void Dispose()
    {
        foreach (TSOFile tso in TSOList)
            tso.Dispose();
    }
}

public class TSOFigureForm : Form
{
    Button btn1;
    ListView lv_fig;
    ListView lv;
    DataGridView dg;

    public TSOFigureForm()
    {
        this.ClientSize = new System.Drawing.Size(800, 600);
        this.Text = "TSOGrid";
        //this.AllowDrop = true;

        btn1 = new Button();
        btn1.Location = new Point(10, 10);
        btn1.Text = "&Dump";
        btn1.Click += new EventHandler(btn1_Click);
        this.Controls.Add(btn1);

        lv_fig = new ListView();
        lv_fig.Bounds = new Rectangle(new Point(10, 40), new Size(100, 200));
        lv_fig.View = View.Details;
        lv_fig.FullRowSelect = true;
        lv_fig.HideSelection = false;
        lv_fig.GridLines = true;

        lv_fig.Columns.Add("Name", -2, HorizontalAlignment.Left);
        lv_fig.SelectedIndexChanged += lv_fig_SelectedIndexChanged;

        this.Controls.Add(lv_fig);

        lv = new ListView();
        lv.Bounds = new Rectangle(new Point(120, 40), new Size(300, 200));
        lv.View = View.Details;
        lv.FullRowSelect = true;
        lv.HideSelection = false;
        lv.GridLines = true;

        lv.Columns.Add("Name", -2, HorizontalAlignment.Left);
        lv.Columns.Add("File", -2, HorizontalAlignment.Left);
        lv.SelectedIndexChanged += lv_SelectedIndexChanged;

        this.Controls.Add(lv);

        dg = new DataGridView();
        dg.Bounds = new Rectangle(new Point(10, 250), new Size(410, 250));
        dg.EditMode = DataGridViewEditMode.EditOnEnter;
        dg.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        this.Controls.Add(dg);
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

    private TSOFigure fig = null;
    private TSOFile tso = null;
    private Shader shader = null;

    public void Clear()
    {
        dg.DataSource = null;
        this.shader = null;
        lv.Items.Clear();
        this.tso = null;
        lv_fig.Items.Clear();
        this.fig = null;
    }

    public void SetTSOFigure(TSOFigure fig)
    {
        this.fig = fig;
        lv_fig.Items.Clear();
        for (int i = 0; i < fig.TSOList.Count; i++)
        {
            TSOFile tso = fig.TSOList[i];
            ListViewItem li = new ListViewItem("TSO #" + i.ToString());
            li.Tag = tso;
            lv_fig.Items.Add(li);
        }
        lv_fig.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
    }

    public void SetTSOFile(TSOFile tso)
    {
        this.tso = tso;
        lv.Items.Clear();
        foreach (TSOSubScript sub_script in tso.sub_scripts)
        {
            ListViewItem li = new ListViewItem(sub_script.Name);
            li.SubItems.Add(sub_script.File);
            li.Tag = sub_script;
            lv.Items.Add(li);
        }
        lv.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
    }

    public void SetShader(Shader shader)
    {
        this.shader = shader;
        dg.DataSource = shader.shader_parameters;
    }

    protected void btn1_Click(object sender, EventArgs e)
    {
        if (shader == null)
            return;
        Console.WriteLine("-- dump shader parameters --");
        foreach (ShaderParameter param in shader.shader_parameters)
            Console.WriteLine("Name {0} F1 {1} F2 {2} F3 {3} F4 {4}", param.Name, param.F1, param.F2, param.F3, param.F4);
    }

    protected void lv_fig_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (lv_fig.SelectedItems.Count == 0)
            return;
        ListViewItem li = lv_fig.SelectedItems[0];
        TSOFile tso = li.Tag as TSOFile;
        SetTSOFile(tso);
    }

    protected void lv_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (lv.SelectedItems.Count == 0)
            return;
        ListViewItem li = lv.SelectedItems[0];
        TSOSubScript sub_script = li.Tag as TSOSubScript;
        SetShader(sub_script.shader);
    }
}

public class TSOSample : IDisposable
{
    internal TSOFigureForm fig_form;
    internal TSOForm form;

    internal Device device;

    internal Direct3D.Font font;
    internal Effect effect;

    private EffectHandle handle_LocalBoneMats;
    private EffectHandle handle_ShadowMap;

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

    internal List<TSOFigure> TSOFigureList = new List<TSOFigure>();

    // キー入力を保持
    internal bool[] keys = new bool[256];
    internal bool[] keysEnabled = new bool[256];

    // カメラ位置
    internal Vector3 camTranslation = new Vector3(0.0f, 0.0f, 0.0f);
    // ライト方向
    internal Vector3 lightDir = new Vector3(0.0f, 0.0f, 1.0f);

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

    private Point lastScreenPoint = Point.Empty;

    private void form_OnMouseDown(object sender, MouseEventArgs e)
    {
        switch (e.Button)
        {
        case MouseButtons.Left:
            if (Control.ModifierKeys == Keys.Control)
                lightDir = ScreenToVector(e.X, e.Y);
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
                lightDir = ScreenToVector(e.X, e.Y);
            else
                camera.Move(-dx, dy, 0.0f);
            break;
        case MouseButtons.Middle:
            camTranslation.X -= dx*0.125f;
            camTranslation.Y += dy*0.125f;
            camera.SetTranslation(camTranslation);
            break;
        case MouseButtons.Right:
            camera.Move(0.0f, 0.0f, dy*0.125f);
            break;
        }

        lastScreenPoint.X = e.X;
        lastScreenPoint.Y = e.Y;
    }

    int figureIndex = 0;

    private void form_OnDragDrop(object sender, DragEventArgs e)
    {
        if (e.Data.GetDataPresent(DataFormats.FileDrop))
        {
            foreach (string src in (string[])e.Data.GetData(DataFormats.FileDrop))
                LoadAnyFile(src);
        }
    }

    const float screenCenterX = 800 / 2.0f;
    const float screenCenterY = 600 / 2.0f;

    public Vector3 ScreenToVector(float screenPointX, float screenPointY)
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

    public void SetFigureIndex(int figureIndex)
    {
        if (figureIndex < 0)
            figureIndex = 0;
        if (figureIndex > TSOFigureList.Count-1)
            figureIndex = 0;
        this.figureIndex = figureIndex;
        TSOFigure fig;
        if (TryGetFigure(out fig))
            fig_form.SetTSOFigure(fig);
        else
            fig_form.Clear();
    }

    public void AddFigureFromTSODirectory(string source_file)
    {
        TSOFigure fig = new TSOFigure();
        List<TSOFile> tso_list = fig.LoadTSOFile(source_file);
        foreach (TSOFile tso in tso_list)
        {
            tso.Open(device, effect);
            fig.AddTSO(tso);
        }
        int idx = TSOFigureList.Count;
        TSOFigureList.Add(fig);
        SetFigureIndex(idx);
    }

    public TSOFigure GetSelectedOrCreateFigure()
    {
        TSOFigure fig;
        if (TSOFigureList.Count == 0)
            fig = new TSOFigure();
        else
            fig = TSOFigureList[figureIndex];
        if (TSOFigureList.Count == 0)
        {
            int idx = TSOFigureList.Count;
            TSOFigureList.Add(fig);
            SetFigureIndex(idx);
        }
        return fig;
    }

    public void LoadTSOFile(string source_file)
    {
        TSOFigure fig = GetSelectedOrCreateFigure();
        List<TSOFile> tso_list = fig.LoadTSOFile(source_file);
        foreach (TSOFile tso in tso_list)
        {
            tso.Open(device, effect);
            fig.AddTSO(tso);
        }
    }

    public bool TryGetFigure(out TSOFigure fig)
    {
        fig = null;
        if (figureIndex < TSOFigureList.Count)
            fig = TSOFigureList[figureIndex];
        return fig != null;
    }

    public void NextTSOFigure()
    {
        SetFigureIndex(figureIndex+1);
    }

    public void LoadTMOFile(string source_file)
    {
        TSOFigure fig;
        if (TryGetFigure(out fig))
        {
            fig.LoadTMOFile(source_file);
            fig.UpdateNodeMapAndBoneMatrices();
            camera.SetCenter(fig.position);
        }
    }

    public void AddFigureFromPNGFile(string source_file)
    {
        List<TSOFigure> fig_list = LoadPNGFile(source_file);
        if (fig_list.Count != 0) //taOb png
        if (fig_list[0].TSOList.Count == 0) //POSE png
        {
            TMOFile tmo = fig_list[0].Tmo;
            TSOFigure fig;
            if (tmo != null && TryGetFigure(out fig))
            {
                fig.Tmo = tmo;
                fig.UpdateNodeMapAndBoneMatrices();
            }
        }
        else
        {
            int idx = TSOFigureList.Count;
            foreach (TSOFigure fig in fig_list)
            {
                fig.OpenTSOFile(device, effect);
                fig.UpdateNodeMapAndBoneMatrices();
                TSOFigureList.Add(fig);
            }
            SetFigureIndex(idx);
        }
        {
            TSOFigure fig;
            if (TryGetFigure(out fig))
                camera.SetCenter(fig.position);
        }
    }

    private void form_OnDragEnter(object sender, DragEventArgs e)
    {
        if (e.Data.GetDataPresent(DataFormats.FileDrop))
        {
            e.Effect = DragDropEffects.Copy;
        }
    }

    private TSOCamera camera = new TSOCamera();
    private Matrix world_matrix = Matrix.Identity;
    private Matrix Transform_View = Matrix.Identity;
    private Matrix Transform_Projection = Matrix.Identity;
    private Matrix Light_View = Matrix.Identity;
    private Matrix Light_Projection = Matrix.Identity;

    public bool InitializeApplication(TSOForm form)
    {
        this.form = form;

        for (int i = 0; i < keysEnabled.Length; i++)
        {
            keysEnabled[i] = true;
        }
        form.KeyDown += new KeyEventHandler(form_OnKeyDown);
        form.KeyUp += new KeyEventHandler(form_OnKeyUp);

        form.MouseDown += new MouseEventHandler(form_OnMouseDown);
        form.MouseMove += new MouseEventHandler(form_OnMouseMove);

        form.DragDrop += new DragEventHandler(form_OnDragDrop);
        form.DragEnter += new DragEventHandler(form_OnDragEnter);

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
        handle_LocalBoneMats = effect.GetParameter(null, "LocalBoneMats");
        handle_ShadowMap = effect.GetTechnique("ShadowMap");
        effect.ValidateTechnique(effect.Technique);

        int devw = 0;
        int devh = 0;
        using (Surface surface = device.DepthStencilSurface)
        {
            devw = surface.Description.Width;
            devh = surface.Description.Height;
        }
        Console.WriteLine("dev {0}x{1}", devw, devh);

        ztex = new Texture(device, 1024, 1024, 1, Usage.RenderTarget, Format.A8R8G8B8, Pool.Default);
        effect.SetValue("texShadowMap", ztex);
        ztex_surface = ztex.GetSurfaceLevel(0);
        {
            ztexw = ztex_surface.Description.Width;
            ztexh = ztex_surface.Description.Height;
        }
        Console.WriteLine("ztex {0}x{1}", ztexw, ztexh);

        ztex_zbuf = device.CreateDepthStencilSurface(ztexw, ztexh, DepthFormat.D16, MultiSampleType.None, 0, false);

        sprite = new Sprite(device);
        w_scale = (float)devw / ztexw;
        h_scale = (float)devh / ztexh;

        dev_surface = device.GetRenderTarget(0);
        dev_zbuf = device.DepthStencilSurface;

        camera.Update();

        Transform_Projection = Matrix.PerspectiveFovLH(
                Geometry.DegreeToRadian(30.0f),
                (float)device.Viewport.Width / (float)device.Viewport.Height,
                1.0f,
                1000.0f );
        Light_Projection = Matrix.PerspectiveFovLH(
                Geometry.DegreeToRadian(45.0f),
                1.0f,
                20.0f,
                250.0f );

        // xxx: for w-buffering
        device.Transform.Projection = Transform_Projection;

        if (effect != null)
        {
            effect.SetValue("proj", Transform_Projection);
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

        device.RenderState.IndexedVertexBlendEnable = true;

        return true;
    }

    internal bool motionEnabled = false;
    internal bool shadowEnabled = false;
    internal bool spriteEnabled = false;

    internal int keySave        = (int)Keys.Return;
    internal int keyMotion      = (int)Keys.Space;
    internal int keyShadow      = (int)Keys.S;
    internal int keySprite      = (int)Keys.Z;
    internal int keyFigure      = (int)Keys.Tab;
    internal int keyDelete      = (int)Keys.Delete;
    internal int keyCameraReset = (int)Keys.D0;

    public void FrameMove()
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
        if (keysEnabled[keyShadow] && keys[keyShadow])
        {
            keysEnabled[keyShadow] = false;
            shadowEnabled = ! shadowEnabled;
        }
        if (keysEnabled[keySprite] && keys[keySprite])
        {
            keysEnabled[keySprite] = false;
            spriteEnabled = ! spriteEnabled;
        }
        if (keysEnabled[keyFigure] && keys[keyFigure])
        {
            keysEnabled[keyFigure] = false;
            NextTSOFigure();
            TSOFigure fig;
            if (TryGetFigure(out fig))
                camera.SetCenter(fig.position);
        }
        if (keysEnabled[keyDelete] && keys[keyDelete])
        {
            keysEnabled[keyDelete] = false;
            foreach (TSOFigure fig in TSOFigureList)
                fig.Dispose();
            TSOFigureList.Clear();
            SetFigureIndex(0);
            GC.Collect(); // free meshes and textures.
        }
        if (keysEnabled[keyCameraReset] && keys[keyCameraReset])
        {
            keysEnabled[keyCameraReset] = false;
            camera.Reset();
        }

        float keyL = 0.0f;
        float keyR = 0.0f;
        float keyU = 0.0f;
        float keyD = 0.0f;
        float keyPush = 0.0f;
        float keyPull = 0.0f;
        float keyZRol = 0.0f;

        if (keys[(int)Keys.Left])
            keyL = 1.0f;
        if (keys[(int)Keys.Right])
            keyR = 1.0f;
        if (keys[(int)Keys.PageUp])
            keyU = 1.0f;
        if (keys[(int)Keys.PageDown])
            keyD = 1.0f;
        if (keys[(int)Keys.Up])
            keyPush = 1.0f;
        if (keys[(int)Keys.Down])
            keyPull = 1.0f;
        if (keys[(int)Keys.A])
            keyZRol = -1.0f;
        if (keys[(int)Keys.D])
            keyZRol = +1.0f;

        camera.Move(keyL - keyR, keyD - keyU, keyPush - keyPull);
        camera.RotZ(Geometry.DegreeToRadian(keyZRol));
        camera.Update();

        Transform_View = camera.GetViewMatrix();
        Transform_View.M31 = -Transform_View.M31;
        Transform_View.M32 = -Transform_View.M32;
        Transform_View.M33 = -Transform_View.M33;
        Transform_View.M34 = -Transform_View.M34;

        // xxx: for w-buffering
        device.Transform.View = Transform_View;
        effect.SetValue("view", Transform_View);

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
        foreach (TSOFigure fig in TSOFigureList)
        foreach (TSOFile tso in fig.TSOList)
            tso.lightDir = lightDir;

        //device.Transform.World = world_matrix;
        foreach (TSOFigure fig in TSOFigureList)
            fig.UpdateBoneMatrices();

        if (motionEnabled)
        {
            foreach (TSOFigure fig in TSOFigureList)
                fig.NextTMOFrame();
        }
    }

    public void Render()
    {
        device.BeginScene();

        device.SetRenderTarget(0, ztex_surface);
        device.DepthStencilSurface = ztex_zbuf;
        device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.White, 1.0f, 0);

        {
            Matrix world_view_matrix = world_matrix * Transform_View;
            Matrix world_view_projection_matrix = world_view_matrix * Transform_Projection;
            effect.SetValue("wld", world_matrix);
            effect.SetValue("wv", world_view_matrix);
            effect.SetValue("wvp", world_view_projection_matrix);
            effect.SetValue("lightview", Light_View);
        }

    if (shadowEnabled)
    {
        device.RenderState.AlphaBlendEnable = false;

        effect.Technique = handle_ShadowMap;

        foreach (TSOFigure fig in TSOFigureList)
        foreach (TSOFile tso in fig.TSOList)
        {
            //tso.BeginRender();

            foreach (TSOMesh tm in tso.meshes)
            foreach (TSOSubMesh tm_sub in tm.sub_meshes)
            {
                device.RenderState.VertexBlend = (VertexBlend)(4 - 1);

                //tso.SwitchShader(tm_sub);
                Matrix[] clipped_boneMatrices = new Matrix[tm_sub.maxPalettes];

                int i = 0;
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
                    tm_sub.dm.DrawSubset(i);
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

        foreach (TSOFigure fig in TSOFigureList)
        foreach (TSOFile tso in fig.TSOList)
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
                effect.SetValue(handle_LocalBoneMats, clipped_boneMatrices);

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

    if (spriteEnabled)
    {
        sprite.Transform = Matrix.Scaling(w_scale, h_scale, 1.0f);
        Rectangle rect = new Rectangle(0, 0, ztexw, ztexh);

        device.RenderState.AlphaBlendEnable = false;

        sprite.Begin(0);
        sprite.Draw(ztex, rect, new Vector3(0, 0, 0), new Vector3(0, 0, 0), Color.White);
        sprite.End();
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
        foreach (TSOFigure fig in TSOFigureList)
            fig.Dispose();
        if (sprite != null)
            sprite.Dispose();
        if (ztex_surface != null)
            ztex_surface.Dispose();
        if (ztex != null)
            ztex.Dispose();
        if (effect != null)
            effect.Dispose();
        if (font != null)
            font.Dispose();
        if (device != null)
            device.Dispose();
    }

    public List<TSOFigure> LoadPNGFile(string source_file)
    {
        List<TSOFigure> fig_list = new List<TSOFigure>();

        if (File.Exists(source_file))
        try
        {
            PNGFile png = new PNGFile();
            TSOFigure fig = null;
            TMOFile tmo = null;

            png.Hsav += delegate(string type)
            {
                fig = new TSOFigure();
                fig_list.Add(fig);
            };
            png.Lgta += delegate(Stream dest, int extract_length)
            {
                fig = new TSOFigure();
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

    public void SaveToBitmap()
    {
      using (Surface sf = device.GetBackBuffer(0, 0, BackBufferType.Mono))
      if (sf != null)
          SurfaceLoader.Save("sample.bmp", ImageFileFormat.Bmp, sf);
    }
}

public class TSOForm : Form
{
    public TSOForm()
    {
        this.ClientSize = new System.Drawing.Size(800, 600);
        this.Text = "TSOView";
        this.AllowDrop = true;
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

static class TSOView
{
    [STAThread]
    static void Main(string[] args) 
    {
        Debug.Listeners.Add(new TextWriterTraceListener(Console.Out));

        using (TSOSample sample = new TSOSample())
        using (TSOFigureForm fig_form = new TSOFigureForm())
        using (TSOForm form = new TSOForm())
        {
            sample.fig_form = fig_form;

            if (sample.InitializeApplication(form))
            {
                foreach (string arg in args)
                    sample.LoadAnyFile(arg);

                fig_form.Show();
                form.Show();
                long wait = (long)(10000000.0f/60.0f);
                long nextTicks = DateTime.Now.Ticks;
                // While the form is still valid, render and process messages
                while (form.Created)
                {
                    if (DateTime.Now.Ticks >= nextTicks)
                    {
                        sample.FrameMove();
                        if (DateTime.Now.Ticks < nextTicks + wait)
                            sample.Render();
                        nextTicks += wait;
                    }
                    Application.DoEvents();
                }
            }

        }
    }
}
}
