namespace LSDView.Graphics.Headless
{
    public class HeadlessTexture2D : ITexture2D
    {
        public int Width { get; }
        public int Height { get; }

        protected readonly float[] _data;

        public HeadlessTexture2D(int width, int height, float[] data)
        {
            Width = width;
            Height = height;
            _data = data;
        }

        public void Dispose()
        {
            // intentionally empty
        }

        public void Bind()
        {
            // intentionally empty
        }

        public void Unbind()
        {
            // intentionally empty
        }

        public void SubImage(float[] data, int x, int y, int width, int height)
        {
            // this basically just emulates glTexSubImage2D()
            int componentsWritten = 0;
            int tStart = Height - y - 1 - (height - 1);
            for (int t = tStart; t < tStart + height; t++)
            {
                for (int s = x; s < x + width; s++)
                {
                    int idx = t * Width * 4 + s * 4;
                    _data[idx] = data[componentsWritten];
                    _data[idx + 1] = data[componentsWritten + 1];
                    _data[idx + 2] = data[componentsWritten + 2];
                    _data[idx + 3] = data[componentsWritten + 3];
                    componentsWritten += 4;
                }
            }
        }

        public float[] GetData() => _data;

        public void Clear()
        {
            for (int i = 0; i < Width * Height; i++)
            {
                _data[i] = 1;
            }
        }
    }
}
