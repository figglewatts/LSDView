using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Threading;
using libLSD.Exceptions;
using libLSD.Formats;
using libLSD.Types;
using LSDView.controller;
using LSDView.graphics;
using LSDView.util;
using OpenTK.Graphics.OpenGL;
using OpenTK;
using OpenTK.Input;
using MouseEventArgs = System.Windows.Forms.MouseEventArgs;

namespace LSDView.view
{
    public partial class LSDViewForm : Form, ILSDView
    {
        private Matrix4 projection;

        private Camera _sceneCamera;
        private float _viewDistance = 5f;
        private const float MAX_VIEW_DISTANCE = 200f;
        private const float MIN_VIEW_DISTANCE = 5f;

        private Vector2 _lastMousePos = Vector2.Zero;
        private Vector2 _mouseDragAmount = Vector2.Zero;
        private const float MOUSE_DRAG_SCALING = 0.01f;
        private const float SCROLL_SENSITIVITY = 0.01f;
	    private const float CAMERA_MOVE_SPEED = 0.005f;
	    private const float CAMERA_ROTATION_SPEED = 0.001f;
	    private bool _cameraFlyMode = false;
	    private Vector3 _cachedCamPos;
	    private Quaternion _cachedCamOrientation;

        public bool MouseDown { get; set; }

        public GLControl ViewingWindow => this._viewingWindow;
        public TreeView ViewOutline => this._viewOutline;
        public event EventHandler OnGLLoad;

        public TMDController TmdController { get; set; }
        public TIMController TimController { get; set; }
        public TIXController TixController { get; set; }
        public VRAMController VramController { get; set; }
		public LBDController LbdController { get; set; }

        public LSDViewForm()
        {
            InitializeComponent();
            _sceneCamera = new Camera();
            _sceneCamera.Transform.Translate(new Vector3(0, 10, -10));
            _sceneCamera.LookAt(Vector3.Zero);
            this.MouseWheel += _viewingWindow_MouseWheel;

            ViewOutline.AfterSelect += (sender, args) =>
            {
                this.Text = ViewOutline.SelectedNode.Text + " - LSDView";
            };
        }

        public Mesh CreateTextureQuad()
        {
            Vector3[] vertPositions = {
                new Vector3(-1, -1, 0),
                new Vector3(-1, 1, 0),
                new Vector3(1, 1, 0),
                new Vector3(1, -1, 0)
            };

            Vector2[] vertUVs = {
                new Vector2(0, 0),
                new Vector2(0, 1),
                new Vector2(1, 1),
                new Vector2(1, 0)
            };

            return new Mesh(
                new[]
                {
                    new Vertex(
                        vertPositions[0], null, vertUVs[0]),
                    new Vertex(
                        vertPositions[1], null, vertUVs[1]),
                    new Vertex(
                        vertPositions[2], null, vertUVs[2]),
                    new Vertex(
                        vertPositions[3], null, vertUVs[3])
                },
                new[] { 1, 0, 2, 2, 0, 3 },
                new Shader("texture", "shaders/texture")
            );
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            ViewingWindow.Resize += new EventHandler(glControl_Resize);
            ViewingWindow.Paint += new PaintEventHandler(glControl_Paint);

            GL.ClearColor(Color.Black);
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.CullFace);

            Application.Idle += Application_Idle;

            // Ensure that the viewport and projection matrix are set correctly.
            glControl_Resize(ViewingWindow, EventArgs.Empty);

            OnGLLoad?.Invoke(this, null);

            GL.ClearColor(Color.DimGray);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            Application.Idle -= Application_Idle;

            base.OnClosing(e);
        }

        void Application_Idle(object sender, EventArgs e)
        {
            while (ViewingWindow.IsIdle)
            {
	            CheckInput();

				Render();
            }
        }

        #region OpenGL functions

        void glControl_Resize(object sender, EventArgs e)
        {
            OpenTK.GLControl c = sender as OpenTK.GLControl;

            if (c.ClientSize.Height == 0)
                c.ClientSize = new System.Drawing.Size(c.ClientSize.Width, 1);

            GL.Viewport(0, 0, c.Size.Width, c.Size.Height);

            float aspect_ratio = c.Size.Width / (float)c.Size.Height;
            projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspect_ratio, 0.1f, 1000);
        }

        void glControl_Paint(object sender, PaintEventArgs e)
        {
            Render();
        }

        private void Render()
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            if (_viewOutline.SelectedNode is IRenderableTreeNode renderableTreeNode)
            {
                foreach (var renderable in renderableTreeNode.Renderables)
                {
                    renderable?.Render(_sceneCamera.View, projection);
                }
            }

            ViewingWindow.SwapBuffers();
        }

		#endregion

		private void CheckInput()
	    {
		    if (!_cameraFlyMode) return;

			KeyboardState keyState = Keyboard.GetState();

		    if (keyState.IsKeyDown(Key.Left))
		    {
			    _sceneCamera.Transform.Rotate(-CAMERA_ROTATION_SPEED, Vector3.UnitY, false);
		    }
			else if (keyState.IsKeyDown(Key.Right))
		    {
				_sceneCamera.Transform.Rotate(CAMERA_ROTATION_SPEED, Vector3.UnitY, false);
			}

		    if (keyState.IsKeyDown(Key.Up))
		    {
				_sceneCamera.Transform.Rotate(-CAMERA_ROTATION_SPEED, _sceneCamera.Transform.Right, false);
			}
			else if (keyState.IsKeyDown(Key.Down))
		    {
			    _sceneCamera.Transform.Rotate(CAMERA_ROTATION_SPEED, _sceneCamera.Transform.Right, false);
		    }

		    if (keyState.IsKeyDown(Key.W))
		    {
			    _sceneCamera.Transform.Translate(_sceneCamera.Transform.Forward * CAMERA_MOVE_SPEED);
		    }
		    else if (keyState.IsKeyDown(Key.S))
		    {
			    _sceneCamera.Transform.Translate(_sceneCamera.Transform.Forward * -CAMERA_MOVE_SPEED);
		    }
		    if (keyState.IsKeyDown(Key.A))
		    {
			    _sceneCamera.Transform.Translate(_sceneCamera.Transform.Right * CAMERA_MOVE_SPEED);
		    }
		    else if (keyState.IsKeyDown(Key.D))
		    {
			    _sceneCamera.Transform.Translate(_sceneCamera.Transform.Right * -CAMERA_MOVE_SPEED);
		    }
		}
		
		private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
	        openFileDialog.Filter = "All files|*.*|TMD files|*.tmd|TIM files|*.tim|TIX files|*.tix|LBD files|*.lbd";
			if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)  
            {
                try
                {
                    string ext = Path.GetExtension(openFileDialog.FileName).ToLowerInvariant();
                    switch (ext)
                    {
                        case ".tmd":
                            TmdController.LoadTMD(openFileDialog.FileName);
                            break;
                        case ".tim":
                            TimController.LoadTIM(openFileDialog.FileName);
                            break;
                        case ".tix":
                            TixController.LoadTIX(openFileDialog.FileName);
                            break;
						case ".lbd":
							LbdController.LoadLBD(openFileDialog.FileName);
							break;
                    }
                    
                }
                catch (BadFormatException ex)
                {
                    MessageBox.Show(ex.Message, "Could not load file", MessageBoxButtons.OK, MessageBoxIcon.Error,
                        MessageBoxDefaultButton.Button1);
                }
            }  
        }

        private void _viewingWindow_MouseDown(object sender, MouseEventArgs e)
        {
            MouseDown = true;
            _mouseDragAmount = Vector2.Zero;
            _lastMousePos = new Vector2(e.X, e.Y);
        }

        private void _viewingWindow_MouseMove(object sender, MouseEventArgs e)
        {
			if (_cameraFlyMode) return;

			if (MouseDown)
            {
                Vector2 newMousePos = new Vector2(e.X, e.Y);
                _mouseDragAmount = newMousePos - _lastMousePos;
                _mouseDragAmount *= MOUSE_DRAG_SCALING;
                _lastMousePos = newMousePos;

                _sceneCamera.ArcBall(_mouseDragAmount.X, -_mouseDragAmount.Y, Vector3.Zero, _viewDistance);
            }
        }

        private void _viewingWindow_MouseUp(object sender, MouseEventArgs e) { MouseDown = false; }

        private void _viewingWindow_MouseWheel(object sender, MouseEventArgs e)
        {
	        if (_cameraFlyMode) return;

			_viewDistance -= (e.Delta * SCROLL_SENSITIVITY);
            _viewDistance = MathHelper.Clamp(_viewDistance, MIN_VIEW_DISTANCE, MAX_VIEW_DISTANCE);
            _sceneCamera.ArcBall(0, 0, Vector3.Zero, _viewDistance);
        }

        private void importTIXToolStripMenuItem_Click(object sender, EventArgs e)
        {
	        openFileDialog.Filter = "TIX files|*.TIX";
			if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    VramController.LoadTIXIntoVRAM(openFileDialog.FileName);

                }
                catch (BadFormatException ex)
                {
                    MessageBox.Show(ex.Message, "Could not load file", MessageBoxButtons.OK, MessageBoxIcon.Error,
                        MessageBoxDefaultButton.Button1);
                }
            }
        }

		private void _viewingWindow_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Z)
			{
				setCameraFly(!_cameraFlyMode);
			}
		}

	    private void setCameraFly(bool state)
	    {
		    _cameraFlyMode = state;
			if (state)
			{
				_cachedCamPos = _sceneCamera.Transform.Position;
				_cachedCamOrientation = _sceneCamera.Transform.Rotation;
				_sceneCamera.Transform.Rotation = Quaternion.Identity;
			}
		    else
			{
				_sceneCamera.Transform.Rotation = _cachedCamOrientation;
				_sceneCamera.Transform.Position = _cachedCamPos;
			}
	    }
	}
}