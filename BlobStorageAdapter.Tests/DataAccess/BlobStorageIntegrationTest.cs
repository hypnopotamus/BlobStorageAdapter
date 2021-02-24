using System;
using Azure.Storage.Blobs;
using DockerDaemon;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BlobStorageAdapter.Tests.DataAccess
{
    [TestClass]
    public abstract class BlobStorageIntegrationTest
    {
        protected BlobServiceClient BlobStorageClient { get; private set; }
        protected BlobContainerClient ContainerClient { get; private set; }

        private const string DefaultConnectionString = "DefaultEndpointsProtocol=http;AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;BlobEndpoint=http://127.0.0.1:10000/devstoreaccount1;";
        private readonly string _containerName = Guid.NewGuid().ToString();
        private static IContainerLifetime? _blobStorageContainer;

        [AssemblyInitialize]
        public static void StartContainer(TestContext _)
        {
            _blobStorageContainer = ContainerBuilder.BuildTransientContainer("mcr.microsoft.com/azure-storage/azurite")
                .ExposingPort(10000, 10000)
                .Start()
                .Result;
        }

        [TestInitialize]
        public void BlobStorageInitialize()
        {
            BlobStorageClient = new BlobServiceClient(DefaultConnectionString);

            var container = BlobStorageClient.CreateBlobContainer(_containerName);
            ContainerClient = BlobStorageClient.GetBlobContainerClient(container.Value.Name);
        }

        [AssemblyCleanup]
        public static void DisposeContainer()
        {
            _blobStorageContainer?.Dispose();
        }
    }
}