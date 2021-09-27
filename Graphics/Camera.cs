using LSDView.Math;
using OpenTK;

namespace LSDView.Graphics
{
    public class Camera
    {
        public Transform Transform { get; private set; }

        public Matrix4 View => Matrix4.LookAt(Transform.Position, Transform.Position + Transform.Forward, Transform.Up);

        public Camera() { Transform = new Transform(); }

        public void LookAt(Vector3 pos)
        {
            Vector3 forward = Transform.Forward;
            Vector3 newForward = Vector3.Normalize(pos - Transform.Position);
            Vector3 rotAxis = Vector3.Cross(forward, newForward);
            float angle = Vector3.CalculateAngle(forward, newForward);

            // rotate to face the pos
            Transform.Rotate(angle, rotAxis, false);
        }

        /// <summary>
        /// Rotate around a target at a distance.
        /// </summary>
        /// <param name="longitude">The angle to rotate around the target from east to west</param>
        /// <param name="latitude">The angle to rotate around the target from north to south</param>
        /// <param name="target">The target</param>
        /// <param name="distance">The distance the camera should be away from the target</param>
        public void ArcBall(float longitude, float latitude, Vector3 target, float distance)
        {
            Transform.Position = Quaternion.FromAxisAngle(Transform.Right, latitude) *
                ((Transform.Position - target).Normalized() * distance) + target;
            Transform.Position = Quaternion.FromAxisAngle(Transform.Up, longitude) *
                ((Transform.Position - target).Normalized() * distance) + target;
            LookAt(target);
        }
    }
}
