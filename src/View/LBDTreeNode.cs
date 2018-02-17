using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LSDView.graphics;

namespace LSDView.view
{
    public class LBDTreeNode : RenderableMeshLayoutTreeNode
    {
        public LBDTreeNode(string text, IRenderable[] layout) : base(text, layout)
        {
            // intentionally empty
        }
    }
}
