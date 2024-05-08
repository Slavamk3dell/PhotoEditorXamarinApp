using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TestXamarinApp
{
    public partial class Editing : ContentPage
    {
        private static byte[] currentImageBytes;
        private static byte[] filterImageBytes;
        public Editing()
        {
            InitializeComponent();
            MessagingCenter.Subscribe<Generation, byte[]>(this, "ImageChanged", (sender, newImage) =>
            {
                currentImageBytes = newImage;
                filterImageBytes = null;
                imageFromGallery.Source = ImageSource.FromStream(() => new MemoryStream(currentImageBytes));
                LabelMessage.IsVisible = false;
                SizeImage.Text = $"Текущий размер: {SKBitmapConverter.GetImageSizeInMB(currentImageBytes):F1} МБ";
                SizeImage.IsVisible = true;
            });
            //MessagingCenter.Subscribe<Filters, byte[]>(this, "ImageChanged", (sender, newImage) =>
            //{
            //    currentImageBytes = newImage;
            //    filterImageBytes = null;
            //    imageFromGallery.Source = ImageSource.FromStream(() => new MemoryStream(currentImageBytes));
            //    LabelMessage.IsVisible = false;
            //    SizeImage.Text = $"Текущий размер: {SKBitmapConverter.GetImageSizeInMB(currentImageBytes):F1} МБ";
            //});
        }

        private async void CompressButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new EditingCompression());
        }

        private void ConvertButtonClicked(object sender, EventArgs e)
        {

        }

        private async void OnPickPhotoButtonClicked(object sender, EventArgs e)
        {
            Stream stream = await DependencyService.Get<IImageService>().GetImageStreamAsync();
            if (stream != null)
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    await stream.CopyToAsync(memoryStream);

                    currentImageBytes = memoryStream.ToArray();

                    imageFromGallery.Source = ImageSource.FromStream(() => new MemoryStream(currentImageBytes));

                    // MessagingCenter.Send(this, "ImageChanged", currentImageBytes);
                }
                LabelMessage.IsVisible = false;
                SizeImage.Text = $"Текущий размер: {SKBitmapConverter.GetImageSizeInMB(currentImageBytes):F1} МБ";
                SizeImage.IsVisible = true;
            }
        }

        private async void SaveToGalleryButtonClicked(object sender, EventArgs e)
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
                    // MessagingCenter.Send(this, "ImageChanged", currentImageBytes);
                }
            }
            else
            {
                await DisplayAlert("Ошибка", "Изображение для сохранения отсутствует", "ОК");
            }
        }

        private async void OnFilterButtonClicked(object sender, EventArgs e)
        {
            float[] filter = new float[0];
            if (currentImageBytes != null)
            {
                string result = await DisplayActionSheet("Выбор фильтра", "Отмена", null, "Оригинал", "Черно-белый", "Теплый", "Холодный", "Светлый", "Темный", "Винтаж", "Зеленка");

                switch (result)
                {
                    case "Оригинал":
                        filterImageBytes = null;
                        imageFromGallery.Source = ImageSource.FromStream(() => new MemoryStream(currentImageBytes));
                        SizeImage.Text = $"Текущий размер: {SKBitmapConverter.GetImageSizeInMB(currentImageBytes):F1} МБ";
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
                            0.7f, 0.2f, 0.1f, 0.0f, 0.0f,
                            0.3f, 0.7f, 0.1f, 0.0f, 0.0f,
                            0.2f, 0.3f, 0.6f, 0.0f, 0.0f,
                            0.0f, 0.0f, 0.0f, 1.0f, 0.0f
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
                    SizeImage.Text = $"Текущий размер: {SKBitmapConverter.GetImageSizeInMB(filterImageBytes ?? currentImageBytes):F1} МБ";
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