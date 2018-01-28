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
using LSDView.util;
using LSDView.view;
using OpenTK;

namespace LSDView.controller
{
    public class TIMController
    {
        public ILSDView View { get; set; }
        public string TIMPath { get; private set; }

        private TIM _tim;
        private Mesh _textureMesh;

        public TIMController(ILSDView view)
        {
            View = view;

            View.OnGLLoad += (sender, e) =>
            {
                _textureMesh = View.CreateTextureQuad();
            };
        }

        public void LoadTIM(string path)
        {
            TIMPath = path;
            using (BinaryReader br = new BinaryReader(File.Open(path, FileMode.Open)))
            {
                _tim = new TIM(br);
            }

            IColor[,] imageColors = _tim.GetImage();
            int width = imageColors.GetLength(1);
            int height = imageColors.GetLength(0);
            float[] imageData = ImageColorsToData(imageColors, width, height);

            _textureMesh.Textures.Clear();

            Logger.Log()(LogLevel.INFO, "Loaded TIM: {0}", path);

            _textureMesh.Textures.Add(new Texture2D(imageData, width, height));

            TreeNode timNode = new RenderableMeshTreeNode(Path.GetFileName(TIMPath), _textureMesh);

            View.ViewOutline.BeginUpdate();
            View.ViewOutline.Nodes.Clear();
            View.ViewOutline.Nodes.Add(timNode);
            View.ViewOutline.EndUpdate();
            View.ViewOutline.SelectedNode = timNode;
        }

        public float[] ImageColorsToData(IColor[,] imageColors, int width, int height)
        {
            float[] imageData = new float[imageColors.Length * 4];

            int i = 0;
            for (int y = height - 1; y >= 0; y--)
            {
                for (int x = 0; x < width; x++)
                {
                    IColor col = imageColors[y, x];
                    imageData[i] = col.Red;
                    imageData[i + 1] = col.Green;
                    imageData[i + 2] = col.Blue;
                    imageData[i + 3] = col.Alpha;
                    i += 4;
                }
            }

            return imageData;
        }
    }
}
