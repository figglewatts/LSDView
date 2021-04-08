using System;
using System.Numerics;
using ImGuiNET;
using LSDView.controller;
using LSDView.Controllers;
using LSDView.GUI.Components;
using LSDView.Util;

namespace LSDView.GUI.GUIComponents
{
    public class MainMenuBar : ImGuiComponent
    {
        private readonly FileDialog _openDialog;
        private readonly FileDialog _openVramDialog;

        private bool _openFileOpenDialog = false;
        private bool _openVramOpenDialog = false;

        private readonly FileOpenController _fileOpenController;
        private readonly VRAMController _vramController;
        private readonly ConfigController _configController;
        private readonly CameraController _cameraController;

        private string _gameDataPathFieldValue;

        public MainMenuBar(FileOpenController fileOpenController,
            VRAMController vramController,
            ConfigController configController,
            CameraController cameraController)
        {
            _configController = configController;
            _gameDataPathFieldValue = _configController.Config.GameDataPath;
            _openDialog = new FileDialog(_configController.Config.GameDataPath, FileDialog.DialogType.Open);
            _openVramDialog = new FileDialog(_configController.Config.GameDataPath, FileDialog.DialogType.Open);
            _fileOpenController = fileOpenController;
            _vramController = vramController;
            _cameraController = cameraController;

            _configController.Config.OnGameDataPathChange += () =>
                _openDialog.InitialDir = _configController.Config.GameDataPath;
            _configController.Config.OnGameDataPathChange += () =>
                _openVramDialog.InitialDir = _configController.Config.GameDataPath;
        }

        public void OpenSetGameDataPath()
        {
            createModal("Set game data path...", new GenericDialog(setGameDataPathDialog),
                new Vector2(500, 85));
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

                if (ImGui.BeginMenu("VRAM"))
                {
                    renderVramMenu();
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

            if (_openVramOpenDialog)
            {
                _openVramDialog.Show(path => _vramController.LoadTIXIntoVRAM(path), ".tix");
                _openVramOpenDialog = false;
            }

            _openDialog.Render();
            _openVramDialog.Render();
        }

        private void renderFileMenu()
        {
            if (ImGui.MenuItem("Open"))
            {
                _openFileOpenDialog = true;
            }

            if (ImGui.BeginMenu("Open Recent"))
            {
                if (_configController.Config.RecentFiles.Count > 0)
                {
                    string fileToOpen = null;
                    foreach (var recentFile in _configController.Config.RecentFiles)
                    {
                        try
                        {
                            var relPath = PathUtil.MakeRelative(recentFile,
                                _configController.Config.GameDataPath);
                            if (ImGui.MenuItem(relPath))
                            {
                                fileToOpen = recentFile;
                                break;
                            }
                        }
                        catch (UriFormatException e)
                        {
                            Logger.Log()(LogLevel.WARN, $"Invalid recent file: '{recentFile}', invalid URI");
                        }
                    }

                    if (fileToOpen != null) _fileOpenController.OpenFile(fileToOpen);
                }
                else
                {
                    ImGui.MenuItem("No recent files!");
                }

                ImGui.EndMenu();
            }

            ImGui.Separator();

            if (ImGui.MenuItem("Set game data path..."))
            {
                OpenSetGameDataPath();
            }
        }

        private void setGameDataPathDialog()
        {
            ImGui.PushItemWidth(-1);
            ImGui.InputText("##gamedata", ref _gameDataPathFieldValue, 1024);
            ImGui.PopItemWidth();
            ImGui.Spacing();
            ImGui.SameLine(ImGui.GetWindowWidth() - 30);
            if (ImGui.Button("Ok"))
            {
                _configController.Config.GameDataPath = _gameDataPathFieldValue;
                _configController.Save();
                destroyModal("Set game data path...");
            }
        }

        private void renderVramMenu()
        {
            if (ImGui.MenuItem("Load VRAM"))
            {
                _openVramOpenDialog = true;
            }

            if (!_vramController.VRAMLoaded) return;

            ImGui.Separator();
            if (ImGui.MenuItem("Export VRAM..."))
            {
                _vramController.ExportLoadedVRAM();
            }
        }

        private void renderHelpMenu()
        {
            if (ImGui.MenuItem("About LSDView"))
            {
                createModal("About", new InfoDialog(InfoDialog.DialogType.Info, $@"LSDView {Version.String}
LSD: Dream Emulator data viewer
https://github.com/Figglewatts/LSDView

Made by Figglewatts, 2020"),
                    new Vector2(300, 100));
            }

            if (ImGui.MenuItem("Recenter view"))
            {
                _cameraController.RecenterView();
            }
        }
    }
}
