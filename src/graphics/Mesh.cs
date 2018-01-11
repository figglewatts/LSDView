using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace LSDView.graphics
{
    public class Mesh : IDisposable
    {
        private readonly VertexArray _verts;
        private Shader _shader;

        public Shader Shader { get; set; }

        public Matrix4 Model { get; set; }

        public Mesh(Vertex[] vertices, int[] indices, Shader shader)
        {
            _verts = new VertexArray(vertices, indices);
            _shader = shader;
            Model = Matrix4.Identity;
        }

        public void Render(Matrix4 view, Matrix4 projection)
        {
            _verts.Bind();
            _shader.Bind();
            _shader.Uniform("Projection", false, projection);
            _shader.Uniform("View", false, view);
            _shader.Uniform("Model", false, Model);
            GL.DrawElements(PrimitiveType.Triangles, _verts.Length, DrawElementsType.UnsignedInt, IntPtr.Zero);
            _shader.Unbind();
            _verts.Unbind();
        }

        public void Dispose()
        {
            _verts.Dispose();
        }
    }
}
