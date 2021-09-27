using System;
using System.Collections.Generic;
using LSDView.Controllers.Interface;
using LSDView.Graphics;
using LSDView.Models;

namespace LSDView.GUI.Components
{
    public class AnimatedMeshListTreeNode : MeshListTreeNode
    {
        protected int _animation { get; }
        protected MOMDocument _entity { get; }
        protected readonly IAnimationController _animationController;

        public AnimatedMeshListTreeNode(string text,
            List<IRenderable> meshes,
            MOMDocument entity,
            int animation,
            IAnimationController animationController,
            IEnumerable<TreeNode> children = null,
            Action<TreeNode> onSelect = null,
            ContextMenu contextMenu = null) : base(text, meshes, children, onSelect, contextMenu)
        {
            _animation = animation;
            _animationController = animationController;
            _entity = entity;
        }

        protected override void internalOnSelect() { _animationController.SetFocus(_entity, _animation); }

        public override void OnDeselect() { _animationController.SetFocus(null); }
    }
}
