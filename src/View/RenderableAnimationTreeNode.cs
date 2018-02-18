using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LSDView.anim;
using LSDView.controller;
using LSDView.graphics;

namespace LSDView.view
{
    public class RenderableAnimationTreeNode : TreeNode, IRenderableTreeNode, IDataTypeTreeNode
    {
        public List<IRenderable> Renderables { get; }
        public AnimationPlayer Player { get; }
        public TODAnimation Animation { get; }

        public RenderableAnimationTreeNode(AnimationPlayer player, TODAnimation anim, string text)
            : base(text)
        {
            Animation = anim;
            Renderables = new List<IRenderable>();
            foreach (var renderable in Animation.AnimationMeshes)
            {
                Renderables.Add(renderable);
            }
            Player = player;
        }

        public void Accept(IOutlineTreeViewVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
