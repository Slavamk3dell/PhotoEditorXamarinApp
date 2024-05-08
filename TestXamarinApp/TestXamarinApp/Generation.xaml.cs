using Microsoft.Scripting.Hosting.Providers;
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
    public partial class Generation : ContentPage
    {
        private static byte[] currentImageBytes;
        private static string apiUrl = "https://api-key.fusionbrain.ai/";
        private static string apiKey = "B902B6DBF831F82154ECE55B796D8390";
        private static string secretKey = "201FBE7F700C01BFD5A26BD2093E00BF";
        public Generation()
        {
            InitializeComponent();
        }

        private async void GenerateImageButton_Clicked(object sender, EventArgs e)
        {
            imageActivityIndicator.IsRunning = true;
            imageActivityIndicator.IsVisible = true;
            ResultImage.IsVisible = false;
            LabelMessage.IsVisible = false;
            GenerationButton.IsEnabled = false;

            var text2ImageApi = new ImageGenerator(apiUrl, apiKey, secretKey);

            var modelId = await text2ImageApi.GetModelIdAsync();

            var prompt = YourEntry.Text;
            var uuid = await text2ImageApi.GenerateImageAsync(prompt, modelId);

            var images = await text2ImageApi.CheckGenerationStatusAsync(uuid);

            if (images.Count < 1)
            {
                await DisplayAlert("Ошибка", "Генерация слишком долгая. Попробуйте позже", "ОК");
                GenerationButton.IsEnabled = true;
                imageActivityIndicator.IsRunning = false;
                imageActivityIndicator.IsVisible = false;
                ResultImage.IsVisible = true;
                if (currentImageBytes == null)
                    LabelMessage.IsVisible = true;
                return;
            }

            currentImageBytes = Base64ImageConverter.ConvertBase64ToByteArray(images[0]);

            ResultImage.Source = ImageSource.FromStream(() => new MemoryStream(currentImageBytes));

            MessagingCenter.Send(this, "ImageChanged", currentImageBytes);

            imageActivityIndicator.IsRunning = false;
            imageActivityIndicator.IsVisible = false;
            ResultImage.IsVisible = true;
            GenerationButton.IsEnabled = true;
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
            else
            {
                await DisplayAlert("Ошибка", "Изображение для сохранения отсутствует", "ОК");
            }
        }
    }
}