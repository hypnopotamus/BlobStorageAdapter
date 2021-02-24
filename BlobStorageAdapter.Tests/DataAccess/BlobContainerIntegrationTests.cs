using System;
using System.Threading.Tasks;
using BlobStorageAdapter.DataAccess;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BlobStorageAdapter.Tests.DataAccess
{
    [TestClass]
    [TestCategory("Integration")]
    public class BlobContainerIntegrationTests : BlobStorageIntegrationTest
    {
        [TestMethod]
        public async Task EnsureExists_ContainerExists_DoesNotCreateANewContainer()
        {
            var containerName = Guid.NewGuid().ToString();
            await BlobStorageClient.CreateBlobContainerAsync(containerName);
            var initialContainerCount = await CountContainers(containerName);

            await BlobContainer.EnsureCreated(BlobStorageClient, containerName);

            var resultingContainerCount = await CountContainers(containerName);
            Assert.AreEqual(initialContainerCount, resultingContainerCount);
        }

        [TestMethod]
        public async Task EnsureExists_ContainerExists_ReturnsAClientToThatContainer()
        {
            var containerName = Guid.NewGuid().ToString();
            await BlobStorageClient.CreateBlobContainerAsync(containerName);

            var client = await BlobContainer.EnsureCreated(BlobStorageClient, containerName);

            Assert.AreEqual(containerName, client.Name);
        }

        [TestMethod]
        public async Task EnsureExists_ContainerNeedsToBeCreated_CreatesANewContainer()
        {
            var containerName = Guid.NewGuid().ToString();
            var initialContainerCount = await CountContainers(containerName);

            await BlobContainer.EnsureCreated(BlobStorageClient, containerName);

            var resultingContainerCount = await CountContainers(containerName);
            Assert.AreEqual(initialContainerCount + 1, resultingContainerCount);
        }

        [TestMethod]
        public async Task EnsureExists_ContainerNeedsToBeCreated_ReturnsAClientToTheNewContainer()
        {
            var containerName = Guid.NewGuid().ToString();

            var client = await BlobContainer.EnsureCreated(BlobStorageClient, containerName);

            Assert.AreEqual(containerName, client.Name);
        }

        private async Task<int> CountContainers(string containerName)
        {
            var containerCount = 0;
            await foreach (var container in BlobStorageClient.GetBlobContainersAsync())
            {
                if (container.Name == containerName) containerCount++;
            }

            return containerCount;
        }
    }
}