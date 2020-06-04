using System.IO;
using LSDView.Models;
using LSDView.Util;
using Newtonsoft.Json;

namespace LSDView.Controllers
{
    public class ConfigController
    {
        public LSDViewConfig Config;

        private const string CONFIG_FILE = "LSDViewConfig.json";
        private const int MAX_RECENT_FILES = 10;

        public ConfigController()
        {
            ensureConfigExists();
            deserializeConfig();
        }

        public void Save() { serializeConfig(); }

        public void AddRecentFile(string recentFile)
        {
            if (Config.RecentFiles.Count + 1 > MAX_RECENT_FILES)
            {
                Config.RecentFiles.RemoveAt(MAX_RECENT_FILES - 1);
            }

            Config.RecentFiles.Insert(0, recentFile);
            Save();
        }

        private void deserializeConfig()
        {
            Logger.Log()(LogLevel.INFO, "Deserializing LSDViewConfig.json");
            Config = JsonConvert.DeserializeObject<LSDViewConfig>(File.ReadAllText(CONFIG_FILE));
        }

        private void serializeConfig()
        {
            Logger.Log()(LogLevel.INFO, "Serializing LSDViewConfig.json");
            File.WriteAllText(CONFIG_FILE, JsonConvert.SerializeObject(Config));
        }

        private void ensureConfigExists()
        {
            if (!File.Exists(CONFIG_FILE))
            {
                Logger.Log()(LogLevel.INFO, "LSDViewConfig.json not found - creating anew");
                Config = new LSDViewConfig();
                serializeConfig();
            }
        }
    }
}
