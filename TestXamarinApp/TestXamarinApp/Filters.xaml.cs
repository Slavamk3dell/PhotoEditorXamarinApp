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
        private static byte[] currentImageBytes;
        private static byte[] filterImageBytes;
        public Filters()
        {
            InitializeComponent();
            MessagingCenter.Subscribe<Generation, byte[]>(this, "ImageChanged", (sender, newImage) =>
            {
                currentImageBytes = newImage;
                filterImageBytes = null;
                imageFromGallery.Source = ImageSource.FromStream(() => new MemoryStream(currentImageBytes));
                LabelMessage.IsVisible = false;
            });
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
                LabelMessage.IsVisible = false;
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
                        if (filterImageBytes != null)
                        {
                            DependencyService.Get<IImageService>().SaveImageToGallery(filterImageBytes);
                            currentImageBytes = filterImageBytes;
                            filterImageBytes = null;
                        }
                        else
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
            else
            {
                await DisplayAlert("Ошибка", "Изображение для сохранения отсутствует", "ОК");
            }
        }

        async void OnActionButtonClicked(object sender, EventArgs e)
        {
            float[] filter = new float[0];
            if (currentImageBytes != null)
            {
                string result = await DisplayActionSheet("Выбор фильтра", "Отмена", null, "Оригинал","Черно-белый", "Теплый", "Холодный", "Светлый", "Темный", "Винтаж", "Зеленка");

                switch (result)
                {
                    case "Оригинал":
                        filterImageBytes = null;
                        imageFromGallery.Source = ImageSource.FromStream(() => new MemoryStream(currentImageBytes));
                        return;

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
                            1.5f, 0.0f, 0.0f, 0.0f, 0.0f,
                            0.0f, 1.2f, 0.0f, 0.0f, 0.0f,
                            0.0f, 0.1f, 0.8f, 0.0f, 0.0f,
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

                    case "Винтаж":
                        filter = new float[]
                        {
                            0.7f, 0.2f, 0.1f, 0.0f, 0.0f, // Красный с добавлением зеленого и синего
                            0.3f, 0.7f, 0.1f, 0.0f, 0.0f, // Зеленый с добавлением красного и синего
                            0.2f, 0.3f, 0.6f, 0.0f, 0.0f, // Синий с добавлением красного и зеленого
                            0.0f, 0.0f, 0.0f, 1.0f, 0.0f  // Альфа-канал
                        };
                        break;

                    case "Зеленка":
                        filter = new float[]
                        {
                            0.2f, 0.5f, 0.2f, 0.0f, 0.0f,
                            0.0f, 1.5f, 0.0f, 0.0f, 0.0f,
                            0.2f, 0.5f, 0.5f, 0.0f, 0.0f,
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
                        filterImageBytes = SKBitmapConverter.GetImageBytesFromSKBitmap(imageBitmap, SKEncodedImageFormat.Jpeg);
                    });
                    imageFromGallery.Source = ImageSource.FromStream(() => new MemoryStream(filterImageBytes));
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
            else
            {
                await DisplayAlert("Ошибка", "Отсутствует изображение", "ОК");
            }
        }
    }
}