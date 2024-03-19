using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using Android.Graphics;
using Color = Android.Graphics.Color;
using SkiaSharp;

namespace TestXamarinApp
{
    public class BitmapConverter
    {
        private const int ResizeRate = 1;

        public static Pixel[,] LoadPixels(Bitmap bmp)
        {
            var pixels = new Pixel[bmp.Width, bmp.Height];

            int[] pixelArray = new int[bmp.Width * bmp.Height];
            bmp.GetPixels(pixelArray, 0, bmp.Width, 0, 0, bmp.Width, bmp.Height);

            int index = 0;
            for (var y = 0; y < bmp.Height; y++)
            {
                for (var x = 0; x < bmp.Width; x++)
                {
                    int pixelColor = pixelArray[index];
                    byte red = (byte)((pixelColor >> 16) & 0xFF);
                    byte green = (byte)((pixelColor >> 8) & 0xFF);
                    byte blue = (byte)(pixelColor & 0xFF);
                    pixels[x, y] = new Pixel(red, green, blue);
                    index++;
                }
            }
            return pixels;
        }

        public static Bitmap ConvertToBitmap(int width, int height, Func<int, int, Color> getPixelColor)
        {
            Bitmap bmp = Bitmap.CreateBitmap(ResizeRate * width, ResizeRate * height, Bitmap.Config.Argb8888);

            int[] pixelArray = new int[width * height];
            int index = 0;
            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    Color pixelColor = getPixelColor(x, y);
                    pixelArray[index] = (pixelColor.A << 24) | (pixelColor.R << 16) | (pixelColor.G << 8) | pixelColor.B;
                    index++;
                }
            }

            bmp.SetPixels(pixelArray, 0, width * ResizeRate, 0, 0, width * ResizeRate, height * ResizeRate);

            return bmp;
        }

        public static Bitmap ConvertToBitmap(double[,] array)
        {
            int width = array.GetLength(0);
            int height = array.GetLength(1);

            return ConvertToBitmap(width, height, (x, y) =>
            {
                int gray = (int)(255 * array[x, y]);
                gray = Math.Min(gray, 255);
                gray = Math.Max(gray, 0);
                return new Color(gray, gray, gray);
            });
        }

        public static Bitmap ConvertToBitmap(Pixel[,] array)
        {
            int width = array.GetLength(0);
            int height = array.GetLength(1);

            return ConvertToBitmap(width, height, (x, y) =>
            {
                Pixel pixel = array[x, y];
                return new Color(pixel.R, pixel.G, pixel.B);
            });
        }

        public static byte[] GetImageBytes(Bitmap bitmap)
        {
            using (var stream = new MemoryStream())
            {
                bitmap.Compress(Bitmap.CompressFormat.Jpeg, 100, stream);
                return stream.ToArray();
            }
        }

        public static Bitmap CompressWithAspectRatio(byte[] imageBytes, int maxWidth, int maxHeight)
        {
            BitmapFactory.Options options = new BitmapFactory.Options();
            options.InJustDecodeBounds = true;

            BitmapFactory.DecodeByteArray(imageBytes, 0, imageBytes.Length, options);

            int width = options.OutWidth;
            int height = options.OutHeight;

            if (width <= maxWidth && height <= maxHeight)
            {
                return BitmapFactory.DecodeByteArray(imageBytes, 0, imageBytes.Length);
            }

            float scaleFactor = Math.Min((float)maxWidth / width, (float)maxHeight / height);

            int newWidth = (int)(width * scaleFactor);
            int newHeight = (int)(height * scaleFactor);

            options.InJustDecodeBounds = false;

            Bitmap bitmap = BitmapFactory.DecodeByteArray(imageBytes, 0, imageBytes.Length, options);

            Bitmap resizedBitmap = Bitmap.CreateScaledBitmap(bitmap, newWidth, newHeight, true);

            bitmap.Recycle();

            return resizedBitmap;
        }
    }
}
