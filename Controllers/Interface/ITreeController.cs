using LSDView.GUI.Components;
using LSDView.Models;
using OpenTK;

namespace LSDView.Controllers.Interface
{
    public interface ITreeController
    {
        TreeView<TreeNode> Tree { get; }

        void PopulateWithDocument(IDocument doc, string rootName);
        void RenderSelectedNode(Matrix4 view, Matrix4 projection);
    }
}
