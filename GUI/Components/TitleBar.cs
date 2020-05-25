using System.Numerics;
using ImGuiNET;
using LSDView.Controllers;

namespace LSDView.GUI.GUIComponents
{
    public class MainMenuBar : ImGuiComponent
    {
        private readonly FileDialog _openDialog;

        private bool _openFileOpenDialog = false;
        private bool _openFileSaveDialog = false;
        private bool _openSchemaOpenDialog = false;

        private readonly FileOpenController _fileOpenController;

        public MainMenuBar(FileOpenController fileOpenController)
        {
            _openDialog = new FileDialog("", FileDialog.DialogType.Open);
            _fileOpenController = fileOpenController;
        }

        protected override void renderSelf()
        {
            if (ImGui.BeginMainMenuBar())
            {
                if (ImGui.BeginMenu("File"))
                {
                    renderFileMenu();
                    ImGui.EndMenu();
                }

                if (ImGui.BeginMenu("Help"))
                {
                    renderHelpMenu();
                    ImGui.EndMenu();
                }
            }

            ImGui.EndMainMenuBar();

            if (_openFileOpenDialog)
            {
                _openDialog.Show(path => _fileOpenController.OpenFile(path), ".lbd|.tix|.mom|.tmd|.tim");
                _openFileOpenDialog = false;
            }

            _openDialog.Render();
        }

        private void renderFileMenu()
        {
            if (ImGui.MenuItem("New"))
            {
                createModal("Test modal", new InfoDialog(InfoDialog.DialogType.Info, "Test message"),
                    new Vector2(200, 200));
            }

            if (ImGui.MenuItem("Open"))
            {
                _openFileOpenDialog = true;
            }

            if (ImGui.BeginMenu("Open Recent"))
            {
                ImGui.MenuItem("file1...");
                ImGui.MenuItem("file2...");
                ImGui.EndMenu();
            }

            ImGui.Separator();

            if (ImGui.MenuItem("Load schema"))
            {
                _openSchemaOpenDialog = true;
            }

            if (ImGui.MenuItem("Unload schema"))
            {
                // TODO: dialog for "are you sure you want to unload?"
            }

            ImGui.Separator();

            if (ImGui.MenuItem("Save")) { }

            if (ImGui.MenuItem("Save As..."))
            {
                _openFileSaveDialog = true;
            }
        }

        private void renderEditMenu()
        {
            if (ImGui.MenuItem("Undo")) { }

            if (ImGui.MenuItem("Redo")) { }

            ImGui.Separator();

            if (ImGui.MenuItem("Cut")) { }

            if (ImGui.MenuItem("Copy")) { }

            if (ImGui.MenuItem("Paste")) { }

            if (ImGui.MenuItem("Select All")) { }
        }

        private void renderHelpMenu()
        {
            if (ImGui.MenuItem("About LSDView"))
            {
                createModal("Test modal", new InfoDialog(InfoDialog.DialogType.Info, "Test message"),
                    new Vector2(200, 200));
            }
        }
    }
}
