using System;
using System.Windows.Forms;

namespace GraphicsEngine
{
    //- 3D Cube
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
            graphics.Dispose();
        }

        public static API GraphicsAPI;

        static void Init()
        {
            form = new GraphicsEngineForm();
            form.Show();
            graphics = GraphicsAPI == API.OpenGL
                ? (Graphics)new OpenGLGraphics(form.Handle)
                : new DirectXGraphics(form);
        }

        static Form form;
        static Graphics graphics;
    }
}