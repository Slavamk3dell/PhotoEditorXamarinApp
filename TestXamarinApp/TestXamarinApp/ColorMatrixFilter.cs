using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace TestXamarinApp
{
    public class ColorMatrixFilter
    {
        public static void ApplyColorFilterToSKBitmap(SKBitmap bitmap, float[] filter)
        {
            using (var paint = new SKPaint())
            {
                paint.ColorFilter = SKColorFilter.CreateColorMatrix(filter);

                using (var canvas = new SKCanvas(bitmap))
                {
                    canvas.DrawBitmap(bitmap, 0, 0, paint: paint);
                }
            }
        }
    }
}
