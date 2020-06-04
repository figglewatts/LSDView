using System.Numerics;
using ImGuiNET;
using LSDView.GUI;

namespace JsonAnything.GUI.GUIComponents
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
                ImGuiTreeNodeFlags flags = ImGuiTreeNodeFlags.DefaultOpen | ImGuiTreeNodeFlags.OpenOnDoubleClick |
                                           ImGuiTreeNodeFlags.OpenOnArrow;

                renderChildren();

                ImGui.End();
            }

            ImGui.PopStyleVar();
        }
    }
}
