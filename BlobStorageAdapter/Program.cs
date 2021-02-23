using System;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using BlobStorageAdapter.DataAccess;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BlobStorageAdapter
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var application = CreateHostBuilder(args).Build();

            await BlobContainer.EnsureCreated(application.Services.GetRequiredService<BlobServiceClient>());

            await application.RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
    }
}