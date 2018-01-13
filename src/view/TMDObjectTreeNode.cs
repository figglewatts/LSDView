using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LSDView.graphics;

namespace LSDView.view
{
    public class TMDObjectTreeNode : TreeNode, IRenderableTreeNode
    {
        public List<IRenderable> Renderables { get; }

        public TMDObjectTreeNode(string text, IRenderable r) : base(text)
        {
            Renderables = new List<IRenderable>();
            Renderables.Add(r);
        }
    }
}
