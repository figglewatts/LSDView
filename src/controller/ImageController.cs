using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using libLSD.Formats;
using LSDView.util;
using LSDView.view;

namespace LSDView.controller
{
    public class ImageController
    {
        public void WriteTIMAsImage(string path, TIM tim, ImageFormat format)
        {
            var image = LibLSDUtil.GetImageDataFromTIM(tim, flip:false);

            Bitmap bmp = ImageUtil.ImageDataToBitmap(image.data, image.width, image.height);

            bmp.Save(path, format);
        }
    }
}
