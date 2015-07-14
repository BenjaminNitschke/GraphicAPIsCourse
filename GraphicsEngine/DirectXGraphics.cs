using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.Windows;
using Device = SharpDX.Direct3D11.Device;
using System;
using System.Windows.Forms;
using SharpDX;
using SharpDX.D3DCompiler;
using System.IO;
using System.Diagnostics;

namespace GraphicsEngine
{
    public class DirectXGraphics : Graphics
    {
        public DirectXGraphics(Form form)
        {
            CreateSwapChain(form);
            CreateBackBufferWithDepthBuffer(form);
            CreateShader();
            CreateVertexBuffer();
            CreateIndexBuffer();
            CreateWorldViewProjection(form);
        }
        
        private void CreateSwapChain(Form form)
        {
            var swapChainDesc = new SwapChainDescription()
            {
                BufferCount = 1,
                Flags = SwapChainFlags.None,
                IsWindowed = true,
                ModeDescription = new ModeDescription(
                    form.ClientSize.Width, form.ClientSize.Height,
                    new Rational(60, 1), Format.R8G8B8A8_UNorm),
                OutputHandle = form.Handle,
                SampleDescription = new SampleDescription(1, 0),
                SwapEffect = SwapEffect.Discard,
                Usage = Usage.RenderTargetOutput
            };
            Device.CreateWithSwapChain(DriverType.Hardware,
                Debugger.IsAttached ? DeviceCreationFlags.Debug : DeviceCreationFlags.None,
                swapChainDesc, out device, out swapChain);
        }

        private void CreateBackBufferWithDepthBuffer(Form form)
        {
            var backBuffer = Texture2D.FromSwapChain<Texture2D>(swapChain, 0);
            backBufferView = new RenderTargetView(device, backBuffer);
            var depthDesc = new Texture2DDescription
            {
                ArraySize = 1,
                BindFlags = BindFlags.DepthStencil,
                Format = Format.D32_Float_S8X24_UInt,
                CpuAccessFlags = CpuAccessFlags.None,
                Width = form.ClientSize.Width,
                Height = form.ClientSize.Height,
                MipLevels = 1,
                OptionFlags = ResourceOptionFlags.None,
                SampleDescription = new SampleDescription(1, 0),
                Usage = ResourceUsage.Default
            };
            var depthTexture = new Texture2D(device, depthDesc);
            depthView = new DepthStencilView(device, depthTexture);
            Context.Rasterizer.SetViewport(new Viewport(0, 0,
                form.ClientSize.Width, form.ClientSize.Height, 0.0f, 1.0f));
            Context.OutputMerger.SetTargets(depthView, backBufferView);
        }

        private void CreateShader()
        {
            var shaderText = File.ReadAllText("LightShader.fx");
            byte[] vertexBytecode = ShaderBytecode.Compile(shaderText, "VS", "vs_4_0");
            vertexShader = new VertexShader(device, vertexBytecode);
            byte[] pixelBytecode = ShaderBytecode.Compile(shaderText, "PS", "ps_4_0");
            pixelShader = new PixelShader(device, pixelBytecode);
            shaderVertexLayout = new InputLayout(device, vertexBytecode,
                new InputElement[]
                {
                    new InputElement("SV_POSITION", 0, Format.R32G32B32_Float, 0, 0),
                    new InputElement("NORMAL", 0, Format.R32G32B32_Float, 3*4, 0)
                });
            constantBuffer = new SharpDX.Direct3D11.Buffer(device,
                128, ResourceUsage.Default, BindFlags.ConstantBuffer,
                CpuAccessFlags.None, ResourceOptionFlags.None, 0);
            sampler = new SamplerState(device, SamplerStateDescription.Default());
        }

        VertexShader vertexShader;
        PixelShader pixelShader;
        InputLayout shaderVertexLayout;
        SharpDX.Direct3D11.Buffer constantBuffer;
        SamplerState sampler;
        Matrix WorldViewProjection;

        class ShaderCompilationException : Exception
        {
            public ShaderCompilationException(string message)
                : base(message) { }
        }

        private void CreateVertexBuffer()
        {
            const int StrideInBytes = 6 * 4;
            vertices = new SharpDX.Direct3D11.Buffer(device,
                Common.NumberOfVertices * StrideInBytes, ResourceUsage.Dynamic,
                BindFlags.VertexBuffer, CpuAccessFlags.Write,
                ResourceOptionFlags.None, StrideInBytes);
            DataStream dataStream;
            Context.MapSubresource(vertices, MapMode.WriteNoOverwrite,
                SharpDX.Direct3D11.MapFlags.None, out dataStream);
            dataStream.Seek(0, System.IO.SeekOrigin.Begin);
            for (int num = 0; num < Common.NumberOfVertices; num++)
            {
                dataStream.WriteRange(Program.common.vertices, num * 3, 3);
                dataStream.WriteRange(Program.common.normals, num * 3, 3);
            }
            Context.UnmapSubresource(vertices, 0);
            verticesBinding = new VertexBufferBinding(vertices, StrideInBytes, 0);
        }

        private void CreateIndexBuffer()
        {
            indices = new SharpDX.Direct3D11.Buffer(device,
                Common.NumberOfIndices * 2, ResourceUsage.Dynamic,
                BindFlags.IndexBuffer, CpuAccessFlags.Write,
                ResourceOptionFlags.None, 2);
            DataStream dataStream;
            Context.MapSubresource(indices, MapMode.WriteNoOverwrite,
                SharpDX.Direct3D11.MapFlags.None, out dataStream);
            dataStream.Seek(0, System.IO.SeekOrigin.Begin);
            dataStream.WriteRange(Program.common.indices);
            Context.UnmapSubresource(indices, 0);
        }

        Device device;
        public DeviceContext Context
        {
            get { return device.ImmediateContext; }
        }
        SwapChain swapChain;
        RenderTargetView backBufferView;
        DepthStencilView depthView;
        SharpDX.Direct3D11.Buffer vertices;
        VertexBufferBinding verticesBinding;
        SharpDX.Direct3D11.Buffer indices;

        private void CreateWorldViewProjection(Form form)
        {
            Matrix projectionMatrix = Matrix.PerspectiveFovRH(Common.FieldOfView,
                (float)form.ClientSize.Width / (float)form.ClientSize.Height,
                Common.NearPlane, Common.FarPlane);
            Matrix viewMatrix = Matrix.Translation(Common.CameraX, Common.CameraY, Common.CameraZ) *
                Matrix.RotationY(Common.CameraRotationY);
            WorldViewProjection = viewMatrix * projectionMatrix;
        }

        public void Render()
        {
            Clear();
            BindShader();
            RenderCube();
        }

        private void Clear()
        {
            var color = Program.common.BackgroundColor;
            Context.ClearDepthStencilView(depthView, DepthStencilClearFlags.Depth, 1.0f, 0);
            Context.ClearRenderTargetView(backBufferView,
                new Color4(color.R / 255.0f, color.G / 255.0f, color.B / 255.0f, 1.0f));
        }

        private void BindShader()
        {
            Context.UpdateSubresource(ref WorldViewProjection, constantBuffer);
            Context.VertexShader.Set(vertexShader);
            Context.VertexShader.SetConstantBuffer(0, constantBuffer);
            Context.PixelShader.Set(pixelShader);
            Context.PixelShader.SetConstantBuffer(0, constantBuffer);
            Context.PixelShader.SetShaderResource(0, null);
            Context.PixelShader.SetSampler(0, sampler);
        }

        private void RenderCube()
        {
            Context.InputAssembler.InputLayout = shaderVertexLayout;
            Context.InputAssembler.SetVertexBuffers(0, verticesBinding);
            Context.InputAssembler.SetIndexBuffer(indices, Format.R16_UInt, 0);
            Context.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
            Context.DrawIndexed(Common.NumberOfIndices, 0, 0);
        }

        public void Present()
        {
            swapChain.Present(0, PresentFlags.None);
        }

        public void Dispose()
        {
            indices.Dispose();
            vertices.Dispose();
            backBufferView.Dispose();
            swapChain.Dispose();
            device.Dispose();
        }
    }
}