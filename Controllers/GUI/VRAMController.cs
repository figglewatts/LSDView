using System.IO;
using libLSD.Formats;
using LSDView.Controllers.Interface;
using LSDView.Graphics;
using LSDView.Util;
using OpenTK.Graphics;
using Serilog;

namespace LSDView.Controller
{
    public class VRAMController : IVRAMController
    {
        public ITexture2D VRAM { get => _vram; protected set => _vram = value; }
        public TIX Tix { get; protected set; }

        public bool VRAMLoaded { get; protected set; }

        private ITexture2D _vram;

        public VRAMController()
        {
            VRAM = Texture2D.Fill(Color4.White, LibLSDUtil.VRAM_WIDTH, LibLSDUtil.VRAM_HEIGHT);
            VRAMLoaded = false;
        }

        public void LoadTIXIntoVRAM(string tixPath)
        {
            Log.Information($"Loading TIX from {tixPath} into virtual VRAM");

            using (BinaryReader br = new BinaryReader(File.Open(tixPath, FileMode.Open)))
            {
                Tix = new TIX(br);
            }

            Log.Information("Successfully loaded TIX");

            LibLSDUtil.TIXToTexture2D(Tix, ref _vram, flip: true);

            VRAMLoaded = true;
        }

        public void ClearVRAM() { VRAM.Clear(); }
    }
}
