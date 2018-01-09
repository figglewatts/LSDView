using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;

namespace LSDView.graphics
{
    class VertexBuffer
    {
        private readonly int _handle;
        private readonly Vertex[] _verts;

        public VertexBuffer(Vertex[] vertArray)
        {
            _verts = vertArray;

            _handle = GL.GenBuffer();
            Bind();
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(Vertex.Size * _verts.Length), _verts, BufferUsageHint.StaticDraw);
            Unbind();
        }

        public void Bind()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, _handle);
        }

        public void Unbind()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        public void Draw()
        {
            GL.EnableClientState(ArrayCap.VertexArray);
            GL.VertexPointer(3, VertexPointerType.Float, Vertex.Size, 0);
            GL.DrawArrays(PrimitiveType.Triangles, 0, _verts.Length);
        }
    }
}
