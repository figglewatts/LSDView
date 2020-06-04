using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace LSDView.util
{
    public static class ImageUtil
    {
        public static byte[] ImageFloatDataToByteData(float[] image)
        {
            byte[] byteData = new byte[image.Length];

            for (int i = 0; i < image.Length; i++)
            {
                byteData[i] = (byte)(image[i] * 255);
            }

            return byteData;
        }

        public static byte[] ImageDataToARGBFromRGBA(byte[] image)
        {
            byte[] argb = new byte[image.Length];

            for (int i = 0; i < image.Length; i += 4)
            {
                argb[i] = image[i + 2];
                argb[i + 1] = image[i + 1];
                argb[i + 2] = image[i];
                argb[i + 3] = image[i + 3];
            }

            return argb;
        }

        public static Bitmap ImageDataToBitmap(float[] image, int width, int height)
        {
            byte[] byteColors = ImageDataToARGBFromRGBA(ImageFloatDataToByteData(image));

            Bitmap bmp = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly,
                bmp.PixelFormat);
            Marshal.Copy(byteColors, 0, bmpData.Scan0, byteColors.Length);
            bmp.UnlockBits(bmpData);

            return bmp;
        }
    }
}
