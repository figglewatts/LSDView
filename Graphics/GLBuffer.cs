using System;
using OpenTK.Graphics.OpenGL;

namespace LSDView.Graphics
{
    public class GLBuffer<T> : IBindable, IDisposable where T : struct
    {
        private readonly int _handle;
        private readonly T[] _elements;
        private readonly int _elementSize;
        private readonly BufferTarget _target;

        public T[] Elements => _elements;
        public int Length => _elements.Length;

        public GLBuffer(T[] data, int elementSize, BufferTarget target = BufferTarget.ArrayBuffer)
        {
            _elements = data;
            _elementSize = elementSize;
            _target = target;

            _handle = GL.GenBuffer();
            Bind();
            GL.BufferData(target, (IntPtr)(_elementSize * _elements.Length), _elements, BufferUsageHint.StaticDraw);
            Unbind();
        }

        public void Bind() { GL.BindBuffer(_target, _handle); }

        public void Unbind() { GL.BindBuffer(_target, 0); }

        public void Dispose() { GL.DeleteBuffer(_handle); }
    }
}
