using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LSDView.math;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace LSDView.graphics
{
    public class Mesh : IDisposable, IRenderable
    {
        private readonly VertexArray _verts;

        public Shader Shader { get; set; }
        public Transform Transform { get; set; }

        public Mesh(Vertex[] vertices, int[] indices, Shader shader)
        {
            _verts = new VertexArray(vertices, indices);
            Shader = shader;
            Transform = new Transform();
        }

        public void Render(Matrix4 modelView, Matrix4 projection)
        {
            _verts.Bind();
            Shader.Bind();
            Shader.Uniform("Projection", false, projection);
            Shader.Uniform("ModelView", false, modelView);
            GL.DrawElements(PrimitiveType.Triangles, _verts.Length, DrawElementsType.UnsignedInt, IntPtr.Zero);
            Shader.Unbind();
            _verts.Unbind();
        }

        public void Dispose()
        {
            _verts.Dispose();
        }
    }
}
