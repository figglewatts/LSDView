using LSDView.Math;

namespace LSDView.Graphics
{
    public static class IVertexArrayExtensions
    {
        public static void CopyVertices(this IVertexArray vertexArray,
            Vertex[] vertices,
            int startIndex = 0,
            Transform transform = null)
        {
            for (int i = 0; i < vertexArray.Vertices.Length; i++)
            {
                vertices[startIndex + i] = vertexArray.Vertices[i];

                // if a transform is given, transform the copied vertices with it
                if (transform != null) vertices[startIndex + i].Transform(transform);
            }
        }

        public static void CopyIndices(this IVertexArray vertexArray, int[] indices, int indexBase, int startIndex = 0)
        {
            for (int i = 0; i < vertexArray.Length; i++)
            {
                indices[startIndex + i] = vertexArray.Indices[i] + indexBase;
            }
        }
    }
}
