using System.IO;
using libLSD.Formats;
using LSDView.controller;
using LSDView.Graphics;
using LSDView.Models;

namespace LSDView.Controllers
{
    public class MOMController
    {
        private readonly TreeController _treeController;
        private readonly VRAMController _vramController;
        private readonly TMDController _tmdController;
        private readonly Shader _shader;

        public MOMController(TreeController treeController, VRAMController vramController, TMDController tmdController)
        {
            _treeController = treeController;
            _vramController = vramController;
            _tmdController = tmdController;
            _shader = new Shader("basic", "Shaders/basic");
        }

        public void LoadMOM(string momPath)
        {
            MOM mom;
            using (BinaryReader br = new BinaryReader(File.Open(momPath, FileMode.Open)))
            {
                mom = new MOM(br);
            }

            MOMDocument document = CreateDocument(mom);
            _treeController.PopulateTreeWithDocument(document, Path.GetFileName(momPath));
        }

        public MOMDocument CreateDocument(MOM mom)
        {
            TMDDocument models = _tmdController.CreateDocument(mom.TMD);

            return new MOMDocument(mom, models);
        }
    }
}
