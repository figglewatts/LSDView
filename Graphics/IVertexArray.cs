namespace LSDView.Graphics
{
    public interface IVertexArray
    {
        Vertex[] Vertices { get; }
        int[] Indices { get; }
        int Length { get; }
        int Tris { get; }
    }
}
