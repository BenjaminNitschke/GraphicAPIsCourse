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
        }

        Device device;
        SwapChain swapChain;
        RenderTargetView backBufferView;

        public void Render()
        {
            device.ImmediateContext.ClearRenderTargetView(backBufferView,
                new Color4(1.0f, 0.0f, 0.0f, 1.0f));
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
