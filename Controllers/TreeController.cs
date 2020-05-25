using System;
using libLSD.Formats;
using LSDView.GUI.Components;
using LSDView.Models;

namespace LSDView.Controllers
{
    public class TreeController
    {
        public TreeView Tree { get; private set; }

        public void PopulateTreeWithDocument(IDocument doc, string rootName)
        {
            Tree.Nodes.Clear();
            switch (doc.Type)
            {
                case DocumentType.LBD:
                    Tree.Nodes.Add(createLBDNode(rootName, ((LBDDocument)doc).Document));
                    break;
                case DocumentType.TMD:
                    break;
                case DocumentType.TIM:
                    break;
                case DocumentType.MOM:
                    break;
                case DocumentType.TIX:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void SetTree(TreeView tree) { Tree = tree; }

        private TreeNode createLBDNode(string name, LBD lbd)
        {
            TreeNode rootNode = new TreeNode(name);

            TreeNode tilesNode = createTMDNode("Tiles", lbd.Tiles);
            rootNode.AddNode(tilesNode);

            if (lbd.MML != null)
            {
                TreeNode objectsNode = createMMLNode("Entities", lbd.MML.Value);
                rootNode.AddNode(objectsNode);
            }

            return rootNode;
        }

        private TreeNode createMMLNode(string name, MML mml)
        {
            TreeNode rootNode = new TreeNode(name);

            for (int i = 0; i < mml.NumberOfMOMs; i++)
            {
                TreeNode momNode = new TreeNode($"Entity {i}");
                rootNode.AddNode(momNode);
            }

            return rootNode;
        }

        private TreeNode createMOMNode(string name, MOM mom)
        {
            TreeNode rootNode = new TreeNode(name);

            TreeNode modelsNode = createTMDNode("Models", mom.TMD);
            rootNode.AddNode(modelsNode);

            TreeNode animationsNode = createMOSNode("Animations", mom.MOS);
            rootNode.AddNode(animationsNode);

            return rootNode;
        }

        private TreeNode createMOSNode(string name, MOS mos)
        {
            TreeNode rootNode = new TreeNode(name);

            for (int i = 0; i < mos.NumberOfTODs; i++)
            {
                TreeNode animationNode = new TreeNode($"Animation {i}");
                rootNode.AddNode(animationNode);
            }

            return rootNode;
        }

        private TreeNode createTMDNode(string name, TMD tmd)
        {
            TreeNode rootNode = new TreeNode(name);

            for (int i = 0; i < tmd.Header.NumObjects; i++)
            {
                TreeNode objNode = new TreeNode($"Object {i}");
                rootNode.AddNode(objNode);
            }

            return rootNode;
        }
    }
}
