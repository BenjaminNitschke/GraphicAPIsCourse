using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.Windows;
using Device = SharpDX.Direct3D11.Device;
using System;
using System.Windows.Forms;
using SharpDX;

namespace GraphicsEngine
{
    public class DirectXGraphics : Graphics
    {
        public DirectXGraphics(Form form)
        {
            var swapChainDesc = new SwapChainDescription()
            {
                BufferCount = 2,
                Flags = SwapChainFlags.None,
                IsWindowed = true,
                ModeDescription = new ModeDescription(
                    form.ClientSize.Width, form.ClientSize.Height,
                    new Rational(60, 1), Format.B8G8R8A8_UNorm),
                OutputHandle = form.Handle,
                SampleDescription = new SampleDescription(1, 0),
                SwapEffect = SwapEffect.Discard,
                Usage = Usage.RenderTargetOutput
            };
            Device.CreateWithSwapChain(DriverType.Hardware, DeviceCreationFlags.None,
                swapChainDesc, out device, out swapChain);
            var backBuffer = Texture2D.FromSwapChain<Texture2D>(swapChain, 0);
            backBufferView = new RenderTargetView(device, backBuffer);
            var depthDesc = new Texture2DDescription
            {
                ArraySize = 1,
                BindFlags = BindFlags.DepthStencil,
                Format = Format.D32_Float,
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
        }

        Device device;
        SwapChain swapChain;
        RenderTargetView backBufferView;
        DepthStencilView depthView;

        public void Render()
        {
            device.ImmediateContext.OutputMerger.SetTargets(backBufferView);
            var color = Program.common.BackgroundColor;
            device.ImmediateContext.ClearRenderTargetView(backBufferView,
                new Color4(color.R / 255.0f, color.G / 255.0f, color.B / 255.0f, 1.0f));
            device.ImmediateContext.ClearDepthStencilView(depthView,
                DepthStencilClearFlags.Depth, 0, 0);
        }

        public void Present()
        {
            swapChain.Present(0, PresentFlags.None);
        }

        public void Dispose()
        {
            backBufferView.Dispose();
            swapChain.Dispose();
            device.Dispose();
        }
    }
}
