using System.Collections.Generic;
using LSDView.Math;
using OpenTK;

namespace LSDView.Graphics
{
    public interface IRenderable
    {
        Transform Transform { get; set; }
        List<Texture2D> Textures { get; set; }
        Shader Shader { get; set; }
        VertexArray Verts { get; }

        void Render(Matrix4 viewMatrix, Matrix4 projectionMatrix);
    }
}
