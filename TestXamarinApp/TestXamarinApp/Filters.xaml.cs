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
        }

        async void OnPickPhotoButtonClicked(object sender, EventArgs e)
        {
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
        }

        async void SaveToGalleryButtonClicked(object sender, EventArgs e)
        {
            if (currentImageBytes != null)
            {
                try
                {
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
                    await DisplayAlert("Готово", "Изображение успешно сохранено!!!", "OK");
                }
            }
        }

        async void OnActionButtonClicked(object sender, EventArgs e)
        {
            float[] filter = new float[0];
            if (currentImageBytes != null)
            {
                string result = await DisplayActionSheet("Выбор фильтра", "Отмена", null, "Черно-белый", "Теплый", "Холодный", "Светлый", "Темный");

                switch (result)
                {
                    case "Черно-белый":
                        filter = new float[]
                        {
                            0.2126f, 0.7152f, 0.0722f, 0, 0,
                            0.2126f, 0.7152f, 0.0722f, 0, 0,
                            0.2126f, 0.7152f, 0.0722f, 0, 0,
                            0,       0,       0,       1, 0
                        };
                        break;

                    case "Теплый":
                        filter = new float[]
                        {
                            1.2f, 0.0f, 0.0f, 0.0f, 0.0f,
                            0.0f, 1.1f, 0.0f, 0.0f, 0.0f,
                            0.0f, 0.1f, 0.9f, 0.0f, 0.0f,
                            0.0f, 0.0f, 0.0f, 1.0f, 0.0f
                        };
                        break;

                    case "Холодный":
                        filter = new float[]
                        {
                            0.8f, 0.0f, 0.0f, 0.0f, 0.0f,
                            0.0f, 0.8f, 0.0f, 0.0f, 0.0f,
                            0.0f, 0.0f, 1.2f, 0.0f, 0.0f,
                            0.0f, 0.0f, 0.0f, 1.0f, 0.0f
                        };
                        break;

                    case "Светлый":
                        filter = new float[]
                        {
                            1.3f, 1.1f, 0.0f, 0.0f, 0.0f,
                            0.3f, 1.2f, 0.3f, 0.0f, 0.0f,
                            0.0f, 0.3f, 1.3f, 0.0f, 0.0f,
                            0.0f, 0.0f, 0.0f, 1.0f, 0.0f
                        };
                        break;

                    case "Темный":
                        filter = new float[]
                        {
                            0.5f, 0.0f, 0.0f, 0.0f, 0.0f,
                            0.0f, 0.5f, 0.0f, 0.0f, 0.0f,
                            0.0f, 0.0f, 0.5f, 0.0f, 0.0f,
                            0.0f, 0.0f, 0.0f, 1.0f, 0.0f
                        };
                        break;

                    default:
                        return;
                }

                try
                {
                    imageActivityIndicator.IsRunning = true;
                    imageActivityIndicator.IsVisible = true;
                    imageFromGallery.IsVisible = false;

                    await Task.Run(() =>
                    {
                        var imageBitmap = SKBitmapConverter.CreateSKBitmapFromBytes(currentImageBytes);
                        ColorMatrixFilter.ApplyColorFilterToSKBitmap(imageBitmap, filter);
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
                    imageActivityIndicator.IsRunning = false;
                    imageActivityIndicator.IsVisible = false;
                    imageFromGallery.IsVisible = true;
                }
            }
        }
    }
}