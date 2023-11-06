using contactPro2.Services.Interfaces;

namespace contactPro2.Services
{
    public class ImageService : IImageService
    {
        private readonly string? _defaultImage = "/img/DefaultContactImage.png";  
        public string? ConvertByteArrayToFile(byte[]? fileData, string? extension)
        {
            try
            {
                if (fileData == null ) 
                {
                    // show default
                    return _defaultImage;
                }
                
                string? imageBase64Data = Convert.ToBase64String(fileData);
                imageBase64Data = string.Format($"data: {extension}; base64, {imageBase64Data}");

                return imageBase64Data;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<byte[]> ConvertFileToByteArrayAsync(IFormFile? file)
        {
            try
            {
                if (file != null)
                { 
                using MemoryStream memoryStream = new MemoryStream();
                await file.CopyToAsync(memoryStream);
                byte[] byteFile = memoryStream.ToArray();
                memoryStream.Close();

                return byteFile;
                }

                return null!;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
