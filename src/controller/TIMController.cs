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
        private Texture2D _texture;

        private Shader _shader;

        private static Mesh _textureMesh;

        public TIMController(ILSDView view)
        {
            View = view;

            View.OnGLLoad += (sender, args) =>
            {
                _shader = new Shader("texture", "shaders/texture");

                Vector3[] vertPositions = new[]
                {
                    new Vector3(-1, -1, 0),
                    new Vector3(-1, 1, 0),
                    new Vector3(1, 1, 0),
                    new Vector3(1, -1, 0)
                };

                Vector2[] vertUVs = new[]
                {
                    new Vector2(0, 0),
                    new Vector2(0, 1),
                    new Vector2(1, 1),
                    new Vector2(1, 0)
                };

                _textureMesh = new Mesh(
                    new Vertex[]
                    {
                        new Vertex(
                            vertPositions[0], null, vertUVs[0]),
                        new Vertex(
                            vertPositions[1], null, vertUVs[1]),
                        new Vertex(
                            vertPositions[2], null, vertUVs[2]),
                        new Vertex(
                            vertPositions[3], null, vertUVs[3])
                    },
                    new int[] { 1, 0, 2, 2, 0, 3 },
                    _shader
                );
            };
        }

        public void LoadTIM(string path)
        {
            TIMPath = path;
            using (BinaryReader br = new BinaryReader(File.Open(path, FileMode.Open)))
            {
                _tim = new TIM(br);
            }

            Console.WriteLine($"TIM Type: {_tim.Header.PixelMode.ToString()}");
            Console.WriteLine($"TIM dim, W: {_tim.PixelData.Width}, H: {_tim.PixelData.Height}");

            IColor[,] imageColors = _tim.GetImage();
            int width = imageColors.GetLength(1);
            int height = imageColors.GetLength(0);
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

            Console.WriteLine(imageData[3584*4]);

            _textureMesh.Textures.Clear();
            _texture?.Dispose();

            Logger.Log()(LogLevel.INFO, "Loaded TIM: {0}", path);

            _texture = new Texture2D(imageData, width, height);
            _textureMesh.Textures.Add(_texture);

            TreeNode timNode = new TMDObjectTreeNode(Path.GetFileName(TIMPath), _textureMesh);

            View.ViewOutline.BeginUpdate();
            View.ViewOutline.Nodes.Clear();
            View.ViewOutline.Nodes.Add(timNode);
            View.ViewOutline.EndUpdate();
            View.ViewOutline.SelectedNode = timNode;
        }
    }
}
