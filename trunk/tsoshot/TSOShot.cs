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
using ArchiveLib;

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

        combinedMatrices[tmo_frame_one].Clear();
        foreach (TSOFile tso in TSOList)
            UpdateBoneMatrices(tso, tmo_frame_one);
        if (tmo.frames != null)
        foreach (TMOFrame tmo_frame in tmo.frames)
        {
            combinedMatrices[tmo_frame].Clear();
            foreach (TSOFile tso in TSOList)
                UpdateBoneMatrices(tso, tmo_frame);
        }
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

    private TMOFrame tmo_frame_one = null;
    private Dictionary<TMOFrame, Dictionary<TSONode, Matrix>> combinedMatrices = null;
    private MatrixStack matrixStack = null;
    private int frame_index = 0;

    public TSOFigure()
    {
        tmo = new TMOFile();
        nodemap = new Dictionary<TSONode, TMONode>();
        tmo_frame_one = new TMOFrame();
        combinedMatrices = new Dictionary<TMOFrame, Dictionary<TSONode, Matrix>>();
        combinedMatrices[tmo_frame_one] = new Dictionary<TSONode, Matrix>();
        matrixStack = new MatrixStack();
    }

    internal char current_row = 'A';

    public void SetCurrentTSOFileName(string filename)
    {
        string basename = Path.GetFileNameWithoutExtension(filename);
        if (basename.Length == 12)
            current_row = basename.ToUpper()[9];
        else
            current_row = 'A';
    }

    public int GetCenterBoneType()
    {
        switch (current_row)
        {
        case 'S'://手首
        case 'X'://腕装備(手甲など)
        case 'Z'://手持ちの小物
            return 1;
        case 'W'://タイツ・ガーター
        case 'I'://靴下
            return 2;
        case 'O'://靴
            return 3;
        default:
            return 0;
        }
    }
    public string GetCenterBoneName()
    {
        switch (current_row)
        {
        case 'A'://身体
            return "|W_Hips";
        case 'E'://瞳
        case 'D'://頭皮(生え際)
        case 'B'://前髪
        case 'C'://後髪
        case 'U'://アホ毛類

        case 'Q'://眼鏡
        case 'V'://眼帯
        case 'Y'://リボン
        case 'P'://頭部装備(帽子等)

        case '3'://イヤリング類
        case '0'://眉毛
        case '2'://ほくろ
        case '1'://八重歯
            return "|W_Hips|W_Spine_Dummy|W_Spine1|W_Spine2|W_Spine3|W_Neck|Head";
        case 'R'://首輪
            return "|W_Hips|W_Spine_Dummy|W_Spine1|W_Spine2|W_Spine3|W_Neck";
        case 'F'://ブラ
        case 'J'://上衣(シャツ等)
        case 'T'://背中(羽など)
        case 'L'://上着オプション(エプロン等)
            return "|W_Hips|W_Spine_Dummy|W_Spine1|W_Spine2|W_Spine3";
        case 'G'://全身下着・水着
        case 'K'://全身衣装(ナース服等)
            return "|W_Hips|W_Spine_Dummy|W_Spine1";
        case 'S'://手首
        case 'X'://腕装備(手甲など)
        case 'Z'://手持ちの小物
            return "|W_Hips";//not reached
        case 'H'://パンツ
        case 'M'://下衣(スカート等)
        case 'N'://尻尾
            return "|W_Hips";
        case 'W'://タイツ・ガーター
        case 'I'://靴下
            return "|W_Hips";//not reached
        case 'O'://靴
            return "|W_Hips";//not reached
        default:
            return "|W_Hips";
        }
    }

    public void UpdateCenterPosition()
    {
        if (TSOList.Count == 0)
            return;

        TSOFile tso = TSOList[0];
        switch (GetCenterBoneType())
        {
        case 1://Hand
            {
            TSONode tso_nodeR;
            TSONode tso_nodeL;
            string boneR = "|W_Hips|W_Spine_Dummy|W_Spine1|W_Spine2|W_Spine3|W_RightShoulder_Dummy|W_RightShoulder|W_RightArm_Dummy|W_RightArm|W_RightArmRoll|W_RightForeArm|W_RightForeArmRoll|W_RightHand";
            string boneL = "|W_Hips|W_Spine_Dummy|W_Spine1|W_Spine2|W_Spine3|W_LeftShoulder_Dummy|W_LeftShoulder|W_LeftArm_Dummy|W_LeftArm|W_LeftArmRoll|W_LeftForeArm|W_LeftForeArmRoll|W_LeftHand";
            if (tso.nodemap.TryGetValue(boneR, out tso_nodeR) && tso.nodemap.TryGetValue(boneL, out tso_nodeL))
            {
                TMOFrame tmo_frame = GetTMOFrame();
                Matrix mR = combinedMatrices[tmo_frame][tso_nodeR];
                Matrix mL = combinedMatrices[tmo_frame][tso_nodeL];
                position = new Vector3((mR.M41+mL.M41)/2.0f, (mR.M42+mL.M42)/2.0f, -(mR.M43+mL.M43)/2.0f);
            }
            else
                Console.WriteLine("bone not found. " + boneR);
            }
            break;
        case 2://Leg
            {
            TSONode tso_nodeR;
            TSONode tso_nodeL;
            string boneR = "|W_Hips|W_RightHips_Dummy|W_RightUpLeg|W_RightUpLegRoll|W_RightLeg";
            string boneL = "|W_Hips|W_LeftHips_Dummy|W_LeftUpLeg|W_LeftUpLegRoll|W_LeftLeg";
            if (tso.nodemap.TryGetValue(boneR, out tso_nodeR) && tso.nodemap.TryGetValue(boneL, out tso_nodeL))
            {
                TMOFrame tmo_frame = GetTMOFrame();
                Matrix mR = combinedMatrices[tmo_frame][tso_nodeR];
                Matrix mL = combinedMatrices[tmo_frame][tso_nodeL];
                position = new Vector3((mR.M41+mL.M41)/2.0f, (mR.M42+mL.M42)/2.0f, -(mR.M43+mL.M43)/2.0f);
            }
            else
                Console.WriteLine("bone not found. " + boneR);
            }
            break;
        case 3://Foot
            {
            TSONode tso_nodeR;
            TSONode tso_nodeL;
            string boneR = "|W_Hips|W_RightHips_Dummy|W_RightUpLeg|W_RightUpLegRoll|W_RightLeg|W_RightLegRoll|W_RightFoot";
            string boneL = "|W_Hips|W_LeftHips_Dummy|W_LeftUpLeg|W_LeftUpLegRoll|W_LeftLeg|W_LeftLegRoll|W_LeftFoot";
            if (tso.nodemap.TryGetValue(boneR, out tso_nodeR) && tso.nodemap.TryGetValue(boneL, out tso_nodeL))
            {
                TMOFrame tmo_frame = GetTMOFrame();
                Matrix mR = combinedMatrices[tmo_frame][tso_nodeR];
                Matrix mL = combinedMatrices[tmo_frame][tso_nodeL];
                position = new Vector3((mR.M41+mL.M41)/2.0f, (mR.M42+mL.M42)/2.0f, -(mR.M43+mL.M43)/2.0f);
            }
            else
                Console.WriteLine("bone not found. " + boneR);
            }
            break;
        default:
            {
            TSONode tso_node;
            string bone = GetCenterBoneName();
            if (tso.nodemap.TryGetValue(bone, out tso_node))
            {
                TMOFrame tmo_frame = GetTMOFrame();
                Matrix m = combinedMatrices[tmo_frame][tso_node];
                position = new Vector3(m.M41, m.M42, -m.M43);
            }
            else
                Console.WriteLine("bone not found. " + bone);
            }
            break;
        }
    }

    //TMOFileを変更したときに呼ぶ必要があります。
    protected void UpdateTMO()
    {
        frame_index = 0;

        combinedMatrices.Clear();
        combinedMatrices[tmo_frame_one] = new Dictionary<TSONode, Matrix>();
        foreach (TMOFrame tmo_frame in tmo.frames)
            combinedMatrices[tmo_frame] = new Dictionary<TSONode, Matrix>();
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
            return tmo.frames[frame_index];
        else
            return tmo_frame_one;
    }

    protected TMOFrame GetTMOFrameSlow(int slow)
    {
        if (tmo.frames != null)
        {
            int i = frame_index - slow;
            if (i < 0)
                i = 0;
            return tmo.frames[i];
        }
        else
            return tmo_frame_one;
    }

    //TSOFileをTSOListに追加します。
    public void AddTSO(TSOFile tso)
    {
        if (tmo.frames != null)
            AddNodeMap(tso);

        TMOFrame tmo_frame = GetTMOFrame();
        UpdateBoneMatrices(tso, tmo_frame);

        TSOList.Add(tso);
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

        if (tmo_frame != tmo_frame_one)
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
        combinedMatrices[tmo_frame][tso_node] = matrixStack.Top;

        foreach (TSONode child_node in tso_node.child_nodes)
            UpdateBoneMatrices(child_node, tmo_frame);

        matrixStack.Pop();
    }

    public Dictionary<TSONode, Matrix> GetCombinedMatrices()
    {
        TMOFrame tmo_frame = GetTMOFrame();
        return combinedMatrices[tmo_frame];
    }

    public Matrix GetCombinedMatrix(TSONode tso_node)
    {
        TMOFrame tmo_frame;
        if (tso_node.slow > 0)
            tmo_frame = GetTMOFrameSlow(tso_node.slow);
        else
            tmo_frame = GetTMOFrame();
        return combinedMatrices[tmo_frame][tso_node];
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

    public List<TSOFile> LoadTSOFile(Stream source)
    {
        List<TSOFile> tso_list = new List<TSOFile>();
        try
        {
            {
                TSOFile tso = new TSOFile();
                tso.Load(source);
                tso_list.Add(tso);
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

public class TSOSample : IDisposable
{
    internal TSOForm form;

    internal Device device;

    internal Direct3D.Font font;
    internal Effect effect;

    private EffectHandle handle_LocalBoneMats;

    internal Surface dev_surface = null;
    internal Surface dev_zbuf = null;

    internal List<TSOFigure> TSOFigureList = new List<TSOFigure>();

    // キー入力を保持
    internal bool[] keys = new bool[256];
    internal bool[] keysEnabled = new bool[256];

    // カメラ位置
    internal Vector3 camTranslation = new Vector3(0.0f, 0.0f, -20.0f);
    // ライト方向
    internal Vector3 lightDir = new Vector3(0.0f, 0.0f, 1.0f);

    // スキンメッシュ関連
    private int maxBones = 16;
    private Matrix[] boneMatrices = null;

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

    const float screenCenterX = 600 / 2.0f;
    const float screenCenterY = 800 / 2.0f;

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

    public void AddFigureFromTSODirectory(string source_file)
    {
        TSOFigure fig = new TSOFigure();
        List<TSOFile> tso_list = fig.LoadTSOFile(source_file);
        foreach (TSOFile tso in tso_list)
        {
            tso.Open(device, effect);
            fig.AddTSO(tso);
        }
        figureIndex = TSOFigureList.Count;
        TSOFigureList.Add(fig);
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
            figureIndex = TSOFigureList.Count;
            TSOFigureList.Add(fig);
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

    public void LoadTSOFile(Stream source)
    {
        TSOFigure fig = GetSelectedOrCreateFigure();
        List<TSOFile> tso_list = fig.LoadTSOFile(source);
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
        figureIndex++;
        if (figureIndex >= TSOFigureList.Count)
            figureIndex = 0;
    }

    public void LoadTMOFile(string source_file)
    {
        TSOFigure fig;
        if (TryGetFigure(out fig))
        {
            fig.LoadTMOFile(source_file);
            fig.UpdateNodeMapAndBoneMatrices();
            fig.UpdateCenterPosition();
            camera.SetCenter(fig.position);
        }
    }

    public void LoadCurrentTMOFile(string source_file)
    {
        current_tmo.Load(source_file);
    }

    public void AssignCurrentTMOFile(string current_TSOFileName)
    {
        TSOFigure fig;
        if (TryGetFigure(out fig))
        {
            fig.SetCurrentTSOFileName(current_TSOFileName);
            fig.Tmo = current_tmo;
            fig.UpdateNodeMapAndBoneMatrices();
            fig.UpdateCenterPosition();
            camera.SetCenter(fig.position);
        }
    }
    TMOFile current_tmo = null;

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
            figureIndex = TSOFigureList.Count;
            foreach (TSOFigure fig in fig_list)
            {
                fig.OpenTSOFile(device, effect);
                fig.UpdateNodeMapAndBoneMatrices();
                TSOFigureList.Add(fig);
            }
        }
        {
            TSOFigure fig;
            if (TryGetFigure(out fig))
            {
                fig.UpdateCenterPosition();
                camera.SetCenter(fig.position);
            }
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

        boneMatrices = new Matrix[maxBones];

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

        int devw = 0;
        int devh = 0;
        using (Surface surface = device.DepthStencilSurface)
        {
            devw = surface.Description.Width;
            devh = surface.Description.Height;
        }
        Console.WriteLine("dev {0}x{1}", devw, devh);

        dev_surface = device.GetRenderTarget(0);
        dev_zbuf = device.DepthStencilSurface;

        camera.Update();
        camera.SetTranslation(camTranslation);

        Transform_Projection = Matrix.PerspectiveFovLH(
                Geometry.DegreeToRadian(30.0f),
                (float)device.Viewport.Width / (float)device.Viewport.Height,
                1.0f,
                1000.0f );

        // xxx: for w-buffering
        device.Transform.Projection = Transform_Projection;

        if (effect != null)
        {
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

        device.RenderState.IndexedVertexBlendEnable = true;

        current_tmo = new TMOFile();
        return true;
    }

    internal bool motionEnabled = false;

    internal int keySave        = (int)Keys.Return;
    internal int keyMotion      = (int)Keys.Space;
    internal int keyFigure      = (int)Keys.Tab;
    internal int keyDelete      = (int)Keys.Delete;

    public void ClearFigureList()
    {
        foreach (TSOFigure fig in TSOFigureList)
            fig.Dispose();
        TSOFigureList.Clear();
        figureIndex = 0;
        GC.Collect(); // free meshes and textures.
    }

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
            ClearFigureList();
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

        foreach (TSOFigure fig in TSOFigureList)
        foreach (TSOFile tso in fig.TSOList)
            tso.lightDir = lightDir;

        //device.Transform.World = world_matrix;

        if (motionEnabled)
        {
            foreach (TSOFigure fig in TSOFigureList)
                fig.NextTMOFrame();
        }
    }

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

        device.SetRenderTarget(0, dev_surface);
        device.DepthStencilSurface = dev_zbuf;
        device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.LightGray, 1.0f, 0);

        device.RenderState.AlphaBlendEnable = true;

        foreach (TSOFigure fig in TSOFigureList)
        foreach (TSOFile tso in fig.TSOList)
        {
            tso.BeginRender();

            foreach (TSOMesh tm in tso.meshes)
            foreach (TSOMesh tm_sub in tm.sub_meshes)
            {
                device.RenderState.VertexBlend = (VertexBlend)(4 - 1);

                tso.SwitchShader(tm_sub);
                Matrix[] clipped_boneMatrices = new Matrix[tm_sub.maxPalettes];

                int i = 0;
                for (int numPalettes = 0; numPalettes < tm_sub.maxPalettes; numPalettes++)
                {
                    //device.Transform.SetWorldMatrixByIndex(numPalettes, combined_matrix);
                    TSONode bone = tm_sub.GetBone(numPalettes);
                    boneMatrices[numPalettes] = bone.GetOffsetMatrix() * fig.GetCombinedMatrix(bone);
                }
                Array.Copy(boneMatrices, clipped_boneMatrices, tm_sub.maxPalettes);
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

    public string GetBitmapPath()
    {
        return Path.ChangeExtension(current_TSOFileName, ".png");
    }

    public void SaveToBitmap()
    {
      using (Surface sf = device.GetBackBuffer(0, 0, BackBufferType.Mono))
      if (sf != null)
      {
          string path = GetBitmapPath();
          string dir = Path.GetDirectoryName(path);
          if (! Directory.Exists(dir))
              Directory.CreateDirectory(dir);
          SurfaceLoader.Save(path, ImageFileFormat.Png, sf);
      }
    }

    public void DumpTAHEntries(string source_file)
    {
        Console.WriteLine("# TAH " + source_file);
        using (FileStream source = File.OpenRead(source_file))
            DumpTAHEntries(source);
    }
    public void DumpTAHEntries(Stream source)
    {
        TAHFile tah = new TAHFile(source);
        try
        {
            tah.LoadEntries();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex);
            return;
        }
        foreach (TAHEntry ent in tah.EntrySet.Entries)
        {
            if (Path.GetExtension(ent.FileName) == ".tso")
            {
                Console.WriteLine(ent.FileName);
                byte[] data = TAHUtil.ReadEntryData(tah.Reader, ent);
                using (MemoryStream ms = new MemoryStream(data))
                    LoadTSOFile(ms);
                current_TSOFileName = ent.FileName;
                AssignCurrentTMOFile(current_TSOFileName);
                {
                    FrameMove();
                    Render();
                    SaveToBitmap();
                    ClearFigureList();
                    Application.DoEvents();
                }
                current_TSOFileName = null;
            }
        }
    }
    string current_TSOFileName = null;

    public void DumpArcEntries(string source_file, IArchive arc)
    {
        try
        {
            arc.Open(source_file);
            if (arc == null)
                return;

            foreach (IArchiveEntry entry in arc)
            {
                if (Path.GetExtension(entry.FileName) == ".tah")
                {
                    Console.WriteLine("# TAH in archive " + entry.FileName);
                    using (MemoryStream ms = new MemoryStream((int)entry.Size))
                    {
                        arc.Extract(entry, ms);
                        ms.Seek(0, SeekOrigin.Begin);
                        DumpTAHEntries(ms);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex);
            return;
        }
    }
    public void DumpZipEntries(string source_file)
    {
        Console.WriteLine("# zip " + source_file);
        IArchive arc = new ZipArchive();
        DumpArcEntries(source_file, arc);
    }
    public void DumpRarEntries(string source_file)
    {
        Console.WriteLine("# rar " + source_file);
        IArchive arc = new RarArchive();
        DumpArcEntries(source_file, arc);
    }
    public void DumpLzhEntries(string source_file)
    {
        Console.WriteLine("# lzh " + source_file);
        IArchive arc = new LzhArchive();
        DumpArcEntries(source_file, arc);
    }

    public void DumpDirEntries(string dir)
    {
        string[] tah_files = Directory.GetFiles(dir, "*.TAH");
        foreach (string file in tah_files)
        {
            DumpTAHEntries(file);
        }
        string[] zip_files = Directory.GetFiles(dir, "*.ZIP");
        foreach (string file in zip_files)
        {
            DumpZipEntries(file);
        }
        string[] rar_files = Directory.GetFiles(dir, "*.RAR");
        foreach (string file in rar_files)
        {
            DumpRarEntries(file);
        }
        string[] lzh_files = Directory.GetFiles(dir, "*.LZH");
        foreach (string file in lzh_files)
        {
            DumpLzhEntries(file);
        }
        string[] entries = Directory.GetDirectories(dir);
        foreach (string entry in entries)
        {
            DumpDirEntries(entry);
        }
    }

    public void TAHDump(string[] args)
    {
        if (args.Length != 1)
        {
            System.Console.WriteLine("Usage: TSOShot <zip file>");
            return;
        }
        string source_file = args[0];
        try
        {
            string ext = Path.GetExtension(source_file).ToUpper();
            if (ext == ".TAH")
            {
                DumpTAHEntries(source_file);
            }
            else if (ext == ".ZIP")
            {
                DumpZipEntries(source_file);
            }
            else if (ext == ".RAR")
            {
                DumpRarEntries(source_file);
            }
            else if (ext == ".LZH")
            {
                DumpLzhEntries(source_file);
            }
            else if (Directory.Exists(source_file))
            {
                DumpDirEntries(source_file);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex);
        }
    }
}

public class TSOForm : Form
{
    public TSOForm()
    {
        this.ClientSize = new System.Drawing.Size(600, 800);
        this.Text = "TSOShot";
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

static class TSOShot
{
    [STAThread]
    static void Main(string[] args) 
    {
        Debug.Listeners.Add(new TextWriterTraceListener(Console.Out));

        using (TSOSample sample = new TSOSample())
        using (TSOForm form = new TSOForm())
        {
            if (sample.InitializeApplication(form))
            {
                sample.LoadCurrentTMOFile("sample.tmo");
                sample.TAHDump(args);

                //form.Show();
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
