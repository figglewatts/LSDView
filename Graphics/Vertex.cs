using System.Runtime.InteropServices;
using LSDView.Math;
using OpenTK;

namespace LSDView.Graphics
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Vertex
    {
        public static readonly int Size = (3 + 3 + 2 + 4) * 4; // size in bytes
        public Vector3 Position;
        public Vector3 Normal;
        public Vector2 TexCoord;
        public Vector4 Color;

        public Vertex(Vector3 position,
            Vector3? normal = null,
            Vector2? texCoord = null,
            Vector4? color = null)
        {
            Position = position;
            Normal = normal ?? Vector3.Zero;
            TexCoord = texCoord ?? Vector2.Zero;
            Color = color ?? Vector4.One;
        }

        /// <summary>
        /// Manually applies a transformation to this vertex's position.
        /// This should only be used once, as multiple times will keep reapplying the transform.
        /// </summary>
        /// <param name="transform">The Transform to apply to this vertex.</param>
        public void Transform(Transform transform) { Position = (new Vector4(Position, 1) * transform.Matrix).Xyz; }
    }
}
