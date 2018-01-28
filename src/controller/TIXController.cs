using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using libLSD.Formats;
using libLSD.Types;
using LSDView.graphics;
using LSDView.view;

namespace LSDView.controller
{
    public class TIXController
    {
        public ILSDView View { get; set; }

        public string TIXPath { get; private set; }

        private TIX _tix;
        private TIMController _timController;
        private List<Mesh> _tixTextureMeshes;

        public TIXController(ILSDView view, TIMController tim)
        {
            View = view;
            _timController = tim;
            _tixTextureMeshes = new List<Mesh>();
        }

        public void LoadTIX(string path)
        {
            using (BinaryReader br = new BinaryReader(File.Open(path, FileMode.Open)))
            {
                _tix = new TIX(br);
            }

            TIXPath = path;

            foreach (Mesh mesh in _tixTextureMeshes)
            {
                mesh.Dispose();
            }
            _tixTextureMeshes.Clear();

            foreach (var chunk in _tix.Chunks)
            {
                foreach (var tim in chunk.TIMs)
                {
                    IColor[,] imageColors = tim.GetImage();
                    int width = imageColors.GetLength(1);
                    int height = imageColors.GetLength(0);
                    float[] imageData = _timController.ImageColorsToData(imageColors, width, height);

                    Texture2D timTex = new Texture2D(imageData, width, height);
                    Mesh textureMesh = View.CreateTextureQuad();
                    textureMesh.Textures.Add(timTex);

                    _tixTextureMeshes.Add(textureMesh);
                }
            }

            View.ViewOutline.BeginUpdate();
            View.ViewOutline.Nodes.Clear();

            TreeNode baseTreeNode = new TreeNode(Path.GetFileName(TIXPath));

            int timNumber = 0;
            foreach (Mesh mesh in _tixTextureMeshes)
            {
                RenderableMeshTreeNode subNode = new RenderableMeshTreeNode($"Texture {timNumber}", mesh);
                baseTreeNode.Nodes.Add(subNode);

                timNumber++;
            }

            View.ViewOutline.Nodes.Add(baseTreeNode);
            View.ViewOutline.EndUpdate();
            View.ViewOutline.SelectedNode = baseTreeNode.Nodes[0];
        }
    }
}
