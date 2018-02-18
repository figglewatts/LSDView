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

namespace LSDView.controller
{
    public class TIXController
    {
        public ILSDView View { get; set; }

        public string TIXPath { get; private set; }

        public List<Mesh> TIXTextureMeshes { get; private set; }

        private TIX _tix;
        private TIMController _timController;
        private DocumentController _documentController;

        public TIXController(ILSDView view, TIMController tim, DocumentController documentController)
        {
            View = view;
            _timController = tim;
            _documentController = documentController;
            TIXTextureMeshes = new List<Mesh>();
        }

        public void LoadTIX(string path)
        {
            using (BinaryReader br = new BinaryReader(File.Open(path, FileMode.Open)))
            {
                _tix = new TIX(br);
            }

            TIXPath = path;

	        Logger.Log()(LogLevel.INFO, "Loaded TIX: {0}", path);

            TIXDocument document = new TIXDocument(_tix);
            document.OnLoad += (sender, args) => CreateMeshes();
            document.OnUnload += (sender, args) => UnloadTIX();
            _documentController.LoadDocument(document, Path.GetFileName(TIXPath));
        }

        public void CreateMeshes()
        {
            foreach (var chunk in _tix.Chunks)
            {
                foreach (var tim in chunk.TIMs)
                {
                    var image = LibLSDUtil.GetImageDataFromTIM(tim);

                    Texture2D timTex = new Texture2D(image.data, image.width, image.height);
                    Mesh textureMesh = View.CreateTextureQuad();
                    textureMesh.Textures.Add(timTex);

                    TIXTextureMeshes.Add(textureMesh);
                }
            }
        }

        public void UnloadTIX()
        {
            foreach (Mesh mesh in TIXTextureMeshes)
            {
                mesh.Dispose();
            }
            TIXTextureMeshes.Clear();
        }

        public void WriteTIX(string filename, TIX? tix = null)
        {
            using (BinaryWriter bw = new BinaryWriter(File.Open(filename, FileMode.Create)))
            {
                if (tix == null)
                {
                    _tix.Write(bw);
                }
                else
                {
                    tix.Value.Write(bw);
                }
            }
        }

        public void WriteTIXTIMs(string baseFilename, TIX? tix = null)
        {
            int i = 0;
            string path = Path.GetDirectoryName(baseFilename);
            string baseFileWithoutExt = Path.GetFileNameWithoutExtension(baseFilename);

            TIX exportTix = tix ?? _tix;

            foreach (TIM tim in exportTix.AllTIMs)
            {
                string filename = Path.Combine(path, baseFileWithoutExt + $"_TIM_{i}.tim");
                using (BinaryWriter bw =
                    new BinaryWriter(File.Open(filename, FileMode.Create))) 
                {
                    tim.Write(bw);
                }

                i++;
            }
        }
    }
}
