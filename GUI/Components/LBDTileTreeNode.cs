using System;
using System.Collections.Generic;
using ImGuiNET;
using libLSD.Formats;
using LSDView.Graphics;
using OpenTK;

namespace LSDView.GUI.Components
{
    public class LBDTileTreeNode : MeshListTreeNode
    {
        protected readonly IRenderable _tileMesh;

        public LBDTileTreeNode(string text,
            IRenderable tileMesh,
            LBDTile tile,
            List<IRenderable> meshes) : base(text, meshes)
        {
            _tileMesh = tileMesh;

            var footstepSoundAndCollision = Convert.ToString(tile.FootstepSoundAndCollision, 2);
            var unknown = tile.UnknownFlag == 1 ? "true" : "false";
            _tooltip = () =>
            {
                ImGui.Text($"{footstepSoundAndCollision} - footstep/collision");
                ImGui.Text($"{unknown} - unknown");
                ImGui.Text($"{tile.UnknownFlag}");
            };
        }

        protected override void internalOnSelect()
        {
            _tileMesh.Material.SetParameter("ColorTint", new Vector4(1.5f, 1.5f, 1.5f, 1f), Vector4.One);
        }

        public override void OnDeselect() { _tileMesh.Material.SetParameter("ColorTint", Vector4.One, Vector4.One); }
    }
}
