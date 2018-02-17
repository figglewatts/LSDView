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
using LSDView.model;
using LSDView.util;
using LSDView.view;
using OpenTK;

namespace LSDView.controller
{
    public class TIMController
    {
        public ILSDView View { get; set; }
        public string TIMPath { get; private set; }
        public Mesh TextureMesh { get; private set; }

        private TIM _tim;
        private DocumentController _documentController;

        public TIMController(ILSDView view, DocumentController documentController)
        {
            View = view;

            _documentController = documentController;

            View.OnGLLoad += (sender, e) =>
            {
                TextureMesh = View.CreateTextureQuad();
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

            Logger.Log()(LogLevel.INFO, "Loaded TIM: {0}", path);

            TIMDocument document = new TIMDocument(_tim);
            document.OnLoad += (sender, args) => TextureMesh.Textures.Add(new Texture2D(image.data, image.width, image.height));
            document.OnUnload += (sender, args) => UnloadTIM();
            _documentController.LoadDocument(document, Path.GetFileName(TIMPath));

        }

        public void UnloadTIM()
        {
            TextureMesh.ClearTextures();
        }
    }
}
