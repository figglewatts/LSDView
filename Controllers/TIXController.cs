using System.Collections.Generic;
using System.IO;
using libLSD.Formats;
using LSDView.Models;

namespace LSDView.Controllers
{
    public class TIXController
    {
        private readonly TreeController _treeController;
        private readonly TIMController _timController;

        public TIXController(TreeController treeController, TIMController timController)
        {
            _treeController = treeController;
            _timController = timController;
        }

        public void LoadTIX(string tixPath)
        {
            TIX tix;
            using (BinaryReader br = new BinaryReader(File.Open(tixPath, FileMode.Open)))
            {
                tix = new TIX(br);
            }

            TIXDocument document = CreateDocument(tix);
            _treeController.PopulateTreeWithDocument(document, Path.GetFileName(tixPath));
        }

        public TIXDocument CreateDocument(TIX tix)
        {
            List<TIMDocument> tims = new List<TIMDocument>();
            foreach (var tim in tix.AllTIMs)
            {
                tims.Add(_timController.CreateDocument(tim));
            }

            return new TIXDocument(tix, tims);
        }
    }
}
