using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Platform;
using OpenTK.Platform.Windows;
using System;
using System.Windows.Forms;

namespace GraphicsEngine
{
    public class OpenGLGraphics : Graphics
    {
        public OpenGLGraphics(Form form)
        {
            this.form = form;
            var windowInfo = Utilities.CreateWindowsWindowInfo(form.Handle);
            context = new GraphicsContext(GraphicsMode.Default, windowInfo);
            context.MakeCurrent(windowInfo);
            context.LoadAll();

            GL.Enable(EnableCap.DepthTest);
            GL.ShadeModel(ShadingModel.Smooth);
            GL.Enable(EnableCap.Lighting);
            GL.Light(LightName.Light0, LightParameter.Ambient, new[] {.2f, .2f, .2f, 1.0f});
            GL.Light(LightName.Light0, LightParameter.Diffuse, new[] {.8f, .8f, .8f, 1.0f});
            GL.Light(LightName.Light0, LightParameter.Position, new[] {4, 5, 5});
            GL.Enable(EnableCap.Light0);
        }

        Form form;

        public void Render()
        {
            GL.Viewport(0, 0, form.ClientSize.Width, form.ClientSize.Height);
            var color = Program.common.BackgroundColor;
            GL.ClearColor(color.R / 255.0f, color.G / 255.0f, color.B / 255.0f, 1);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            Matrix4 prespective = Matrix4.CreatePerspectiveFieldOfView((float)Math.PI / 2,
                (float)form.ClientSize.Width/ (float)form.ClientSize.Height,
                0.1f, 100.0f);
            GL.LoadMatrix(ref prespective);

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
            GL.Rotate(60, Vector3d.UnitY);
            GL.Translate(3, -1.5f, -2.5f);

            GL.EnableClientState(ArrayCap.NormalArray);
            GL.NormalPointer(NormalPointerType.Float, 0, Program.common.normals);
            GL.EnableClientState(ArrayCap.VertexArray);
            GL.VertexPointer(3, VertexPointerType.Float, 0, Program.common.vertices);
            GL.DrawElements(PrimitiveType.Triangles, Common.NumberOfIndices,
                DrawElementsType.UnsignedShort, Program.common.indices);
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
