using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace LSDView.math
{
    public class Transform
    {
        private Vector3 _position;
        public Vector3 Position
        {
            get => _position;
            set
            {
                _position = value;
                recomputeMatrix();
            }
        }

        private Vector3 _scale;
        public Vector3 Scale
        {
            get => _scale;
            set
            {
                _scale = value;
                recomputeMatrix();
            }
        }

        private Quaternion _rotation;
        public Quaternion Rotation
        {
            get => _rotation;
            set
            {
                _rotation = value;
                recomputeMatrix();
            }
        }

        public Matrix4 Matrix { get; private set; }

        public Vector3 Up => (Vector3.UnitY * new Matrix3(Matrix)).Normalized();
        public Vector3 Forward => (Vector3.UnitZ * new Matrix3(Matrix)).Normalized();
        public Vector3 Right => (Vector3.UnitX * new Matrix3(Matrix)).Normalized();

        public Matrix4 InverseMatrix => Matrix4.CreateTranslation(-Position)
                                        * Matrix4.CreateFromQuaternion(Rotation.Inverted())
                                        * Matrix4.CreateScale(Vector3.One);

        public Transform()
        {
            Position = Vector3.Zero;
            Scale = Vector3.One;
            Rotation = Quaternion.Identity;
            Matrix = Matrix4.Identity;
        }

        public Vector3 ToWorld(Vector3 pos)
        {
            Vector4 worldPos = new Vector4(pos, 1) * Matrix;
            return new Vector3(worldPos);
        }

        public Vector3 ToLocal(Vector3 pos)
        {
            Vector4 localPos = new Vector4(pos, 1) * InverseMatrix;
            return new Vector3(localPos);
        }

        public Transform Translate(Vector3 v)
        {
            Matrix = Matrix4.CreateTranslation(v) * Matrix;
            Position += v;
            return this;
        }

        public Transform Rescale(Vector3 s)
        {
            Scale *= s;
            Matrix = Matrix4.CreateScale(s) * Matrix;
            return this;
        }

        public Transform Rotate(Quaternion q, bool local)
        {
            if (local)
            {
                Matrix = Matrix * Matrix4.CreateFromQuaternion(q);
                Rotation *= q;
            }
            else
            {
                Matrix = Matrix4.CreateFromQuaternion(q) * Matrix;
                Rotation = q * Rotation;
            }

            return this;
        }

        public Transform Rotate(Vector3 euler, bool local)
        {
            Quaternion rot = Quaternion.FromEulerAngles(euler);
            return Rotate(rot, local);
        }

        public Transform Rotate(float angle, Vector3 axis, bool local)
        {
            Quaternion rot = Quaternion.FromAxisAngle(axis, angle);
            return Rotate(rot, local);
        }

        private void recomputeMatrix()
        {
	        Matrix = Matrix4.CreateFromQuaternion(Rotation) *
	                 Matrix4.CreateScale(Scale) * Matrix4.CreateTranslation(Position);

        }
    }
}
