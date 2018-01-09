using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using libLSD.Types;
using OpenTK.Graphics.OpenGL;

namespace LSDView.graphics
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct Vertex
    {
        public static readonly int Size = 3 * 4; // size in bytes
        public readonly Vec3 Position;

        public Vertex(Vec3 position) { Position = position; }
    }
}
