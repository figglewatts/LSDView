using System.IO;
using LSDView.Util;

namespace LSDView.Controllers
{
    public class FileOpenController
    {
        private readonly LBDController _lbdController;

        public FileOpenController(LBDController lbdController) { _lbdController = lbdController; }

        public void OpenFile(string filePath)
        {
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
