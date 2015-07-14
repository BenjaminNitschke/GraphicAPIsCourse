using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Platform;
using OpenTK.Platform.Windows;
using System;
using System.Windows.Forms;

namespace GraphicsEngine
{
    public class OpenGL4Graphics : Graphics
    {
        public OpenGL4Graphics(Form form)
        {
            this.form = form;
            CreateContext(form);
            CreateVertexBuffer();
            CreateShader();
            CreateIndexBuffer();
        }

        private void CreateContext(Form form)
        {
            var windowInfo = Utilities.CreateWindowsWindowInfo(form.Handle);
            context = new GraphicsContext(GraphicsMode.Default, windowInfo);
            context.MakeCurrent(windowInfo);
            context.LoadAll();
            GL.Enable(EnableCap.DepthTest);
        }

        private void CreateVertexBuffer()
        {
            GL.GenBuffers(1, out vertices);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertices);
            const int StrideInBytes = 6 * 4;
            float[] vertexData = new float[Program.common.vertices.Length + Program.common.normals.Length];
            for (int num = 0; num < Common.NumberOfVertices; num++)
            {
                for (int i = 0; i < 3; i++)
                    vertexData[num * 6 + i] = Program.common.vertices[num * 3 + i];
                for (int i = 0; i < 3; i++)
                    vertexData[num * 6 + 3 + i] = Program.common.normals[num * 3 + i];
            }
            GL.BufferData(BufferTarget.ArrayBuffer, new IntPtr(Common.NumberOfVertices * StrideInBytes),
                vertexData, BufferUsageHint.StaticDraw);
        }

        private void CreateShader()
        {
            shaderProgram = GL.CreateProgram();
            vertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShader, @"
#version 150
uniform mat4 WorldViewProjection;
uniform vec3 LightDirection;
in vec3 position;
in vec3 normal;
out vec4 color;
void main()
{
    gl_Position = WorldViewProjection * vec4(position, 1);
    float Ambient = 0.2;
    float Diffuse = 0.8;
    float lightIntensity = clamp(dot(LightDirection, normal), 0, 1);
    float light = Ambient + Diffuse * lightIntensity;
    color = vec4(light, light, light, 1.0);
}");
            fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShader, @"
#version 150
precision highp float;
in vec4 color;
out vec4 fragColor;

void main()
{
    fragColor = color;
}");

            GL.CompileShader(vertexShader);
            int compileStatus;
            GL.GetShader(vertexShader, ShaderParameter.CompileStatus, out compileStatus);
            if (compileStatus == 0)
                throw new Exception("OpenGL4 error compiling vertex shader: " + GL.GetShaderInfoLog(vertexShader));
            GL.CompileShader(fragmentShader);
            GL.GetShader(fragmentShader, ShaderParameter.CompileStatus, out compileStatus);
            if (compileStatus == 0)
                throw new Exception("OpenGL4 error compiling fragment shader");

            GL.AttachShader(shaderProgram, vertexShader);
            GL.AttachShader(shaderProgram, fragmentShader);

            GL.LinkProgram(shaderProgram);

            // vertex format
            GL.UseProgram(shaderProgram);
            var positionLocation = GL.GetAttribLocation(shaderProgram, "position");
            var normalLocation = GL.GetAttribLocation(shaderProgram, "normal");
            const int StrideInBytes = 6 * 4;
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, StrideInBytes, 0);
            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, StrideInBytes, 3 * 4);

            Matrix4 prespective = Matrix4.CreatePerspectiveFieldOfView(Common.FieldOfView,
                (float)form.ClientSize.Width / (float)form.ClientSize.Height,
                Common.NearPlane, Common.FarPlane);
            Matrix4 view = Matrix4.CreateTranslation(Common.CameraX, Common.CameraY, Common.CameraZ) *
                Matrix4.CreateRotationY(Common.CameraRotationY);
            WorldViewProjection = view * prespective;
            LightDirection = new Vector3(Common.LightX, Common.LightY, Common.LightZ);
            LightDirection.Normalize();
            // Set uniforms
            GL.UseProgram(shaderProgram);
            int worldViewProjectionLocation =
                GL.GetUniformLocation(shaderProgram, "WorldViewProjection");
            if (worldViewProjectionLocation <= 0)
                throw new Exception("Failed to find WorldViewProjection in shader");
            GL.UniformMatrix4(worldViewProjectionLocation, false, ref WorldViewProjection);
            int lightDirectionLocation = GL.GetUniformLocation(shaderProgram, "LightDirection");
            GL.Uniform3(lightDirectionLocation, ref LightDirection);

        }

        private void CreateIndexBuffer()
        {
            GL.GenBuffers(1, out indices);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, indices);
            GL.BufferData(BufferTarget.ElementArrayBuffer, new IntPtr(Common.NumberOfIndices * 3),
                Program.common.indices, BufferUsageHint.StaticDraw);
        }

        Form form;
        int vertices;
        int indices;
        int shaderProgram;
        int vertexShader;
        int fragmentShader;
        Matrix4 WorldViewProjection;
        Vector3 LightDirection;

        public void Render()
        {
            GL.Viewport(0, 0, form.ClientSize.Width, form.ClientSize.Height);
            var color = Program.common.BackgroundColor;
            GL.ClearColor(color.R / 255.0f, color.G / 255.0f, color.B / 255.0f, 1);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.UseProgram(shaderProgram);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertices);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, indices);
            GL.DrawElements(PrimitiveType.Triangles, Common.NumberOfIndices,
                DrawElementsType.UnsignedShort, new IntPtr(0));
            /*use VertexBuffer
            GL.EnableClientState(ArrayCap.NormalArray);
            GL.NormalPointer(NormalPointerType.Float, 0, Program.common.normals);
            GL.EnableClientState(ArrayCap.VertexArray);
            GL.VertexPointer(3, VertexPointerType.Float, 0, Program.common.vertices);
            GL.DrawElements(PrimitiveType.Triangles, Common.NumberOfIndices,
                DrawElementsType.UnsignedShort, Program.common.indices);
             */
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