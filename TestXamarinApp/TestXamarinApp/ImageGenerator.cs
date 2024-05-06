using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TestXamarinApp
{
    public class ImageGenerator
    {
        private readonly string apiUrl;
        private readonly string apiKey;
        private readonly string secretKey;
        private readonly HttpClient httpClient;

        public ImageGenerator(string url, string apiKey, string secretKey)
        {
            this.apiUrl = url;
            this.apiKey = apiKey;
            this.secretKey = secretKey;
            this.httpClient = new HttpClient();
            this.httpClient.DefaultRequestHeaders.Add("X-Key", $"Key {apiKey}");
            this.httpClient.DefaultRequestHeaders.Add("X-Secret", $"Secret {secretKey}");
        }

        public async Task<string> GetModelIdAsync()
        {
            var response = await httpClient.GetAsync($"{apiUrl}key/api/v1/models");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var json = JArray.Parse(content);
            return json[0]["id"].ToString();
        }

        public async Task<string> GenerateImageAsync(string prompt, string modelId, int images = 1, int width = 1024, int height = 1024)
        {
            var parameters = new
            {
                type = "GENERATE",
                numImages = images,
                width = width,
                height = height,
                generateParams = new
                {
                    query = prompt
                }
            };

            var formData = new MultipartFormDataContent();

            formData.Add(new StringContent(modelId), "model_id");
            formData.Add(
                new StringContent(JsonConvert.SerializeObject(parameters), System.Text.Encoding.UTF8, "application/json"),
                "params",
                "params.json"
            );

            var response = await httpClient.PostAsync($"{apiUrl}key/api/v1/text2image/run", formData);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            var json = JObject.Parse(responseContent);
            return json["uuid"].ToString();
        }

        public async Task<List<string>> CheckGenerationStatusAsync(string requestId, int attempts = 10, int delayInSeconds = 10)
        {
            var results = new List<string>();

            while (attempts > 0)
            {
                var response = await httpClient.GetAsync($"{apiUrl}key/api/v1/text2image/status/{requestId}");
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var json = JObject.Parse(content);

                if (json["status"].ToString() == "DONE")
                {
                    var images = json["images"].ToObject<List<string>>();
                    results.AddRange(images);
                    break;
                }

                attempts--;
                await Task.Delay(TimeSpan.FromSeconds(delayInSeconds));
            }

            return results;
        }
    }
}
