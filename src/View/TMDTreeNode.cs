using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using libLSD.Formats;
using LSDView.controller;

namespace LSDView.view
{
    public class TMDTreeNode : RenderableMeshListTreeNode, IDataTypeTreeNode
    {
        public TMD Tmd { get; }

        public TMDTreeNode(string text, TMD tmd) : base(text)
        {
            Tmd = tmd;
        }

        public void Accept(IOutlineTreeViewVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
