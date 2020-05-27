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
            var mouseState = Mouse.GetCursorState();

            if (mouseState.IsButtonDown(MouseButton.Left)) arcBallRotation(mouseState);
            if (mouseState.IsButtonDown(MouseButton.Middle)) middleMousePanning(mouseState);
            scrollZooming(mouseState);

            _lastMouseState = mouseState;
            _lastScroll = mouseState.Scroll.Y;
        }

        private Vector2 DragDelta(MouseState state)
        {
            return new Vector2(state.X, state.Y) - new Vector2(_lastMouseState.X, _lastMouseState.Y);
        }

        private float ScrollDelta(MouseState state) { return state.Scroll.Y - _lastScroll; }

        private void arcBallRotation(MouseState state)
        {
            var dragDelta = DragDelta(state);
            _cam.ArcBall(MathHelper.DegreesToRadians(dragDelta.X), MathHelper.DegreesToRadians(-dragDelta.Y),
                _arcBallTarget, _arcBallDistance);
        }

        private void middleMousePanning(MouseState state)
        {
            var dragDelta = DragDelta(state);
            var translationVec = _cam.Transform.Right * dragDelta.X + _cam.Transform.Up * dragDelta.Y;
            translationVec *= PAN_SPEED;
            _cam.Transform.Translate(translationVec);
            _arcBallTarget += translationVec;
        }

        private void scrollZooming(MouseState state)
        {
            _arcBallDistance -= ScrollDelta(state);
            _arcBallDistance = MathHelper.Clamp(_arcBallDistance, MIN_ARCBALL_DISTANCE, MAX_ARCBALL_DISTANCE);
            _cam.ArcBall(0, 0, _arcBallTarget, _arcBallDistance);
        }
    }
}
