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
            common = new Common();
            form = new GraphicsEngineForm();
            form.Text += " - " + GraphicsAPI;
            form.Show();
            graphics = GraphicsAPI == API.OpenGL
                ? (Graphics)new OpenGLGraphics(form)
                : new DirectXGraphics(form);
        }

        static Form form;
        static Graphics graphics;

        public static void ToggleAPIAndRestart()
        {
            GraphicsAPI = GraphicsAPI == API.OpenGL ? API.DirectX : API.OpenGL;
            form.Close();
            Init();
        }
    }
}