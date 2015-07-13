using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Platform;
using OpenTK.Platform.Windows;
using System;
using System.Windows.Forms;

namespace GraphicsEngine
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            GraphicsAPI = API.OpenGL;
            Init();
            while (true)
            {
                Application.DoEvents();
                if (form.IsDisposed)
                    break;
                graphics.Render();
                graphics.Present();
            }
        }

        public static API GraphicsAPI;

        static void Init()
        {
            form = new GraphicsEngineForm();
            form.Show();
            graphics = GraphicsAPI == API.OpenGL
                ? (Graphics)new OpenGLGraphics(form.Handle)
                : new DirectXGraphics(form.Handle);
        }

        static Form form;
        static Graphics graphics;
    }
}