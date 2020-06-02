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
        private readonly ExportController _exportController;

        public TreeController(AnimationController animationController, ExportController exportController)
        {
            _animationController = animationController;
            _exportController = exportController;
        }

        public void PopulateTreeWithDocument(IDocument doc, string rootName)
        {
            Tree.Deselect();
            Tree.Nodes.Clear();
            MeshListTreeNode node;
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
            MeshListTreeNode rootNode = new MeshListTreeNode(name, new List<Mesh> {tixDoc.TIMs[0].TextureMesh},
                contextMenu: new ContextMenu(
                    new Dictionary<string, Action>
                    {
                        {
                            "Export as TIX",
                            () =>
                            {
                                _exportController.OpenDialog(
                                    filePath => { _exportController.ExportOriginal(tixDoc.Document, filePath); },
                                    ".tix");
                            }
                        }
                    }));

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
                    {
                        "Export as TIM",
                        () =>
                        {
                            _exportController.OpenDialog(
                                filePath => { _exportController.ExportOriginal(timDoc.Document, filePath); }, ".tim");
                        }
                    }
                }));
        }

        private MeshListTreeNode createLBDNode(string name, LBDDocument lbdDoc)
        {
            MeshListTreeNode rootNode = new MeshListTreeNode(name, lbdDoc.TileLayout, contextMenu: new ContextMenu(
                new Dictionary<string, Action>
                {
                    {
                        "Export as LBD",
                        () =>
                        {
                            _exportController.OpenDialog(
                                filePath => { _exportController.ExportOriginal(lbdDoc.Document, filePath); }, ".lbd");
                        }
                    }
                }));

            TreeNode tilesNode = createTMDNode("Tiles", lbdDoc.TilesTMD);
            rootNode.AddNode(tilesNode);

            if (lbdDoc.Entities != null)
            {
                TreeNode objectsNode = createMMLNode("Entities", lbdDoc.Entities, lbdDoc.Document.MML.Value);
                rootNode.AddNode(objectsNode);
            }

            return rootNode;
        }

        private TreeNode createMMLNode(string name, List<MOMDocument> entities, MML mml)
        {
            MeshListTreeNode rootNode = new MeshListTreeNode(name, entities[0].Models.ObjectMeshes,
                contextMenu: new ContextMenu(
                    new Dictionary<string, Action>
                    {
                        {
                            "Export as MML",
                            () =>
                            {
                                _exportController.OpenDialog(
                                    filePath => { _exportController.ExportOriginal(mml, filePath); },
                                    ".mml");
                            }
                        }
                    }));

            for (int i = 0; i < entities.Count; i++)
            {
                MeshListTreeNode momNode = createMOMNode($"Entity {i}", entities[i]);
                rootNode.AddNode(momNode);
            }

            return rootNode;
        }

        private MeshListTreeNode createMOMNode(string name, MOMDocument momDoc)
        {
            MeshListTreeNode rootNode = new MeshListTreeNode(name, momDoc.Models.ObjectMeshes,
                contextMenu: new ContextMenu(
                    new Dictionary<string, Action>
                    {
                        {
                            "Export as MOM",
                            () =>
                            {
                                _exportController.OpenDialog(
                                    filePath => { _exportController.ExportOriginal(momDoc.Document, filePath); },
                                    ".mom");
                            }
                        }
                    }));

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
                    _animationController, contextMenu: new ContextMenu(
                        new Dictionary<string, Action>
                        {
                            {
                                "Export as MOS",
                                () =>
                                {
                                    _exportController.OpenDialog(
                                        filePath => { _exportController.ExportOriginal(mos, filePath); }, ".mos");
                                }
                            }
                        }));

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
            MeshListTreeNode rootNode = new MeshListTreeNode(name, tmdDoc.ObjectMeshes, contextMenu: new ContextMenu(
                new Dictionary<string, Action>
                {
                    {
                        "Export as TMD",
                        () =>
                        {
                            _exportController.OpenDialog(
                                filePath => { _exportController.ExportOriginal(tmdDoc.Document, filePath); }, ".tmd");
                        }
                    }
                }));

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
