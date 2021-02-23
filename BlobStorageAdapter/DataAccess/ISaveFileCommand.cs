using System.Threading.Tasks;

namespace BlobStorageAdapter.DataAccess
{
    public interface ISaveFileCommand
    {
        Task Save(string name, string content);
    }
}