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
    /// TSOFile��Direct3D��Ń����_�����O���܂��B
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

    internal Mesh sphere = null;

    /// <summary>
    /// viewer���ێ����Ă���t�B�M���A���X�g
    /// </summary>
    public List<Figure> FigureList = new List<Figure>();

    TMOFile baseTMO = null;

    // ���C�g����
    internal Vector3 lightDir = new Vector3(0.0f, 0.0f, 1.0f);

    Vector3 target = new Vector3(5.0f, 10.0f, 0.0f);

    /// <summary>
    /// �t�^���w�ɂ�����ڕW���ړ����܂��B
    /// </summary>
    public void MoveTarget(float dx, float dy, float dz)
    {
        if (dx == 0 && dy == 0 && dz == 0)
            return;
        target.X -= dx;
        target.Y -= dy;
        target.Z -= dz;
        solved = false;
    }

    /// <summary>
    /// �t�^���w�ɂ�����ڕW���X�N���[�����W�Ŏw�肵�܂��B
    /// </summary>
    private void SetTargetOnScreen(float x, float y)
    {
        Figure fig;
        if (TryGetFigure(out fig))
        {
            Debug.Assert(fig.Tmo.nodemap != null, "fig.Tmo.nodemap should not be null");
            TMONode bone;
            if (fig.Tmo.nodemap.TryGetValue(current_effector_name, out bone))
            {
                Vector3 v = target;
                v = Vector3.TransformCoordinate(v, Transform_View);
                v = Vector3.TransformCoordinate(v, Transform_Projection);
                target = ScreenToWorld(x, y, v.Z, ref Transform_View, ref Transform_Projection);
                solved = false;
            }
        }
    }

    // �}�E�X�|�C���g���Ă���X�N���[�����W
    private Point lastScreenPoint = Point.Empty;

    private void control_OnSizeChanged(object sender, EventArgs e)
    {
        Transform_Projection = Matrix.PerspectiveFovLH(
                Geometry.DegreeToRadian(30.0f),
                (float)control.Width / (float)control.Height,
                1.0f,
                1000.0f );
    }

    private bool clicked = false;

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
        clicked = true;
    }

    private void form_OnMouseUp(object sender, MouseEventArgs e)
    {
        clicked = false;
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
            if (Control.ModifierKeys == Keys.Shift)
                SetTargetOnScreen(e.X, e.Y);
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
        clicked = false;  // correct?
    }

    // �I���t�B�M���Aindex
    int fig_index = 0;

    // �X�N���[���̒��S���W
    private float screenCenterX = 800 / 2.0f;
    private float screenCenterY = 600 / 2.0f;

    /// <summary>
    /// control��ێ����܂��B�X�N���[���̒��S���W���X�V���܂��B
    /// </summary>
    /// <param name="control">control</param>
    protected void SetControl(Control control)
    {
        this.control = control;
        screenCenterX = control.ClientSize.Width / 2.0f;
        screenCenterY = control.ClientSize.Height / 2.0f;
    }

    /// <summary>
    /// �w��X�N���[�����W����X�N���[�����S�֌������x�N�g���𓾂܂��B
    /// </summary>
    /// <param name="screenPointX">�X�N���[��X���W</param>
    /// <param name="screenPointY">�X�N���[��Y���W</param>
    /// <returns>�����x�N�g��</returns>
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

    /// ���ƃ��C�̏Փ˂������܂��B
    public bool DetectSphereRayCollision(float sphereRadius, ref Vector3 sphereCenter, ref Vector3 rayStart, ref Vector3 rayOrientation, out Vector3 collisionPoint, out float collisionTime)
    {
        collisionTime = 0.0f;
        collisionPoint = Vector3.Empty;

        Vector3 u = rayStart - sphereCenter;
        float a = Vector3.Dot(rayOrientation, rayOrientation);
        float b = Vector3.Dot(rayOrientation, u);
        float c = Vector3.Dot(u, u) - sphereRadius*sphereRadius;
        if (a <= float.Epsilon)
            //�덷
            return false;
        float d = b*b - a*c;
        if (d < 0.0f)
            //�Փ˂��Ȃ�
            return false;
        collisionTime = (-b - (float)Math.Sqrt(d))/a;
        collisionPoint = rayStart + rayOrientation*collisionTime;
        return true;
    }

    /// �X�N���[���ʒu�����[���h���W�֕ϊ����܂��B
    public Vector3 ScreenToWorld(float screenX, float screenY, float z, ref Matrix view, ref Matrix proj)
    {
        //viewport�s����쐬
        Matrix m = Matrix.Identity;
        Viewport vp = device.Viewport;
        m.M11 = (float)vp.Width/2;
        m.M22 = -1.0f*(float)vp.Height/2;
        m.M33 = (float)vp.MaxZ - (float)vp.MinZ;
        m.M41 = (float)(vp.X + vp.Width/2);
        m.M42 = (float)(vp.Y + vp.Height/2);
        m.M43 = vp.MinZ;

        //�X�N���[���ʒu
        Vector3 v = new Vector3(screenX, screenY,  z);

        Matrix inv_m = Matrix.Invert(m);
        Matrix inv_proj = Matrix.Invert(proj);
        Matrix inv_view = Matrix.Invert(view);

        //�X�N���[���ʒu�����[���h���W�֕ϊ�
        return Vector3.TransformCoordinate(v, inv_m * inv_proj * inv_view);
    }

    /// <summary>
    /// �C�ӂ̃t�@�C����ǂݍ��݂܂��B
    /// </summary>
    /// <param name="source_file">�C�ӂ̃p�X</param>
    public void LoadAnyFile(string source_file)
    {
        LoadAnyFile(source_file, false);
    }

    /// <summary>
    /// �C�ӂ̃t�@�C����ǂݍ��݂܂��B
    /// </summary>
    /// <param name="source_file">�C�ӂ̃p�X</param>
    /// <param name="append">FigureList�����������ɒǉ����邩</param>
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
    /// �t�B�M���A�I�����ɌĂяo�����n���h��
    /// </summary>
    public event EventHandler FigureEvent;

    /// <summary>
    /// �t�B�M���A��I�����܂��B
    /// </summary>
    /// <param name="fig_index">�t�B�M���A�ԍ�</param>
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
    /// �w��f�B���N�g������t�B�M���A���쐬���Ēǉ����܂��B
    /// </summary>
    /// <param name="source_file">TSOFile���܂ރf�B���N�g��</param>
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
        if (fig.Tmo.frames == null)
            fig.Tmo = BaseTMO;
        fig.UpdateNodeMapAndBoneMatrices();
        int idx = FigureList.Count;
        FigureList.Add(fig);
        SetFigureIndex(idx);
        if (FigureEvent != null)
            FigureEvent(this, EventArgs.Empty);
        camera.SetCenter(fig.Center);
    }

    /// <summary>
    /// �I���t�B�M���A�𓾂܂��B
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
    /// �I���t�B�M���A�𓾂܂��B�Ȃ���΍쐬���܂��B
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
            if (fig.Tmo.frames == null)
                fig.Tmo = BaseTMO;
            int idx = FigureList.Count;
            FigureList.Add(fig);
            SetFigureIndex(idx);
        }
        return fig;
    }

    /// <summary>
    /// �w��p�X����TSOFile��ǂݍ��݂܂��B
    /// </summary>
    /// <param name="source_file">�p�X</param>
    public void LoadTSOFile(string source_file)
    {
        using (Stream source_stream = File.OpenRead(source_file))
            LoadTSOFile(source_stream);
    }

    /// <summary>
    /// �w��X�g���[������TSOFile��ǂݍ��݂܂��B
    /// </summary>
    /// <param name="source_stream">�X�g���[��</param>
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
        fig.UpdateNodeMapAndBoneMatrices();
        if (FigureEvent != null)
            FigureEvent(this, EventArgs.Empty);
        camera.SetCenter(fig.Center);
    }

    /// <summary>
    /// �I���t�B�M���A�𓾂܂��B
    /// </summary>
    public bool TryGetFigure(out Figure fig)
    {
        fig = null;
        if (fig_index < FigureList.Count)
            fig = FigureList[fig_index];
        return fig != null;
    }

    /// ���̃t�B�M���A��I�����܂��B
    public void NextFigure()
    {
        SetFigureIndex(fig_index+1);
        Figure fig;
        if (TryGetFigure(out fig))
            camera.SetCenter(fig.Center);
    }

    /// <summary>
    /// �w��p�X����TMOFile��ǂݍ��݂܂��B
    /// </summary>
    /// <param name="source_file">�p�X</param>
    public void LoadTMOFile(string source_file)
    {
        using (Stream source_stream = File.OpenRead(source_file))
            LoadTMOFile(source_stream);
    }

    /// <summary>
    /// �w��X�g���[������TMOFile��ǂݍ��݂܂��B
    /// </summary>
    /// <param name="source_stream">�X�g���[��</param>
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
            camera.SetCenter(fig.Center);
        }
    }

    /// <summary>
    /// �w��p�X����PNGFile��ǂݍ��݃t�B�M���A���쐬���Ēǉ����܂��B
    /// </summary>
    /// <param name="source_file">PNGFile �̃p�X</param>
    /// <param name="append">FigureList�����������ɒǉ����邩</param>
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
                if (fig.Tmo.frames == null)
                    fig.Tmo = BaseTMO;
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

    private SimpleCamera camera = new SimpleCamera();

    /// <summary>
    /// �J����
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

    private Matrix world_matrix = Matrix.Identity;
    private Matrix Transform_View = Matrix.Identity;
    private Matrix Transform_Projection = Matrix.Identity;
    private Matrix Light_View = Matrix.Identity;
    private Matrix Light_Projection = Matrix.Identity;

    private VertexBuffer vbGauss;

    /// <summary>
    /// device���쐬���܂��B
    /// </summary>
    /// <param name="control">�����_�����O��ƂȂ�control</param>
    /// <returns>device�̍쐬�ɐ���������</returns>
    public bool InitializeApplication(Control control)
    {
        return InitializeApplication(control, false);
    }

    /// <summary>
    /// device���쐬���܂��B
    /// </summary>
    /// <param name="control">�����_�����O��ƂȂ�control</param>
    /// <param name="shadowMapEnabled">�V���h�E�}�b�v���쐬���邩</param>
    /// <returns>device�̍쐬�ɐ���������</returns>
    public bool InitializeApplication(Control control, bool shadowMapEnabled)
    {
        this.shadowMapEnabled = shadowMapEnabled;
        SetControl(control);

        control.SizeChanged += new EventHandler(control_OnSizeChanged);
        control.MouseDown += new MouseEventHandler(form_OnMouseDown);
        control.MouseUp += new MouseEventHandler(form_OnMouseUp);
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
        sphere = Mesh.Sphere(device, 0.25f, 8, 6);
        camera.Update();
        OnDeviceReset(device, null);

        effector_dictionary["|W_Hips|W_Spine_Dummy|W_Spine1|W_Spine2|W_Spine3|W_LeftShoulder_Dummy|W_LeftShoulder|W_LeftArm_Dummy|W_LeftArm|W_LeftArmRoll|W_LeftForeArm|W_LeftForeArmRoll|W_LeftHand"] =
            new string[] {
                "|W_Hips|W_Spine_Dummy|W_Spine1|W_Spine2|W_Spine3|W_LeftShoulder_Dummy|W_LeftShoulder|W_LeftArm_Dummy|W_LeftArm|W_LeftArmRoll|W_LeftForeArm",
                "|W_Hips|W_Spine_Dummy|W_Spine1|W_Spine2|W_Spine3|W_LeftShoulder_Dummy|W_LeftShoulder|W_LeftArm_Dummy|W_LeftArm",
                "|W_Hips|W_Spine_Dummy|W_Spine1|W_Spine2|W_Spine3|W_LeftShoulder_Dummy|W_LeftShoulder" };

        effector_dictionary["|W_Hips|W_Spine_Dummy|W_Spine1|W_Spine2|W_Spine3|W_RightShoulder_Dummy|W_RightShoulder|W_RightArm_Dummy|W_RightArm|W_RightArmRoll|W_RightForeArm|W_RightForeArmRoll|W_RightHand"] =
            new string[] {
                "|W_Hips|W_Spine_Dummy|W_Spine1|W_Spine2|W_Spine3|W_RightShoulder_Dummy|W_RightShoulder|W_RightArm_Dummy|W_RightArm|W_RightArmRoll|W_RightForeArm",
                "|W_Hips|W_Spine_Dummy|W_Spine1|W_Spine2|W_Spine3|W_RightShoulder_Dummy|W_RightShoulder|W_RightArm_Dummy|W_RightArm",
                "|W_Hips|W_Spine_Dummy|W_Spine1|W_Spine2|W_Spine3|W_RightShoulder_Dummy|W_RightShoulder" };

        effector_dictionary["|W_Hips|W_RightHips_Dummy|W_RightUpLeg|W_RightUpLegRoll|W_RightLeg|W_RightLegRoll|W_RightFoot"] =
            new string[] {
                "|W_Hips|W_RightHips_Dummy|W_RightUpLeg|W_RightUpLegRoll|W_RightLeg",
                "|W_Hips|W_RightHips_Dummy|W_RightUpLeg" };

        effector_dictionary["|W_Hips|W_LeftHips_Dummy|W_LeftUpLeg|W_LeftUpLegRoll|W_LeftLeg|W_LeftLegRoll|W_LeftFoot"] =
            new string[] {
                "|W_Hips|W_LeftHips_Dummy|W_LeftUpLeg|W_LeftUpLegRoll|W_LeftLeg",
                "|W_Hips|W_LeftHips_Dummy|W_LeftUpLeg" };

        current_effector_name = "|W_Hips|W_Spine_Dummy|W_Spine1|W_Spine2|W_Spine3|W_LeftShoulder_Dummy|W_LeftShoulder|W_LeftArm_Dummy|W_LeftArm|W_LeftArmRoll|W_LeftForeArm|W_LeftForeArmRoll|W_LeftHand";

        //should be update target when select figure
        this.FigureEvent += delegate(object sender, EventArgs e)
        {
            Figure fig;
            if (TryGetFigure(out fig))
            {
                Debug.Assert(fig.TSOList[0].nodemap != null, "fig.TSOList[0].nodemap should not be null");
                TSONode bone;
                if (fig.TSOList[0].nodemap.TryGetValue(current_effector_name, out bone))
                    target = bone.GetWorldPosition();
            }
        };

        baseTMO = new TMOFile();
        baseTMO.Load(Application.StartupPath + @"\" + @"base.tmo");

        constraint = TMOConstraint.Load(@"angle-GRABIA.xml");

        return true;
    }
    string current_effector_name = null;

    TMOFile BaseTMO
    {
        get { return baseTMO; }
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
    /// �S�t�B�M���A���폜���܂��B
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
    /// �I���t�B�M���A���폜���܂��B
    /// </summary>
    public void RemoveSelectedFigure()
    {
        Figure fig;
        if (TryGetFigure(out fig))
        {
            fig.Dispose();
            FigureList.Remove(fig);
            SetFigureIndex(fig_index-1);
            GC.Collect(); // free meshes and textures.
        }
    }

    internal bool motionEnabled = false;
    internal bool shadowShown = false;
    internal bool SpriteShown = false;

    /// <summary>
    /// ���[�V�����̗L����؂�ւ��܂��B
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
    /// �V���h�E�}�b�v�̗L����؂�ւ��܂��B
    /// </summary>
    public void SwitchShadowShown()
    {
        shadowShown = ! shadowShown;
    }

    /// <summary>
    /// �X�v���C�g�̗L����؂�ւ��܂��B
    /// </summary>
    public void SwitchSpriteShown()
    {
        SpriteShown = ! SpriteShown;
    }

    /// <summary>
    /// ���̃V�[���t���[���ɐi�݂܂��B
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

            //�t���[���ԍ���ʒm����B
            //camera.SetFrameIndex(frame_index);
        }
        FrameMove(frame_index);
    }

    public void FrameMove(int frame_index)
    {
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

        if (motionEnabled)
        {
            //�t���[���ԍ���ʒm����B
            foreach (Figure fig in FigureList)
                fig.SetFrameIndex(frame_index);

            //device.Transform.World = world_matrix;
            foreach (Figure fig in FigureList)
                fig.UpdateBoneMatrices();
        }
        else if (! solved)
        {
            Figure fig;
            if (TryGetFigure(out fig))
            {
                foreach (TSOFile tso in fig.TSOList)
                    Solve(tso, current_effector_name);
                fig.UpdateBoneMatricesWithoutTMOFrame();
            }
        }
    }
    bool solved = false;
    long wait = (long)(10000000.0f / 60.0f);

    //�t���[���ԍ�
    private int frame_index = 0;
    public int FrameIndex { get { return frame_index; } set { frame_index = value; } }

    /// <summary>
    /// tmo file���ōő�̃t���[�������𓾂܂��B
    /// </summary>
    /// <returns>�t���[������</returns>
    public int GetMaxFrameLength()
    {
        int max = 0;
        foreach (Figure fig in FigureList)
            if (fig.Tmo.frames != null && max < fig.Tmo.frames.Length)
                max = fig.Tmo.frames.Length;
        return max;
    }

    /// <summary>
    /// �V�[���������_�����O���܂��B
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
 
    //�Փ˔���
    {
        Figure fig;
        if (TryGetFigure(out fig))
        {
            Debug.Assert(fig.TSOList[0].nodemap != null, "fig.TSOList[0].nodemap should not be null");
            TSONode effector;
            if (fig.TSOList[0].nodemap.TryGetValue(current_effector_name, out effector))
            {
                foreach (string effector_name in effector_dictionary.Keys)
                {
                    TSONode bone;
                    if (fig.TSOList[0].nodemap.TryGetValue(effector_name, out bone))
                    {
                        bool found = FindBoneOnScreenPoint(lastScreenPoint.X, lastScreenPoint.Y, bone);
                        if (found && clicked)
                        {
                            current_effector_name = bone.Name;
                            effector = bone;
                            target = bone.GetWorldPosition();
                        }
                        Vector4 color;
                        if (found)
                            color = new Vector4(1,1,1,1);
                        else
                            color = ( bone == effector ) ? new Vector4(0,1,0,0.5f) : new Vector4(1,0,0,0.5f);

                        DrawMeshSub(sphere, bone.combined_matrix * world_matrix, color);
                    }
                }
            }
        }
    }

    //�t�^���w�ɂ�����ڕW��`��
    {
        DrawMeshSub(sphere, Matrix.Translation(target), new Vector4(1,1,0,0.5f));
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

    void DrawShadowMap()
    {
        device.RenderState.AlphaBlendEnable = false;

        device.SetRenderTarget(0, renderSurfaces[0]);
        device.DepthStencilSurface = renderZ;
        device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.White, 1.0f, 0);

        effect.SetValue("lightview", Light_View);
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
    /// Direct3D���b�V����`�悵�܂��B
    /// </summary>
    /// <param name="mesh">���b�V��</param>
    /// <param name="wld">���[���h�ϊ��s��</param>
    /// <param name="color">�`��F</param>
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

    private bool FindBoneOnScreenPoint(float x, float y, TSONode bone)
    {
        Figure fig;
        if (TryGetFigure(out fig))
        {
            Matrix m = bone.combined_matrix * world_matrix;

            float sphereRadius = 0.25f;
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

    Dictionary<string, string[]> effector_dictionary = new Dictionary<string, string[]>();
    TMOConstraint constraint = null;

    /// <summary>
    /// �t�^���w�ɂ����𓾂܂��B
    /// </summary>
    /// <param name="tso">tso</param>
    /// <param name="effector_name">�G�t�F�N�^node����</param>
    private void Solve(TSOFile tso, string effector_name)
    {
        Debug.Assert(tso.nodemap != null, "tso.nodemap should not be null");
        TSONode effector;
        if (tso.nodemap.TryGetValue(effector_name, out effector))
        {
            foreach (string node_name in effector_dictionary[effector_name])
            {
                TSONode node;
                if (tso.nodemap.TryGetValue(node_name, out node))
                {
                    Solve(effector, node);

                    Vector3 angle = TMOMat.ToAngle(node.Rotation);
                    TMOConstraintItem item = constraint.GetItem(node.ShortName);

                    if (angle.X < item.Min.X)
                        angle.X = item.Min.X;
                    if (angle.X > item.Max.X)
                        angle.X = item.Max.X;

                    if (angle.Y < item.Min.Y)
                        angle.Y = item.Min.Y;
                    if (angle.Y > item.Max.Y)
                        angle.Y = item.Max.Y;

                    if (angle.Z < item.Min.Z)
                        angle.Z = item.Min.Z;
                    if (angle.Z > item.Max.Z)
                        angle.Z = item.Max.Z;

                    node.Rotation = TMOMat.ToQuaternion(angle);
                }
            }
        }
    }

    /// <summary>
    /// Cyclic-Coordinate-Descent (CCD) �@�ɂ��t�^���w�̎����ł��B
    /// </summary>
    /// <param name="effector">�G�t�F�N�^node</param>
    /// <param name="node">�Ώ�node</param>
    public void Solve(TSONode effector, TSONode node)
    {
        Vector3 worldTargetP = target;

        Vector3 worldEffectorP = effector.GetWorldPosition();
        Vector3 worldNodeP = node.GetWorldPosition();

        Matrix invCoord = Matrix.Invert(node.GetWorldCoordinate());
        Vector3 localEffectorP = Vector3.TransformCoordinate(worldEffectorP, invCoord);
        Vector3 localTargetP = Vector3.TransformCoordinate(worldTargetP, invCoord);

        Quaternion q;
        if (RotationVectorToVector(localEffectorP, localTargetP, out q))
            node.Rotation = q * node.Rotation;
        if ((localEffectorP - localTargetP).LengthSq() < 0.1f)
            solved = true;
    }

    /// <summary>
    /// v1��v2�ɍ��킹���]�𓾂܂��B
    /// </summary>
    /// <param name="v1">v1</param>
    /// <param name="v2">v2</param>
    /// <param name="q">q</param>
    /// <returns>��]���K�v�ł��邩</returns>
    public bool RotationVectorToVector(Vector3 v1, Vector3 v2, out Quaternion q)
    {
        Vector3 n1 = Vector3.Normalize(v1);
        Vector3 n2 = Vector3.Normalize(v2);
        float dotProduct = Vector3.Dot(n1, n2);
        float angle = (float)Math.Acos(dotProduct);
        bool needRotate = (angle > float.Epsilon);
        if (needRotate)
        {
            Vector3 axis = Vector3.Cross(n1, n2);
            q = Quaternion.RotationAxis(axis, angle);
        }
        else
            q = Quaternion.Identity;
        return needRotate;
    }

    /// <summary>
    /// ����object��j�����܂��B
    /// </summary>
    public void Dispose()
    {
        foreach (Figure fig in FigureList)
            fig.Dispose();
        if (sphere != null)
            sphere.Dispose();
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
    /// �w��p�X����PNGFile��ǂݍ��݃t�B�M���A���쐬���܂��B
    /// </summary>
    /// <param name="source_file">PNGFile�̃p�X</param>
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
    /// �o�b�N�o�b�t�@��Bitmap�t�@�C���ɕۑ����܂��B
    /// </summary>
    /// <param name="file">�t�@�C����</param>
    public void SaveToBitmap(string file)
    {
      using (Surface sf = device.GetBackBuffer(0, 0, BackBufferType.Mono))
      if (sf != null)
          SurfaceLoader.Save(file, ImageFileFormat.Bmp, sf);
    }

    public void SaveToPng(string file)
    {
      using (Surface sf = device.GetBackBuffer(0, 0, BackBufferType.Mono))
      if (sf != null)
          SurfaceLoader.Save(file, ImageFileFormat.Png, sf);
    }
}
}
