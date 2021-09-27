using System.Collections.Generic;
using System.Linq;
using LSDView.Math;
using OpenTK;

namespace LSDView.Graphics.Headless
{
    /// <summary>
    /// An implementation of IRenderable that doesn't need an OpenGL context.
    /// </summary>
    public class HeadlessMesh : IRenderable
    {
        public Transform Transform { get; }
        public List<ITexture2D> Textures { get; }
        public Material Material { get; }
        public IVertexArray Verts { get; }

        public HeadlessMesh(Vertex[] vertices, int[] indices)
        {
            Verts = new HeadlessVertexArray(vertices, indices);
            Transform = new Transform();
            Textures = new List<ITexture2D>();
            Material = null;
        }

        public static HeadlessMesh CreateQuad()
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

            return new HeadlessMesh(
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
                new[] { 1, 0, 2, 2, 0, 3 }
            );
        }

        public void Render(Matrix4 viewMatrix, Matrix4 projectionMatrix)
        {
            // intentionally empty
        }

        public void Dispose()
        {
            // intentionally empty
        }

        public static HeadlessMesh CombineMeshes(params IRenderable[] renderables)
        {
            var totalVertexCount = renderables.Sum(r => r.Verts.Vertices.Length);
            var totalIndexCount = renderables.Sum(r => r.Verts.Length);

            var verts = new Vertex[totalVertexCount];
            var indices = new int[totalIndexCount];

            int vertsRunningCount = 0;
            int indicesRunningCount = 0;
            foreach (var renderable in renderables)
            {
                renderable.Verts.CopyVertices(verts, vertsRunningCount, renderable.Transform);
                renderable.Verts.CopyIndices(indices, vertsRunningCount, indicesRunningCount);

                vertsRunningCount += renderable.Verts.Vertices.Length;
                indicesRunningCount += renderable.Verts.Length;
            }

            return new HeadlessMesh(verts, indices);
        }
    }
}
