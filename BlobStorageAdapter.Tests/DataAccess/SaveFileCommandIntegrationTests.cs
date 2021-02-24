using System;
using System.IO;
using System.Threading.Tasks;
using BlobStorageAdapter.DataAccess;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MemoryStream = System.IO.MemoryStream;

namespace BlobStorageAdapter.Tests.DataAccess
{
    [TestClass]
    [TestCategory("Integration")]
    public class SaveFileCommandIntegrationTests : BlobStorageIntegrationTest
    {
        private ISaveFileCommand _command;

        [TestInitialize]
        public void TestInitialize()
        {
            _command = new SaveFileCommand(ContainerClient);
        }

        [TestMethod]
        public async Task Save_FileNameUniqueInBlob_UploadsContent()
        {
            var (name, content) = (Guid.NewGuid().ToString(), Guid.NewGuid().ToString());

            await _command.Save(name, content);

            using var reader = new StreamReader((await ContainerClient.GetBlobClient(name).DownloadAsync()).Value.Content);
            var actualContent = await reader.ReadToEndAsync();
            Assert.AreEqual(content, actualContent);
        }

        [TestMethod]
        public async Task Save_DuplicateFileName_ThrowsException()
        {
            var name = Guid.NewGuid().ToString();
            await ContainerClient.GetBlobClient(name).UploadAsync(new MemoryStream());

            var exception = await Assert.ThrowsExceptionAsync<Azure.RequestFailedException>(() =>  _command.Save(name, string.Empty));

            StringAssert.Contains(exception.Message, "already exists");
        }
    }
}