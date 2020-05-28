using System.Collections.Generic;
using System.IO;
using libLSD.Formats;
using LSDView.controller;
using LSDView.Graphics;
using LSDView.Models;
using LSDView.Util;

namespace LSDView.Controllers
{
    public class TMDController
    {
        private readonly TreeController _treeController;
        private readonly VRAMController _vramController;
        private readonly Shader _shader;

        public TMDController(TreeController treeController, VRAMController vramController)
        {
            _treeController = treeController;
            _vramController = vramController;
            _shader = new Shader("basic", "Shaders/basic");
        }

        public void LoadTMD(string tmdPath)
        {
            TMD tmd;
            using (BinaryReader br = new BinaryReader(File.Open(tmdPath, FileMode.Open)))
            {
                tmd = new TMD(br);
            }

            TMDDocument document = CreateDocument(tmd);
            _treeController.PopulateTreeWithDocument(document, Path.GetFileName(tmdPath));
        }

        public TMDDocument CreateDocument(TMD tmd)
        {
            List<Mesh> objectMeshes = LibLSDUtil.CreateMeshesFromTMD(tmd, _shader, _vramController.VRAMTexture);
            return new TMDDocument(tmd, objectMeshes);
        }
    }
}
