using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using libLSD.Formats;
using LSDView.controller;
using LSDView.view;

namespace LSDView.view
{
    public class MOMTreeNode : RenderableMeshListTreeNode, IDataTypeTreeNode
    {
        public MOM Mom { get; }

        public MOMTreeNode(string text, MOM mom) : base(text)
        {
            Mom = mom;
        }

        public void Accept(IOutlineTreeViewVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
