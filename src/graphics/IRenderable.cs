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
        Shader Shader { get; set; }

        void Render(Matrix4 viewMatrix, Matrix4 projectionMatrix);
    }
}
