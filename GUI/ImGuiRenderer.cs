using System;
using System.Runtime.InteropServices;
using IconFonts;
using ImGuiNET;
using OpenTK;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;
using Serilog;

namespace LSDView.GUI
{
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct ImGuiVert
    {
        public Vector2 pos;
        public Vector2 uv;
        public uint col;

        public const int PosOffset = 0;
        public const int UVOffset = 8;
        public const int ColOffset = 16;
        public static readonly int Size = sizeof(ImGuiVert);
    }

    public static class ImGuiRenderer
    {
        // variables
        private static float _lastScrollValue = 0;
        private static System.Numerics.Vector2 _mousePos = System.Numerics.Vector2.Zero;

        // device objects
        private static int _vertexShaderHandle, _fragmentShaderHandle, _programHandle;
        private static int _attribLocationTex;
        private static int _attribLocationProjMtx;
        private static int _attribLocationPosition;
        private static int _attribLocationUV;
        private static int _attribLocationColor;
        private static int _vboHandle;
        private static int _elementsHandle;
        private static int _fontsTextureHandle;

        public static void Init()
        {
            IntPtr ctx = ImGui.CreateContext();
            ImGui.SetCurrentContext(ctx);

            ImGuiIOPtr io = ImGui.GetIO();
            io.Fonts.AddFontDefault();

            io.KeyMap[(int)ImGuiKey.Tab] = (int)Key.Tab;
            io.KeyMap[(int)ImGuiKey.LeftArrow] = (int)Key.Left;
            io.KeyMap[(int)ImGuiKey.RightArrow] = (int)Key.Right;
            io.KeyMap[(int)ImGuiKey.UpArrow] = (int)Key.Up;
            io.KeyMap[(int)ImGuiKey.DownArrow] = (int)Key.Down;
            io.KeyMap[(int)ImGuiKey.PageUp] = (int)Key.PageUp;
            io.KeyMap[(int)ImGuiKey.PageDown] = (int)Key.PageDown;
            io.KeyMap[(int)ImGuiKey.Home] = (int)Key.Home;
            io.KeyMap[(int)ImGuiKey.End] = (int)Key.End;
            io.KeyMap[(int)ImGuiKey.Delete] = (int)Key.Delete;
            io.KeyMap[(int)ImGuiKey.Backspace] = (int)Key.BackSpace;
            io.KeyMap[(int)ImGuiKey.Enter] = (int)Key.Enter;
            io.KeyMap[(int)ImGuiKey.Escape] = (int)Key.Escape;
            io.KeyMap[(int)ImGuiKey.A] = (int)Key.A;
            io.KeyMap[(int)ImGuiKey.C] = (int)Key.C;
            io.KeyMap[(int)ImGuiKey.V] = (int)Key.V;
            io.KeyMap[(int)ImGuiKey.X] = (int)Key.X;
            io.KeyMap[(int)ImGuiKey.Y] = (int)Key.Y;
            io.KeyMap[(int)ImGuiKey.Z] = (int)Key.Z;

            io.DisplayFramebufferScale = System.Numerics.Vector2.One;

            AddFontFromFileTTF("Fonts/fa-solid-900.ttf", 16,
                new[] { (char)FontAwesome5.IconMin, (char)FontAwesome5.IconMax, (char)0 });
        }

        public static void BeginFrame(double deltaTime)
        {
            ImGuiIOPtr io = ImGui.GetIO();

            io.DeltaTime = (float)deltaTime;

            if (_fontsTextureHandle <= 0)
            {
                createDeviceObjects();
            }

            updateInput();

            ImGui.NewFrame();
        }

        public static void EndFrame()
        {
            ImGui.Render();
            unsafe
            {
                renderDrawData(ImGui.GetDrawData());
            }
        }

        public static void Shutdown() { destroyDeviceObjects(); }

        public static void AddKeyChar(char keyChar)
        {
            ImGuiIOPtr io = ImGui.GetIO();
            io.AddInputCharacter(keyChar);
        }

        public static void UpdateMousePos(int x, int y)
        {
            _mousePos.X = x;
            _mousePos.Y = y;
        }

        public static void Resize(int w, int h) { ImGui.GetIO().DisplaySize = new System.Numerics.Vector2(w, h); }

        public static void AddFontFromFileTTF(string filename,
            float sizePixels,
            char[] glyphRanges)
        {
            unsafe
            {
                ImFontConfigPtr nativeConfig = ImGuiNative.ImFontConfig_ImFontConfig();
                nativeConfig.MergeMode = true;
                nativeConfig.PixelSnapH = true;

                GCHandle rangeHandle = GCHandle.Alloc(new ushort[]
                {
                    glyphRanges[0], glyphRanges[1], 0
                }, GCHandleType.Pinned);
                try
                {
                    ImGui.GetIO().Fonts.AddFontFromFileTTF(filename, sizePixels, nativeConfig,
                        rangeHandle.AddrOfPinnedObject());
                    ImGui.GetIO().Fonts.Build();
                }
                finally
                {
                    ImGuiNative.ImFontConfig_destroy(nativeConfig);
                    if (rangeHandle.IsAllocated) rangeHandle.Free();
                }
            }
        }

        private static void updateInput()
        {
            ImGuiIOPtr io = ImGui.GetIO();

            if (!GuiApplication.Instance.Visible || !GuiApplication.Instance.Focused) return;

            MouseState mouse = Mouse.GetCursorState();
            KeyboardState keyboard = Keyboard.GetState();

            for (int i = 0; i < (int)Key.LastKey; i++)
            {
                io.KeysDown[i] = keyboard[(Key)i];
            }

            io.KeyShift = keyboard[Key.LShift] || keyboard[Key.RShift];
            io.KeyCtrl = keyboard[Key.LControl] || keyboard[Key.RControl];
            io.KeyAlt = keyboard[Key.LAlt] || keyboard[Key.RAlt];

            io.MousePos = _mousePos;
            io.MouseDown[0] = mouse.LeftButton == ButtonState.Pressed;
            io.MouseDown[1] = mouse.RightButton == ButtonState.Pressed;
            io.MouseDown[2] = mouse.MiddleButton == ButtonState.Pressed;

            float scrollDelta = mouse.Scroll.Y - _lastScrollValue;
            io.MouseWheel = scrollDelta;
            _lastScrollValue = mouse.Scroll.Y;
        }

        private static unsafe void renderDrawData(ImDrawDataPtr drawData)
        {
            // scale coordinates for retina displays
            ImGuiIOPtr io = ImGui.GetIO();
            int fbWidth = (int)(io.DisplaySize.X * io.DisplayFramebufferScale.X);
            int fbHeight = (int)(io.DisplaySize.Y * io.DisplayFramebufferScale.Y);
            if (fbWidth < 0 || fbHeight < 0) return;
            drawData.ScaleClipRects(io.DisplayFramebufferScale);

            // backup GL state
            GL.GetInteger(GetPName.ActiveTexture, out int lastActiveTexture);
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.GetInteger(GetPName.CurrentProgram, out int lastProgram);
            GL.GetInteger(GetPName.TextureBinding2D, out int lastTexture);
            GL.GetInteger(GetPName.SamplerBinding, out int lastSampler);
            GL.GetInteger(GetPName.ArrayBufferBinding, out int lastArrayBuffer);
            GL.GetInteger(GetPName.VertexArrayBinding, out int lastVertexArray);
            int[] lastPolygonMode = new int[2];
            GL.GetInteger(GetPName.PolygonMode, lastPolygonMode);
            int[] lastViewport = new int[4];
            GL.GetInteger(GetPName.Viewport, lastViewport);
            int[] lastScissorBox = new int[4];
            GL.GetInteger(GetPName.ScissorBox, lastScissorBox);
            GL.GetInteger(GetPName.BlendSrcRgb, out int lastBlendSrcRgb);
            GL.GetInteger(GetPName.BlendDstRgb, out int lastBlendDstRgb);
            GL.GetInteger(GetPName.BlendSrcAlpha, out int lastBlendSrcAlpha);
            GL.GetInteger(GetPName.BlendDstAlpha, out int lastBlendDstAlpha);
            GL.GetInteger(GetPName.BlendEquationRgb, out int lastBlendEquationRgb);
            GL.GetInteger(GetPName.BlendEquationAlpha, out int lastBlendEquationAlpha);
            bool lastEnableBlend = GL.IsEnabled(EnableCap.Blend);
            bool lastEnableCullFace = GL.IsEnabled(EnableCap.CullFace);
            bool lastEnableDepthTest = GL.IsEnabled(EnableCap.DepthTest);
            bool lastEnableScissorTest = GL.IsEnabled(EnableCap.ScissorTest);

            // setup GL state
            GL.Enable(EnableCap.Blend);
            GL.BlendEquation(BlendEquationMode.FuncAdd);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            GL.Disable(EnableCap.CullFace);
            GL.Disable(EnableCap.DepthTest);
            GL.Enable(EnableCap.ScissorTest);
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);

            // setup viewport with orthographic projecton matrix
            GL.Viewport(0, 0, fbWidth, fbHeight);
            float l = 0;
            float r = io.DisplaySize.X;
            float t = 0;
            float b = io.DisplaySize.Y;
            Matrix4 orthoProjection = new Matrix4(
                2.0f / (r - l), 0, 0, 0,
                0, 2.0f / (t - b), 0, 0,
                0, 0, -1.0f, 0,
                (r + l) / (l - r), (t + b) / (b - t), 0, 1.0f
            );

            GL.UseProgram(_programHandle);
            GL.Uniform1(_attribLocationTex, 0);
            GL.UniformMatrix4(_attribLocationProjMtx, false, ref orthoProjection);
            GL.BindSampler(0, 0);

            // recreate the VAO every time
            int vaoHandle = GL.GenVertexArray();
            GL.BindVertexArray(vaoHandle);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vboHandle);
            GL.EnableVertexAttribArray(_attribLocationPosition);
            GL.EnableVertexAttribArray(_attribLocationUV);
            GL.EnableVertexAttribArray(_attribLocationColor);
            GL.VertexAttribPointer(_attribLocationPosition, 2, VertexAttribPointerType.Float, false,
                ImGuiVert.Size,
                ImGuiVert.PosOffset);
            GL.VertexAttribPointer(_attribLocationUV, 2, VertexAttribPointerType.Float, false,
                ImGuiVert.Size,
                ImGuiVert.UVOffset);
            GL.VertexAttribPointer(_attribLocationColor, 4, VertexAttribPointerType.UnsignedByte, false,
                ImGuiVert.Size,
                ImGuiVert.ColOffset);

            // draw
            System.Numerics.Vector2 pos = new System.Numerics.Vector2(l, t);
            for (int n = 0; n < drawData.CmdListsCount; n++)
            {
                ImDrawListPtr cmdList = drawData.CmdListsRange[n];
                int indexBufferOffset = 0;

                GL.BindBuffer(BufferTarget.ArrayBuffer, _vboHandle);
                GL.BufferData(BufferTarget.ArrayBuffer, cmdList.VtxBuffer.Size * ImGuiVert.Size,
                    cmdList.VtxBuffer.Data, BufferUsageHint.StreamDraw);

                GL.BindBuffer(BufferTarget.ElementArrayBuffer, _elementsHandle);
                GL.BufferData(BufferTarget.ElementArrayBuffer, cmdList.IdxBuffer.Size * sizeof(ushort),
                    cmdList.IdxBuffer.Data, BufferUsageHint.StreamDraw);

                for (int cmdIndex = 0; cmdIndex < cmdList.CmdBuffer.Size; cmdIndex++)
                {
                    ImDrawCmdPtr pCmd = cmdList.CmdBuffer[cmdIndex];

                    Vector4 clipRect = new Vector4(
                        pCmd.ClipRect.X - pos.X,
                        pCmd.ClipRect.Y - pos.Y,
                        pCmd.ClipRect.Z - pos.X,
                        pCmd.ClipRect.W - pos.Y
                    );
                    GL.Scissor((int)clipRect.X, (int)(fbHeight - clipRect.W), (int)(clipRect.Z - clipRect.X),
                        (int)(clipRect.W - clipRect.Y));

                    GL.BindTexture(TextureTarget.Texture2D, pCmd.TextureId.ToInt32());
                    GL.DrawElements(BeginMode.Triangles, (int)pCmd.ElemCount,
                        DrawElementsType.UnsignedShort,
                        indexBufferOffset);

                    indexBufferOffset += (int)pCmd.ElemCount * 2;
                }
            }

            GL.DeleteVertexArray(vaoHandle);

            // restore modified GL state
            GL.UseProgram(lastProgram);
            GL.BindTexture(TextureTarget.Texture2D, lastTexture);
            GL.BindSampler(0, lastSampler);
            GL.ActiveTexture((TextureUnit)lastActiveTexture);
            GL.BindVertexArray(lastVertexArray);
            GL.BindBuffer(BufferTarget.ArrayBuffer, lastArrayBuffer);
            GL.BlendEquationSeparate((BlendEquationMode)lastBlendEquationRgb,
                (BlendEquationMode)lastBlendEquationAlpha);
            GL.BlendFuncSeparate((BlendingFactorSrc)lastBlendSrcRgb, (BlendingFactorDest)lastBlendDstRgb,
                (BlendingFactorSrc)lastBlendSrcAlpha, (BlendingFactorDest)lastBlendDstAlpha);
            if (lastEnableBlend) GL.Enable(EnableCap.Blend);
            else GL.Disable(EnableCap.Blend);
            if (lastEnableCullFace) GL.Enable(EnableCap.CullFace);
            else GL.Disable(EnableCap.CullFace);
            if (lastEnableDepthTest) GL.Enable(EnableCap.DepthTest);
            else GL.Disable(EnableCap.DepthTest);
            if (lastEnableScissorTest) GL.Enable(EnableCap.ScissorTest);
            else GL.Disable(EnableCap.ScissorTest);
            GL.PolygonMode(MaterialFace.FrontAndBack, (PolygonMode)lastPolygonMode[0]);
            GL.Viewport(lastViewport[0], lastViewport[1], lastViewport[2], lastViewport[3]);
            GL.Scissor(lastScissorBox[0], lastScissorBox[1], lastScissorBox[2], lastScissorBox[3]);
        }

        private static void createDeviceObjects()
        {
            // backup GL state
            GL.GetInteger(GetPName.TextureBinding2D, out int lastTexture);
            GL.GetInteger(GetPName.ArrayBufferBinding, out int lastArrayBuffer);
            GL.GetInteger(GetPName.VertexArrayBinding, out int lastVertexArray);

            const string vertexShader = @"
                #version 130
                uniform mat4 ProjMtx;
                in vec2 Position;
                in vec2 UV;
                in vec4 Color;
                out vec2 Frag_UV;
                out vec4 Frag_Color;
                void main()
                {
                    Frag_UV = UV;
                    Frag_Color = Color / 255.0;
                    gl_Position = ProjMtx * vec4(Position.xy, 0, 1);
                }
            ";

            const string fragmentShader = @"
                #version 130
                uniform sampler2D Texture;
                in vec2 Frag_UV;
                in vec4 Frag_Color;
                out vec4 Out_Color;
                void main()
                {
                    Out_Color = Frag_Color * texture(Texture, Frag_UV.st);
                }
            ";

            // create shaders
            _vertexShaderHandle = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(_vertexShaderHandle, vertexShader);
            GL.CompileShader(_vertexShaderHandle);
            checkShader(_vertexShaderHandle, "vertex");

            _fragmentShaderHandle = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(_fragmentShaderHandle, fragmentShader);
            GL.CompileShader(_fragmentShaderHandle);
            checkShader(_fragmentShaderHandle, "fragment");

            _programHandle = GL.CreateProgram();
            GL.AttachShader(_programHandle, _vertexShaderHandle);
            GL.AttachShader(_programHandle, _fragmentShaderHandle);
            GL.LinkProgram(_programHandle);
            checkProgram(_programHandle, "main");

            _attribLocationTex = GL.GetUniformLocation(_programHandle, "Texture");
            _attribLocationProjMtx = GL.GetUniformLocation(_programHandle, "ProjMtx");
            _attribLocationPosition = GL.GetAttribLocation(_programHandle, "Position");
            _attribLocationUV = GL.GetAttribLocation(_programHandle, "UV");
            _attribLocationColor = GL.GetAttribLocation(_programHandle, "Color");

            // create buffers
            _vboHandle = GL.GenBuffer();
            _elementsHandle = GL.GenBuffer();

            createFontsTexture();

            // restore modified openGL state
            GL.BindTexture(TextureTarget.Texture2D, lastTexture);
            GL.BindBuffer(BufferTarget.ArrayBuffer, lastArrayBuffer);
            GL.BindVertexArray(lastVertexArray);
        }

        private static void checkShader(int handle, string name)
        {
            GL.GetShader(handle, ShaderParameter.CompileStatus, out int status);
            GL.GetShader(handle, ShaderParameter.InfoLogLength, out int logLength);
            if (status == 0)
            {
                Log.Error("ERROR: Failed to compile ImGui {0} shader", name);
            }

            if (logLength > 0)
            {
                GL.GetShaderInfoLog(handle, out string infoLog);
                Log.Error("Shader info log: {0}", infoLog);
            }
        }

        private static void checkProgram(int handle, string name)
        {
            GL.GetProgram(handle, GetProgramParameterName.LinkStatus, out int status);
            GL.GetProgram(handle, GetProgramParameterName.InfoLogLength, out int logLength);
            if (status == 0)
            {
                Log.Error("ERROR: Failed to link ImGui {0} shader program", name);
            }

            if (logLength > 0)
            {
                GL.GetProgramInfoLog(handle, out string infoLog);
                Log.Error("Program info log: {0}", infoLog);
            }
        }

        private static void destroyDeviceObjects()
        {
            if (_vboHandle > 0) GL.DeleteBuffer(_vboHandle);
            if (_elementsHandle > 0) GL.DeleteBuffer(_elementsHandle);
            _vboHandle = _elementsHandle = 0;

            if (_programHandle > 0 && _vertexShaderHandle > 0) GL.DetachShader(_programHandle, _vertexShaderHandle);
            if (_vertexShaderHandle > 0) GL.DeleteShader(_vertexShaderHandle);
            _vertexShaderHandle = 0;

            if (_programHandle > 0 && _fragmentShaderHandle > 0) GL.DetachShader(_programHandle, _fragmentShaderHandle);
            if (_fragmentShaderHandle > 0) GL.DeleteShader(_fragmentShaderHandle);
            _fragmentShaderHandle = 0;

            if (_programHandle > 0) GL.DeleteProgram(_programHandle);
            _programHandle = 0;

            destroyFontsTexture();
        }

        private static void createFontsTexture()
        {
            // build texture atlas
            ImGuiIOPtr io = ImGui.GetIO();
            io.Fonts.GetTexDataAsRGBA32(out IntPtr pixels, out int width, out int height, out int bytesPerPixel);

            // copy data to managed array
            byte[] pixelsArray = new byte[width * height * bytesPerPixel];
            unsafe
            {
                Marshal.Copy(pixels, pixelsArray, 0, pixelsArray.Length);
            }

            // create texture
            GL.GetInteger(GetPName.TextureBinding2D, out int lastTexture);
            _fontsTextureHandle = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, _fontsTextureHandle);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter,
                (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter,
                (int)TextureMagFilter.Linear);
            GL.PixelStore(PixelStoreParameter.UnpackRowLength, 0);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, width, height, 0,
                PixelFormat.Rgba, PixelType.UnsignedByte, pixelsArray);

            io.Fonts.SetTexID((IntPtr)_fontsTextureHandle);
            io.Fonts.ClearTexData();

            // restore state
            GL.BindTexture(TextureTarget.Texture2D, lastTexture);
        }

        private static void destroyFontsTexture()
        {
            if (_fontsTextureHandle > 0)
            {
                ImGuiIOPtr io = ImGui.GetIO();
                GL.DeleteTexture(_fontsTextureHandle);
                io.Fonts.SetTexID(IntPtr.Zero);
                _fontsTextureHandle = 0;
            }
        }
    }
}
