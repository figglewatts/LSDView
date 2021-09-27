using System.Net;
using LSDView.Controllers.Interface;
using Newtonsoft.Json.Linq;
using Serilog;

namespace LSDView.Controllers
{
    public class UpdateCheckerController : IUpdateCheckerController
    {
        protected const string UPDATE_API_URL = "https://api.github.com/repos/figglewatts/lsdview/releases/latest";

        public bool IsUpdateAvailable()
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
                    Log.Warning($"Unable to check for update: {exception}");
                    return false;
                }
            }
        }
    }
}
