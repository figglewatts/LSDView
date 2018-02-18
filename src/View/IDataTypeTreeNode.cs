using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LSDView.controller;

namespace LSDView.view
{
    public interface IDataTypeTreeNode
    {
        void Accept(IOutlineTreeViewVisitor visitor);
    }
}
