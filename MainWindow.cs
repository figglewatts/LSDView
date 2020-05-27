using System;
using System.Collections.Generic;
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

        private const int WINDOW_WIDTH = 800;
        private const int WINDOW_HEIGHT = 600;
        private const string WINDOW_TITLE = "LSDView";
        private const int GL_MAJOR_VERSION = 4;
        private const int GL_MINOR_VERSION = 0;

        private readonly List<ImGuiComponent> _guiComponents;

        private readonly Camera _cam;
        private Matrix4 _proj;
        private readonly Framebuffer _fbo;

        // controllers
        private LBDController _lbdController;
        private TreeController _treeController;
        private VRAMController _vramController;
        private CameraController _cameraController;
        private ConfigController _configController;

        private FileOpenController _fileOpenController;
        // --------------

        public MainWindow() : base(WINDOW_WIDTH, WINDOW_HEIGHT, GraphicsMode.Default, WINDOW_TITLE,
            GameWindowFlags.Default, DisplayDevice.Default, GL_MAJOR_VERSION, GL_MINOR_VERSION,
            GraphicsContextFlags.Default)
        {
            Instance = this;

            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.CullFace);

            _cam = new Camera();
            _cam.Transform.Translate(new Vector3(0, 0, -3));
            _fbo = new Framebuffer(WINDOW_WIDTH, WINDOW_HEIGHT, FramebufferTarget.Framebuffer);

            ImGuiRenderer.Init();
            _guiComponents = new List<ImGuiComponent>();

            createControllers();

            ApplicationArea area = new ApplicationArea();

            TreeView<MeshTreeNode> outlineView = new TreeView<MeshTreeNode>();
            _treeController.SetTree(outlineView);

            area.AddChild(new Columns(2, new List<ImGuiComponent>
                {outlineView, new FramebufferArea(_fbo)}, new[] {250f, -1}));

            var menuBar = new MainMenuBar(_fileOpenController, _vramController, _configController);

            _guiComponents.Add(area);
            _guiComponents.Add(menuBar);
        }

        private void createControllers()
        {
            _configController = new ConfigController();
            _treeController = new TreeController();
            _vramController = new VRAMController();
            _cameraController = new CameraController(_cam);
            _lbdController = new LBDController(_treeController, _vramController);
            _fileOpenController = new FileOpenController(_lbdController, _configController);
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

            _cameraController.Update();
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            ImGuiRenderer.BeginFrame(e.Time);

            foreach (var component in _guiComponents)
            {
                component.Render();
            }

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

        protected override void OnKeyPress(KeyPressEventArgs e) { ImGuiRenderer.AddKeyChar(e.KeyChar); }

        protected override void OnMouseMove(MouseMoveEventArgs e) { ImGuiRenderer.UpdateMousePos(e.X, e.Y); }

        private void HandleInput() { }
    }
}
