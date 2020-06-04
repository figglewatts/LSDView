using System;

namespace LSDView.GUI.Components
{
    public class GenericDialog : ImGuiComponent
    {
        private readonly Action _dialogContents;

        public GenericDialog(Action dialogContents) { _dialogContents = dialogContents; }

        protected override void renderSelf() { _dialogContents(); }
    }
}
