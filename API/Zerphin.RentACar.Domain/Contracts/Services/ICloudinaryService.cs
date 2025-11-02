namespace Zerphin.RentACar.Domain.Contracts.Services;

public interface ICloudinaryService
{
    Task<string?> UploadImageAsync(string base64String, string folder, string? fileName = null);
    Task<bool> DeleteImageAsync(string imageUrl);
}

