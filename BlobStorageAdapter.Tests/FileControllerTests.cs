using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlobStorageAdapter.Controllers;
using BlobStorageAdapter.Models;
using BlobStorageAdapter.TestDoubles.DataAccess;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BlobStorageAdapter.Tests
{
    [TestClass]
    public class FileControllerTests
    {
        private readonly FileController _controller;

        private readonly FakeGetFilesCommand _getFilesCommand = new();
        private readonly FakeSaveFilesCommand _saveFilesCommand = new();

        public FileControllerTests()
        {
            _controller = new FileController(() => _getFilesCommand, () => _saveFilesCommand);
        }

        [TestMethod]
        public async Task AddFile_AllCases_SavesContentInTxt()
        {
            var content = Guid.NewGuid().ToString();

            await _controller.AddFile(content);

            Assert.AreEqual(1, _saveFilesCommand.Files.Count(f => f.content == content));
            var savedFile = _saveFilesCommand.Files.Single(f => f.content == content);
            StringAssert.EndsWith(savedFile.name, ".txt");
        }

        [TestMethod]
        public async Task AddFile_AllCases_ReturnsFileModel()
        {
            var content = Guid.NewGuid().ToString();

            var file = await _controller.AddFile(content);

            Assert.AreEqual(content, file.Content);
            Assert.AreEqual(1, _saveFilesCommand.Files.Count(f => f.name.Contains(file.Id.ToString())));
        }

        [TestMethod]
        public async Task GetFiles_FilesExist_ReturnsFileModelWithoutTxtExtensionName()
        {
            var files = Enumerable.Repeat<Func<File>>(() => new File(Guid.NewGuid().ToString(), Guid.NewGuid()), 5)
                .Select(f => f())
                .ToArray();
            foreach(var (content, id) in files) _getFilesCommand.Files.Add(($"{id}.txt", content));

            var retrievedFiles = _controller.GetFiles();

            var actualFiles = new List<File>();
            await foreach (var file in retrievedFiles)
            {
                actualFiles.Add(file);
            }
            Assert.IsTrue(files.OrderBy(f => f.Id).SequenceEqual(actualFiles.OrderBy(f => f.Id)));
        }
    }
}
