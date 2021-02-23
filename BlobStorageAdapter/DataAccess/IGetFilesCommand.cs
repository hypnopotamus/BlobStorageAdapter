using System.Collections.Generic;

namespace BlobStorageAdapter.DataAccess
{
    public interface IGetFilesCommand
    {
        IAsyncEnumerable<(string name, string content)> GetFiles();
    }
}