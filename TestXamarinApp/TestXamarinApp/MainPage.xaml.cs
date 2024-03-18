using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;
using System.IO;
using System.Threading;
using Android.Graphics;

namespace TestXamarinApp
{
    public partial class MainPage : ContentPage
    {
        public static byte[] currentImageBytes;
        public MainPage()
        {
            InitializeComponent();
            //string uriImage = "https://img.freepik.com/free-photo/cute-domestic-kitten-sits-at-window-staring-outside-generative-ai_188544-12519.jpg?size=626&ext=jpg&ga=GA1.1.1292351815.1710028800&semt=ais";
            //imageFromGellary.Source = new Uri(uriImage);
        }

        async void OnPickPhotoButtonClicked(object sender, EventArgs e)
        {
            (sender as Xamarin.Forms.Button).IsEnabled = false;

            Stream stream = await DependencyService.Get<IImageService>().GetImageStreamAsync();
            if (stream != null)
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    await stream.CopyToAsync(memoryStream);

                    currentImageBytes = memoryStream.ToArray();

                    imageFromGallery.Source = ImageSource.FromStream(() => new MemoryStream(currentImageBytes));
                }
            }
            
            (sender as Xamarin.Forms.Button).IsEnabled = true;
        }

        async void SaveToGalleryButtonClicked(object sender, EventArgs e)
        {
            if (currentImageBytes != null)
            {
                try
                {
                    (sender as Xamarin.Forms.Button).IsEnabled = false;
                    await Task.Run(() =>
                    {
                        DependencyService.Get<IImageService>().SaveImageToGallery(currentImageBytes);
                    });
                }
                catch (Exception)
                {
                    await DisplayAlert("Ошибка", "Не удалось сохранить изображение", "OK");
                    return;
                }
                finally
                {
                    (sender as Xamarin.Forms.Button).IsEnabled = true;
                    await DisplayAlert("Готово", "Изображение успешно сохранено!!!", "OK");
                }
            }
        }

        async void OnActionButtonClicked(object sender, EventArgs e)
        {
            if (currentImageBytes != null)
            {
                try
                {
                    (sender as Xamarin.Forms.Button).IsEnabled = false;

                    imageActivityIndicator.IsRunning = true;
                    imageActivityIndicator.IsVisible = true;
                    imageFromGallery.IsVisible = false;

                    await Task.Run(() =>
                    {
                        Bitmap bitmapImage = BitmapConverter.CompressWithAspectRatio(currentImageBytes, maxWidth: 1920, maxHeight: 1920);
                        Pixel[,] pixelsOfBitmap = BitmapConverter.LoadPixels(bitmapImage);
                        var colorScale = ColorScaleFilter.ToGrayscale(pixelsOfBitmap);
                        Bitmap bitmapOfGrayScale = BitmapConverter.ConvertToBitmap(colorScale);
                        currentImageBytes = BitmapConverter.GetImageBytes(bitmapOfGrayScale);
                    });

                    imageFromGallery.Source = ImageSource.FromStream(() => new MemoryStream(currentImageBytes));
                }
                catch (Exception)
                {
                    await DisplayAlert("Ошибка", "Что-то пошло не так!!!", "OK");
                    return;
                }
                finally
                {
                    (sender as Xamarin.Forms.Button).IsEnabled = true;

                    imageActivityIndicator.IsRunning = false;
                    imageActivityIndicator.IsVisible = false;
                    imageFromGallery.IsVisible = true;

                    await DisplayAlert("Готово", "Изображение успешно обработано.", "OK");
                } 
            }
        }
    }
}
