using System;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;

namespace LSDView.Graphics
{
    public class Texture2D : IDisposable, IBindable
    {
        public int Width { get; }
        public int Height { get; }

        public PixelInternalFormat InternalFormat { get; }
        public PixelFormat Format { get; }
        public PixelType PixelType { get; }

        public int Handle => _handle;

        private readonly int _handle;

        public Texture2D(
            int width,
            int height,
            float[] data = null,
            PixelInternalFormat internalFormat = PixelInternalFormat.Rgba32f,
            PixelFormat colorFormat = PixelFormat.Rgba,
            PixelType dataType = PixelType.Float)
        {
            _handle = GL.GenTexture();
            Bind();
            Width = width;
            Height = height;
            InternalFormat = internalFormat;
            Format = colorFormat;
            PixelType = dataType;

            if (data != null)
            {
                GL.TexImage2D(TextureTarget.Texture2D, 0, internalFormat, width, height, 0, colorFormat,
                    dataType, data);
            }
            else
            {
                GL.TexImage2D(TextureTarget.Texture2D, 0, internalFormat, Width, Height, 0, colorFormat, dataType,
                    IntPtr.Zero);
            }

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS,
                (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT,
                (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter,
                (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter,
                (int)TextureMagFilter.Nearest);
            Unbind();
        }

        public static Texture2D Fill(Color4 color, int width, int height)
        {
            float[] colorData = new float[width * height * 4];
            for (int i = 0; i < width * height * 4; i += 4)
            {
                colorData[i] = color.R;
                colorData[i + 1] = color.G;
                colorData[i + 2] = color.B;
                colorData[i + 3] = color.A;
            }

            return new Texture2D(width, height, colorData);
        }

        public void Dispose() { GL.DeleteTexture(_handle); }

        public void Bind() { GL.BindTexture(TextureTarget.Texture2D, _handle); }

        public void Unbind() { GL.BindTexture(TextureTarget.Texture2D, 0); }

        public void SubImage(float[] data, int x, int y, int width, int height)
        {
            Bind();
            GL.TexSubImage2D(TextureTarget.Texture2D, 0, x, y, width, height, PixelFormat.Rgba, PixelType.Float, data);
            Unbind();
        }

        public void Clear()
        {
            GL.ClearTexImage(_handle, 0, PixelFormat.Rgba, PixelType.Float, new float[] {1, 1, 1, 1});
        }
    }
}
