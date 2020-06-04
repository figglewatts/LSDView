using System.IO;
using LSDView.Util;

namespace LSDView.Controllers
{
    public class FileOpenController
    {
        private readonly LBDController _lbdController;
        private readonly TMDController _tmdController;
        private readonly MOMController _momController;
        private readonly TIMController _timController;
        private readonly TIXController _tixController;
        private readonly ConfigController _configController;

        public FileOpenController(LBDController lbdController,
            TMDController tmdController,
            MOMController momController,
            TIMController timController,
            TIXController tixController,
            ConfigController configController)
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
            var ext = Path.GetExtension(filePath)?.ToLowerInvariant();
            switch (ext)
            {
                case ".lbd":
                    _lbdController.LoadLBD(filePath);
                    break;
                case ".tmd":
                    _tmdController.LoadTMD(filePath);
                    break;
                case ".mom":
                    _momController.LoadMOM(filePath);
                    break;
                case ".tim":
                    _timController.LoadTIM(filePath);
                    break;
                case ".tix":
                    _tixController.LoadTIX(filePath);
                    break;
                default:
                    Logger.Log()(LogLevel.WARN, $"Unable to open file {filePath}, unsupported type.");
                    break;
            }
        }
    }
}
