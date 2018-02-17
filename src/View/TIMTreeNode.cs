using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using libLSD.Formats;
using LSDView.graphics;

namespace LSDView.view
{
    public class TIMTreeNode : RenderableMeshTreeNode
    {
        public TIM Tim { get; }

        public TIMTreeNode(string text, IRenderable r, TIM tim) : base(text, r)
        {
            Tim = tim;
        }
    }
}
