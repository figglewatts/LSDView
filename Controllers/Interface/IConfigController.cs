using LSDView.Models;

namespace LSDView.Controllers.Interface
{
    public interface IConfigController
    {
        LSDViewConfig Config { get; }

        void Save();
        void AddRecentFile(string recentFile);
    }
}
