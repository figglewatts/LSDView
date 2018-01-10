using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using LSDView.util;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace LSDView.graphics
{
    public class Shader : IBindable, IDisposable
    {
        private int _vertHandle;
        private int _fragHandle;
        private int _progHandle;

        public string Name { get; }

        public Shader(string name, string path)
        {
            this.Name = name;
            compileAndLink(path);
        }

        private void compileAndLink(string path)
        {
            // load and compile vertex shader
            _vertHandle = loadSource(path, ShaderType.VertexShader);
            GL.CompileShader(_vertHandle);
            checkCompileErr(_vertHandle, ShaderType.VertexShader);

            // load and compile fragment shader
            _fragHandle = loadSource(path, ShaderType.FragmentShader);
            GL.CompileShader(_fragHandle);
            checkCompileErr(_fragHandle, ShaderType.FragmentShader);

            // link the shader program
            _progHandle = GL.CreateProgram();
            GL.AttachShader(_progHandle, _vertHandle);
            GL.AttachShader(_progHandle, _fragHandle);
            GL.LinkProgram(_progHandle);
            checkLinkErr(_progHandle);

            // clean up unused handles
            GL.DeleteShader(_vertHandle);
            GL.DeleteShader(_fragHandle);
            _vertHandle = _fragHandle = 0;
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

        private bool checkCompileErr(int shader, ShaderType type)
        {
            GL.GetShader(shader, ShaderParameter.CompileStatus, out int success);
            if (success != 1)
            {
                string infoLog = GL.GetShaderInfoLog(shader);
                Logger.Log()(LogLevel.ERR, "Could not compile {0}: {1}", type.ToString(), this.Name);
                Logger.Log()(LogLevel.ERR, "Info log: {0}", infoLog);
            }

            return success == 1;
        }

        private bool checkLinkErr(int program)
        {
            GL.GetProgram(program, GetProgramParameterName.LinkStatus, out int success);
            if (success != 1)
            {
                string infoLog = GL.GetProgramInfoLog(program);
                Logger.Log()(LogLevel.ERR, "Could not link program: {0}", this.Name);
                Logger.Log()(LogLevel.ERR, "Info log: {0}", infoLog);
            }

            return success == 1;
        }

        private int getUniformLocation(string name) { return GL.GetUniformLocation(_progHandle, name); }

        public void Uniform(string name, bool value) { GL.Uniform1(getUniformLocation(name), value ? 1 : 0); }

        public void Uniform(string name, int value) { GL.Uniform1(getUniformLocation(name), value); }

        public void Uniform(string name, float value) { GL.Uniform1(getUniformLocation(name), value); }

        public void Uniform(string name, bool transpose, Matrix4 value)
        {
            GL.UniformMatrix4(getUniformLocation(name), transpose, ref value);
        }

        public void Uniform(string name, bool transpose, Matrix3 value)
        {
            GL.UniformMatrix3(getUniformLocation(name), transpose, ref value);
        }

        public void Uniform(string name, Vector2 value) { GL.Uniform2(getUniformLocation(name), value); }

        public void Uniform(string name, Vector3 value) { GL.Uniform3(getUniformLocation(name), value); }

        public void Uniform(string name, Vector4 value) { GL.Uniform4(getUniformLocation(name), value); }

        public int GetAttribLocation(string name) { return GL.GetAttribLocation(_progHandle, name); }

        public void Bind()
        {
            GL.UseProgram(_progHandle);
        }

        public void Unbind()
        {
            GL.UseProgram(0);
        }

        public void Dispose()
        {
            GL.DeleteProgram(_progHandle);
        }
    }
}
