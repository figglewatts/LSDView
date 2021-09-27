namespace LSDView.Graphics
{
    public interface ITexture2D : IDisposable, IBindable
    {
        int Width { get; }
        int Height { get; }

        void SubImage(float[] data, int x, int y, int width, int height);
        float[] GetData();
        void Clear();
    }
}
