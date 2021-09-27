using System.IO;
using LSDView.Controllers.Interface;
using LSDView.Models;
using Newtonsoft.Json;
using Serilog;

namespace LSDView.Controllers
{
    public class ConfigController : IConfigController
    {
        public LSDViewConfig Config { get; protected set; }

        protected const string CONFIG_FILE = "LSDViewConfig.json";
        protected const int MAX_RECENT_FILES = 10;

        public ConfigController()
        {
            ensureConfigExists();
            deserializeConfig();
        }

        public void Save() { serializeConfig(); }

        public virtual void AddRecentFile(string recentFile)
        {
            if (Config.RecentFiles.Count + 1 > MAX_RECENT_FILES)
            {
                Config.RecentFiles.RemoveAt(MAX_RECENT_FILES - 1);
            }

            Config.RecentFiles.Insert(0, recentFile);
            Save();
        }

        protected void deserializeConfig()
        {
            Log.Information("Deserializing LSDViewConfig.json");
            Config = JsonConvert.DeserializeObject<LSDViewConfig>(File.ReadAllText(CONFIG_FILE));
        }

        protected void serializeConfig()
        {
            Log.Information("Serializing LSDViewConfig.json");
            File.WriteAllText(CONFIG_FILE, JsonConvert.SerializeObject(Config));
        }

        protected void ensureConfigExists()
        {
            if (!File.Exists(CONFIG_FILE))
            {
                Log.Information("LSDViewConfig.json not found - creating anew");
                Config = new LSDViewConfig();
                serializeConfig();
            }
        }
    }
}
