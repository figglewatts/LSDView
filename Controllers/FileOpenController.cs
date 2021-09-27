using System.IO;
using libLSD.Formats;
using LSDView.Controllers.Interface;
using LSDView.Models;
using Serilog;

namespace LSDView.Controllers
{
    public class FileOpenController : IFileOpenController
    {
        protected readonly IFileFormatController<LBD, LBDDocument> _lbdController;
        protected readonly IFileFormatController<TMD, TMDDocument> _tmdController;
        protected readonly IFileFormatController<MOM, MOMDocument> _momController;
        protected readonly IFileFormatController<TIM, TIMDocument> _timController;
        protected readonly IFileFormatController<TIX, TIXDocument> _tixController;
        protected readonly IConfigController _configController;

        public FileOpenController(IFileFormatController<LBD, LBDDocument> lbdController,
            IFileFormatController<TMD, TMDDocument> tmdController,
            IFileFormatController<MOM, MOMDocument> momController,
            IFileFormatController<TIM, TIMDocument> timController,
            IFileFormatController<TIX, TIXDocument> tixController,
            IConfigController configController)
        {
            _lbdController = lbdController;
            _tmdController = tmdController;
            _momController = momController;
            _timController = timController;
            _tixController = tixController;
            _configController = configController;
        }

        public void OpenFile(string filePath)
        {
            _configController.AddRecentFile(filePath);
            Log.Information($"Loading file from: {filePath}");
            var ext = Path.GetExtension(filePath)?.ToLowerInvariant();
            switch (ext)
            {
                case ".lbd":
                    _lbdController.Load(filePath);
                    break;
                case ".tmd":
                    _tmdController.Load(filePath);
                    break;
                case ".mom":
                    _momController.Load(filePath);
                    break;
                case ".tim":
                    _timController.Load(filePath);
                    break;
                case ".tix":
                    _tixController.Load(filePath);
                    break;
                default:
                    Log.Error($"Unable to open file {filePath}, unsupported type.");
                    break;
            }
        }
    }
}
