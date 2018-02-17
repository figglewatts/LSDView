using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LSDView.graphics;
using LSDView.view;

namespace LSDView.view
{
    public class TMDObjectTreeNode : RenderableMeshTreeNode
    {
        public TMDObjectTreeNode(string text, IRenderable r) : base(text, r)
        {
            // intentionally empty
        }
    }
}
