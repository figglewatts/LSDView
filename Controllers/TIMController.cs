using System.IO;
using libLSD.Formats;
using LSDView.Graphics;
using LSDView.Models;
using LSDView.Util;

namespace LSDView.Controllers
{
    public class TIMController
    {
        private readonly TreeController _treeController;
        private readonly Shader _shader;

        public TIMController(TreeController treeController)
        {
            _treeController = treeController;
            _shader = new Shader("texture", "Shaders/texture");
        }

        public void LoadTIM(string timPath)
        {
            TIM tim;
            using (BinaryReader br = new BinaryReader(File.Open(timPath, FileMode.Open)))
            {
                tim = new TIM(br);
            }

            TIMDocument document = CreateDocument(tim);
            _treeController.PopulateTreeWithDocument(document, Path.GetFileName(timPath));
        }

        public TIMDocument CreateDocument(TIM tim)
        {
            Mesh textureMesh = Mesh.CreateQuad(_shader);
            var img = LibLSDUtil.GetImageDataFromTIM(tim);
            textureMesh.Textures.Add(new Texture2D(img.width, img.height, img.data));

            return new TIMDocument(tim, textureMesh);
        }
    }
}
