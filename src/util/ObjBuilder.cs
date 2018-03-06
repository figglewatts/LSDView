using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace LSDView.util
{
    public class ObjBuilder
    {
        private StringBuilder _builder;

        public ObjBuilder()
        {
            _builder = new StringBuilder();
        }

        public ObjBuilder Comment(string comment)
        {
            _builder.AppendLine($"# {comment}");
            return this;
        }

        public ObjBuilder Vertex(Vector3 pos)
        {
            _builder.AppendLine($"v {pos.X} {pos.Y} {pos.Z}");
            return this;
        }

        public ObjBuilder Normal(Vector3 normal)
        {
            _builder.AppendLine($"vn {normal.X} {normal.Y} {normal.Z}");
            return this;
        }

        public ObjBuilder UV(Vector2 uv)
        {
            _builder.AppendLine($"vt {uv.X} {uv.Y}");
            return this;
        }

        public ObjBuilder Group(string name)
        {
            _builder.AppendLine($"g {name}");
            return this;
        }

        public ObjBuilder Face(ObjFaceBuilder.ObjFace face)
        {
            _builder.Append("f ");
            for (int i = 0; i < face.Verts.Length; i++)
            {
                var f = face[i];
                _builder.Append($"{f.v}");
                if (f.t != -1)
                {
                    _builder.Append($"/{f.t}");
                }
                if (f.n != -1)
                {
                    _builder.Append($"/{f.n}");
                }
                _builder.Append(" ");
            }
            _builder.AppendLine();
            return this;
        }

        public override string ToString()
        {
            return _builder.ToString();
        }
    }

    public class ObjFaceBuilder
    {
        public List<int> Vertices { get; }
        public List<int> Normals { get; }
        public List<int> UVs { get; }

        public ObjFaceBuilder()
        {
            Vertices = new List<int>();
            Normals = new List<int>();
            UVs = new List<int>();
        }

        public ObjFaceBuilder Vertex(int v, int? n = null, int? t = null)
        {
            Vertices.Add(v);
            Normals.Add(n ?? -1);
            UVs.Add(t ?? -1);
            return this;
        }

        public ObjFaceBuilder Clear()
        {
            Vertices.Clear();
            Normals.Clear();
            UVs.Clear();
            return this;
        }

        public ObjFace Build()
        {
            return new ObjFace(this);
        }

        public class ObjFace
        {
            public int[] Verts { get; }
            public int[] Normals { get; }
            public int[] UVs { get; }

            public ObjFace(ObjFaceBuilder builder)
            {
                Verts = builder.Vertices.ToArray();
                Normals = builder.Normals.ToArray();
                UVs = builder.UVs.ToArray();
            }

            public (int v, int n, int t) this[int i] => (Verts[i], Normals[i], UVs[i]);
        }
    }
}
