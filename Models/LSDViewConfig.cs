using System.Collections.Generic;
using Newtonsoft.Json;

namespace LSDView.Models
{
    [JsonObject]
    public class LSDViewConfig
    {
        public string StreamingAssetsPath = @"D:\Documents\git-repos\LSDRevamped\LSDR\Assets\StreamingAssets";
        public List<string> RecentFiles = new List<string>();
    }
}
