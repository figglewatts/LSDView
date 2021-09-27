using libLSD.Formats;
using LSDView.Controllers.Interface;
using LSDView.Graphics;
using LSDView.Graphics.Headless;
using LSDView.Models;
using LSDView.Util;

namespace LSDView.Controllers.Headless
{
    public class HeadlessTIMController : IFileFormatController<TIM, TIMDocument>
    {
        public TIM Load(string path) { return LibLSDUtil.LoadTIM(path); }

        public TIMDocument CreateDocument(TIM tim)
        {
            // 1 if not using CLUT, otherwise number of CLUTs
            int numMeshes = tim.ColorLookup?.NumberOfCLUTs ?? 1;
            IRenderable[] timMeshes = new IRenderable[numMeshes];

            for (int i = 0; i < numMeshes; i++)
            {
                var img = LibLSDUtil.GetImageDataFromTIM(tim, clutIndex: i);
                HeadlessMesh textureMesh = HeadlessMesh.CreateQuad();
                ITexture2D tex = new HeadlessTexture2D(img.width, img.height, img.data);
                textureMesh.Textures.Add(tex);
                timMeshes[i] = textureMesh;
            }

            return new TIMDocument(tim, timMeshes);
        }
    }
}
