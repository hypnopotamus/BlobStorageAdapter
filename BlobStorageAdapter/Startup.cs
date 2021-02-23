using System;
using Azure.Storage.Blobs;
using BlobStorageAdapter.DataAccess;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace BlobStorageAdapter
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAzureClients(c => c.AddBlobServiceClient(Configuration.GetConnectionString("BlobStorage")));
            services.AddTransient<BlobContainerClient>(p => BlobContainer.EnsureCreated(p.GetRequiredService<BlobServiceClient>()).Result);

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "BlobStorageAdapter", Version = "v1"});
            });

            services.AddTransient<IGetFilesCommand, GetFilesCommand>();
            services.AddTransient<Func<IGetFilesCommand>>(p => p.GetRequiredService<IGetFilesCommand>);
            services.AddTransient<ISaveFileCommand, SaveFileCommand>();
            services.AddTransient<Func<ISaveFileCommand>>(p => p.GetRequiredService<ISaveFileCommand>);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "BlobStorageAdapter v1"));
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}