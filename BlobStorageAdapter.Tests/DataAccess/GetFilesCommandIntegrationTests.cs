using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlobStorageAdapter.DataAccess;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BlobStorageAdapter.Tests.DataAccess
{
    [TestClass]
    [TestCategory("Integration")]
    public class GetFilesCommandIntegrationTests : BlobStorageIntegrationTest
    {
        private IGetFilesCommand _command;

        [TestInitialize]
        public void TestInitialize()
        {
            _command = new GetFilesCommand(ContainerClient);
        }

        [TestMethod]
        public async Task GetFiles_FilesExistInStorage_ReturnsStringContentOfAllFiles()
        {
            var expectedFiles = Enumerable.Repeat<Func<Task<(string name, string content)>>>
                (
                    async () =>
                    {
                        var (name, content) = ($"{Guid.NewGuid()}.txt", Guid.NewGuid().ToString());

                        await ContainerClient.GetBlobClient(name)
                            .UploadAsync(new MemoryStream(Encoding.Default.GetBytes(content)));

                        return (name, content);
                    },
                    5
                )
                .Select(f => f().Result)
                .ToArray();

            var files = _command.GetFiles();

            var actualFiles = new List<(string name, string content)>();
            await foreach(var file in files) actualFiles.Add(file);
            Assert.IsTrue(expectedFiles.OrderBy(f => f.name).SequenceEqual(actualFiles.OrderBy(f => f.name)));
        }

        [TestMethod]
        public async Task GetFiles_NoFilesInStorage_ReturnsEmptyEnumerable()
        {
            var files = _command.GetFiles();

            var fileCount = 0;
            await foreach (var _ in files) fileCount++;
            Assert.AreEqual(0, fileCount);
        }
    }
}