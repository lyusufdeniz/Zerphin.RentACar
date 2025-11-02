using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;
using Zerphin.RentACar.Domain.Contracts.Services;
using Zerphin.RentACar.Domain.Options;


namespace Zerphin.RentACar.Infrastructure.Services;

public class CloudinaryService : ICloudinaryService
{
    private readonly Cloudinary _cloudinary;

    public CloudinaryService(IOptions<CloudinaryOptions> cloudinaryOptions)
    {
        var options = cloudinaryOptions.Value;
        var account = new Account(options.CloudName, options.ApiKey, options.ApiSecret);
        _cloudinary = new Cloudinary(account);
    }

    public async Task<string?> UploadImageAsync(string base64String, string folder, string? fileName = null)
    {
        if (string.IsNullOrWhiteSpace(base64String))
            return null;

        try
        {
            var base64Data = base64String.Contains(",") 
                ? base64String.Split(',')[1] 
                : base64String;

            var imageBytes = Convert.FromBase64String(base64Data);

            var publicId = string.IsNullOrWhiteSpace(fileName)
                ? $"{folder}/{Guid.NewGuid()}"
                : $"{folder}/{fileName}";

            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(Guid.NewGuid().ToString(), new MemoryStream(imageBytes)),
                PublicId = publicId.Replace(Path.GetExtension(publicId), ""),
                Folder = folder
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);

            return uploadResult.StatusCode == System.Net.HttpStatusCode.OK 
                ? uploadResult.SecureUrl.ToString() 
                : null;
        }
        catch
        {
            return null;
        }
    }

    public async Task<bool> DeleteImageAsync(string imageUrl)
    {
        if (string.IsNullOrWhiteSpace(imageUrl))
            return false;

        try
        {
            if (!Uri.TryCreate(imageUrl, UriKind.Absolute, out var uri))
                return false;

            var segments = uri.Segments.ToList();
            if (segments.Count < 2)
                return false;

            var path = string.Join("", segments.Skip(1));
            var publicId = path.Split('.').First().Trim('/');

            var deletionResult = await _cloudinary.DestroyAsync(new DeletionParams(publicId));

            return deletionResult.Result == "ok";
        }
        catch
        {
            return false;
        }
    }
}


