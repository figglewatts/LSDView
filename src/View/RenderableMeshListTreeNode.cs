using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LSDView.graphics;

namespace LSDView.view
{
    public class RenderableMeshListTreeNode : TreeNode, IRenderableTreeNode
    {
        public List<IRenderable> Renderables
        {
            get
            {
                List<IRenderable> renderables = new List<IRenderable>();
                foreach (var n in Nodes)
                {
                    if (n is RenderableMeshTreeNode objNode)
                        renderables.AddRange(objNode.Renderables);
                }

                return renderables;
            }
        }

        public RenderableMeshListTreeNode(string text) : base(text) {}
    }
}
