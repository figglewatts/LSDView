using System.Numerics;
using LSDView.GUI.GUIComponents;

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

        public void Show()
        {
            createModal(Name, new InfoDialog(InfoDialog.DialogType.Info, Content), new Vector2(500, 50));
        }

        protected override void renderSelf() { }
    }
}
