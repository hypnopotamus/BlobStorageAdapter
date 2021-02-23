using System.Collections.Generic;
using System.IO;
using Azure.Storage.Blobs;

namespace BlobStorageAdapter.DataAccess
{
    public class GetFilesCommand : IGetFilesCommand
    {
        private readonly BlobContainerClient _blobClient;

        public GetFilesCommand(BlobContainerClient blobClient)
        {
            _blobClient = blobClient;
        }

        public async IAsyncEnumerable<(string name, string content)> GetFiles()
        {
            await foreach (var file in _blobClient.GetBlobsAsync())
            {
                var fileClient = _blobClient.GetBlobClient(file.Name);
                using var fileContent = (await fileClient.DownloadAsync()).Value;
                await using var contentStream = new MemoryStream();

                await fileContent.Content.CopyToAsync(contentStream);
                contentStream.Position = 0;

                using var reader = new StreamReader(contentStream);
                yield return (file.Name, await reader.ReadToEndAsync());
            }
        }
    }
}