using System.IO;
using LSDView.Util;

namespace LSDView.Controllers
{
    public class FileOpenController
    {
        private readonly LBDController _lbdController;
        private readonly TMDController _tmdController;
        private readonly MOMController _momController;
        private readonly ConfigController _configController;

        public FileOpenController(LBDController lbdController,
            TMDController tmdController,
            MOMController momController,
            ConfigController configController)
        {
            _lbdController = lbdController;
            _tmdController = tmdController;
            _momController = momController;
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
                default:
                    Logger.Log()(LogLevel.WARN, $"Unable to open file {filePath}, unsupported type.");
                    break;
            }
        }
    }
}
