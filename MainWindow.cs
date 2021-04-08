using System;
using System.Collections.Generic;
using System.Drawing;
using IconFonts;
using ImGuiNET;
using JsonAnything.GUI.GUIComponents;
using LSDView.controller;
using LSDView.Controllers;
using LSDView.Graphics;
using LSDView.GUI;
using LSDView.GUI.Components;
using LSDView.GUI.GUIComponents;
using LSDView.Util;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.ES30;
using OpenTK.Input;
using FramebufferTarget = OpenTK.Graphics.OpenGL4.FramebufferTarget;
using Vector2 = System.Numerics.Vector2;

namespace LSDView
{
    public class MainWindow : GameWindow
    {
        public static MainWindow Instance = null;

        public Action NextUpdateActions;
        public Action NextGuiRender;

        public Matrix4 ProjectionMatrix => _proj;
        public Camera Camera => _cam;
        public Vector2 Dimensions => new Vector2(Width, Height);

        private const int WINDOW_WIDTH = 800;
        private const int WINDOW_HEIGHT = 600;
        private const string WINDOW_TITLE = "LSDView";
        private const int GL_MAJOR_VERSION = 4;
        private const int GL_MINOR_VERSION = 0;

        private readonly List<ImGuiComponent> _guiComponents;

        private readonly Camera _cam;
        private Matrix4 _proj;
        private readonly Framebuffer _fbo;

        // ui
        private FileDialog _exportFileDialog;
        private Modal _updateAvailableModal;

        // --------------

        // controllers
        private TreeController _treeController;
        private VRAMController _vramController;
        private CameraController _cameraController;
        private ConfigController _configController;
        private LBDController _lbdController;
        private TMDController _tmdController;
        private MOMController _momController;
        private TIMController _timController;
        private TIXController _tixController;
        private FileOpenController _fileOpenController;
        private AnimationController _animationController;
        private ExportController _exportController;
        private UpdateCheckerController _updateCheckerController;

        // --------------

        public MainWindow() : base(WINDOW_WIDTH, WINDOW_HEIGHT, GraphicsMode.Default, WINDOW_TITLE,
            GameWindowFlags.Default, DisplayDevice.Default, GL_MAJOR_VERSION, GL_MINOR_VERSION,
            GraphicsContextFlags.Default)
        {
            Instance = this;

            Icon = new Icon(typeof(MainWindow), "appicon.ico");

            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.CullFace);

            _cam = new Camera();
            _cam.Transform.Translate(new Vector3(0, 0, -3));
            _fbo = new Framebuffer(WINDOW_WIDTH, WINDOW_HEIGHT, FramebufferTarget.Framebuffer);

            _proj = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(60f),
                (float)_fbo.Width / _fbo.Height, 0.1f, 100f);

            ImGuiRenderer.Init();
            _guiComponents = new List<ImGuiComponent>();

            _updateAvailableModal = new Modal("New update available!",
                "Download at https://github.com/Figglewatts/LSDView/releases/latest");

            createControllers();

            ApplicationArea area = new ApplicationArea();

            TreeView<TreeNode> outlineView = new TreeView<TreeNode>();
            _treeController.SetTree(outlineView);

            area.AddChild(new Columns(2, new List<ImGuiComponent>
                {outlineView, new FramebufferArea(_fbo)}, new[] {250f, -1}));

            var menuBar = new MainMenuBar(_fileOpenController, _vramController, _configController, _cameraController);

            if (string.IsNullOrWhiteSpace(_configController.Config.GameDataPath))
            {
                // show the set game data path dialog if it hasn't yet been set
                menuBar.OpenSetGameDataPath();
            }

            _guiComponents.Add(area);
            _guiComponents.Add(menuBar);
            _guiComponents.Add(_exportFileDialog);
            _guiComponents.Add(_updateAvailableModal);
        }

        private void createControllers()
        {
            _configController = new ConfigController();
            _exportFileDialog =
                new FileDialog(_configController.Config.GameDataPath, FileDialog.DialogType.Save);
            _exportController = new ExportController(_exportFileDialog, _configController);
            _animationController = new AnimationController();
            _treeController = new TreeController(_animationController, _exportController);
            _vramController = new VRAMController(_exportController);
            _cameraController = new CameraController(_cam);
            _tmdController = new TMDController(_treeController, _vramController);
            _momController = new MOMController(_treeController, _vramController, _tmdController);
            _lbdController = new LBDController(_treeController, _vramController, _tmdController, _momController);
            _timController = new TIMController(_treeController);
            _tixController = new TIXController(_treeController, _timController);
            _fileOpenController =
                new FileOpenController(_lbdController, _tmdController, _momController, _timController, _tixController,
                    _configController);
            _updateCheckerController = new UpdateCheckerController(_updateAvailableModal);
        }

        protected override void OnResize(EventArgs e)
        {
            GL.Viewport(0, 0, Width, Height);
            ImGuiRenderer.Resize(Width, Height);
        }

        protected override void OnLoad(EventArgs e)
        {
            CursorVisible = true;
            GL.ClearColor(0.2f, 0.2f, 0.2f, 1);

            Logger.Log()(LogLevel.INFO, "Initialized LSDView");

            var io = ImGui.GetIO();

            ImFontConfig cfg = new ImFontConfig
            {
                MergeMode = 1,
                PixelSnapH = 1,
                GlyphOffset = new Vector2(1, 1),
                GlyphMinAdvanceX = 20,
            };
            ImGuiRenderer.AddFontFromFileTTF("Fonts/fa-solid-900.ttf", 16, cfg,
                new[] {(char)FontAwesome5.IconMin, (char)FontAwesome5.IconMax, (char)0});
        }

        protected override void OnUnload(EventArgs e) { ImGuiRenderer.Shutdown(); }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            HandleInput();

            NextUpdateActions?.Invoke();
            NextUpdateActions = null;

            _cameraController.Update();
            _animationController.Update(e.Time);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            ImGuiRenderer.BeginFrame(e.Time);

            foreach (var component in _guiComponents)
            {
                component.Render();
            }

            NextGuiRender?.Invoke();
            NextGuiRender = null;

            ImGuiRenderer.EndFrame();

            _fbo.Bind();
            GL.Viewport(0, 0, _fbo.Width, _fbo.Height);
            _proj = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(60f),
                (float)_fbo.Width / _fbo.Height, 0.1f, 100f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            _treeController.RenderSelectedNode(_cam.View, _proj);

            _fbo.Unbind();

            SwapBuffers();
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            if (!Focused || !Visible) return;
            ImGuiRenderer.AddKeyChar(e.KeyChar);
        }

        protected override void OnMouseMove(MouseMoveEventArgs e)
        {
            if (!Focused || !Visible) return;
            ImGuiRenderer.UpdateMousePos(e.X, e.Y);
        }

        private void HandleInput() { }
    }
}
