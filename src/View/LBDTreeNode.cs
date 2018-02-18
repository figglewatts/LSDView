using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using libLSD.Formats;
using LSDView.controller;
using LSDView.graphics;

namespace LSDView.view
{
    public class LBDTreeNode : RenderableMeshLayoutTreeNode, IDataTypeTreeNode
    {
        public LBD Lbd { get; }

        public LBDTreeNode(string text, IRenderable[] layout, LBD lbd) : base(text, layout)
        {
            Lbd = lbd;
        }

        public void Accept(IOutlineTreeViewVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
