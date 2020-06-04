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

        public Material Material { get; set; }
        public Transform Transform { get; set; }
        public List<Texture2D> Textures { get; set; }

        public Mesh(Vertex[] vertices, int[] indices, Shader shader)
        {
            _verts = new VertexArray(vertices, indices);
            Material = new Material(shader);
            Transform = new Transform();
            Textures = new List<Texture2D>();
        }

        public static Mesh CreateQuad(Shader shader)
        {
            Vector3[] vertPositions =
            {
                new Vector3(-1, -1, 0),
                new Vector3(-1, 1, 0),
                new Vector3(1, 1, 0),
                new Vector3(1, -1, 0)
            };

            Vector2[] vertUVs =
            {
                new Vector2(0, 0),
                new Vector2(0, 1),
                new Vector2(1, 1),
                new Vector2(1, 0)
            };

            return new Mesh(
                new[]
                {
                    new Vertex(
                        vertPositions[0], null, vertUVs[0]),
                    new Vertex(
                        vertPositions[1], null, vertUVs[1]),
                    new Vertex(
                        vertPositions[2], null, vertUVs[2]),
                    new Vertex(
                        vertPositions[3], null, vertUVs[3])
                },
                new[] {1, 0, 2, 2, 0, 3},
                shader
            );
        }

        public void Render(Matrix4 view, Matrix4 projection)
        {
            _verts.Bind();
            BindTextures();
            Material.Bind();
            Material.Shader.Uniform("Projection", false, projection);
            Material.Shader.Uniform("View", false, view);
            Material.Shader.Uniform("Model", false, Transform.Matrix);
            GL.DrawElements(PrimitiveType.Triangles, _verts.Length, DrawElementsType.UnsignedInt, IntPtr.Zero);
            Material.Unbind();
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
