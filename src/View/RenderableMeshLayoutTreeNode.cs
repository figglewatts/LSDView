using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LSDView.graphics;

namespace LSDView.view
{
	public class RenderableMeshLayoutTreeNode : TreeNode, IRenderableTreeNode
	{
		public List<IRenderable> Renderables { get; }

		public RenderableMeshLayoutTreeNode(string text, IRenderable[] layout)
			: base(text)
		{
			Renderables = new List<IRenderable>();
			foreach (IRenderable r in layout)
			{
				Renderables.Add(r);
			}
		}
	}
}
