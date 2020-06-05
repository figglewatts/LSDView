using System.IO;
using libLSD.Formats;
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

        public VRAMController() { VRAMTexture = Texture2D.Fill(Color4.White, VRAM_WIDTH, VRAM_HEIGHT); }

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
        }

        public void ClearVRAM() { VRAMTexture.Clear(); }
    }
}
