using libLSD.Formats;
using LSDView.Controllers.Interface;
using LSDView.Models;
using LSDView.Util;

namespace LSDView.Controllers.Headless
{
    public class HeadlessMOMController : IFileFormatController<MOM, MOMDocument>
    {
        protected readonly IFileFormatController<TMD, TMDDocument> _tmdController;

        public HeadlessMOMController(IFileFormatController<TMD, TMDDocument> tmdController)
        {
            _tmdController = tmdController;
        }

        public MOM Load(string path) { return LibLSDUtil.LoadMOM(path); }

        public MOMDocument CreateDocument(MOM mom)
        {
            TMDDocument models = _tmdController.CreateDocument(mom.TMD);

            return new MOMDocument(mom, models);
        }
    }
}
