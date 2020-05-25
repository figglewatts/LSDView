using System;
using System.Collections.Generic;
using IconFonts;
using ImGuiNET;
using JsonAnything.GUI.GUIComponents;
using LSDView.GUI;
using LSDView.GUI.GUIComponents;
using LSDView.Util;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.ES30;
using OpenTK.Input;
using Vector2 = System.Numerics.Vector2;

namespace LSDView
{
    public class MainWindow : GameWindow
    {
        private const int WINDOW_WIDTH = 800;
        private const int WINDOW_HEIGHT = 600;
        private const string WINDOW_TITLE = "LSDView";
        private const int GL_MAJOR_VERSION = 4;
        private const int GL_MINOR_VERSION = 0;

        private List<ImGuiComponent> _guiComponents;

        public MainWindow() : base(WINDOW_WIDTH, WINDOW_HEIGHT, GraphicsMode.Default, WINDOW_TITLE,
            GameWindowFlags.Default, DisplayDevice.Default, GL_MAJOR_VERSION, GL_MINOR_VERSION,
            GraphicsContextFlags.Default)
        {
            ImGuiRenderer.Init();
            _guiComponents = new List<ImGuiComponent>();

            ApplicationArea area = new ApplicationArea(this);
            _guiComponents.Add(new MainMenuBar(this));
            _guiComponents.Add(area);
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

        protected override void OnUpdateFrame(FrameEventArgs e) { HandleInput(); }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            ImGuiRenderer.BeginFrame(e.Time);

            foreach (var component in _guiComponents)
            {
                component.Render();
            }

            ImGuiRenderer.EndFrame();

            SwapBuffers();
        }

        protected override void OnKeyPress(KeyPressEventArgs e) { ImGuiRenderer.AddKeyChar(e.KeyChar); }

        protected override void OnMouseMove(MouseMoveEventArgs e) { ImGuiRenderer.UpdateMousePos(e.X, e.Y); }

        private void HandleInput() { }
    }
}
