using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LSDView.graphics;

namespace LSDView.view
{
    public interface IRenderableTreeNode
    {
        List<IRenderable> Renderables { get; }
    }
}
