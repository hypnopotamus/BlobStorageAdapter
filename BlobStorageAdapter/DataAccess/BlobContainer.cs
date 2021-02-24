using System.Threading.Tasks;
using Azure.Storage.Blobs;

namespace BlobStorageAdapter.DataAccess
{
    public static class BlobContainer
    {
        private const string ContainerName = "SomeNeatFiles";

        public static async Task<BlobContainerClient> EnsureCreated(BlobServiceClient client, string containerName = ContainerName)
        {
            if (!await ContainerExists(client, containerName))
            {
                await client.CreateBlobContainerAsync(containerName);
            }

            return client.GetBlobContainerClient(containerName);
        }

        private static async Task<bool> ContainerExists(BlobServiceClient client, string containerName)
        {
            var containers = client.GetBlobContainersAsync(prefix: containerName);
            await foreach (var page in containers)
            {
                if (page.Name == containerName) return true;
            }

            return false;
        }
    }
}