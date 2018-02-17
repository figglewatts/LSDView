using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using libLSD.Formats;
using LSDView.graphics;
using LSDView.model;
using LSDView.view;

namespace LSDView.controller
{
    public class OutlineController
    {
        public ILSDView View { get; set; }

        public TMDController TMDController { get; set; }
        public TIMController TIMController { get; set; }
        public TIXController TIXController { get; set; }
        public MOMController MOMController { get; set; }
        public LBDController LBDController { get; set; }

        public OutlineController(ILSDView view)
        {
            View = view;
        }

        public void PopulateOutlineWithDocument(IDocument doc, string rootName)
        {
            TreeNode node = null;
            switch (doc.Type)
            {
                case DocumentType.TMD:
                    node = CreateTMDNode(rootName, TMDController.Meshes);
                    break;
                case DocumentType.TIM:
                    node = CreateTIMNode(rootName, TIMController.TextureMesh);
                    break;
                case DocumentType.TIX:
                    node = CreateTIXNode(rootName, TIXController.TIXTextureMeshes);
                    break;
                case DocumentType.MOM:
                    node = CreateMOMNode(rootName, MOMController.MomData);
                    break;
                case DocumentType.LBD:
                    node = CreateLBDNode(rootName, LBDController.TileLayout, LBDController.TileMeshes,
                        LBDController.Moms);
                    break;
                default:
                    throw new ArgumentException($"Unrecognized document type {doc.Type}");
            }

            View.ViewOutline.BeginUpdate();
            View.ViewOutline.Nodes.Add(node);
            View.ViewOutline.EndUpdate();
        }

        public void ClearOutline()
        {
            View.ViewOutline.BeginUpdate();
            View.ViewOutline.Nodes.Clear();
            View.ViewOutline.EndUpdate();
        }

        private TreeNode CreateTMDNode(string name, List<Mesh> meshes)
        {
            TreeNode tmdNode = new RenderableMeshListTreeNode(name);

            int i = 0;
            foreach (var m in meshes)
            {
                tmdNode.Nodes.Add(new RenderableMeshTreeNode("Object " + i.ToString(), m));
                i++;
            }

            View.ViewOutline.SelectedNode = tmdNode;
            return tmdNode;
        }

        private TreeNode CreateTIMNode(string name, Mesh textureMesh)
        {
            TreeNode timNode = new RenderableMeshTreeNode(name, textureMesh);

            View.ViewOutline.SelectedNode = timNode;
            return timNode;
        }

        private TreeNode CreateTIXNode(string name, List<Mesh> tixTextureMeshes)
        {
            TreeNode baseTreeNode = new RenderableMeshTreeNode(name, tixTextureMeshes[0]);

            int timNumber = 0;
            foreach (Mesh mesh in tixTextureMeshes)
            {
                TreeNode subNode = CreateTIMNode($"Texture {timNumber}", mesh);
                baseTreeNode.Nodes.Add(subNode);

                timNumber++;
            }

            View.ViewOutline.SelectedNode = baseTreeNode;
            return baseTreeNode;
        }

        private TreeNode CreateMOMNode(string name, MOMData data)
        {
            TreeNode momNode = CreateTMDNode(name, data.MomTmd);

            int i = 0;
            foreach (var anim in data.Animations)
            {
                RenderableAnimationTreeNode animNode =
                    new RenderableAnimationTreeNode(View.AnimPlayer, anim, "TOD " + i.ToString());
                momNode.Nodes.Add(animNode);
                i++;
            }

            View.ViewOutline.SelectedNode = momNode;
            return momNode;
        }

        private TreeNode CreateLBDNode(string name, List<Mesh> tileLayout, List<Mesh> tileMeshes, List<MOMData> moms)
        {
            TreeNode lbdNode = new RenderableMeshLayoutTreeNode(name, tileLayout.ToArray());

            TreeNode tilesTmdNode = CreateTMDNode("Tiles TMD", tileMeshes);

            int i = 0;
            foreach (var mom in moms)
            {
                TreeNode momNode = CreateMOMNode($"MOM {i}", mom);
                lbdNode.Nodes.Add(momNode);

                i++;
            }

            lbdNode.Nodes.Add(tilesTmdNode);
            View.ViewOutline.SelectedNode = lbdNode;
            return lbdNode;
        }
    }
}
