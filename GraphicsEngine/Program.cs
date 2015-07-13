using System;
using System.Drawing;
using System.Windows.Forms;

namespace GraphicsEngine
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            GraphicsAPI = API.DirectX;
            common = new Common();
            Init();
            while (true)
            {
                Application.DoEvents();
                if (form.IsDisposed)
                    break;
                graphics.Render();
                graphics.Present();
            }
            graphics.Dispose();
        }

        public static API GraphicsAPI;
        public static Common common;

        static void Init()
        {
            form = new GraphicsEngineForm();
            form.Show();
            graphics = GraphicsAPI == API.OpenGL
                ? (Graphics)new OpenGLGraphics(form)
                : new DirectXGraphics(form);
        }

        static Form form;
        static Graphics graphics;
    }
}