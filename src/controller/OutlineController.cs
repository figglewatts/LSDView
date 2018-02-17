using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using libLSD.Formats;
using LSDView.model;
using LSDView.view;

namespace LSDView.controller
{
    public class OutlineController
    {
        public ILSDView View { get; set; }

        public TMDController TMDController { get; set; }
        public TIMController TIMController { get; set; }

        public OutlineController(ILSDView view)
        {
            View = view;
        }

        public void PopulateOutlineWithDocument(IDocument doc)
        {
            switch (doc.Type)
            {
                case DocumentType.TMD:
                    PopulateOutlineWithTMD(doc as AbstractDocument<TMD>);
                    break;
                case DocumentType.TIM:
                    PopulateOutlineWithTIM(doc as AbstractDocument<TIM>);
                    break;
            }
        }

        public void ClearOutline()
        {
            View.ViewOutline.BeginUpdate();
            View.ViewOutline.Nodes.Clear();
            View.ViewOutline.EndUpdate();
        }

        private void PopulateOutlineWithTMD(AbstractDocument<TMD> tmd)
        {
            TreeNode tmdNode = new RenderableMeshListTreeNode(Path.GetFileName(TMDController.TMDPath));

            int i = 0;
            foreach (var m in TMDController.Meshes)
            {
                tmdNode.Nodes.Add(new RenderableMeshTreeNode("Object " + i.ToString(), m));
                i++;
            }

            View.ViewOutline.BeginUpdate();
            View.ViewOutline.Nodes.Add(tmdNode);
            View.ViewOutline.EndUpdate();
            View.ViewOutline.SelectedNode = tmdNode;
        }

        private void PopulateOutlineWithTIM(AbstractDocument<TIM> tim)
        {
            TreeNode timNode = new RenderableMeshTreeNode(Path.GetFileName(TIMController.TIMPath),TIMController.TextureMesh);

            View.ViewOutline.BeginUpdate();
            View.ViewOutline.Nodes.Clear();
            View.ViewOutline.Nodes.Add(timNode);
            View.ViewOutline.EndUpdate();
            View.ViewOutline.SelectedNode = timNode;
        }
    }
}
