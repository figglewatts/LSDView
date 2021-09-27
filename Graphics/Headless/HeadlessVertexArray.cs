namespace LSDView.Graphics.Headless
{
    /// <summary>
    /// An implementation of IVertexArray that doesn't need an OpenGL context.
    /// </summary>
    public class HeadlessVertexArray : IVertexArray
    {
        public Vertex[] Vertices => _verts;
        public int[] Indices => _indices;
        public int Length => _indices.Length;
        public int Tris => Length / 3;

        protected readonly Vertex[] _verts;
        protected readonly int[] _indices;

        public HeadlessVertexArray(Vertex[] vertices, int[] indices)
        {
            _verts = vertices;
            _indices = indices;
        }
    }
}
