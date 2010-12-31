using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace pmdview
{
    class Viewer
    {
        Device device = null;
        Effect effect = null;
        Control control = null;

        Matrix world_matrix = Matrix.Identity;
        Matrix Transform_View = Matrix.Identity;
        Matrix Transform_Projection = Matrix.Identity;

        static Vector4 CreateColorVector4(float power)
        {
            return new Vector4(power, power, power, 1);
        }

        public bool InitializeGraphics(Control control)
        {
            this.control = control;
            PresentParameters pp = new PresentParameters();

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

            string effect_file = Path.Combine(Application.StartupPath, @"basic.fx");
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

            Transform_Projection = Matrix.PerspectiveFovLH(Geometry.DegreeToRadian(30.0f), (float)device.Viewport.Width / (float)device.Viewport.Height, 1.0f, 1000.0f);
            // xxx: for w-buffering
            device.Transform.Projection = Transform_Projection;
            //effect.SetValue("proj", Transform_Projection);

            Transform_View = Matrix.LookAtLH(new Vector3(0, +10.0f, -44.0f), new Vector3(0, +10.0f, 0), new Vector3(0, 1, 0));
            // xxx: for w-buffering
            device.Transform.View = Transform_View;
            //effect.SetValue("view", Transform_View);

            effect.SetValue("LightDirection", new Vector4(0.0f, -0.0f, +1.0f, 0.0f));
            effect.SetValue("CameraPosition", new Vector4(0.0f, +10.0f, -44.0f, 0.0f));

            {
                float diffuse_power = 0.8f;
                float ambient_power = 0.6f;
                float specular_power = 0.5f;
                effect.SetValue("LightDiffuse", CreateColorVector4(diffuse_power));
                effect.SetValue("LightAmbient", CreateColorVector4(ambient_power));
                effect.SetValue("LightSpecular", CreateColorVector4(specular_power));
            }

            device.RenderState.Lighting = false;
            device.RenderState.CullMode = Cull.CounterClockwise;

            device.TextureState[0].AlphaOperation = TextureOperation.Modulate;
            device.TextureState[0].AlphaArgument1 = TextureArgument.TextureColor;
            device.TextureState[0].AlphaArgument2 = TextureArgument.Current;

            device.RenderState.SourceBlend = Blend.SourceAlpha;
            device.RenderState.DestinationBlend = Blend.InvSourceAlpha;
            device.RenderState.AlphaTestEnable = true;
            device.RenderState.ReferenceAlpha = 0x08;
            device.RenderState.AlphaFunction = Compare.GreaterEqual;

            device.VertexDeclaration = new VertexDeclaration(device, PmdFile.ve);

            return true;
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
                case ".pmd":
                    LoadPmdFile(source_file);
                    break;
                case ".vmd":
                    LoadVmdFile(source_file);
                    break;
            }
        }

        PmdFile pmd = null;

        public void LoadPmdFile(string source_file)
        {
            Console.WriteLine("loading {0}", source_file);
            if (pmd != null)
                pmd.Dispose();
            pmd = new PmdFile();
            pmd.Load(source_file);
            vmd = pmd.GenerateVmd();
            UpdateNodemap();
            UpdatePose(frame_index);
            UpdateBoneMatrices();
            pmd.WriteVertexBuffer(device);
            pmd.WriteIndexBuffer(device);
            pmd.Open(device, effect);
            control.Invalidate();
        }

        VmdFile vmd = null;

        public void LoadVmdFile(string source_file)
        {
            Console.WriteLine("loading {0}", source_file);
            vmd = new VmdFile();
            vmd.Load(source_file);
            UpdateNodemap();
            UpdatePose(frame_index);
            UpdateBoneMatrices();
            pmd.RewriteVertexBuffer(device);
            control.Invalidate();
        }

        long wait = (long)(10000000.0f / 30.0f);
        long start_ticks = 0;
        int start_frame_index = 0;
        int frame_index = 0;

        /// <summary>
        /// 次のシーンフレームに進みます。
        /// </summary>
        public void FrameMove()
        {
            if (vmd == null)
                return;
            int frame_len = vmd.FrameLength;
            if (frame_len > 0)
            {
                long dt = DateTime.Now.Ticks - start_ticks;
                int new_frame_index = (int)((start_frame_index + dt / wait) % frame_len);
                Debug.Assert(new_frame_index >= 0);
                Debug.Assert(new_frame_index < frame_len);
                frame_index = new_frame_index;
            }
            FrameMove(frame_index);
        }

        /// <summary>
        /// 指定モーションフレームに進みます。
        /// </summary>
        public void FrameMove(int frame_index)
        {
            Debug.Assert(frame_index >= 0);
            this.frame_index = frame_index;
            UpdatePose(frame_index);
            UpdateBoneMatrices();
            pmd.RewriteVertexBuffer(device);
            control.Invalidate();
        }

        Dictionary<PmdNode, VmdNode> nodemap = new Dictionary<PmdNode, VmdNode>();

        public void UpdateNodemap()
        {
            nodemap.Clear();
            foreach (PmdNode node in pmd.nodes)
            {
                VmdNode vmd_node;
                if (vmd.nodemap.TryGetValue(node.name, out vmd_node))
                    nodemap[node] = vmd_node;
            }
        }

        public void UpdatePose(int frame_index)
        {
            foreach (PmdNode node in pmd.nodes)
            {
                VmdNode vmd_node;
                if (nodemap.TryGetValue(node, out vmd_node))
                {
                    node.rotation = vmd_node.matrices[frame_index].rotation;
                    node.translation = vmd_node.matrices[frame_index].translation + node.local_translation;
                }
                else
                {
                    node.rotation = Quaternion.Identity;
                    node.translation = node.local_translation;
                }
            }
        }

        MatrixStack matrixStack = new MatrixStack();

        public void UpdateBoneMatrices()
        {
            foreach (PmdNode node in pmd.root_nodes)
            {
                matrixStack.LoadMatrix(Matrix.Identity);
                UpdateBoneMatrices(node);
            }
        }

        /// <summary>
        /// bone行列を更新します。
        /// </summary>
        public void UpdateBoneMatrices(PmdNode node)
        {
            matrixStack.Push();
            Matrix m = Matrix.RotationQuaternion(node.rotation) * Matrix.Translation(node.translation);
            matrixStack.MultiplyMatrixLocal(m);
            node.combined_matrix = matrixStack.Top;
            foreach (PmdNode child_node in node.children)
                UpdateBoneMatrices(child_node);
            matrixStack.Pop();
        }

        /// <summary>
        /// シーンをレンダリングします。
        /// </summary>
        public void Render()
        {
            device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.CornflowerBlue, 1.0f, 0);
            device.BeginScene();

            {
                Matrix world_view_matrix = world_matrix * Transform_View;
                Matrix world_view_projection_matrix = world_view_matrix * Transform_Projection;
                effect.SetValue("WorldMatrix", world_matrix);
                //effect.SetValue("WorldViewMatrix", world_view_matrix);
                effect.SetValue("WorldViewProjMatrix", world_view_projection_matrix);
            }

            if (pmd != null && pmd.vb_position != null)
            {
                device.SetStreamSource(0, pmd.vb_position, 0);
                device.SetStreamSource(1, pmd.vb_texcoord, 0);
                device.Indices = pmd.ib;

                foreach (PmdMaterial material in pmd.materials)
                {
                    effect.SetValue("MaterialDiffuse", material.Diffuse);
                    effect.SetValue("MaterialAmbient", material.Ambient);
                    effect.SetValue("MaterialEmmisive", material.Emissive);
                    effect.SetValue("MaterialSpecular", material.Specular);
                    effect.SetValue("SpecularPower", material.SpecularPower);
                    effect.SetValue("MaterialToon", material.MaterialToon);
                    effect.SetValue("use_texture", material.use_texture);
                    if (material.use_texture)
                        effect.SetValue("ObjectTexture", pmd.texmap[material.texture_file]);
                    effect.SetValue("use_toon", material.use_toon);

                    int npass = effect.Begin(0);
                    for (int ipass = 0; ipass < npass; ipass++)
                    {
                        //Console.WriteLine("render {0} {1}", pmd, ipass);
                        effect.BeginPass(ipass);
                        device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, pmd.vertices.Length, material.face_vertex_start, material.FaceCount);
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
    }
}
