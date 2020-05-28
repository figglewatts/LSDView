using System;
using System.Collections.Generic;
using LSDView.Controllers;
using LSDView.Graphics;
using LSDView.Models;

namespace LSDView.GUI.Components
{
    public class AnimatedMeshListTreeNode : MeshListTreeNode
    {
        protected int _animation { get; }
        protected MOMDocument _entity { get; }
        protected readonly AnimationController _animationController;

        public AnimatedMeshListTreeNode(string text,
            List<Mesh> meshes,
            MOMDocument entity,
            int animation,
            AnimationController animationController,
            IEnumerable<TreeNode> children = null,
            Action<TreeNode> onSelect = null) : base(text, meshes, children, onSelect)
        {
            _animation = animation;
            _animationController = animationController;
            _entity = entity;
        }

        protected override void internalOnSelect() { _animationController.SetFocus(_entity, _animation); }

        public override void OnDeselect() { _animationController.SetFocus(null); }
    }
}
