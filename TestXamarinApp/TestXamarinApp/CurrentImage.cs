using Android.Graphics;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace TestXamarinApp
{
    public class CurrentImage
    {
        public static CurrentImage СurrentImage;

        private byte[] ImageBytes { get; set; }
        private SKEncodedImageFormat ImageFormat { get; set; }

        public CurrentImage(byte[] imageBytes, SKEncodedImageFormat imageFormat)
        {
            ImageBytes = imageBytes;
            ImageFormat = imageFormat;
        }
    }
}
