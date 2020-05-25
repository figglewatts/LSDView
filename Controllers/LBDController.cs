using System.IO;
using libLSD.Formats;
using LSDView.Models;

namespace LSDView.Controllers
{
    public class LBDController
    {
        private readonly TreeController _treeController;

        public LBDController(TreeController treeController) { _treeController = treeController; }

        public void LoadLBD(string lbdPath)
        {
            LBD lbd;
            using (BinaryReader br = new BinaryReader(File.Open(lbdPath, FileMode.Open)))
            {
                lbd = new LBD(br);
            }

            LBDDocument document = new LBDDocument(lbd);
            _treeController.PopulateTreeWithDocument(document, Path.GetFileName(lbdPath));
        }
    }
}
