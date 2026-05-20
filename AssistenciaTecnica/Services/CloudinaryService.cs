using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace AssistenciaTecnica.Services
{
    public class CloudinaryService
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryService(IConfiguration configuration)
        {
            var account = new Account(
                configuration["Cloudinary:CloudName"],
                configuration["Cloudinary:ApiKey"],
                configuration["Cloudinary:ApiSecret"]
            );

            _cloudinary = new Cloudinary(account);
        }

        public async Task<string> UploadImagemAsync(IFormFile? arquivo, string pasta)
        {
            if (arquivo == null || arquivo.Length == 0)
                return "";

            await using var stream = arquivo.OpenReadStream();

            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(arquivo.FileName, stream),
                Folder = pasta,
                UseFilename = false,
                UniqueFilename = true,
                Overwrite = false
            };

            var resultado = await _cloudinary.UploadAsync(uploadParams);

            return resultado.SecureUrl?.ToString() ?? "";
        }
    }
}