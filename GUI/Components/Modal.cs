using LSDView.GUI.GUIComponents;
using OpenTK;

namespace LSDView.GUI.Components
{
    public class Modal : ImGuiComponent
    {
        public string Name { get; }
        public string Content { get; }

        public Modal(string name, string content)
        {
            Name = name;
            Content = content;
        }

        public void ShowModal()
        {
            createModal(Name, new InfoDialog(InfoDialog.DialogType.Info, Content), new Vector2(500, 50));
        }

        protected override void renderSelf() { }
    }
}
