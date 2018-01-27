using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;

namespace LSDView.graphics
{
    public class Texture2D : IDisposable, IBindable
    {
        public int Width { get; }
        public int Height { get; }

        private readonly int _handle;

        public Texture2D(float[] data, int width, int height)
        {
            _handle = GL.GenTexture();
            Bind();
            Width = width;
            Height = height;
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba32f, width, height, 0, PixelFormat.Rgba, PixelType.Float, data);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
            Unbind();
        }

        public void Dispose()
        {
            GL.DeleteTexture(_handle);
        }

        public void Bind()
        {
            GL.BindTexture(TextureTarget.Texture2D, _handle);
        }

        public void Unbind()
        {
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }
    }
}
