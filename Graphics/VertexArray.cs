using OpenTK.Graphics.OpenGL;

namespace LSDView.Graphics
{
    public class VertexArray : IBindable, IDisposable
    {
        private readonly int _handle;
        private readonly GLBuffer<Vertex> _vertexBuffer;
        private readonly GLBuffer<int> _indexBuffer;

        private readonly VertexAttrib[] _attribs = new VertexAttrib[]
        {
            new VertexAttrib(0, 3, VertexAttribPointerType.Float, Vertex.Size, 0),              // in_Position
            new VertexAttrib(1, 3, VertexAttribPointerType.Float, Vertex.Size, 3 * 4),          // in_Normal
            new VertexAttrib(2, 2, VertexAttribPointerType.Float, Vertex.Size, (3 + 3) * 4),    // in_TexCoord
            new VertexAttrib(3, 4, VertexAttribPointerType.Float, Vertex.Size, (3 + 3 + 2) * 4) // in_Color
        };

        public Vertex[] Vertices => _vertexBuffer.Elements;
        public int[] Indices => _indexBuffer.Elements;
        public int Length => _indexBuffer.Length;
        public int Tris => Length / 3;

        public VertexArray(Vertex[] vertices, int[] indices)
        {
            _handle = GL.GenVertexArray();
            Bind();

            _vertexBuffer = new GLBuffer<Vertex>(vertices, Vertex.Size, BufferTarget.ArrayBuffer);
            _indexBuffer = new GLBuffer<int>(indices, sizeof(int), BufferTarget.ElementArrayBuffer);

            _vertexBuffer.Bind();
            _indexBuffer.Bind();
            foreach (var attrib in _attribs)
            {
                attrib.Set();
            }
        }

        public void Bind() { GL.BindVertexArray(_handle); }

        public void Unbind() { GL.BindVertexArray(0); }

        public void Dispose()
        {
            _vertexBuffer.Dispose();
            _indexBuffer.Dispose();
            GL.DeleteVertexArray(_handle);
        }
    }
}
