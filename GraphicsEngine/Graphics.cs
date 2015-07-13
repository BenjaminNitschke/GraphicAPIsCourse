using System;

namespace GraphicsEngine
{
    public interface Graphics : IDisposable
    {
        void Render();
        void Present();
    }
}