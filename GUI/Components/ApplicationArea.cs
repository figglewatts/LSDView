using System.Numerics;
using ImGuiNET;

namespace LSDView.GUI.Components
{
    public class ApplicationArea : ImGuiComponent
    {
        private const int TITLEBAR_HEIGHT = 19;

        private readonly ImGuiIOPtr _io;

        public ApplicationArea() { _io = ImGui.GetIO(); }

        protected override void renderSelf()
        {
            ImGui.SetNextWindowPos(new Vector2(0, TITLEBAR_HEIGHT), ImGuiCond.Always, Vector2.Zero);
            ImGui.SetNextWindowSize(new Vector2(_io.DisplaySize.X, _io.DisplaySize.Y - TITLEBAR_HEIGHT),
                ImGuiCond.Always);
            ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding, 0);
            if (ImGui.Begin("",
                ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse |
                ImGuiWindowFlags.NoSavedSettings |
                ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoBringToFrontOnFocus))
            {
                renderChildren();

                ImGui.End();
            }

            ImGui.PopStyleVar();
        }
    }
}
