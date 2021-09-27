using libLSD.Formats;
using LSDView.Graphics;

namespace LSDView.Controllers.Interface
{
    public interface IVRAMController
    {
        ITexture2D VRAM { get; }
        TIX Tix { get; }
        bool VRAMLoaded { get; }

        void LoadTIXIntoVRAM(string tixPath);
        void ClearVRAM();
    }
}
