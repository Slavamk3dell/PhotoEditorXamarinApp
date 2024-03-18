using System.IO;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace TestXamarinApp
{
    public interface IImageService
    {
        Task<Stream> GetImageStreamAsync();
        void SaveImageToGallery(byte[] imageBytes);
    }
}