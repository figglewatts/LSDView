using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace LSDView.Models
{
    [JsonObject]
    public class LSDViewConfig
    {
        public string GameDataPath
        {
            get => _gameDataPath;
            set
            {
                _gameDataPath = value;
                OnGameDataPathChange?.Invoke();
            }
        }

        public List<string> RecentFiles = new List<string>();

        [JsonIgnore] public Action OnGameDataPathChange;

        private string _gameDataPath = "";
    }
}
