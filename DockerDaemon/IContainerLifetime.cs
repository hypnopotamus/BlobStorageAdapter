using System;

namespace DockerDaemon
{
    public interface IContainerLifetime : IAsyncDisposable, IDisposable
    {
        
    }
}