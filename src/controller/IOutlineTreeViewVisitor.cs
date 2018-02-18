using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LSDView.view;

namespace LSDView.controller
{
    public interface IOutlineTreeViewVisitor
    {
        void Visit(LBDTreeNode node);
        void Visit(MOMTreeNode node);
        void Visit(TIMTreeNode node);
        void Visit(TIXTreeNode node);
        void Visit(TMDTreeNode node);
        void Visit(TMDObjectTreeNode node);
        void Visit(RenderableAnimationTreeNode node);
    }
}
