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
using TDCG;

namespace TSOBone
{

//指定boneを先端とするbone treeを辿る。
//bone行列を出力する。
public class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        Debug.Listeners.Add(new TextWriterTraceListener(Console.Out));

        if (args.Length < 1)
        {
            Console.WriteLine("Usage: TSOBone <source file> <node name> <swivel>");
            return;
        }
        string source_file = args[0];
        string node_name = "W_LeftShoulder";
        if (args.Length > 1)
        {
            node_name = args[1];
        }
        float swivel = 0.0f;
        if (args.Length > 2)
        {
            swivel = float.Parse(args[2]);
        }

        TSOFile tso = new TSOFile();
        tso.Load(source_file);

        Program program = new Program(tso);
        program.Solve(swivel);
        //program.DumpNodeTree(node_name);

        using (Viewer viewer = new Viewer())
        using (ViewerForm form = new ViewerForm())
        {
            if (viewer.InitializeApplication(form))
            {
                viewer.TSOFile = tso;
                viewer.RootNodeName = node_name;

                form.Show();

                while (form.Created)
                {
                    viewer.Render();
                    Application.DoEvents();
                }
            }
        }

    }

    internal TSOFile tso;
    internal Dictionary<string, TSONode> nodemap;

    public Program(TSOFile tso)
    {
        this.tso = tso;

        nodemap = new Dictionary<string, TSONode>();
        foreach (TSONode node in tso.nodes)
            nodemap[node.ShortName] = node;
    }

    public Vector3 GetTranslationFromMatrix(ref Matrix m)
    {
        return new Vector3(m.M41, m.M42, m.M43);
    }

    public void Solve(float swivel)
    {
        Matrix mR1 = nodemap["W_LeftArm"].transformation_matrix;
        Vector3 vR1Pos = GetTranslationFromMatrix(ref mR1);

        Matrix mR1RollLocal = nodemap["W_LeftArmRoll"].transformation_matrix;
        Vector3 vR1RollLocalPos = GetTranslationFromMatrix(ref mR1RollLocal);
        Matrix mRyLocal = nodemap["W_LeftForeArm"].transformation_matrix;
        Vector3 vRyLocalPos = GetTranslationFromMatrix(ref mRyLocal);
        Matrix mRy = mRyLocal * mR1RollLocal;
        Vector3 vRyPos = GetTranslationFromMatrix(ref mRy);

        Matrix mRyRollLocal = nodemap["W_LeftForeArmRoll"].transformation_matrix;
        Vector3 vRyRollLocalPos = GetTranslationFromMatrix(ref mRyRollLocal);
        Matrix mR2Local = nodemap["W_LeftHand"].transformation_matrix;
        Matrix mR2 = mR2Local * mRyRollLocal * mRy;
        Vector3 vR2Pos = GetTranslationFromMatrix(ref mR2);

        Vector3 move = new Vector3(-1,0,0);
        Vector3 target = vR2Pos + move;
        Matrix target_m = mR2 * Matrix.Translation(move);

        DumpVector3("vR1Pos", ref vR1Pos);
        DumpVector3("vRyPos", ref vRyPos);
        DumpVector3("vR2Pos", ref vR2Pos);
        DumpVector3("target", ref target);

        float upRollLen = Vector3.Length(vR1RollLocalPos);
        float loRollLen = Vector3.Length(vRyRollLocalPos);
        float upperLength = Vector3.Length(vRyPos);
        float lowerLength = Vector3.Length(vR2Pos - vRyPos);

        Console.WriteLine("upperLength {0:F2}", upperLength);
        Console.WriteLine("lowerLength {0:F2}", lowerLength);

        Limb.Solver solver = new Limb.Solver(upperLength, lowerLength);

        Matrix R1;
        Matrix Ry;
        Matrix R2;

        Console.WriteLine("swivel {0:F2}", swivel);

        if (solver.Solve(out R1, out Ry, out R2, ref target_m, swivel))
        {
            DumpMatrix("R1", ref R1);
            DumpMatrix("Ry", ref Ry);
            DumpMatrix("R2", ref R2);

            Matrix m;
            m = R2 * Matrix.Translation(0,0,lowerLength) * Ry * Matrix.Translation(0,0,upperLength) * R1;
            DumpMatrix("mgoal", ref m);

            /*
            Matrix m;
            MatrixStack stack = new MatrixStack();

            stack.LoadIdentity();

            stack.MultiplyMatrixLocal(R1);
            stack.TranslateLocal(0.0f, 0.0f, upperLength);

            stack.MultiplyMatrixLocal(Ry);
            stack.TranslateLocal(0.0f, 0.0f, lowerLength);

            stack.MultiplyMatrixLocal(R2);
            m = stack.Top;
            DumpMatrix("mgoal", ref m);
            */

            nodemap["W_LeftArmRoll"].transformation_matrix = Matrix.Translation(0,0,upRollLen) * R1;
            nodemap["W_LeftForeArm"].transformation_matrix = Matrix.Translation(0,0,upperLength-upRollLen);
            nodemap["W_LeftForeArmRoll"].transformation_matrix = Matrix.Translation(0,0,loRollLen) * Ry;
            nodemap["W_LeftHand"].transformation_matrix = R2 * Matrix.Translation(0,0,lowerLength-loRollLen);
        }
    }

    public void DumpNodeTree(string node_name)
    {
        TSONode node;

        if (nodemap.TryGetValue(node_name, out node))
        {
            DumpNode(node);
        }
    }

    public void DumpNode(TSONode node)
    {
        DumpMatrix(node.ShortName, ref node.transformation_matrix);
        foreach (TSONode node_child in node.child_nodes)
            DumpNode(node_child);
    }

    public void DumpVector3(string name, ref Vector3 v)
    {
        Console.WriteLine("v {0}", name);
        Console.WriteLine("[ {0:F2}, {1:F2}, {2:F2} ]", v.X, v.Y, v.Z);
    }

    public void DumpMatrix(string name, ref Matrix m)
    {
        Console.WriteLine("m {0}", name);
        Console.WriteLine("[ [ {0:F2}, {1:F2}, {2:F2}, {3:F2} ], ", m.M11, m.M12, m.M13, m.M14);
        Console.WriteLine("  [ {0:F2}, {1:F2}, {2:F2}, {3:F2} ], ", m.M21, m.M22, m.M23, m.M24);
        Console.WriteLine("  [ {0:F2}, {1:F2}, {2:F2}, {3:F2} ], ", m.M31, m.M32, m.M33, m.M34);
        Console.WriteLine("  [ {0:F2}, {1:F2}, {2:F2}, {3:F2} ] ]", m.M41, m.M42, m.M43, m.M44);
    }
}

public class ViewerForm : Form
{
    public ViewerForm()
    {
        this.FormBorderStyle = FormBorderStyle.FixedSingle;
        this.ClientSize = new Size(640, 480);
        this.Text = "TSOBone";

        this.MaximumSize = this.Size;
        this.MinimumSize = this.Size;
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

public class Viewer : IDisposable
{
    internal Device device;
    internal Effect effect;

    internal TSOFile tso;
    internal string root_node_name;

    internal Dictionary<string, TSONode> nodemap;

    public TSOFile TSOFile {
        set {
            tso = value;

            nodemap = new Dictionary<string, TSONode>();
            foreach (TSONode node in tso.nodes)
                nodemap[node.ShortName] = node;
        }
    }
    public string RootNodeName { set { root_node_name = value; } }

    // マウスポイントしているスクリーン座標
    private Point lastScreenPoint = Point.Empty;

    private void form_OnMouseMove(object sender, MouseEventArgs e)
    {
        int dx = e.X - lastScreenPoint.X;
        int dy = e.Y - lastScreenPoint.Y;

        switch (e.Button)
        {
        case MouseButtons.Left:
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

    public bool InitializeApplication(Control control)
    {
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
        }
        catch (DirectXException ex)
        {
            Console.WriteLine("Error: " + ex);
            return false;
        }

        string effect_file = Path.Combine(Application.StartupPath, @"tmps.fx");
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
        effect.Technique = "PhongShader";

        device.RenderState.Lighting = true;

        camera = new Camera();

        proj = Matrix.PerspectiveFovLH( Geometry.DegreeToRadian(30), (float)device.Viewport.Width / (float)device.Viewport.Height, 1.0f, 1000.0f );

        //device.Transform.View = view;
        //device.Transform.Projection = proj;

        box = Mesh.Box(device, 0.2f, 0.2f, 0.2f);
        matrix_stack = new MatrixStack();

        return true;
    }

    private Matrix view;
    private Matrix proj;
    private Mesh box = null;
    private MatrixStack matrix_stack = null;
    private Camera camera = null;

    public void Render()
    {
        camera.Update();
        view = camera.GetViewMatrix();
        view.M31 = -view.M31;
        view.M32 = -view.M32;
        view.M33 = -view.M33;
        view.M34 = -view.M34;

        device.BeginScene();
        device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.CornflowerBlue, 1.0f, 0);

        DumpNodeTree(root_node_name);

        device.EndScene();

        device.Present();
        Thread.Sleep(30);
    }

    public void DumpNodeTree(string node_name)
    {
        TSONode node;

        if (nodemap.TryGetValue(node_name, out node))
        {
            matrix_stack.LoadIdentity();
            DrawBox(matrix_stack.Top, new Vector4(1.0f, 1.0f, 0.0f, 1.0f));
            DumpNode(node);
        }
    }

    public void DumpNode(TSONode node)
    {
        matrix_stack.Push();
        //Console.WriteLine("node {0}", node.ShortName);
        matrix_stack.MultiplyMatrixLocal(node.transformation_matrix);
        DrawBox(matrix_stack.Top, GetNodeColor(node));
        foreach (TSONode child_node in node.child_nodes)
            DumpNode(child_node);
        matrix_stack.Pop();
    }

    public Vector4 GetNodeColor(TSONode node)
    {
        Vector4 color;
        if (node.ShortName == "W_LeftArm")
            color = new Vector4(1.0f, 0.0f, 0.0f, 1.0f);
        else
        if (node.ShortName == "W_LeftForeArm")
            color = new Vector4(0.0f, 1.0f, 0.0f, 1.0f);
        else
        if (node.ShortName == "W_LeftHand")
            color = new Vector4(0.0f, 0.0f, 1.0f, 1.0f);
        else
            color = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);
        return color;
    }

    public void DrawBox(Matrix world, Vector4 color)
    {
        Matrix wvp = world * view * proj;
        effect.SetValue("g_mWorld", world);
        effect.SetValue("g_mWorldIT", Matrix.TransposeMatrix(Matrix.Invert(world)));
        effect.SetValue("g_mWorldViewProj", wvp);

        effect.SetValue("g_vSurfColor", color);

        int npass = effect.Begin(0);
        for (int ipass = 0; ipass < npass; ipass++)
        {
            effect.BeginPass(ipass);
            box.DrawSubset(0);
            effect.EndPass();
        }
        effect.End();
    }

    public void Dispose()
    {
        if (box != null)
            box.Dispose();
        if (effect != null)
            effect.Dispose();
        if (device != null)
            device.Dispose();
    }
}
}
