using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BlobStorageAdapter.DataAccess;
using BlobStorageAdapter.Models;
using Microsoft.AspNetCore.Mvc;

namespace BlobStorageAdapter.Controllers
{
    [Route("api/Files/Text")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private const string FileExtension = ".txt";

        private readonly Func<IGetFilesCommand> _getFilesCommandFactory;
        private readonly Func<ISaveFileCommand> _saveFileCommandFactory;

        public FileController
        (
            Func<IGetFilesCommand> getFilesCommandFactory,
            Func<ISaveFileCommand> saveFileCommandFactory
        )
        {
            _getFilesCommandFactory = getFilesCommandFactory;
            _saveFileCommandFactory = saveFileCommandFactory;
        }

        [HttpPost]
        public async Task<File> AddFile([FromBody] string content)
        {
            var file = new File(content, Guid.NewGuid());

            await _saveFileCommandFactory().Save($"{file.Id}{FileExtension}", file.Content);

            return file;
        }

        [HttpGet]
        public async IAsyncEnumerable<File> GetFiles()
        {
            static Guid FileNameToGuid(string name) => Guid.Parse(name.Replace(FileExtension, string.Empty));

            await foreach (var (name, content) in _getFilesCommandFactory().GetFiles())
            {
                yield return new File(content, FileNameToGuid(name));
            }
        }
    }
}