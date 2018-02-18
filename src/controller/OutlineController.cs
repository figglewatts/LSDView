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
                    node = CreateTMDNode(rootName, TMDController.Meshes, ((TMDDocument)doc).Document);
                    break;
                case DocumentType.TIM:
                    node = CreateTIMNode(rootName, TIMController.TextureMesh, ((TIMDocument)doc).Document);
                    break;
                case DocumentType.TIX:
                    node = CreateTIXNode(rootName, TIXController.TIXTextureMeshes, ((TIXDocument)doc).Document);
                    break;
                case DocumentType.MOM:
                    node = CreateMOMNode(rootName, MOMController.MomData);
                    break;
                case DocumentType.LBD:
                    node = CreateLBDNode(rootName, LBDController.TileLayout, LBDController.TileMeshes,
                        LBDController.Moms, ((LBDDocument)doc).Document);
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

        private TreeNode CreateTMDNode(string name, List<Mesh> meshes, TMD tmd)
        {
            TreeNode tmdNode = new TMDTreeNode(name, tmd);

            int i = 0;
            foreach (var m in meshes)
            {
                tmdNode.Nodes.Add(new TMDObjectTreeNode("Object " + i.ToString(), m, tmd, i));
                i++;
            }

            View.ViewOutline.SelectedNode = tmdNode;
            return tmdNode;
        }

        private TreeNode CreateTIMNode(string name, Mesh textureMesh, TIM tim)
        {
            TreeNode timNode = new TIMTreeNode(name, textureMesh, tim);

            View.ViewOutline.SelectedNode = timNode;
            return timNode;
        }

        private TreeNode CreateTIXNode(string name, List<Mesh> tixTextureMeshes, TIX tix)
        {
            TreeNode baseTreeNode = new TIXTreeNode(name, tixTextureMeshes[0], tix);

            int timNumber = 0;
            foreach (Mesh mesh in tixTextureMeshes)
            {
                TreeNode subNode = CreateTIMNode($"Texture {timNumber}", mesh, tix.AllTIMs[timNumber]);
                baseTreeNode.Nodes.Add(subNode);

                timNumber++;
            }

            View.ViewOutline.SelectedNode = baseTreeNode;
            return baseTreeNode;
        }

        private TreeNode CreateMOMNode(string name, MOMData data)
        {
            TreeNode momNode = new MOMTreeNode(name, data.Mom);

            TreeNode momTmdNode = CreateTMDNode("Models", data.MomTmd, data.Mom.TMD);
            momNode.Nodes.Add(momTmdNode);

            int i = 0;
            foreach (var anim in data.Animations)
            {
                RenderableAnimationTreeNode animNode =
                    new RenderableAnimationTreeNode(View.AnimPlayer, anim, "Animation " + i.ToString());
                momNode.Nodes.Add(animNode);
                i++;
            }

            View.ViewOutline.SelectedNode = momNode;
            return momNode;
        }

        private TreeNode CreateLBDNode(string name, List<Mesh> tileLayout, List<Mesh> tileMeshes, List<MOMData> moms, LBD lbd)
        {
            TreeNode lbdNode = new LBDTreeNode(name, tileLayout.ToArray(), lbd);

            TreeNode tilesTmdNode = CreateTMDNode("Tiles", tileMeshes, lbd.Tiles);

            int i = 0;
            foreach (var mom in moms)
            {
                TreeNode momNode = CreateMOMNode($"Entity {i}", mom);
                lbdNode.Nodes.Add(momNode);

                i++;
            }

            lbdNode.Nodes.Add(tilesTmdNode);
            View.ViewOutline.SelectedNode = lbdNode;
            return lbdNode;
        }
    }
}
