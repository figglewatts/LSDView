using System.IO;
using libLSD.Formats;
using LSDView.Controllers.Interface;
using LSDView.Graphics;
using LSDView.Util;
using Serilog;

namespace LSDView.Controllers.Headless
{
    public class HeadlessVRAMController : IVRAMController
    {
        public ITexture2D VRAM { get; protected set; }
        public TIX Tix { get; protected set; }
        public bool VRAMLoaded { get; protected set; }

        public void LoadTIXIntoVRAM(string tixPath)
        {
            Log.Information($"Loading TIX from {tixPath} into virtual VRAM");

            using (BinaryReader br = new BinaryReader(File.Open(tixPath, FileMode.Open)))
            {
                Tix = new TIX(br);
            }

            Log.Information("Successfully loaded TIX");

            if (VRAM != null)
            {
                disposeOldVRAM();
            }

            VRAM = LibLSDUtil.TIXToTexture2D(Tix, headless: true, flip: false);

            VRAMLoaded = true;
        }

        public void ClearVRAM() { VRAM.Clear(); }

        protected void disposeOldVRAM()
        {
            VRAM.Dispose();
            VRAM = null;
        }
    }
}
