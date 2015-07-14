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
            GL.Light(LightName.Light0, LightParameter.Diffuse, new[] {1, 1, 1, 1.0f});
            GL.Light(LightName.Light0, LightParameter.Position, new[] {Common.LightX, Common.LightY, Common.LightZ});
            GL.Enable(EnableCap.Light0);

            GL.MatrixMode(MatrixMode.Projection);
            Matrix4 prespective = Matrix4.CreatePerspectiveFieldOfView(Common.FieldOfView,
                (float)form.ClientSize.Width / (float)form.ClientSize.Height,
                Common.NearPlane, Common.FarPlane);
            GL.LoadMatrix(ref prespective);

            GL.MatrixMode(MatrixMode.Modelview);
            Matrix4 view = Matrix4.CreateTranslation(Common.CameraX, Common.CameraY, Common.CameraZ) *
                Matrix4.CreateRotationY(Common.CameraRotationY);
            GL.LoadMatrix(ref view);
        }

        Form form;

        public void Render()
        {
            GL.Viewport(0, 0, form.ClientSize.Width, form.ClientSize.Height);
            var color = Program.common.BackgroundColor;
            GL.ClearColor(color.R / 255.0f, color.G / 255.0f, color.B / 255.0f, 1);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            
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
