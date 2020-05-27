using System.IO;
using LSDView.Util;

namespace LSDView.Controllers
{
    public class FileOpenController
    {
        private readonly LBDController _lbdController;
        private readonly ConfigController _configController;

        public FileOpenController(LBDController lbdController, ConfigController configController)
        {
            _lbdController = lbdController;
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
                default:
                    Logger.Log()(LogLevel.WARN, $"Unable to open file {filePath}, unsupported type.");
                    break;
            }
        }
    }
}
