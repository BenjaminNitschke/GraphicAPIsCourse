using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Platform;
using OpenTK.Platform.Windows;
using System;

namespace GraphicsEngine
{
    public class OpenGLGraphics : Graphics
    {
        public OpenGLGraphics(IntPtr windowHandle)
        {
            var windowInfo = Utilities.CreateWindowsWindowInfo(windowHandle);
            context = new GraphicsContext(GraphicsMode.Default, windowInfo);
            context.MakeCurrent(windowInfo);
            context.LoadAll();
        }

        public void Render()
        {
            GL.ClearColor(1.0f, 0, 0, 1);
            GL.Clear(ClearBufferMask.ColorBufferBit);
        }

        public void Present()
        {
            context.SwapBuffers();
        }

        public GraphicsContext context;

        public void Dispose()
        {
            context.Dispose();
        }
    }
}
