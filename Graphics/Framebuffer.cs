using System;
using OpenTK.Graphics.OpenGL4;

namespace LSDView.Graphics
{
    public class Framebuffer : IDisposable, IBindable
    {
        public int Width { get; private set; }
        public int Height { get; private set; }
        public FramebufferTarget Target { get; private set; }

        public Texture2D Texture => _colorAttachment;

        private readonly Texture2D _colorAttachment;
        private readonly Texture2D _depthAttachment;
        private readonly int _handle;

        public Framebuffer(int width,
            int height,
            FramebufferTarget target)
        {
            Width = width;
            Height = height;
            Target = target;

            _handle = GL.GenFramebuffer();
            _colorAttachment = new Texture2D(Width, Height);
            _depthAttachment = new Texture2D(Width, Height, null, PixelInternalFormat.DepthComponent32,
                PixelFormat.DepthComponent, PixelType.UnsignedInt);

            Bind();

            GL.FramebufferTexture2D(target, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D,
                _colorAttachment.Handle, 0);
            GL.FramebufferTexture2D(target, FramebufferAttachment.DepthAttachment, TextureTarget.Texture2D,
                _depthAttachment.Handle, 0);

            Unbind();
        }

        public void Resize(int newWidth, int newHeight)
        {
            _colorAttachment.Bind();
            GL.TexImage2D(TextureTarget.Texture2D, 0, _colorAttachment.InternalFormat, newWidth, newHeight, 0,
                _colorAttachment.Format, _colorAttachment.PixelType, IntPtr.Zero);
            _colorAttachment.Unbind();

            _depthAttachment.Bind();
            GL.TexImage2D(TextureTarget.Texture2D, 0, _depthAttachment.InternalFormat, newWidth, newHeight, 0,
                _depthAttachment.Format, _depthAttachment.PixelType, IntPtr.Zero);
            _depthAttachment.Unbind();

            Width = newWidth;
            Height = newHeight;
        }

        public void Dispose()
        {
            _colorAttachment.Dispose();
            _depthAttachment.Dispose();
            GL.DeleteFramebuffer(_handle);
        }

        public void Bind() { GL.BindFramebuffer(Target, _handle); }
        public void Unbind() { GL.BindFramebuffer(Target, 0); }
    }
}
