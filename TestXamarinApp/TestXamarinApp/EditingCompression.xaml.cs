using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TestXamarinApp
{
    public partial class EditingCompression : ContentPage
    {
        private static byte[] CurrentImageBytes = null;
        private static double CurrentImageSize;
        private static string CurrentImageFormat;
        private Editing _editingPage;
        private static byte[] OriginalImageBytes = null;

        public EditingCompression(CompressMessage message, Editing editingPage)
        {
            InitializeComponent();
            CurrentImageSize = message.ImageSize;
            OriginalImageBytes = message.ImageBytes;
            CurrentImageFormat = message.ImageFormat;
            _editingPage = editingPage;
            SizeLabel.Text = $"Размер после сжатия: {CurrentImageSize:F1} МБ";
        }

        private void SliderValueChanged(object sender, ValueChangedEventArgs e)
        {
            double value = Math.Round(e.NewValue);
            label.Text = string.Format("Значение: {0}", value);
        }

        private void ApplyButtonClicked(object sender, EventArgs e)
        {
            CurrentImageBytes = SKBitmapConverter.GetImageBytesFromSKBitmap(SKBitmapConverter.CreateSKBitmapFromBytes(OriginalImageBytes), 
                SKBitmapConverter.ConvertStringToEncodedImageFormat(CurrentImageFormat), 
                (int)Math.Round(slider.Value));
            CurrentImageSize = SKBitmapConverter.GetImageSizeInMB(CurrentImageBytes);
            SizeLabel.Text = $"Размер после сжатия: {CurrentImageSize:F1} МБ";
        }

        private async void ConfirmButtonClicked(object sender, EventArgs e)
        {
            _editingPage.UpdateImageData(CurrentImageBytes ?? OriginalImageBytes, CurrentImageSize);
            CurrentImageBytes = null;
            await Navigation.PopAsync();
        }
    }
}