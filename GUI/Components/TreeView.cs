using System.Collections.Generic;
using ImGuiNET;

namespace LSDView.GUI.Components
{
    public class TreeView : ImGuiComponent
    {
        public List<TreeNode> Nodes;

        private TreeNode _selected = null;

        public TreeView() { Nodes = new List<TreeNode>(); }

        protected override void renderSelf()
        {
            ImGui.BeginChild("tree");
            foreach (var node in Nodes)
            {
                node.OnSelectInChildren(select);
                node.FlagsInChildren(ImGuiTreeNodeFlags.OpenOnArrow | ImGuiTreeNodeFlags.DefaultOpen |
                                     ImGuiTreeNodeFlags.OpenOnDoubleClick);
                node.Render();
            }

            ImGui.EndChild();
        }

        private void select(TreeNode node)
        {
            if (_selected != null) _selected.Flags &= ~ImGuiTreeNodeFlags.Selected;
            _selected = node;
            node.Flags |= ImGuiTreeNodeFlags.Selected;
        }
    }
}
