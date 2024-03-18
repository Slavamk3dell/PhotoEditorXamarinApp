using Android.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace TestXamarinApp
{
    public class Pixel
    {
        public Pixel(byte r, byte g, byte b)
        {
            R = r;
            G = g;
            B = b;
        }

        public Pixel(Color color)
        {
            R = color.R;
            G = color.G;
            B = color.B;
        }

        public byte R { get; }
        public byte G { get; }
        public byte B { get; }

        public override string ToString()
        {
            return $"Pixel({R}, {G}, {B})";
        }
    }
}
