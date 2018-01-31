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

	        var image = LibLSDUtil.GetImageDataFromTIM(_tim);

            _textureMesh.Textures.Clear();

            Logger.Log()(LogLevel.INFO, "Loaded TIM: {0}", path);

            _textureMesh.Textures.Add(new Texture2D(image.data, image.width, image.height));

            TreeNode timNode = new RenderableMeshTreeNode(Path.GetFileName(TIMPath), _textureMesh);

            View.ViewOutline.BeginUpdate();
            View.ViewOutline.Nodes.Clear();
            View.ViewOutline.Nodes.Add(timNode);
            View.ViewOutline.EndUpdate();
            View.ViewOutline.SelectedNode = timNode;
        }

        
    }
}
