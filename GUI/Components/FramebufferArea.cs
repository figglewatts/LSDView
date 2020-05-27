using System;
using System.Numerics;
using ImGuiNET;
using LSDView.Graphics;

namespace LSDView.GUI.Components
{
    public class FramebufferArea : ImGuiComponent
    {
        private Vector2 _lastDimension;
        private readonly Framebuffer _framebuffer;

        public FramebufferArea(Framebuffer frameBuffer) { _framebuffer = frameBuffer; }

        protected override void renderSelf()
        {
            var region = ImGui.GetContentRegionAvail();
            if (_lastDimension != region)
            {
                _framebuffer.Resize((int)region.X, (int)region.Y);
                Console.WriteLine("Resizing");
                _lastDimension = region;
            }

            ImGui.Image((IntPtr)_framebuffer.Texture.Handle, region, new Vector2(0, 1), new Vector2(1, 0));
        }
    }
}
