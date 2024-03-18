using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace TestXamarinApp
{
    public class ColorScaleFilter
    {
        public static double[,] ToGrayscale(Pixel[,] original)
        {
            var xColumn = original.GetLength(0);
            var yColumn = original.GetLength(1);
            var grayScale = new double[xColumn, yColumn];
            for (int x = 0; x < xColumn; x++)
            {
                for (int y = 0; y < yColumn; y++)
                {
                    grayScale[x, y] = (0.299 * original[x, y].R
                                       + 0.587 * original[x, y].G
                                       + 0.114 * original[x, y].B)
                                       / 255;
                }
            }

            return grayScale;
        }

        public static Pixel[,] ToBrightPinkPixelScale(Pixel[,] original)
        {
            var xColumn = original.GetLength(0);
            var yColumn = original.GetLength(1);
            var brightPinkScale = new Pixel[xColumn, yColumn];

            for (int x = 0; x < xColumn; x++)
            {
                for (int y = 0; y < yColumn; y++)
                {
                    byte intensity = (byte)(0.299 * original[x, y].R
                                          + 0.587 * original[x, y].G
                                          + 0.114 * original[x, y].B);

                    byte red = intensity;
                    byte green = (byte)(0.5 * intensity);
                    byte blue = (byte)(0.8 * intensity);

                    brightPinkScale[x, y] = new Pixel(red, green, blue);
                }
            }
            return brightPinkScale;
        }
    }
}
