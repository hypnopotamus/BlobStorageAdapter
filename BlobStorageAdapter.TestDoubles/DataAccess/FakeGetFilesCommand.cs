using System.Collections.Generic;
using System.Threading.Tasks;
using BlobStorageAdapter.DataAccess;

namespace BlobStorageAdapter.TestDoubles.DataAccess
{
    public class FakeGetFilesCommand : IGetFilesCommand
    {
        public IList<(string name, string content)> Files { get; } = new List<(string name, string content)>();

        public async IAsyncEnumerable<(string name, string content)> GetFiles()
        {
            foreach (var (name, content) in Files) yield return (name, content);

            await Task.CompletedTask;
        }
    }
}