using System.Collections.Generic;
using System.IO;
using libLSD.Formats;
using LSDView.Controllers.Interface;
using LSDView.Models;
using LSDView.Util;

namespace LSDView.Controllers.GUI
{
    public class GUITIXController : IFileFormatController<TIX, TIXDocument>
    {
        protected readonly ITreeController _treeController;
        protected readonly IFileFormatController<TIM, TIMDocument> _timController;

        public GUITIXController(ITreeController treeController, IFileFormatController<TIM, TIMDocument> timController)
        {
            _treeController = treeController;
            _timController = timController;
        }

        public TIX Load(string tixPath)
        {
            var tix = LibLSDUtil.LoadTIX(tixPath);

            TIXDocument document = CreateDocument(tix);
            _treeController.PopulateWithDocument(document, Path.GetFileName(tixPath));

            return tix;
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
