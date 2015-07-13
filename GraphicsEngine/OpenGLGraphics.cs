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
            // Create 3D Cube from Vertices and Indices
            vertices = new float[NumberOfVertices * 3] // 6*4 = 24 edge points
            {
                1, 1, 1,  -1, 1, 1,  -1,-1, 1,   1,-1, 1,   // v0,v1,v2,v3 (front)
                1, 1, 1,   1,-1, 1,   1,-1,-1,   1, 1,-1,   // v0,v3,v4,v5 (right)
                1, 1, 1,   1, 1,-1,  -1, 1,-1,  -1, 1, 1,   // v0,v5,v6,v1 (top)
                -1, 1, 1,  -1, 1,-1,  -1,-1,-1,  -1,-1, 1,  // v1,v6,v7,v2 (left)
                -1,-1,-1,   1,-1,-1,   1,-1, 1,  -1,-1, 1,  // v7,v4,v3,v2 (bottom)
                1,-1,-1,  -1,-1,-1,  -1, 1,-1,   1, 1,-1    // v4,v7,v6,v5 (back)
            };
            normals = new float[NumberOfVertices * 3]
            {
                0, 0, 1,   0, 0, 1,   0, 0, 1,   0, 0, 1,   // v0,v1,v2,v3 (front)
                1, 0, 0,   1, 0, 0,   1, 0, 0,   1, 0, 0,   // v0,v3,v4,v5 (right)
                0, 1, 0,   0, 1, 0,   0, 1, 0,   0, 1, 0,   // v0,v5,v6,v1 (top)
                -1, 0, 0,  -1, 0, 0,  -1, 0, 0,  -1, 0, 0,  // v1,v6,v7,v2 (left)
                0,-1, 0,   0,-1, 0,   0,-1, 0,   0,-1, 0,   // v7,v4,v3,v2 (bottom)
                0, 0,-1,   0, 0,-1,   0, 0,-1,   0, 0,-1    // v4,v7,v6,v5 (back)
            };
            indices = new short[NumberOfIndices]
            {
                // Front face
                0, 1, 2,   2, 3, 0,
                // Right face
                4, 5, 6,   6, 7, 4,
                // Top face
                8, 9,10,  10,11, 8,
                // Left face
                12,13,14,  14,15,12,
                // Bottom face
                16,17,18,  18,19,16,
                // Back face
                20,21,22,  22,23,20
            };

            GL.Enable(EnableCap.DepthTest);
            GL.ShadeModel(ShadingModel.Smooth);
            GL.Enable(EnableCap.Lighting);
            GL.Light(LightName.Light0, LightParameter.Ambient, new[] {.2f, .2f, .2f, 1.0f});
            GL.Light(LightName.Light0, LightParameter.Diffuse, new[] {.8f, .8f, .8f, 1.0f});
            GL.Light(LightName.Light0, LightParameter.Position, new[] {4, 5, 5});
            GL.Enable(EnableCap.Light0);
        }

        Form form;
        float[] vertices;
        float[] normals;
        const float Left = -1, Top = -1, Back = -1;
        const float Right = 1, Bottom = 1, Front = 1;
        const int NumberOfFaces = 6;
        const int NumberOfVertices = NumberOfFaces * 4;
        short[] indices;
        const int NumberOfIndices = CubeFaces * 2 * 3;

        public void Render()
        {
            GL.Viewport(0, 0, form.ClientSize.Width, form.ClientSize.Height);
            GL.ClearColor(0, 0, 0, 1);
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
            GL.NormalPointer(NormalPointerType.Float, 0, normals);
            GL.EnableClientState(ArrayCap.VertexArray);
            GL.VertexPointer(3, VertexPointerType.Float, 0, vertices);
            GL.DrawElements(PrimitiveType.Triangles, NumberOfIndices,
                DrawElementsType.UnsignedShort, indices);
        }

        public const int CubeFaces = 6;

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
