using System;



// Yes i really did make a new file for a single function, i want my code to look clean.

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
