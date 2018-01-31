using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using libLSD.Formats;
using libLSD.Types;
using LSDView.graphics;
using LSDView.util;
using LSDView.view;

namespace LSDView.controller
{
    public class VRAMController
    {
        public ILSDView View { get; set; }

        public Texture2D VRAMTexture { get; private set; }

        public const int VRAM_WIDTH = 2056;
        public const int VRAM_HEIGHT = 512;

        private readonly TIMController _timController;

        public VRAMController(ILSDView view, TIMController timController)
        {
            View = view;

            _timController = timController;

            View.OnGLLoad += (sender, args) =>
            {
                VRAMTexture = new Texture2D(VRAM_WIDTH, VRAM_HEIGHT);
                VRAMTexture.Clear();
            };
        }

        public void LoadTIXIntoVRAM(string tixPath)
        {
            TIX tix;
            using (BinaryReader br = new BinaryReader(File.Open(tixPath, FileMode.Open)))
            {
                tix = new TIX(br);
            }

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

        public void ClearVRAM()
        {
            VRAMTexture.Clear();
        }
    }
}
