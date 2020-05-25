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

        public InfoDialog(DialogType type, string message, MainWindow window)
            : base(window)
        {
            Type = type;
            Message = message;
        }

        public override void Render() { ImGui.Text(Message); }
    }
}
