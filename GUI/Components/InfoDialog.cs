using ImGuiNET;

namespace LSDView.GUI.GUIComponents
{
    public class InfoDialog : ImGuiComponent
    {
        public enum DialogType
        {
            Info,
            Warning,
            Error
        }

        public DialogType Type { get; }
        public string Message { get; }

        // TODO: configurable buttons...

        public InfoDialog(DialogType type, string message)
            : base()
        {
            Type = type;
            Message = message;
        }

        protected override void renderSelf() { ImGui.Text(Message); }
    }
}
