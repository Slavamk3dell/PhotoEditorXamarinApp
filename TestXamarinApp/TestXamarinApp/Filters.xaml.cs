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
using SkiaSharp;
using SkiaSharp.Views.Forms;

namespace TestXamarinApp
{
    public partial class Filters : ContentPage
    {
        public static byte[] currentImageBytes;
        public Filters()
        {
            InitializeComponent();
            imageFromGallery.Source = "filters.png";
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
                        var imageBitmap = SKBitmapConverter.CreateSKBitmapFromBytes(currentImageBytes);
                        ColorMatrixFilter.ApplyColorFilterToSKBitmap(imageBitmap, new float[]
                        {
                            0.2126f, 0.7152f, 0.0722f, 0, 0,
                            0.2126f, 0.7152f, 0.0722f, 0, 0,
                            0.2126f, 0.7152f, 0.0722f, 0, 0,
                            0,       0,       0,       1, 0
                        });
                        currentImageBytes = SKBitmapConverter.GetImageBytesFromSKBitmap(imageBitmap, SKEncodedImageFormat.Jpeg);
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
                }
            }
        }
    }
}