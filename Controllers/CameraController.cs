using LSDView.Graphics;
using OpenTK;
using OpenTK.Input;

namespace LSDView.Controllers
{
    public class CameraController
    {
        private Camera _cam;
        private MouseState _lastMouseState;
        private float _lastScroll;
        private bool _dragging;

        private Vector3 _arcBallTarget = Vector3.Zero;
        private float _arcBallDistance = 10f;

        private const float MIN_ARCBALL_DISTANCE = 1f;
        private const float MAX_ARCBALL_DISTANCE = 80f;
        private const float PAN_SPEED = 0.05f;

        public CameraController(Camera cam) { _cam = cam; }

        public void Update()
        {
            if (!MainWindow.Instance.Focused) return;

            var mouseState = Mouse.GetCursorState();

            if (mouseState.IsButtonDown(MouseButton.Left)) arcBallRotation(mouseState);
            if (mouseState.IsButtonDown(MouseButton.Right)) panning(mouseState);
            scrollZooming(mouseState);

            _lastMouseState = mouseState;
            _lastScroll = mouseState.Scroll.Y;
        }

        public void RecenterView()
        {
            _arcBallTarget = Vector3.Zero;
            _cam.ArcBall(0, 0, _arcBallTarget, _arcBallDistance);
        }

        private Vector2 dragDelta(MouseState state)
        {
            return new Vector2(state.X, state.Y) - new Vector2(_lastMouseState.X, _lastMouseState.Y);
        }

        private float scrollDelta(MouseState state) { return state.Scroll.Y - _lastScroll; }

        private void arcBallRotation(MouseState state)
        {
            var delta = dragDelta(state);
            _cam.ArcBall(MathHelper.DegreesToRadians(-delta.X), MathHelper.DegreesToRadians(delta.Y),
                _arcBallTarget, _arcBallDistance);
        }

        private void panning(MouseState state)
        {
            var delta = dragDelta(state);
            var translationVec = _cam.Transform.Right * delta.X + _cam.Transform.Up * delta.Y;
            translationVec *= PAN_SPEED;
            _cam.Transform.Translate(translationVec);
            _arcBallTarget += translationVec;
        }

        private void scrollZooming(MouseState state)
        {
            _arcBallDistance -= scrollDelta(state);
            _arcBallDistance = MathHelper.Clamp(_arcBallDistance, MIN_ARCBALL_DISTANCE, MAX_ARCBALL_DISTANCE);
            _cam.ArcBall(0, 0, _arcBallTarget, _arcBallDistance);
        }
    }
}
