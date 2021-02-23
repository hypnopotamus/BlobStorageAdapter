using System.Threading.Tasks;
using Azure.Storage.Blobs;

namespace BlobStorageAdapter.DataAccess
{
    public static class BlobContainer
    {
        private const string ContainerName = "SomeNeatFiles";
        public static async Task<BlobContainerClient> EnsureCreated(BlobServiceClient client)
        {
            if (!await ContainerExists(client))
            {
                await client.CreateBlobContainerAsync(ContainerName);
            }

            return client.GetBlobContainerClient(ContainerName);
        }

        private static async Task<bool> ContainerExists(BlobServiceClient client)
        {
            var containers = client.GetBlobContainersAsync(prefix: ContainerName);
            await foreach (var page in containers)
            {
                if (page.Name == ContainerName) return true;
            }

            return false;
        }
    }
}