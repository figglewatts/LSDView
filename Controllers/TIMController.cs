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
            Logger.Log()(LogLevel.INFO, $"Loading TIM from: {timPath}");

            TIM tim;
            using (BinaryReader br = new BinaryReader(File.Open(timPath, FileMode.Open)))
            {
                tim = new TIM(br);
            }

            Logger.Log()(LogLevel.INFO, $"Successfully loaded TIM");

            TIMDocument document = CreateDocument(tim);
            _treeController.PopulateTreeWithDocument(document, Path.GetFileName(timPath));
        }

        public TIMDocument CreateDocument(TIM tim)
        {
            // 1 if not using CLUT, otherwise number of CLUTs
            int numMeshes = tim.ColorLookup?.NumberOfCLUTs ?? 1;
            Mesh[] timMeshes = new Mesh[numMeshes];

            for (int i = 0; i < numMeshes; i++)
            {
                var img = LibLSDUtil.GetImageDataFromTIM(tim, clutIndex: i);
                Mesh textureMesh = Mesh.CreateQuad(_shader);
                textureMesh.Textures.Add(new Texture2D(img.width, img.height, img.data));
                timMeshes[i] = textureMesh;
            }

            return new TIMDocument(tim, timMeshes);
        }
    }
}
