using hookset_server.models;

namespace hookset_server
{

    public interface IBlobStorageService {
        public Task<string> uploadBlob(string fileName, string filePath);
    }

    public class BlobStorageService: IBlobStorageService
    {
        private readonly BlobServiceClient _blobServiceClient;
        private BlobContainerClient _containerClient;

        public BlobStorageService(BlobServiceClient blobServiceClient) { 
            _blobServiceClient = blobServiceClient;
            _containerClient = _blobServiceClient.GetBlobContainerClient("hooket-dev-images");
        }

        public async Task<string> uploadBlob(string fileName, string filePath)
        {
            var blobClient = _containerClient.GetBlobClient(fileName);

            var status = await blobClient.UploadAsync(filePath);

            return blobClient.Uri.AbsoluteUri;
        }
    }
}
