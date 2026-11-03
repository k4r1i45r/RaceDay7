namespace RaceDay7.Services
{
    public class BlobStorageService : IBlobStorageService
    {

        private const long MaxBytes = 2 * 1024 * 1024; // 2 MB

        private static readonly Dictionary<string, string> AllowedContentTypes = new(StringComparer.OrdinalIgnoreCase)
        {
            [".jpg"] = "image/jpeg",
            [".png"] = "image/png",
            [".gif"] = "image/gif",
            [".webp"] = "image/webp"
        };

        private readonly BlobServiceClient _container;

        public BlobStorageService(IConfiguration config)
        {
            var connectionString = config["AzureStorage:ConnectionString"];
            var containerName = config["AzureStorage:ContainerName"] ?? "event-banner";

            if (string.IsNullOrWhiteSpace(connectionString))
                throw new InvalidOperationException("Azure Storage connection string is not configured.");

            try
            {
                _container = new BlobContainerClient(connectionString.Trim(), containerName);
            }
            catch (FormatException)
            {
                var parts = connectionString.Split(';', StringSplitOptions.RemoveEmptyEntries).Select(p => p.Split('=')[0].Trim());
                throw new InvalidOperationException(
                   $"Connection string is invalid. Length={connectionString.Length}; " +
                   $"starts with '{connectionString[..Math.Min(25, connectionString.Length)]}'; " +
                   $"keys found: {string.Join(",", parts)}");
            }
        }

        public string? Validate(IFormFile file)
        {
            if (file.Length == 0) return "The selected file is empty.";
            if (file.Length > MaxBytes) return "The banner must be 2 MB or smaller.";

            var ext = Path.GetExtension(file.FileName);

            if (!AllowedContentTypes.TryGetValue(ext, out var expectedType) ||
                !string.Equals(file.ContentType, expectedType, StringComparison.OrdinalIgnoreCase))
                     return "Only JPG, PNG or WebP images are allowed.";

            //Check the file's real signature, not just its name/content-type
          using var stream = file.OpenReadStream();
            Span<byte> h = stackalloc byte[12];
        }
        }
    }


