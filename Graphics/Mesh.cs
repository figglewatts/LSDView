using System;
using System.Collections.Generic;
using LSDView.Math;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace LSDView.Graphics
{
    public class Mesh : IDisposable, IRenderable
    {
        private readonly VertexArray _verts;

        public VertexArray Verts => _verts;

        public Shader Shader { get; set; }
        public Transform Transform { get; set; }
        public List<Texture2D> Textures { get; set; }

        public Mesh(Vertex[] vertices, int[] indices, Shader shader)
        {
            _verts = new VertexArray(vertices, indices);
            Shader = shader;
            Transform = new Transform();
            Textures = new List<Texture2D>();
        }

        public void Render(Matrix4 view, Matrix4 projection)
        {
            _verts.Bind();
            BindTextures();
            Shader.Bind();
            Shader.Uniform("Projection", false, projection);
            Shader.Uniform("View", false, view);
            Shader.Uniform("Model", false, Transform.Matrix);
            GL.DrawElements(PrimitiveType.Triangles, _verts.Length, DrawElementsType.UnsignedInt, IntPtr.Zero);
            Shader.Unbind();
            UnbindTextures();
            _verts.Unbind();
        }

        public void Dispose() { _verts.Dispose(); }

        public void ClearTextures()
        {
            foreach (Texture2D tex in Textures)
            {
                tex.Dispose();
            }

            Textures.Clear();
        }

        private void BindTextures()
        {
            for (int i = 0; i < Textures.Count; i++)
            {
                GL.ActiveTexture(TextureUnit.Texture0 + i);
                Textures[i].Bind();
            }

            GL.ActiveTexture(TextureUnit.Texture0);
        }

        private void UnbindTextures()
        {
            for (int i = 0; i < Textures.Count; i++)
            {
                GL.ActiveTexture(TextureUnit.Texture0 + i);
                Textures[i].Unbind();
            }

            GL.ActiveTexture(TextureUnit.Texture0);
        }
    }
}
