using System;
using System.Collections.Generic;
using LSDView.Graphics;

namespace LSDView.GUI.Components
{
    public class MeshTreeNode : TreeNode
    {
        public readonly List<Mesh> Meshes;

        public MeshTreeNode(string text,
            List<Mesh> meshes,
            IEnumerable<TreeNode> children = null,
            Action<TreeNode> onSelect = null) :
            base(text, children, onSelect)
        {
            Meshes = meshes;
        }
    }
}
