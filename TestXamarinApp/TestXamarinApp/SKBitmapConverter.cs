using System;
using System.Collections.Generic;
using System.Text;
using SkiaSharp;
using System.IO;
using Xamarin.Forms;
using SkiaSharp.Views.Forms;

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

        public static byte[] GetImageBytesFromSKBitmap(SKBitmap bitmap, SKEncodedImageFormat format, int quality = 100)
        {
            using (SKImage image = SKImage.FromBitmap(bitmap))
            using (var data = image.Encode(format, quality))
            {
                return data.ToArray();
            }
        }

        public static double GetImageSizeInMB(byte[] imageBytes)
        {
            long sizeInBytes = imageBytes.Length;
            double sizeInMB = (double)sizeInBytes / (1024 * 1024);
            return sizeInMB;
        }
    }
}
