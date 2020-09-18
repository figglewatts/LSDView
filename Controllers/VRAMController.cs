using System.Drawing.Imaging;
using System.IO;
using libLSD.Formats;
using LSDView.Controllers;
using LSDView.Graphics;
using LSDView.Util;
using OpenTK.Graphics;

namespace LSDView.controller
{
    public class VRAMController
    {
        public Texture2D VRAMTexture { get; private set; }

        public const int VRAM_WIDTH = 2056;
        public const int VRAM_HEIGHT = 512;

        public bool VRAMLoaded { get; private set; }

        private ExportController _exportController;

        public VRAMController(ExportController exportController)
        {
            VRAMTexture = Texture2D.Fill(Color4.White, VRAM_WIDTH, VRAM_HEIGHT);
            _exportController = exportController;
            VRAMLoaded = false;
        }

        public void LoadTIXIntoVRAM(string tixPath)
        {
            Logger.Log()(LogLevel.INFO, $"Loading TIX from {tixPath} into virtual VRAM");

            TIX tix;
            using (BinaryReader br = new BinaryReader(File.Open(tixPath, FileMode.Open)))
            {
                tix = new TIX(br);
            }

            Logger.Log()(LogLevel.INFO, $"Successfully loaded TIX");

            foreach (var chunk in tix.Chunks)
            {
                foreach (var tim in chunk.TIMs)
                {
                    var image = LibLSDUtil.GetImageDataFromTIM(tim);

                    int actualXPos = (tim.PixelData.XPosition - 320) * 2;
                    int actualYPos = 512 - tim.PixelData.YPosition - image.height;

                    VRAMTexture.SubImage(image.data, actualXPos, actualYPos, image.width, image.height);
                }
            }

            VRAMLoaded = true;
        }

        public void ExportLoadedVRAM()
        {
            if (!VRAMLoaded)
            {
                Logger.Log()(LogLevel.WARN, "Unable to export VRAM, not currently loaded");
                return;
            }

            _exportController.OpenDialog(
                (filePath => { _exportController.ExportVRAM(VRAMTexture, filePath, ImageFormat.Png); }), ".png");
        }

        public void ClearVRAM() { VRAMTexture.Clear(); }
    }
}
