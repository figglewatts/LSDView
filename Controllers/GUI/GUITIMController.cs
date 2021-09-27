using System.IO;
using libLSD.Formats;
using LSDView.Controllers.Interface;
using LSDView.Graphics;
using LSDView.Models;
using LSDView.Util;

namespace LSDView.Controllers.GUI
{
    public class GUITIMController : IFileFormatController<TIM, TIMDocument>
    {
        protected readonly ITreeController _treeController;
        protected readonly Shader _shader;

        public GUITIMController(ITreeController treeController)
        {
            _treeController = treeController;
            _shader = new Shader("texture", "Shaders/texture");
        }

        public TIM Load(string timPath)
        {
            var tim = LibLSDUtil.LoadTIM(timPath);

            TIMDocument document = CreateDocument(tim);
            _treeController.PopulateWithDocument(document, Path.GetFileName(timPath));

            return tim;
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
                ITexture2D tex = new Texture2D(img.width, img.height, img.data);
                textureMesh.Textures.Add(tex);
                timMeshes[i] = textureMesh;
            }

            return new TIMDocument(tim, timMeshes);
        }
    }
}
