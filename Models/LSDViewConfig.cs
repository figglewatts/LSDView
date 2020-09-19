using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace LSDView.Models
{
    [JsonObject]
    public class LSDViewConfig
    {
        public string StreamingAssetsPath
        {
            get => _streamingAssetsPath;
            set
            {
                _streamingAssetsPath = value;
                OnStreamingAssetsPathChange?.Invoke();
            }
        }

        public List<string> RecentFiles = new List<string>();

        [JsonIgnore] public Action OnStreamingAssetsPathChange;

        private string _streamingAssetsPath = "";
    }
}
