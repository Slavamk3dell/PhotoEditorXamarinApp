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
        public Generation()
        {
            InitializeComponent();
        }

        private async void GenerateImageButton_Clicked(object sender, EventArgs e)
        {
            var apiUrl = "https://api-key.fusionbrain.ai/";
            var apiKey = "E7385E834DD1111F486D3B3D1FC87425";
            var secretKey = "6E025281D9209C85D8FDB19621A0072B";

            var text2ImageApi = new Text2ImageAPI(apiUrl, apiKey, secretKey);

            // Step 1: Get a model ID
            var modelId = await text2ImageApi.GetModelIdAsync();

            // Step 2: Generate an image
            var prompt = YourEntry.Text;
            var uuid = await text2ImageApi.GenerateImageAsync(prompt, modelId);

            Console.WriteLine($"Request sent. UUID: {uuid}");

            // Step 3: Check image generation status
            var images = await text2ImageApi.CheckGenerationStatusAsync(uuid);

            var imageBytes = Base64ImageConverter.ConvertBase64ToByteArray(images[0]);


            ResultImage.Source = ImageSource.FromStream(() => new MemoryStream(imageBytes));


        }
    }
}