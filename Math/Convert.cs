
namespace LSDView.Math
{
    public class Convert
    {
        public static byte ToByte(float a)
        { 
            return (byte)System.Math.Round(a * 255.0);
        }
    }
}
