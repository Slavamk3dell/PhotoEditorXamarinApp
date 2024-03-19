using System;
using System.Collections.Generic;
using System.Text;
using SkiaSharp;
using System.IO;
using Xamarin.Forms;
using Android.Graphics;

namespace TestXamarinApp
{
    public class SKBitmapConverter
    {
        public static SKBitmap CreateSKBitmapFromBytes(byte[] imageBytes)
        {
            try
            {
                using (SKCodec codec = SKCodec.Create(new MemoryStream(imageBytes)))
                {
                    SKImageInfo info = codec.Info;
                    SKBitmap bitmap = new SKBitmap(info.Width, info.Height);
                    SKCodecResult result = codec.GetPixels(bitmap.Info, bitmap.GetPixels());

                    if (result == SKCodecResult.Success)
                    {
                        return bitmap;
                    }
                    else
                    {
                        bitmap.Dispose();
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при создании SKBitmap: {ex.Message}");
                return null;
            }
        }

        public static SKBitmap ApplyGrayScaleFilter(SKBitmap originalBitmap)
        {
            SKBitmap resultBitmap = new SKBitmap(originalBitmap.Info);

            using (SKCanvas canvas = new SKCanvas(resultBitmap))
            {
                using (SKPaint paint = new SKPaint())
                {
                    paint.ColorFilter = SKColorFilter.CreateColorMatrix(new float[]
                    {
                0.21f, 0.72f, 0.07f, 0, 0,
                0.21f, 0.72f, 0.07f, 0, 0,
                0.21f, 0.72f, 0.07f, 0, 0,
                0,     0,     0,     1, 0
                    });

                    // Рисуем исходное изображение с применением цветового фильтра
                    canvas.DrawBitmap(originalBitmap, 0, 0, paint);
                }
            }

            return resultBitmap;
        }


        public static byte[] ConvertSKBitmapToBytes(SKBitmap bitmap, SKEncodedImageFormat format)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                bitmap.Encode(stream, format, 100);

                return stream.ToArray();
            }
        }
    }
}
