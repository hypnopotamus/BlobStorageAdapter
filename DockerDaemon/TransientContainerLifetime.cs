using System;
using System.Threading.Tasks;
using Docker.DotNet;
using Docker.DotNet.Models;

namespace DockerDaemon
{
    public class TransientContainerLifetime : IContainerLifetime
    {
        private readonly string _containerId;

        internal TransientContainerLifetime(string containerId)
        {
            _containerId = containerId;
        }

        public async ValueTask DisposeAsync()
        {
            using var configuration = new DockerClientConfiguration();
            using var client = configuration.CreateClient();

            await client.Containers.RemoveContainerAsync
            (
                _containerId,
                new ContainerRemoveParameters {Force = true}
            );

            GC.SuppressFinalize(this);
        }

        public void Dispose() => DisposeAsync().AsTask().Wait();

        ~TransientContainerLifetime()
        {
            Dispose();
        }
    }
}