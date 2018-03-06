using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LSDView.math;
using OpenTK;

namespace LSDView.graphics
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
