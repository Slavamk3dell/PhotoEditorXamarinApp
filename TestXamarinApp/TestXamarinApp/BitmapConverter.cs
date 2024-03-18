using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Android.Graphics;
using Color = Android.Graphics.Color;

namespace TestXamarinApp
{
    public class BitmapConverter
    {
        private const int ResizeRate = 1;
        public static Pixel[,] LoadPixels(Bitmap bmp)
        {
            var pixels = new Pixel[bmp.Width, bmp.Height];
            for (var x = 0; x < bmp.Width; x++)
            {
                for (var y = 0; y < bmp.Height; y++)
                {
                    int pixelColor = bmp.GetPixel(x, y);
                    byte red = (byte)((pixelColor >> 16) & 0xFF);
                    byte green = (byte)((pixelColor >> 8) & 0xFF);
                    byte blue = (byte)(pixelColor & 0xFF);
                    pixels[x, y] = new Pixel(red, green, blue);
                }
            }
            return pixels;
        }

        public static Bitmap ConvertToBitmap(int width, int height, Func<int, int, Color> getPixelColor)
        {
            Bitmap bmp = Bitmap.CreateBitmap(ResizeRate * width, ResizeRate * height, Bitmap.Config.Argb8888);
            for (var x = 0; x < width; x++)
                for (var y = 0; y < height; y++)
                    bmp.SetPixel(ResizeRate * x, ResizeRate * y, getPixelColor(x, y));

            return bmp;
        }

        public static Bitmap ConvertToBitmap(Pixel[,] array)
        {
            int width = array.GetLength(0);
            int height = array.GetLength(1);
            return ConvertToBitmap(width, height,
                (x, y) => new Color(array[x, y].R, array[x, y].G, array[x, y].B));
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

        public static Bitmap BytesToBitmap(byte[] imageBytes)
        {
            Bitmap bitmap = BitmapFactory.DecodeByteArray(imageBytes, 0, imageBytes.Length);
            return bitmap;
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
            using (MemoryStream memoryStream = new MemoryStream(imageBytes))
            {
                Bitmap bitmap = BitmapFactory.DecodeStream(memoryStream);

                int width = bitmap.Width;
                int height = bitmap.Height;

                float scaleFactor = Math.Min((float)maxWidth / width, (float)maxHeight / height);

                int newWidth = (int)(width * scaleFactor);
                int newHeight = (int)(height * scaleFactor);

                Bitmap resizedBitmap = Bitmap.CreateScaledBitmap(bitmap, newWidth, newHeight, true);

                return resizedBitmap;
            }
        }
    }
}
