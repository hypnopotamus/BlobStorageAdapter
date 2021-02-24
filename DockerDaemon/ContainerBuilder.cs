using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using Docker.DotNet;
using Docker.DotNet.Models;

namespace DockerDaemon
{
    public class ContainerBuilder : IContainerBuilder
    {
        private readonly string _containerName;
        private readonly IDictionary<int, int> _exposedPorts = new Dictionary<int, int>();
        private readonly IDictionary<string, string> _environmentVariables = new Dictionary<string, string>();

        public static IContainerBuilder BuildTransientContainer(string containerName) =>
            new ContainerBuilder(containerName);


        private ContainerBuilder(string containerName)
        {
            if (string.IsNullOrEmpty(containerName)) throw new InvalidOperationException();

            _containerName = containerName;
        }

        public IContainerBuilder ExposingPort(int hostPort, int containerPort)
        {
            _exposedPorts[hostPort] = containerPort;

            return this;
        }

        public IContainerBuilder WithEnvironmentVariable(string name, string value)
        {
            _environmentVariables[name] = value;

            return this;
        }

        public async Task<IContainerLifetime> Start()
        {
            using var configuration = new DockerClientConfiguration();
            using var client = configuration.CreateClient();

            await client.Images.CreateImageAsync(
                new ImagesCreateParameters {FromImage = _containerName, Tag = "latest"}, null,
                new Progress<JSONMessage>());
            var container = await client.Containers.CreateContainerAsync
            (
                new CreateContainerParameters
                {
                    Image = _containerName,
                    Env = _environmentVariables.Select(ev => $"{ev.Key}={ev.Value}").ToList(),
                    ExposedPorts = _exposedPorts.Select(p => $"{p.Value}/tcp").ToDictionary(p => p, _ => new EmptyStruct()),
                    HostConfig = new HostConfig
                    {
                        PortBindings = _exposedPorts.ToDictionary
                        (
                            p => $"{p.Value}/tcp",
                            p => (IList<PortBinding>) new List<PortBinding>
                            {
                                new() {HostPort = p.Key.ToString()}
                            }
                        )
                    }
                }
            );
            await client.Containers.StartContainerAsync(container.ID, new ContainerStartParameters());

            return new TransientContainerLifetime(container.ID);
        }
    }
}