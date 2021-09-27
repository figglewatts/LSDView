using System.Collections.Generic;
using System.IO;
using libLSD.Formats;
using LSDView.Controllers.Interface;
using LSDView.Graphics;
using LSDView.Models;
using LSDView.Util;

namespace LSDView.Controllers.GUI
{
    public class GUITMDController : IFileFormatController<TMD, TMDDocument>
    {
        protected readonly ITreeController _treeController;
        protected readonly IVRAMController _vramController;
        protected readonly Shader _shader;

        public GUITMDController(ITreeController treeController, IVRAMController vramController)
        {
            _treeController = treeController;
            _vramController = vramController;
            _shader = new Shader("basic", "Shaders/basic");
        }

        public TMD Load(string tmdPath)
        {
            var tmd = LibLSDUtil.LoadTMD(tmdPath);

            TMDDocument document = CreateDocument(tmd);
            _treeController.PopulateWithDocument(document, Path.GetFileName(tmdPath));

            return tmd;
        }

        public TMDDocument CreateDocument(TMD tmd)
        {
            List<IRenderable> objectMeshes =
                LibLSDUtil.CreateMeshesFromTMD(tmd, _shader, _vramController.VRAM, headless: false);
            return new TMDDocument(tmd, objectMeshes);
        }
    }
}
