using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace LSDView.graphics
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Vertex
    {
        public static readonly int Size = (3 + 3 + 2 + 4) * 4; // size in bytes
        public Vector3 Position;
        public Vector3 Normal;
        public Vector2 TexCoord;
        public Vector4 Color;

        public Vertex(Vector3 position, Vector3? normal = null,
            Vector2? texCoord = null, Vector4? color = null)
        {
            Position = position;
            Normal = normal ?? Vector3.Zero;
            TexCoord = texCoord ?? Vector2.Zero;
            Color = color ?? Vector4.One;
        }
    }
}
