using System;
using System.Drawing;
using System.Text;

namespace GraphicsEngine
{
    class Common
    {
        public Common()
        {
            BackgroundColor = Color.Black;
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
                0, 2, 1,   2, 0, 3,
                // Right face
                4, 6, 5,   6, 4, 7,
                // Top face
                8, 10,9,   10,8, 11,
                // Left face
                12,14,13,  14,12,15,
                // Bottom face
                16,18,17,  18,16,19,
                // Back face
                20,22,21,  22,20,23
            };
        }

        public Color BackgroundColor;
        public float[] vertices;
        public float[] normals;
        public const float Left = -1, Top = -1, Back = -1;
        public const float Right = 1, Bottom = 1, Front = 1;
        public const int NumberOfFaces = 6;
        public const int NumberOfVertices = NumberOfFaces * 4;
        public const int CubeFaces = 6;
        public const int NumberOfIndices = CubeFaces * 2 * 3;
        public short[] indices;
        public const float PI = (float)Math.PI;
        public const float PI2 = (float)Math.PI * 2;
        public const float PIHalf = (float)Math.PI / 2;
        public const float PIThird = (float)Math.PI / 3;
        public const float FieldOfView = PI / 2;
        public const float NearPlane = 0.1f;
        public const float FarPlane = 100.0f;
        public const float CameraX = 3;
        public const float CameraY = -1.5f;
        public const float CameraZ = -2.5f;
        public const float CameraRotationY = PIThird;
    }
}
