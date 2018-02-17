using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using libLSD.Exceptions;
using libLSD.Formats;
using libLSD.Types;
using LSDView.graphics;
using LSDView.model;
using LSDView.util;
using LSDView.view;
using OpenTK;

namespace LSDView.controller
{
    public class TMDController
    {
        public string TMDPath { get; private set; }
        public List<Mesh> Meshes;
        public ILSDView View { get; set; }

        private Shader _shader;
        private TMD _tmd;
        private VRAMController _vramController;
        private DocumentController _documentController;

        public TMDController(ILSDView view, VRAMController vramController, DocumentController documentController)
        {
            Meshes = new List<Mesh>();

            _vramController = vramController;
            _documentController = documentController;

            View = view;

            View.OnGLLoad += (sender, args) =>
            {
                _shader = new Shader("basic", "shaders/basic");
            };
        }

        public void LoadTMD(string path)
        {
            using (BinaryReader br = new BinaryReader(File.Open(path, FileMode.Open)))
            {
                _tmd = new TMD(br);
                // TODO: check if TMD loaded correctly
            }

            UnloadTMD();

            TMDPath = path;

            Logger.Log()(LogLevel.INFO, "Loaded TMD: {0}", path);

            Meshes = LibLSDUtil.CreateMeshesFromTMD(_tmd, _shader, _vramController.VRAMTexture);

            TMDDocument document = new TMDDocument(_tmd);
            _documentController.LoadDocument(document);

        }

        public void WriteTMD(string path)
        {
            
        }

        private void UnloadTMD()
        {
            foreach (Mesh m in Meshes)
            {
                m.Dispose();
            }
            Meshes.Clear();
        }
    }
}
