using System;
using System.Collections.Generic;
using System.Text;

namespace TestXamarinApp
{
    public class MedianFilter
    {
        public static double[,] MedianFilterForGray(double[,] original)
        {
            var rowCount = original.GetLength(0);
            var columnCount = original.GetLength(1);
            var medianScale = new double[rowCount, columnCount];
            for (int x = 0; x < rowCount; x++)
                for (int y = 0; y < columnCount; y++)
                {
                    var listOfPixelsInCurrentWindow = new List<double>();
                    int[] d = { -1, 0, 1 };
                    for (int i = 0; i < 3; i++)
                        for (int j = 0; j < 3; j++)
                        {
                            int neighborX = x + d[i];
                            int neighborY = y + d[j];
                            if (neighborX >= 0 && neighborX < rowCount && neighborY >= 0 && neighborY < columnCount)
                                listOfPixelsInCurrentWindow.Add(original[neighborX, neighborY]);
                        }
                    medianScale[x, y] = FindMedian(listOfPixelsInCurrentWindow);
                }
            return original;
        }

        public static Pixel[,] MedianFilterForRGB(Pixel[,] original)
        {
            var rowCount = original.GetLength(0);
            var columnCount = original.GetLength(1);
            var medianScale = new Pixel[rowCount, columnCount];
            for (int x = 0; x < rowCount; x++)
                for (int y = 0; y < columnCount; y++)
                {
                    var listOfPixelsRed = new List<byte>();
                    var listOfPixelsGreen = new List<byte>();
                    var listOfPixelsBlue = new List<byte>();
                    int[] d = { -1, 0, 1 };
                    for (int i = 0; i < 3; i++)
                        for (int j = 0; j < 3; j++)
                        {
                            int neighborX = x + d[i];
                            int neighborY = y + d[j];
                            if (neighborX >= 0 && neighborX < rowCount && neighborY >= 0 && neighborY < columnCount)
                            {
                                listOfPixelsRed.Add(original[neighborX, neighborY].R);
                                listOfPixelsGreen.Add(original[neighborX, neighborY].G);
                                listOfPixelsBlue.Add(original[neighborX, neighborY].B);
                            }
                        }
                    medianScale[x, y] = new Pixel(
                        FindMedian(listOfPixelsRed),
                        FindMedian(listOfPixelsGreen),
                        FindMedian(listOfPixelsBlue)
                        );
                }
            return medianScale;
        }

        private static double FindMedian(List<double> listOfPixelsAround)
        {
            listOfPixelsAround.Sort();
            var length = listOfPixelsAround.Count;
            if (length % 2 != 0) return listOfPixelsAround[length / 2];
            int middleIndex1 = length / 2 - 1;
            int middleIndex2 = length / 2;
            return (listOfPixelsAround[middleIndex1] + listOfPixelsAround[middleIndex2]) / 2.0;
        }

        private static byte FindMedian(List<byte> listOfPixels)
        {
            listOfPixels.Sort();
            var length = listOfPixels.Count;
            if (length % 2 != 0) return listOfPixels[length / 2];
            int middleIndex1 = length / 2 - 1;
            int middleIndex2 = length / 2;
            byte result = (byte)((listOfPixels[middleIndex1] + listOfPixels[middleIndex2]) / 2);
            return result;
        }
    }
}
