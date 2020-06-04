using System.Collections.Generic;
using Newtonsoft.Json;

namespace LSDView.Models
{
    [JsonObject]
    public class LSDViewConfig
    {
        public string StreamingAssetsPath = "";
        public List<string> RecentFiles = new List<string>();
    }
}
