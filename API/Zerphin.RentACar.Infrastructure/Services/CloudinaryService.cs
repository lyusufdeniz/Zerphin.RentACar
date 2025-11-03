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
        
        // Validate Cloudinary configuration
        if (string.IsNullOrWhiteSpace(options.CloudName) || 
            string.IsNullOrWhiteSpace(options.ApiKey) || 
            string.IsNullOrWhiteSpace(options.ApiSecret))
        {
            throw new InvalidOperationException(
                "Cloudinary configuration is missing. Please ensure Cloudinary:CloudName, Cloudinary:ApiKey, and Cloudinary:ApiSecret are set in appsettings.json");
        }

        var account = new Account(options.CloudName, options.ApiKey, options.ApiSecret);
        _cloudinary = new Cloudinary(account);
    }

    public async Task<string?> UploadImageAsync(string base64String, string folder, string? fileName = null)
    {
        if (string.IsNullOrWhiteSpace(base64String))
        {
            return null;
        }

        if (string.IsNullOrWhiteSpace(folder))
        {
            return null;
        }

        MemoryStream? imageStream = null;
        
        try
        {
            // Parse base64 string
            string base64Data;
            if (base64String.Contains(","))
            {
                var parts = base64String.Split(',', 2);
                if (parts.Length != 2)
                {
                    return null;
                }
                base64Data = parts[1];
            }
            else
            {
                base64Data = base64String;
            }

            // Validate base64 string
            if (string.IsNullOrWhiteSpace(base64Data))
            {
                return null;
            }

            // Convert base64 to bytes
            byte[] imageBytes;
            try
            {
                imageBytes = Convert.FromBase64String(base64Data);
                if (imageBytes.Length == 0)
                {
                    return null;
                }
            }
            catch (FormatException)
            {
                return null;
            }

            // Create memory stream (will be disposed properly)
            imageStream = new MemoryStream(imageBytes);

            // Generate public ID
            var publicId = string.IsNullOrWhiteSpace(fileName)
                ? $"{folder}/{Guid.NewGuid()}"
                : $"{folder}/{Path.GetFileNameWithoutExtension(fileName)}";

            // Sanitize folder and publicId
            folder = folder.Trim('/').Replace(" ", "_");
            publicId = publicId.Replace(" ", "_");

            // Prepare upload parameters
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(Guid.NewGuid().ToString(), imageStream),
                PublicId = publicId,
                Folder = folder,
                Overwrite = true
            };

            // Upload to Cloudinary
            var uploadResult = await _cloudinary.UploadAsync(uploadParams);

            // Check result
            if (uploadResult.StatusCode == System.Net.HttpStatusCode.OK && uploadResult.SecureUrl != null)
            {
                var imageUrl = uploadResult.SecureUrl.ToString();
                return imageUrl;
            }
            else
            {
                return null;
            }
        }
        catch (Exception)
        {
            return null;
        }
        finally
        {
            // Ensure stream is disposed
            imageStream?.Dispose();
        }
    }

    public async Task<bool> DeleteImageAsync(string imageUrl)
    {
        if (string.IsNullOrWhiteSpace(imageUrl))
        {
            return false;
        }

        try
        {
            if (!Uri.TryCreate(imageUrl, UriKind.Absolute, out var uri))
            {
                return false;
            }

            var segments = uri.Segments.ToList();
            if (segments.Count < 2)
            {
                return false;
            }

            var path = string.Join("", segments.Skip(1));
            var publicId = path.Split('.').First().Trim('/');

            if (string.IsNullOrWhiteSpace(publicId))
            {
                return false;
            }

            var deletionResult = await _cloudinary.DestroyAsync(new DeletionParams(publicId));

            if (deletionResult.Result == "ok")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        catch (Exception)
        {
            return false;
        }
    }
}



