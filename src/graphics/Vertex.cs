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
        public readonly Vector3 Position;
        public readonly Vector3 Normal;
        public readonly Vector2 TexCoord;
        public readonly Vector4 Color;

        public Vertex(Vector3 position, Vector3 normal,
            Vector2 texCoord, Vector4 color)
        {
            Position = position;
            Normal = normal;
            TexCoord = texCoord;
            Color = color;
        }
    }
}
