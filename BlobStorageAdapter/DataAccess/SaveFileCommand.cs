using System.IO;
using System.Text;
using System.Threading.Tasks;
using Azure.Storage.Blobs;

namespace BlobStorageAdapter.DataAccess
{
    public class SaveFileCommand : ISaveFileCommand
    {
        private readonly BlobContainerClient _blobClient;

        public SaveFileCommand(BlobContainerClient blobClient)
        {
            _blobClient = blobClient;
        }

        public Task Save(string name, string content)
        {
            using var contentStream = new MemoryStream(Encoding.Default.GetBytes(content));

            return _blobClient.GetBlobClient(name).UploadAsync(contentStream);
        }
    }
}