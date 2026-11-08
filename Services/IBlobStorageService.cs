using Microsoft.AspNetCore.Http;
using System.Threading;
using System.Threading.Tasks;

namespace RaceDay7.Services
{
    public record BlobUploadResult(string BlobName, string Url);

    public interface IBlobStorageService
    {
        string? Validate(IFormFile file);
        Task<BlobUploadResult> UploadBannerAsync(IFormFile file, CancellationToken ct = default);
        Task DeleteAsync(string? blobName, CancellationToken ct = default);
    }
}