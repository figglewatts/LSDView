using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using libLSD.Formats;
using LSDView.anim;
using LSDView.graphics;
using LSDView.model;
using LSDView.util;
using LSDView.view;

namespace LSDView.controller
{
    public class MOMController
    {
        public ILSDView View { get; set; }
        public MOMData MomData { get; private set; }
        public string MomPath { get; private set; }

        private VRAMController _vramController;
        private MOM _mom;
        private Shader _shader;
        private DocumentController _documentController;

        public MOMController(ILSDView view, VRAMController vram, DocumentController documentController)
        {
            View = view;
            _vramController = vram;
            _documentController = documentController;

            View.OnGLLoad += (sender, args) =>
            {
                _shader = new Shader("basic", "shaders/basic");
            };
        }

        public void LoadMOM(string path)
        {
            using (BinaryReader br = new BinaryReader(File.Open(path, FileMode.Open)))
            {
                _mom = new MOM(br);
            }

            MomPath = path;

            MOMDocument document = new MOMDocument(MomData);
            document.OnLoad = (sender, args) => CreateMeshes();
            document.OnUnload = (sender, args) => UnloadMOM();
            _documentController.LoadDocument(document, Path.GetFileName(MomPath));
        }

        public void CreateMeshes()
        {
            List<Mesh> momTmd = LibLSDUtil.CreateMeshesFromTMD(_mom.TMD, _shader, _vramController.VRAMTexture);
            List<TODAnimation> momAnimations = new List<TODAnimation>();
            foreach (var anim in _mom.MOS.TODs)
            {
                List<Mesh> animatedMeshes = LibLSDUtil.CreateMeshesFromTMD(_mom.TMD, _shader, _vramController.VRAMTexture);
                TODAnimation animationObj = new TODAnimation(animatedMeshes, anim);
                momAnimations.Add(animationObj);
            }
            MomData = new MOMData(_mom, momTmd, momAnimations);
        }

        public void UnloadMOM()
        {
            if (MomData != null)
            {
                MomData.Dispose();
                MomData = null;
            }
        }
    }
}
