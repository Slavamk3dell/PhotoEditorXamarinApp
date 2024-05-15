using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TestXamarinApp
{
    public partial class Editing : ContentPage
    {
        private static byte[] CurrentImageBytes;
        private static byte[] FilterImageBytes;
        private static string CurrentImageFormat;
        private static double CurrentImageSize;
        public Editing()
        {
            InitializeComponent();
            MessagingCenter.Subscribe<Generation, byte[]>(this, "ImageChanged", (sender, newImage) =>
            {
                CurrentImageBytes = newImage;
                FilterImageBytes = null;
                CurrentImageSize = SKBitmapConverter.GetImageSizeInMB(CurrentImageBytes);
                CurrentImageFormat = SKBitmapConverter.GetImageFormat(CurrentImageBytes);
                imageFromGallery.Source = ImageSource.FromStream(() => new MemoryStream(CurrentImageBytes));
                LabelMessage.IsVisible = false;
                SizeImage.Text = $"Текущий размер: {CurrentImageSize:F1} МБ";
                FormatImage.Text = $"Текущий формат: {CurrentImageFormat}";
                SizeImage.IsVisible = true;
                FormatImage.IsVisible = true;
            });
        }

        private async void CompressButtonClicked(object sender, EventArgs e)
        {
            if (CurrentImageFormat == "png")
            {
                await DisplayAlert("Ошибка", "Формат PNG не поддерживает сжатие", "ОК");
                return;
            }
            if (CurrentImageBytes == null)
            {
                await DisplayAlert("Ошибка", "Отсутствует изображение", "ОК");
                return;
            }

            var message = new CompressMessage
            {
                ImageBytes = CurrentImageBytes,
                ImageSize = CurrentImageSize,
                ImageFormat = CurrentImageFormat
            };
            await Navigation.PushAsync(new EditingCompression(message, this));
        }

        public void UpdateImageData(byte[] compressedImageBytes, double compressedImageSize)
        {
            CurrentImageSize = compressedImageSize;
            CurrentImageBytes = compressedImageBytes;
            imageFromGallery.Source = ImageSource.FromStream(() => new MemoryStream(CurrentImageBytes));
            SizeImage.Text = $"Текущий размер: {CurrentImageSize:F1} МБ";
        }

        private async void ConvertButtonClicked(object sender, EventArgs e)
        {
            if (CurrentImageBytes != null)
            {
                string result = await DisplayActionSheet("Конвертировать в", "Отмена", null, "JPEG", "PNG", "WEBP");

                imageActivityIndicator.IsRunning = true;
                imageActivityIndicator.IsVisible = true;
                imageFromGallery.IsVisible = false;
                SizeImage.IsVisible= false;
                FormatImage.IsVisible= false;

                try
                {
                    switch (result)
                    {
                        case "JPEG":
                            CurrentImageBytes = SKBitmapConverter.GetImageBytesFromSKBitmap(SKBitmapConverter.CreateSKBitmapFromBytes(CurrentImageBytes), SKEncodedImageFormat.Jpeg);
                            break;
                        case "PNG":
                            CurrentImageBytes = SKBitmapConverter.GetImageBytesFromSKBitmap(SKBitmapConverter.CreateSKBitmapFromBytes(CurrentImageBytes), SKEncodedImageFormat.Png);
                            break;
                        case "WEBP":
                            CurrentImageBytes = SKBitmapConverter.GetImageBytesFromSKBitmap(SKBitmapConverter.CreateSKBitmapFromBytes(CurrentImageBytes), SKEncodedImageFormat.Webp);
                            break;
                        default:
                            imageActivityIndicator.IsRunning = false;
                            imageActivityIndicator.IsVisible = false;
                            imageFromGallery.IsVisible = true;
                            SizeImage.IsVisible = true;
                            FormatImage.IsVisible = true;
                            return;
                    }
                }
                catch (Exception)
                {
                    await DisplayAlert("Ошибка", "Не удалось конвертировать изображение", "ОК");
                    imageActivityIndicator.IsRunning = false;
                    imageActivityIndicator.IsVisible = false;
                    imageFromGallery.IsVisible = true;
                    SizeImage.IsVisible = true;
                    FormatImage.IsVisible = true;
                    return;
                }

                imageFromGallery.Source = ImageSource.FromStream(() => new MemoryStream(CurrentImageBytes));
                CurrentImageSize = SKBitmapConverter.GetImageSizeInMB(CurrentImageBytes);
                CurrentImageFormat = SKBitmapConverter.GetImageFormat(CurrentImageBytes);
                SizeImage.Text = $"Текущий размер: {CurrentImageSize:F1} МБ";
                FormatImage.Text = $"Текущий формат: {CurrentImageFormat}";

                imageActivityIndicator.IsRunning = false;
                imageActivityIndicator.IsVisible = false;
                imageFromGallery.IsVisible = true;
                SizeImage.IsVisible = true;
                FormatImage.IsVisible = true;
            }
            else
            {
                await DisplayAlert("Ошибка", "Отсутствует изображение", "ОК");
            }
        }

        private async void OnPickPhotoButtonClicked(object sender, EventArgs e)
        {
            Stream stream = await DependencyService.Get<IImageService>().GetImageStreamAsync();
            byte[] imageBytes = null;
            string imageFormat = null;
            if (stream != null)
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    await stream.CopyToAsync(memoryStream);
                    imageBytes = memoryStream.ToArray();
                }

                imageFormat = SKBitmapConverter.GetImageFormat(imageBytes);
                if (imageFormat == "Unknown")
                {
                    await DisplayAlert("Ошибка", "Приложение не поддерживает формат данного изображения.\nПоддержанные форматы: JPEG, PNG, WEBP", "OK");
                    return;
                }

                CurrentImageBytes = imageBytes;
                imageFromGallery.Source = ImageSource.FromStream(() => new MemoryStream(CurrentImageBytes));
                LabelMessage.IsVisible = false;
                CurrentImageSize = SKBitmapConverter.GetImageSizeInMB(CurrentImageBytes);
                CurrentImageFormat = imageFormat;
                SizeImage.Text = $"Текущий размер: {CurrentImageSize:F1} МБ";
                FormatImage.Text = $"Текущий формат: {CurrentImageFormat}";
                FormatImage.IsVisible = true;
                SizeImage.IsVisible = true;
            }
        }

        private async void SaveToGalleryButtonClicked(object sender, EventArgs e)
        {
            if (CurrentImageBytes != null)
            {
                try
                {
                    await Task.Run(() =>
                    {
                        if (FilterImageBytes != null)
                        {
                            DependencyService.Get<IImageService>().SaveImageToGallery(FilterImageBytes, CurrentImageFormat);
                            CurrentImageBytes = FilterImageBytes;
                            CurrentImageSize = SKBitmapConverter.GetImageSizeInMB(CurrentImageBytes);
                            SizeImage.Text = $"Текущий размер: {CurrentImageSize:F1} МБ";
                            FilterImageBytes = null;
                        }
                        else
                            DependencyService.Get<IImageService>().SaveImageToGallery(CurrentImageBytes, CurrentImageFormat);
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
            if (CurrentImageBytes != null)
            {
                string result = await DisplayActionSheet("Выбор фильтра", "Отмена", null, "Оригинал", "Черно-белый", "Теплый", "Холодный", "Светлый", "Темный", "Винтаж", "Зеленка");

                switch (result)
                {
                    case "Оригинал":
                        FilterImageBytes = null;
                        imageFromGallery.Source = ImageSource.FromStream(() => new MemoryStream(CurrentImageBytes));
                        SizeImage.Text = $"Текущий размер: {SKBitmapConverter.GetImageSizeInMB(CurrentImageBytes):F1} МБ";
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
                        var imageBitmap = SKBitmapConverter.CreateSKBitmapFromBytes(CurrentImageBytes);
                        ColorMatrixFilter.ApplyColorFilterToSKBitmap(imageBitmap, filter);
                        FilterImageBytes = SKBitmapConverter.GetImageBytesFromSKBitmap(imageBitmap, 
                            SKBitmapConverter.ConvertStringToEncodedImageFormat(CurrentImageFormat));
                    });
                    imageFromGallery.Source = ImageSource.FromStream(() => new MemoryStream(FilterImageBytes));
                    SizeImage.Text = $"Текущий размер: {SKBitmapConverter.GetImageSizeInMB(FilterImageBytes ?? CurrentImageBytes):F1} МБ";
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