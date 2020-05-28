using System;
using System.Collections.Generic;
using System.Linq;
using ImGuiNET;

namespace LSDView.GUI.Components
{
    public class TreeNode : ImGuiComponent
    {
        public string Text;
        public ImGuiTreeNodeFlags Flags;
        public Action<TreeNode> OnSelect;

        public TreeNode(string text, IEnumerable<TreeNode> children = null, Action<TreeNode> onSelect = null)
        {
            if (children != null)
            {
                _children.AddRange(children);
            }
            else
            {
                Flags |= ImGuiTreeNodeFlags.Leaf;
            }

            OnSelect = onSelect;
            Text = text;
        }

        public void AddNode(TreeNode node)
        {
            AddChild(node);
            Flags &= ~ImGuiTreeNodeFlags.Leaf;
        }

        public void AddNodes(IEnumerable<TreeNode> nodes)
        {
            AddChildren(nodes);
            Flags &= ~ImGuiTreeNodeFlags.Leaf;
        }

        public void OnSelectInChildren(Action<TreeNode> onSelect)
        {
            OnSelect = onSelect;
            foreach (var child in _children.OfType<TreeNode>())
            {
                child.OnSelectInChildren(onSelect);
            }
        }

        public void FlagsInChildren(ImGuiTreeNodeFlags flags)
        {
            Flags |= flags;
            foreach (var child in _children.OfType<TreeNode>())
            {
                child.FlagsInChildren(flags);
            }
        }

        protected virtual void internalOnSelect() { }

        public virtual void OnDeselect() { }

        protected override void renderSelf()
        {
            bool show = ImGui.TreeNodeEx(Text, Flags);
            if (ImGui.IsItemClicked())
            {
                OnSelect?.Invoke(this);
                internalOnSelect();
            }

            if (show)
            {
                renderChildren();
                ImGui.TreePop();
            }
        }
    }
}
