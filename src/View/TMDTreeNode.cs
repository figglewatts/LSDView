using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using libLSD.Formats;

namespace LSDView.view
{
    public class TMDTreeNode : RenderableMeshListTreeNode
    {
        public TMD Tmd { get; }

        public TMDTreeNode(string text, TMD tmd) : base(text)
        {
            Tmd = tmd;
        }
    }
}
