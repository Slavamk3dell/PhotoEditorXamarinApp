using System;
using System.Collections.Generic;
using System.Text;

namespace TestXamarinApp
{
    public class Base64ImageConverter
    {
        public static byte[] ConvertBase64ToByteArray(string base64String)
        {
            try
            {
                return Convert.FromBase64String(base64String);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error converting Base64 to byte array: {ex.Message}");
                return null;
            }
        }
    }
}
