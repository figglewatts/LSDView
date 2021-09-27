using System.IO;
using libLSD.Formats;
using LSDView.Controllers.Interface;
using LSDView.Models;
using LSDView.Util;

namespace LSDView.Controllers.GUI
{
    public class GUIMOMController : IFileFormatController<MOM, MOMDocument>
    {
        protected readonly ITreeController _treeController;
        protected readonly IFileFormatController<TMD, TMDDocument> _tmdController;

        public GUIMOMController(ITreeController treeController,
            IFileFormatController<TMD, TMDDocument> tmdController)
        {
            _treeController = treeController;
            _tmdController = tmdController;
        }

        public MOM Load(string momPath)
        {
            var mom = LibLSDUtil.LoadMOM(momPath);

            MOMDocument document = CreateDocument(mom);
            _treeController.PopulateWithDocument(document, Path.GetFileName(momPath));

            return mom;
        }

        public MOMDocument CreateDocument(MOM mom)
        {
            TMDDocument models = _tmdController.CreateDocument(mom.TMD);

            return new MOMDocument(mom, models);
        }
    }
}
