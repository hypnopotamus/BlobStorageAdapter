using System.Collections.Generic;
using System.Threading.Tasks;
using BlobStorageAdapter.DataAccess;

namespace BlobStorageAdapter.TestDoubles.DataAccess
{
    public class FakeSaveFilesCommand : ISaveFileCommand
    {
        public IList<(string name, string content)> Files { get; } = new List<(string name, string content)>();

        public Task Save(string name, string content)
        {
            Files.Add((name, content));

            return Task.CompletedTask;
        }
    }
}