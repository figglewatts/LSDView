using System;
using LSDView.GUI.GUIComponents;

namespace LSDView.Controllers.GUI
{
    public class GUIExportController : AbstractExportController
    {
        protected FileDialog _fileExportDialog;

        public void ProvideFileExportDialog(FileDialog fileExportDialog) { _fileExportDialog = fileExportDialog; }

        public void OpenDialog(Action<string> onSubmit, string fileSaveType)
        {
            GuiApplication.Instance.NextGuiRender += () =>
            {
                _fileExportDialog.ShowDialog(onSubmit, fileSaveType: fileSaveType);
            };
        }
    }
}
