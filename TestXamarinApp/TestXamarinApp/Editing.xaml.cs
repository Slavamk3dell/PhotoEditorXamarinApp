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
        public Editing()
        {
            InitializeComponent();
            MessagingCenter.Subscribe<Generation, byte[]>(this, "ImageChanged", (sender, newImage) =>
            {
                currentImageBytes = newImage;
                imageFromGallery.Source = ImageSource.FromStream(() => new MemoryStream(currentImageBytes));
                LabelMessage.IsVisible = false;
                SizeImage.Text = $"Текущий размер: {SKBitmapConverter.GetImageSizeInMB(currentImageBytes):F1} МБ";
            });
            MessagingCenter.Subscribe<Filters, byte[]>(this, "ImageChanged", (sender, newImage) =>
            {
                currentImageBytes = newImage;
                imageFromGallery.Source = ImageSource.FromStream(() => new MemoryStream(currentImageBytes));
                LabelMessage.IsVisible = false;
                SizeImage.Text = $"Текущий размер: {SKBitmapConverter.GetImageSizeInMB(currentImageBytes):F1} МБ";
            });
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

                    MessagingCenter.Send(this, "ImageChanged", currentImageBytes);
                }
                LabelMessage.IsVisible = false;
                SizeImage.Text = $"Текущий размер: {SKBitmapConverter.GetImageSizeInMB(currentImageBytes):F1} МБ";
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
    }
}