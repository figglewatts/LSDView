using System.Net;
using LSDView.GUI.Components;
using LSDView.Util;
using Newtonsoft.Json.Linq;

namespace LSDView.Controllers
{
    public class UpdateCheckerController
    {
        protected const string UPDATE_API_URL = "https://api.github.com/repos/figglewatts/lsdview/releases/latest";
        protected readonly bool _updateRequired = false;
        protected readonly Modal _updateAvailableModal;

        public UpdateCheckerController(Modal updateAvailableModal)
        {
            _updateRequired = isUpdateAvailable();
            _updateAvailableModal = updateAvailableModal;
            if (_updateRequired)
            {
                _updateAvailableModal.Show();
            }
        }

        private bool isUpdateAvailable()
        {
            // if we're developing locally, always return false
            if (Version.String.Equals("#{VERSION}#")) return false;

            using (WebClient wc = new WebClient())
            {
                wc.Headers.Add("User-Agent", "LSDView");
                try
                {
                    var json = wc.DownloadString(UPDATE_API_URL);
                    dynamic releaseInfo = JObject.Parse(json);
                    string releaseName = releaseInfo.name;
                    return !Version.String.Equals(releaseName);
                }
                catch (WebException exception)
                {
                    Logger.Log()(LogLevel.WARN, $"Unable to check for update: {exception}");
                    return false;
                }
            }
        }
    }
}
