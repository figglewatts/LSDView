using System.Numerics;
using ImGuiNET;

namespace LSDView.GUI.GUIComponents
{
    public class MainMenuBar : ImGuiComponent
    {
        private readonly FileDialog _openDialog;
        private readonly FileDialog _openSchemaDialog;
        private readonly FileDialog _saveDialog;

        private bool _openFileOpenDialog = false;
        private bool _openFileSaveDialog = false;
        private bool _openSchemaOpenDialog = false;

        public MainMenuBar(MainWindow window)
            : base(window)
        {
            _openDialog = new FileDialog("", FileDialog.DialogType.Open, window);
            _openSchemaDialog = new FileDialog("", FileDialog.DialogType.Open, window);
            _saveDialog = new FileDialog("", FileDialog.DialogType.Save, window);
        }

        public override void Render()
        {
            if (ImGui.BeginMainMenuBar())
            {
                if (ImGui.BeginMenu("File"))
                {
                    renderFileMenu();
                    ImGui.EndMenu();
                }

                if (ImGui.BeginMenu("Edit"))
                {
                    renderEditMenu();
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
                _openDialog.Show((s) => { }, "*.json");
                _openFileOpenDialog = false;
            }

            if (_openFileSaveDialog)
            {
                _saveDialog.Show((s) => { }, "*", ".json");
                _openFileSaveDialog = false;
            }

            if (_openSchemaOpenDialog)
            {
                _openSchemaDialog.Show((s) => { }, "*.json");
                _openSchemaOpenDialog = false;
            }

            _openDialog.Render();
            _openSchemaDialog.Render();
            _saveDialog.Render();

            renderModals();
        }

        private void renderFileMenu()
        {
            if (ImGui.MenuItem("New"))
            {
                createModal("Test modal", new InfoDialog(InfoDialog.DialogType.Info, "Test message", _window),
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
                createModal("Test modal", new InfoDialog(InfoDialog.DialogType.Info, "Test message", _window),
                    new Vector2(200, 200));
            }
        }
    }
}
