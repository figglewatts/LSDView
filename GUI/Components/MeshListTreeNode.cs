using System;
using System.Collections.Generic;
using LSDView.Graphics;

namespace LSDView.GUI.Components
{
    public class MeshListTreeNode : TreeNode
    {
        public readonly List<IRenderable> Meshes;

        public MeshListTreeNode(string text,
            List<IRenderable> meshes,
            IEnumerable<TreeNode> children = null,
            Action<TreeNode> onSelect = null,
            ContextMenu contextMenu = null) :
            base(text, children, onSelect, contextMenu)
        {
            Meshes = meshes;
        }
    }
}
