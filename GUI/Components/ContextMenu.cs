using System;
using System.Collections.Generic;
using ImGuiNET;

namespace LSDView.GUI.Components
{
    public class ContextMenu
    {
        public readonly Dictionary<string, Action> MenuItems;

        public ContextMenu(Dictionary<string, Action> menuItems) { MenuItems = menuItems; }

        public void Render()
        {
            if (ImGui.BeginPopupContextItem())
            {
                foreach (var item in MenuItems)
                {
                    if (ImGui.Selectable(item.Key)) item.Value();
                }

                ImGui.EndPopup();
            }
        }

        public bool Equals(ContextMenu other) { return Equals(MenuItems, other.MenuItems); }

        public override bool Equals(object obj) { return obj is ContextMenu other && Equals(other); }

        public override int GetHashCode() { return (MenuItems != null ? MenuItems.GetHashCode() : 0); }
    }
}
