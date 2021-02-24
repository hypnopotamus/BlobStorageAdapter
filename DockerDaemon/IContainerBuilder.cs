using System.Threading.Tasks;

namespace DockerDaemon
{
    public interface IContainerBuilder
    {
        IContainerBuilder ExposingPort(int hostPort, int containerPort);
        IContainerBuilder WithEnvironmentVariable(string name, string value);
        Task<IContainerLifetime> Start();
    }
}