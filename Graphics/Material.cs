using System;
using System.Collections.Generic;
using OpenTK;

namespace LSDView.Graphics
{
    public class Material : IBindable
    {
        protected struct MaterialParameter
        {
            public readonly MaterialParameterType ParamType;
            public readonly object Value;
            public readonly object DefaultValue;

            public MaterialParameter(MaterialParameterType type, object value, object defaultValue)
            {
                ParamType = type;
                Value = value;
                DefaultValue = defaultValue;
            }

            public void ApplyTo(Shader shader, string withUniformName)
            {
                switch (ParamType)
                {
                    case MaterialParameterType.Bool:
                        shader.Uniform(withUniformName, (bool)Value);
                        break;
                    case MaterialParameterType.Int:
                        shader.Uniform(withUniformName, (int)Value);
                        break;
                    case MaterialParameterType.Float:
                        shader.Uniform(withUniformName, (float)Value);
                        break;
                    case MaterialParameterType.Matrix3:
                        shader.Uniform(withUniformName, false, (Matrix3)Value);
                        break;
                    case MaterialParameterType.Matrix4:
                        shader.Uniform(withUniformName, false, (Matrix4)Value);
                        break;
                    case MaterialParameterType.Vector2:
                        shader.Uniform(withUniformName, (Vector2)Value);
                        break;
                    case MaterialParameterType.Vector3:
                        shader.Uniform(withUniformName, (Vector3)Value);
                        break;
                    case MaterialParameterType.Vector4:
                        shader.Uniform(withUniformName, (Vector4)Value);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(ParamType), ParamType,
                            "Unknown MaterialParameterType");
                }
            }

            public void UnapplyTo(Shader shader, string withUniformName)
            {
                switch (ParamType)
                {
                    case MaterialParameterType.Bool:
                        shader.Uniform(withUniformName, (bool)DefaultValue);
                        break;
                    case MaterialParameterType.Int:
                        shader.Uniform(withUniformName, (int)DefaultValue);
                        break;
                    case MaterialParameterType.Float:
                        shader.Uniform(withUniformName, (float)DefaultValue);
                        break;
                    case MaterialParameterType.Matrix3:
                        shader.Uniform(withUniformName, false, (Matrix3)DefaultValue);
                        break;
                    case MaterialParameterType.Matrix4:
                        shader.Uniform(withUniformName, false, (Matrix4)DefaultValue);
                        break;
                    case MaterialParameterType.Vector2:
                        shader.Uniform(withUniformName, (Vector2)DefaultValue);
                        break;
                    case MaterialParameterType.Vector3:
                        shader.Uniform(withUniformName, (Vector3)DefaultValue);
                        break;
                    case MaterialParameterType.Vector4:
                        shader.Uniform(withUniformName, (Vector4)DefaultValue);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(ParamType), ParamType,
                            "Unknown MaterialParameterType");
                }
            }
        }

        protected enum MaterialParameterType
        {
            Bool,
            Int,
            Float,
            Matrix4,
            Matrix3,
            Vector2,
            Vector3,
            Vector4
        }

        public readonly Shader Shader;
        protected readonly Dictionary<string, MaterialParameter> _parameters;

        public Material(Shader shader)
        {
            Shader = shader;
            _parameters = new Dictionary<string, MaterialParameter>();
        }

        public void SetParameter(string name, bool value, bool defaultValue = default)
        {
            _parameters[name] = new MaterialParameter(MaterialParameterType.Bool, value, defaultValue);
        }

        public void SetParameter(string name, int value, int defaultValue = default)
        {
            _parameters[name] = new MaterialParameter(MaterialParameterType.Int, value, defaultValue);
        }

        public void SetParameter(string name, float value, float defaultValue = default)
        {
            _parameters[name] = new MaterialParameter(MaterialParameterType.Float, value, defaultValue);
        }

        public void SetParameter(string name, Matrix4 value, Matrix4 defaultValue = default)
        {
            _parameters[name] = new MaterialParameter(MaterialParameterType.Matrix4, value, defaultValue);
        }

        public void SetParameter(string name, Matrix3 value, Matrix3 defaultValue = default)
        {
            _parameters[name] = new MaterialParameter(MaterialParameterType.Matrix3, value, defaultValue);
        }

        public void SetParameter(string name, Vector2 value, Vector2 defaultValue = default)
        {
            _parameters[name] = new MaterialParameter(MaterialParameterType.Vector2, value, defaultValue);
        }

        public void SetParameter(string name, Vector3 value, Vector3 defaultValue = default)
        {
            _parameters[name] = new MaterialParameter(MaterialParameterType.Vector3, value, defaultValue);
        }

        public void SetParameter(string name, Vector4 value, Vector4 defaultValue = default)
        {
            _parameters[name] = new MaterialParameter(MaterialParameterType.Vector4, value, defaultValue);
        }

        protected void apply()
        {
            foreach (var paramElt in _parameters)
            {
                var paramName = paramElt.Key;
                var param = paramElt.Value;
                param.ApplyTo(Shader, paramName);
            }
        }

        protected void unapply()
        {
            foreach (var paramElt in _parameters)
            {
                var paramName = paramElt.Key;
                var param = paramElt.Value;
                param.UnapplyTo(Shader, paramName);
            }
        }

        public void Bind()
        {
            Shader.Bind();
            apply();
        }

        public void Unbind()
        {
            unapply();
            Shader.Unbind();
        }
    }
}
