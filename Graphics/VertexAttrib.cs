using OpenTK.Graphics.OpenGL4;

namespace LSDView.Graphics
{
    public class VertexAttrib
    {
        private readonly int _index;
        private readonly int _size;
        private readonly VertexAttribPointerType _type;
        private readonly bool _normalize;
        private readonly int _stride;
        private readonly int _offset;

        public VertexAttrib(int index,
            int size,
            VertexAttribPointerType type,
            int stride,
            int offset,
            bool normalize = false)
        {
            _index = index;
            _size = size;
            _type = type;
            _stride = stride;
            _offset = offset;
            _normalize = normalize;
        }

        public void Set()
        {
            GL.EnableVertexAttribArray(_index);
            GL.VertexAttribPointer(_index, _size, _type, _normalize, _stride, _offset);
        }
    }
}
