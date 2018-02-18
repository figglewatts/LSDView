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
    public class TIXTreeNode : RenderableMeshTreeNode, IDataTypeTreeNode
    {
        public TIX Tix { get; }

        public TIXTreeNode(string text, IRenderable r, TIX tix) : base(text, r)
        {
            Tix = tix;
        }

        public void Accept(IOutlineTreeViewVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
