using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;

namespace LSDView.graphics
{
    class Shader
    {
        private readonly int _vertHandle;
        private readonly int _fragHandle;
        private readonly int _progHandle;

        public string Name { get; }

        public Shader(string name, string path)
        {
            compileAndLink(path);
        }

        private void compileAndLink(string path)
        {

        }

        private int loadSource(string path, ShaderType type)
        {
            string completePath = path;
            if (type == ShaderType.VertexShader)
                path += ".vert";
            else if (type == ShaderType.FragmentShader)
                path += ".frag";
            else
                throw new ArgumentException("loadSource expects ShaderType to be Vertex or Fragment shader");

            string source = File.ReadAllText(path);
            int handle = GL.CreateShader(type);
            GL.ShaderSource(handle, source);
            return handle;
        }
    }
}
