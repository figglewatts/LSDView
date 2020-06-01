using System;
using System.Collections.Generic;
using libLSD.Formats;
using LSDView.Graphics;
using LSDView.GUI.Components;
using LSDView.Models;
using OpenTK;

namespace LSDView.Controllers
{
    public class TreeController
    {
        public TreeView<MeshListTreeNode> Tree { get; private set; }

        private readonly AnimationController _animationController;

        public TreeController(AnimationController animationController) { _animationController = animationController; }

        public void PopulateTreeWithDocument(IDocument doc, string rootName)
        {
            Tree.Deselect();
            Tree.Nodes.Clear();
            MeshListTreeNode node = null;
            switch (doc.Type)
            {
                case DocumentType.LBD:
                    node = createLBDNode(rootName, doc as LBDDocument);
                    break;
                case DocumentType.TMD:
                    node = createTMDNode(rootName, doc as TMDDocument);
                    break;
                case DocumentType.TIM:
                    node = createTIMNode(rootName, doc as TIMDocument);
                    break;
                case DocumentType.MOM:
                    node = createMOMNode(rootName, doc as MOMDocument);
                    break;
                case DocumentType.TIX:
                    node = createTIXNode(rootName, doc as TIXDocument);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            Tree.SetNode(node);
        }

        public void RenderSelectedNode(Matrix4 view, Matrix4 projection)
        {
            if (Tree.Selected is MeshListTreeNode renderableTreeNode)
            {
                foreach (var mesh in renderableTreeNode.Meshes)
                {
                    mesh.Render(view, projection);
                }
            }
        }

        public void SetTree(TreeView<MeshListTreeNode> tree) { Tree = tree; }

        private MeshListTreeNode createTIXNode(string name, TIXDocument tixDoc)
        {
            MeshListTreeNode rootNode = new MeshListTreeNode(name, new List<Mesh> {tixDoc.TIMs[0].TextureMesh});

            for (int i = 0; i < tixDoc.TIMs.Count; i++)
            {
                MeshListTreeNode timNode = createTIMNode($"Texture {i}", tixDoc.TIMs[i]);
                rootNode.AddNode(timNode);
            }

            return rootNode;
        }

        private MeshListTreeNode createTIMNode(string name, TIMDocument timDoc)
        {
            return new MeshListTreeNode(name, new List<Mesh> {timDoc.TextureMesh}, contextMenu: new ContextMenu(
                new Dictionary<string, Action>
                {
                    {"Export as TIM", () => { }}
                }));
        }

        private MeshListTreeNode createLBDNode(string name, LBDDocument lbdDoc)
        {
            MeshListTreeNode rootNode = new MeshListTreeNode(name, lbdDoc.TileLayout);

            TreeNode tilesNode = createTMDNode("Tiles", lbdDoc.TilesTMD);
            rootNode.AddNode(tilesNode);

            if (lbdDoc.Entities != null)
            {
                TreeNode objectsNode = createMMLNode("Entities", lbdDoc.Entities);
                rootNode.AddNode(objectsNode);
            }

            return rootNode;
        }

        private TreeNode createMMLNode(string name, List<MOMDocument> entities)
        {
            MeshListTreeNode rootNode = new MeshListTreeNode(name, entities[0].Models.ObjectMeshes);

            for (int i = 0; i < entities.Count; i++)
            {
                MeshListTreeNode momNode = createMOMNode($"Entity {i}", entities[i]);
                rootNode.AddNode(momNode);
            }

            return rootNode;
        }

        private MeshListTreeNode createMOMNode(string name, MOMDocument momDoc)
        {
            MeshListTreeNode rootNode = new MeshListTreeNode(name, momDoc.Models.ObjectMeshes);

            MeshListTreeNode modelsNode = createTMDNode("Models", momDoc.Models);
            rootNode.AddNode(modelsNode);

            AnimatedMeshListTreeNode animationsNode =
                createMOSNode("Animations", momDoc.Document.MOS, momDoc);
            rootNode.AddNode(animationsNode);

            return rootNode;
        }

        private AnimatedMeshListTreeNode createMOSNode(string name, MOS mos, MOMDocument entity)
        {
            AnimatedMeshListTreeNode rootNode =
                new AnimatedMeshListTreeNode(name, entity.Models.ObjectMeshes, entity, 0,
                    _animationController);

            for (int i = 0; i < mos.NumberOfTODs; i++)
            {
                AnimatedMeshListTreeNode animationNode = new AnimatedMeshListTreeNode($"Animation {i}",
                    entity.Models.ObjectMeshes, entity,
                    i, _animationController);
                rootNode.AddNode(animationNode);
            }

            return rootNode;
        }

        private MeshListTreeNode createTMDNode(string name, TMDDocument tmdDoc)
        {
            MeshListTreeNode rootNode = new MeshListTreeNode(name, tmdDoc.ObjectMeshes);

            for (int i = 0; i < tmdDoc.Document.Header.NumObjects; i++)
            {
                MeshListTreeNode objNode =
                    new MeshListTreeNode($"Object {i}", new List<Mesh> {tmdDoc.ObjectMeshes[i]});
                rootNode.AddNode(objNode);
            }

            return rootNode;
        }
    }
}
