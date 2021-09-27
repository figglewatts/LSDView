using System.Collections.Generic;
using LSDView.Math;
using OpenTK;

namespace LSDView.Graphics
{
    public interface IRenderable : IDisposable
    {
        Transform Transform { get; }
        List<ITexture2D> Textures { get; }
        Material Material { get; }
        IVertexArray Verts { get; }


        void Render(Matrix4 viewMatrix, Matrix4 projectionMatrix);
    }
}
